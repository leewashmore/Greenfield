using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Persisting;

namespace TopDown.Core.Persisting
{
    public interface IUsersDataManager
    {
        String GetUserEmail(String username);        
    }
}