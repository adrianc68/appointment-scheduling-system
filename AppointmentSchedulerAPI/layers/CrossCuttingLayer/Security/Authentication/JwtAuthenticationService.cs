using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication
{
    public class JwtAuthenticationService : IAuthenticationService<JwtUserCredentials, JwtTokenResult>
    {
        private readonly string issuer;
        private readonly string audience;
        private readonly string secretKey;


        public JwtAuthenticationService(string issuer, string audience, string secretKey)
        {
            this.issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            this.audience = audience ?? throw new ArgumentNullException(nameof(audience));
            this.secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        }

        public JwtTokenResult? Authenticate(JwtUserCredentials credentials)
        {
            if (!ValidateCredentials(credentials))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            // $$$>> It needs to be changed
            // var userRole = GetRoleForUser(credentials.Username); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                 [
                     new Claim(ClaimTypes.Name, credentials.Username),
                    new Claim(ClaimTypes.Role, credentials.Role.ToString())
                 ]),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtTokenResult
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public JwtTokenResult? RefreshCredentials(JwtUserCredentials credentials)
        {
            return Authenticate(credentials);
        }

        public bool ValidateCredentials(JwtUserCredentials credentials)
        {
            if (credentials is not JwtUserCredentials jwtUserCredentials)
                return false;
            // $$$>> It needs get password from database!!!!!
            return !string.IsNullOrEmpty(jwtUserCredentials.Username) && jwtUserCredentials.Password == "securepassword";
        }

        private string GetUserDataFromDatabase()
        {
            return "$$$>> getUserRole, getEmail";
        }

        private string GetUserRoleFromDatabase()
        {
            return "$$$>> change me please";
        }
    }
}
