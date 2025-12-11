using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MinAPI.Endpoints
{
    public static class TestEndpoints
    {
        public static RouteGroupBuilder MapTestEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/not-found/{id}", (int id) =>
            {
                if (id <= 0)
                {
                    return Results.NotFound(new { Message = $"Item with ID {id} not found." });
                }
                return Results.Ok(new { Id = id, Name = "Item Found" });
            });

            group.MapGet("/validation-single", () =>
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Field1", new[] { "Field1 is required." } }
                };
                return Results.ValidationProblem(errors);
            });

            group.MapGet("/validation-multiple", () =>
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Field1", new[] { "Field1 is required." } },
                    { "Field2", new[] { "Field2 must be at least 5 characters long." } }
                };
                return Results.ValidationProblem(errors);
            });

            group.MapGet("/database-error", () =>
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateException("Simulated database update error.");
            });

            group.MapGet("/custom-error", () =>
            {
                throw new Exception("This is a custom error for testing.");
            });

            group.MapGet("/unauthorized", () =>
            {
                return Results.Unauthorized();
            });

            group.MapGet("/argument-null", () =>
            {
                string? value = null;
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Simulated ArgumentNullException.");
                }
                return Results.Ok();
            });

            group.MapGet("/invalid-operation", () =>
            {
                throw new InvalidOperationException("Simulated InvalidOperationException.");
            });

            group.MapGet("/timeout", async () =>
            {
                await Task.Delay(5000); // Simulate delay
                throw new TimeoutException("The operation has timed out.");
            });

            group.MapGet("/generic-error", () =>
            {
               throw new Exception("Simulated generic exception.");
            });

             group.MapGet("/db-update-error", () =>
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Simulated concurrency exception.");
            });

            group.MapGet("/success", () =>
            {
                return Results.Ok(new { Message = "Operation successful." });
            });

            return group;
        }
    }
}
