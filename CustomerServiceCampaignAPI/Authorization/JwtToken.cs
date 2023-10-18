using CustomerServiceCampaignAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CustomerServiceCampaignAPI.Authorization
{
    public static class JwtToken
    {
        public static SymmetricSecurityKey SIGNING_KEY { get; private set; }

        static JwtToken()
        {
            //Get secret key from the appsettings.js and initialize SIGNING_KEY
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string secretKey = configuration["JwtSettings:SecretKey"];
            SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public static string GenerateJwtToken(Agent Agent)
        {
            //Specify signing algorithm
            var credentials = new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256);
            //Create header
            var header = new JwtHeader(credentials);
            //Define token expiry time
            DateTime expiry = DateTime.UtcNow.AddHours(1);
            int ts = (int) (expiry - new DateTime(1970,1,1)).TotalSeconds;
            //Define payload
            var payload = new JwtPayload
            {
                {"email", Agent.Email},
                {"exp", ts},
                {"iss", "https://localhost:44305"},
                {"aud","https://localhost:44305"}

            };
            //Create token in string format
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(secToken);

            return tokenString;

        }
    }
}
