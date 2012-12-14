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
    }
}
