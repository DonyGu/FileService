using System;
using Comm100.Framework.Domain.Specifications;
using FileService.Domain.Entities;

namespace FileService.Domain.Specifications
{
    public class FileFilterSpecification : BaseSpecification<File>
    {
        public FileFilterSpecification(StorageType? storageType)
            : base(f => f.Content.StorageType == storageType)
        {
        }
        public FileFilterSpecification(byte[] checksum)
            : base(f => f.Checksum == checksum)
        {
        }
        public FileFilterSpecification(DateTime dateTime)
            : base(f => f.ExpireTime < dateTime)
        {
        }
        public void ApplyPaging(int pageIndex, int pageSize)
        {
            base.ApplyPaging(pageIndex, pageSize);
        }
        public void AddContentInclude()
        {
            base.AddInclude((a => a.Content));
        }

    }
}