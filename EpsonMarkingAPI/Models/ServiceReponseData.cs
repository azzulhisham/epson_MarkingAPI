using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    /// <summary>
    /// Data model for response from specific service
    /// </summary>
    public class ServiceReponseData
    {
        /// <summary>
        /// Success / Failure code
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// Data Value
        /// </summary>
        public string DataValue { get; set; }

        /// <summary>
        /// Additional description when the service is return.
        /// </summary>
        public string Description { get; set; }
    }
}