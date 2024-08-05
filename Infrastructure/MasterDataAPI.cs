using Contracts.Infrastructure;
using Entities;
using Entities.Account;
using Entities.MasterData;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Entities.Infrastructure;
using Entities.Account.Dto;

namespace Infrastructure;

public class MasterDataAPI(IHttpClientFactory httpClientFactory) : IMasterDataApi
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MasterDataAPI");

    private static async Task<T> ProcessApiResponseContent<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(result);

        if (!apiResponse.Success)
            throw new InvalidOperationException(apiResponse.Message);

        var jsonRawContent = JsonConvert.SerializeObject(apiResponse.Content);

        return JsonConvert.DeserializeObject<T>(jsonRawContent);
    }

    public async Task<IEnumerable<MenuItem>> GetMenuSetting(string userId)
    {
        string url = _httpClient.BaseAddress + string.Format("api/MasterData/GetMenuSettingGetMenuSettingByUserIdAndSystemCode?userId={0}&systemCode=CRCU", userId);
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        return await ProcessApiResponseContent<IEnumerable<MenuItem>>(response);
    }
    public async Task<UserInfo> GetUserInfoByUserId(string userId)
    {
        var url = _httpClient.BaseAddress + string.Format("api/Authentication/GetUserInfoByUserId?userId={0}", userId);

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return default;

        return await ProcessApiResponseContent<UserInfo>(response);
    }

    public async Task SendEmail(SendEmailParam param)
    {
        var url = _httpClient.BaseAddress + "/api/MailCenter/SendEmail";

        using var formData = new MultipartFormDataContent
        {
            { new StringContent(param.FromName), "FromName" },
            { new StringContent(param.FromAddress), "FromAddress" },
            { new StringContent(param.Recipient), "Recipient" },
            { new StringContent(param.Subject), "Subject" },
            { new StringContent(param.Body), "Body" },
            { new StringContent(param.CreateUser), "CreateUser" },
            { new StringContent(param.CopyRecipient), "CopyRecipient" }
        };

        foreach(var filePath in param.AttachmentsPath)
        {
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var file = new FileInfo(filePath);

            formData.Add(new ByteArrayContent(fileBytes), "Files", file.Name);
        }

        await _httpClient.PostAsync(url, formData);
    }

    #region auth
    public async Task<T> Login<T>(LoginDto user, bool enableOtp)
    {
        if (user is null) throw new InvalidDataException("User is null");

        var url = _httpClient.BaseAddress + "api/Authentication/Login";

        var userLoginDto = new UserLoginDto(user.UseID, user.UsePass, enableOtp);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, userLoginDto);
        
        return await ProcessApiResponseContent<T>(response);
    }
    public async Task<TokenResponse> RefreshToken(RegenerateTokenForUserDto user)
    {
        if (user is null) throw new InvalidDataException("User is null");

        var url = _httpClient.BaseAddress + "api/Authentication/RefreshToken";

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, user);

        return await ProcessApiResponseContent<TokenResponse>(response);
    }
    #endregion
}
