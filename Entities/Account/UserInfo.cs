namespace Entities.Account;

public class UserInfo
{
    public int UseComCod { get; set; }
    public string UseID { get; set; } = "";
    public string UseNam { get; set; } = "";
    public string UseEmail { get; set; } = "";
    public string UseDep { get; set; } = "";
    public string vendor { get; set; } = "";
    public string vendorDescription { get; set; } = "";

    public UserInfo()
    {

    }

    public UserInfo(TokenResponse user)
    {
        UseComCod = user.UseComCod;
        UseID = user.UseID;
        UseNam = user.UseNam;
        UseEmail = user.UseEmail;
        UseDep = user.UseDep;
        vendor = user.vendor;
        vendorDescription = user.vendorDescription;
    }
}
