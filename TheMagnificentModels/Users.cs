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

    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            Sale = new HashSet<Sale>();
            Roles = new HashSet<Roles>();
        }

        Regex CharacterRegex = new Regex(@"^[\p{L} \.'\-]+$");
        Regex NumberRegex = new Regex(@"^(\+?46|0)7[\d\-\s]{8,14}$");

        /// <summary>
        /// Method for logging in to the system
        /// </summary>
        /// <param name="agencyNr"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Users LoginUser(int agencyNr, string password)
        {
            var user = DataManager.GetUser(agencyNr);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }
            else return new Users();
        }

        /// <summary>
        /// Checks wether a user has the proper credentials.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsUserAuthorized(string roleName)
        {
            foreach (var role in this.Roles)
            {
                // incase uppercase in DB and not on request
                if (string.Equals(role.Permission, roleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method for registering a new user in the system
        /// </summary>
        /// <returns>Fel meddelande annars tom</returns>
        public List<string> RegisterNewUser()
        {
            List<string> errors = CheckErrors();

            if (errors.Count == 0) // If no errors, add to database
            {
                Password = BCrypt.Net.BCrypt.HashPassword(Password);
                if (!DataManager.AddUser(this))
                {
                    errors.Add("Användaren kunde inte skapas, kontrollera att person nummret inte används av en annan person.");
                }
            }
            return errors;
        }

        /// <summary>
        ///  Method for updating an existing user in the system
        /// </summary>
        /// <returns></returns>
        public List<string> UpdateUser()
        {
            List<string> errors = CheckErrors();

            if (errors.Count == 0) // If no errors, add to database
            {
                if (!DataManager.UpdateUser(this))
                {
                    errors.Add("Uppdateringen av användaren misslyckades");
                }
            }
            return errors;
        }

        /// <summary>
        /// Kontrollera errors - separerad då detta används på flera ställen
        /// Method for checking errors. Seperated since this is used in multiple locations.
        /// </summary>
        /// <returns>List of errormessages</returns>
        public List<string> CheckErrors()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(Firstname))
                errors.Add("Saknar förnamn");
            else if (!CharacterRegex.Match(Firstname).Success)
                errors.Add("Förnamn får endast innehålla bokstäver");

            if (string.IsNullOrEmpty(Lastname))
                errors.Add("Saknar efternamn");
            else if (!CharacterRegex.Match(Lastname).Success)
                errors.Add("Förnamn får endast innehålla bokstäver");

            if (string.IsNullOrEmpty(Password))
                errors.Add("Saknar lösernord");

            if (Taxrate < 0)
                errors.Add("Skattesatsen måste vara över 0");
            else if (Taxrate >= 100)
                errors.Add("Skattesatsen måste vara mindre än 100");

            if (PersonNr.ToString().Length != 12)
                errors.Add("Personnumret måste vara 12 tecken årårmmddxxxx");

            if (string.IsNullOrEmpty(ZipCity.City))
                errors.Add("Saknar stad");

            if (string.IsNullOrEmpty(StreetAdress))
                errors.Add("Saknar adress");

            if (Zipcode.ToString().Length != 5)
                errors.Add("Postnummer måste vara 5 tecken xxxxx");

            if (Zipcode.ToString().Length == 5)
            {
                var zcity = DataManager.GetZipCity(Zipcode);
                if (zcity != null && !zcity.City.Equals(ZipCity.City))
                {
                    errors.Add("Detta postnummer existerar redan, men med en annan stad.");
                }
            }

            if (string.IsNullOrEmpty(Phonenumber))
                errors.Add("Saknar ett telefonnummer");
            else if (!NumberRegex.Match(Phonenumber).Success)
                errors.Add("Telefonnumret får inte innehålla bokstäver");


            return errors;
        }

        /// <summary>
        /// Method for changing a user password
        /// </summary>
        /// <param name="oldpw"></param>
        /// <param name="newpw"></param>
        /// <param name="newpw2"></param>
        /// <returns>Message confirming an action</returns>
        public string ChangePassword(string oldpw, string newpw, string newpw2)
        {
            if (string.IsNullOrEmpty(newpw) || string.IsNullOrEmpty(newpw2) || !String.Equals(newpw, newpw2))
                return "Nya lösernordet stämmer inte överns med varandra"; // fixa texten här.
            var tuser = DataManager.GetUser(this.AgentNumber); // in case someone else changes password at the same time.
            if (string.IsNullOrEmpty(oldpw))
            {
                return "Du måste ange ditt gamla lösernord";
            }
            else if (BCrypt.Net.BCrypt.Verify(oldpw, tuser.Password))
            {
                this.Password = BCrypt.Net.BCrypt.HashPassword(newpw);
                DataManager.UpdateUser(this);
                return "Lösenordet har ändrats.";
            }
            else
            {
                return "Det gamla lösenordet stämde inte överens, testa igen.";
            }
        }

        /// <summary>
        /// Force update a users password (via admin page)
        /// </summary>
        /// <param name="newpw"></param>
        /// <param name="newpw2"></param>
        /// <returns>Message confirming an action</returns>
        public string ChangePassword(string newpw, string newpw2)
        {
            if (!String.Equals(newpw, newpw2))
                return "Nya lösernordet stämmer inte överns med varandra"; // fixa texten här.
            var tuser = DataManager.GetUser(this.AgentNumber); // in case someone else changes password at the same time.
            this.Password = BCrypt.Net.BCrypt.HashPassword(newpw);
            DataManager.UpdateUser(this);
            return "Lösenordet har ändrats.";
        }

        public string Fullname
        {
            get { return Firstname + " " + Lastname; }
        }

        public string UserIdAndName
        {
            get { return AgentNumber + " - " + Fullname; }
        }

        /// <summary>
        /// Get the main role
        /// </summary>
        public string GetMainRole
        {
            get
            {
                try
                {
                    if (Roles.Count > 0)
                    {
                        return Roles.First().Permission;
                    }
                }
                catch (Exception) { }
                return "";
            }
        }

        [Key]
        public int AgentNumber { get; set; }

        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string Lastname { get; set; }

        [Required]
        [StringLength(98)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Phonenumber { get; set; }

        public double Taxrate { get; set; }

        public int Zipcode { get; set; }

        [Required]
        [StringLength(64)]
        public string StreetAdress { get; set; }

        public long PersonNr { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sale> Sale { get; set; }

        public virtual ZipCity ZipCity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Roles> Roles { get; set; }
    }
}
