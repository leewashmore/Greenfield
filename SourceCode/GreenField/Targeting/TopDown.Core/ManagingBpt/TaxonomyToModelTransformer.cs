using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core;
using TopDown.Core.ManagingBpt;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingCountries;
using TopDown.Core.Persisting;


namespace TopDown.Core
{
	public class TaxonomyToModelTransformer
	{
		private ModelBuilder modelBuilder;
		private ExpressionPicker picker;
		private GlobeTraverser traverser;

		public TaxonomyToModelTransformer(
			ExpressionPicker picker,
			ModelBuilder modelBuilder,
			GlobeTraverser traverser
		)
		{
			this.picker = picker;
			this.modelBuilder = modelBuilder;
			this.traverser = traverser;
		}

		public RegionModel CreateRegion(Computations computations, RegionNode node)
		{
            var residents = new List<IRegionModelResident>();
            foreach (var child in node.GetResidents())
            {
                var model = this.CreateModelOnceResolved(computations, child);
                residents.Add(model);
            }
            
            var result = this.modelBuilder.CreateRegionModel(
                node.Name,
				computations.BaseActiveFormula,
                residents
			);
			return result;
		}

		public OtherModel CreateOther(Computations computations, OtherNode otherNode)
		{
            var basketCountries = new List<BasketCountryModel>();
            var backetCountryNodes = otherNode.GetBasketCountries();
            foreach (var basketCountryNode in backetCountryNodes)
            {
                var basketCountry = this.CreateBasketCountry(computations, basketCountryNode);
                basketCountries.Add(basketCountry);
            }
            
            var result = this.modelBuilder.CreateOtherModel(
                basketCountries,
                new List<UnsavedBasketCountryModel>()
            );
            return result;
		}

		public BasketCountryModel CreateBasketCountry(Computations computations, BasketCountryNode basketCountryNode)
		{
			var baseExpression = this.modelBuilder.CreateBaseExpression();
			var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
			
			var result = this.modelBuilder.CreateBasketCountryModel(
				basketCountryNode.Basket,
				computations,
				baseExpression,
				portfolioAdjustmentExpression
			);
			return result;
		}

		public BasketRegionModel CreateBasketRegion(Computations computations, BasketRegionNode basketRegionNode)
		{
			var countries = basketRegionNode.Basket.Countries.Select(x => this.CreateCountry(x)).ToList();
			var baseExpression = this.modelBuilder.CreateBaseExpression();
			var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
			
			var result = this.modelBuilder.CreateBasketRegionModel(
				basketRegionNode.Basket,
				countries,
				computations,
				baseExpression,
				portfolioAdjustmentExpression
			);
			return result;
		}

		public CountryModel CreateCountry(Country country)
		{
			var result = this.modelBuilder.CreateCountryModel(country);
			return result;
		}

		public IGlobeResident CreateModelOnceResolved(Computations computations, ITaxonomyResident resident)
		{
			var resolver = new CreateModel_TaxonomyResidentResolver(this, computations);
			resident.Accept(resolver);
			return resolver.Result;
		}

		private class CreateModel_TaxonomyResidentResolver : ITaxonomyResidentResolver
		{
			private TaxonomyToModelTransformer creator;
			private Computations computations;

			public CreateModel_TaxonomyResidentResolver(TaxonomyToModelTransformer creator, Computations computations)
			{
				this.creator = creator;
				this.computations = computations;
			}

			public IGlobeResident Result { get; private set; }

			public void Resolve(RegionNode node)
			{
				this.Result = this.creator.CreateRegion(this.computations, node);
			}

			public void Resolve(OtherNode node)
			{
				this.Result = this.creator.CreateOther(this.computations, node);
			}

			public void Resolve(BasketRegionNode node)
			{
				this.Result = this.creator.CreateBasketRegion(this.computations, node);
			}
		}

		public IRegionModelResident CreateModelOnceResolved(Computations computations, IRegionNodeResident resident)
		{
			var resolver = new CreateModel_RegionNodeResidentResolver(this, computations);
			resident.Accept(resolver);
			return resolver.Result;
		}

		private class CreateModel_RegionNodeResidentResolver : IRegionNodeResidentResolver
		{
			private TaxonomyToModelTransformer creator;
			private Computations computations;

			public CreateModel_RegionNodeResidentResolver(TaxonomyToModelTransformer creator, Computations computations)
			{
				this.creator = creator;
				this.computations = computations;
			}

			public IRegionModelResident Result { get; private set; }

			public void Resolve(RegionNode regionNode)
			{
				this.Result = this.creator.CreateRegion(this.computations, regionNode);
			}

			public void Resolve(BasketRegionNode basketRegionNode)
			{
				this.Result = this.creator.CreateBasketRegion(this.computations, basketRegionNode);
			}

			public void Resolve(BasketCountryNode node)
			{
				this.Result = this.creator.CreateBasketCountry(this.computations, node);
			}
		}
	}
}
