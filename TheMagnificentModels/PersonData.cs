namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Text.RegularExpressions;
    using TheMagnificentModels.Utilities;

    [Table("PersonData")]
    public partial class PersonData
    {   
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonData()
        {
            PersonProspectContact = new HashSet<PersonProspectContact>();
            InsuranceObject = new HashSet<InsuranceObject>();
            InsuranceTaker = new HashSet<InsuranceTaker>();
        }

        Regex CharacterRegex = new Regex(@"^[\p{L}\s\.'\-]+$");
        Regex NumberRegex = new Regex(@"^(\+?46|0)7[\d\-\s]{8,14}$");

        /// <summary>
        /// Checks for errors and returns errormessages if any is found
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <returns></returns>
        public List<string> CheckInsuranceTakerErrors()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(Firstname))
                errors.Add("F�rs�kringstagare - Saknar f�rnamn");
            else if (!CharacterRegex.Match(Firstname).Success)
                errors.Add("F�rs�kringstagare - F�rnamn f�r endast inneh�lla bokst�ver");

            if (string.IsNullOrEmpty(Lastname))
                errors.Add("F�rs�kringstagare - Saknar efternamn");
            else if (!CharacterRegex.Match(Lastname).Success)
                errors.Add("F�rs�kringstagare - F�rnamn f�r endast inneh�lla bokst�ver");

            if (PersonNr.ToString().Length != 12)
                errors.Add("F�rs�kringstagare - Personnumret m�ste vara 12 tecken �r�rmmddxxxx");

            if (string.IsNullOrEmpty(StreetAdress) || ZipCity == null || string.IsNullOrEmpty(ZipCity.City) || Zipcode.ToString().Length != 5)
            {
                errors.Add("F�rs�kringstagare - Saknar Fullst�ndig adress");
            }
            else
            {
                var zcity = DataManager.GetZipCity((int)Zipcode);
                if (zcity != null && !zcity.City.Equals(ZipCity.City))
                {
                    errors.Add("F�rs�kringstagare - Detta postnummer existerar redan, men med en annan stad.");
                }
            }

            if (string.IsNullOrEmpty(Phonenumber))
                errors.Add("F�rs�kringstagare - Saknar ett telefonnummer");
            else if (!NumberRegex.Match(Phonenumber).Success)
                errors.Add("F�rs�kringstagare - Telefonnumret f�r inte inneh�lla bokst�ver");

            if (!string.IsNullOrEmpty(Homenumber))
                if (!NumberRegex.Match(Homenumber).Success)
                    errors.Add("F�rs�kringstagare - Hemnumret f�r inte inneh�lla bokst�ver");

            return errors;
        }

        /// <summary>
        /// Checks for errors and returns errormessages if any is found
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <returns></returns>
        public List<string> CheckInsuranceObjectErrors()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(Firstname))
                errors.Add("F�rs�kringsObjekt - Saknar f�rnamn");
            else if (!CharacterRegex.Match(Firstname).Success)
                errors.Add("F�rs�kringsObjekt - F�rnamn f�r endast inneh�lla bokst�ver");

            if (string.IsNullOrEmpty(Lastname))
                errors.Add("F�rs�kringsObjekt - Saknar efternamn");
            else if (!CharacterRegex.Match(Lastname).Success)
                errors.Add("F�rs�kringsObjekt - F�rnamn f�r endast inneh�lla bokst�ver");

            if (PersonNr.ToString().Length != 12)
                errors.Add("F�rs�kringsObjekt - Personnumret m�ste vara 12 tecken �r�rmmddxxxx");

            return errors;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PersonNr { get; set; }

        [Required]
        [StringLength(50)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string Lastname { get; set; }

        [StringLength(50)]
        public string Phonenumber { get; set; }

        [StringLength(50)]
        public string Homenumber { get; set; }

        [StringLength(98)]
        public string Email { get; set; }

        public int? Zipcode { get; set; }

        [StringLength(64)]
        public string StreetAdress { get; set; }

        public virtual ZipCity ZipCity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonProspectContact> PersonProspectContact { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceObject> InsuranceObject { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceTaker> InsuranceTaker { get; set; }
        /// <summary>
        /// Gets FirstName and Lastname of a person
        /// </summary>
        public string Name
        {
            get { return Firstname + " " + Lastname; }
        }
        /// <summary>
        /// Gets PersonNr of a person
        /// </summary>
        public string IdNumber
        {
            get { return "Privatperson: " + PersonNr; }
        }
        /// <summary>
        /// Gets StreetAdress, Zipcode and ZipCity of a person
        /// </summary>
        public string Adress
        {
            get { return StreetAdress + " " + Zipcode + ", " + ZipCity.City; }
        }

        /// <summary>
        /// Get the customer ID of the person from its InsuranceTaker object (0 if not existing)
        /// </summary>
        public int CustomerID
        {
            get
            {
                try
                {
                    if (InsuranceTaker.Count > 0)
                    {
                        return InsuranceTaker.First().Id;
                    }
                }
                catch (Exception) { }
                return 0;
            }
        }

    }
}
