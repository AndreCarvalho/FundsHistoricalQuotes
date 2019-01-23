using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FundsHistoricalQuotes.Core
{
    public class QuotesCalculationService : IQuotesCalculationService
    {
        private const int MaxNumberOfDaysApart = 5;
        
        public IEnumerable<Performance> CalculatePerformanceOverPeriods(IEnumerable<Quote> quotes, IEnumerable<DateTime> dates)
        {
            var quotesByDate = quotes.OrderBy(q => q.Date).ToImmutableArray();
            var datesSorted = dates.ToImmutableSortedSet();

            var quotesList = new List<Quote>();

            foreach (var dateTime in datesSorted)
            {
                // TODO: this algorithm can be improved for performance
                var closestQuoteByDate = quotesByDate.SkipWhile(q => q.Date.Date < dateTime.Date)
                    .FirstOrDefault();

                var quote = new Quote
                {
                    Date = dateTime,
                    // ReSharper disable once PossibleNullReferenceException
                    Value = QuoteIsEligible(closestQuoteByDate, dateTime) ? closestQuoteByDate.Value : double.NaN
                };

                quotesList.Add(quote);
            }

            var lastQuote = quotesByDate.Last();

            return quotesList.Select(quote => new Performance
            {
                DateTime = quote.Date,
                Variation = (lastQuote.Value - quote.Value) / quote.Value * 100
            });
        }

        private static bool QuoteIsEligible(Quote closestQuoteByDate, DateTime dateTime)
        {
            return !(closestQuoteByDate is null) && QuoteIsWithinRange(dateTime, closestQuoteByDate);
        }

        private static bool QuoteIsWithinRange(DateTime dateTime, Quote quote)
        {
            return Math.Abs((dateTime - quote.Date).Days) <= MaxNumberOfDaysApart;
        }
    }
}