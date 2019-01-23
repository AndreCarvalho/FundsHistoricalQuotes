using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FundsHistoricalQuotes.Core;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;

namespace FundsHistoricalQuotes.Tests.Unit
{
    public class EtfHistoricalQuotesInMemoryCacheDecoratorTests
    {
        [Theory]
        [AutoSubstituteDataAttribute]
        public async Task GetQuotesFor_SameArgumentsWithMultipleCallc_HitsDecoratedJustOnceAndThenTheCache(
            string tickerSymbol,
            DateTime? from,
            DateTime? to,
            [Frozen] IEtfHistoricalQuotesProvider decorated,
            ICollection<Quote> freshQuotes)
        {
            // Arrange
            decorated.GetQuotesFor(tickerSymbol, from, to).Returns(freshQuotes);

            // This is more an integration test...
            var cache = new MemoryCache(new MemoryCacheOptions());
            var sut = new EtfHistoricalQuotesInMemoryCacheDecorator(decorated, cache);
            
            // Act
            await sut.GetQuotesFor(tickerSymbol, from, to);
            await sut.GetQuotesFor(tickerSymbol, from, to);
            await sut.GetQuotesFor(tickerSymbol, from, to);
            
            // Assert
            await decorated.Received(1).GetQuotesFor(tickerSymbol, from, to);
        }
    }
}