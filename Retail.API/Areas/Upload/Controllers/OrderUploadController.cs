using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Retail.Util;
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
            orderModel = orderObj.JObjectTransToModel<retail_order>();
            JArray jArray = (JArray)JsonConvert.DeserializeObject(orderObj["OrderDetail"].ObjToString());
            List<retail_order_item> orderDetails = new List<retail_order_item>();
            foreach (var item in jArray)
            {
                retail_order_item orderDetail = new retail_order_item();
                orderDetail = item.JTokenTransToModel<retail_order_item>();
                orderDetails.Add(orderDetail);
            }
            db.BeginTran();
            try
            {
                if (orderModel.Id > 0)
                {
                    db.Update<retail_order>(x => new retail_order() { CustomId = orderModel.CustomId, CustomName = orderModel.CustomName, Relation = orderModel.Relation, RelationPhone = orderModel.RelationPhone, Address = orderModel.Address, PayMentType = orderModel.PayMentType, OriginalPrice = orderModel.OriginalPrice, OrderDiscount = orderModel.OrderDiscount, OrderDiscountAmount = orderModel.OrderDiscountAmount, ReceivePrice = orderModel.ReceivePrice, PayStatus = orderModel.PayStatus, PayDate = orderModel.PayDate, OrderStatus = orderModel.OrderStatus, Remark = orderModel.Remark, SaleMan = orderModel.SaleMan }, q => q.Id == orderModel.Id, false);
                }
                else
                {
                    orderModel.Id = db.AddReturnId(orderModel);
                }
                foreach (var item in orderDetails)
                {
                    var existsItem = db.Single<retail_order_item>(x => x.OrderNum == item.OrderNum && x.ItemGuid == item.ItemGuid);
                    if (existsItem != null && existsItem.Id > 0)
                    {
                        db.Update<retail_order_item>(x => new retail_order_item() { ProductId = item.ProductId, ProductName = item.ProductName, ProductUnitName = item.ProductUnitName, SizeId = item.SizeId, SizeName = item.SizeName, IsSingleSize = item.IsSingleSize, OriginalPrice = item.OriginalPrice, ItemDiscount = item.ItemDiscount, ItemDiscountPrice = item.ItemDiscountPrice, PromotionPrice = item.PromotionPrice, BuyPrice = item.BuyPrice, BuyCount = item.BuyCount, PackUnitName = item.PackUnitName, ConversionValue = item.ConversionValue, ProductDescription = item.ProductDescription, Remark = item.Remark }, q => q.Id == existsItem.Id, false);
                    }
                    else
                    {
                        item.CreateDate = AppSetting.TimeNow;
                        item.Id = db.AddReturnId(item);
                    }
                }
                return Ok("订单上传成功");
            }
            catch (SqlSugar.SqlSugarException ex)
            {
                return Problem(ex.Message);
            }

        }
    }
}