using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBpt;
using TopDown.Core.ManagingBaskets;
using System.Diagnostics;
using TopDown.Core.Persisting;

namespace TopDown.Core.Gadgets.BasketPicker
{
	public class ModelBuilder
	{
        private BasketExtractor basketExtractor;
        private BasketRenderer basketRenderer;
        
        [DebuggerStepThrough]
        public ModelBuilder(BasketExtractor basketExtractor, BasketRenderer basketRenderer)
        {
            this.basketExtractor = basketExtractor;
            this.basketRenderer = basketRenderer;
        }

        public RootModel CreateRootModel(IEnumerable<TargetingTypeGroup> targetingTypeGroups, IEnumerable<UsernameBasketInfo> baskets)
		{
			var groups = this.TransformToGroups(targetingTypeGroups, baskets);
			var result = new RootModel(groups);
			return result;
		}

        public IEnumerable<TargetingGroupModel> TransformToGroups(IEnumerable<TargetingTypeGroup> targetingTypeGroups, IEnumerable<UsernameBasketInfo> baskets)
		{
			return targetingTypeGroups.Select(x => this.TransformToGroup(x, baskets));
		}

        public TargetingGroupModel TransformToGroup(TargetingTypeGroup groupModel, IEnumerable<UsernameBasketInfo> baskets)
		{
            var result = new TargetingGroupModel(
                groupModel.Id,
                groupModel.Name,
                groupModel.GetTargetingTypes()
                    .SelectMany(x => this.basketExtractor.ExtractBaskets(x))
					.GroupBy(x => x.Id) // getting rid of the same baskets
                    .Select(x => new BasketModel(x.Key, this.basketRenderer.RenderBasketOnceResolved(x.First()))),
                baskets);
            
            return result;
		}

	}
}
