using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Aims.Core.Sql
{
    public class SafeFieldWrapper<TInfo> : IField<TInfo>
    {
        private IField<TInfo> field;
        public SafeFieldWrapper(IField<TInfo> field)
        {
            this.field = field;
        }
        public void Populate(TInfo info, SqlDataReader reader, int index)
        {
            try
            {
                field.Populate(info, reader, index);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to process a value from the \"" + reader.GetName(index) + "\" column (" + index + ").", exception);
            }
        }
    }
}
