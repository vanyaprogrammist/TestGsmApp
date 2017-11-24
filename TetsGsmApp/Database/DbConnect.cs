using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetsGsmApp.Database
{
    public class Phone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
    }

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class MobileContext : DbContext
    {
        public MobileContext() : base("conn")
        { }

        public DbSet<Phone> Phones { get; set; }
    }
}
