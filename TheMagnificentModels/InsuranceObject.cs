namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("InsuranceObject")]
    public partial class InsuranceObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InsuranceObject()
        {
            Sale = new HashSet<Sale>();
            PersonData = new HashSet<PersonData>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObjectId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sale> Sale { get; set; }

        public virtual ICollection<PersonData> PersonData { get; set; }

        /// <summary>
        /// Get the first person in the collection
        /// </summary>
        public PersonData FirstPerson
        {
            get
            {
                try
                {
                    if (PersonData.Count > 0)
                    {
                        return PersonData.First();
                    }
                }
                catch (Exception) { }
                return null;
            }
        }
    }
}
