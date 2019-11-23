using System;
using Comm100.Framework.Domain.Specifications;
using FileService.Domain.Entities;

namespace FileService.Domain.Specifications
{
    public class FileFilterSpecification : BaseSpecification<File>
    {
        public FileFilterSpecification(int count, StorageType? storageType)
            : base(f => f.Content.StorageType == storageType)
        {
        }
    }
}