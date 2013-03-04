using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using System.Data.SqlClient;
using Aims.Expressions;
using Aims.Core;
using TopDown.Core.Helpers;

namespace TopDown.Core.ManagingBpst.ChangingTtgbsbv
{
    public class ChangesetApplier : ChangesetApplierBase<Changeset, IChange>
    {
        protected override IEnumerable<IChange> GetChanges(Changeset changeset)
        {
            return changeset.Changes;
        }

        protected override void MakeSureThereWereNoChangesSince(Changeset changeset, IDataManager manager)
        {
            var latestChangeset = manager.GetLatestTargetingTypeGroupBasketSecurityBaseValueChangeset();
            if (changeset.LatestChangesetSnapshot.Id < latestChangeset.Id)
            {
                throw new ValidationException(
                    new ErrorIssue("User \"" + latestChangeset.Username + "\" modified the TTG-B-S-Bv composition on " + latestChangeset.Timestamp + ".")
                );
            }
        }

        protected override void ApplyChangeset(Changeset changeset, Int32 changesetId, Int32 computationId, IDataManager manager)
        {
            var changesetInfo = new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(
                changesetId,
                changeset.Username,
                DateTime.Now, // <-- is going to be ignored
                computationId
            );
            manager.InsertTargetingTypeGroupBasketSecurityBaseValueChangeset(changesetInfo);
        }

        protected override IEnumerator<Int32> RequestChangeIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveTargetingTypeGroupBasketSecurityBaseValueChangeIds(howMany);
            return result;
        }

        protected override IEnumerator<Int32> RequestChangesetIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveTargetingTypeGroupBasketSecurityBaseValueChangesetIds(howMany);
            return result;
        }

        protected override void ApplyChangeOnceResolved(IChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var resolver = new Apply_IBaseChangeResolver(this, manager, changeset, changesetId, changeId);
            change.Accept(resolver);
        }

        private class Apply_IBaseChangeResolver : IChangeResolver
        {
            private ChangesetApplier applier;
            private IDataManager manager;
            private Int32 changesetId;
            private Int32 changeId;
            private Changeset changeset;

            public Apply_IBaseChangeResolver(ChangesetApplier applier, IDataManager manager, Changeset changeset, Int32 changesetId, Int32 changeId)
            {
                this.applier = applier;
                this.manager = manager;
                this.changeset = changeset;
                this.changesetId = changesetId;
                this.changeId = changeId;
            }

            public void Resolve(InsertChange change)
            {
                this.applier.ApplyInsertChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(DeleteChange change)
            {
                this.applier.ApplyDeleteChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.ApplyUpdateChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }
        }

        protected void ApplyInsertChange(InsertChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeGroupBasketSecurityBaseValueChangeInfo
            (
                changeId,
                changeset.TargetingTypeGroupId,
                changeset.BasketId,
                change.SecurityId,
                null,
                change.BaseValueAfter,
                changesetId,
                change.Comment
            );
            manager.InsertTargetingTypeGroupBasketSecurityBaseValueChange(changeInfo);

            var info = new TargetingTypeGroupBasketSecurityBaseValueInfo(
                changeset.TargetingTypeGroupId,
                changeset.BasketId,
                change.SecurityId,
                change.BaseValueAfter,
                changeId
            );
            manager.InsertTargetingTypeGroupBasketSecurityBaseValue(info);
        }

        protected void ApplyDeleteChange(DeleteChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeGroupBasketSecurityBaseValueChangeInfo
            (
                changeId,
                changeset.TargetingTypeGroupId,
                changeset.BasketId,
                change.SecurityId,
                change.BaseValueBefore,
                null,
                changesetId,
                change.Comment
            );
            manager.InsertTargetingTypeGroupBasketSecurityBaseValueChange(changeInfo);
            manager.DeleteTargetingTypeGroupBasketSecurityBaseValue(changeset.TargetingTypeGroupId, changeset.BasketId, change.SecurityId);
        }

        protected void ApplyUpdateChange(UpdateChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeGroupBasketSecurityBaseValueChangeInfo
            (
                changeId,
                changeset.TargetingTypeGroupId,
                changeset.BasketId,
                change.SecurityId,
                change.BaseValueBefore,
                change.BaseValueAfter,
                changesetId,
                change.Comment
            );
            manager.InsertTargetingTypeGroupBasketSecurityBaseValueChange(changeInfo);

            var info = new TargetingTypeGroupBasketSecurityBaseValueInfo(
               changeset.TargetingTypeGroupId,
               changeset.BasketId,
               change.SecurityId,
               change.BaseValueAfter,
               changeId
           );
            manager.UpdateTargetingTypeGroupBasketSecurityBaseValue(info);
        }



        internal List<String> PrepareToSend(Changeset changeset, IDataManager manager, Aims.Core.SecurityRepository securityRepository)
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
            private ChangesetApplier applier;
            private IDataManager manager;
            private List<String> mail;
            private String username;
            private DateTime date;
            private SecurityRepository securityRepository;

            public Mail_IChangeResolver(ChangesetApplier applier, IDataManager manager, SecurityRepository securityRepository, List<String> mail, String username, DateTime date)
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
            bodyAppendix.AppendLine("Adjustment for BASE of " + security.Name + "(" + security.ShortName + ") from " + MailSender.TransformTargetToString(change.BaseValueBefore) + " to " + MailSender.TransformTargetToString(change.BaseValueAfter));
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailInsertChange(InsertChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var security = securityRepository.FindSecurity(change.SecurityId);
            bodyAppendix.AppendLine("Adjustment for BASE: " + security.Name + "(" + security.ShortName + ") was added to " + MailSender.TransformTargetToString(change.BaseValueAfter));
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailDeleteChange(DeleteChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var security = securityRepository.FindSecurity(change.SecurityId);
            bodyAppendix.AppendLine("Adjustment for BASE: " + security.Name + "(" + security.ShortName + ") was removed [last target was " + MailSender.TransformTargetToString(change.BaseValueBefore) + "]");
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }
    }
}
