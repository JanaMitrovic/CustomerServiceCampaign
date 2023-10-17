using CustomerServiceCampaignAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CustomerServiceCampaignAPI.Authorization
{
    public static class JwtToken
    {
        //private const string SECRET_KEY = "JU/GTX3gaRu+C6NZHNsVr2GfnGMo3j52uEtNyKzfUFE=";
        //public static readonly SymmetricSecurityKey SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));

        public static SymmetricSecurityKey SIGNING_KEY { get; private set; }

        static JwtToken()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string secretKey = configuration["JwtSettings:SecretKey"];
            SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public static string GenerateJwtToken(Agent Agent)
        {
            var credentials = new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);

            DateTime expiry = DateTime.UtcNow.AddHours(1);
            int ts = (int) (expiry - new DateTime(1970,1,1)).TotalSeconds;

            var payload = new JwtPayload
            {
                {"sub", Agent.Email},
                {"name", Agent.Name},
                {"email", Agent.Email},
                {"exp", ts},
                {"iss", "https://localhost:44305"},
                {"aud","https://localhost:44305"}

            };
            
            var secToken = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);

            Console.WriteLine(tokenString);
            Console.WriteLine("Consume token");
            return tokenString;

        }
    }
}
