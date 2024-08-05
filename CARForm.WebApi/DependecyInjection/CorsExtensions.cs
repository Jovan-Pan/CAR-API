namespace WebApi.DependecyInjection;

public static class CorsExtensions
{
    public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Access the nested JSON object for CORS settings
        var corsSettings = configuration.GetSection("CorsSettings");

        // Read the AllowedOrigins and AllowedMethods from the CorsSettings section
        var allowedOrigins = corsSettings["AllowedOrigins"]?.Split(",");
        var allowedMethods = corsSettings["AllowedMethods"]?.Split(",") ?? new[] { "GET", "POST" };

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors",
                policy =>
                {
                    policy
                    .WithOrigins(allowedOrigins)
                    .WithMethods(allowedMethods)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")
                    .WithExposedHeaders("X-HMAC");
                });
        });
    }
}
