using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;
using TopDown.Core.ManagingBaskets;
using Aims.Core;

namespace TopDown.Core.ManagingBpt.ChangingTtbbv
{
    public class ChangesetApplier : ChangesetApplierBase<Changeset, IChange>
    {
        protected override IEnumerable<IChange> GetChanges(Changeset changeset)
        {
            return changeset.Changes;
        }

        protected override void MakeSureThereWereNoChangesSince(Changeset changeset, IDataManager manager)
        {
            var latestChangeset = manager.GetLatestTargetingTypeBasketBaseValueChangeset();
            if (changeset.LatestChangesetSnapshot.Id < latestChangeset.Id)
            {
                throw new ValidationException(
                    new ErrorIssue("User \"" + latestChangeset.Username + "\" modified the TT-B-Bv composition on " + latestChangeset.Timestamp + ".")
                );
            }
        }

        protected override IEnumerator<Int32> RequestChangesetIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveTargetingTypeBasketBaseValueChangesetIds(howMany);
            return result;
        }

        protected override void ApplyChangeset(Changeset changeset, Int32 changesetId, Int32 computationId, IDataManager manager)
        {
            var changesetInfo = new TargetingTypeBasketBaseValueChangesetInfo(
                changesetId,
                changeset.Username,
                DateTime.Now, // <--------- will be ignored
                computationId
            );
            manager.InsertTargetingTypeBasketBaseValueChangeset(changesetInfo);
        }

        protected override IEnumerator<Int32> RequestChangeIds(Int32 howMany, IDataManager manager)
        {
            var result = manager.ReserveTargetingTypeBasketBaseValueChangeIds(howMany);
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

            public ApplyChange_IChangeResolver(
                ChangesetApplier applier,
                Changeset changeset,
                Int32 changeId,
                Int32 changesetId,
                IDataManager manager
            )
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

            public void Resolve(UpdateChange change)
            {
                this.applier.ApplyUpdateChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.ApplyInsertChange(change, this.changeset, this.changeId, this.changesetId, this.manager);
            }
        }

        public void ApplyInsertChange(InsertChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeBasketBaseValueChangeInfo
            (
                changeId,
                changeset.TargetingTypeId,
                change.BasketId,
                null,
                change.BaseValueAfter,
                changesetId,
                change.Comment
            );
            manager.InsertTargetingTypeBasketBaseValueChange(changeInfo);

            var info = new TargetingTypeBasketBaseValueInfo(
                changeset.TargetingTypeId,
                change.BasketId,
                change.BaseValueAfter,
                changeId
            );
            manager.InsertTargetingTypeBasketBaseValue(info);
        }

        public void ApplyUpdateChange(UpdateChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeBasketBaseValueChangeInfo
            (
                changeId,
                changeset.TargetingTypeId,
                change.BasketId,
                change.BaseValueBefore,
                change.BaseValueAfter,
                changesetId,
                change.Comment
            );
            manager.InsertTargetingTypeBasketBaseValueChange(changeInfo);

            var info = new TargetingTypeBasketBaseValueInfo(
                changeset.TargetingTypeId,
                change.BasketId,
                change.BaseValueAfter,
                changeId
            );
            manager.UpdateTargetingTypeBasketBaseValue(info);
        }

        public void ApplyDeleteChange(DeleteChange change, Changeset changeset, Int32 changeId, Int32 changesetId, IDataManager manager)
        {
            var changeInfo = new TargetingTypeBasketBaseValueChangeInfo
            (
                changeId,
                changeset.TargetingTypeId,
                change.BasketId,
                change.BaseValueBefore,
                null,
                changesetId,
                change.Comment
            );
            manager.InsertTargetingTypeBasketBaseValueChange(changeInfo);
            manager.DeleteTargetingTypeBasketBaseValue(changeset.TargetingTypeId, change.BasketId);
        }

        internal List<String> PrepareToSend(Changeset changeset, IDataManager manager, Aims.Core.SecurityRepository securityRepository, ManagingBaskets.BasketRepository basketRepository, string ttName)
        {
            var result = new List<String>();
            if (changeset != null)
            {
                var date = DateTime.Now;
                foreach (var change in changeset.Changes)
                {
                    var resolver = new Mail_IChangeResolver(this, manager, securityRepository, basketRepository, result, changeset.Username, date, ttName);
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
            private String ttName;
            private BasketRepository basketRepository;


            public Mail_IChangeResolver(ChangesetApplier applier, IDataManager manager, SecurityRepository securityRepository, BasketRepository basketRepository, List<String> mail, String username, DateTime date, String ttName)
            {
                this.applier = applier;
                this.manager = manager;
                this.mail = mail;
                this.username = username;
                this.date = date;
                this.securityRepository = securityRepository;
                this.ttName = ttName;
                this.basketRepository = basketRepository;
            }

            public void Resolve(DeleteChange change)
            {
                this.applier.MailDeleteChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date, this.ttName, this.basketRepository);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.MailInsertChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date, this.ttName, this.basketRepository);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.MailUpdateChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date, this.ttName, this.basketRepository);
            }


        }

        internal void MailUpdateChange(UpdateChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date, String ttName, BasketRepository basketRepository)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var basket = basketRepository.GetBasket(change.BasketId);
            string basketName = "";
            if (basket.TryAsCountryBasket() != null)
            {
                basketName = basket.AsCountryBasket().Country.Name;
            }
            else
            {
                basketName = basket.AsRegionBasket().Name;
            }
            bodyAppendix.AppendLine("BASE Adjustment in " + basketName + " for " + ttName + " from " + change.BaseValueBefore + " to " + change.BaseValueAfter);
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailInsertChange(InsertChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date, String ttName, BasketRepository basketRepository)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var basket = basketRepository.GetBasket(change.BasketId);
            string basketName = "";
            if (basket.TryAsCountryBasket() != null)
            {
                basketName = basket.AsCountryBasket().Country.Name;
            }
            else
            {
                basketName = basket.AsRegionBasket().Name;
            }
            bodyAppendix.AppendLine("BASE Adjustment in " + basketName + " for " + ttName + " from [empty] to " + change.BaseValueAfter);
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailDeleteChange(DeleteChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date, String ttName, BasketRepository basketRepository)
        {
            StringBuilder bodyAppendix = new StringBuilder("\n");
            bodyAppendix.AppendLine("---" + date + ", Approved by: " + username + "---");
            var basket = basketRepository.GetBasket(change.BasketId);
            string basketName = "";
            if (basket.TryAsCountryBasket() != null)
            {
                basketName = basket.AsCountryBasket().Country.Name;
            }
            else
            {
                basketName = basket.AsRegionBasket().Name;
            }
            bodyAppendix.AppendLine("BASE Adjustment in " + basketName + " for " + ttName + " from " + change.BaseValueBefore + " to [empty]");
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }
    }
}
