using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    public class DbContextConfiguration : DbConfiguration
    {
        public DbContextConfiguration()
        {
            var now = SqlProviderServices.Instance;
            SqlProviderServices.TruncateDecimalsToScale = false;
            this.SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }
}