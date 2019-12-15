namespace TheMagnificentModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InsuranceBaseValues
    {
        public int Id { get; set; }

        public int InsuranceId { get; set; }

        public int StartDate { get; set; }

        public int? BaseValue { get; set; }

        public int? AcqValue { get; set; }

        public virtual Insurance Insurance { get; set; }
    }
}
