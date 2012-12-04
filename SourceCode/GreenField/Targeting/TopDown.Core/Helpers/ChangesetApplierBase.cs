using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;

namespace TopDown.Core
{
	public abstract class ChangesetApplierBase<TChangeset, TChange>
	{
		public void Apply(Int32 computationId, TChangeset changeset, IDataManager manager)
		{
            this.MakeSureThereWereNoChangesSince(changeset, manager);

            var chagesetIdRange = this.RequestChangesetIds(1, manager);
            if (!chagesetIdRange.MoveNext()) throw new ApplicationException("Unable to reserve an ID for the changeset.");
            var changesetId = chagesetIdRange.Current;

            this.ApplyChangeset(changeset, changesetId, computationId, manager);

            var changes = this.GetChanges(changeset);
            var changeCount = changes.Count();
            var changeIdRange = this.RequestChangeIds(changeCount, manager);

            foreach (var change in changes)
            {
                if (!changeIdRange.MoveNext()) throw new ApplicationException("Unable to reserve an ID for a change.");
                var changeId = changeIdRange.Current;
                this.ApplyChangeOnceResolved(change, changeset, changeId, changesetId, manager);
            }
		}

        protected abstract IEnumerable<TChange> GetChanges(TChangeset changeset);

        protected abstract void MakeSureThereWereNoChangesSince(TChangeset changeset, IDataManager manager);

        protected abstract IEnumerator<Int32> RequestChangesetIds(Int32 howMany, IDataManager manager);
        
        protected abstract void ApplyChangeset(TChangeset changeset, Int32 changesetId, Int32 computationId, IDataManager manager);

        protected abstract IEnumerator<Int32> RequestChangeIds(Int32 howMany, IDataManager manager);

        protected abstract void ApplyChangeOnceResolved(TChange change, TChangeset changeset, int changeId, int changesetId, IDataManager manager);
	}
}
