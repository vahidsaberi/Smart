namespace Identity.Application.Tokens;

public record RefreshTokenRequest(string Token, string RefreshToken);