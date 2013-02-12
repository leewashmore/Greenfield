using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingBpt;
using Newtonsoft.Json;
using System.IO;
using TopDown.Core._Testing;
using System.Diagnostics;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPst;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.Gadgets.PortfolioPicker;
using TopDown.Core.Gadgets.BasketPicker;
using TopDown.Core.ManagingPortfolios;
using Aims.Expressions;
using TopDown.Core.ManagingCalculations;
using Aims.Core;
using Aims.Core.Sql;
using System.Configuration;

namespace TopDown.Core
{
    public class Facade
    {
        private IDataManagerFactory dataManagerFactory;
        private IUsersDataManagerFactory dataUsersManagerFactory;
        private ISqlConnectionFactory connectionFactory;
        private ManagingCalculations.Hopper hopper;
        private ManagingComments.CommentManager commentManager;
        private ISqlConnectionFactory usersConnectionFactory;

        public Facade(
            ISqlConnectionFactory connectionFactory,
            ISqlConnectionFactory usersConnectionFactory,
            IDataManagerFactory dataManagerFactory,
            IUsersDataManagerFactory dataUsersManagerFactory,
            RepositoryManager repositoryManager,
            ManagingBpt.ModelManager bptManager,
            ExpressionPicker expressionPicker,
            CommonParts commonParts,
            PstManager pstManager,
            BasketManager basketManager,
            ProtfolioPickerManager protfolioPickerManager,
            Gadgets.BasketPicker.ModelManager basketPickerManager,
            ManagingBpst.ModelManager bpstModelManager,
            ManagingPortfolios.PortfolioManager portfolioManager,
            ManagingCalculations.Hopper hopper,
            ManagingComments.CommentManager commentsManager
        )
        {
            this.usersConnectionFactory = usersConnectionFactory;
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.dataUsersManagerFactory = dataUsersManagerFactory;
            this.RepositoryManager = repositoryManager;
            this.BptManager = bptManager;
            this.ExpressionPicker = expressionPicker;
            this.CommonParts = commonParts;
            this.PstManager = pstManager;
            this.BasketManager = basketManager;
            this.PortfiolioPickerManager = protfolioPickerManager;
            this.BasketPickerManager = basketPickerManager;
            this.BpstManager = bpstModelManager;
            this.PortfolioManager = portfolioManager;
            this.hopper = hopper;
            this.commentManager = commentsManager;
        }

        public RepositoryManager RepositoryManager { get; private set; }
        public ManagingBpt.ModelManager BptManager { get; private set; }
        public ExpressionPicker ExpressionPicker { get; private set; }
        public CommonParts CommonParts { get; private set; }
        public PstManager PstManager { get; private set; }
        public BasketManager BasketManager { get; private set; }
        public Gadgets.PortfolioPicker.ProtfolioPickerManager PortfiolioPickerManager { get; private set; }
        public Gadgets.BasketPicker.ModelManager BasketPickerManager { get; private set; }
        public ManagingBpst.ModelManager BpstManager { get; private set; }
        public ManagingPortfolios.PortfolioManager PortfolioManager { get; private set; }

        // bpt module
        public ManagingBpt.RootModel GetBptModel(
            Int32 targetingTypeId,
            String portfolioId,
            String username
        )
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var result = this.BptManager.GetRootModel(
                    targetingTypeId,
                    portfolioId,
                    true,
                    manager,
                    username
                );
                
