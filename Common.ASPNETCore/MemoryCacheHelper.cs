using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.ASPNETCore;

namespace Common.ASPNETCore
{
    public class MemoryCacheHelper : IMemoryCacheHelper
    {
        public TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 60)
        {
            throw new NotImplementedException();
        }

        public Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            throw new NotImplementedException();
        }

        public void Remove(string cacheKey)
        {
            throw new NotImplementedException();
        }
    }
}
