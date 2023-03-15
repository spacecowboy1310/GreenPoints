using System.Diagnostics.CodeAnalysis;

public class DescriptionProperty
{
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string Description { get; set; }
}
