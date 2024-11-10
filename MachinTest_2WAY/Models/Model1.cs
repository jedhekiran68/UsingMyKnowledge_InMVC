using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MachinTest_2WAY.Models
{
    public partial class Model1 : DbContext
    {
        public Model1(string connectionString) : base(connectionString)
        {
        }
        public virtual DbSet<TblCategoryList> TblCategoryLists { get; set; }
        public virtual DbSet<TblProductList> TblProductLists { get; set; }
    }
}

