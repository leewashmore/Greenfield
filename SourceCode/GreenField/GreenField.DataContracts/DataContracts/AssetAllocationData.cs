using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract class for Asset Allocation Gadget
    /// </summary>
    [DataContract]
    public class AssetAllocationData
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public decimal? PortfolioWeight { get; set; }

        [DataMember]
        public decimal? ModelWeight { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight { get; set; }

        [DataMember]
        public decimal? ActivePosition { get; set; }

        [DataMember]
        public string PortfolioId { get; set; }
    }
}