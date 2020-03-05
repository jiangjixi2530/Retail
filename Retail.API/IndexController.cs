using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Retail.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Retail.API
{
    [ApiController]
    [Route("[controller].htm")]
    public class IndexController : Controller
    {
        [HttpGet]
        [NoCompanyRequired]
        public IActionResult Get()
        {
            LogHelper.WriteLog(LogType.BASE,"服务启动成功");
            return Content("服务已启动");
        }
    }
}
