using Microsoft.Extensions.Options;
using Services;
using Services.Helper;
using System.Text;

namespace WebApi.Handler;

public class HMACDelegatingHandler(IOptions<AppSettings> appSetting) : DelegatingHandler
{
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null &&
            request.RequestUri.ToString().Contains(appSetting.Value.MesMasterApi.ApiUrl, StringComparison.OrdinalIgnoreCase))
        {
            string messageToHash = ReconstructMessage(request);
            string hmacValue = await HMACHasher.HashValue(messageToHash.ToLower(), appSetting.Value.MesMasterApi.ApiSecretKey);
            request.Headers.Add("X-HMAC", hmacValue);
        }
        return await base.SendAsync(request, cancellationToken);
    }

    private string ReconstructMessage(HttpRequestMessage request)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.Append(request.Method.Method);

        var pathAndQuery = request.RequestUri!.PathAndQuery;

        if (pathAndQuery.Contains(appSetting.Value.MesMasterApi.ApiBasePath))
            pathAndQuery = pathAndQuery.Replace(appSetting.Value.MesMasterApi.ApiBasePath, string.Empty);

        messageBuilder.Append(pathAndQuery);

        return messageBuilder.ToString();
    }
}
