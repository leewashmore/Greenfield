using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract(IsReference = false)]
    public class MembershipUserInfo
    {
        [DataMemberAttribute(Name = "UserName", IsRequired = true)]
        public string UserName { get; set; }

        [DataMemberAttribute(Name = "Email", IsRequired = false)]
        public string Email { get; set; }

        [DataMemberAttribute(Name = "IsApproved", IsRequired = false)]
        public bool IsApproved { get; set; }

        [DataMemberAttribute(Name = "IsLockedOut", IsRequired = false)]
        public bool IsLockedOut { get; set; }

        [DataMemberAttribute(Name = "IsOnline", IsRequired = false)]
        public bool IsOnline { get; set; }

        [DataMemberAttribute(Name = "Comment", IsRequired = true)]
        public string Comment { get; set; }
        
        [DataMemberAttribute(Name = "CreateDate", IsRequired = false)]
        public DateTime CreateDate { get; set; }

        [DataMemberAttribute(Name = "LastActivityDate", IsRequired = false)]
        public DateTime LastActivityDate { get; set; }

        [DataMemberAttribute(Name = "LastLockOutDate", IsRequired = false)]
        public DateTime LastLockOutDate { get; set; }

        [DataMemberAttribute(Name = "LastLogInDate", IsRequired = false)]
        public DateTime LastLogInDate { get; set; }

        [DataMemberAttribute(Name = "ProviderUserKey", IsRequired = true)]
        public string ProviderUserKey { get; set; }
        
        [DataMemberAttribute(Name = "ProviderName", IsRequired = false)]
        public string ProviderName { get; set; }

        [DataMemberAttribute(Name = "PasswordQuestion", IsRequired = true)]
        public string PasswordQuestion { get; set; }

        [DataMemberAttribute(Name = "LastPassWordChangedDate", IsRequired = false)]
        public DateTime LastPassWordChangedDate { get; set; }


    }
}
