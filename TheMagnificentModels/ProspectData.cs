using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagnificentModels
{
    
    /// <summary>
    /// Used for printing prospect
    /// </summary>
    public class ProspectData
    {

        
        /// <summary>
        /// Sale data to get agent id
        /// </summary>
        public Sale InsuranceSale { get; set; }

        //PersonData of the person which the prospect is connected to
        public PersonData Person { get; set; }

    }
}
