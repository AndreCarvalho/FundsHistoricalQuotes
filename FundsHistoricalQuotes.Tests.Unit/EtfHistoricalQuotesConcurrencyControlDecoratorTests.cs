using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FundsHistoricalQuotes.Core;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace FundsHistoricalQuotes.Tests.Unit
{
    public class EtfHistoricalQuotesConcurrencyControlDecoratorTests
    {
        [Theory]
        [AutoSubstituteDataAttribute]
        public async Task GetQuotesFor_ShouldBlockConcurrentExecutionForSameTickerSymbol(
            [Frozen] IEtfHistoricalQuotesProvider decorated,
            string tickerSymbol,
            ICollection<Quote> quotes,
            EtfHistoricalQuotesConcurrencyControlDecorator sut)
        {
            // Arrange
            var delay = TimeSpan.FromSeconds(1);
            const int numberOfConcurrentRequests = 5;

            async Task<IEnumerable<Quote>> ReturnWithDelay(CallInfo _)
            {
                await Task.Delay(delay);
                return quotes;
            }

            decorated.GetQuotesFor(tickerSymbol).Returns(ReturnWithDelay);

            // Act
            var tasks = Enumerable.Range(1, numberOfConcurrentRequests).Select(_ => sut.GetQuotesFor(tickerSymbol));

            var stopwatch = Stopwatch.StartNew();
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.Elapsed >= delay * numberOfConcurrentRequests, $"It took {stopwatch.Elapsed.TotalSeconds} s");
        }

        [Theory]
        [AutoSubstituteDataAttribute]
        public async Task GetQuotesFor_ShouldNotBlockConcurrentExecutionForDifferentTickerSymbol(
            [Frozen] IEtfHistoricalQuotesProvider decorated,
            ICollection<Quote> quotes,
            EtfHistoricalQuotesConcurrencyControlDecorator sut)
        {
            // Arrange
            var delay = TimeSpan.FromSeconds(1);
            const int numberOfConcurrentRequests = 5;
            
            var tickerSymbols = Enumerable.Range(1, numberOfConcurrentRequests).Select(_ => Guid.NewGuid().ToString()).ToArray();

            async Task<IEnumerable<Quote>> ReturnWithDelay(CallInfo _)
            {
                await Task.Delay(delay);
                return quotes;
            }

            decorated.GetQuotesFor(Arg.Any<string>()).Returns(ReturnWithDelay);

            // Act
            var tasks = tickerSymbols.Select(symbol => sut.GetQuotesFor(symbol));

            var stopwatch = Stopwatch.StartNew();
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.InRange(stopwatch.Elapsed.TotalSeconds, delay.TotalSeconds, delay.TotalSeconds * 1.2);
        }
    }
}