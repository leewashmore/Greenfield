using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Aims.Data.Server;

namespace GreenField.Targeting.Server
{
    [ServiceContract]
    public interface IFacade
    {
        [OperationContract]
        BroadGlobalActive.RootModel GetBroadGlobalActive(Int32 targetingTypeId, String bgaPortfolioId);

        [OperationContract]
        IEnumerable<BroadGlobalActive.Picker.TargetingTypeModel> GetTargetingTypePortfolioPicker();

        [OperationContract]
        BroadGlobalActive.RootModel RecalculateBroadGlobalActive(BroadGlobalActive.RootModel model);

        [OperationContract]
        IEnumerable<SecurityModel> PickSecurities(String pattern, Int32 atMost);

        [OperationContract]
        IEnumerable<SecurityModel> PickSecuritiesFromBasket(String pattern, Int32 atMost, Int32 basketId);

        [OperationContract]
        IEnumerable<IssueModel> SaveBroadGlobalActive(BroadGlobalActive.RootModel model, string username);

		// backet targets

        [OperationContract]
        BasketTargets.PickerModel GetBasketPicker(string username);

        [OperationContract]
        BasketTargets.RootModel GetBasketTargets(Int32 targetingTypeGroupId, Int32 basketId);

        [OperationContract]
        BasketTargets.RootModel RecalculateBasketTargets(BasketTargets.RootModel model);

		[OperationContract]
        IEnumerable<IssueModel> SaveBasketTargets(BasketTargets.RootModel model, string username);

		// bottom up

        [OperationContract]
        BottomUp.PickerModel GetBottomUpPortfolioPicker(string username);

        [OperationContract]
        BottomUp.RootModel GetBottomUpModel(String bottomUpPortfolioId);

        [OperationContract]
        BottomUp.RootModel RecalculateBottomUp(BottomUp.RootModel model);

        [OperationContract]
        IEnumerable<IssueModel> SaveBottomUp(BottomUp.RootModel model, string username);


        // comments

        [OperationContract]
        IEnumerable<CommentModel> RequestCommentsForBasketPortfolioSecurityTarget(Int32 basketId, String broadGlbalActivePortfolioId, String securityId);

        [OperationContract]
        IEnumerable<CommentModel> RequestCommentsForTargetingTypeGroupBasketSecurityBaseValue(Int32 targetingTypeGroupId, Int32 basketId, String securityId);

        [OperationContract]
        IEnumerable<CommentModel> RequestCommentsForTargetingTypeBasketBaseValue(Int32 targetingTypeId, Int32 basketId);
        
        [OperationContract]
        IEnumerable<CommentModel> RequestCommentsForTargetingTypeBasketPortfolioTarget(Int32 targetingTypeId, string portfolioId, Int32 basketId);

        [OperationContract]
        IEnumerable<CommentModel> RequestCommentsForBgaPortfolioSecurityFactor(string portfolioId, string securityId);

        [OperationContract]
        IEnumerable<CommentModel> RequestCommentsForBuPortfolioSecurityTarget(string portfolioId, string securityId);
    }
}
