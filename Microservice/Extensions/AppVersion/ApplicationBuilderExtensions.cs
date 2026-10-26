using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fusion.Microservice.Extensions.AppVersion;

internal static class ApplicationBuilderExtensions
{
    internal static void UseAppVersion(this IEndpointRouteBuilder app)
    {
        app.MapGet("/app-version", async context =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var gitCommitSha = AppVersionAccessor.GetGitCommitSha();
            var pipelineId = AppVersionAccessor.GetPipelineId();

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                pipeline_id = pipelineId,
                git_commit_sha = gitCommitSha
            }));
        });
    }
}