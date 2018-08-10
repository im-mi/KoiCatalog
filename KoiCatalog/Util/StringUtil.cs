using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace KoiCatalog.Util
{
    public static class StringUtil
    {
        public static bool FuzzyContains(this string self, string value)
        {
            const CompareOptions compareOptions =
                CompareOptions.IgnoreCase
                | CompareOptions.IgnoreSymbols
                | CompareOptions.IgnoreWidth
                | CompareOptions.IgnoreKanaType;
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, value, compareOptions) != -1;
        }

        public static bool EqualsWildcard(
            this string self, string other, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            if (other == null && self == null) return true;
            if (other == string.Empty && self == string.Empty) return true;
            if (other == string.Empty && self != string.Empty) return false;
            if (other == null || self == null) return false;

            var otherSplits = other.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            // Check for wildcard-only string.
            if (otherSplits.Length == 0) return true;
            
            var i = 0;
            var subIndex = 0;

            // Match the start of the string, if applicable.
            var matchStart = !other.StartsWith("*");
            if (matchStart)
            {
                if (!self.StartsWith(otherSplits[0], comparisonType)) return false;
                i++;
                subIndex = otherSplits[0].Length;
            }

            for (; i < otherSplits.Length; i++)
            {
                var split = otherSplits[i];
                subIndex = self.IndexOf(split, subIndex, comparisonType);
                if (subIndex == -1) return false;
                subIndex += split.Length;
            }

            // Match the end of the string, if applicable.
            var matchEnd = !other.EndsWith("*");
            if (matchEnd && subIndex != self.Length)
                return false;

            return true;
        }

        public static string BytesToString(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            return string.Join(string.Empty, bytes.Select(i => $"{i:x2}"));
        }
        
        public static string NormalizeString(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            return string.Join(" ", str.Split().Where(i => i.Length > 0));
        }

        private static IEnumerable<string> GetMultiQuerySplitEnumerable(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            return OrOperatorExpression.Split(str).Select(NormalizeString).Where(i => i.Length > 0);
        }

        /// <summary>
        /// Test against a string that may contain operators like OR.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static bool TestOperatorString(string str, Func<string, bool> function)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (function == null) throw new ArgumentNullException(nameof(function));
            return GetMultiQuerySplitEnumerable(str).Any(function.Invoke);
        }

        private static readonly Regex OrOperatorExpression = new Regex(@"(\s+|^)(OR|ＯＲ)(\s+|$)",
            RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string FormatCamelcase(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            return string.Join(" ", CapitalizeTitleWords(SplitCamelcase(str)));
        }

        private static readonly Regex CamelcaseMatchExpression =
            new Regex(@"([A-ZＡ-Ｚ]*[^A-ZＡ-Ｚ]*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static IEnumerable<string> SplitCamelcase(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            return CamelcaseMatchExpression.Split(str)
                .Where(i => i.Length > 0)
                .ToList();
        }

        private static IEnumerable<string> CapitalizeTitleWords(IEnumerable<string> words)
        {
            if (words == null) throw new ArgumentNullException(nameof(words));
            var results = new List<string>();

            var i = -1;
            foreach (var word in words)
            {
                i++;

                string result;
                if (i == 0)
                {
                    result = CapitalizeWord(word);
                }
                else
                {
                    var lowercaseWord = word.ToLower();
                    if (LowercaseTitleWords.Contains(lowercaseWord))
                        result = lowercaseWord;
                    else
                        result = CapitalizeWord(word);
                }

                results.Add(result);
            }

            return results;
        }

        private static string CapitalizeWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (word.Length == 0) return word;
            return word[0].ToString().ToUpper() + word.Substring(1);
        }

        private static readonly string[] LowercaseTitleWords =
        {
            "a", "an", "the", "at", "by", "for", "in", "of", "on", "to", "up", "and", "as", "but", "or", "nor",
        };
    }
}
