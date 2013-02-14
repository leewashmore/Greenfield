using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;
using System.Net.Mail;
using Aims.Core;
using TopDown.Core.ManagingBaskets;

namespace TopDown.Core.ManagingBpst.ChangingBpst
{
    public class PortfolioTargetChangesetApplier : ChangesetApplierBase<Changeset, IChange>
    {
        protected override IEnumerable<IChange> GetChanges(Changeset changeset)
        {
            return changeset.Changes;
        }

        protected override void MakeSureThereWereNoChangesSince(Changeset changeset, IDataManager manager)
        {
            var latestChangeset = manager.GetLatestBasketPortfolioSecurityTargetChangeset();
            if (changeset.LatestChangesetSnapshot.Id < latestChangeset.Id)
            {
                throw new ValidationException(
                    new ErrorIssue("User \"" + latestChangeset.Username + "\" modified the B-P-S-T composition on " + latestChangeset.Timestamp + ".")
                );
            }
        }

        protected override void ApplyChangeset(Changeset changeset, Int32 changesetId, Int32 computationId, IDataManager manager)
        {
            var changesetInfo = new BasketPortfolioSecurityTargetChangesetInfo(
                changesetId,
                changeset.Username,
                DateTime.Now, // <---- will be ignored
                computationId
            );
            manager.InsertBasketPortfolioSecurityTargetChangeset(changesetInfo);
        }

        public List<String> PrepareToSend(Changeset changeset, IDataManager manager, SecurityRepository securityRepository)
        {
            
            var result = new List<String>();
            if (changeset != null)
            {
                var date = DateTime.Now;
                foreach (var change in changeset.Changes)
                {
                    var resolver = new Mail_IChangeResolver(this, manager, securityRepository, result, changeset.Username, date);
                    change.Accept(resolver);
                }
            }
            
            return result;
        }


        private class Mail_IChangeResolver : IChangeResolver
        {
            private PortfolioTargetChangesetApplier applier;
            private IDataManager manager;
            private List<String> mail;
            private String username;
            private DateTime date;
            private SecurityRepository securityRepository;

            public Mail_IChangeResolver(PortfolioTargetChangesetApplier applier, IDataManager manager, SecurityRepository securityRepository, List<String> mail, String username, DateTime date)
            {
                this.applier = applier;
                this.manager = manager;
                this.mail = mail;
                this.username = username;
                this.date = date;
                this.securityRepository = securityRepository;
            }

            public void Resolve(DeleteChange change)
            {
                this.applier.MailDeleteChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.MailInsertChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.MailUpdateChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date);
            }
        }

        internal void MailUpdateChange(UpdateChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var security = securityRepository.FindSecurity(change.SecurityId);
            bodyAppendix.AppendLine("Adjustment for Portfolio " + change.PortfolioId + " of " + security.Name + "(" + security.ShortName + ") from " + change.TargetBefore*100 + " to " + change.TargetAfter*100);
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailInsertChange(InsertChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var security = securityRepository.FindSecurity(change.SecurityId);
            bodyAppendix.AppendLine("Adjustment for Portfolio " + change.PortfolioId + ": " + security.Name + "(" + security.ShortName + ") was added to " + change.TargetAfter*100);
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailDeleteChange(DeleteChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var security = securityRepository.FindSecurity(change.SecurityId);
            bodyAppendix.AppendLine("Adjustment for Portfolio " + change.PortfolioId + ": " + security.Name + "(" + security.ShortName + ") was removed [last target was " + change.TargetBefore*100 + "]");
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }
        

        protected override IEnumerator<Int32> RequestChangeIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveBasketPortfolioSecurityTargetChangeIds(howMany);
            return result;
        }

        protected override IEnumerator<Int32> RequestChangesetIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveBasketPortfolioSecurityTargetChangesetIds(howMany);
            return result;
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
            private PortfolioTargetChangesetApplier applier;

            public ApplyChangeOnceResolved_IChangeResolver(PortfolioTargetChangesetApplier applier, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
            {
                this.manager = manager;
                this.changesetId = changesetId;
                this.changeId = changeId;
                this.changeset = changeset;
                this.applier = applier;
            }

            public void Resolve(DeleteChange change)
            {
                this.applier.ApplyDeleteChange(change, changeset, changeId, changesetId, manager);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.ApplyInsertChange(change, changeset, changeId, changesetId, manager);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.ApplyUpdateChange(change, changeset, changeId, changesetId, manager);
            }
        }

        protected void ApplyInsertChange(InsertChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new BasketPortfolioSecurityTargetChangeInfo(
                changeId,
                changeset.BasketId,
                change.PortfolioId,
                change.SecurityId,
                null,
                change.TargetAfter,
                changesetId,
				change.Comment
            );
            manager.InsertBasketPortfolioSecurityTargetChange(changeInfo);

            var info = new BasketPortfolioSecurityTargetInfo(
                changeset.BasketId,
                change.PortfolioId,
                change.SecurityId,
                change.TargetAfter,
                changeId
            );
            manager.InsertBasketPortfolioSecurityTarget(info);
        }

        protected void ApplyUpdateChange(UpdateChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new BasketPortfolioSecurityTargetChangeInfo(
                changeId,
                changeset.BasketId,
                change.PortfolioId,
                change.SecurityId,
                change.TargetBefore,
                change.TargetAfter,
                changesetId,
				change.Comment
            );
            manager.InsertBasketPortfolioSecurityTargetChange(changeInfo);

            var info = new BasketPortfolioSecurityTargetInfo
            (
                changeset.BasketId,
                change.PortfolioId,
                change.SecurityId,
                change.TargetAfter,
                changeId
            );
            manager.UpdateBasketPortfolioSecurityTarget(info);
        }

        protected void ApplyDeleteChange(DeleteChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new BasketPortfolioSecurityTargetChangeInfo(
                changeId,
                changeset.BasketId,
                change.PortfolioId,
                change.SecurityId,
                change.TargetBefore,
                null,
                changesetId,
				change.Comment
            );
            manager.InsertBasketPortfolioSecurityTargetChange(changeInfo);
            manager.DeleteBasketPortfolioSecurityTarget(changeset.BasketId, change.PortfolioId, change.SecurityId);
        }

        
    }
}