                return result;
            }
        }

        public void RecalculateBptModel(ManagingBpt.RootModel root, CalculationTicket ticket)
        {
            this.BptManager.RecalculateRootModel(
                root,
                this.connectionFactory,
                this.dataManagerFactory,
                ticket
            );
        }

        public IEnumerable<IValidationIssue> ApplyBroadGlobalActiveModelIfValid(ManagingBpt.RootModel root, String username, CalculationTicket ticket)
        {
            string userEmail = "";
            using (var connection = this.usersConnectionFactory.CreateConnection())
            {
                var manager = this.dataUsersManagerFactory.CreateDataManager(connection, null);
                userEmail = manager.GetUserEmail(username);
            }

            using (var connection = this.connectionFactory.CreateConnection())
            {
                using (var ondemandManager = this.CreateOnDemandDataManager(connection))
                {
                    var targetingTypeRepository = this.RepositoryManager.ClaimTargetingTypeRepository(ondemandManager);
                    var securityRepository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                    var basketRepository = this.RepositoryManager.ClaimBasketRepository(ondemandManager);
                    var portfolioRepository = this.RepositoryManager.ClaimPortfolioRepository(ondemandManager);
                    var calculation = new CalculationInfo();
                    var issues = this.BptManager.ApplyIfValid(
                        root,
                        username,
                        userEmail,
                        connection,
                        targetingTypeRepository,
                        securityRepository,
                        basketRepository,
                        portfolioRepository,
                        ticket,
                        ref calculation
                    );
                    if (!issues.Any(x => x is ErrorIssue))
                    {
                        this.Calculate(calculation.Id, true);
                    }
                    return issues;
                }
            }
        }


        // security picker

        /// <summary>
        /// Gets a list of securities whose names match the pattern.
        /// </summary>
        public IEnumerable<ISecurity> GetSecurities(String securityNamePattern, Int32 atMost)
        {
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var repository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var securities = repository.FindSomeUsingPattern(securityNamePattern, atMost, security => true);
                return securities;
            }
        }

        public IEnumerable<ISecurity> GetSecurities(String securityNamePattern, Int32 atMost, Int32 basketId)
        {
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var securityRepository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var basketRepository = this.RepositoryManager.ClaimBasketRepository(ondemandManager);
                var basket = basketRepository.GetBasket(basketId);
                var securities = securityRepository.FindSomeUsingPattern(
                    securityNamePattern,
                    atMost,
                    security => this.BasketManager.IsSecurityFromBasket(security, basket));
                return securities;
            }
        }



        // P-S-T module

        /// <summary>
        /// Gets portfolio-security-target composition.
        /// </summary>
        /// <param name="portfolioId">Portfolio ID for which composition is requested.</param>
        public ManagingPst.RootModel GetPstModel(String portfolioId)
        {
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var securityRepository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var result = this.PstManager.GetPstRootModel(ondemandManager.Claim(), portfolioId, securityRepository);
                return result;
            }
        }

        public IEnumerable<IValidationIssue> ApplyPstModelIfValid(ManagingPst.RootModel model, String username, CalculationTicket ticket)
        {
            var issues = this.PstManager.Validate(model, ticket);
            if (issues.Any()) return issues;
            string userEmail = "";
            using (var connection = this.usersConnectionFactory.CreateConnection())
            {
                var manager = this.dataUsersManagerFactory.CreateDataManager(connection, null);
                userEmail = manager.GetUserEmail(username);
            }

            using (var connection = this.connectionFactory.CreateConnection())
            {
                CalculationInfo info = new CalculationInfo();
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var retVal = this.PstManager.ApplyIsValid(model, username, userEmail, connection, ticket, this.RepositoryManager.ClaimSecurityRepository(manager), ref info);
                if (!retVal.Any())
                {
                    this.Calculate(info.Id, true);
                }
                return retVal;
            }
        }

        public Gadgets.PortfolioPicker.RootModel GetTargetingTypePortfolioPickerModel()
        {
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var targetingTypeRepository = this.RepositoryManager.ClaimTargetingTypeRepository(ondemandManager);
                var portfolioRepository = this.RepositoryManager.ClaimPortfolioRepository(ondemandManager);
                var result = this.PortfiolioPickerManager.GetRootModel(targetingTypeRepository, portfolioRepository);
                return result;
            }
        }

        // backet picker

        public Gadgets.BasketPicker.RootModel GetBasketPickerRootModel(String username)
        {
            IEnumerable<TargetingTypeGroup> targetingTypeGroups;
            IEnumerable<UsernameBasketInfo> userBaskets;
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var targetingTypeGroupRepository = this.RepositoryManager.ClaimTargetingTypeGroupRepository(ondemandManager);
                targetingTypeGroups = targetingTypeGroupRepository.GetTargetingTypeGroups();
                userBaskets = ondemandManager.Claim().GetUsernameBaskets(username);
            }
            var result = this.BasketPickerManager.GetRootModel(targetingTypeGroups, userBaskets);
            
            return result;
        }

        // B-P-S-T module

        public ManagingBpst.RootModel GetBpstModel(Int32 targetingTypeGroupId, Int32 basketId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var model = this.BpstManager.GetRootModel(
                    targetingTypeGroupId,
                    basketId,
                    manager
                );
                return model;
            }
        }

        internal OnDemandDataManager CreateOnDemandDataManager()
        {
            return new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory);
        }

        internal OnDemandDataManager CreateOnDemandDataManager(SqlConnection connection)
        {
            return new OnDemandDataManager(connection, this.dataManagerFactory);
        }

        public IEnumerable<BottomUpPortfolio> GetBottomUpPortfolios(String username)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var compositions = manager.GetUsernameBottomUpPortfolios(username);
                var portfolioRepository = this.RepositoryManager.ClaimPortfolioRepository(manager);
                var result = new List<BottomUpPortfolio>();
                foreach (var composition in compositions)
                {
                    var bottomUpPortfolio = portfolioRepository.GetBottomUpPortfolio(composition.BottomUpPortfolioId);
                    result.Add(bottomUpPortfolio);
                }
                return result;
            }
        }

        public IEnumerable<IValidationIssue> ApplyBpstModelIfValid(
            ManagingBpst.RootModel model,
            String username,
            CalculationTicket ticket)
        {
#warning NEXT: make sure everybody validates before opening a  connection
            var issues = this.BpstManager.Validate(model, ticket);
            if (issues.Any()) return issues;
            string userEmail = "";
            using (var connection = this.usersConnectionFactory.CreateConnection())
            {
                var manager = this.dataUsersManagerFactory.CreateDataManager(connection, null);
                userEmail = manager.GetUserEmail(username);
            }

            using (var connection = this.connectionFactory.CreateConnection())
            {
                CalculationInfo calculationInfo = new CalculationInfo();
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var retVal = this.BpstManager.ApplyIfValid(model, username, userEmail, connection, ticket, this.RepositoryManager.ClaimSecurityRepository(manager), this.RepositoryManager.ClaimBasketRepository(manager), ref calculationInfo);
                if (!retVal.Any())
                {
                    this.Calculate(calculationInfo.Id, true);
                }
                return retVal;
            }
        }


        public IEnumerable<CalculationWithChangesets> GetCalculationChangesets(Int32 howMany)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var calculations = manager.GetAllCalculationWithChangesets(howMany);
                return calculations;
            }
        }

        public IEnumerable<TargetRecord> Calculate(Int32 calculationId, Boolean seriously)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var managerOfRepositories = this.dataManagerFactory.CreateDataManager(connection, null);
                var securityRepository = this.RepositoryManager.ClaimSecurityRepository(managerOfRepositories);
                var portfolioRepository = this.RepositoryManager.ClaimPortfolioRepository(managerOfRepositories);
                using (var transaction = connection.BeginTransaction())
                {
                    var manager = this.dataManagerFactory.CreateDataManager(connection, transaction);
                    try
                    {
                        var tagrets = this.hopper.RecalculateEverything(calculationId, manager);
                        var fileManager = new TradingTargetsFileManager();
                        
                        if (seriously)
                        {

                            string fileContent = fileManager.GetFileContent(securityRepository, manager);
                            var now = DateTime.Now;
                            var fileName = "AshmoreEMM_Models - as of " +  now.ToString("yyyyMMdd-hhmmss")  + ".csv";
                            var directory = ConfigurationManager.AppSettings["TargetingFileOutputDirectory"];
                            if (!directory.EndsWith(@"\"))
                                directory += @"\";
                            using (StreamWriter sw = new StreamWriter(directory + fileName))
                            {
                                sw.Write(fileContent);
                            }
                            
                            transaction.Commit();
                            
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                        var result = tagrets.Select(x => new TargetRecord(
                        portfolioRepository.GetBroadGlobalActivePortfolio(x.BroadGlobalActivePortfolioId),
                        securityRepository.GetSecurity(x.SecurityId),
                        x.Target
                            )).ToArray();
                        return result;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        if (seriously)
                        {
                            this.dataManagerFactory.CreateDataManager(connection, null).FinishTargetingCalculationUnsafe(calculationId, 2, e.ToString());
                        }
                        throw e;
                    }
                }
            }
        }

        public IEnumerable<ManagingComments.CommentModel> GetCommentsForBasketPortfolioSecurityTarget(
            Int32 basketId,
            String broadGlbalActivePortfolioId,
            String securityId
        )
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var comments = this.commentManager.GetCommentsForBasketPortfolioSecurityTarget(
                    basketId,
                    broadGlbalActivePortfolioId,
                    securityId,
                    manager
                );
                return comments;
            }
        }

        public IEnumerable<ManagingComments.CommentModel> GetCommentsForTargetingTypeGroupBasketSecurityBaseValue(Int32 targetingTypeGroupId, Int32 basketId, String securityId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var comments = this.commentManager.GetCommentsForTargetingTypeGroupBasketSecurityBaseValue(
                    targetingTypeGroupId,
                    basketId,
                    securityId,
                    manager
                );
                return comments;
            }
        }


        public IEnumerable<ManagingComments.CommentModel> RequestCommentsForTargetingTypeBasketBaseValue(int targetingTypeId, int basketId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var comments = this.commentManager.RequestCommentsForTargetingTypeBasketBaseValue(
                    targetingTypeId,
                    basketId,
                    manager
                );
                return comments;
            }
        }

        public IEnumerable<ManagingComments.CommentModel> RequestCommentsForTargetingTypeBasketPortfolioTarget(int targetingTypeId, string portfolioId, int basketId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var comments = this.commentManager.RequestCommentsForTargetingTypeBasketPortfolioTarget(
                    targetingTypeId,
                    portfolioId,
                    basketId,
                    manager
                );
                return comments;
            }
        }

        public IEnumerable<ManagingComments.CommentModel> RequestCommentsForBgaPortfolioSecurityFactor(string portfolioId, string securityId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var comments = this.commentManager.RequestCommentsForBgaPortfolioSecurityFactor(
                    portfolioId,
                    securityId,
                    manager
                );
                return comments;
            }
        }

        public IEnumerable<ManagingComments.CommentModel> RequestCommentsForBuPortfolioSecurityTarget(string portfolioId, string securityId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, null);
                var comments = this.commentManager.RequestCommentsForBuPortfolioSecurityTarget(
                    portfolioId,
                    securityId,
                    manager
                );
                return comments;
            }
        }

        
    }
}
