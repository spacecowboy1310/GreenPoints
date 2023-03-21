using System.Diagnostics.CodeAnalysis;

public class User
{
    public int Id { get; set; }
    [NotNull]
    public string Username { get; set; }
    [NotNull]
    public string Password { get; set; }
    [NotNull]
    public string Mail { get; set; }
    public Role Role { get; set; }
    public List<GreenPoint> Collaborations { get; set; }
}

public class TemporalUser
{
    public Guid ID { get; set; }
    [NotNull]
    public string Username { get; set; }
    [NotNull]
    public string Password { get; set; }
    [NotNull]
    public string Mail { get; set; }
}

public class Role
{
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
}
