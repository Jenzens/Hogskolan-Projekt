namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonProspectContact")]
    public partial class PersonProspectContact
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PersonNr { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InsuranceTakerId { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime ProspectContactDate { get; set; }

        public virtual InsuranceTaker InsuranceTaker { get; set; }

        public virtual PersonData PersonData { get; set; }
    }
}
