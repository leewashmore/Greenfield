using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace GreenField.Targeting.Server
{
    [ServiceContract]
    public interface IFacade
    {
        [OperationContract]
        BroadGlobalActive.RootModel GetBroadGlobalActive(Int32 targetingTypeId, String bgaPortfolioId, DateTime benchmarkDate);

        [OperationContract]
        IEnumerable<BroadGlobalActive.Picker.TargetingTypeModel> GetTargetingTypePortfolioPicker();

        [OperationContract]
        BroadGlobalActive.RootModel RecalculateBroadGlobalActive(BroadGlobalActive.RootModel model);

        [OperationContract]
        IEnumerable<SecurityModel> PickSecurities(String pattern, Int32 atMost);

        [OperationContract]
        IEnumerable<SecurityModel> PickSecuritiesFromBasket(String pattern, Int32 atMost, Int32 basketId);

        [OperationContract]
        IEnumerable<IssueModel> SaveBroadGlobalActive(BroadGlobalActive.RootModel model);

		// backet targets

        [OperationContract]
        BasketTargets.PickerModel GetBasketPicker();

        [OperationContract]
        BasketTargets.RootModel GetBasketTargets(Int32 targetingTypeGroupId, Int32 basketId, DateTime benchmarkDate);

        [OperationContract]
        BasketTargets.RootModel RecalculateBasketTargets(BasketTargets.RootModel model, DateTime benchmarkDate);

		[OperationContract]
		IEnumerable<IssueModel> SaveBasketTargets(BasketTargets.RootModel model, DateTime benchmarkDate);

		// bottom up

        [OperationContract]
        BottomUp.PickerModel GetBottomUpPortfolioPicker();

        [OperationContract]
        BottomUp.RootModel GetBottomUpModel(String bottomUpPortfolioId);

        [OperationContract]
        BottomUp.RootModel RecalculateBottomUp(BottomUp.RootModel model);

        [OperationContract]
        IEnumerable<IssueModel> SaveBottomUp(BottomUp.RootModel model);
    }
}
