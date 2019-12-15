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

    [Table("CompanyData")]
    public partial class CompanyData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyData()
        {
            InsuranceTaker = new HashSet<InsuranceTaker>();
        }

        Regex CharacterRegex = new Regex(@"^[\p{L} \.'\-]+$");
        Regex NumberRegex = new Regex(@"^(\+?46|0)7[\d\-\s]{8,14}$");


        /// <summary>
        ///  Checks for errors and returns errormessages if any is found
        /// </summary>
        public List<string> CheckInsuranceTakerErrors()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(CompanyName))
                errors.Add("F�rs�kringstagare - Saknar F�retagsnamn");
            else if (!CharacterRegex.Match(CompanyName).Success)
                errors.Add("F�rs�kringstagare - F�retagsnamnet f�r endast inneh�lla bokst�ver");

            if (string.IsNullOrEmpty(Phonenumber))
                errors.Add("F�rs�kringstagare - Saknar Telefonnummer");
            else if (!NumberRegex.Match(Phonenumber).Success)
                errors.Add("F�rs�kringstagare - Telefonnumret f�r inte inneh�lla bokst�ver");

            if (string.IsNullOrEmpty(Faxnumber))
                errors.Add("F�rs�kringstagare - Saknar Faxnummer");

            if (string.IsNullOrEmpty(Email))
                errors.Add("F�rs�kringstagare - Saknar Email");

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

            if (OrgNr.ToString().Length <= 1)
                errors.Add("F�rs�kringstagare - Saknar Organistationsnummer");

            if (CompanyContactData == null)
                errors.Add("F�rs�kringstagare - Saknar kontaktperson");
            else
            {
                if (string.IsNullOrEmpty(CompanyContactData.Email))
                    errors.Add("F�rs�kringstagare - Kontaktperson - Saknar Email");

                if (string.IsNullOrEmpty(CompanyContactData.Firstname))
                    errors.Add("F�rs�kringstagare - Kontaktperson - Saknar F�rnamn");
                else if (!CharacterRegex.Match(CompanyContactData.Firstname).Success)
                    errors.Add("F�rs�kringstagare - Kontaktperson - F�rnamn f�r endast inneh�lla bokst�ver");

                if (string.IsNullOrEmpty(CompanyContactData.Lastname))
                    errors.Add("F�rs�kringstagare - Kontaktperson - Saknar Efternamn");
                else if (!CharacterRegex.Match(CompanyContactData.Lastname).Success)
                    errors.Add("F�rs�kringstagare - Kontaktperson - Efternamn f�r endast inneh�lla bokst�ver");

                if (string.IsNullOrEmpty(CompanyContactData.Phonenumber))
                    errors.Add("F�rs�kringstagare - Kontaktperson - Saknar Telefonnummer");
                else if (!NumberRegex.Match(Phonenumber).Success)
                    errors.Add("F�rs�kringstagare - Kontaktperson - Telefonnumret f�r inte inneh�lla bokst�ver");
            }

            return errors;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrgNr { get; set; }

        [Required]
        [StringLength(128)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(50)]
        public string Phonenumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Faxnumber { get; set; }

        [Required]
        [StringLength(98)]
        public string Email { get; set; }

        public int Zipcode { get; set; }

        [Required]
        [StringLength(64)]
        public string StreetAdress { get; set; }

        public int ContactId { get; set; }

        public virtual CompanyContactData CompanyContactData { get; set; }

        public virtual ZipCity ZipCity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceTaker> InsuranceTaker { get; set; }

        public string Name
        {
            get { return CompanyName; }
        }

        public string IdNumber
        {
            get { return "F�retag: " + OrgNr; }
        }

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
