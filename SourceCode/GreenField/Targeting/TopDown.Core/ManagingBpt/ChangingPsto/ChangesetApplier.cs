using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.ChangingPsto
{
    public class ChangesetApplier : ChangesetApplierBase<Changeset, IChange>
    {
        protected override IEnumerable<IChange> GetChanges(Changeset changeset)
        {
            return changeset.Changes;
        }

        protected override void MakeSureThereWereNoChangesSince(Changeset changeset, IDataManager manager)
        {
            var latestChangeset = manager.GetLatestBgaPortfolioSecurityFactorChangeset();
            if (changeset.LatestChangesetSnapshot.Id < latestChangeset.Id)
            {
                throw new ValidationException(
                    new ErrorIssue("User \"" + latestChangeset.Username + "\" modified the P-S-TO composition on " + latestChangeset.Timestamp + ".")
                );
            }
        }

        protected override IEnumerator<Int32> RequestChangeIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveBgaPortfolioSecurityFactorChangeIds(howMany);
            return result;
        }

        protected override IEnumerator<Int32> RequestChangesetIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveBgaPortfolioSecurityFactorChangesetIds(howMany);
            return result;
        }

        protected override void ApplyChangeset(Changeset changeset, Int32 changesetId, Int32 computationId, IDataManager manager)
        {
            var changesetInfo = new BgaPortfolioSecurityFactorChangesetInfo(
                changesetId,
                changeset.Username,
                DateTime.Now, //  <---- isn't going to be used
                computationId
            );
            manager.InsertBgaPortfolioSecurityFactorChangeset(changesetInfo);
        }

        protected override void ApplyChangeOnceResolved(IChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var resolver = new ApplyChangeOnceResolved_IChangeResolver(this, changeset, changeId, changesetId, manager);
            change.Accept(resolver);
        }

        private class ApplyChangeOnceResolved_IChangeResolver : IChangeResolver
        {
            private IDataManager manager;
            private Int32 changesetId;
            private Int32 changeId;
            private Changeset changeset;
            private ChangesetApplier applier;

            public ApplyChangeOnceResolved_IChangeResolver(
                ChangesetApplier applier,
                Changeset changeset,
                Int32 changeId,
                Int32 changesetId,
                IDataManager manager
            )
            {
                this.manager = manager;
                this.changesetId = changesetId;
                this.changeId = changeId;
                this.changeset = changeset;
                this.applier = applier;
            }

            public void Resolve(DeleteChange change)
            {
                this.applier.ApplyDeleteChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.ApplyUpdateChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.ApplyInsertChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }
        }

        protected void ApplyInsertChange(InsertChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new BgaPortfolioSecurityFactorChangeInfo(
                changeId,
                changeset.PortfolioId,
                change.SecurityId,
                null,
                change.TargetOverlayAfter,
                change.Comment,
                changesetId
            );
            manager.InsertBgaPortfolioSecurityFactorChange(changeInfo);

            var info = new BgaPortfolioSecurityFactorInfo(
                changeset.PortfolioId,
                change.SecurityId,
                change.TargetOverlayAfter,
                changeId
            );
            manager.InsertBgaPortfolioSecurityFactor(info);
        }

        protected void ApplyUpdateChange(UpdateChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new BgaPortfolioSecurityFactorChangeInfo(
                changeId,
                changeset.PortfolioId,
                change.SecurityId,
                change.TargetBefore,
                change.TargetAfter,
                change.Comment,
                changesetId
            );
            manager.InsertBgaPortfolioSecurityFactorChange(changeInfo);

            var info = new BgaPortfolioSecurityFactorInfo(
                changeset.PortfolioId,
                change.SecurityId,
                change.TargetAfter,
                changeId
            );
            manager.UpdateBgaPortfolioSecurityFactor(info);
        }

        protected void ApplyDeleteChange(DeleteChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new BgaPortfolioSecurityFactorChangeInfo(
                changeId,
                changeset.PortfolioId,
                change.SecurityId,
                change.TargetOverlayBefore,
                null,
                change.Comment,
                changesetId
            );
            manager.InsertBgaPortfolioSecurityFactorChange(changeInfo);
            manager.DeleteBgaPortfolioSecurityFactor(changeset.PortfolioId, change.SecurityId);
        }



    }
}
