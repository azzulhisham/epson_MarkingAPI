namespace EpsonMarkingAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    /// <summary>
    /// Marking Records data model
    /// </summary>
    public partial class MarkingRecords
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Imi No.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string SpecNo { get; set; }

        /// <summary>
        /// Ina Code
        /// </summary>
        [Required]
        [StringLength(20)]
        public string InaCode { get; set; }

        /// <summary>
        /// Lot No.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string LotNo { get; set; }

        /// <summary>
        /// Freq. Data in generating the marking data
        /// </summary>
        [Required]
        [StringLength(20)]
        public string FreqData { get; set; }

        /// <summary>
        /// Marking Data
        /// </summary>
        [Required]
        [StringLength(50)]
        public string MarkingData { get; set; }

        /// <summary>
        /// Week Code
        /// </summary>
        [Required]
        [StringLength(12)]
        public string WeekCode { get; set; }

        /// <summary>
        /// Ppackage Type
        /// </summary>
        [StringLength(10)]
        public string PackageType { get; set; }

        /// <summary>
        /// Machine Operator
        /// </summary>
        [StringLength(20)]
        public string MrkOperator { get; set; }

        /// <summary>
        /// Marking Date
        /// </summary>
        [StringLength(10)]
        public string MarkingDate { get; set; }

        /// <summary>
        /// Marking Time
        /// </summary>
        [StringLength(10)]
        public string MarkingTime { get; set; }

        /// <summary>
        /// Additional Description
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// Machine Identity
        /// </summary>
        [StringLength(20)]
        public string MachineNo { get; set; }

        /// <summary>
        /// PCS System Data
        /// </summary>
        [StringLength(50)]
        public string PcsData { get; set; }

        /// <summary>
        /// Actual Marking Data & Time
        /// </summary>
        public DateTime MrkDateTime { get; set; }

        /// <summary>
        /// Actual Freq value
        /// </summary>
        [DataType("decimal(18 ,6")]
        public decimal FreqValue { get; set; }

        /// <summary>
        /// Date of this record
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
    }
}
