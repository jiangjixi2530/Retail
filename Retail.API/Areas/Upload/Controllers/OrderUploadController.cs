using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Retail.Util.Extend;

namespace Retail.API.Areas.Upload.Controllers
{
    [Route("upload")]
    [ApiController]
    public class OrderUploadController : ControllerBase
    {
        [HttpPost("OrderInfoUpload.htm")]
        public IActionResult OrderInfoUpload()
        {
            var companyId = Request.Headers["CompanyId"].ToString().ToInt();
            var companyPid = Request.Headers["CompanyPid"].ToString().ToInt();
            HttpContext.Response.ContentType = "application/json";
            string jsonOrder = "";
            using (var sr = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var v = sr.ReadToEndAsync();//或者sr.ReadToEnd()
                v.Wait();
                jsonOrder = v.Result;
            }
            JObject orderObj = (JObject)JsonConvert.DeserializeObject(jsonOrder);
            string orderNum = orderObj["OrderNum"].ToString();
            SugarHandler db = new SugarHandler();
            var orderModel = db.Single<retail_order>(x => x.OrderNum == orderNum);
            if (orderModel == null)
            {
                orderModel = new retail_order();
            }
            JObjectToModel<retail_order>.JObjectTansToModel(orderObj, orderModel);
            //if (orderModel.Id == 0)
            //{
            //    orderModel.OrderNum = orderNum;
            //    orderModel.OrderDate = orderObj["OrderDate"].ToString().ConvertStringToDateTime();
            //}
            //orderModel.CustomId = orderObj["CustomId"].ToString().ToInt();
            //orderModel.CustomName = orderObj["CustomName"].ToString();
            //orderModel.Relation = orderObj["Relation"].ToString();
            //orderModel.RelationPhone = orderObj["RelationPhone"].ToString();
            //orderModel.Address = orderObj["Address"].ToString();
            //orderModel.PayMentType = orderObj["PayMentType"].ToString().ToInt();
            return Ok(true);
        }
    }
}