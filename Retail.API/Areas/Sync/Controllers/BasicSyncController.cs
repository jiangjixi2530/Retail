using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Retail.Util.Extend;
using SqlSugar;

namespace Retail.API.Areas.Sync.Controllers
{
    [Route("sync/")]
    [ApiController]
    public class BasicSyncController : ControllerBase
    {
        /// <summary>
        /// 同步客户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("getCustom.htm")]
        public IActionResult GetCustom()
        {
            var companyId = Request.Headers["CompanyId"].ObjToInt();
            var companyPid = Request.Headers["CompanyPid"].ObjToInt();
            try
            {
                SugarHandler db = new SugarHandler();
                var customs = db.Queryable<retail_custom, retail_ref_company_custom>((t1, t2) => new object[] { JoinType.Inner, t1.Id == t2.CustomId }).Where((t1, t2) => t2.CompanyId == companyId && t1.CompanyPid == companyPid && t1.Status == 1).ToList();
                return Ok(customs);
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
        /// <summary>
        /// 同步单位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("HttpPost.htm")]
        public IActionResult GetUnit()
        {
            var companyId = Request.Headers["CompanyId"].ObjToInt();
            try
            {
                SugarHandler db = new SugarHandler();
                var units = db.Queryable<retail_unit, retail_ref_company_unit>((t1, t2) => new object[] { JoinType.Inner, t1.Id == t2.UnitId }).Where((t1, t2) => t2.CompanyId == companyId).ToList();
                return Ok(units);
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}