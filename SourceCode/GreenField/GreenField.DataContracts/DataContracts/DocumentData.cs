using System;
using System.Net;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace GreenField.DataContracts
{
    [DataContract]
    public class DocumentCatalogData
    {
        [DataMember]
        public Object FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public String FilePath { get; set; }
        
        [DataMember]
        public String FileMetaTags{ get; set; }
        
        [DataMember]
        public String FileUploadedBy { get; set; }

        [DataMember]
        public DateTime FileUploadedOn { get; set; }
    }

    [DataContract]
    public class DocumentCategoricalData
    {
        [DataMember]
        public DocumentCatalogData DocumentCatalogData { get; set; }

        [DataMember]
        public String DocumentCompanyName { get; set; }

        [DataMember]
        public String DocumentSecurityName { get; set; }

        [DataMember]
        public String DocumentSecurityTicker { get; set; }

        [DataMember]
        public DocumentCategoryType DocumentCategoryType { get; set; }

        [DataMember]
        public List<CommentDetails> CommentDetails { get; set; }
    }

    [DataContract(Name = "DocumentCategoryType")]
    public enum DocumentCategoryType
    {
        [EnumMember]
        [DescriptionAttribute("Company Meeting Notes")]
        COMPANY_MEETING_NOTES = 1,

        [EnumMember]
        [DescriptionAttribute("Company Issued Documents")]
        COMPANY_ISSUED_DOCUMENTS = 2,

        [EnumMember]
        [DescriptionAttribute("Earning Calls")]
        EARNING_CALLS = 3,

        [EnumMember]
        [DescriptionAttribute("Models")]
        MODELS = 4,

        [EnumMember]
        [DescriptionAttribute("IC Presentations")]
        IC_PRESENTATIONS = 5,

        [EnumMember]
        [DescriptionAttribute("Broker reports")]
        BROKER_REPORTS = 6,

        [EnumMember]
        [DescriptionAttribute("Company Financial Filings")]
        COMPANY_FINANCIAL_FILINGS = 7,

        [EnumMember]
        [DescriptionAttribute("Blog")]
        BLOG = 8,
    }

    [DataContract]
    public class CommentDetails
    {
        [DataMember]
        public String Comment { get; set; }

        [DataMember]
        public String CommentBy { get; set; }

        [DataMember]
        public DateTime CommentOn { get; set; }
    }

}
