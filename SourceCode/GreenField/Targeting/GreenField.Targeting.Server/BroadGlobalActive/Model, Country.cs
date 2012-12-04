using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract(Name = "BgaCountryModel")]
    public class CountryModel : GlobeResident
    {
        [DebuggerStepThrough]
        public CountryModel()
        {
        }

        [DebuggerStepThrough]
        public CountryModel(
            ExpressionModel benchmarkExpression,
            Server.CountryModel country,
            ExpressionModel overlayExpression
        )
            : this()
        {
            this.Benchmark = benchmarkExpression;
            this.Country = country;
            this.Overlay = overlayExpression;
        }

        [DataMember]
        public ExpressionModel Benchmark { get; set; }

        [DataMember]
        public Server.CountryModel Country { get; set; }

        [DataMember]
        public ExpressionModel Overlay { get; set; }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
