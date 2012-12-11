using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.Gadgets.PortfolioPicker
{
    public class ProtfolioPickerManager
    {
		private ModelToJsonSerializer serializer;

        [DebuggerStepThrough]
        public ProtfolioPickerManager(ModelToJsonSerializer serializer)
        {
            this.serializer = serializer;
        }

		public RootModel GetRootModel(TargetingTypeRepository targetingTypeRepository, PortfolioRepository portfolioRepository)
		{
			var targetingTypes = targetingTypeRepository.GetTargetingTypes()
				.Select(targetingType =>
				{
					var portfolios = targetingType.BroadGlobalActivePortfolios
						.Select(x => new PortfolioModel(x.Id, x.Name));
					return new TargetingTypeModel(targetingType.Id, targetingType.Name, portfolios);
				});

			var result = new RootModel(targetingTypes);
			return result;
		}

		public String SerializeToJson(RootModel root)
		{
			var builder = new StringBuilder();

			using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
			{
				writer.WriteArray(root.TargetingTypes, targetingType =>
				{
					writer.Write(delegate
					{
						this.serializer.SerializeTargetingType(targetingType, writer);
					});
				});
			}
			var result = builder.ToString();
			return result;
		}
	}
}
