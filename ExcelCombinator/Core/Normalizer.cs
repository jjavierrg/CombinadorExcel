using ExcelCombinator.Interfaces;

namespace ExcelCombinator.Core
{
    public class Normalizer : INormalizer
    {
        public string Normalize(string value)
        {
            if (value == null)
                return null;

            return value
                    .Trim()
                    .ToUpper();
        }
    }
}
