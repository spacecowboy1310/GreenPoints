public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Mail { get; set; }
    public List<string> Roles { get; set; }
    public List<GreenPoint> Collaborations { get; set; }
}

public class roleRequest
{
    public int UserId { get; set; }
    public List<string> NewRoles { get; set; }
}
