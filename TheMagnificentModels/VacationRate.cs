namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using TheMagnificentModels.Utilities;

    [Table("VacationRate")]
    public partial class VacationRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StartDate { get; set; }

        [Column("VacationRate")]
        public double VacationRate1 { get; set; } // 1 because of otherwise conflict with name

        [Column("VacationVar")]
        public double VacationVariable { get; set; }

        /// <summary>
        /// Try to register vacationrate 
        /// </summary>
        /// <returns></returns>
        public List<string> TryRegister()
        {
            List<string> errors = CheckErrors();

            if (errors.Count == 0) // if no erros, add to database
            {
                if (!DataManager.AddVacationRate(this))
                {
                    if (!DataManager.UpdateVactionRate(this))
                    {
                        errors.Add("Sparandet av semesterersättningen misslyckades");
                    }
                }
            }
            return errors;
        }

        /// <summary>
        /// Control values
        /// </summary>
        /// <returns>List of errors</returns>
        public List<string> CheckErrors()
        {
            List<string> errors = new List<string>();

            if (StartDate < 0)
                errors.Add("Kan inte vara förekristus");

            if (VacationRate1 < 0)
                errors.Add("Semesterersättningen kan inte mindre än 0");
            else if (VacationRate1 >= 100)
                errors.Add("Semesterersättningen kan inte vara mer än 100");

            if (VacationVariable < 0)
                errors.Add("Semesterersättnings marginalen kan inte mindre än 0");
            else if (VacationVariable >= 100)
                errors.Add("Semesterersättnings marginalen kan inte vara mer än 100");

            if (VacationVariable > VacationRate1)
                errors.Add("Marginalen kan inte vara större än ersättningen");


            return errors;
        }
    }
}
