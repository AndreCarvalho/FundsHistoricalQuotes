using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FundsHistoricalQuotes.Core
{
    public interface IEtfHistoricalQuotesProvider
    {
        Task<IEnumerable<Quote>> GetQuotesFor(string morningstarTickerSymbol, DateTime? from = null,
            DateTime? to = null);
    }
}