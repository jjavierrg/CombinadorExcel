using ExcelCombinator.Interfaces;
using System.Text.RegularExpressions;

namespace ExcelCombinator.Core
{
    public class Normalizer : INormalizer
    {
        public string Normalize(string value)
        {
            if (value == null)
                return null;

            value = value.ToLower().Trim()
                .Replace('à', 'a')
                .Replace('è', 'e')
                .Replace('ì', 'i')
                .Replace('ò', 'o')
                .Replace('ù', 'u')
                .Replace('á', 'a')
                .Replace('é', 'e')
                .Replace('í', 'i')
                .Replace('ó', 'o')
                .Replace('ú', 'u')
                .Replace('ä', 'a')
                .Replace('ë', 'e')
                .Replace('ï', 'i')
                .Replace('ö', 'o')
                .Replace('ü', 'u')
                .Replace('ç', 'c')
                .Replace('ñ', 'n');

            return Regex.Replace(value, "[^a-zA-Z0-9]", "");
        }
    }
}
