using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BrunoMikoski.DebugTools
{
    public static class StringExtensions
    {
        // Smarter conversion of programmer-style identifiers (camelCase, PascalCase, snake_case, etc.)
        // into human readable text with spaces and sentence casing while preserving acronyms.
        public static string ToHumanReadableText(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string s = input.Trim();

            // Remove common Hungarian/prefix patterns and leading/trailing underscores
            s = RemoveKnownPrefixes(s);
            s = s.Trim('_');

            // Normalize common separators to spaces
            s = s.Replace('\t', ' ')
                 .Replace('\n', ' ')
                 .Replace('_', ' ')
                 .Replace('-', ' ')
                 .Replace('.', ' ')
                 .Replace('/', ' ')
                 .Replace('\\', ' ');

            // Collapse multiple spaces
            s = MULTI_SPACE.Replace(s, " ").Trim();

            // Insert spaces for camelCase, PascalCase, acronym boundaries, and digit transitions
            s = REGEX_LOWER_UPPER.Replace(s, "$1 $2");
            s = REGEX_ACRONYM_WORD.Replace(s, "$1 $2");
            s = REGEX_LETTER_DIGIT.Replace(s, "$1 $2");
            s = REGEX_DIGIT_LETTER.Replace(s, "$1 $2");

            // Collapse spaces again in case regex added extra
            s = MULTI_SPACE.Replace(s, " ").Trim();

            // Sentence case while preserving acronyms (ALL CAPS tokens) and numbers
            var tokens = s.Split(' ');
            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];
                if (string.IsNullOrEmpty(t))
                    continue;

                if (IsAllCaps(t) && t.Length > 1)
                {
                    // Preserve acronyms like GUI, URL
                    tokens[i] = t;
                }
                else if (t.All(c => !char.IsLetter(c)))
                {
                    // Numbers or symbols-only token
                    tokens[i] = t;
                }
                else
                {
                    tokens[i] = t.ToLowerInvariant();
                }
            }

            s = string.Join(" ", tokens);
            if (s.Length > 0)
                s = char.ToUpperInvariant(s[0]) + (s.Length > 1 ? s.Substring(1) : string.Empty);

            return s;
        }

        private static bool IsAllCaps(string token)
        {
            // Consider a token an acronym if all letters are uppercase (digits allowed)
            bool hasLetter = false;
            foreach (char c in token)
            {
                if (char.IsLetter(c))
                {
                    hasLetter = true;
                    if (!char.IsUpper(c))
                        return false;
                }
            }
            return hasLetter; // at least one letter and all letters were upper
        }

        private static string RemoveKnownPrefixes(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            // Remove repeated known prefixes (e.g., m__value, _m_Value)
            bool removed;
            do
            {
                removed = false;
                foreach (var p in KNOWN_PREFIXES)
                {
                    if (s.StartsWith(p, StringComparison.Ordinal))
                    {
                        s = s.Substring(p.Length);
                        removed = true;
                    }
                }
            } while (removed);

            return s;
        }

        private static readonly string[] KNOWN_PREFIXES = { "m_", "s_", "k_", "_" };

        private static readonly Regex REGEX_LOWER_UPPER = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);
        private static readonly Regex REGEX_ACRONYM_WORD = new Regex("([A-Z]+)([A-Z][a-z])", RegexOptions.Compiled);
        private static readonly Regex REGEX_LETTER_DIGIT = new Regex("([A-Za-z])([0-9])", RegexOptions.Compiled);
        private static readonly Regex REGEX_DIGIT_LETTER = new Regex("([0-9])([A-Za-z])", RegexOptions.Compiled);
        private static readonly Regex MULTI_SPACE = new Regex(@"\s+", RegexOptions.Compiled);
    }
}
