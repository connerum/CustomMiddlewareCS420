using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using CustomWebProject;
using System.Text;

namespace CustomMiddlewareTests
{
    public class AuthenticationMiddlewareTests
    {
        [Theory]
        [InlineData("/", HttpStatusCode.Unauthorized)]
        [InlineData("/?username=user1", HttpStatusCode.Unauthorized)]
        [InlineData("/?username=user1&password=password1", HttpStatusCode.OK)]
        [InlineData("/?username=user5&password=password2", HttpStatusCode.Unauthorized)]
        public async Task MiddlewareTest(string url, HttpStatusCode expectedStatusCode)
        {
            var webHostBuilder = new WebHostBuilder().Configure(app =>
            {
                app.UseAuthenticationMiddleware();
                app.Run(async context =>
                {
                    var response = context.Response;
                    response.ContentType = "text/plain";
                    await using (var writer = new StreamWriter(response.Body, Encoding.UTF8, leaveOpen: true))
                    {
                        await writer.WriteAsync("Test Response");
                        await writer.FlushAsync();
                    }
                });
            });

            var server = new TestServer(webHostBuilder);
            var client = server.CreateClient();
            var response = await client.GetAsync(url);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}
