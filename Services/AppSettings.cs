using Microsoft.AspNetCore.Http;

namespace Services;

public class ConnectionStrings
{
    public string MDMConnectionString { get; init; } = string.Empty;
}

public class NLogSettings
{
    public bool LogError { get; init; } = true;
    public bool LogInfo { get; set; }
    public bool LogWarning { get; init; }
    public bool LogDebug { get; init; }
}

public class CookieSettings
{
    public bool HttpOnly { get; set; } = true;
    public bool Secure { get; set; }
    public int SameSiteInt { get; set; }
    public int CookieExpiryInDays { get; set; }

    public SameSiteMode SamSiteMode
    {
        get
        {
            switch (SameSiteInt)
            {
                case 0:
                    return SameSiteMode.None;
                case 1:
                    return SameSiteMode.Lax;
                case 2:
                    return SameSiteMode.Strict;
                default:
                    return SameSiteMode.Lax;
            }
        }
    }
}

public class MesMasterApi
{
    public string ApiUrl { get; set; } = string.Empty;
    public string ApiBasePath { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class AppSettings
{
    public string BuildVersion { get; init; } = "";
    public string AppName { get; init; } = "";
    public ConnectionStrings ConnectionStrings { get; init; } = new();
    public bool EnableOtp { get; init; }
    public NLogSettings NLogSettings { get; init; } = new();
    public CookieSettings CookieSettings { get; init; } = new();
    public MesMasterApi MesMasterApi {  get; init; } = new();
    
    public string MDMConnectionString
    {
        get
        {
            return ConnectionStrings is null ? string.Empty : ConnectionStrings.MDMConnectionString;
        }
    }
    public int CacheExpiryByMinutes { get; init; }
}