using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Retail.Util.Extend;
using Retail.DAL.Models;
using Retail.DAL.Repository;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Retail.API
{
    public class CompanyAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
                return;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if(controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
          .Any(a => a.GetType().Equals(typeof(NoCompanyRequiredAttribute))))
            {
                return;
            }
            if (context.ModelState.IsValid)
            {
                var companyId = context.HttpContext.Request.Headers["CompanyId"].ToString().ToInt();
                var companyPid = context.HttpContext.Request.Headers["CompanyPid"].ToString().ToInt();
                var isValid = companyId > 0;
                if (isValid)
                {
                    SugarHandler db = new SugarHandler();
                    var company = db.Single<retail_company>(x => x.Id == companyId);
                    isValid = company != null && company.ParentId == companyPid;
                }
                if (!isValid)
                {
                    context.Result = new ObjectResult(new ProblemDetails() { Detail = "企业信息不正确" });
                }
            }
        }
    }
    public class NoCompanyRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }

}
