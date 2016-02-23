using System;

namespace Jaime.Extensions {
    public static class StringExtensions {
        public static bool IsNullEmptyOrWhiteSpace(this string s) {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }

        public static bool IsValidUri(this string s) {
            try {
                Uri uri;
                return Uri.TryCreate(s, UriKind.Absolute, out uri) && uri.Scheme == Uri.UriSchemeHttp;
            } catch (Exception) {
                return false;
            }
        }
    }
}