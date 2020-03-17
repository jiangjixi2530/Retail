using System;
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
    public class ProductUploadController : ControllerBase
    {
        [HttpPost("categoryUpload.htm")]
        public IActionResult CategoryUpload()
        {
            SugarHandler db = new SugarHandler();
            try
            {
                //企业Id
                var companyId = Request.Headers["companyId"].ToString().ToInt();
                //企业Pid
                var companyPid = Request.Headers["companyPid"].ToString().ToInt();
                string jsonCategory = Request.Form["jsonCategory"].ToString();
                if (string.IsNullOrEmpty(jsonCategory))
                {
                    return Problem("参数不正确");
                }
                JObject obj = JsonConvert.DeserializeObject<JObject>(jsonCategory);
                var category = obj.JObjectTransToModel<retail_category>();
                category.Id = obj["categoryId"].ToString().ToInt();
                if (category.Id == 0)
                {
                    category.CompanyPid = companyPid;
                    category.Status = 1;
                    var result = (category.Id = db.AddReturnId(category)) > 0;
                    retail_ref_company_category company_Category = new retail_ref_company_category() { CategoryId = category.Id, CompanyId = companyId, CompanyPid = companyPid };
                    result = db.Add(company_Category);
                    db.CommitTran();
                    if (result)
                        return Ok(category);
                }
                else
                {
                    var result = db.Update<retail_category>(x => new retail_category() { CategoryCode = category.CategoryCode, CategoryName = category.CategoryName, SordIndex = category.SordIndex, CategoryType = category.CategoryType, ParentId = category.ParentId }, q => q.Id == category.Id, false);
                    db.CommitTran();
                    if (result)
                    {
                        return Ok("更新成功");
                    }
                }
                return Problem("参数有误");
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return Problem(ex.Message);
            }
        }
        [HttpPost("productUpload.htm")]
        public IActionResult ProductUpload()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}