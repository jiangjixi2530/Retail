using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Retail.Util.Extend;

namespace Retail.API.Areas.Upload.Controllers
{
    [Route("upload/")]
    [ApiController]
    public class CustomInfoController : ControllerBase
    {
        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("CustomUpload.htm")]
        public IActionResult CustomUpload()
        {
            try
            {
                //企业Id
                var companyId = Request.Form["companyId"].ToString().ToInt();
                //企业Pid
                var companyPid = Request.Form["companyPid"].ToString().ToInt();
                SugarHandler db = new SugarHandler();
                string jsonCustom = Request.Form["jsonCustom"].ToString();
                JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonCustom);
                retail_custom custom = new retail_custom();
                custom.Id = jObject["CustomId"].ObjToInt();
                custom.CustomCode = jObject["CustomCode"].ObjToString();
                custom.AreaId = jObject["AreaId"].ObjToInt();
                custom.LegalPerson = jObject["LegalPerson"].ObjToString();
                custom.Phone = jObject["Phone"].ObjToString();
                custom.Fax = jObject["Fax"].ObjToString();
                custom.Address = jObject["Address"].ObjToString();
                custom.Relation = jObject["Relation"].ObjToString();
                custom.CompanyPid = companyPid;
                custom.RelationPhone = jObject["RelationPhone"].ObjToString();
                custom.Remark = jObject["Remark"].ObjToString();
                if (custom.Id == 0)
                {
                    custom.Status = 1;
                    custom.Id = db.AddReturnId(custom);
                    retail_ref_company_custom company_Custom = new retail_ref_company_custom();
                    company_Custom.CustomId = custom.Id;
                    company_Custom.CompanyId = companyId;
                    db.AddReturnBool(company_Custom);
                    JObject obj = new JObject();
                    obj["CustomId"] = custom.Id;
                    return Ok(obj);
                }
                else
                {
                    if (db.Update<retail_custom>(x => new retail_custom { CustomCode = custom.CustomCode, CustomName = custom.CustomName, AreaId = custom.AreaId, LegalPerson = custom.LegalPerson, Phone = custom.Phone, Fax = custom.Fax, Address = custom.Address, Relation = custom.Relation, RelationPhone = custom.RelationPhone, Remark = custom.Remark }, q => q.Id == custom.Id, false))
                    {
                        return Ok(true);
                    }
                }
                return Problem("更新失败，请检查参数");
            }
            catch (SqlSugar.SqlSugarException ex)
            {
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
                        if(db.Update<retail_custom>(x => new retail_custom() { Status = customStatus }, q => q.Id == customId))
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