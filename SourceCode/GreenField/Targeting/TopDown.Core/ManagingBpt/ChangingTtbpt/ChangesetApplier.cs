using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Expressions;
using Aims.Core;
using TopDown.Core.ManagingBaskets;

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
                    new ErrorIssue("User \"" + latestChangeset.Username + "\" modified the TT-B-P-T composition on " + latestChangeset.Timestamp + ".")
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

        internal object PrepareToSend(ChangingTtbpt.Changeset changeset, IDataManager manager, Aims.Core.SecurityRepository securityRepository, ManagingBaskets.BasketRepository basketRepository, String ttName, String portfolioName)
        {
            var result = new List<String>();
            if (changeset != null)
            {
                var date = DateTime.Now;
                foreach (var change in changeset.Changes)
                {
                    var resolver = new Mail_IChangeResolver(this, manager, securityRepository, basketRepository, result, changeset.Username, date, ttName, portfolioName);
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
            private String portfolioName;


            public Mail_IChangeResolver(ChangesetApplier applier, IDataManager manager, SecurityRepository securityRepository, BasketRepository basketRepository, List<String> mail, String username, DateTime date, String ttName, String portfolioName)
            {
                this.applier = applier;
                this.manager = manager;
                this.mail = mail;
                this.username = username;
                this.date = date;
                this.securityRepository = securityRepository;
                this.ttName = ttName;
                this.basketRepository = basketRepository;
                this.portfolioName = portfolioName;
            }

            public void Resolve(DeleteChange change)
            {
                this.applier.MailDeleteChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date, this.ttName, this.basketRepository, this.portfolioName);
            }

            public void Resolve(InsertChange change)
            {
                this.applier.MailInsertChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date, this.ttName, this.basketRepository, this.portfolioName);
            }

            public void Resolve(UpdateChange change)
            {
                this.applier.MailUpdateChange(change, this.manager, this.securityRepository, this.mail, this.username, this.date, this.ttName, this.basketRepository, this.portfolioName);
            }


        }

        internal void MailUpdateChange(UpdateChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date, String ttName, BasketRepository basketRepository, String portfolioName)
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
            bodyAppendix.AppendLine(portfolioName + " Adjustment in " + basketName + " for " + ttName + " from " + change.TargetBefore*100 + " to " + change.TargetAfter*100);
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailInsertChange(InsertChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date, String ttName, BasketRepository basketRepository, String portfolioName)
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
            bodyAppendix.AppendLine(portfolioName + " Adjustment in " + basketName + " for " + ttName + " from [empty] to " + change.TargetAfter*100);
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }

        internal void MailDeleteChange(DeleteChange change, IDataManager dataManager, SecurityRepository securityRepository, List<String> mailMessage, String username, DateTime date, String ttName, BasketRepository basketRepository, String portfolioName)
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
            bodyAppendix.AppendLine(portfolioName + " Adjustment in " + basketName + " for " + ttName + " from " + change.TargetBefore*100 + " to [empty]");
            bodyAppendix.AppendLine("COMMENT: " + change.Comment);
            mailMessage.Add(bodyAppendix.ToString());
        }
    }
}