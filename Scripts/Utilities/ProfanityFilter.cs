using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


/*
 * Profanity Filter
 *
 * Detects profanity and attempts to catch common methods of bypassing
 * word filters, including leetspeak, inserted punctuation, whitespace,
 * and wildcard substitutions.
 *
 * Detection process:
 *
 * 1. Multi-token checking:
 *    Combines consecutive short strings separated by whitespace to detect
 *    attempts to disguise profanity by inserting spaces between characters
 *    or groups of characters (e.g. "f u c k" or "fu ck").
 *
 * 2. Original word checking:
 *    Checks the original word against the profanity dictionary.
 *
 * 3. Leetspeak substitution:
 *    Replaces common character substitutions with their alphabetical
 *    equivalents (e.g. "5" becomes "s", "1" becomes "i").
 *
 * 4. Punctuation normalization:
 *    Removes internal punctuation while preserving asterisks for
 *    subsequent wildcard checking.
 *
 * 5. Word variations:
 *    Checks common plural variations by removing a trailing "s".
 *
 * 6. Embedded profanity:
 *    Checks for profanity contained within larger words, while consulting
 *    an exclusion list to help prevent innocent words from being flagged.
 *    This is the famous Scunthorpe problem - see below.
 *
 * 7. Wildcard checking:
 *    Treats asterisks as single-character wildcards to detect partially
 *    obscured profanity (e.g. "pu**y" or "d*ck").
 *    
 *    Interesting note:  The name comes from Scunthorpe, an industrial town 
 *    in North Lincolnshire, England. In April 1996, residents trying to sign 
 *    up for AOL UK found their applications rejected because the town's 
 *    name embeds an offensive four-letter word
 */



namespace KioskHelper
{
    public static class ProfanityFilter
    {
        //Leet speak character substitution map
        private static readonly Dictionary<char, char> leetMap = new Dictionary<char, char>
        {            
            { '0', 'o' },
            { '1', 'i' },
            { '3', 'e' },
            { '4', 'a' },
            { '5', 's' },
            { '7', 't' },
            { '8', 'b' },
            { '$', 's' },
            { '@', 'a' },
            { 'z', 's' },
            { '!', 'i' }
        };


        //Returns true if the input contains a word in the ProfanityDictionary.Words list
        public static bool ContainsProfanity(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            //Initial test - removes whitespace and then iteratively tests the combined string
            //flags words like 'f u c k m e' and 'fu ck'
            string[] chars = Regex.Split(input.Trim(), @"\s+");

            for (int i = 0; i < chars.Length; i++)
            {
                string combined = "";

                for (int j = i; j < chars.Length && chars[j].Length <= 3; j++)
                {
                    combined += chars[j];
                    if (combined.Length > 20) break;
                    if (CheckToken(combined)) return true;
                }
            }

            //Now split original input into individual words delimited by whitespace (space, tab,line feed, etc.)
            //and test each word individually.
            string[] words = Regex.Split(input, @"\s+");

            foreach (string word in words)
            {
                if (CheckToken(word)) return true;
            }

            return false;
        }

        private static bool CheckToken(string input)
        {
            //Remove leading/trailing punctuation
            //Preserve * for wildcard checking
            string word = Regex.Replace(input, @"^[^\w*]+|[^\w*]+$", "");

            if (string.IsNullOrEmpty(word)) return false;

            //Check original word
            if (CheckWords(word)) return true;

            //Apply leetspeak substitutions
            string leet = ReplaceChars(word, leetMap);

            if (CheckWords(leet)) return true;

            //Remove internal punctuation, preserving *
            string normalized = Regex.Replace(leet, @"[^a-zA-Z0-9*]", "");

            if (CheckWords(normalized)) return true;                       

            //Remove trailing s
            if (normalized.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                string singular = normalized.Substring(0, normalized.Length - 1);
                if (CheckWords(singular)) return true;
            }

            //embedded swears check - and checks against exceptions list
            if (CheckContainsWords(normalized)) return true;

            //Finally do wildcard checking
            if (normalized.Contains('*'))
            {
                string pattern = "^" + Regex.Escape(normalized).Replace(@"\*", "[a-z]") + "$";

                foreach (string badWord in ProfanityDictionary.Words)
                {
                    if (Regex.IsMatch(badWord, pattern, RegexOptions.IgnoreCase)) return true;
                }
            }

            //No swear!
            return false;
        }


        public static string ReplaceChars(string input, Dictionary<char, char> map)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            //preallocate the length
            var sb = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                //Append mapped character if found, otherwise keep original
                sb.Append(map.TryGetValue(c, out char replacement) ? replacement : c);
            }

            return sb.ToString();
        }


        private static bool CheckWords(string input)
        {
            return ProfanityDictionary.Words.Contains(input);
        }


        //checks for embedded swears
        private static bool CheckContainsWords(string input)
        {
            if (ProfanityDictionary.Exceptions.Contains(input)) return false;

            foreach (string word in ProfanityDictionary.Words)
            {
                if (input.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}