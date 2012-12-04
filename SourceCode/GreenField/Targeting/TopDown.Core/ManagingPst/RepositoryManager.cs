using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    public class RepositoryManager
    {
        public const String StorageKey = "BottomUpPortfolioSecurityTargetRepository";
        private IStorage<PortfolioSecurityTargetRepository> storage;
        private InfoCopier copier;

        [DebuggerStepThrough]
        public RepositoryManager(InfoCopier copier, IStorage<PortfolioSecurityTargetRepository> storage)
        {
            this.copier = copier;
            this.storage = storage;
        }

        public PortfolioSecurityTargetRepository ClaimRepository(
            BuPortfolioSecurityTargetChangesetInfo latestChangesetSnapshoot,
            IDataManager manager
        )
        {
            var found = this.storage[StorageKey];
            if (found != null)
            {
                var latestChangeset = manager.GetLatestPortfolioSecurityTargetChangeSet();
                if (latestChangesetSnapshoot.Id < latestChangeset.Id)
                {
                    throw new ValidationException(
                        new ValidationIssue("User \"" + latestChangeset.Username + "\" modified the bottom-up-portfolio/security/target composition on " + latestChangeset.Timestamp + ".")
                    );
                }
                else
                {
                    return found;
                }
            }
            else
            {
                var targetInfos = manager.GetAllPortfolioSecurityTargets();
                found = new PortfolioSecurityTargetRepository(this.copier, targetInfos);
                this.storage[StorageKey] = found;
                return found;
            }
        }

        public void DropRepository()
        {
            this.storage[StorageKey] = null;
        }
    }
}
