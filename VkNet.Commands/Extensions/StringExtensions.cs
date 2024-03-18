namespace VkNet.Commands.Extensions;

internal static class StringExtensions
{
    internal static string RemovePrefixes(this string s, IEnumerable<string> prefixes)
    {
        foreach (var prefix in prefixes)
        {
            var index = s.IndexOf(prefix, StringComparison.Ordinal);
            if (index != -1)
            {
                return s.Remove(index, prefix.Length);
            }
        }

        return s;
    }
}