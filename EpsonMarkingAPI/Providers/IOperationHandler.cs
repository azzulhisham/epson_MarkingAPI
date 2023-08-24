using EpsonMarkingAPI.Models;

namespace EpsonMarkingAPI.Providers
{
    /// <summary>
    /// interface of the operation handler
    /// </summary>
    public interface IOperationHandler
    {
        /// <summary>
        /// Handling Reqeust
        /// </summary>
        /// <param name="markingDataReq"></param>
        /// <returns></returns>
        ServiceReponseData RequestHandle(MarkingDataReq markingDataReq);
    }
}