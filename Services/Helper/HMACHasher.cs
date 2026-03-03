using System.Security.Cryptography;
using System.Text;

namespace Services.Helper;

public static class HMACHasher
{
    public static async Task<string> HashValue(string valueToHash, string secretKey)
    {
        var strStreamOne = new MemoryStream(Encoding.UTF8.GetBytes(valueToHash));
        byte[] hashOne;
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        using (var hmac = new HMACSHA256(key))
        {
            hashOne = await hmac.ComputeHashAsync(strStreamOne);
        }

        return Convert.ToHexString(hashOne);
    }
}
