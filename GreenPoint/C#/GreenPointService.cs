using System.Net.Http.Json;

namespace GreenPoints.C_;
public class GreenPointService
{
    private readonly HttpClient http;
    public List<GreenPoint> GreenPoints { get; set; } = new List<GreenPoint>();
    public List<EditGreenPoint> EditGreenPoints { get; set; } = new List<EditGreenPoint>();

    public GreenPointService(HttpClient http)
    {
        this.http = http;
    }

    public async Task PostGreenPointRequest(EditGreenPoint editGreenPoint)
    {
        _ = await http.PostAsJsonAsync(Endpoints.PostURLForGreenPointRequest(), editGreenPoint);
    }

    public async Task PostGreenPointAccept(AcceptRequest request)
    {
        _ = await http.PostAsJsonAsync(Endpoints.PostURLForGreenPointAccept(), request);
    }

    public async Task GetGreenPointsInArea(double lat1, double lon1, double lat2, double lon2)
    {
        GreenPoints = await http.GetFromJsonAsync<List<GreenPoint>>(Endpoints.GetURLForGreenPoints(lat1, lon1, lat2, lon2));    
    }

    public async Task<GreenPoint> GetGreenPointById(int id)
    {
        return await http.GetFromJsonAsync<GreenPoint>(Endpoints.GetURLForGreenPoints(id));
    }

    public async Task GetGreenPointRequest()
    {
        EditGreenPoints = await http.GetFromJsonAsync<List<EditGreenPoint>>(Endpoints.GetURLForGreenPointRequest());
    }

}
