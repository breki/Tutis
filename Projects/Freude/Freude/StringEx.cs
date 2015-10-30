using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Freude
{
    [SuppressMessage ("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    internal static class StringEx
    {
        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void AppendList (this StringBuilder s, IEnumerable<string> items, string itemDelimiter)
        {
            Contract.Requires (s != null);
            Contract.Requires (items != null);

            string delimiter = null;
            foreach (string item in items)
            {
                s.Append (delimiter);
                s.Append (item);
                delimiter = itemDelimiter;
            }
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string Concat<TItem> (this IEnumerable<TItem> items, Func<TItem, string> formatterFunc, string itemDelimiter)
        {
            Contract.Requires (items != null);
            Contract.Requires (formatterFunc != null);
            Contract.Ensures (Contract.Result<string> () != null);

            StringBuilder s = new StringBuilder ();
            string actualDelimiter = null;

            foreach (TItem item in items)
            {
                s.Append (actualDelimiter);
                s.Append (formatterFunc (item));
                actualDelimiter = itemDelimiter;
            }

            return s.ToString ();
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string Concat<TItem> (
            this IEnumerable<TItem> items,
            Func<TItem, string> formatterFunc,
            string itemDelimiter,
            int startingIndex,
            int length)
        {
            Contract.Requires (items != null);
            Contract.Requires (formatterFunc != null);
            Contract.Ensures (Contract.Result<string> () != null);

            StringBuilder s = new StringBuilder ();
            string actualDelimiter = null;

            int index = 0;
            foreach (TItem item in items)
            {
                if (index >= startingIndex && index < startingIndex + length)
                {
                    s.Append (actualDelimiter);
                    s.Append (formatterFunc (item));
                    actualDelimiter = itemDelimiter;
                }

                index++;
            }

            return s.ToString ();
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string CropEnd (this string value, int length)
        {
            Contract.Requires (value != null);
            Contract.Requires (length >= 0);
            Contract.Requires (value.Length - length >= 0);
            Contract.Ensures (Contract.Result<string> () != null);

            return value.Substring (0, value.Length - length);
        }

        [SuppressMessage ("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "char")]
        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string ExtractUpTo (this string value, char upToChar)
        {
            Contract.Requires (value != null);
            Contract.Ensures (Contract.Result<string> () != null);

            int i = value.IndexOf (upToChar);
            if (i == -1)
                throw new InvalidOperationException ();

            return value.Substring (0, i);
        }

        [SuppressMessage ("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string ExtractUpTo (this string value, Predicate<char> charPredicate, out int index)
        {
            Contract.Requires (value != null);
            Contract.Requires (charPredicate != null);
            Contract.Ensures (Contract.Result<string> () != null);

            index = 0;
            for (; index < value.Length; index++)
            {
                if (charPredicate (value[index]))
                    break;
            }

            return value.Substring (0, index);
        }

        [SuppressMessage ("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fmt")]
        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string Fmt (this string format, params object[] args)
        {
            Contract.Requires (format != null);
            Contract.Requires (args != null);
            Contract.Ensures (Contract.Result<string> () != null);

            return string.Format (CultureInfo.InvariantCulture, format, args);
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool HasWhiteSpace (this string s)
        {
            Contract.Requires (s != null);

            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace (s[i]))
                    return true;
            }

            return false;
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string JoinLines (
            IEnumerable<string> lines,
            int startingLineIndex,
            int howManyLines)
        {
            Contract.Requires (lines != null);
            Contract.Ensures (Contract.Result<string> () != null);

            StringBuilder s = new StringBuilder ();

            int i = 0;
            foreach (string line in lines)
            {
                if (i >= startingLineIndex && i < startingLineIndex + howManyLines)
                {
                    if (i < startingLineIndex + howManyLines - 1)
                        s.AppendLine (line);
                    else
                        s.Append (line);
                }

                i++;
            }

            return s.ToString ();
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string Reverse (this string s)
        {
            if (s == null)
                return null;

            if (s.Length < 2)
                return s;

            char[] charArray = s.ToCharArray ();
            Array.Reverse (charArray);
            return new string (charArray);
        }

        public static IList<string> SplitIntoLines (this string value)
        {
            Contract.Requires (value != null);
            Contract.Ensures (Contract.Result<IList<string>> () != null);
            Contract.Ensures (Contract.ForAll(Contract.Result<IList<string>>(), x => x != null));

            List<string> lines = new List<string> ();
            using (StringReader reader = new StringReader (value))
            {
                string line;
                while ((line = reader.ReadLine ()) != null)
                    lines.Add (line);
            }

            Contract.Assume(Contract.ForAll (lines, x => x != null));
            return lines;
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IList<string> SplitIntoWords (this string value)
        {
            Contract.Requires (value != null);
            Contract.Ensures (Contract.Result<IList<string>> () != null);

            List<string> words = new List<string> ();

            int i = 0;
            int? wordStart = null;
            bool isInQuotes = false;
            while (i <= value.Length)
            {
                if (i == value.Length)
                {
                    if (isInQuotes)
                        throw new ArgumentException ("Missing end quotes");

                    if (wordStart != null)
                    {
                        Contract.Assume(wordStart.Value >= 0 && wordStart.Value < i);
                        string word = value.Substring (wordStart.Value, i - wordStart.Value);
                        words.Add (word);
                    }

                    break;
                }

                if (value[i] == ' ' && false == isInQuotes)
                {
                    if (wordStart != null)
                    {
                        Contract.Assume (wordStart.Value >= 0 && wordStart.Value < i);
                        string word = value.Substring (wordStart.Value, i - wordStart.Value);
                        words.Add (word);
                        wordStart = null;
                    }
                }
                else if (value[i] == '\"')
                {
                    if (false == isInQuotes)
                    {
                        if (wordStart != null)
                        {
                            Contract.Assume (wordStart.Value >= 0 && wordStart.Value < i);
                            string word = value.Substring (wordStart.Value, i - wordStart.Value);
                            words.Add (word);
                            wordStart = null;
                        }

                        isInQuotes = true;
                        wordStart = i;
                    }
                    else
                    {
                        Contract.Assume(wordStart.HasValue);

                        int wordLength = i - (wordStart.Value + 1);

                        if (wordLength > 0)
                        {
                            Contract.Assume (wordStart.Value >= -1);
                            string word = value.Substring (wordStart.Value + 1, wordLength);
                            words.Add(word);
                        }

                        isInQuotes = false;
                        wordStart = null;
                    }
                }
                else
                {
                    if (wordStart == null)
                        wordStart = i;
                }

                i++;
            }

            return words;
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string ToUnderscoreName (this string name, CultureInfo culture)
        {
            Contract.Requires (name != null);
            Contract.Requires (culture != null);
            Contract.Ensures (Contract.Result<string> () != null);

            StringBuilder s = new StringBuilder ();

            bool? previousWasUpper = null;
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                bool currentIsUpper = char.IsUpper (c);

                if (currentIsUpper)
                {
                    if (previousWasUpper.HasValue && !previousWasUpper.Value)
                        s.Append ("_");

                    s.Append (char.ToLower (c, culture));
                }
                else
                    s.Append (c);

                previousWasUpper = currentIsUpper;
            }

            return s.ToString ();
        }

        [SuppressMessage ("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        [SuppressMessage ("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        [SuppressMessage ("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string TrimEx (this string value, out int start, out int end)
        {
            Contract.Requires (value != null);
            Contract.Ensures (Contract.Result<string> () != null);

            for (start = 0; start < value.Length; start++)
                if (!char.IsWhiteSpace (value[start]))
                    break;

            string trimmedStart = value.Substring (start, value.Length - start);
            for (end = 0; end < trimmedStart.Length; end++)
                if (!char.IsWhiteSpace (trimmedStart[trimmedStart.Length - end - 1]))
                    break;

            return trimmedStart.Substring (0, trimmedStart.Length - end);
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string WildcardsToRegex (this string pattern)
        {
            Contract.Requires (pattern != null);
            Contract.Ensures (Contract.Result<string> () != null);

            return "^" + Regex.Escape (pattern).Replace (@"\*", ".*").Replace (@"\?", ".") + "$";
        }
    }
}