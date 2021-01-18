using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Comm100.Framework.Infrastructure
{
    public class HintInterceptor : DbCommandInterceptor
    {
        private static readonly Regex _tableAliasRegex = new Regex(@"(?<tableAlias>AS \[Extent\d+\](?! WITH \(*HINT*\)))", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        [ThreadStatic] public static string HintValue;

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            if (command.CommandText.Contains("LEFT JOIN [t_fileService_fileContent] AS [t0]"))
            {
                command.CommandText = command.CommandText
                    .Replace("LEFT JOIN [t_fileService_fileContent] AS [t0]", 
                    "LEFT JOIN [t_fileService_fileContent] AS [t0] WITH (INDEX(IX_T_FileService_fileContent_StorageType))");
            }
            //if (!String.IsNullOrWhiteSpace(HintValue))
            //{
            //    command.CommandText = _tableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (*HINT*)");
            //    command.CommandText = command.CommandText.Replace("*HINT*", HintValue);
            //}

            HintValue = String.Empty;
            return result;
        }
    }
}
