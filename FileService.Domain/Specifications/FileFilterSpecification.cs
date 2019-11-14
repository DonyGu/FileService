using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Comm100.Framework.Domain.Specifications;
using FileService.Domain.Entities;

namespace FileService.Domain.Specifications
{
    public class FileFilterSpecification : ISpecification<File>
    {
        public FileFilterSpecification(int count, StorageType? storageType, string checksum)
        {
            throw new NotImplementedException();
        }

        public FileFilterSpecification(int count, StorageType? storageType)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<File, bool>> Criteria => throw new NotImplementedException();

        public List<Expression<Func<File, object>>> Includes => throw new NotImplementedException();

        public List<string> IncludeStrings => throw new NotImplementedException();

        public Expression<Func<File, object>> OrderBy => throw new NotImplementedException();

        public Expression<Func<File, object>> OrderByDescending => throw new NotImplementedException();

        public Expression<Func<File, object>> GroupBy => throw new NotImplementedException();

        public int Take => throw new NotImplementedException();

        public int Skip => throw new NotImplementedException();

        public bool IsPagingEnabled => throw new NotImplementedException();
    }
}