using Comm100.Framework.Domain.Repository;
using FileService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Domain
{
    public interface IFileRepository : IRepository<string, File>
    {
        IEnumerable<File> GetTopInDb(int count);
    }
}
