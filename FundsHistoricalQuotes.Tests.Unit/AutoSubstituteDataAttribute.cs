using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace FundsHistoricalQuotes.Tests.Unit
{
    public class AutoSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoSubstituteDataAttribute()
            : base(() => new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true }))
        {
        }
    }
}