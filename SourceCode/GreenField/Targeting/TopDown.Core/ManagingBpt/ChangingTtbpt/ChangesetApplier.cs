using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.ChangingTtbpt
{
    public class ChangesetApplier : ChangesetApplierBase<Changeset, IChange>
    {
        protected override IEnumerable<IChange> GetChanges(Changeset changeset)
        {
            return changeset.Changes;
        }

        protected override void MakeSureThereWereNoChangesSince(Changeset changeset, IDataManager manager)
        {
            var latestChangeset = manager.GetLatestTargetingTypeBasketPortfolioTargetChangeset();
            if (changeset.LatestChangesetSnapshot.Id < latestChangeset.Id)
            {
                throw new ValidationException(
                    new ValidationIssue("User \"" + latestChangeset.Username + "\" modified the TT-B-P-T composition on " + latestChangeset.Timestamp + ".")
                );
            }
        }

        protected override IEnumerator<Int32> RequestChangesetIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveTargetingTypeBasketPortfolioTargetChangesetIds(howMany);
            return result;
        }

        protected override void ApplyChangeset(Changeset changeset, Int32 changesetId, Int32 computationId, IDataManager manager)
        {
            var changesetInfo = new TargetingTypeBasketPortfolioTargetChangesetInfo(
                changesetId,
                changeset.Username,
                DateTime.Now, // <---- is going to be ignored
                computationId
            );
            manager.InsertTargetingTypeBasketPortfolioTargetChangeset(changesetInfo);
        }

        protected override IEnumerator<Int32> RequestChangeIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveTargetingTypeBasketPortfolioTargetChangeIds(howMany);
            return result;
        }

        protected override void ApplyChangeOnceResolved(IChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var resolver = new ApplyChange_IChangeResolver(this, changeset, changeId, changesetId, manager);
            change.Accept(resolver);
        }

        private class ApplyChange_IChangeResolver : IChangeResolver
        {
            private ChangesetApplier applier;
            private Changeset changeset;
            private Int32 changeId;
            private Int32 changesetId;
            private IDataManager manager;

            public ApplyChange_IChangeResolver(ChangesetApplier applier, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
            {
                this.applier = applier;
                this.changeset = changeset;
                this.changeId = changeId;
                this.changesetId = changesetId;
                this.manager = manager;
            }
            public void Resolve(DeleteChange change)
            {
                this.applier.ApplyDeleteChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.ApplyInsertChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.ApplyUpdateChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }
        }

        protected void ApplyInsertChange(InsertChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeBasketPortfolioTargetChangeInfo(
                changeId,
                changeset.TargetingTypeId,
                change.BasketId,
                changeset.PortfolioId,
                null,
                change.TargetAfter,
                change.Comment,
                changesetId
            );

            manager.InsertTargetingTypeBasketPortfolioTargetChange(changeInfo);

            var info = new TargetingTypeBasketPortfolioTargetInfo(
                changeset.TargetingTypeId,
                change.BasketId,
                changeset.PortfolioId,
                change.TargetAfter,
                changeId
            );
            manager.InsertTargetingTypeBasketPortfolioTarget(info);
        }

        protected void ApplyUpdateChange(UpdateChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeBasketPortfolioTargetChangeInfo(
                changeId,
                changeset.TargetingTypeId,
                change.BasketId,
                changeset.PortfolioId,
                change.TargetBefore,
                change.TargetAfter,
                change.Comment,
                changesetId
            );

            manager.InsertTargetingTypeBasketPortfolioTargetChange(changeInfo);

            var info = new TargetingTypeBasketPortfolioTargetInfo(
                changeset.TargetingTypeId,
                change.BasketId,
                changeset.PortfolioId,
                change.TargetAfter,
                changeId
            );

            manager.UpdateTargetingTypeBasketPortfolioTarget(info);
        }

        protected void ApplyDeleteChange(DeleteChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeBasketPortfolioTargetChangeInfo(
                changeId,
                changeset.TargetingTypeId,
                change.BasketId,
                changeset.PortfolioId,
                change.TargetBefore,
                null,
                change.Comment,
                changesetId
            );

            manager.InsertTargetingTypeBasketPortfolioTargetChange(changeInfo);

            manager.DeleteTargetingTypeBasketPortfolioTarget(changeset.TargetingTypeId, change.BasketId, changeset.PortfolioId);
        }
    }
}