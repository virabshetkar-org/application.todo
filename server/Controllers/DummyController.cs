using Microsoft.AspNetCore.Mvc;

namespace Service.Todo.Controllers;

[ApiController]
[Route("/api/todo/[controller]")]
public class DummyController : ControllerBase
{
    [HttpGet(Name = "HelloWorld")]
    public string Get()
    {
        return "Hello, World!!!";
    }
}
