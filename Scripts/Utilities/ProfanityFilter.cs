using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/*
 * Profanity Filter
 * 
 * Checks an input against a profanity dictionary. Returns true if profanity was found.
 * Does a five stage approach
 * 1. original input is tested
 * 2. punctuation is removed while preserving spaces and tested again
 * 3. leet speak substitution is done and tested again
 * 4. trailing s are removed for plural handling
 * 5. spaces are removed and tested one final time
 *  
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

            //initial test 
            string[] chars = Regex.Split(input.Trim(), @"\s+");

            for (int i = 0; i < chars.Length; i++)
            {
                string combined = "";

                for (int j = i; j < chars.Length && chars[j].Length == 1; j++)
                {
                    combined += chars[j];
                    if (CheckToken(combined)) return true;
                }
            }

            //Split input into individual words
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