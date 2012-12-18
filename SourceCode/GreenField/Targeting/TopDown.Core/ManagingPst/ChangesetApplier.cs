using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    public class ChangesetApplier
    {
        public void Apply(Int32 calculationId, Changeset changeset, IDataManager manager)
        {
            var latestChangeSet = manager.GetLatestPortfolioSecurityTargetChangeSet();
            if (changeset.LatestChangeset.Id < latestChangeSet.Id)
            {
                throw new ValidationException(
                    new ErrorIssue("User \"" + latestChangeSet.Username + "\" modified the P-S-T composition on " + latestChangeSet.Timestamp + ".")
                );
            }

            var changesetIdRange = manager.ReservePortfolioSecurityTargetChangesetIds(1);
            if (!changesetIdRange.MoveNext()) throw new ApplicationException("There is no ID reserved for the P-S-T changeset.");
            var changesetId = changesetIdRange.Current;


            var changesetInfo = new BuPortfolioSecurityTargetChangesetInfo(
                changesetId,
                changeset.Username,
                DateTime.Now, /* is going to be ignored */
                calculationId
            );
            manager.InsertPortfolioSecurityTargetChangeset(changesetInfo);

            var changes = changeset.Changes;
            var changeIdRange = manager.ReservePortfolioSecurityTargetChangeIds(changes.Count());

            foreach (var change in changes)
            {
                if (!changeIdRange.MoveNext()) throw new ApplicationException("There is no ID reserved for the P-S-T change.");
                var changeId = changeIdRange.Current;

                var resolver = new Apply_IPstChangeResolver(this, manager, changesetId, changeId);
                change.Accept(resolver);
            }
        }

        private class Apply_IPstChangeResolver : IPstChangeResolver
        {
            private ChangesetApplier applier;
            private IDataManager manager;
            private Int32 changesetId;
            private Int32 changeId;

            public Apply_IPstChangeResolver(ChangesetApplier applier, IDataManager manager, Int32 changesetId, Int32 changeId)
            {
                this.applier = applier;
                this.manager = manager;
                this.changesetId = changesetId;
                this.changeId = changeId;
            }

            public void Resolve(PstUpdateChange change)
            {
                this.applier.ApplyUpdateChange(change, this.manager, this.changesetId, this.changeId);
            }

            public void Resolve(PstDeleteChange change)
            {
                this.applier.ApplyDeleteChange(change, this.manager, this.changesetId, this.changeId);
            }

            public void Resolve(PstInsertChange change)
            {
                this.applier.ApplyInsertChange(change, this.manager, this.changesetId, this.changeId);
            }
        }

        protected void ApplyUpdateChange(PstUpdateChange change, IDataManager manager, Int32 changesetId, Int32 changeId)
        {
            var changeInfo = new BuPortfolioSecurityTargetChangeInfo
            (
                changeId,
                change.PortfolioId,
                change.SecurityId,
                change.TargetBefore,
                change.TargetAfter,
                changesetId,
                change.Comment
            );
            manager.InsertPortfolioSecurityTargetChange(changeInfo);

            var pstInfo = new BuPortfolioSecurityTargetInfo(
                change.PortfolioId,
                change.SecurityId,
                change.TargetAfter,
                changeId
            );
            manager.UpdatePortfolioSecurityTarget(pstInfo);
        }

        protected void ApplyInsertChange(PstInsertChange change, IDataManager manager, Int32 changesetId, Int32 changeId)
        {
            var changeInfo = new BuPortfolioSecurityTargetChangeInfo
            (
                changeId,
                change.PortfolioId,
                change.SecurityId,
                null,
                change.TargetAfter,
                changesetId,
                change.Comment
            );
            manager.InsertPortfolioSecurityTargetChange(changeInfo);

            var pstInfo = new BuPortfolioSecurityTargetInfo(
                change.PortfolioId,
                change.SecurityId,
                change.TargetAfter,
                changeId
            );
            manager.InsertPortfolioSecurityTarget(pstInfo);
        }

        protected void ApplyDeleteChange(PstDeleteChange change, IDataManager manager, Int32 changesetId, Int32 changeId)
        {
            var changeInfo = new BuPortfolioSecurityTargetChangeInfo
            (
                changeId,
                change.PortfolioId,
                change.SecurityId,
                change.TargetBefore,
                null,
                changesetId,
                change.Comment
            );
            manager.InsertPortfolioSecurityTargetChange(changeInfo);

            manager.DeletePortfolioSecurityTarget(change.PortfolioId, change.SecurityId);
        }
    }
}
