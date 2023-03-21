using System.Diagnostics.CodeAnalysis;

public class GreenPoint
{
    public int Id {  get; set; }
    [NotNull]
    public long Latitude { get; set; }
    [NotNull]
    public long Longitude { get; set; }
    [NotNull]
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }
    public List<User> Collaborators { get; set; }

}

public class EditGreenPoint
{
    public int Id { get; set; }
    [AllowNull]
    public long? Latitude { get; set; }
    [AllowNull]
    public long? Longitude { get; set; }
    [AllowNull]
    public string Name { get; set; }
    public List<EditDescriptionProperty> Properties { get; set; }
    public User Collaborator { get; set; }
    public GreenPoint Original { get; set; }

}
