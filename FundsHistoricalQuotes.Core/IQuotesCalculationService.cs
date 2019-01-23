using System;
using System.Collections.Generic;

namespace FundsHistoricalQuotes.Core
{
    public interface IQuotesCalculationService
    {
        IEnumerable<Performance> CalculatePerformanceOverPeriods(IEnumerable<Quote> quotes, IEnumerable<DateTime> dates);
    }
}