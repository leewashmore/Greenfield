using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using Aims.Expressions;
using TopDown.Core.ManagingCalculations;

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

        public void Apply(RootModel model, String username, SqlConnection connection, ref CalculationInfo calculationInfo)
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

    }
}
