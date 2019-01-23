using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FundsHistoricalQuotes.Core
{
    public class Variations : ReadOnlyDictionary<string, double>
    {
        public Variations(IDictionary<string, double> dictionary) : base(dictionary)
        {
        }
    }
}