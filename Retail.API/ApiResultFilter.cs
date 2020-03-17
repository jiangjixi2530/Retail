using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Retail.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retail.API
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Action执行完成,返回结果处理
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception == null)
            {
                ObjectResult objectResult = null;
                if (actionExecutedContext.Result.GetType().Equals(typeof(JsonResult)))
                {
                    objectResult = new ObjectResult(RetailResult<object>.ToSuccess((actionExecutedContext.Result as JsonResult).Value));
                }
                else
                {
                    //执行成功 取得由 API 返回的资料
                    ObjectResult result = actionExecutedContext.Result as ObjectResult;
                    if (result != null)
                    {
                        if (result.StatusCode == 200)
                        {
                            objectResult = new ObjectResult(RetailResult<object>.ToSuccess(result.Value));
                        }
                        else
                        {
                            var r = result.Value as ProblemDetails;
                            objectResult = new ObjectResult(RetailResult<object>.ToFail(r.Detail));
                        }
                    }
                }
                if (objectResult != null)
                {
                    // 重新封装回传格式
                    actionExecutedContext.Result = objectResult;
                }
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
