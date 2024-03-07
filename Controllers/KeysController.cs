using APIx.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[ApiController]
[Route("[controller]")]
public class KeysController : ControllerBase
{
    [HttpPost(Name = "/")]
    public IActionResult Post([FromBody] PostKeysDTO keys)
    {
        //mandar info pro service
        //checar token
        //checar se usuario existe e pegar suas keys (menos de 20 total, menos de 5 nesse banco)
        //checar se a key já existe
        //checar se a key é válida
        //checar se a conta já existe (se n criar)
        //criar key (talvez junto com a conta)

        return Ok("I'm alive!");
    }
}