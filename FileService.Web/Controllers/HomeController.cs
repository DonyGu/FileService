using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Comm100.Framework.Common;

namespace FileService.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    { 
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            var versionHelper = new VersionHelper();
            return Content(versionHelper.GetVersion());
        }
    } 
}