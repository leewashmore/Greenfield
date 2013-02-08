using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using Aims.Expressions;
using TopDown.Core.ManagingCalculations;
using Aims.Core;
using TopDown.Core.ManagingBaskets;
using System.Net.Mail;
using TopDown.Core.Helpers;

namespace TopDown.Core.ManagingBpst
{
    public class ModelApplier
    {
        private IDataManagerFactory dataManagerFactory;
        private ChangingTtgbsbv.ChangesetApplier ttgbsbvChangesetApplier;
        private ChangingTtgbsbv.ModelToChangesetTransformer ttgbsbvModelTransformer;
        private ChangingBpst.PortfolioTargetChangesetApplier bpstChangesetApplier;
        private ChangingBpst.ModelToChangesetTransformter bpstModelTransformter;
        private ManagingCalculations.CalculationRequester calculationRequester;
        private ModelValidator modelValidator;
        private RepositoryManager repositoryManager;
        
        public ModelApplier(
            IDataManagerFactory dataManagerFactory,
            ChangingTtgbsbv.ChangesetApplier ttgbsbvChangesetApplier,
            ChangingTtgbsbv.ModelToChangesetTransformer ttgbsbvModelTransformer,
			ChangingBpst.PortfolioTargetChangesetApplier bpstChangesetApplier,
            ChangingBpst.ModelToChangesetTransformter bpstModelTransformter,
            ManagingCalculations.CalculationRequester calculationRequester,
            ModelValidator modelValidator,
            RepositoryManager repositoryManager
        )
        {
            this.dataManagerFactory = dataManagerFactory;
            this.bpstChangesetApplier = bpstChangesetApplier;
            this.ttgbsbvChangesetApplier = ttgbsbvChangesetApplier;
            this.ttgbsbvModelTransformer = ttgbsbvModelTransformer;
            this.bpstModelTransformter = bpstModelTransformter;
            this.calculationRequester = calculationRequester;
            this.modelValidator = modelValidator;
            this.repositoryManager = repositoryManager;
        }


        protected IEnumerable<IValidationIssue> Validate(RootModel model, CalculationTicket ticket)
        {
            var issues = this.modelValidator.ValidateRoot(model, ticket);
            return issues;
        }

        public void Apply(RootModel model, String username, String userEmail, SqlConnection connection, SecurityRepository securityRepository, BasketRepository basketRepository,  ref CalculationInfo calculationInfo)
        {
            var ttgbsbvChangesetOpt = this.ttgbsbvModelTransformer.TryTransformToChangeset(username, model);
            var bpstChangesetOpt = this.bpstModelTransformter.TryTransformToChangeset(username, model);
            if (ttgbsbvChangesetOpt == null && bpstChangesetOpt == null)
            {
                // there's nothing to apply, why bother?
                return;
            }
            
            using (var transaction = connection.BeginTransaction())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection, transaction);

                calculationInfo = this.calculationRequester.RequestCalculation(manager);

                if (ttgbsbvChangesetOpt != null)
                {
                    this.ttgbsbvChangesetApplier.Apply(calculationInfo.Id, ttgbsbvChangesetOpt, manager);
                }
                if (bpstChangesetOpt != null)
                {
                    this.bpstChangesetApplier.Apply(calculationInfo.Id, bpstChangesetOpt, manager);
                   
                }
                SendNotification(ttgbsbvChangesetOpt, bpstChangesetOpt, manager, securityRepository, basketRepository, userEmail);
                transaction.Commit();
            }

            if (bpstChangesetOpt != null)
            {
                this.repositoryManager.DropBasketSecurityPortfolioTargetRepository();
            }

            if (ttgbsbvChangesetOpt != null)
            {
                this.repositoryManager.DropTargetingTypeGroupBasketSecurityBaseValueRepository();
            }
        }

        public void SendNotification(ChangingTtgbsbv.Changeset ttgbsbvChangeset, ChangingBpst.Changeset bpstChangeset, IDataManager manager, SecurityRepository securityRepository, BasketRepository basketRepository, String userEmail)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = false;
            int basketId;
            if (ttgbsbvChangeset != null)
            {
                basketId = ttgbsbvChangeset.BasketId;
            }
            else
            {
                basketId = bpstChangeset.BasketId;
            }
            var basket = basketRepository.GetBasket(basketId);
            string basketName = basket.TryAsCountryBasket() != null ? basket.AsCountryBasket().Country.Name : basket.AsRegionBasket().Name;
            
            var bpstChanges = this.bpstChangesetApplier.PrepareToSend(bpstChangeset, manager, securityRepository);
            var ttgbsbvChanges = this.ttgbsbvChangesetApplier.PrepareToSend(ttgbsbvChangeset, manager, securityRepository);


            mail.Body = "The following changes were made to the " + basketName + "\n" + (ttgbsbvChangeset != null ? String.Join("\n", ttgbsbvChanges) : "\n") + ( bpstChangeset != null ? String.Join("\n", bpstChanges) : "");
            mail.Subject = "Targeting: Stock Selection changes in " + basketName;
            MailSender.SendTargetingAlert(mail, userEmail);
        }

    }
}
