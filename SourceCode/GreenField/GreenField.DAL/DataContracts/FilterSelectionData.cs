using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class FilterSelectionData : IEquatable<FilterSelectionData>
    {
        [DataMember]
        public String Filtertype { get; set; }

        [DataMember]
        public String FilterValues { get; set; }

        public bool Equals(FilterSelectionData other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return Filtertype.Equals(other.Filtertype) && FilterValues.Equals(other.FilterValues);
        }

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashProductName = Filtertype.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = FilterValues.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }
}