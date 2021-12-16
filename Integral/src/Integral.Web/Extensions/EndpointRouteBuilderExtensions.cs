using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;

namespace Integral.Web.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapRobotsTxt(this IEndpointRouteBuilder endpoints, IWebHostEnvironment env)
        {
            endpoints.MapFallbackToFile("robots.txt", $"/assets/robots/robots.{env.EnvironmentName}.txt");
        }
    }
}
