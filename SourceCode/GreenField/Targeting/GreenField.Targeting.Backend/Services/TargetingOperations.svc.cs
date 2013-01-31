using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Net.Mail;
using System.Resources;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;
using TopDown.Core;
using System.Web.Caching;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingCalculations;
using Aims.Core.Sql;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingBpt;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingBpt.ChangingTtbpt;
using TopDown.Core.ManagingPst;
using TopDown.Core.Gadgets.PortfolioPicker;
using GreenField.Targeting.Backend.Helpers;

namespace GreenField.Targeting.Backend
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TargetingOperations : GreenField.Targeting.Server.IFacade
    {
        private Server.Facade facade;

        public TargetingOperations(Server.FacadeSettings settings)
        {
            this.facade = new Server.Facade(settings);
        }

        protected TResult Watch<TResult>(String failureMessage, Func<TResult> action)
        {
            TResult result;
            try
            {
                result = action();
            }
            catch (Exception exception)
            {
                throw new ApplicationException(failureMessage + " Reason: " + exception.Message, exception);
            }
            return result;
        }



        public Server.BroadGlobalActive.RootModel GetBroadGlobalActive(Int32 targetingTypeId, String bgaPortfolioId, String username)
        {
            return this.Watch("Unable to get the broad global active data for targeting type (ID: " + targetingTypeId + ") and broad global active portfolio (ID: " + bgaPortfolioId + ").", delegate
            {
                return this.facade.GetBroadGlobalActive(targetingTypeId, bgaPortfolioId, username);
            });
        }

        public IEnumerable<Server.BroadGlobalActive.Picker.TargetingTypeModel> GetTargetingTypePortfolioPicker()
        {
            return this.Watch("Unable to get the data for targeting type and porfolio picker.", delegate
            {
                return this.facade.GetTargetingTypePortfolioPicker();
            });
        }

        public Server.BroadGlobalActive.RootModel RecalculateBroadGlobalActive(Server.BroadGlobalActive.RootModel model)
        {
            return this.Watch("Unable to recalculate the broad global active data.", delegate
            {
                return this.facade.RecalculateBroadGlobalActive(model);
            });
        }

        public IEnumerable<Aims.Data.Server.SecurityModel> PickSecurities(String pattern, Int32 atMost)
        {
            return this.Watch("Unable to get a securities for the pattern \"" + pattern + "\".", delegate
            {
                return this.facade.PickSecurities(pattern, atMost);
            });
        }

        public IEnumerable<Aims.Data.Server.SecurityModel> PickSecuritiesFromBasket(String pattern, Int32 atMost, Int32 basketId)
        {
            return this.Watch("Unable to get securities for the pattern \"" + pattern + "\" from the basket (ID: " + basketId + ").", delegate
            {
                return this.facade.PickSecuritiesFromBasket(pattern, atMost, basketId);
            });
        }

        public IEnumerable<Server.IssueModel> SaveBroadGlobalActive(Server.BroadGlobalActive.RootModel model, string username)
        {
            return this.Watch("Unable to save the broad global active data.", delegate
            {
                return this.facade.SaveBroadGlobalActive(model, username);
            });
        }

        public Server.BasketTargets.PickerModel GetBasketPicker(string username)
        {
            return this.Watch("Unable to get the data for the basket picker.", delegate
            {
                return this.facade.GetBasketPicker(username);
            });
        }

        public Server.BasketTargets.RootModel GetBasketTargets(Int32 targetingTypeGroupId, Int32 basketId)
        {
            return this.Watch("Unable to get basket targets for the targeting type group (ID: " + targetingTypeGroupId + "), basket (ID: " + basketId + ").", delegate
            {
                return this.facade.GetBasketTargets(targetingTypeGroupId, basketId);
            });
        }

        public Server.BasketTargets.RootModel RecalculateBasketTargets(Server.BasketTargets.RootModel model)
        {
            return this.Watch("Unable to recalculate the basket targets.", delegate
            {
                return this.facade.RecalculateBasketTargets(model);
            });
        }

        public IEnumerable<Server.IssueModel> SaveBasketTargets(Server.BasketTargets.RootModel model, string username)
        {
            return this.Watch("Unable to save the basket targets.", delegate
            {
                return this.facade.SaveBasketTargets(model, username);
            });
        }

        public Server.BottomUp.PickerModel GetBottomUpPortfolioPicker(string username)
        {
            return this.Watch("Unable to get the bottom-up portfolio picker data.", delegate
            {
                return this.facade.GetBottomUpPortfolioPicker(username);
            });
        }

        public Server.BottomUp.RootModel GetBottomUpModel(String bottomUpPortfolioId)
        {
            return this.Watch("Unable to get a bottom-up data for the bottom-up portfolio (ID: " + bottomUpPortfolioId + ").", delegate
            {
                return this.facade.GetBottomUpModel(bottomUpPortfolioId);
            });
        }

        public Server.BottomUp.RootModel RecalculateBottomUp(Server.BottomUp.RootModel model)
        {
            return this.Watch("Unable to recalculate the bottom-up data.", delegate
            {
                return this.facade.RecalculateBottomUp(model);
            });
        }

        public IEnumerable<Server.IssueModel> SaveBottomUp(Server.BottomUp.RootModel model, string username)
        {
            return this.Watch("Unable to save the bottom-up data.", delegate
            {
                return this.facade.SaveBottomUp(model, username);
            });
        }

        // comments

        public IEnumerable<Server.CommentModel> RequestCommentsForBasketPortfolioSecurityTarget(int basketId, string broadGlbalActivePortfolioId, string securityId)
        {
            return this.Watch(String.Format("Getting comments for BasketPortfolioSecurityTarget: basketId - {0}, broadGlbalActivePortfolioId - {1}, securityId - {2}", basketId, broadGlbalActivePortfolioId, securityId), delegate
            {
                return this.facade.RequestCommentsForBasketPortfolioSecurityTarget(basketId, broadGlbalActivePortfolioId, securityId);
            });
        }


        public IEnumerable<Server.CommentModel> RequestCommentsForTargetingTypeGroupBasketSecurityBaseValue(Int32 targetingTypeGroupId, Int32 basketId, String securityId)
        {
            return this.Watch(String.Format("Getting comments for TargetingTypeGroupBasketSecurityBaseValue: basketId - {0}, targetingTypeGroupId - {1}, securityId - {2}", basketId, targetingTypeGroupId, securityId), delegate
            {
                return this.facade.RequestCommentsForTargetingTypeGroupBasketSecurityBaseValue(targetingTypeGroupId, basketId, securityId);
            });
            
        }


        public IEnumerable<Server.CommentModel> RequestCommentsForTargetingTypeBasketBaseValue(int targetingTypeId, int basketId)
        {
            return this.Watch(String.Format("Getting comments for RequestCommentsForTargetingTypeBasketBaseValue: basketId - {0}, targetingTypeId - {1}", basketId, targetingTypeId), delegate
            {
                return this.facade.RequestCommentsForTargetingTypeBasketBaseValue(targetingTypeId, basketId);
            });
        }


        public IEnumerable<Server.CommentModel> RequestCommentsForTargetingTypeBasketPortfolioTarget(int targetingTypeId, string portfolioId, int basketId)
        {
            return this.Watch(String.Format("Getting comments for RequestCommentsForTargetingTypeBasketPortfolioTarget: basketId - {0}, targetingTypeId - {1}, portfolioId - {2}", basketId, targetingTypeId, portfolioId), delegate
            {
                return this.facade.RequestCommentsForTargetingTypeBasketPortfolioTarget(targetingTypeId, portfolioId, basketId);
            });
        }


        public IEnumerable<Server.CommentModel> RequestCommentsForBgaPortfolioSecurityFactor(string portfolioId, string securityId)
        {
            return this.Watch(String.Format("Getting comments for RequestCommentsForBgaPortfolioSecurityFactor: portfolioId - {0}, securityId - {1}", portfolioId, securityId), delegate
            {
                return this.facade.RequestCommentsForBgaPortfolioSecurityFactor(portfolioId, securityId);
            });
        }


        public IEnumerable<Server.CommentModel> RequestCommentsForBuPortfolioSecurityTarget(string portfolioId, string securityId)
        {
            return this.Watch(String.Format("Getting comments for RequestCommentsForBuPortfolioSecurityTarget: portfolioId - {0}, securityId - {1}", portfolioId, securityId), delegate
            {
                return this.facade.RequestCommentsForBuPortfolioSecurityTarget(portfolioId, securityId);
            });
        }
    }
}
