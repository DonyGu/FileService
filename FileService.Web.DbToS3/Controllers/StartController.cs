using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
// using Comm100.Web.Controllers;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Exceptions;
using Comm100.Framework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Web.DbToS3.Controllers
{
    [ApiController]
    public class StartController : ControllerBase
    {
        public readonly IDbToS3Service _dbToS3Service;
        public readonly IDeleteExpriedFilesService _deleteExpiredFilesService;

        [HttpGet]
        public void Start()
        {
            this._dbToS3Service.Start();
            //this._deleteExpiredFilesService.Start();
        }
    }
}