using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.Persisting;
using Aims.Core;

namespace TopDown.Core.ManagingBenchmarks
{
    public class BenchmarkManager
    {
        public const String BenchmarkManagerCacheKey = "BenchmarkManager";
        private IStorage<BenchmarkRepository> benchmarkRepositoryStorage;

        public BenchmarkManager(IStorage<BenchmarkRepository> benchmarkRepositoryStorage)
        {
            this.benchmarkRepositoryStorage = benchmarkRepositoryStorage;
        }

        public BenchmarkRepository ClaimBenchmarkRepository(IOnDamand<IDataManager> ondemandManager, DateTime timestamp)
        {
			var stored = this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey];
			if (stored == null)
			{
                stored = new BenchmarkRepository(timestamp, ondemandManager.Claim());
				this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey] = stored;
			}
			else if (stored.Timestamp == timestamp)
			{
				// do nothing
			}
			else
			{
                stored = new BenchmarkRepository(timestamp, ondemandManager.Claim());
				this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey] = stored;
			}
			return stored;
        }
        
        public BenchmarkRepository ClaimBenchmarkRepository(IDataManager manager, DateTime timestamp)
        {
            var stored = this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey];
            if (stored == null)
            {
                stored = new BenchmarkRepository(timestamp, manager);
                this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey] = stored;
            }
            else if (stored.Timestamp == timestamp)
            {
                // do nothing
            }
            else
            {
                stored = new BenchmarkRepository(timestamp, manager);
                this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey] = stored;
            }
            return stored;
        }
		
        public void DropRepository()
		{
			this.benchmarkRepositoryStorage[BenchmarkManagerCacheKey] = null;
		}
    }
}
