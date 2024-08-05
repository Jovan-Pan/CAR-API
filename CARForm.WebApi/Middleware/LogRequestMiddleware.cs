using Contracts.Infrastructure;
using System.Diagnostics;

namespace WebApi.Middleware;

public class LogRequestMiddleware(RequestDelegate next, ILoggerManager logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (logger.IsLogInfo())
        {
            // Log Request Details
            var request = await FormatRequest(context.Request);
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var endpoint = $"{context.Request.Method} {context.Request.Path}";

            logger.LogInfo($"Incoming request from IP: {ipAddress} to endpoint: {endpoint} with payload: {request}");

            // Copy original response body to read later
            var originalBodyStream = context.Response.Body;

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var stopwatch = Stopwatch.StartNew();
            await next(context);
            stopwatch.Stop();

            // Log Response Details
            var response = await FormatResponse(context.Response);
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            logger.LogInfo($"Response from endpoint: {endpoint} with status code: {context.Response.StatusCode} and payload: {response} (Elapsed Time: {elapsedMilliseconds} ms)");

            // Copy the response body back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        else
        {
            await next(context);
        }
    }

    private static async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var body = request.Body;

        // Rewind the request body to the beginning
        body.Seek(0, SeekOrigin.Begin);

        // Read the request body as a string
        var requestBodyText = await new StreamReader(body).ReadToEndAsync();

        // Rewind the request body again so it can be read by the next middleware
        body.Seek(0, SeekOrigin.Begin);

        return requestBodyText;
    }

    private static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return responseBodyText;
    }
}
