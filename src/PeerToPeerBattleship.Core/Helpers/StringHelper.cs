using System.Globalization;
using System.Text;

namespace PeerToPeerBattleship.Core.Helpers
{
    public static class StringHelper
    {
        public static string RemoveAccent(this string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}
