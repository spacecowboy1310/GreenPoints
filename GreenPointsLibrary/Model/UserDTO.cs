﻿public class UserDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Mail { get; set; }
    public List<string> Roles { get; set; }
    public List<GreenPoint> Collaborations { get; set; }
}