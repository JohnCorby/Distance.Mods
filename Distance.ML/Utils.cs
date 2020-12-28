using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distance.ML {
    public static class Utils {
        public static byte[] Encode(this string @this) => Encoding.ASCII.GetBytes(@this);
        public static string Decode(this byte[] @this) => Encoding.ASCII.GetString(@this);

        public static string Join<T>(this IEnumerable<T> @this, string separator) =>
            string.Join(separator, @this.Select(t => t.ToStringWithNullCheck()).ToArray());

        public static PlayerDataLocal? PlayerDataLocal => G.Sys.PlayerManager_.Current_?.playerData_;
    }
}
