using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FundsHistoricalQuotes.Core
{
    public class MorningstarEtfHistoricalQuotesProvider : IEtfHistoricalQuotesProvider
    {
        private readonly MorningstarHttpClient _client;

        private const string QueryString = "?id={0}%5D22%5D1%5D" +
                                           "currencyId=EUR&" +
                                           "idtype=Morningstar&" +
                                           "priceType=&" +
                                           "frequency=daily&" +
                                           "startDate={1}&" +
                                           "endDate={2}&" +
                                           "outputType=COMPACTJSON";

        public MorningstarEtfHistoricalQuotesProvider(MorningstarHttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Quote>> GetQuotesFor(string morningstarTickerSymbol, DateTime? from = null,
            DateTime? to = null)
        {
            var fromQuery = (from ?? DateTime.Today.AddYears(-1)).ToString("yyyy-MM-dd");
            var toQuery = (to ?? DateTime.Today).ToString("yyyy-MM-dd");

            var uriString = string.Format(QueryString, morningstarTickerSymbol, fromQuery, toQuery);
            var httpResponseMessage = await _client.HttpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(uriString, UriKind.Relative)
            });

            httpResponseMessage.EnsureSuccessStatusCode();

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            var contentPayload = JsonConvert.DeserializeObject<string[][]>(responseContent);

            return contentPayload.Select(tupleArray => new Quote
            {
                Date = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(tupleArray[0])).Date,
                Value = double.Parse(tupleArray[1], NumberStyles.AllowDecimalPoint)
            }).ToList();
        }
    }
}