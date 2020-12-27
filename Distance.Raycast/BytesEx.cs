using System.Text;

namespace Distance.Raycast {
    public static class BytesEx {
        public static byte[] Encode(this string @this) => Encoding.ASCII.GetBytes(@this);
        public static string Decode(this byte[] @this) => Encoding.ASCII.GetString(@this);
    }
}
