using Comm100.Framework.Config;
using FileService.Infrastructure.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileService.Web.StandbyToMain.Controllers
{
    [Route("log")]
    public class LogController : BaseController
    {
        public LogController(IConfigService configService) : base(configService)
        {
        }

        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index(string guid = "")
        {
            if (!await HasLogin())
            {
                if (!await Login(guid))
                {
                    return BadRequest();
                }
            }

            var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            var contents = fileProvider.GetDirectoryContents("/logs");
            List<string> fileNames = new List<string>();
            foreach (var content in contents)
            {
                fileNames.Add(content.Name);
            }
            TempData["logLink"] = fileNames;
            return View();
        }

        [HttpGet]
        [Route("logPreview")]
        public async Task<IActionResult> logPreview(string fileName)
        {
            if (!await HasLogin())
            {
                return RedirectToAction("Index");
            }


            if (!Regex.IsMatch(fileName, @"^[0-9a-z-_.]+[log|txt]$", RegexOptions.IgnoreCase))
            {
                return RedirectToAction("Index");
            }

            var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            var file = fileProvider.GetFileInfo($"/logs/{fileName}");

            string txtData = null;
            Stream stream = file.CreateReadStream();
            using (StreamReader sr = new StreamReader(stream, System.Text.Encoding.Default))
            {
                txtData = await sr.ReadToEndAsync();
                sr.Close();
            }
            ContentResult rs = new ContentResult();
            rs.Content = txtData;
            rs.ContentType = "text/plain";
             
            return rs;
        }
    }
}