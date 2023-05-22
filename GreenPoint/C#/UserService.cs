using System.Net.Http.Json;

namespace GreenPoints.C_;
public class UserService
{

    private readonly HttpClient http;
    public UserDTO? user { get; set; }
    public string? token { get; set; }

    public UserService(HttpClient http)
    {
        this.http = http;
        this.user = null;
        this.token = null;
    }

    public async Task<bool> Login(UserDTO userModel)
    {
        var resoult = await http.PostAsJsonAsync(Endpoints.PostURLForLogin(), userModel);
        if (resoult.IsSuccessStatusCode)
        {
            UserWithToken? userWithToken = await resoult.Content.ReadFromJsonAsync<UserWithToken>();

            if (userWithToken != null)
            {
                user = userWithToken.User;
                token = userWithToken.Token;
                return true;
            }
        }
        return false;
    }

    public async Task<bool> Register(TemporalUser userModel)
    {
        var resoult = await http.PostAsJsonAsync(Endpoints.PostURLForRegister(), userModel);
        if (resoult.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }

    public async Task Confirm(Guid id)
    {
        _ = await http.GetAsync(Endpoints.GetURLForConfirm(id));
    }

    public async Task GetUserById(roleRequest request)
    {
        _ = await http.PatchAsJsonAsync<roleRequest>(Endpoints.PostURLForChangeRole(), request);
    }
}
