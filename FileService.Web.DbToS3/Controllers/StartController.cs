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
    [Route("")]
    [ApiController]
    public class StartController : ControllerBase
    {
        public readonly IDbToS3Service _dbToS3Service;
        public readonly IDeleteExpriedFilesService _deleteExpiredFilesService;

        public StartController(IDbToS3Service dbToS3Service, IDeleteExpriedFilesService deleteExpriedFilesService)
        {
            this._dbToS3Service = dbToS3Service;
            this._deleteExpiredFilesService = deleteExpriedFilesService;
        }
        [HttpGet]
        public string Start()
        {
            this._dbToS3Service.Start();
            this._deleteExpiredFilesService.Start();
            return "start";
        }
    }
}