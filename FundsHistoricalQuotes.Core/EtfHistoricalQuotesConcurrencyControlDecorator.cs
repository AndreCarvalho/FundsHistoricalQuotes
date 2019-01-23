using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FundsHistoricalQuotes.Core
{
    public class EtfHistoricalQuotesConcurrencyControlDecorator : IEtfHistoricalQuotesProvider
    {
        private readonly IEtfHistoricalQuotesProvider _decorated;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locksDictionary;

        public EtfHistoricalQuotesConcurrencyControlDecorator(IEtfHistoricalQuotesProvider decorated)
        {
            _decorated = decorated;

            _locksDictionary = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public async Task<IEnumerable<Quote>> GetQuotesFor(string morningstarTickerSymbol, DateTime? from = null, DateTime? to = null)
        {
            var semaphoreSlim = _locksDictionary.GetOrAdd(morningstarTickerSymbol, _ => new SemaphoreSlim(1));

            await semaphoreSlim.WaitAsync();

            try
            {
                return await _decorated.GetQuotesFor(morningstarTickerSymbol, from, to);
            }
            finally
            {
                semaphoreSlim.Release();                
            }
        }
    }
}