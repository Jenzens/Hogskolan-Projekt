namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("InsuranceTaker")]
    public partial class InsuranceTaker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InsuranceTaker()
        {
            Sale = new HashSet<Sale>();
            PersonProspectContact = new HashSet<PersonProspectContact>();
            CompanyData = new HashSet<CompanyData>();
            PersonData = new HashSet<PersonData>();
        }

        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sale> Sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonProspectContact> PersonProspectContact { get; set; }

        public virtual ICollection<CompanyData> CompanyData { get; set; }

        public virtual ICollection<PersonData> PersonData { get; set; }

        /// <summary>
        /// Get the first PersonData in the collection
        /// </summary>
        public PersonData GetPersonData
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
        /// <summary>
        /// Get the first CompanyData in the collection
        /// </summary>
        public CompanyData GetCompanyData
        {
            get
            {
                try
                {
                    if (CompanyData.Count > 0)
                    {
                        return CompanyData.First();
                    }
                }
                catch (Exception) { }
                return null;
            }
        }

    }
}
