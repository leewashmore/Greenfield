using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.Persisting;
using Aims.Expressions;
using TopDown.Core.ManagingCalculations;
using Aims.Core;
using TopDown.Core.ManagingBaskets;
using System.Net.Mail;
using TopDown.Core.Helpers;

namespace TopDown.Core.ManagingBpt
{
	public class ModelApplier
	{
		private ChangingBt.ChangesetApplier btChangesetApplier;
		private ChangingBt.ModelToChangesetTransformer btModelTransformer;
		private ChangingPsto.ChangesetApplier pstoChangesetApplier;
		private ChangingPsto.ModelToChangesetTransformer pstoModelTransformer;
		private ChangingTtbbv.ChangesetApplier ttbbvChangesetApplier;
		private ChangingTtbbv.ModelToChangesetTransformer ttbbvModelTransformer;
		private ChangingTtbpt.ChangesetApplier ttbptChangesetApplier;
		private ChangingTtbpt.ModelToChangesetTransformer ttbptModelTransformer;
		private ModelValidator modelValidator;
        private IDataManagerFactory dataManagerFactory;
        private ManagingCalculations.CalculationRequester calculationRequester;

		public ModelApplier(
			ChangingBt.ChangesetApplier btChangesetApplier,
			ChangingBt.ModelToChangesetTransformer btModelTransformer,
			ChangingPsto.ChangesetApplier pstoChangesetApplier,
			ChangingPsto.ModelToChangesetTransformer pstoModelTransformer,
			ChangingTtbbv.ChangesetApplier ttbbvChangesetApplier,
			ChangingTtbbv.ModelToChangesetTransformer ttbbvModelTransformer,
			ChangingTtbpt.ChangesetApplier ttbptChangesetApplier,
			ChangingTtbpt.ModelToChangesetTransformer ttbptModelTransformer,
			ModelValidator modelValidator,
            IDataManagerFactory dataManagerFactory,
            ManagingCalculations.CalculationRequester calculationRequester
		)
		{
			this.btChangesetApplier = btChangesetApplier;
			this.btModelTransformer = btModelTransformer;
			this.pstoChangesetApplier = pstoChangesetApplier;
			this.pstoModelTransformer = pstoModelTransformer;
			this.ttbbvChangesetApplier = ttbbvChangesetApplier;
			this.ttbbvModelTransformer = ttbbvModelTransformer;
			this.ttbptChangesetApplier = ttbptChangesetApplier;
			this.ttbptModelTransformer = ttbptModelTransformer;
			this.modelValidator = modelValidator;
            this.dataManagerFactory = dataManagerFactory;
            this.calculationRequester = calculationRequester;
		}

		public IEnumerable<IValidationIssue> ApplyIfValid(
			RootModel model,
			Taxonomy taxonomy,
			RepositoryManager repositoryManager,
			String username,
            String userEmail,
			SqlConnection connection,
            SecurityRepository securityRepository,
            BasketRepository basketRepository,
            PortfolioRepository portfolioRepository,
            CalculationTicket ticket,
            ref CalculationInfo info
		)
		{
            var issues = this.ValidateModelAndPermissions(model, username, ticket);
            var traverser = new IssueTraverser();
            var traversedIssues = traverser.TraverseAll(issues);
			if (traversedIssues.Any(x => x is ErrorIssue)) return issues;

			try
			{
				this.Apply(model, taxonomy, repositoryManager, username, userEmail, connection, 
                    securityRepository, basketRepository,  portfolioRepository, ref info);
				return issues;
			}
			catch (ValidationException exception)
			{
                return issues.Union(new IValidationIssue[] { exception.Issue });
			}
		}

		protected void Apply(
			RootModel model,
			Taxonomy taxonomy,
			RepositoryManager repositoryManager,
			String username,
            String userEmail,
			SqlConnection connection,
            SecurityRepository securityRepository, 
            BasketRepository basketRepository,
            PortfolioRepository portfolioRepository,
            ref CalculationInfo calculationInfo
		)
		{
			using (var transaction = connection.BeginTransaction())
			{
                var manager = this.dataManagerFactory.CreateDataManager(connection, transaction);

				var btChangesetOpt = this.btModelTransformer.TryTransformToChangeset(model, taxonomy);
				if (btChangesetOpt != null)
				{
					// saving unsaved baskets and modified taxonomy
					this.btChangesetApplier.Apply(btChangesetOpt, connection, transaction, repositoryManager);
                    

                }

				var pstoChangesetOpt = this.pstoModelTransformer.TryTransformToChangeset(model, username);
				var ttbbvChangesetOpt = this.ttbbvModelTransformer.TryTransformToChangeset(model, username);
				var ttbptChangesetOpt = this.ttbptModelTransformer.TryTransformToChangeset(model, username);

				if (pstoChangesetOpt == null && ttbbvChangesetOpt == null && ttbptChangesetOpt == null)
				{
					return; // no changes to apply, why bother?
				}

                calculationInfo = this.calculationRequester.RequestCalculation(manager);

				if (pstoChangesetOpt != null)
				{
                    this.pstoChangesetApplier.Apply(calculationInfo.Id, pstoChangesetOpt, manager);
				}
				if (ttbbvChangesetOpt != null)
				{
                    this.ttbbvChangesetApplier.Apply(calculationInfo.Id, ttbbvChangesetOpt, manager);
				}
				if (ttbptChangesetOpt != null)
				{
                    this.ttbptChangesetApplier.Apply(calculationInfo.Id, ttbptChangesetOpt, manager);
				}

                SendNotification(model.TargetingType.Name, model.Portfolio.Name, pstoChangesetOpt, ttbbvChangesetOpt, ttbptChangesetOpt, manager, securityRepository, basketRepository, portfolioRepository, userEmail);
//#warning HACK!!! prevent test changes to DB
				//transaction.Rollback();
                transaction.Commit();
			}
		}

        private void SendNotification(String ttName, String portfolioName, ChangingPsto.Changeset pstoChangeset, ChangingTtbbv.Changeset ttbbvChangeset, ChangingTtbpt.Changeset ttbptChangeset, IDataManager manager, SecurityRepository securityRepository, BasketRepository basketRepository, PortfolioRepository portfolioRepository, string userEmail)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = false;

            var pstoChanges = this.pstoChangesetApplier.PrepareToSend(pstoChangeset, manager, securityRepository, portfolioRepository, portfolioName);
            var ttbbvChanges = this.ttbbvChangesetApplier.PrepareToSend(ttbbvChangeset, manager, securityRepository, basketRepository, ttName);
            var ttbptChanges = this.ttbptChangesetApplier.PrepareToSend(ttbptChangeset, manager, securityRepository, basketRepository, ttName, portfolioName);


            mail.Body = "The following Asset Allocation changes were made to the Global Active Accounts:\n" + (ttbbvChangeset != null ? String.Join("\n", ttbbvChanges) : "\n") + (ttbptChangeset != null ? String.Join("\n", ttbptChanges) : "\n") + (pstoChangeset != null ? String.Join("\n", pstoChanges) : "");
            mail.Subject = "Targeting: Asset Allocation Changes";
            MailSender.SendTargetingAlert(mail, userEmail);
        }

        public IEnumerable<IValidationIssue> ValidateModelAndPermissions(RootModel model, String username, CalculationTicket ticket)
		{
#warning Make use of the username.
            return this.ValidateModel(model, ticket);
		}

        public IEnumerable<IValidationIssue> ValidateModel(RootModel model, CalculationTicket ticket)
        {
            var issues = this.modelValidator.ValidateRoot(model, ticket);
            return issues;
        }
	}
}
