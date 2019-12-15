namespace TheMagnificentModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("InsuranceApplicationRow")]
    public partial class InsuranceApplicationRow
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InsuranceApplicationId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string InsuranceApplicationCN { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InsuranceId { get; set; }

        public double? Premium { get; set; }

        public int? BaseValue { get; set; }

        public virtual Insurance Insurance { get; set; }

        public virtual Sale Sale { get; set; }
    }
}
