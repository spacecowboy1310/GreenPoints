using System.Diagnostics.CodeAnalysis;

public class GreenPoint
{
    public int Id {  get; set; }
    [NotNull]
    public double Latitude { get; set; }
    [NotNull]
    public double Longitude { get; set; }
    [NotNull]
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }
    public List<User> Collaborators { get; set; }

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
    public List<EditDescriptionProperty> Properties { get; set; }
    public User Collaborator { get; set; }
    [AllowNull]
    public GreenPoint? Original { get; set; }
    public void setCollaborator(User collaborator)
    {
        Collaborator = collaborator;
        foreach (EditDescriptionProperty property in Properties)
        {
            property.Collaborator = collaborator;
        }
    }
}
