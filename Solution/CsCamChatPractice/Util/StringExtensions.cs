using System.Text.RegularExpressions;

namespace CsCamChatPractice.Util
{
    public static class StringExtensions
    {
        private const string trictIPv4Pattern =
            @"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)" +
            @"(\.(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)){3}$";

        public static bool IsValidIPAddress(this string? input)
        {
            return string.IsNullOrWhiteSpace(input) is false
                && Regex.IsMatch(input, trictIPv4Pattern);
        }
    }

}
