using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Retail.DAL.Models;
using Retail.DAL.Repository;

namespace Retail.API.Areas.Sync.Controllers
{
    [Route("sync/")]
    [ApiController]
    public class CompanySyncController : ControllerBase
    {
        [NoCompanyRequired]
        [HttpPost("GetCheckCompanyCode.htm")]
        public IActionResult GetCompanyInfo()
        {
            try
            {
                string code = Request.Form["code"].ToString();
                if (string.IsNullOrEmpty(code))
                {
                    return Problem("参数有误");
                }
                SugarHandler db = new SugarHandler();
                var company = db.Single<retail_company>(x => x.UniqueCode == code);
                if (company == null || company.Id == 0)
                {
                    return Problem("编码有误");
                }
                var machines = db.GetList<retail_machine>(x => x.CompanyId == company.Id);
                var employees = db.GetList<retail_employee>(x => x.CompanyId == company.Id);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["Company"] = company;
                dict["Machines"] = machines;
                dict["Employees"] = employees;
                return Ok(dict);
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}