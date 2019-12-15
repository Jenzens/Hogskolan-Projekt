namespace TheMagnificentModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class InsuranceAcqVariables
    {
        public int Id { get; set; }

        public int InsuranceId { get; set; }

        public int Startdate { get; set; }

        public double AcqVariable { get; set; }

        public virtual Insurance Insurance { get; set; }
    }
}
