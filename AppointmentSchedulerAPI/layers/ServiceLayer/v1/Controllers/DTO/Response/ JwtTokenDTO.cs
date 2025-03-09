public class  JwtTokenDTO
{
    public required string Token { get; set; }
    public required DateTime Expiration { get; set; }
}