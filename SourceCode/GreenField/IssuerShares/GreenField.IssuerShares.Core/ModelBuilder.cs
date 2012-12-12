using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using GreenField.IssuerShares.Core.Persisting;

namespace GreenField.IssuerShares.Core
{
    public class ModelBuilder
    {
        public const Char YesPreferred = 'Y';
        public const Char NoPreferred = 'N';

        public ItemModel CreateItem(IssuerSharesCompositionInfo compositionInfo, SecurityRepository securityRepository)
        {
            var security = securityRepository.GetSecurity(compositionInfo.SecurityId);
            var preferred = this.TurnPreferredCharToBoolean(compositionInfo.Preferred);
            
            var result = new ItemModel(
                security,
                preferred
            );
            return result;
        }

        public Boolean TurnPreferredCharToBoolean(Char preferred)
        {
            Boolean result;
            if (preferred == YesPreferred)
            {
                result = true;
            }
            else if (preferred == NoPreferred)
            {
                result = false;
            }
            else
            {
                throw new ApplicationException("Unexpected value of the Preferred field: \"" + preferred + "\". Only \"" + YesPreferred + "\" and \"" + NoPreferred + "\" are supported.");
            }
            return result;
        }
    }
}
