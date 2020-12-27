using System.Text;

namespace Distance.ML {
    public static class BytesEx {
        public static byte[] Encode(this string @this) => Encoding.ASCII.GetBytes(@this);
        public static string Decode(this byte[] @this) => Encoding.ASCII.GetString(@this);
    }
}
