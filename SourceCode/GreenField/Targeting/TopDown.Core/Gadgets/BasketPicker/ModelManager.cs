using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.Persisting;

namespace TopDown.Core.Gadgets.BasketPicker
{
    public class ModelManager
    {
        private ModelBuilder modelBuilder;
        private ModelToJsonSerializer modelSerializer;

        [DebuggerStepThrough]
        public ModelManager(ModelBuilder modelBuilder, ModelToJsonSerializer modelSerializer)
        {
            this.modelBuilder = modelBuilder;
            this.modelSerializer = modelSerializer;
        }

        public RootModel GetRootModel(IEnumerable<TargetingTypeGroup> targetingTypeGroups, IEnumerable<UsernameBasketInfo> baskets)
        {
            var result = this.modelBuilder.CreateRootModel(targetingTypeGroups, baskets);
            return result;
        }

        public String ToJson(RootModel model)
        {
            var builder = new StringBuilder();

            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.Write(delegate
                {
                    this.modelSerializer.Serialize(model, writer);
                });
            }

            var result = builder.ToString();

            return result;
        }
    }
}
