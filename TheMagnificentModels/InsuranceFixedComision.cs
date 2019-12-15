namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InsuranceFixedComision")]
    public partial class InsuranceFixedComision
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InsuranceApplicationId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string InsuranceApplicationCN { get; set; }

        public int Commision { get; set; }

        public virtual Sale Sale { get; set; }
    }
}
