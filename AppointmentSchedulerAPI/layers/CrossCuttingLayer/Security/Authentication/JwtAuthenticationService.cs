
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication
{
    public class JwtAuthenticationService : IAuthenticationService<JwtUserCredentials, JwtTokenResult, JwtTokenData>
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                 [
                     new Claim("username", credentials.Username),
                     new Claim("uuid", credentials.Uuid),
                     new Claim("email", credentials.Email),
                    new Claim("role", credentials.Role)
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

        public JwtTokenData? ValidateCredentials(JwtTokenResult token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            JwtTokenData newToken;
            try
            {
                tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);
                JwtSecurityToken see = (JwtSecurityToken)validatedToken;
                var claims = see.Claims;
                newToken = new JwtTokenData
                {
                    Email = claims.First(claim => claim.Type == "email").Value,
                    Username = claims.First(claim => claim.Type == "username").Value,
                    Role = claims.First(claim => claim.Type == "role").Value,
                    Uuid = claims.First(claim => claim.Type == "uuid").Value
                };
            }
            catch (Exception ex) when (ex is SecurityTokenException ||
                           ex is ArgumentNullException ||
                           ex is ArgumentException ||
                           ex is SecurityTokenMalformedException ||
                           ex is SecurityTokenDecryptionFailedException ||
                           ex is SecurityTokenEncryptionKeyNotFoundException ||
                           ex is SecurityTokenExpiredException ||
                           ex is SecurityTokenInvalidAudienceException ||
                           ex is SecurityTokenInvalidLifetimeException ||
                           ex is SecurityTokenInvalidSignatureException ||
                           ex is SecurityTokenNoExpirationException ||
                           ex is SecurityTokenNotYetValidException ||
                           ex is SecurityTokenReplayAddFailedException ||
                           ex is SecurityTokenReplayDetectedException)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }
            return newToken;
        }
    }
}
