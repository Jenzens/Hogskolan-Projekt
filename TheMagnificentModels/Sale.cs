namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using TheMagnificentModels.Utilities;

    [Table("Sale")]
    public partial class Sale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sale()
        {
            InsuranceApplicationRows = new HashSet<InsuranceApplicationRow>();
        }

        /// <summary>
        /// Register a new sale
        /// </summary>
        /// <param name="user"></param>
        /// <param name="insuranceTaker"></param>
        /// <param name="insuranceObject"></param>
        /// <returns>List of errors or empty on success</returns>
        public List<string> RegisterSale(Users user, List<InsuranceApplicationRow> appRows, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            List<string> errors = new List<string>();
            errors.AddRange(CheckSaleErrors(user, appRows));
            if (insuranceTaker.CompanyData.Count > 0)
            {
                foreach ( CompanyData company in insuranceTaker.CompanyData )
                {
                    errors.AddRange(company.CheckInsuranceTakerErrors());
                }
            }
            if (insuranceTaker.PersonData.Count > 0)
            {
                foreach ( PersonData person in insuranceTaker.PersonData )
                {
                    errors.AddRange(person.CheckInsuranceTakerErrors());
                    foreach (PersonData iobject in insuranceObject.PersonData)
                    {
                        if (person.PersonNr == iobject.PersonNr)
                            errors.Add("Sale - Försäkringstagare och försäkringsobjekt får inte vara samma person");
                    }
                }
            }
            foreach ( PersonData iobject in insuranceObject.PersonData )
            {
                errors.AddRange(iobject.CheckInsuranceObjectErrors());
            }

            if (appRows[0].Insurance == null)
                errors.Add("Sale - Saknar försäkrings val");

            RegDate = DateTime.Now; //Current time to know when it got registered

            if (errors.Count == 0)
            {
                if (appRows[1].Insurance != null)
                {
                    //check if addon is valid for the insurance otherwise remove the addon
                    var found = false;
                    foreach( var item in appRows[1].Insurance.Insurance1 )
                    {
                        if( item.InsuranceId == appRows[0].Insurance.InsuranceId )
                        {
                            found = true;
                        }
                    }
                    if (found) // the addon is ok, so just send the approws that was sent
                    {
                        if (appRows.ElementAt(1).BaseValue.HasValue)
                            if (appRows.ElementAt(1).BaseValue < 0)
                                errors.Add("Addon - Grundbeloppet får inte lägre än 0"); // Be sure the addon is actually Available at first then check error
                        if (errors.Count != 0) return errors;
                        DataManager.CreateSale(this, appRows, user, insuranceTaker, insuranceObject);
                    }
                    else // the addon was not ok, only send the normal insurance
                    {
                        appRows.RemoveAt(1); // remove the addon
                        DataManager.CreateSale(this, appRows, user, insuranceTaker, insuranceObject);
                    }
                }
                else // no addon was selected just send whole approws
                {
                    DataManager.CreateSale(this, appRows, user, insuranceTaker, insuranceObject);
                }
            }

            return errors;
        }

        /// <summary>
        /// Update an existing sale
        /// </summary>
        /// <param name="user"></param>
        /// <param name="appRows"></param>
        /// <param name="insuranceTaker"></param>
        /// <param name="insuranceObject"></param>
        /// <returns>List of errors or empty on success</returns>
        public List<string> UpdateSale(Users user, List<InsuranceApplicationRow> appRows, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            List<string> errors = new List<string>();
            errors.AddRange(CheckSaleErrors(user, appRows));
            if (insuranceTaker.CompanyData.Count > 0)
            {
                foreach (CompanyData company in insuranceTaker.CompanyData)
                {
                    errors.AddRange(company.CheckInsuranceTakerErrors());
                }
            }
            if (insuranceTaker.PersonData.Count > 0)
            {
                foreach (PersonData person in insuranceTaker.PersonData)
                {
                    errors.AddRange(person.CheckInsuranceTakerErrors());
                    foreach (PersonData iobject in insuranceObject.PersonData)
                    {
                        if (person.PersonNr == iobject.PersonNr)
                            errors.Add("Sale - Försäkringstagare och försäkringsobjekt får inte vara samma person");
                    }
                }
            }
            foreach (PersonData iobject in insuranceObject.PersonData)
            {
                errors.AddRange(iobject.CheckInsuranceObjectErrors());
            }

            if (appRows[0].Insurance == null)
                errors.Add("Sale - Saknar försäkrings val");

            RegDate = DateTime.Now; // current time to know when it got registered

            if (errors.Count == 0)
            {
                if (appRows.ElementAt(1).Insurance != null)
                {
                    //check if addon is valid for the insurance otherwise remove the addon
                    var found = false;
                    foreach (var item in appRows.ElementAt(1).Insurance.Insurance1)
                    {
                        if (item.InsuranceId == appRows.ElementAt(0).Insurance.InsuranceId)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) // the addon is ok, so just send the approws that was sent
                    {
                        if (appRows.ElementAt(1).BaseValue.HasValue)
                            if (appRows.ElementAt(1).BaseValue < 0)
                                errors.Add("Addon - Grundbeloppet får inte lägre än 0"); // Be sure the addon is actually Available at first then check error
                        if (errors.Count != 0) return errors;
                        DataManager.UpdateSale(this, appRows, insuranceTaker, insuranceObject);
                    }
                    else // the addon was not ok, only send the normal insurance
                    {
                        appRows.RemoveAt(1); // remove the addon
                        DataManager.UpdateSale(this, appRows, insuranceTaker, insuranceObject);
                    }
                }
                else // no addon was selected just send whole approws
                {
                    appRows.RemoveAt(1); // remove the addon object
                    DataManager.UpdateSale(this, appRows, insuranceTaker, insuranceObject);
                }
            }

            return errors;
        }

        /// <summary>
        /// Check so all fields are correct and return errors accordingly
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <returns>A list of errors, or empty if no issues</returns>
        public List<string> CheckSaleErrors(Users user, List<InsuranceApplicationRow> appRows)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(InsuranceCompany))
                errors.Add("Sale - Saknar försäkringsbolag");

            if (user == null || user.AgentNumber < 1)
                errors.Add("Sale - Saknar försäljarensuppgifter");

            if (PaymentType < 1)
                errors.Add("Sale - Saknar betalningssätt");

            if (appRows.ElementAt(0).Insurance == null)
                errors.Add("Sale - Saknar val av försäkring");
            else
            {
                if (appRows.ElementAt(0).Premium.HasValue)
                    if (appRows.ElementAt(0).Premium < 0)
                        errors.Add("Sale - Premie får inte vara lägre än 0");

                if (appRows.ElementAt(0).Insurance.InsuranceBaseValues.Count > 0)
                    if (!appRows.ElementAt(0).BaseValue.HasValue)
                        errors.Add("Sale - Du måste välja ett grundbelopp");
            }

            if (StartDate.HasValue && ExpiaryDate.HasValue)
            {
                if (StartDate.Value.Ticks > ExpiaryDate.Value.Ticks)
                    errors.Add("Sale - Utgångsdatumet kan inte vara tidigare än start datumet.");
            }

            return errors;
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceApplicationId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string InsuranceCompany { get; set; }

        public int InsuranceTakerId { get; set; }

        public int InsuranceObjectId { get; set; }

        public int AgentId { get; set; }

        public int PaymentType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiaryDate { get; set; }

        [Column(TypeName = "text")]
        public string Comments { get; set; }

        public DateTime RegDate { get; set; }

        public virtual InsuranceFixedComision InsuranceFixedComision { get; set; }

        public virtual InsuranceObject InsuranceObject { get; set; }

        public virtual InsuranceTaker InsuranceTaker { get; set; }

        public virtual PaymentTypes PaymentTypes { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceApplicationRow> InsuranceApplicationRows { get; set; }

        /// <summary>
        /// Get Insurance, returns null if not found
        /// </summary>
        public InsuranceApplicationRow FirstAPItem
        {
            get
            {
                try
                {
                    if (InsuranceApplicationRows.Count > 0)
                    {
                        return InsuranceApplicationRows.First();
                    }
                } catch (Exception) { }
                return null;
            }
        }

        /// <summary>
        /// Get Insurance Addon, returns null if not found
        /// </summary>
        public InsuranceApplicationRow SecondAPItem
        {
            get
            {
                try
                {
                    if (InsuranceApplicationRows.Count > 1)
                    {
                        return InsuranceApplicationRows.ElementAt(1);
                    }
                }
                catch (Exception) { }
                return null;
            }
        }

    }
}
