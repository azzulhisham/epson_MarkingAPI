using EpsonMarkingAPI.Common;

namespace EpsonMarkingAPI.Services
{
    /// <summary>
    /// interface for masg marking data api
    /// </summary>
    public interface IMaSgMarkingDataApi
    {
        /// <summary>
        /// Get Marking Data Method
        /// </summary>
        /// <param name="specNo"></param>
        /// <param name="qryItem"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        FunctionStatus.SuccessFailure GetMarkingData(string specNo, string qryItem, ref string returnValue);
    }
}