namespace MachinTest_2WAY.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TblProductList")]
    public partial class TblProductList
    {
        [Key]
        public long ProductId { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        public long? categoryId { get; set; }

        [StringLength(50)]
        public string CategoryName { get; set; }
    }
}
