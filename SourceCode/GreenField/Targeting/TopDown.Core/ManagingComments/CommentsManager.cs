using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingComments
{
    public class CommentManager
    {
        public IEnumerable<CommentModel> GetCommentsForBasketPortfolioSecurityTarget(Int32 basketId, String broadGlbalActivePortfolioId, String securityId, IDataManager manager)
        {
            var changes = manager.GetBasketPortfolioSecurityTargetChanges(basketId, broadGlbalActivePortfolioId, securityId);
            var changesetIds = changes.Select(x => x.ChangesetId).ToArray();
            var changsets = manager.GetBasketPortfolioSecurityTargetChangesets(changesetIds);

            var result = this.WeldTogether(changes, changsets);
            return result;
        }

        private IEnumerable<CommentModel> WeldTogether(IEnumerable<IChangeInfoBase> changes, IEnumerable<ChangesetInfoBase> changsets)
        {
            var result = new List<CommentModel>();
            var map = changsets.ToDictionary(x => x.Id);
            foreach (var change in changes)
            {
                var commentOpt = TryCreateComment(map, change);
                if (commentOpt == null) throw new ApplicationException();
                result.Add(commentOpt);
            }
            return result;
        }

        private static CommentModel TryCreateComment(Dictionary<int, ChangesetInfoBase> map, IChangeInfoBase change)
        {
            CommentModel comment;
            ChangesetInfoBase changeset;
            if (map.TryGetValue(change.ChangesetId, out changeset))
            {
                comment = new CommentModel(change, changeset);
            }
            else
            {
                comment = null;
            }
            return comment;
        }

        public IEnumerable<CommentModel> GetCommentsForTargetingTypeGroupBasketSecurityBaseValue(int targetingTypeGroupId, int basketId, string securityId, IDataManager manager)
        {
            var changes = manager.GetTargetingTypeGroupBasketSecurityBaseValueChanges(targetingTypeGroupId, basketId, securityId);
            var changesetIds = changes.Select(x => x.ChangesetId).ToArray();
            var changsets = manager.GetTargetingTypeGroupBasketSecurityBaseValueChangesets(changesetIds);

            var result = this.WeldTogether(changes, changsets);
            return result;
        }

        public IEnumerable<CommentModel> RequestCommentsForTargetingTypeBasketBaseValue(int targetingTypeId, int basketId, IDataManager manager)
        {
            var changes = manager.GetTargetingTypeBasketBaseValueChanges(targetingTypeId, basketId);
            var changesetIds = changes.Select(x => x.ChangesetId).ToArray();
            var changsets = manager.GetTargetingTypeBasketBaseValueChangesets(changesetIds);

            var result = this.WeldTogether(changes, changsets);
            return result;
        }

        public IEnumerable<CommentModel> RequestCommentsForTargetingTypeBasketPortfolioTarget(int targetingTypeId, string portfolioId, int basketId, IDataManager manager)
        {
            var changes = manager.GetTargetingTypeBasketPortfolioTargetChanges(targetingTypeId, portfolioId, basketId);
            var changesetIds = changes.Select(x => x.ChangesetId).ToArray();
            var changsets = manager.GetTargetingTypeBasketPortfolioTargetChangesets(changesetIds);

            var result = this.WeldTogether(changes, changsets);
            return result;
        }

        public IEnumerable<CommentModel> RequestCommentsForBgaPortfolioSecurityFactor(string portfolioId, string securityId, IDataManager manager)
        {
            var changes = manager.GetBgaPortfolioSecurityFactorChanges(portfolioId, securityId);
            var changesetIds = changes.Select(x => x.ChangesetId).ToArray();
            var changsets = manager.GetBgaPortfolioSecurityFactorChangesets(changesetIds);

            var result = this.WeldTogether(changes, changsets);
            return result;
        }

        public IEnumerable<CommentModel> RequestCommentsForBuPortfolioSecurityTarget(string portfolioId, string securityId, IDataManager manager)
        {
            var changes = manager.GetBuPortfolioSecurityTargetChanges(portfolioId, securityId);
            var changesetIds = changes.Select(x => x.ChangesetId).ToArray();
            var changsets = manager.GetBuPortfolioSecurityTargetChangesets(changesetIds);

            var result = this.WeldTogether(changes, changsets);
            return result;
        }
    }
}
