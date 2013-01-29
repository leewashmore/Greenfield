using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingPortfolios;
using TopDown.Core.ManagingBpt;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingBpst;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingCalculations
{
	public class Hopper
	{
		private RepositoryManager repositoryManager;
		private ManagingBpt.ModelManager bptManager;
		private ManagingBpst.ModelManager bpstManager;
		private BasketRenderer basketRenderer;

		[DebuggerStepThrough]
		public Hopper(
			RepositoryManager repositoryManager,
			ManagingBpt.ModelManager bptManager,
			ManagingBpst.ModelManager bpstManager,
			BasketRenderer basketRenderer
		)
		{
			this.repositoryManager = repositoryManager;
			this.bptManager = bptManager;
			this.basketRenderer = basketRenderer;
			this.bpstManager = bpstManager;
		}

        

		public IEnumerable<BgaPortfolioSecurityTargetInfo> RecalculateEverything(Int32 calculationId, IDataManager manager)
		{
			try
			{
				var targets = this.RecalculateEverythingUnsafe(calculationId, manager);
				return targets;
			}
			catch
			{
                
				// it is a good question what to do in case the calculation has failed
				// we could mark calculation as 'failed' in the database, but.. what if we lost the connection?
				// so setting status to 'failed' is not a reliable mechanism...

				// another thing can happend that we tried to run a calculation and we failed
				// calculation still sits in the database as a request that has never been run
				// is there anybody who can run it for us one more time? I don't think so...
				// see, currently we run a calculation on clicking the 'save' button from the web-page
				// thank god we create a calculation request and preform it in the scope of the same transaction
				// so that if we fail the request will go away as the transaction rolls back

				// well, if creading a calculation request and running the calculation is separated we are in trouble
				// because this way we will have orphaned calculation requests that can only be run if somebody specifically go to the database, get their ID's, and run them
				// if nodbody does it that these requests will block the subsequent request from being processed according to the logic of the RecalculateEverythingUnsafe method below
				// which is something you, dear reader of this, need to consider
#warning POTENTIAL BAD SITUATION, NEEDS TO BE RESOLVED ON REFACTORING (get details from the comment above).
				throw;
			}
		}

		protected IEnumerable<BgaPortfolioSecurityTargetInfo> RecalculateEverythingUnsafe(Int32 calculationId, IDataManager manager)
		{
			// getting the calculation and checking if it is in the mint condition
			// (making sure it hasn't been yet perfomed or failed)
			var calculationInfo = manager.GetTargetingCalculation(calculationId);
			this.MakeSureRequestedCalculationIsValid(calculationInfo);

			// we want to make sure that the requested calculation still makes sense
			// in order to do that we need to check if it is the latest one
			// if it is the latest one then we can proceed
			// if it is not then somebody else went ahead and changed something
			// and there is a good chance that our changes got overriden
			// to be on the safe side we just cancel this recalculation at the first sign of any possible conflict

			// so... in order to make the decision we need to have 2 latest calculations
			// why? because we need to check the last one and make sure it is this very request
			// we need to check the previous one and make sure it is in the finished or failed status
			// if it is not than either calculation hasn't been yet started or it is still running
			// both these options are bad things to mess with
			var latestCalculationInfos = manager.Get2MostRecentTargetingCalculations();
			this.MakeSureRequestedCalculationIsStillValid(calculationInfo, latestCalculationInfos);

            manager.StartTargetingCalculation(calculationId);

			// seems like we are good to go now
			// we want to work with the fresh data
			this.repositoryManager.DropEverything();

			// as we recalculating everything we need to go through all of targeting types and their portfolios
			var targetingTypeRepository = this.repositoryManager.ClaimTargetingTypeRepository(manager);
			var targetingTypes = targetingTypeRepository.GetTargetingTypes();
			var result = new List<BgaPortfolioSecurityTargetInfo>();
			this.RecalculateTargetingTypes(targetingTypes, result, manager);

			// by this moment we got a collection of the result records
			// now we want to save them
			manager.DeleteBgaPortfolioSecurityTargets(result);
			manager.InsertBgaPortfolioSecurityTargets(result);

#warning Not sure if we need a log, we probably don't as the calculation process is very straight forward.
			manager.FinishTargetingCalculationUnsafe(calculationId, 1, "One day there will be a log here");
			
			return result;
		}


		protected void RecalculateTargetingTypes(
			IEnumerable<TargetingType> targetingTypes,
			List<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager
		)
		{
			var issues = new List<IValidationIssue>();
			foreach (var targetingType in targetingTypes)
			{
				try
				{
					this.RecalculateTargetingType(targetingType, result, manager);
				}
				catch (ValidationException exception)
				{
					issues.Add(exception.Issue);
				}
			}
			if (issues.Any(x => x is ErrorIssue))
			{
				var issue = new CompoundValidationIssue("Unable to do recalculation.", issues);
				throw new ValidationException(issue);
			}
		}

		protected void RecalculateTargetingType(
			TargetingType targetingType,
			ICollection<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager
		)
		{
			var issues = new List<IValidationIssue>();
			foreach (var portfolio in targetingType.BroadGlobalActivePortfolios)
			{
				try
				{
					this.RecalculatePortfolio(
						targetingType,
						portfolio,
						result,
						manager
					);
				}
				catch (ValidationException exception)
				{
					issues.Add(exception.Issue);
				}

                if (issues.Any(x => x is ErrorIssue))
				{
					var issue = new CompoundValidationIssue(
						"Unable to recalculate the \"" + targetingType.Name + "\" targeting type (ID: " + targetingType.Id + ").",
						issues
					);
					throw new ValidationException(issue);
				}
			}
		}

		protected void RecalculatePortfolio(
			TargetingType targetingType,
			BroadGlobalActivePortfolio portfolio,
			ICollection<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager
		)
		{
			var portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(manager);

			var root = this.bptManager.GetRootModel(
				targetingType.Id,
				portfolio.Id,
				false /* we coundn't care less about benchmarks at this point */,
				manager
			);

			var ticket = new CalculationTicket();

			// let's see if computations are valid
			var issues = this.bptManager.Validate(root, ticket).ToList();
            if (issues.Any(x => x is ErrorIssue))
			{
				var issue = new CompoundValidationIssue("Unable to recalculate the \"" + portfolio.Name + "\" portfolio (ID: " + portfolio.Id + ").", issues);
				throw new ValidationException(issue);
			}

			var models = this.bptManager.Traverser.TraverseGlobe(root.Globe);
			foreach (var model in models)
			{
				try
				{
					this.RecalculateModelOnceResolved(
						model,
						targetingType,
						portfolio,
						result,
						manager,
						ticket
					);
				}
				catch (ValidationException exception)
				{
					issues.Add(exception.Issue);
				}
			}

            if (issues.Any(x => x is ErrorIssue))
			{
				var issue = new CompoundValidationIssue("Unable to recalculate the \"" + portfolio.Name + "\" portfolio (ID: " + portfolio.Id + ").", issues);
				throw new ValidationException(issue);
			}
		}

		protected void RecalculateModelOnceResolved(
			IModel model,
			TargetingType targetingType,
			BroadGlobalActivePortfolio portfolio,
			ICollection<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager,
			CalculationTicket ticket
		)
		{
			var resolver = new RecalculateModelOnceResolved_IModelResolver(
				this,
				targetingType,
				portfolio,
				result,
				manager,
				ticket
			);
			model.Accept(resolver);
		}

		private class RecalculateModelOnceResolved_IModelResolver : IModelResolver
		{
			private IDataManager manager;
			private BroadGlobalActivePortfolio portfolio;
			private Hopper hopper;
			private CalculationTicket ticket;
			private TargetingType targetingType;
			private ICollection<BgaPortfolioSecurityTargetInfo> result;

			public RecalculateModelOnceResolved_IModelResolver(
				Hopper hopper,
				TargetingType targetingType,
				BroadGlobalActivePortfolio portfolio,
				ICollection<BgaPortfolioSecurityTargetInfo> result,
				IDataManager manager,
				CalculationTicket ticket
			)
			{
				this.hopper = hopper;
				this.targetingType = targetingType;
				this.portfolio = portfolio;
				this.result = result;
				this.manager = manager;
				this.ticket = ticket;
			}

			public void Resolve(BasketCountryModel model)
			{
				this.hopper.RecalculateBasketCountry(
					model,
					this.targetingType,
					this.portfolio,
					this.result,
					this.manager,
					this.ticket
				);
			}

			public void Resolve(BasketRegionModel model)
			{
				this.hopper.RecalculateBasketRegion(
					model,
					this.targetingType,
					this.portfolio,
					this.result,
					this.manager,
					this.ticket
				);
			}

			public void Resolve(CountryModel model)
			{
				// do nothing
			}

			public void Resolve(OtherModel model)
			{
				// do nothing
			}

			public void Resolve(RegionModel model)
			{
				// do nothing
			}

			public void Resolve(UnsavedBasketCountryModel model)
			{
				throw new ValidationException(
					new ErrorIssue("There is an unsaved basket for the \"" + model.Country.Name + "\" country (ISO: " + model.Country.IsoCode + ") which hasn't been saved to the system yet.")
				);
			}
		}

		protected void RecalculateBasketRegion(
			BasketRegionModel model,
			TargetingType targetingType,
			BroadGlobalActivePortfolio portfolio,
			ICollection<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager,
			CalculationTicket ticket
			)
		{
			var baseValue = model.Base.Value(ticket);
			if (!baseValue.HasValue) throw new ValidationException(new ErrorIssue("There is no base value defined for the \"" + model.Basket.Name + "\" region."));

			var portfolioScaledValue = model.PortfolioScaled.Value(ticket);
			if (!portfolioScaledValue.HasValue) throw new ValidationException(new ErrorIssue("There is no portfolio scaled value defined for the \"" + model.Basket.Name + "\" region."));

			this.RecalculateSecurityTargets(
				portfolioScaledValue.Value,
				model.Basket,
				targetingType,
				portfolio,
				result,
				manager,
				ticket
			);
		}

		protected void RecalculateBasketCountry(
			BasketCountryModel model,
			TargetingType targetingType,
			BroadGlobalActivePortfolio portfolio,
			ICollection<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager,
			CalculationTicket ticket
		)
		{
			var baseValue = model.Base.Value(ticket);
			if (!baseValue.HasValue) throw new ValidationException(new ErrorIssue("There is no base value defined for the \"" + model.Basket.Country.Name + "\" country (ISO: " + model.Basket.Country.IsoCode + ")."));

			var portfolioScaledValue = model.PortfolioScaled.Value(ticket);
			if (!portfolioScaledValue.HasValue) throw new ValidationException(new ErrorIssue("There is no portfolio scaled value defined for the \"" + model.Basket.Country.Name + "\" country (ISO: " + model.Basket.Country.IsoCode + ")."));

			this.RecalculateSecurityTargets(
				portfolioScaledValue.Value,
				model.Basket,
				targetingType,
				portfolio,
				result,
				manager,
				ticket
			);
		}

		protected void RecalculateSecurityTargets(
			Decimal portfolioScaled,
			IBasket basket,
			TargetingType targetingType,
			BroadGlobalActivePortfolio portfolio,
			ICollection<BgaPortfolioSecurityTargetInfo> result,
			IDataManager manager,
			CalculationTicket ticket
		)
		{
			var securityRepositry = this.repositoryManager.ClaimSecurityRepository(manager);
			var targetingTypeGroupRepository = this.repositoryManager.ClaimTargetingTypeGroupRepository(manager);
			var bpstRepository = this.repositoryManager.CliamBasketPortfolioSecurityTargetRepository(manager);
			var ttgbsbvRepository = this.repositoryManager.ClaimTargetingTypeGroupBasketSecurityBaseValueRepository(manager);
			var targetingTypeGroup = targetingTypeGroupRepository.GetTargetingTypeGroup(targetingType.TargetingTypeGroupId);
			var portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(manager);
			
			var core = this.bpstManager.GetCoreModel(
				targetingTypeGroup,
				basket,
				securityRepositry,
				ttgbsbvRepository,
				bpstRepository,
				portfolioRepository
			);

			this.MakeSureBpstCoreModelIsValid(core, ticket);

			foreach (var securityModel in core.Securities)
			{
				var unscaledTarget = this.ResolveToUnscaledTarget(portfolio, securityModel, ticket);
				var scaledTaget = unscaledTarget * portfolioScaled;
				var targetInfo = new BgaPortfolioSecurityTargetInfo(
					portfolio.Id,
					securityModel.Security.Id,
					scaledTaget
				);
				result.Add(targetInfo);
			}
		}

		protected void MakeSureBpstCoreModelIsValid(CoreModel model, CalculationTicket ticket)
		{
			var issues = this.bpstManager.Validate(model, ticket);
            if (issues.Any(x => x is ErrorIssue))
			{
				throw new ValidationException(
					new CompoundValidationIssue(
						"B-P-S-T model of the \"" + model.TargetingTypeGroup.Name + "\" targeting type group and \"" +
						this.basketRenderer.RenderBasketOnceResolved(model.Basket) + "\" basket is invalid.", issues
					)
				);
			}
		}

		protected Decimal ResolveToUnscaledTarget(BroadGlobalActivePortfolio portfolio, SecurityModel model, CalculationTicket ticket)
		{
			var baseValue = model.Base.Value(ticket);
			/*if (!baseValue.HasValue) throw new ValidationException(
				new ValidationIssue("There is no base value assigned to the \"" + model.Security.Name + "\" security.")
			);*/
            if (!baseValue.HasValue)
            {
#warning THE FOLLOWING IS A BUG BECAUSE EVEN THOUGH WE ASSIGN THE BASE VALUE TO 0 THIS VALUE DOESN'T GO BACK TO THE EXPRESSION IT BELONGS TO
                baseValue = 0;
            }
			
			var portfolioModelOpt = model.PortfolioTargets.Where(x => x.BroadGlobalActivePortfolio.Id == portfolio.Id).FirstOrDefault();
			if (portfolioModelOpt == null) return baseValue.Value;

			var portfolioTarget = portfolioModelOpt.Target.Value(ticket);
			/*if (!portfolioTarget.HasValue) throw new ValidationException(
				new ValidationIssue("There is a portfolio target model which however doesn't have a value assigned.")
			);*/
            if (!portfolioTarget.HasValue)
            {
                portfolioTarget = baseValue;
            }

			return portfolioTarget.Value;
		}

		protected void MakeSureRequestedCalculationIsValid(TargetingCalculationInfo calculationInfo)
		{
			if (calculationInfo.StatusCode > 0)
			{
				var issue = new ErrorIssue(
					"Your calculation (ID: " + calculationInfo.Id + ") has already been run on " + calculationInfo.StartedOn + ", it doesn't make sense to run the same calcualtion multiple times."
				);
				throw new ValidationException(issue);
			}
		}

		protected void MakeSureRequestedCalculationIsStillValid(TargetingCalculationInfo calculationInfo, IEnumerable<TargetingCalculationInfo> latestCalculationInfos)
		{
			try
			{
				var latestCalculation = latestCalculationInfos.GetEnumerator();
				if (!latestCalculation.MoveNext()) throw new ApplicationException();
				var first = latestCalculation.Current;
				if (latestCalculation.MoveNext())
				{
					var second = latestCalculation.Current;

					if (first.Id == calculationInfo.Id)
					{
						// our calculation is on top which is good, however we need to check if the previous calculation was finished
						if (second.StatusCode < 1) throw new ApplicationException("There is a calculation (ID: " + second.Id + ") already in the queue. This calculation is either running or failed and somebody needs to clean it up. Try again if a few minutes. If you get the same issue, contact your administrator.");
					}
					else if (second.Id == calculationInfo.Id)
					{
						// our calculation is second in the list meaning it must be finished
						if (second.StatusCode < 1)
						{
							throw new ApplicationException("Your calculation (ID: " + second.Id + ") has already been run on " + calculationInfo.StartedOn + ", it doesn't make sense to run the same calcualtion multiple times.");
						}
						else
						{
							throw new ApplicationException("Your calculation is just weird.");
						}
					}
					else
					{
						throw new ApplicationException("Your calculation (ID: " + calculationInfo.Id + ") is neither the last one nor the one right before the last one. I am not sure how this can be possible. Anyway, it is a bad situation and there will be no calculations at this time. Sorry.");
					}
				}
				else
				{
					// there is only one calculation which is...
					if (first.Id != calculationInfo.Id)
					{
						throw new ApplicationException("There is only one calculation in the queue and it is not yours. Weird...");
					}
				}
			}
			catch (Exception exception)
			{
				throw new ValidationException(new ErrorIssue(exception.Message));
			}
		}

	}
}
