using System;
using System.Text.RegularExpressions;

namespace KioskHelper
{
    public static class ProfanityFilter
    {
        public static bool ContainsProfanity(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            //First check: Original input
            if (CheckWords(input)) return true;

            //Second check: Remove punctuation and check again
            input = Regex.Replace(input, @"[^\w\s]", " ");

            return CheckWords(input);
        }


        private static bool CheckWords(string input)
        {
            string[] words = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in ProfanityDictionary.Words)
            {
                foreach (string inputWord in words)
                {
                    if (string.Equals(inputWord, word, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }
    }
}