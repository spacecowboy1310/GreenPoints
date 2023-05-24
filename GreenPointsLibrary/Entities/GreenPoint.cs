using System.Diagnostics.CodeAnalysis;

public class GreenPoint
{
    public int Id { get; set; }
    [NotNull]
    public double Latitude { get; set; }
    [NotNull]
    public double Longitude { get; set; }
    [NotNull]
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }
    public List<User> Collaborators { get; set; }
    public GreenPointDTO ToDTO()
    {
        List<int> ids = new();
        foreach (User user in Collaborators)
        {
            ids.Add(user.Id);
        }
        return new()
        {
            Id = Id,
            Latitude = Latitude,
            Longitude = Longitude,
            Name = Name,
            Properties = Properties,
            Collaborators = ids
        };
    }
}

public class EditGreenPoint
{
    public int Id { get; set; }
    [AllowNull]
    public double? Latitude { get; set; }
    [AllowNull]
    public double? Longitude { get; set; }
    [AllowNull]
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }
    public int CollaboratorId { get; set; }
    [AllowNull]
    public GreenPoint? Original { get; set; }
}

public class DescriptionProperty
{
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string Description { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual GreenPoint? GreenPoint { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual EditGreenPoint? EditGreenPoint { get; set; }
}
