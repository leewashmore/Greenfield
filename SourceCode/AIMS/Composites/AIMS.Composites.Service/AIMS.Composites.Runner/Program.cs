using AIMS.Composites.Service;

namespace AIMS.Composites.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dumper = new ConsoleDumper();

            var compositesOperations = new CompositesOperations(dumper);
            compositesOperations.TruncateCompositeLTHoldings();
            compositesOperations.PopulateCompositeLTHoldings();
        }
    }
}