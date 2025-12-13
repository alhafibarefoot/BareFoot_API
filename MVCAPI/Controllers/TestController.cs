using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVCAPI.Controllers
{
    [ApiController]
    [Route("api/test")]
    [Tags("Testing")]
    public class TestController : ControllerBase
    {
        [HttpGet("not-found/{id}")]
        public IActionResult GetNotFound(int id)
        {
            if (id <= 0)
            {
                return NotFound(new { Message = $"Item with ID {id} not found." });
            }
            return Ok(new { Id = id, Name = "Item Found" });
        }

        [HttpGet("validation-single")]
        public IActionResult GetValidationSingle()
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Field1", new[] { "Field1 is required." } }
            };
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        [HttpGet("validation-multiple")]
        public IActionResult GetValidationMultiple()
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Field1", new[] { "Field1 is required." } },
                { "Field2", new[] { "Field2 must be at least 5 characters long." } }
            };
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        [HttpGet("database-error")]
        public IActionResult GetDatabaseError()
        {
            throw new DbUpdateException("Simulated database update error.");
        }

        [HttpGet("custom-error")]
        public IActionResult GetCustomError()
        {
            throw new Exception("This is a custom error for testing.");
        }

        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }

        [HttpGet("argument-null")]
        public IActionResult GetArgumentNull()
        {
            string? value = null;
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Simulated ArgumentNullException.");
            }
            return Ok();
        }

        [HttpGet("invalid-operation")]
        public IActionResult GetInvalidOperation()
        {
            throw new InvalidOperationException("Simulated InvalidOperationException.");
        }

        [HttpGet("timeout")]
        public async Task<IActionResult> GetTimeout()
        {
            await Task.Delay(5000); // Simulate delay
            throw new TimeoutException("The operation has timed out.");
        }

        [HttpGet("generic-error")]
        public IActionResult GetGenericError()
        {
            throw new Exception("Simulated generic exception.");
        }

        [HttpGet("db-update-error")]
        public IActionResult GetDbUpdateConcurrencyError()
        {
            throw new DbUpdateConcurrencyException("Simulated concurrency exception.");
        }

        [HttpGet("success")]
        public IActionResult GetSuccess()
        {
            return Ok(new { Message = "Operation successful." });
        }
    }
}
