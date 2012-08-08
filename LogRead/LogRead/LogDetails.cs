using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogRead
{
    public class LogDetails
    {
        public Boolean LogSelection { get; set; }
        public String LogDateTime { get; set; }
        public String LogCategory { get; set; }
        public Boolean LogUserIsLogged { get; set; }
        public String LogUserName { get; set; }
        public String LogType { get; set; }

        public String LogExceptionMessage { get; set; }
        public String LogExceptionStackTrace { get; set; }

        public String LogMethodNameSpace { get; set; }
        public String LogArgumentIndex { get; set; }
        public String LogArgumentValue { get; set; }

        public String LogAccountDetail { get; set; }
        public String LogRoleDetail { get; set; }

        public String LogTimeStamp { get; set; }
    }
}
