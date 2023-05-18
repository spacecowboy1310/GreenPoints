using System.Net.Http.Json;

namespace GreenPoints.C_;
public class UserService
{

    private readonly HttpClient http;

    public UserService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<dynamic> Login(UserDTO userModel)
    {
        var reosult = await http.PostAsJsonAsync(Endpoints.PostURLForLogin(), userModel);
        if (reosult.IsSuccessStatusCode) { return reosult.Content; }
        else { return null; }
    }

    public async Task Register(TemporalUser userModel)
    {
       _ = await http.PostAsJsonAsync(Endpoints.PostURLForRegister(), userModel);
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
