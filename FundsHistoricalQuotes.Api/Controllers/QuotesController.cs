using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FundsHistoricalQuotes.Core;
using Microsoft.AspNetCore.Mvc;

namespace FundsHistoricalQuotes.Api.Controllers
{
    [Route("api/quotes")]
    public class QuotesController : Controller
    {
        private readonly IEtfHistoricalQuotesProvider _etfHistoricalQuotesProvider;
        private readonly IQuotesCalculationService _quotesCalculationService;

        public QuotesController(IQuotesCalculationService quotesCalculationService,
            IEtfHistoricalQuotesProvider etfHistoricalQuotesProvider)
        {
            _etfHistoricalQuotesProvider = etfHistoricalQuotesProvider;
            _quotesCalculationService = quotesCalculationService;
        }

        [Route("{morningstarTickerSymbol}")]
        public async Task<IActionResult> GetLastQuote(string morningstarTickerSymbol)
        {
            var lastQuote = (await _etfHistoricalQuotesProvider.GetQuotesFor(morningstarTickerSymbol)).Last();

            return Ok(lastQuote);
        }

        [Route("{morningstarTickerSymbol}/variations")]
        // TODO: support periods like "1M, 3M, 6M, 1Y, 5Y"
        public async Task<IActionResult> GetVariations(string morningstarTickerSymbol,
            string periodsCommaSeparated = null)
        {
            var quotes =
                await _etfHistoricalQuotesProvider.GetQuotesFor(morningstarTickerSymbol, DateTime.Today.AddYears(-5));

            var now = DateTime.Now;

            var periods = new Dictionary<DateTime, string>
            {
                {now.AddMonths(-1), "1M"},
                {now.AddMonths(-3), "3M"},
                {now.AddMonths(-6), "6M"},
                {now.AddYears(-1), "1Y"},
                {now.AddYears(-3), "3Y"},
                {now.AddYears(-5), "5Y"}
            };

            var variations = _quotesCalculationService.CalculatePerformanceOverPeriods(quotes, periods.Keys);

            var performancesDictionary = variations.ToImmutableDictionary(x => x.DateTime);

            var response = periods
                .Select(kvp => new {label = kvp.Value, value = performancesDictionary[kvp.Key]})
                .ToImmutableDictionary(x => x.label, x => x.value);

            return Ok(response);
        }
    }
}