using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwt;

namespace Back
{
    public static class Token
    {
        private const string secretKey = "abcd1234"; //私钥


        public static string GetToken(TokenInfo M)
        {
            var payload = new Dictionary<string, dynamic>
               {
                    {"id", M.id},//自定义字段，用户id
                    {"password", M.password},//自定义字段 密码
                };
            return Jwt.JsonWebToken.Encode(payload, secretKey, Jwt.JwtHashAlgorithm.HS256);
        }

        public static Dictionary<string, dynamic> VerifyToken(string token)
        {
            try
            {
                var data = JsonWebToken.DecodeToObject<Dictionary<string, object>>(token, Token.secretKey);
                return data;
            }
            catch
            {
                // Given token is either expired or hashed with an unsupported algorithm.
                return null;
            }
        }
    }

    public class TokenInfo
    {
        public TokenInfo()
        {
            password = "";
        }
        public decimal id { get; set; }
        public string password { get; set; }
    }
}
