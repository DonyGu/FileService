using Comm100.Framework.Common;
using Comm100.Framework.Infrastructure;
using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FileService.Infrastructure.Repositories
{
    public class FileRepository : EFRepository<string, File>, IFileRepository
    {
        public FileRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<File> GetTopInDb(int count)
        {
            var sql = @"
SELECT [t1].[FileKey], [t1].[Checksum], [t1].[CreationTime], [t1].[ExpireTime], [t1].[SiteId], [t2].[Checksum], [t2].[Content], [t2].[Link], [t2].[Name], [t2].[StorageType]
FROM (
    SELECT TOP (@count) [t].[FileKey], [t].[Checksum], [t].[CreationTime], [t].[ExpireTime], [t].[SiteId]
    FROM [t_fileService_file] AS [t]
    LEFT JOIN [t_fileService_fileContent] AS [t0] 
	WITH (INDEX(IX_T_FileService_fileContent_StorageType)) ON [t].[Checksum] = [t0].[Checksum]
    WHERE ([t0].[StorageType] = 0) 
) AS [t1]
LEFT JOIN [t_fileService_fileContent] AS [t2] ON [t1].[Checksum] = [t2].[Checksum]
";
            var paras = new List<DbParameter>();
            paras.Add(new SqlParameter { ParameterName = "@count", Value = count });

            IList<File> files = new List<File>();
            using (var reader = this._dbContext.GetDataReader(sql, paras))
            {
                while (reader.Read())
                {
                    files.Add(new File
                    {
                        FileKey = reader["FileKey"].ToString(),
                        SiteId = Convert.ToInt32(reader["SiteId"]),
                        CreationTime = Convert.ToDateTime(reader["CreationTime"]),
                        ExpireTime = Convert.ToDateTime(reader["ExpireTime"]),
                        Checksum = (byte[])reader["Checksum"],
                        Content = new FileContent
                        {
                            Checksum = (byte[])reader["Checksum"],
                            Name = reader["Name"].ToString(),
                            Content = reader["Content"] as byte[],
                            Link = Convert.ToString(reader["Link"]),
                            StorageType = (StorageType)Convert.ToInt16(reader["StorageType"])
                        }
                    });
                }
                var connection = _dbContext.Database.GetDbConnection();
                if (connection.State==System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return files;
        }
    }
}
