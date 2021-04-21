using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChavez.Cerberus
{
    internal class AppConfigurations
    {
        public DbOptions DbOptions { get; set; }
        public string BaseUri { get; set; }
    }

    internal class DbOptions
    {
        public string ConnectionString { get; set; }
        public bool EnableTokenCleanup { get; set; }
        public string Schema { get; set; }
        public ushort TokenCleanupBatchSize { get; set; }
        public ushort TokenCleanupInterval { get; set; }
    }
}
