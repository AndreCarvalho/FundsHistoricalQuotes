using System;
using System.Net.Http;

namespace FundsHistoricalQuotes.Core
{
    public class MorningstarHttpClient
    {
        public MorningstarHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
            HttpClient.BaseAddress =
                new Uri(
                    "http://tools.morningstar.es/api/rest.svc/timeseries_price/2nhcdckzon");
        }

        public HttpClient HttpClient { get; }
    }
}