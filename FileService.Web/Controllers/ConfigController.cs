using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comm100.Framework.Common;
using Comm100.Framework.Config;
using FileService.Infrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Web.Controllers
{
    [Route("config")]
    public class ConfigController : BaseController
    {
        private readonly IConfigService _configService;
 
        public ConfigController(IConfigService configService):base(configService)
        {
            this._configService = configService;
        }
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> List(string guid = "")
        {
            if (!await HasLogin())
            {
                if (!await Login(guid))
                {
                    return BadRequest();
                }
            }
            var configs = _configService.List();
            return View(configs);
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> Update(string value)
        {
            LogHelper.Error($"post config list: {value}");
            if (!await HasLogin())
            {
                return RedirectToAction("List");
            }
            await _configService.Set("IPWhiteList", value);
            LogHelper.Error($"post config list: {value}");
            return RedirectToAction("List");
        }


    }
}