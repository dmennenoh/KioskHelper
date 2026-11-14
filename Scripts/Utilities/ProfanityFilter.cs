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
 * 5. spaces are removed and tested one final time * 
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

        public static bool ContainsProfanity(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            //First check: Original input
            if (CheckWords(input)) return true;

            //Second check: Remove punctuation and symbols, preserving spaces
            string normalized = Regex.Replace(input, @"[^a-zA-Z0-9\s]", " ");

            if (CheckWords(normalized)) return true;

            //Third check: Leetspeak substitution
            normalized = ReplaceChars(normalized, leetMap);

            if (CheckWords(normalized)) return true;

            //Fourth check: Remove trailing 's' from words
            normalized = Regex.Replace(normalized, @"\b(\w+)s\b", "$1");

            if (CheckWords(normalized)) return true;

            //Fifth check: Remove spaces to catch separated characters
            normalized = normalized.Replace(" ", "");

            return CheckWords(normalized);
        }


        public static string ReplaceChars(string input, Dictionary<char, char> map)
        {
            if (string.IsNullOrEmpty(input)) return input;

            //Pre-allocate exact capacity for optimal performance
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
            string[] words = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string inputWord in words)
            {
                if (ProfanityDictionary.Words.Contains(inputWord)) return true;
            }

            return false;
        }
    }
}