using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Retail.Util.Extend;

namespace Retail.API.Areas.Upload.Controllers
{
    [Route("upload/")]
    [ApiController]
    public class BasicUploadController : ControllerBase
    {
        [HttpPost("machineBindUpload.htm")]
        public IActionResult MachineBindUpload()
        {
            try
            {
                var companyId = Request.Headers["CompanyId"].ToString().ToInt();
                var companyPid = Request.Headers["CompanyPid"].ToString().ToInt();
                int machineId = Request.Form["machineId"].ToString().ToInt();
                string bindUid = Request.Form["bindUid"].ToString();
                string bindComputerName = Request.Form["bindComputerName"].ToString();
                if (string.IsNullOrEmpty(bindUid))
                {
                    return Problem("绑定Uid不允许为空");
                }
                SugarHandler db = new SugarHandler();
                var machine = db.Single<retail_machine>(x => x.Id == machineId && x.CompanyId == companyId);
                if (machine == null)
                {
                    return Problem("查询不到对应的机器");
                }
                if (!string.IsNullOrEmpty(machine.BindUid) && !machine.BindUid.Equals(bindUid,StringComparison.OrdinalIgnoreCase))
                {
                    return Problem("该机器号已被绑定");
                }
                db.Update<retail_machine>(x => new retail_machine() { BindUid = bindUid, BindComputerName = bindComputerName }, q => q.Id == machineId, false);
                return Ok("机器绑定成功");
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}