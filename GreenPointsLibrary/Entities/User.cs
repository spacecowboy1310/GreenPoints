﻿using System.Diagnostics.CodeAnalysis;

public class User
{
    public int Id { get; set; }
    [NotNull]
    public string Username { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    [NotNull]
    public string Password { get; set; }
    [NotNull]
    public string Mail { get; set; }
    public List<Role> Roles { get; set; }
    public List<GreenPoint> Collaborations { get; set; }
    public UserDTO ToDTO()
    {
        List<string> roles = new();
        foreach (Role role in Roles)
        {
            roles.Add(role.Name);
        }
        List<int> collaborations = new();
        foreach (GreenPoint greenPoint in Collaborations ?? new())
        {
            collaborations.Add(greenPoint.Id);
        }
        return new()
        {
            Id = Id,
            Username = Username,
            Password = Password,
            Mail = Mail,
            Roles = roles,
            Collaborations = collaborations
        };
    }
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
    public List<User> Users { get; set; }
}

public static class Roles
{
    public static readonly string Collaborator = "Collaborator";
    public static readonly string Editor = "Editor";
    public static readonly string Administrator = "Administrator";
}
