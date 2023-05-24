public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Mail { get; set; }
    public List<string> Roles { get; set; }
    public List<int> Collaborations { get; set; }
}

public class RoleRequest
{
    public int UserId { get; set; }
    public List<string> NewRoles { get; set; }
}

public class UserWithToken
{
    public UserDTO User { get; set; }
    public string Token { get; set; }

    public UserWithToken(UserDTO user, string token)
    {
        User = user;
        Token = token;
    }
}
