using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Auth.Models;
using Application.Auth.Commands;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Выполняет вход пользователя в систему
    /// </summary>
    /// <param name="command">Модель с данными для авторизации (имя пользователя и пароль)</param>
    /// <returns>JWT-токен и сведения о пользователе</returns>
    /// <response code="200">Авторизация выполнена успешно</response>
    /// <response code="400">Неверное имя пользователя или пароль</response>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
