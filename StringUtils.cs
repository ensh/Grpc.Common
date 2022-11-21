namespace Vtb.Trade.Grpc.Common
{
    using System.Text;
    public static unsafe class StringUtils
    {
        static StringUtils()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            s_encoding = Encoding.GetEncoding("windows-1251");
        }

        public static readonly Encoding s_encoding;
        public const int MAXSTRSIZE = 256;
        public const int MAXWRITE = 1024;

        public static byte[] AsBytes(this string str)
            => s_encoding.GetBytes(str);

        public static string AsString(byte* bytes) => AsString(bytes, MAXSTRSIZE);
        public static string AsString(byte* bytes, int len)
        {
            for (int i = 0; i < len; i++)
                if (bytes[i] == 0)
                    return s_encoding.GetString(bytes, i);
            return s_encoding.GetString(bytes, len);
        }
    }
}
