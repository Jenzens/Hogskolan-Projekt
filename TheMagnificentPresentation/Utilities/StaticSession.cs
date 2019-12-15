using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMagnificentModels;

namespace TheMagnificentPresentation.Utilities
{
    public static class StaticSession
    {
        /// <summary>
        /// Static holder of the user to access it at frontend
        /// </summary>
        public static Users User { get; set; }
    }
}
