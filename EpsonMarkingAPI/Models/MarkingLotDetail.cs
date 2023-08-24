using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    /// <summary>
    /// Marking Lot Detail
    /// </summary>
    public class MarkingLotDetail
    {
        /// <summary>
        /// Lot No
        /// </summary>
        [Required]
        public string LotNo { get; set; }

        /// <summary>
        /// Spec No
        /// </summary>
        [Required]
        public string SpecNo { get; set; }

        /// <summary>
        /// Item No
        /// </summary>
        [Required]
        public string ItemNo { get; set; }
    }
}