using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using Newtonsoft.Json;
using System.IO;
using Aims.Expressions;
using TopDown.Core.ManagingCalculations;
using Aims.Core;
using TopDown.Core.Helpers;

namespace TopDown.Core.ManagingPst
{
    public class PstManager
    {
        private ChangesetApplier applier;
        private ModelToChangesetTransformer transformer;
        private ModelFromJsonDeserializer modelDeserializer;
        private ModelBuilder modelBuilder;
        private RepositoryManager repositoryManager;
        private ModelChangeDetector modelChangeDetector;
        private IDataManagerFactory dataManagerFactory;
        private ManagingCalculations.CalculationRequester calculationRequester;
        private ModelToJsonSerializer modelSerializer;
        private ModelValidator modelValidator;

        public PstManager(
            ChangesetApplier applier,
            ModelValidator modelValidator,
            ModelToChangesetTransformer transformer,
            ModelFromJsonDeserializer modelDeserializer,
            ModelBuilder modelBuilder,
            RepositoryManager repositoryManager,
            ModelChangeDetector modelChangeDetector,
            IDataManagerFactory dataManagerFactory,
            ManagingCalculations.CalculationRequester calculationRequester,
            ModelToJsonSerializer modelSerializer
        )
        {
            this.applier = applier;
            this.modelValidator = modelValidator;
            this.transformer = transformer;
            this.modelDeserializer = modelDeserializer;
            this.modelBuilder = modelBuilder;
            this.repositoryManager = repositoryManager;
            this.modelChangeDetector = modelChangeDetector;
            this.dataManagerFactory = dataManagerFactory;
            this.calculationRequester = calculationRequester;
            this.modelSerializer = modelSerializer;
        }

        public RootModel GetPstRootModel(IDataManager manager, String portfolioId, SecurityRepository securityRepository)
        {
            var targets = manager.GetPortfolioSecurityTargets(portfolioId).OrderByDescending(x => x.Target);
            var latestChangeSetInfo = manager.GetLatestPortfolioSecurityTargetChangeSet();

            var items = new List<ItemModel>();
            foreach (var target in targets)
            {
                var security = securityRepository.GetSecurity(target.SecurityId);
                var targetExpression = this.modelBuilder.CreateTargetExpression();
                var item = this.modelBuilder.CreateItem(security, targetExpression);
                item.Target.InitialValue = target.Target;
                items.Add(item);
            }

            var targetTotalExpression = this.modelBuilder.CreateTargetTotalExpression(items);
            var cashExpression = this.modelBuilder.CreateCashExpression(targetTotalExpression);

            var result = new RootModel(
                portfolioId,
                latestChangeSetInfo,
                items,
                targetTotalExpression,
                cashExpression
            );

            return result;
        }

        public RootModel DeserializeFromJson(
            String pstCompositionAsJson,
            SecurityRepository securityRepository
        )
        {
            RootModel composition;
            using (var reader = new JsonReader(new JsonTextReader(new StringReader(pstCompositionAsJson))))
            {
                composition = this.modelDeserializer.DeserializeRoot(reader, securityRepository);
            }
            return composition;
        }

        public String SerializeToJson(RootModel root, CalculationTicket ticket)
        {
            var builder = new StringBuilder();
            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.Write(delegate
                {
                    this.modelSerializer.SerializeRoot(writer, root, ticket);
                    writer.Write(this.modelChangeDetector.HasChanged(root), JsonNames.Unsaved);
                });
            }

            var result = builder.ToString();
            return result;
        }

        public IEnumerable<IValidationIssue> ApplyIsValid(RootModel composition, String username, String userEmail, SqlConnection connection, CalculationTicket ticket, SecurityRepository securityRepository, ref CalculationInfo info)
        {
            var issues = this.modelValidator.ValidateRoot(composition, ticket);
            if (issues.Any()) return issues;

            try
            {
                var changesetOpt = this.transformer.TryTransformToChangeset(username, composition);
                if (changesetOpt != null)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        var manager = this.dataManagerFactory.CreateDataManager(connection, transaction);

                        info = this.calculationRequester.RequestCalculation(manager);
                        this.applier.Apply(info.Id, changesetOpt, manager);
                        try
                        {
                            var message = this.applier.PrepareToSend(changesetOpt, manager, securityRepository);
                            MailSender.SendTargetingAlert(message, userEmail);
                        }
                        catch (Exception e)
                        {
                            throw new EmailNotificationException("See inner exception for details.", e);
                        }
                        // it's important to drop the repository (or at least part of it if you can manage to carefully resolve everything) as we changed the composition
                        this.repositoryManager.DropRepository();
                        transaction.Commit();
                    }
                }
                return No.ValidationIssues;
            }
            catch (ValidationException exception)
            {
                return new IValidationIssue[] { exception.Issue };
            }
        }

        public IEnumerable<IValidationIssue> Validate(RootModel model, CalculationTicket ticket)
        {
            var issues = this.modelValidator.ValidateRoot(model, ticket);
            return issues;
        }
    }
}
