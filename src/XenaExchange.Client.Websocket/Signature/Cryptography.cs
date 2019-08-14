using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace XenaExchange.Client.Websocket.Signature
{
    /// <summary>
    /// All cryptographic algorithms for authorization are placed here.
    /// </summary>
    public static class XenaSignature
    {
        public static string Sign(string apiKey, string data)
        {
            var digest = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(data));
            var keyInfo = ASN1.ParseASN1String(apiKey);
            if (!ASN1.NIST_P_256_CURVE.Equals(keyInfo.CurveNameHex, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NotSupportedException("Curves other than NIST P256 are not supported");
            }
            var q = new ECPoint { X = keyInfo.Qx, Y = keyInfo.Qy };
            var d = keyInfo.PrivateKey;
            var ecParams = new ECParameters { D = d, Curve = ECCurve.NamedCurves.nistP256, Q = q };
            using (var dsa = ECDsa.Create(ecParams))
            {
                return Utils.ByteArrayToHexString(dsa.SignHash(digest));
            }
        }
    }

    static class ASN1
    {
        public static ASN1ECKeyStructure ParseASN1String(string input)
        {
            var res = new ASN1ECKeyStructure();
            var bytes = Utils.GetBytes(input);
            var stream = new MemoryStream(bytes);

            int token;
            do
            {
                token = stream.ReadByte();
                switch (token)
                {
                    case 0x30: continue;    // sequence opening tag
                    case 0x77: res.Version = ReadToken(stream); break;
                    case 0x04: res.PrivateKey = ReadToken(stream); break;
                    case 0xA0:
                        {
                            var content = ReadToken(stream);
                            var contentStream = new MemoryStream(content);
                            if (contentStream.ReadByte() == 0x06)
                            {
                                res.CurveName = ReadToken(contentStream);
                            }
                        }
                        break;
                    case 0xA1:
                        {
                            var content = ReadToken(stream);
                            var contentStream = new MemoryStream(content);
                            if (contentStream.ReadByte() == 0x03)
                            {
                                var child = ReadToken(contentStream);
                                var childStream = new MemoryStream(child);
                                if (childStream.ReadByte() == 0x00 && childStream.ReadByte() == 0x04)
                                {
                                    var pubkey = new byte[childStream.Length - 2];
                                    childStream.Read(pubkey, 0, pubkey.Length);
                                    res.PublicKey = pubkey;
                                }
                            }
                        }
                        break;
                }
            }
            while (token != -1);

            return res;
        }

        public const string NIST_P_256_CURVE = "2A8648CE3D030107";

        private static byte[] ReadToken(Stream stream)
        {
            var contentLength = stream.ReadByte();
            var res = new byte[contentLength];
            for (var i = 0; i < contentLength; i++)
            {
                res[i] = (byte)stream.ReadByte();
            }
            return res;
        }
    }

    internal static class Utils
    {
        public static byte[] GetBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }

    internal sealed class ASN1ECKeyStructure
    {
        public byte[] Version { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] CurveName { get; set; }
        public string CurveNameHex => CurveName != null ? Utils.ByteArrayToHexString(CurveName) : null;
        public byte[] PublicKey { get; set; }
        public byte[] Qx
        {
            get
            {
                if (PublicKey == null) return null;
                if (PublicKey.Length % 2 != 0) throw new InvalidDataException("Public key length is not even");
                var res = new byte[PublicKey.Length / 2];
                Array.Copy(PublicKey, 0, res, 0, PublicKey.Length / 2);
                return res;
            }
        }
        public byte[] Qy
        {
            get
            {
                if (PublicKey == null) return null;
                if (PublicKey.Length % 2 != 0) throw new InvalidDataException("Public key length is not even");
                var res = new byte[PublicKey.Length / 2];
                Array.Copy(PublicKey, PublicKey.Length / 2, res, 0, PublicKey.Length / 2);
                return res;
            }
        }
    }
}
