namespace GreenPointsAPI.Entities;

public class GreenPoint
{

    public string Coords { get; set; }
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }

}
