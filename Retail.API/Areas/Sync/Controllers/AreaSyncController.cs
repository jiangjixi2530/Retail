using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Retail.DAL.Repository;
using Retail.DAL.Models;
using Retail.Util;

namespace Retail.API.Areas.Sync.Controllers
{
    [Route("sync/")]
    [ApiController]
    public class AreaSyncController : ControllerBase
    {
        [HttpPost("GetProvince.htm")]
        public IActionResult GetProvince()
        {
            try
            {
                SugarHandler db = new SugarHandler();
                return Ok(db.Queryable<retail_province>().OrderBy(x => x.ProvinceId).ToList());
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("GetCity.htm")]
        public IActionResult GetCity()
        {
            try
            {
                SugarHandler db = new SugarHandler();
                return Ok(db.Queryable<retail_city>().OrderBy(x => x.ProvinceId).ToList());
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost("GetArea.htm")]
        public IActionResult GetArea()
        {
            try
            {
                SugarHandler db = new SugarHandler();
                return new JsonResult(db.Queryable<retail_area>().OrderBy(x => x.CityId).ToList());
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
