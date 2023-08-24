using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class FunctionStatus
    {
        /// <summary>
        /// Method return status
        /// </summary>
        public enum SuccessFailure
        {
            /// <summary>
            /// Failure
            /// </summary>
            error = -1,

            /// <summary>
            /// Sucess
            /// </summary>
            Success = 0
        }
    }
}