using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace WebApi.Handler;

public class CustomOpenIdConnectConfigurationRetriever : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        var doc = await retriever.GetDocumentAsync(address, cancel);

        var jsonDoc = JsonDocument.Parse(doc);
        var root = jsonDoc.RootElement;

        var config = new OpenIdConnectConfiguration();

        if (root.TryGetProperty("issuer", out var issuer))
            config.Issuer = issuer.GetString();

        if (root.TryGetProperty("jwks_uri", out var jwksUri))
            config.JwksUri = jwksUri.GetString();

        if (root.TryGetProperty("authorization_endpoint", out var authEndpoint))
            config.AuthorizationEndpoint = authEndpoint.GetString();

        if (root.TryGetProperty("token_endpoint", out var tokenEndpoint))
            config.TokenEndpoint = tokenEndpoint.GetString();

        if (root.TryGetProperty("userinfo_endpoint", out var userinfoEndpoint))
            config.UserInfoEndpoint = userinfoEndpoint.GetString();

        if (root.TryGetProperty("end_session_endpoint", out var endSessionEndpoint))
            config.EndSessionEndpoint = endSessionEndpoint.GetString();

        if (root.TryGetProperty("introspection_endpoint", out var introspectionEndpoint))
            config.IntrospectionEndpoint = introspectionEndpoint.GetString();

        if (root.TryGetProperty("response_types_supported", out var responseTypes))
        {
            foreach (var item in responseTypes.EnumerateArray())
                config.ResponseTypesSupported.Add(item.GetString());
        }

        if (root.TryGetProperty("scopes_supported", out var scopes))
        {
            foreach (var item in scopes.EnumerateArray())
                config.ScopesSupported.Add(item.GetString());
        }

        if (root.TryGetProperty("id_token_signing_alg_values_supported", out var signingAlgs))
        {
            foreach (var item in signingAlgs.EnumerateArray())
                config.IdTokenSigningAlgValuesSupported.Add(item.GetString());
        }

        if (!string.IsNullOrEmpty(config.JwksUri))
        {
            try
            {
                var jwksDoc = await retriever.GetDocumentAsync(config.JwksUri, cancel);
                var jwks = new JsonWebKeySet(jwksDoc);

                foreach (var key in jwks.Keys)
                    config.SigningKeys.Add(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load signing keys: {ex.Message}");
            }
        }

        return config;
    }
}
