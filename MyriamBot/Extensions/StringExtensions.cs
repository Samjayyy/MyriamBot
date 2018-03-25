using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyriamBot.Extensions
{
    static class StringExtensions
    {
        private static readonly char[] WhiteSpaces = { ' ', '\r', '\n', '\t' };
        public static HashSet<string> ParseKeywords(this string msg)
        {
            return new HashSet<string>(msg.Split(WhiteSpaces, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower().Clean()));
        }

        public static string CleanSeperated(this string msg)
        {
            return string.Join(" ", msg.Split(WhiteSpaces, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Clean()));
        }

        public static string Clean(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            return Regex.Replace(str, @"[^a-zA-Z]", "");
        }
    }
}
