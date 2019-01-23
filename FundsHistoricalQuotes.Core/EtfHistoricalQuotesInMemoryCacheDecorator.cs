using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace FundsHistoricalQuotes.Core
{
    public class EtfHistoricalQuotesInMemoryCacheDecorator : IEtfHistoricalQuotesProvider
    {
        private readonly IEtfHistoricalQuotesProvider _decorated;
        private readonly IMemoryCache _memoryCache;

        public EtfHistoricalQuotesInMemoryCacheDecorator(IEtfHistoricalQuotesProvider decorated,
            IMemoryCache memoryCache)
        {
            _decorated = decorated;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Quote>> GetQuotesFor(string morningstarTickerSymbol, DateTime? from = null,
            DateTime? to = null)
        {
            var cacheKey = $"{morningstarTickerSymbol}-{from}-{to}";

            return await _memoryCache.GetOrCreateAsync(cacheKey,
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8);
                    return _decorated.GetQuotesFor(morningstarTickerSymbol, from, to);
                });
        }
    }
}