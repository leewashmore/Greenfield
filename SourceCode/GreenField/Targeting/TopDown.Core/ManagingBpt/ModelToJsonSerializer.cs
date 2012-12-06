using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingCountries;
using TopDown.Core.Persisting;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
	public class ModelToJsonSerializer : ModelToJsonSerializerBase
	{
		private ExpressionToJsonSerializer expressionWriter;
		private ManagingPortfolios.PortfolioToJsonSerializer portfolioSerializer;

		[DebuggerStepThrough]
		public ModelToJsonSerializer(
			ExpressionToJsonSerializer expressionWriter,
			ManagingPortfolios.PortfolioToJsonSerializer portfolioSerializer
		)
		{
			this.expressionWriter = expressionWriter;
			this.portfolioSerializer = portfolioSerializer;
		}


		public void SerializeRoot(IJsonWriter writer, RootModel model, CalculationTicket ticket)
		{
			writer.Write(model.TargetingType.Id, JsonNames.TargetingTypeId);
			writer.Write(model.Portfolio.Id, JsonNames.PortfolioId);
            writer.Write(model.BenchmarkDate, JsonNames.BenchmarkDate);
			writer.Write(JsonNames.LatestChangesets, delegate
			{
				writer.Write(JsonNames.TargetingTypeBasketBaseValue, delegate
				{
					this.SerializeTargetingTypeBasketBaseValueChangeset(model.LatestTtbbvChangeset, writer);
				});
				writer.Write(JsonNames.TargetingTypeBasketPortfolioTarget, delegate
				{
					this.SerializeTargetingTypeBasketPortfolioTargetChangeset(model.LatestTtbptChangeset, writer);
				});
				writer.Write(JsonNames.PortfolioSecurityTargetOverlay, delegate
				{
					this.SerializePortfolioSecurityTargetOverlayChangeset(model.LatestPstoChangeset, writer);
				});
                writer.Write(JsonNames.PortfolioSecurityTarget, delegate
                {
                    this.SerializePortfolioSecurityTargetChangeset(model.LatestPstChangeset, writer);
                });
			});
			writer.Write(JsonNames.Root, delegate
			{
                this.SerializeGlobe(writer, model.Globe, ticket);
			});
			writer.Write(JsonNames.Cash, delegate
			{
                this.SerializeCash(writer, model.Cash, ticket);
			});
			writer.Write(JsonNames.Overlay, delegate
			{
                this.SerializeOverlay(model.Factors, writer, ticket);
			});
            this.expressionWriter.SerializeOnceResolved(model.PortfolioScaledTotal, JsonNames.PortfolioScaledTotal, writer, ticket);
		}


        protected void SerializeOverlay(Overlaying.RootModel overlay, IJsonWriter writer, CalculationTicket ticket)
		{
			writer.WriteArray(overlay.Items, JsonNames.Items, item =>
			{
				writer.Write(delegate
				{
#warning set up a special object for JSON property names in order not to confuse them with string values
					writer.Write(JsonNames.Portfolio, delegate
					{
						this.portfolioSerializer.SerializeBottomUpPortfolio(item.BottomUpPortfolio, writer);
					});
					this.expressionWriter.SerializeOnceResolved(
                        item.OverlayFactor,
                        JsonNames.OverlayFactor,
                        writer,
                        ticket
                    );
				});
			});
		}

		protected void SerializeTargetingTypeBasketBaseValueChangeset(
			TargetingTypeBasketBaseValueChangesetInfo changesetInfo, IJsonWriter writer)
		{
			this.SerializeChangeset(changesetInfo, writer);
		}

		protected void SerializeTargetingTypeBasketPortfolioTargetChangeset(
			TargetingTypeBasketPortfolioTargetChangesetInfo changesetInfo, IJsonWriter writer)
		{
			this.SerializeChangeset(changesetInfo, writer);
		}

		protected void SerializePortfolioSecurityTargetOverlayChangeset(
			BgaPortfolioSecurityFactorChangesetInfo changesetInfo, IJsonWriter writer)
		{
			this.SerializeChangeset(changesetInfo, writer);
		}

        protected void SerializePortfolioSecurityTargetChangeset(
            BuPortfolioSecurityTargetChangesetInfo changesetInfo, IJsonWriter writer)
        {
            this.SerializeChangeset(changesetInfo, writer);
        }

		

		protected void SerializeBasketCountry(IJsonWriter writer, BasketCountryModel model, String discriminator, CalculationTicket ticket)
		{
			this.AddDiscriminatorIfAny(writer, discriminator);
			writer.Write(model.Basket.Id, JsonNames.BasketId);
			this.SerializeCountry(writer, model.Basket.Country);
            this.expressionWriter.SerializeOnceResolved(model.Benchmark, JsonNames.Benchmark, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.BaseActive, JsonNames.BaseActive, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Overlay, JsonNames.Overlay, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioAdjustment, JsonNames.PortfolioAdjustment, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioScaled, JsonNames.PortfolioScaled, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueExposure, JsonNames.TrueExposure, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueActive, JsonNames.TrueActive, writer, ticket);
		}

		protected void SerializeBasketRegion(IJsonWriter writer, BasketRegionModel model, String discriminator, CalculationTicket ticket)
		{
			this.AddDiscriminatorIfAny(writer, discriminator);
			writer.Write(model.Basket.Id, JsonNames.BasketId);
			writer.Write(model.Basket.Name, JsonNames.Name);
            this.expressionWriter.SerializeOnceResolved(model.Benchmark, JsonNames.Benchmark, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.BaseActive, JsonNames.BaseActive, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Overlay, JsonNames.Overlay, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioAdjustment, JsonNames.PortfolioAdjustment, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioScaled, JsonNames.PortfolioScaled, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueExposure, JsonNames.TrueExposure, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueActive, JsonNames.TrueActive, writer, ticket);
			writer.WriteArray(model.Countries, JsonNames.Countries, country =>
			{
				writer.Write(delegate
				{
					this.SerializeCountry(writer, country.Country);
                    this.expressionWriter.SerializeOnceResolved(country.Benchmark, JsonNames.Benchmark, writer, ticket);
                    this.expressionWriter.SerializeOnceResolved(country.Overlay, JsonNames.Overlay, writer, ticket);
				});
			});
		}

		protected void SerializeUnsavedBasketCountry(IJsonWriter writer, UnsavedBasketCountryModel model, String disciminator, CalculationTicket ticket)
		{
			this.AddDiscriminatorIfAny(writer, disciminator);
			this.SerializeCountry(writer, model.Country);
            this.expressionWriter.SerializeOnceResolved(model.Benchmark, JsonNames.Benchmark, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.BaseActive, JsonNames.BaseActive, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Overlay, JsonNames.Overlay, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioAdjustment, JsonNames.PortfolioAdjustment, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioScaled, JsonNames.PortfolioScaled, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueExposure, JsonNames.TrueExposure, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueActive, JsonNames.TrueActive, writer, ticket);
		}

		protected void SerializeGlobe(IJsonWriter writer, GlobeModel root, CalculationTicket ticket)
		{
            this.expressionWriter.SerializeOnceResolved(root.Benchmark, JsonNames.Benchmark, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(root.Overlay, JsonNames.Overlay, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(root.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(root.PortfolioAdjustment, JsonNames.PortfolioAdjustment, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(root.PortfolioScaled, JsonNames.PortfolioScaled, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(root.TrueExposure, JsonNames.TrueExposure, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(root.TrueActive, JsonNames.TrueActive, writer, ticket);
			writer.WriteArray(root.Residents, JsonNames.Residents, resident =>
			{
				writer.Write(delegate
				{
					resident.Accept(new IBreakdownModelResident_Resolver(this, writer, ticket));
				});
			});
		}

		protected void SerializeCash(IJsonWriter writer, CashModel cash, CalculationTicket ticket)
		{
            this.expressionWriter.SerializeOnceResolved(cash.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(cash.Scaled, JsonNames.CashScaled, writer, ticket);
		}

		protected void SerializeRegion(IJsonWriter writer, RegionModel model, String discriminator, CalculationTicket ticket)
		{
			this.AddDiscriminatorIfAny(writer, discriminator);
			writer.Write(model.Name, JsonNames.Name);
            this.expressionWriter.SerializeOnceResolved(model.Benchmark, JsonNames.Benchmark, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.BaseActive, JsonNames.BaseActive, writer, ticket);
			this.expressionWriter.SerializeOnceResolved(model.Overlay, JsonNames.Overlay, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioAdjustment, JsonNames.PortfolioAdjustment, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioScaled, JsonNames.PortfolioScaled, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueExposure, JsonNames.TrueExposure, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueActive, JsonNames.TrueActive, writer, ticket);
			writer.WriteArray(model.Residents, JsonNames.Residents, resident =>
			{
				writer.Write(delegate
				{
                    resident.Accept(new IRegionModelResident_Resolver(this, writer, ticket));
				});
			});
		}

		protected void SerializeOther(IJsonWriter writer, OtherModel model, String discriminator, CalculationTicket ticket)
		{
			this.AddDiscriminatorIfAny(writer, discriminator);
            this.expressionWriter.SerializeOnceResolved(model.Benchmark, JsonNames.Benchmark, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Base, JsonNames.Base, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.BaseActive, JsonNames.BaseActive, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.Overlay, JsonNames.Overlay, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioAdjustment, JsonNames.PortfolioAdjustment, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.PortfolioScaled, JsonNames.PortfolioScaled, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueExposure, JsonNames.TrueExposure, writer, ticket);
            this.expressionWriter.SerializeOnceResolved(model.TrueActive, JsonNames.TrueActive, writer, ticket);

			writer.WriteArray(model.BasketCountries, JsonNames.BasketCountries, basketCountry =>
			{
				writer.Write(delegate
				{
                    this.SerializeBasketCountry(writer, basketCountry, JsonNames.BasketCountry, ticket);
				});
			});

			writer.WriteArray(model.UnsavedBasketCountries, JsonNames.UnsavedBasketCountries, unsavedBasketCountry =>
			{
				writer.Write(delegate
				{
                    this.SerializeUnsavedBasketCountry(writer, unsavedBasketCountry, JsonNames.UnsavedBasketCountry, ticket);
				});
			});
		}

		protected void SerializeCountry(IJsonWriter writer, Country country)
		{
			writer.Write(JsonNames.Country, delegate
			{
				writer.Write(country.IsoCode, JsonNames.IsoCode);
				writer.Write(country.Name, JsonNames.Name);
			});
		}

		private class IBreakdownModelResident_Resolver : IGlobeResidentResolver
		{
			private ModelToJsonSerializer parent;
			private IJsonWriter writer;
            private CalculationTicket ticket;
			public IBreakdownModelResident_Resolver(ModelToJsonSerializer parent, IJsonWriter writer, CalculationTicket ticket)
			{
				this.parent = parent;
				this.writer = writer;
                this.ticket = ticket;
			}
			public void Resolve(RegionModel region)
			{
				this.parent.SerializeRegion(writer, region, JsonNames.Region, this.ticket);
			}
			public void Resolve(OtherModel other)
			{
                this.parent.SerializeOther(writer, other, JsonNames.Other, this.ticket);
			}
			public void Resolve(BasketRegionModel model)
			{
                this.parent.SerializeBasketRegion(writer, model, JsonNames.BasketRegion, this.ticket);
			}
		}

		private class IRegionModelResident_Resolver : IRegionModelResidentResolver
		{
			private ModelToJsonSerializer parent;
			private IJsonWriter writer;
            private CalculationTicket ticket;
			public IRegionModelResident_Resolver(ModelToJsonSerializer parent, IJsonWriter writer, CalculationTicket ticket)
			{
				this.parent = parent;
				this.writer = writer;
                this.ticket = ticket;
			}
			public void Resolve(BasketCountryModel basketCountry)
			{
                this.parent.SerializeBasketCountry(this.writer, basketCountry, JsonNames.BasketCountry, this.ticket);
			}
			public void Resolve(BasketRegionModel basketRegion)
			{
                this.parent.SerializeBasketRegion(this.writer, basketRegion, JsonNames.BasketRegion, this.ticket);
			}
			public void Resolve(RegionModel region)
			{
                this.parent.SerializeRegion(this.writer, region, JsonNames.Region, this.ticket);
			}
		}

		protected void AddDiscriminatorIfAny(IJsonWriter writer, String discriminator)
		{
			if (!String.IsNullOrWhiteSpace(discriminator))
			{
				writer.Write(discriminator, JsonNames.Discriminator);
			}
		}
	}
}
