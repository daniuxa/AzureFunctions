namespace AzureFunctions.Requests;

public class UserInfoRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}