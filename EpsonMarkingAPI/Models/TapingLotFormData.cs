using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    /// <summary>
    /// Taping Lot Form Data Structure
    /// </summary>
    public class TapingLotFormData
    {
        /// <summary>
        /// product definition
        /// </summary>
        public string A_PRODUCT { get; set; }

        /// <summary>
        /// spec no. definition
        /// </summary>
        public string B_SPEC { get; set; }

        /// <summary>
        /// output frequency definition
        /// </summary>
        public string C_FREQ { get; set; }

        /// <summary>
        /// Ina Code definition
        /// </summary>
        public string D_INA_CODE { get; set; }

        /// <summary>
        /// Lot No Definition
        /// </summary>
        public string E_OGLOTNO { get; set; }

        /// <summary>
        /// Mother Lot Definition
        /// </summary>
        public string F_LOTNO { get; set; }

        /// <summary>
        /// Week Code definition 
        /// </summary>
        public string G_WKCODE { get; set; }

        /// <summary>
        /// Quantity used definitioin
        /// </summary>
        public int H_USEDQTY { get; set; }
    }
}