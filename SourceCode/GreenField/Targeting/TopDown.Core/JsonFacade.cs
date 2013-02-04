using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBpt;
using System.IO;
using Newtonsoft.Json;
using TopDown.Core.Overlaying;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingPortfolios;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBpst;
using Aims.Expressions;
using TopDown.Core.ManagingBenchmarks;
using Aims.Core;

namespace TopDown.Core
{
    public class JsonFacade
    {
        private Facade facade;
        private ValidationManager validationManager;
        private SecurityToJsonSerializer securitySerializer;

        public JsonFacade(
            Facade facade,
            ValidationManager validationManager,
            ManagingSecurities.SecurityToJsonSerializer securitySerializer
        )
        {
            this.facade = facade;
            this.securitySerializer = securitySerializer;
            this.validationManager = validationManager;
        }

        /// <summary>
        /// Gets JSON representation of securities retreived using the first few characters represented by the pattern that matched the names of securities.
        /// </summary>
        public String GetSecurities(String securityNamePattern, Int32 atMost)
        {
            var securities = this.facade.GetSecurities(securityNamePattern, atMost);
            var json = this.SecuritiesToJson(securities);
            return json;
        }

        public String GetSecurities(String securityNamePattern, Int32 atMost, Int32 basketId)
        {
            var securities = this.facade.GetSecurities(securityNamePattern, atMost, basketId);
            var json = this.SecuritiesToJson(securities);
            return json;
        }

        public String GetTargetingTypePortfolioPickerModel()
        {
            var targetings = this.facade.GetTargetingTypePortfolioPickerModel();
            var json = this.facade.PortfiolioPickerManager.SerializeToJson(targetings);
            return json;
        }

        public String GetBreakdown(Int32 targetingId, String portfolioId, CalculationTicket ticket, String username)
        {
            var breakdown = this.facade.GetBptModel(targetingId, portfolioId, username);
            var result = this.facade.BptManager.SerializeToJson(breakdown, ticket);
            return result;
        }

        public String RecalculateBreakdown(String bptAsJson, CalculationTicket ticket)
        {
            BasketRepository basketRepository;
            SecurityRepository securityRepository;
            PortfolioRepository portfolioRepository;
            TargetingTypeRepository targetingTypeRepository;
            using (var ondemandManager = this.facade.CreateOnDemandDataManager())
            {
                basketRepository = this.facade.RepositoryManager.ClaimBasketRepository(ondemandManager);
                securityRepository = this.facade.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                portfolioRepository = this.facade.RepositoryManager.ClaimPortfolioRepository(ondemandManager);
                targetingTypeRepository = this.facade.RepositoryManager.ClaimTargetingTypeRepository(ondemandManager);
            }

            var root = this.facade.BptManager.DeserializeFromJson(
                bptAsJson,
                basketRepository,
                securityRepository,
                portfolioRepository,
                targetingTypeRepository
            );
            this.facade.RecalculateBptModel(root, ticket);
            var result = this.facade.BptManager.SerializeToJson(root, ticket);
            return result;
        }

        public String TryApplyBptModel(String bptAsJson, String username, CalculationTicket ticket)
        {
            BasketRepository basketRepository;
            SecurityRepository securityRepository;
            PortfolioRepository portfolioRepository;
            TargetingTypeRepository targetingTypeRepository;
            using (var ondemandManager = this.facade.CreateOnDemandDataManager())
            {
                basketRepository = this.facade.RepositoryManager.ClaimBasketRepository(ondemandManager);
                securityRepository = this.facade.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                portfolioRepository = this.facade.RepositoryManager.ClaimPortfolioRepository(ondemandManager);
                targetingTypeRepository = this.facade.RepositoryManager.ClaimTargetingTypeRepository(ondemandManager);
            }
            var model = this.facade.BptManager.DeserializeFromJson(
                bptAsJson,
                basketRepository,
                securityRepository,
                portfolioRepository,
                targetingTypeRepository
            );
            var issues = this.facade.ApplyBroadGlobalActiveModelIfValid(model, username, ticket);
            var issuesAsJson = this.validationManager.SerializeToJson(issues);
            return issuesAsJson;
        }


        public String GetPstComposition(String portfolioId, CalculationTicket ticket)
        {
            var composition = this.facade.GetPstModel(portfolioId);
            var result = this.facade.PstManager.SerializeToJson(composition, ticket);
            return result;
        }

