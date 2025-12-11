using Microsoft.AspNetCore.Mvc;

namespace MinAPI.Endpoints
{
    public static class HelloEndPoints
    {
        public static RouteGroupBuilder MapHello(this RouteGroupBuilder group)
        {
            group.MapGet("/", () => "[GET] Hello World!");
            group.MapPost("/", () => "[POST] Hello World!");
            group.MapPut("/", () => "[PUT] Hello World!");
            group.MapDelete("/", () => "[DELETE] Hello World!");

            group.MapGet(
                "/QueryString",
                ([FromQuery] string name) =>
                {
                    return $"Hello {name}";
                }
            );
            return group;
        }
    }
}
