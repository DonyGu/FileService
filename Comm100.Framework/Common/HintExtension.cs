using Comm100.Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Common
{
    public static class HintExtension
    {
        public static DbSet<T> WithHint<T>(this DbSet<T> set, string hint) where T : class
        {
            HintInterceptor.HintValue = hint;
            return set;
        }
    }
}
