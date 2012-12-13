using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBpt;
using TopDown.Core.ManagingCountries;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingBpt
{
	public class ModelBuilder
	{
		private ExpressionPicker picker;
		private CommonParts commonParts;
		private DefaultValues defaultValues;


		public ModelBuilder(
			ExpressionPicker picker,
			CommonParts commonParts,
			DefaultValues defaultValues,
			Overlaying.ModelBuilder overlayModelBuilder
		)
		{
			this.picker = picker;
			this.commonParts = commonParts;
			this.defaultValues = defaultValues;
			this.OverlayModelBuilder = overlayModelBuilder;
		}

		public Overlaying.ModelBuilder OverlayModelBuilder { get; private set; }

		public IModelFormula<IModel, Decimal?> CreateBaseActiveFormula()
		{
			return new Computing.BaseActiveFormula(this.picker);
		}

		public IModelFormula<IModel, Decimal?> CreatePortfolioScaledFormula(
			IModelFormula<IModel, Decimal?> baseLessOverlayFormula,
			IExpression<Decimal?> baseLessOverlayPositiveTotal,
			IExpression<Decimal?> baseLessOverlayTotal
		)
		{
			return new Computing.PortfolioScaledFormula(baseLessOverlayFormula, baseLessOverlayPositiveTotal, baseLessOverlayTotal);
		}

        public IFormula<Decimal?> CreateCashScaledFormula(
            IExpression<Decimal?> rescaledBase,
            IExpression<Decimal?> baseLessOverlayPositiveTotal,
            IExpression<Decimal?> baseLessOverlayTotal
        )
        {
            return new Computing.CashScaledFormula(rescaledBase, baseLessOverlayPositiveTotal, baseLessOverlayTotal);
        }


		public IModelFormula<IModel, Decimal?> CreateBaseLessOverlayFormula(
			IModelFormula<IModel, Decimal?> rescaledBaseForAdjustmentFormula
		)
		{
			return new Computing.BaseLessOverlayFormula(this.picker, rescaledBaseForAdjustmentFormula);
		}


		public IModelFormula<IModel, Decimal?> CreateRescaledBaseFormula(
			IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal,
			IExpression<Decimal?> porfolioAdjustmentTotal
		)
		{
			return new Computing.RescaledBaseFormula(baseWherePortfoioAdjustmentSetTotal, porfolioAdjustmentTotal);
		}


		public IFormula<Decimal?> CreateRescaledBaseForCashFormula(
			IExpression<Decimal?> cashBase,
			IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal,
			IExpression<Decimal?> porfolioAdjustmentTotal
		)
		{
			return new Computing.RescaledBaseForCashFormula(cashBase, baseWherePortfoioAdjustmentSetTotal, porfolioAdjustmentTotal);
		}

		public Expression<Decimal?> CreateRescaledBaseForCash(
			IFormula<Decimal?> rescaledBaseForCashFormula
		)
		{
			return new Expression<Decimal?>(
				ValueNames.RescaledBaseForCash,
                rescaledBaseForCashFormula,
                this.commonParts.NullableDecimalValueAdapter,
                this.commonParts.ValidateWhatever
            );
		}

        public Expression<Decimal?> CreateScaledCash(
            IFormula<Decimal?> scaledCashFormula
        )
        {
            return new Expression<Decimal?>(
                ValueNames.CashScaled,
                scaledCashFormula,
                this.commonParts.NullableDecimalValueAdapter,
                this.commonParts.ValidateWhatever
            );
        }



		public IFormula<Decimal?> CreateBaseForCashFormula(GlobeModel root)
		{
			return new Computing.BaseForCashFormula(root);
		}

		public IExpression<Decimal?> CreateBaseForCash(IFormula<Decimal?> baseForCashFormula)
		{
			return new Expression<Decimal?>(
				ValueNames.BaseForCash,
				baseForCashFormula,
				this.commonParts.NullableDecimalValueAdapter,
				this.commonParts.ValidateNonNegativeOrNull
			);
		}

		
		/// <summary>
		/// Sum of the base values of the records that have their portfolio adjustment values set.
		/// </summary>
		public IExpression<Decimal?> CreateBaseWherePortfoioAdjustmentSetTotal(GlobeTraverser traverser, GlobeModel root)
		{
			var result = new Expression<Decimal?>(
				ValueNames.BaseWherePortfolioAdjustmentSetTotal,
				new Computing.BaseWherePortfoioAdjustmentSetTotalFormula(root, traverser),
				this.commonParts.NullableDecimalValueAdapter,
                this.commonParts.ValidateWhatever
			);
			return result;
		}



		public IExpression<Decimal?> CreateBaseLessOverlayPositiveTotalExpression(
			GlobeModel root,
			GlobeTraverser traverser,
			IModelFormula<IModel, Decimal?> baseLessOverlayFormula,
			IExpression<Decimal?> cashRescaledBase
		)
		{
			var models = traverser.TraverseGlobe(root);
			var result = new Expression<Decimal?>(
				ValueNames.BaseLessOverlayPositiveTotal,
                new BaseLessOverlayPositiveTotalFormula(
                    models,
                    baseLessOverlayFormula,
                    cashRescaledBase
                ),
                this.commonParts.NullableDecimalValueAdapter,
                this.commonParts.ValidateWhatever
            );
            return result;

		}

		public IExpression<Decimal?> CreateBaseLessOverlayTotalExpression(
			GlobeModel root,
			GlobeTraverser traverser,
			IModelFormula<IModel, Decimal?> baseLessOverlayFormula,
			IExpression<Decimal?> cashRescaledBase
		)
		{
			var models = traverser.TraverseGlobe(root);
            var result = new Expression<Decimal?>(
				ValueNames.BaseLessOverlayTotal,
                new BaseLessOverlayTotalFormula(
                    models,
                    baseLessOverlayFormula,
                    cashRescaledBase
                ),
                this.commonParts.NullableDecimalValueAdapter,
                this.commonParts.ValidateWhatever
            );
			return result;
		}

		public IModelFormula<IModel, Decimal?> CreateTrueExposureFormula()
		{
			return new Computing.TrueExposureFormula(this.picker);
		}

		public IModelFormula<IModel, Decimal?> CreateTrueActiveFormula()
		{
			return new Computing.TrueActiveFormula(this.picker);
		}


		public Computations CreateComputations(GlobeModel root, GlobeTraverser traverser)
		{
			var computations = new Computations();

			computations.BaseActiveFormula = this.CreateBaseActiveFormula();
			var portfolioAdjustmentTotal = computations.PortfolioAdjustmentTotal = root.PortfolioAdjustment;
			var baseWherePortfoioAdjustmentSetTotal = computations.BaseWherePortfoioAdjustmentSetTotal = this.CreateBaseWherePortfoioAdjustmentSetTotal(traverser, root);
			computations.BaseLessOverlayFormula = this.CreateBaseLessOverlayFormula(
				this.CreateRescaledBaseFormula(
					computations.BaseWherePortfoioAdjustmentSetTotal,
					computations.PortfolioAdjustmentTotal
				)
			);

			computations.CashBase = this.CreateBaseForCash(this.CreateBaseForCashFormula(root));
			computations.CashRescaledBase = this.CreateRescaledBaseForCash(this.CreateRescaledBaseForCashFormula(computations.CashBase, baseWherePortfoioAdjustmentSetTotal, portfolioAdjustmentTotal));

			computations.BaseLessOverlayPositiveTotal = this.CreateBaseLessOverlayPositiveTotalExpression(root, traverser, computations.BaseLessOverlayFormula, computations.CashRescaledBase);
			computations.BaseLessOverlayTotal = this.CreateBaseLessOverlayTotalExpression(root, traverser, computations.BaseLessOverlayFormula, computations.CashRescaledBase);
			computations.PortfolioScaledFormula = this.CreatePortfolioScaledFormula(computations.BaseLessOverlayFormula, computations.BaseLessOverlayPositiveTotal, computations.BaseLessOverlayTotal);
            computations.CashScaled = this.CreateScaledCash(this.CreateCashScaledFormula(computations.CashRescaledBase, computations.BaseLessOverlayPositiveTotal, computations.BaseLessOverlayTotal));
			computations.TrueExposureFormula = this.CreateTrueExposureFormula();
			computations.TrueActiveFormula = this.CreateTrueActiveFormula();

			return computations;
		}

        public UnsavedBasketCountryModel CreateUnsavedBasketModel(
			Country country,
			Computations computations,
			EditableExpression baseExpression,
			EditableExpression portfolioAdjustmentExpression
		)
		{
			var result = new UnsavedBasketCountryModel(
				country,
                new UnchangableExpression<Decimal>(
					ValueNames.Base,
                    this.defaultValues.DefaultBenchmark,
                    this.commonParts.DecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				baseExpression,
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.BaseActive,
                    self,
                    computations.BaseActiveFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                new UnchangableExpression<Decimal>(
					ValueNames.Overlay,
                    this.defaultValues.DefaultOverlay,
                    this.commonParts.DecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				portfolioAdjustmentExpression,
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.PortfolioScaled,
                    self,
                    computations.PortfolioScaledFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.TrueExposure,
                    self,
                    computations.TrueExposureFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.TrueActive,
                    self,
                    computations.TrueActiveFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                this.CreateBasketCountryModel(null, computations, baseExpression, portfolioAdjustmentExpression)
			);

			return result;
		}

		public BasketRegionModel CreateBasketRegionModel(
			ManagingBaskets.RegionBasket basket,
			IEnumerable<CountryModel> countries,
			Computations computations,
			EditableExpression baseExpression,
			EditableExpression portfolioAdjustmentExpression
		)
		{
			var result = new BasketRegionModel(
				basket,
                new SumExpression(
					ValueNames.Benchmark,
                    countries.Select(x => x.Benchmark),
                    this.defaultValues.DefaultBenchmark,
                    this.commonParts.ValidateWhatever
                ),
				baseExpression,
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.BaseActive,
                    self,
                    computations.BaseActiveFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				new SumExpression(
					ValueNames.Overlay,
                    countries.Select(x => x.Overlay),
                    this.defaultValues.DefaultOverlay,
                    this.commonParts.ValidateWhatever
                ),
				portfolioAdjustmentExpression,
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.PortfolioScaled,
                    self,
                    computations.PortfolioScaledFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.TrueExposure,
                    self,
                    computations.TrueExposureFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.TrueActive,
                    self,
                    computations.TrueActiveFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				countries
			);
			return result;
		}


		public BasketCountryModel CreateBasketCountryModel(
			ManagingBaskets.CountryBasket basket,
			Computations computations,
			EditableExpression baseExpression,
			EditableExpression portfolioAdjustmentExpression
		)
		{
			var result = new BasketCountryModel(
				basket,
				new UnchangableExpression<Decimal>(
					ValueNames.Benchmark,
					this.defaultValues.DefaultBenchmark,
					this.commonParts.DecimalValueAdapter,
					this.commonParts.ValidateWhatever
				),
				baseExpression,
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.BaseActive,
                    self,
                    computations.BaseActiveFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				new UnchangableExpression<Decimal>(
					ValueNames.Overlay,
					this.defaultValues.DefaultOverlay,
					this.commonParts.DecimalValueAdapter,
					this.commonParts.ValidateWhatever
				),
				portfolioAdjustmentExpression,
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.PortfolioScaled,
                    self,
                    computations.PortfolioScaledFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.TrueExposure,
                    self,
                    computations.TrueExposureFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.TrueActive,
                    self,
                    computations.TrueActiveFormula,
                    this.commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
				this.commonParts
			);

			return result;
		}

        public GlobeModel CreateGlobeModel(ICollection<IGlobeResident> residents)
        {
            var result = new GlobeModel(
                residents,
                new NullableSumExpression(
					ValueNames.Base,
                    residents.Select(x => this.picker.Base.TryPickExpression(x)),
                    this.defaultValues.DefaultBase,
                    this.commonParts.ValidateWhatever
                ),
                new SumExpression(
					ValueNames.Benchmark,
                    residents.Select(x => this.picker.Benchmark.TryPickExpression(x)),
                    this.defaultValues.DefaultBenchmark,
                    this.commonParts.ValidateWhatever
                ),
                new SumExpression(
					ValueNames.Overlay,
                    residents.Select(x => this.picker.Overlay.TryPickExpression(x)),
                    this.defaultValues.DefaultOverlay,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.PortfolioAdjsutment,
                    residents.Select(x => this.picker.PortfolioAdjustment.TryPickExpression(x)),
                    this.defaultValues.DefaultPortfolioAdjustment,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.PortfolioScaled,
                    residents.Select(x => this.picker.PortfolioScaled.TryPickExpression(x)),
                    this.defaultValues.DefaultPortfolioScaled,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.TrueExposure,
                    residents.Select(x => this.picker.TrueExposure.TryPickExpression(x)),
                    this.defaultValues.DefaultTrueExposure,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.TrueActive,
                    residents.Select(x => this.picker.TrueActive.TryPickExpression(x)),
                    this.defaultValues.DefaultTrueActive,
                    this.commonParts.ValidateWhatever
                )
            );
            return result;
        }

        public RegionModel CreateRegionModel(
            String name,
            IModelFormula<IModel, Decimal?> baseActiveFormula,
            IEnumerable< IRegionModelResident> residents
        )
        {
            var result = new RegionModel(
                name,
                new SumExpression(
					ValueNames.Benchmark,
                    residents.Select(x => picker.Benchmark.TryPickExpression(x)),
                    this.defaultValues.DefaultBenchmark,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.Base,
                    residents.Select(x => picker.Base.TryPickExpression(x)),
                    this.defaultValues.DefaultBase,
                    this.commonParts.ValidateWhatever
                ),
                self => new ModelFormulaExpression<IModel, Decimal?>(
					ValueNames.BaseActive,
                    self,
                    baseActiveFormula,
                    commonParts.NullableDecimalValueAdapter,
                    this.commonParts.ValidateWhatever
                ),
                new SumExpression(
					ValueNames.Overlay,
                    residents.Select(x => picker.Overlay.TryPickExpression(x)),
                    this.defaultValues.DefaultOverlay,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.PortfolioAdjsutment,
                    residents.Select(x => picker.PortfolioAdjustment.TryPickExpression(x)),
                    this.defaultValues.DefaultPortfolioAdjustment,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.PortfolioScaled,
                    residents.Select(x => picker.PortfolioScaled.TryPickExpression(x)),
                    this.defaultValues.DefaultPortfolioScaled,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.TrueExposure,
                    residents.Select(x => picker.TrueExposure.TryPickExpression(x)),
                    this.defaultValues.DefaultTrueExposure,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.TrueActive,
                    residents.Select(x => picker.TrueActive.TryPickExpression(x)),
                    this.defaultValues.DefaultTrueActive,
                    this.commonParts.ValidateWhatever
                ),
                residents
            );
            return result;
        }

        public OtherModel CreateOtherModel(List<BasketCountryModel> basketCountries, List<UnsavedBasketCountryModel> unsavedBasketCountries)
        {
            var result = new OtherModel(
                new SumExpression(
					ValueNames.Benchmark,
                    basketCountries.Select(x => x.Benchmark).Union(unsavedBasketCountries.Select(x => x.Benchmark)),
                    this.defaultValues.DefaultBenchmark,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.Base,
                    basketCountries.Select(x => x.Base).Union(unsavedBasketCountries.Select(x => x.Base)),
                    this.defaultValues.DefaultBase,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.BaseActive,
                    basketCountries.Select(x => x.BaseActive).Union(unsavedBasketCountries.Select(x => x.BaseActive)),
                    this.defaultValues.DefaultBaseActive,
                    this.commonParts.ValidateWhatever
                ),
                new SumExpression(
					ValueNames.Overlay,
                    basketCountries.Select(x => x.Overlay).Union(unsavedBasketCountries.Select(x => x.Overlay)),
                    this.defaultValues.DefaultOverlay,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.PortfolioAdjsutment,
                    basketCountries.Select(x => x.PortfolioAdjustment).Union(unsavedBasketCountries.Select(x => x.PortfolioAdjustment)),
                    this.defaultValues.DefaultPortfolioAdjustment,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.PortfolioScaled,
                    basketCountries.Select(x => x.PortfolioScaled).Union(unsavedBasketCountries.Select(x => x.PortfolioScaled)),
                    this.defaultValues.DefaultPortfolioScaled,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.TrueExposure,
                    basketCountries.Select(x => x.TrueExposure).Union(unsavedBasketCountries.Select(x => x.TrueExposure)),
                    this.defaultValues.DefaultTrueExposure,
                    this.commonParts.ValidateWhatever
                ),
                new NullableSumExpression(
					ValueNames.TrueActive,
                    basketCountries.Select(x => x.TrueActive).Union(unsavedBasketCountries.Select(x => x.TrueActive)),
                    this.defaultValues.DefaultTrueActive,
                    this.commonParts.ValidateWhatever
                ),
                basketCountries,
                unsavedBasketCountries
            );
            return result;

        }

        public IExpression<Decimal?> CreatePortfolioScaledTotalExpression(
            IExpression<Decimal?> cashBaseExpression,
            IExpression<Decimal?> globePortfolioScaledExpression)
        {
            var result = new Expression<Decimal?>(
				ValueNames.PortfolioScaledTotal,
                new Computing.PortfolioScaledTotalFormula(
                    cashBaseExpression,
                    globePortfolioScaledExpression
                ),
                this.commonParts.NullableDecimalValueAdapter,
                this.commonParts.ValidateWhatever
            );
            return result;
        }


		public EditableExpression CreateBaseExpression()
		{
			return new EditableExpression(
				ValueNames.Base,
				this.defaultValues.DefaultBase,
				this.commonParts.NullableDecimalValueAdapter,
				this.ValidateBaseExpression
			);
		}

		protected IEnumerable<IValidationIssue> ValidateBaseExpression(EditableExpression expression)
		{
			var issues = new List<IValidationIssue>();
			issues.AddRange(this.commonParts.ValidateComment(expression));
			issues.AddRange(this.commonParts.ValidateNonNegative(expression.Name, expression.EditedValue));
			return issues;
		}

		public EditableExpression CreatePortfolioAdjustmentExpression()
		{
			return this.commonParts.CreateInputExpression(
				ValueNames.PortfolioAdjsutment,
				this.defaultValues.DefaultPortfolioAdjustment
			);
		}

		public CountryModel CreateCountryModel(Country country)
		{
			var result = new CountryModel(country,
				this.defaultValues.DefaultBenchmark,
				this.defaultValues.DefaultOverlay,
				this.commonParts
			);
			return result;
		}
	}
}
