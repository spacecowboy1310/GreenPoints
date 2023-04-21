using System.Diagnostics.CodeAnalysis;

public class DescriptionProperty
{
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string Description { get; set; }
}

public class EditDescriptionProperty
{
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string Description { get; set; }
    [NotNull]
    public User Collaborator { get; set; }
    public int EditGreenPointId { get; set; }
    public DescriptionProperty ToDescriptionProperty()
    {
        return new()
        {
            Name = Name,
            Description = Description
        };
    }
}
