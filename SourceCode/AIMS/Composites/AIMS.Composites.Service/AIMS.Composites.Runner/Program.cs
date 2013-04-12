using AIMS.Composites.Service;

namespace AIMS.Composites.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var compositesOperations = new CompositesOperations();
            compositesOperations.PopulateCompositeLTHoldings();
        }
    }
}