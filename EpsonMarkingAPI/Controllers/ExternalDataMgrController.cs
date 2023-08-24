using EpsonMarkingAPI.Models;
using EpsonMarkingAPI.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using static EpsonMarkingAPI.Common.FunctionStatus;

namespace EpsonMarkingAPI.Controllers
{
    /// <summary>
    /// Epson External Data Mmanager API
    /// </summary>
    //[Authorize]
    [Serializable]
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/ExternalDataMgr")]
    public class ExternalDataMgrController : ApiController
    {
        /// <summary>
        /// Get Taping Lot Form Data from I.T. Database
        /// </summary>
        /// <param name="lotNo"></param>
        /// <returns>list of Lot Form data</returns>
        [SwaggerResponse(HttpStatusCode.OK, "return a list of Lot Form data", typeof(List<TapingLotFormData>))]
        [Route("~/api/GetTapingLotFormData")]
        [HttpGet]
        public IHttpActionResult GetTapingLotFormData([FromUri] string lotNo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required parameters.");
            }

            List<TapingLotFormData> tapingLotFormData = null;
            var externalDataConnection = ConfigurationManager.AppSettings["externalDataConnection"];
            var externalDataTable = ConfigurationManager.AppSettings["externalDataTable"];

            ExternalDataManagerApi externalDataManagerApi = new ExternalDataManagerApi(externalDataConnection, externalDataTable);
            Exception exception = externalDataManagerApi.GetTapingLotFormData(lotNo, ref tapingLotFormData);

            if (exception == null)
            {
                if (tapingLotFormData != null && tapingLotFormData.Count > 0)
                {
                    return Ok(tapingLotFormData);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return InternalServerError(exception);
            }
        }

        [SwaggerResponse(HttpStatusCode.OK, "return a list of Lot Form data", typeof(List<TapingLotFormData>))]
        [Route("~/api/GetTapingLotFormDataPCS")]
        [HttpGet]
        public IHttpActionResult GetTapingLotFormDataPCS([FromUri] string vLotNo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required parameters.");
            }

            List<TapingLotFormData> tapingLotFormDatas = null;
            //tapingLotFormDatas.Add(new TapingLotFormData()
            //{
            //    F_LOTNO = "EMZ2756400",
            //    G_WKCODE = "E0828",
            //    H_USEDQTY = 2197
            //});
            //tapingLotFormDatas.Add(new TapingLotFormData()
            //{
            //    F_LOTNO = "EMZ3763100",
            //    G_WKCODE = "E0828",
            //    H_USEDQTY = 1146
            //});
            //tapingLotFormDatas.Add(new TapingLotFormData()
            //{
            //    F_LOTNO = "EMZ3142400",
            //    G_WKCODE = "E07W8",
            //    H_USEDQTY = 3569
            //});

            var externalDataConnection = ConfigurationManager.AppSettings["pcsDataConnection"];
            var externalDataQuery = ConfigurationManager.AppSettings["pcsDataQuery"];

            externalDataQuery = string.Format(externalDataQuery, vLotNo);

            ExternalDataManagerApi externalDataManagerApi = new ExternalDataManagerApi(externalDataConnection, null);
            Exception exception = externalDataManagerApi.GetTapingLotFormDataPCS(vLotNo, externalDataQuery, ref tapingLotFormDatas);

            if (exception == null)
            {
                if (tapingLotFormDatas != null && tapingLotFormDatas.Count > 0)
                {
                    return Ok(tapingLotFormDatas);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return InternalServerError(exception);
            }
        }

    }
}
