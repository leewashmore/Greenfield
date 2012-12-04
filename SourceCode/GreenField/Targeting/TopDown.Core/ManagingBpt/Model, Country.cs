using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingCountries;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class CountryModel : IModel
    {
        [DebuggerStepThrough]
        public CountryModel(Country country, Decimal defaultBenchmark, Decimal defaultOverlay, CommonParts commonParts)
        {
            this.Country = country;
            this.Benchmark = new UnchangableExpression<Decimal>(
				ValueNames.Benchmark,
				defaultBenchmark,
				commonParts.DecimalValueAdapter,
				commonParts.ValidateWhatever
			);
            this.Overlay = new UnchangableExpression<Decimal>(
				ValueNames.Overlay,
				defaultOverlay,
				commonParts.DecimalValueAdapter,
				commonParts.ValidateWhatever
			);
        }

        public Country Country { get; private set; }
        public UnchangableExpression<Decimal> Benchmark { get; private set; }
        public UnchangableExpression<Decimal> Overlay { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IModelResolver resolver)
        {
            resolver.Resolve(this);
        }
	}
}
