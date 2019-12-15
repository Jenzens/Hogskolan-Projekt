namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using TheMagnificentModels.Utilities;

    public partial class CommisionValues
    {
        public int CommisionYear { get; set; }

        public int LowestAcqValue { get; set; }

        public int HighestAcqValue { get; set; }

        public double ChildAcqVariable { get; set; }

        public double AdultAcqVariable { get; set; }

        public int Id { get; set; }

        public List<string> RegisterCommissionValues()
        {
            List<string> errors = CheckCommissionErrors();

            if (errors.Count == 0)
            {
                if (Id > 0)
                {
                    DataManager.UpdateCommissionValues(this);
                }
                else
                {
                    DataManager.AddCommissionValues(this);
                }
            }

            return errors;
        }

        /// <summary>
        ///  Checks for errors and returns error messages if any is found
        /// </summary>
        public List<string> CheckCommissionErrors()
        {
            List<string> errors = new List<string>();

            if (CommisionYear.ToString().Length == 0)
                errors.Add("Saknar kalender�r");
            else if (CommisionYear < 0)
                errors.Add("Kalend�r m�ste vara efter kristus");

            if (LowestAcqValue.ToString().Length == 0)
                errors.Add("Saknar l�gsta akv v�rde");
            else if (LowestAcqValue < 0)
                errors.Add("L�gsta akv v�rdet f�r inte vara mindre �n 0");

            if (HighestAcqValue.ToString().Length == 0)
                errors.Add("Saknar h�gsta akv v�rde");
            else if (HighestAcqValue < 0)
                errors.Add("H�gsta akv v�rdet f�r inte vara mindre �n 0");
            else if (HighestAcqValue <= LowestAcqValue)
                errors.Add("H�gsta akv v�rdet m�ste vara h�gre �n l�gsta");

            if (ChildAcqVariable.ToString().Length == 0)
                errors.Add("Saknar barn akv variabel");
            else if (ChildAcqVariable < 0)
                errors.Add("Barn akv variabeln f�r inte vara mindre �n 0");

            if (AdultAcqVariable.ToString().Length == 0)
                errors.Add("Saknar vuxen akv variabel");
            else if (AdultAcqVariable < 0)
                errors.Add("Vuxen akv variabeln f�r inte vara mindre �n 0");

            return errors;
        }
    }
}
