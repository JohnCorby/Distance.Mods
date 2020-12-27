using System.Text;

namespace Distance.Raycast {
    public static class BytesEx {
        public static byte[] Encode(this string @this) => Encoding.UTF8.GetBytes(@this);
        public static string Decode(this byte[] @this) => Encoding.UTF8.GetString(@this);
    }
}
