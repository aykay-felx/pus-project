using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System;

namespace schools_web_api.TokenManager.Extensions 
{
    public static class StringExtensions 
    {
        public static string Escape(this string str)
        {
            return Regex.Escape(str);
        }

        public static bool isValidEmail(this string str)
        {
            return new EmailAddressAttribute().IsValid(str);
        }

        public static bool isValid(this string str)
        {
            bool isNotNull = !string.IsNullOrEmpty(str);

            if (!isNotNull) 
            { 
                return false;
            }

            bool doesNotContainWhiteSpace = str == str.Trim();
            bool doesNotContainSpecialSigns = str.Escape() == str;

            return doesNotContainWhiteSpace && doesNotContainSpecialSigns;
        }

        public static bool isValidPassword(this string str)
        {
            return str.isValid() && str.Length == 64;
        }

        public static bool isEmpty(this string[] str)
        {
            return str == null || str.Length == 0;
        }

        public static bool isValid(this string[] str)
        {
            string cityPattern = @"[a-zA-ZĄąĆćĘęŁłŃńÓóŚśŹźŻż]+[[ -]?[a-zA-ZĄąĆćĘęŁłŃńÓóŚśŹźŻż]+]*";

            bool isNotEmpty = str.isEmpty();
            bool hasValidCities = Array.TrueForAll(str, s => {
                var match = Regex.Match(s, cityPattern);
                return match.Success && match.Value == s;
            });

            return isNotEmpty && hasValidCities;
        }
    }
}