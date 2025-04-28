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
        string url = _httpClient.BaseAddress + string.Format("api/MasterData/GetMenuSettingGetMenuSettingByUserIdAndSystemCode?userId={0}&systemCode=CAR", userId);
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
        Uri baseAddressUri = _httpClient.BaseAddress;
        string baseAddress = baseAddressUri.ToString();

        if (baseAddress.EndsWith("/"))
        {
            baseAddress = baseAddress.TrimEnd('/');
        }

        Uri baseAddressUrinew = new Uri(baseAddress);
        var url = baseAddressUrinew + "/api/MailCenter/SendEmail";
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

        //foreach(var filePath in param.AttachmentsPath)
        //{
        //    var fileBytes = await File.ReadAllBytesAsync(filePath);
        //    var file = new FileInfo(filePath);

        //    formData.Add(new ByteArrayContent(fileBytes), "Files", file.Name);
        //}

        if (param.AttachmentsPath != null)
        {
            foreach (var filePath in param.AttachmentsPath)
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(filePath);
                    var file = new FileInfo(filePath);
                    formData.Add(new ByteArrayContent(fileBytes), "Files", file.Name);
                }
            }
        }

        // Linked files untuk embedded images
        if (param.LinkedFiles != null)
        {
            foreach (var linkedFile in param.LinkedFiles)
            {
                if (!string.IsNullOrEmpty(linkedFile.FilePath) && File.Exists(linkedFile.FilePath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(linkedFile.FilePath);
                    var file = new FileInfo(linkedFile.FilePath);
                    var content = new ByteArrayContent(fileBytes);

                    // Penting: set header Content-ID
                    content.Headers.Add("Content-ID", $"<{linkedFile.ContentId}>");

                    formData.Add(content, "LinkedFiles", file.Name);
                }
            }
        }

        //await _httpClient.PostAsync(url, formData);

        //// Check if the request was successful
        HttpResponseMessage response = await _httpClient.PostAsync(url, formData);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Success! Response: {responseBody}");
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error Details: {errorContent}");
        }
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
