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
                errors.Add("Saknar kalenderår");
            else if (CommisionYear < 0)
                errors.Add("Kalendår måste vara efter kristus");

            if (LowestAcqValue.ToString().Length == 0)
                errors.Add("Saknar lägsta akv värde");
            else if (LowestAcqValue < 0)
                errors.Add("Lägsta akv värdet får inte vara mindre än 0");

            if (HighestAcqValue.ToString().Length == 0)
                errors.Add("Saknar högsta akv värde");
            else if (HighestAcqValue < 0)
                errors.Add("Högsta akv värdet får inte vara mindre än 0");
            else if (HighestAcqValue <= LowestAcqValue)
                errors.Add("Högsta akv värdet måste vara högre än lägsta");

            if (ChildAcqVariable.ToString().Length == 0)
                errors.Add("Saknar barn akv variabel");
            else if (ChildAcqVariable < 0)
                errors.Add("Barn akv variabeln får inte vara mindre än 0");

            if (AdultAcqVariable.ToString().Length == 0)
                errors.Add("Saknar vuxen akv variabel");
            else if (AdultAcqVariable < 0)
                errors.Add("Vuxen akv variabeln får inte vara mindre än 0");

            return errors;
        }
    }
}
