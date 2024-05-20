using Microsoft.AspNetCore.Mvc;
using MinAPI.Data.Models;

namespace MinAPI.EndPoints
{
    public static class DemoEndPoints
    {
        public static RouteGroupBuilder MapDemo(this RouteGroupBuilder group)
        {
            var builder = WebApplication.CreateBuilder();
            var varmyenv = builder.Configuration.GetValue<string>("myenv");

            group.MapGet("/", () => varmyenv);
            group.MapGet("/FromModel", (IDateTime dateTime) => dateTime.Now);

            group.MapGet("/FromRegisteredService", ([FromServices] IDateTime dateTime) => dateTime.Now);

            return group;
        }
    }
}
