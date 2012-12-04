using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core._Testing
{
    public class RandomGenerator
    {
        private Random random;
        public RandomGenerator(Int32 seed)
        {
            this.random = new Random(seed);
        }

        public Int32 Number(Int32 min, Int32 count)
        {
            var inRange = this.random.Next(count);
            var result = inRange + min;
            return result;
        }

        public TValue Item<TValue>(TValue[] values)
        {
            var index = this.Number(0, values.Length);
            return values[index];
        }

        public TValue Item<TValue>(IEnumerable<TValue> values)
        {
            var index = this.Number(0, values.Count());
            return values.Skip(index).Take(1).Single();
        }


        public IList<Decimal> DecimalDistribution(Int32 count, Decimal scale)
        {
            var total = 0m;
            var result = new Decimal[count];
            for (var index = 0; index < count; index++)
            {
                total += result[index] = (Decimal) this.random.NextDouble();
            }
            
            for (var index = 0; index < count; index++)
            {
                result[index] = scale * (result[index] / total);
            }
            return result;
        }

        public IList<Decimal> DecimalDistribution(Int32 count)
        {
            return this.DecimalDistribution(count, 1.0m);
        }



        public IList<TValue> Pick<TValue>(IEnumerable<TValue> values, Int32 numberToPick)
        {
            var result = new List<TValue>();
            var deminishing = new List<TValue>(values);
            while (numberToPick > 0 && deminishing.Count > 0)
            {
                var index = this.Number(0, deminishing.Count);
                var picked = deminishing[index];
                deminishing.RemoveAt(index);
                result.Add(picked);
            }
            return result;
        }

        public Double Double(Double scale)
        {
            return this.random.NextDouble() * scale;
        }


        public Decimal Decimal(Decimal scale)
        {
            return ((Decimal)this.random.NextDouble()) * scale;
        }
    }
}
