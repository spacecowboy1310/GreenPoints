public class GreenPointDTO
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }
    public List<int> Collaborators { get; set; }

}

public class EditGreenPointDTO
{
    public int Id { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Name { get; set; }
    public List<DescriptionProperty> Properties { get; set; }
    public int Collaborator { get; set; }
    public int? Original { get; set; }
}

public class AcceptRequest
{
    public List<int>? ChangeIDs { get; set; }
    public GreenPointDTO? GreenPoint { get; set; }
}