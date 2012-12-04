using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core
{
    public abstract class ModelToJsonSerializerBase
    {
        protected void SerializeChangeset(ChangesetInfoBase changesetInfo, IJsonWriter writer)
        {
            writer.Write(changesetInfo.Id, JsonNames.Id);
            writer.Write(changesetInfo.Username, JsonNames.Username);
            writer.Write(changesetInfo.Timestamp, JsonNames.Timestamp);
            writer.Write(changesetInfo.CalculationId, JsonNames.CalcualtionId);
        }
    }
}
