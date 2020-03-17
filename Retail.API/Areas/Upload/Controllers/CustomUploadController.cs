using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Retail.Util;
using Retail.Util.Extend;

namespace Retail.API.Areas.Upload.Controllers
{
    [Route("upload/")]
    [ApiController]
    public class CustomUploadController : ControllerBase
    {
        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("CustomUpload.htm")]
        public IActionResult CustomUpload()
        {
            LogHelper.WriteLog(LogType.BASE, "进入客户信息上传接口");
            try
            {
                //企业Id
                var companyId = Request.Headers["companyId"].ToString().ToInt();
                LogHelper.WriteLog(LogType.BASE, $"企业id：{companyId}");
                //企业Pid
                var companyPid = Request.Headers["companyPid"].ToString().ToInt();
                LogHelper.WriteLog(LogType.BASE, $"企业id：{companyPid}");
                SugarHandler db = new SugarHandler();
                string jsonCustom = Request.Form["jsonCustom"].ToString();
                LogHelper.WriteLog(LogType.BASE, $"客户信息：{jsonCustom}");
                JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonCustom);
                retail_custom custom = new retail_custom();
                custom = jObject.JTokenTransToModel<retail_custom>();
                custom.CompanyPid = companyPid;
                custom.Id = jObject["CustomId"].ObjToInt();
                if (custom.Id == 0)
                {
                    LogHelper.WriteLog(LogType.BASE, $"新增客户");
                    custom.Status = 1;
                    custom.CreateDate = DateTime.Now;
                    custom.ModifyDate = DateTime.Now;
                    custom.Id = db.AddReturnId(custom);
                    retail_ref_company_custom company_Custom = new retail_ref_company_custom();
                    company_Custom.CustomId = custom.Id;
                    company_Custom.CompanyId = companyId;
                    db.AddReturnBool(company_Custom);
                    JObject obj = new JObject();
                    obj["CustomId"] = custom.Id;
                    return Ok(custom);
                }
                else
                {
                    LogHelper.WriteLog(LogType.BASE, $"更新客户");
                    if (db.Update<retail_custom>(x => new retail_custom { CustomCode = custom.CustomCode, CustomName = custom.CustomName,ProvinceId=custom.ProvinceId,CityId=custom.CityId, AreaId = custom.AreaId, LegalPerson = custom.LegalPerson, Phone = custom.Phone, Fax = custom.Fax, Address = custom.Address, Relation = custom.Relation, RelationPhone = custom.RelationPhone, Remark = custom.Remark, ModifyDate = DateTime.Now }, q => q.Id == custom.Id, false))
                    {
                        return Ok(true);
                    }
                }
                return Problem("更新失败，请检查参数");
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(LogType.BASE, ex);
                return Problem("参数异常:" + ex.Message);
            }
        }
        /// <summary>
        /// 更新客户状态
        /// </summary>
        /// <returns></returns>
        [HttpPost("CustomStatusUpload.htm")]
        public IActionResult CustomStatusUpload()
        {
            var customId = Request.Form["CustomId"].ToString().ToInt();
            try
            {
                if (customId > 0)
                {
                    if (!Request.Form["CustomStatus"].Equals(string.Empty))
                    {
                        var customStatus = Request.Form["CustomStatus"].ObjToInt();
                        SugarHandler db = new SugarHandler();
                        if (db.Update<retail_custom>(x => new retail_custom() { Status = customStatus }, q => q.Id == customId))
                        {
                            return Ok("信息更新成功");
                        }
                    }
                }
                return Problem("更新失败，请检查参数是否准确");
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}