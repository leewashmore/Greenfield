using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Sql;
using GreenField.IssuerShares.Core.Persisting;
using System.Data.SqlClient;
using Aims.Core;
using DataLoader.Core;

namespace GreenField.IssuerShares.Server
{
    public class Facade : IFacade
    {
        private Serializer serializer;
        private Core.ModelManager modelManager;
        private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory dataManagerFactory;
        private Aims.Data.Server.Serializer commonSerializer;
        private Deserializer deserializer;

        public RepositoryManager RepositoryManager { get; private set; }

        public Facade(FacadeSettings settings) : this(settings.ModelManager, settings.CommonSerializer, settings.Serializer, settings.Deserializer, settings.ConnectionFactory, settings.DataManagerFactory, settings.RepositoryManager)
        { 
            
        }
        
        public Facade(
            Core.ModelManager modelManager,
            Aims.Data.Server.Serializer commonSerializer,
            Serializer serializer,
            Deserializer deserializer,
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory dataManagerFactory,
            RepositoryManager repositoryManager
        )
        {
            this.modelManager = modelManager;
            this.serializer = serializer;
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.RepositoryManager = repositoryManager;
            this.commonSerializer = commonSerializer;
            this.deserializer = deserializer;
        }

        public RootModel GetRootModel(String securityShortName)
        {
            using (var ondemandManager = CreateOnDemandDataManager())
            {
                var repository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var security = repository.FindSecurityByShortName(securityShortName);
                
                    var model = this.modelManager.GetRootModel(security.IssuerId);
                    var serializedModel = this.serializer.SerializeRoot(model);
                    return serializedModel;
                
            }
        }

        

        public IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(String pattern, Int32 atMost, String securityShortName)
        {
            using (var ondemandManager = CreateOnDemandDataManager())
            {
                var repository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var security = repository.FindSecurityByShortName(securityShortName);
                
                var securities = repository.FindSomeUsingPattern(pattern, atMost, x => x.IssuerId == security.IssuerId && (x.SecurityType == "EQUITY" || x.SecurityType == "ADR" || x.SecurityType == "GDR"));
                var result = this.commonSerializer.SerializeSecurities(securities);
                return result;
                
            }
        }


        public IEnumerable<GreenField.IssuerShares.Server.IssuerSecurityShareRecordModel> GetIssuerSharesBySecurityShortName(String securityShortName)
        {
            using (var ondemandManager = CreateOnDemandDataManager())
            {
                var repository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var security = repository.FindSecurityByShortName(securityShortName);
                
                    var securities = repository.FindByIssuer(security.IssuerId).Where(x => x.SecurityType == "EQUITY" || x.SecurityType == "ADR" || x.SecurityType == "GDR");

                    var loader = CreateIssuerSharesLoader();

                    var records = loader.GetShareRecords(security.IssuerId, securities.Select(x => new DataLoader.Core.BackendProd.GF_SECURITY_BASEVIEW { ASEC_SEC_SHORT_NAME = x.ShortName, TICKER = x.Ticker, SECURITY_ID = Convert.ToInt32(x.Id), ISSUER_ID = x.IssuerId, TRADING_CURRENCY = "FAKE CURRENCY" }));
                    var result = this.serializer.SerializeShareRecords(records);

                    return result;
                
            }
        }

        private IssueSharesLoaderExtension CreateIssuerSharesLoader()
        {
            var settings = new IssuerSharesSettings
            {
                RecordsPerBulk = Settings.RecordsPerBulk,
                RecordsPerChunk = Settings.RecordsPerChunk,
                MaxNumberOfDaysToStopExtrapolatingAfter = Settings.MaxNumberOfDaysToStopExtrapolatingAfter,
                NumberOfDaysAgoToStartLoadingFrom = Settings.NumberOfDaysAgoToStartLoadingFrom != 0 ? Settings.NumberOfDaysAgoToStartLoadingFrom : null, // can be also: null
                NumberOfDaysBeforeLoadingDateToBeGuaranteeFromHittingGap = Settings.NumberOfDaysBeforeLoadingDateToBeGuaranteeFromHittingGap,
                WebServiceUri = new Uri(Settings.ODataServiceUri),
                ConnectionStringToAims = Settings.ConnectionToAims,
                ConnectionStringToAimsEntities = Settings.ConnectionToAimsEntities
            };

            var dumper = new TraceDumper();

            var targetPuller = new TargetDataPuller(settings.ConnectionStringToAimsEntities);
            var sourcePuller = new SourceDataPuller(settings.WebServiceUri, settings.RecordsPerChunk, dumper);
            var monitor = new DataLoader.Core.Monitor(dumper);
            var pusher = new IssueSharesPusherExtension(monitor, settings.ConnectionStringToAims, settings.RecordsPerBulk);
            var filler = new GapsFiller<IssuerShareRecord>(settings.MaxNumberOfDaysToStopExtrapolatingAfter, new IssuerSharesGapFillerAdapter());
            var transformer = new IssuerSharesTransformer(monitor);
            var eraser = new IssuerSharesEraser(settings.ConnectionStringToAims);
            var loader = new IssueSharesLoaderExtension(
                settings.NumberOfDaysBeforeLoadingDateToBeGuaranteeFromHittingGap,
                monitor,
                targetPuller,
                sourcePuller,
                transformer,
                filler,
                pusher,
                eraser
            );

            return loader;
        }

        internal OnDemandDataManager CreateOnDemandDataManager()
        {
            return new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory);
        }

        internal OnDemandDataManager CreateOnDemandDataManager(SqlConnection connection)
        {
            return new OnDemandDataManager(connection, this.dataManagerFactory);
        }




        public RootModel UpdateIssueSharesComposition(RootModel model)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var items = model.Items.ConvertAll<GreenField.IssuerShares.Core.ItemModel>(x => new GreenField.IssuerShares.Core.ItemModel(deserializer.DeserializeSecurity(x.Security), x.Preferred));
                var tran = connection.BeginTransaction();
                var dataManager = this.dataManagerFactory.CreateDataManager(connection, tran);
                var insertRecordsCount = dataManager.UpdateIssuerSharesComposition(model.Issuer.Id, items);
                tran.Commit();
                //Calls to the Data-Loader Application
                var loader = CreateIssuerSharesLoader();
                loader.RunForSpecificIssuers(new[] { model.Issuer.Id }, null, Settings.ConnectionToAims); //Added Connection String for Data-Loader application Issuer Shares
                loader.Pusher.ExecuteGetDataProcedure(model.Issuer.Id);
                return model;
            }
        }
    }
}
