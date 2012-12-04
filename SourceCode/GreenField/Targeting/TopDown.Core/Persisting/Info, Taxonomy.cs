using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the TAXONOMY table.
    /// </summary>
    public class TaxonomyInfo
    {
        [DebuggerStepThrough]
        public TaxonomyInfo()
        {
        }

        [DebuggerStepThrough]
        public TaxonomyInfo(Int32 id, String definitionXml)
        {
            this.Id = id;
            this.DefinitionXml = definitionXml;
        }

        public Int32 Id { get; set; }
        public String DefinitionXml { get; set; }
    }
}
