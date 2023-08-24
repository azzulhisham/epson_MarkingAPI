using EpsonMarkingAPI.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EpsonMarkingAPI.Controllers
{
    /// <summary>
    /// Data Manager for Marking API
    /// </summary>
    [Serializable]
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/DataManager")]
    public class DataManagerController : ApiController
    {
        private ApplicationUnit _applicationUnit = new ApplicationUnit();

        /// <summary>
        /// Get Marking Records By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>MarkingRecords</returns>
        [SwaggerResponse(HttpStatusCode.OK, "return back the Url", typeof(MarkingRecords))]
        [Route("~/api/GetMarkingRecordsById")]
        [HttpGet]
        public IHttpActionResult GetMarkingRecordsById(long id)
        {
            var data = _applicationUnit.MarkingRecordsRepo.GetById(id);

            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get Marking Records By Lot Details ()
        /// </summary>
        /// <param name="markingLotDetail"></param>
        /// <returns>List of MarkingRecords</returns>
        [SwaggerResponse(HttpStatusCode.OK, "return back the Url", typeof(List<MarkingRecords>))]
        [Route("~/api/GetMarkingRecordsByLotDetails")]
        [HttpGet]
        public IHttpActionResult GetMarkingRecordsByLotDetails([FromUri] MarkingLotDetail markingLotDetail)
        {
            var data = _applicationUnit.MarkingRecordsRepo.FindFor(c =>
                                    c.LotNo == markingLotDetail.LotNo &&
                                    c.SpecNo == markingLotDetail.SpecNo &&
                                    c.InaCode == markingLotDetail.ItemNo
                                    ).ToList();

            if (data != null && data.Count > 0)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get Marking Records By Lot Details ()
        /// </summary>
        /// <param name="markingRecords"></param>
        /// <returns>List of MarkingRecords</returns>
        [SwaggerResponse(HttpStatusCode.OK, "return back the Url", typeof(List<MarkingRecords>))]
        [Route("~/api/CreateIfNotExist")]
        [HttpPost]
        public IHttpActionResult CreateIfNotExist(MarkingRecords markingRecords)
        {
            try
            {
                if (markingRecords.Id == 0)
                {
                    _applicationUnit.MarkingRecordsRepo.Insert(markingRecords);
                    return Created<string>("Id=", markingRecords.Id.ToString());
                }
                else
                {
                    _applicationUnit.MarkingRecordsRepo.Update(markingRecords);
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}