        protected String SecuritiesToJson(IEnumerable<ISecurity> securities)
        {
            var builder = new StringBuilder();
            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.WriteArray(securities, security =>
                {
                    writer.Write(delegate
                    {
                        this.securitySerializer.SerializeSecurityOnceResolved(security, writer);
                    });
                });
            }
            var result = builder.ToString();
            return result;
        }

        public String RecalculatePstModel(String pstAsJson, CalculationTicket ticket)
        {
            SecurityRepository securityRepository;
            using (var ondemandManager = this.facade.CreateOnDemandDataManager())
            {
                securityRepository = this.facade.RepositoryManager.ClaimSecurityRepository(ondemandManager);
            }
            var composition = this.facade.PstManager.DeserializeFromJson(pstAsJson, securityRepository);
            var refereshedPstCompositionAsJson = this.facade.PstManager.SerializeToJson(composition, ticket);
            return refereshedPstCompositionAsJson;
        }

        public String TryApplyPstModel(String pstAsJson, String username, CalculationTicket ticket)
        {
            SecurityRepository securityRepository;
            using (var ondemandManager = this.facade.CreateOnDemandDataManager())
            {
                securityRepository = this.facade.RepositoryManager.ClaimSecurityRepository(ondemandManager);
            }
            var model = this.facade.PstManager.DeserializeFromJson(pstAsJson, securityRepository);
            var issues = this.facade.ApplyPstModelIfValid(model, username, ticket);
            var issuesAsJson = this.validationManager.SerializeToJson(issues);
            return issuesAsJson;
        }

        public String GetBasketPicker(String username)
        {
            var model = this.facade.GetBasketPickerRootModel(username);
            var json = this.facade.BasketPickerManager.ToJson(model);
            return json;
        }

        public String GetBpstModel(Int32 targetingTypeGroupId, Int32 basketId, CalculationTicket ticket)
        {
            var model = this.facade.GetBpstModel(
                targetingTypeGroupId,
                basketId
            );
            var json = this.facade.BpstManager.SerializeToJson(model, ticket);
            return json;
        }

        public String RecalculateBpstModel(String bpstModelAsJson, CalculationTicket ticket)
        {
            SecurityRepository securityRepository;
            TargetingTypeGroupRepository targetingTypeGroupRepository;
            BasketRepository basketRepository;
            PortfolioRepository portfolioRepository;
            ManagingBpst.RootModel model;
            using (var ondemandManager = this.facade.CreateOnDemandDataManager())
            {
                securityRepository = this.facade.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                targetingTypeGroupRepository = this.facade.RepositoryManager.ClaimTargetingTypeGroupRepository(ondemandManager);
                basketRepository = this.facade.RepositoryManager.ClaimBasketRepository(ondemandManager);
                portfolioRepository = this.facade.RepositoryManager.ClaimPortfolioRepository(ondemandManager);

                model = this.facade.BpstManager.DeserializeFromJson(
                    bpstModelAsJson,
                    securityRepository,
                    targetingTypeGroupRepository,
                    basketRepository,
                    portfolioRepository,
                    ondemandManager
                );
            }
            var json = this.facade.BpstManager.SerializeToJson(model, ticket);
            return json;
        }

        public String TryApplyBpstModel(String bpstAsJson, String username, CalculationTicket ticket)
        {
            SecurityRepository securityRepository;
            TargetingTypeGroupRepository targetingTypeGroupRepository;
            BasketRepository basketRepository;
            PortfolioRepository portfolioRepository;

            ManagingBpst.RootModel model;
            using (var ondemandManager = this.facade.CreateOnDemandDataManager())
            {
                securityRepository = this.facade.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                targetingTypeGroupRepository = this.facade.RepositoryManager.ClaimTargetingTypeGroupRepository(ondemandManager);
                basketRepository = this.facade.RepositoryManager.ClaimBasketRepository(ondemandManager);
                portfolioRepository = this.facade.RepositoryManager.ClaimPortfolioRepository(ondemandManager);

                model = this.facade.BpstManager.DeserializeFromJson(
                    bpstAsJson,
                    securityRepository,
                    targetingTypeGroupRepository,
                    basketRepository,
                    portfolioRepository,
                    ondemandManager
                );
            }

            var issues = this.facade.ApplyBpstModelIfValid(model, username, ticket);
            var issuesAsJson = this.validationManager.SerializeToJson(issues);
            return issuesAsJson;
        }

        public String GetBottomUpPortfolios(String username)
        {
            var result = this.facade.GetBottomUpPortfolios(username);
            var json = this.facade.PortfolioManager.SerializeToJson(result);
            return json;
        }
    }
}