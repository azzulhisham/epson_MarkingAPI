using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    /// <summary>
    /// Marking Data Request Object
    /// </summary>
    public class MarkingDataReq
    {
        /// <summary>
        /// IMI No.
        /// </summary>
        [Required]
        public string SpecNo { get; set; }

        /// <summary>
        /// Use Freq. Value when query for Marking Data otherwise please enter Ina Code
        /// </summary>
        [Required]
        public string QueryItem { get; set; }
    }
}