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
    [Route("sync")]
    [ApiController]
    public class ProductSyncController : ControllerBase
    {
        [HttpGet("getCategoryInfo.htm")]
        public IActionResult GetCategoryInfo()
        {
            var companyId = Request.Headers["CompanyId"].ToString().ToInt();
            try
            {
                SugarHandler db = new SugarHandler();
                var products = db.Queryable<retail_category, retail_ref_company_category>((t1, t2) => new object[] { JoinType.Inner, t1.Id == t2.CategoryId }).Where((t1, t2) => (t2.CompanyId == companyId && t1.Status == 1)).ToList();
                return Ok(products);
            }
            catch (SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("getProductInfo.htm")]
        public IActionResult GetProductInfo()
        {
            var companyId = Request.Headers["CompanyId"].ToString().ToInt();
            try
            {
                SugarHandler db = new SugarHandler();
                var products = db.Queryable<retail_product, retail_ref_company_product>((t1, t2) => new object[] { JoinType.Inner, t1.Id == t2.ProductId }).Where((t1, t2) => (t2.CompanyId == companyId && t1.Status == 1)).ToList();
                return Ok(products);
            }
            catch (SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("getProductSizeInfo.htm")]
        public IActionResult GetProductSizeInfo()
        {
            var companyId = Request.Headers["CompanyId"].ToString().ToInt();
            try
            {
                SugarHandler db = new SugarHandler();
                var products = db.Queryable<retail_product_size, retail_product, retail_ref_company_product>((t1, t2, t3) => new object[] { JoinType.Inner, t1.ProductId == t2.Id,JoinType.Inner,t2.Id==t3.ProductId }).Where((t1, t2,t3) => (t3.CompanyId == companyId && t2.Status == 1)).ToList();
                return Ok(products);
            }
            catch (SqlSugarException ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}