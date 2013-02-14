using System;
using System.Diagnostics;

namespace Aims.Core
{
    /// <summary>
    /// As of now there is no entity in the database that backs the issuer.
    /// Issuer is extracted from GF_SECURIY_BASEVIEW.
    /// </summary>
    public class Issuer
    {
        [DebuggerStepThrough]
        public Issuer(String id, String name)
        {
            this.Id = id;
            this.Name = name;
        }

        public String Id { get; private set; }
        public String Name { get; private set; }
    }
}
