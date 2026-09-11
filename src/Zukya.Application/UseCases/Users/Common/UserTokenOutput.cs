namespace Zukya.Application.UseCases.Users.Common;

public class UserTokenOutput(string accessToken, UserOutput user)
{
    public string AccessToken { get; set; } = accessToken;
    public UserOutput User { get; set; } = user;
}
