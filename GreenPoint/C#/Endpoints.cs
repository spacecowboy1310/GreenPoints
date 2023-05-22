namespace GreenPoints.C_;
public static class Endpoints
{
    private static string URI = "https://localhost:7204";

    private static string Login = "/login";
    private static string Register = "/register";
    private static string Confirm = "/confirm";
    private static string ChangeRole = "/changeRole";

    private static string GreenPoints = "/greenpoints";
    private static string Request = "/request";
    private static string Accept = "/accept";

    public static string GetURLForConfirm(Guid id) { return URI + Confirm + $"/{id}"; }
    public static string PostURLForLogin() { return URI + Login; }
    public static string PostURLForRegister() { return URI + Register; }
    public static string PostURLForChangeRole() { return URI + ChangeRole; }

    // GREEN POINTS

    public static string GetURLForGreenPoints(double lat1, double lon1, double lat2, double lon2)
    {
        return URI + GreenPoints + Request + $"/{lat1}/{lon1}/{lat2}/{lon2}";
    }
    public static string GetURLForGreenPoints(int id) { return URI + GreenPoints + $"/{id}";  }
    public static string GetURLForGreenPointRequest() { return URI + GreenPoints + Request;  }
    public static string PostURLForGreenPointRequest() { return URI + GreenPoints + Request; }
    public static string PostURLForGreenPointAccept() { return URI + GreenPoints + Accept; }

}
