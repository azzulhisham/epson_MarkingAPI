using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    /// <summary>
    /// Marking Data
    /// </summary>
    public class MarkingData
    {
        public int RowNo { get; set; }
        public int RowData { get; set; }
        public string DataFormat { get; set; }
        public string Value { get; set; }
    }
}