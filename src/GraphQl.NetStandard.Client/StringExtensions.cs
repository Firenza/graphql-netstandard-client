using System;

namespace GraphQl.NetStandard.Client
{
    internal static class StringExtensions
    {
        public static string ToLowerCaseFirstCharacter(this string str)
        {
            var firstCharacter = str[0];
            var lowerCaseFirstCharacter = Char.ToLowerInvariant(firstCharacter);
            var result = lowerCaseFirstCharacter + str.Substring(1);

            return result;
        }
    }
}
