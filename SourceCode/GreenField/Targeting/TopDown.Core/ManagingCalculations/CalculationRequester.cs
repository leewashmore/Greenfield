using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingCalculations
{
    public class CalculationRequester
    {
        public CalculationInfo RequestCalculation(IDataManager manager)
        {
            var targetingCOmputationIdRange = manager.ReserveTargetingComputationIds(1);
            if (!targetingCOmputationIdRange.MoveNext()) throw new ApplicationException("Unable to reserve a targeting computation ID.");
            var computationId = targetingCOmputationIdRange.Current;

            var targetingComputationInfo = new TargetingCalculationInfo(
                computationId,
                0,
                DateTime.Now, // <---- doesn't matter, isn't going to be used anyways
                null,
                null,
                null
            );
            manager.InsertTargetingComputation(targetingComputationInfo);
            return new CalculationInfo { Id = computationId };
        }
    }
}
