using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Comm100.Framework.Domain.Specifications;
using FileService.Domain.Entities;

namespace FileService.Domain.Specifications
{
    public class FileContentFilterSpecification : BaseSpecification<File>
    {
        public FileContentFilterSpecification(int count, StorageType? storageType, string checksum)
            :base(_=>true)
        {
            throw new NotImplementedException();
        }

        public FileContentFilterSpecification(StorageType? storageType)
            :base(_=>true)
        {
            throw new NotImplementedException();
        }
    }
}