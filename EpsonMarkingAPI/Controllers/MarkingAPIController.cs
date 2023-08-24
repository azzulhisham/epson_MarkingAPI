using EpsonMarkingAPI.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EpsonMarkingAPI.Controllers
{
    /// <summary>
    /// Epson Marking API
    /// </summary>
    //[Authorize]
    [Serializable]
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/MarkingAPI")]
    public class MarkingAPIController : ApiController
    {
        /// <summary>
        /// Get Connection Status
        /// </summary>
        /// <returns>string</returns>
        [SwaggerResponse(HttpStatusCode.OK, "return back the Url", typeof(ServiceReponseData))]
        [Route("~/api/GetMarkingDataSgMa")]
        [HttpGet]
        public IHttpActionResult GetMarkingDataSgMa([FromUri] MarkingDataReq markingDataReq)
        {
            //query string :: http://localhost:54376/api/GetMarkingDataSgMa?markingDataReq.specNo=531PAP%20%2003FC&markingDataReq.frequency=22.745300&markingDataReq.itemNo=testing

            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required parameters.");
            }


            var serviceHandler = ConfigurationManager.AppSettings["masgMarkingApiService"];
            Providers.IOperationHandler operationHandler = new Providers.OperationHandler(serviceHandler);

            ServiceReponseData result = operationHandler.RequestHandle(markingDataReq);

            if (result.ResultCode >= 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Description);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markingDataReq"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "return back the Url", typeof(ServiceReponseData))]
        [Route("~/api/GetMarkingWeekCodeSgMa")]
        [HttpGet]
        public IHttpActionResult GetMarkingWeekCodeSgMa([FromUri] MarkingDataReq markingDataReq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required parameters.");
            }

            var serviceHandler = ConfigurationManager.AppSettings["masgWeekCodeApiService"];
            Providers.IOperationHandler operationHandler = new Providers.OperationHandler(serviceHandler);

            ServiceReponseData result = operationHandler.RequestHandle(markingDataReq);

            if (result.ResultCode >= 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Description);
            }
        }
    }
}
