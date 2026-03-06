using Application.Flights.Commands.CreateFlight;
using Application.Flights.Commands.UpdateFlight;
using Application.Flights.DTOs;
using Application.Flights.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FlightsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список всех рейсов с возможностью фильтрации по пункту отправления и назначения.
    /// </summary>
    /// <param name="origin">Пункт отправления (необязательно)</param>
    /// <param name="destination">Пункт назначения (необязательно)</param>
    /// <returns>Список рейсов, отсортированных по времени прилёта</returns>
    /// <response code="200">Список рейсов успешно возвращён</response>
    /// <response code="401">Пользователь не авторизован</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFlights([FromQuery] string? origin, [FromQuery] string? destination)
    {
        var query = new GetFlightsQuery
        {
            Origin = origin,
            Destination = destination
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Создать новый рейс (доступно только пользователям с ролью Moderator)
    /// </summary>
    /// <param name="command">Данные для создания рейса</param>
    /// <returns>Информация о созданном рейсе</returns>
    /// <response code="201">Рейс успешно создан</response>
    /// <response code="400">Ошибка валидации данных</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Доступ запрещён (требуется роль Moderator)</response>
    [HttpPost]
    [Authorize(Policy = "Moderator")]
    [ProducesResponseType(typeof(FlightDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateFlight([FromBody] CreateFlightCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFlights), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить статус рейса по его ID (доступно только пользователям с ролью Moderator)
    /// </summary>
    /// <param name="id">ID рейса, который нужно обновить</param>
    /// <param name="command">Команда с новым статусом рейса</param>
    /// <returns>Без содержимого</returns>
    /// <response code="204">Статус рейса успешно обновлён</response>
    /// <response code="400">ID рейса в URL не совпадает с ID в теле запроса или ошибка валидации</response>
    /// <response code="404">Рейс с указанным ID не найден</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Доступ запрещён (требуется роль Moderator)</response>
    [HttpPut("{id}/status")]
    [Authorize(Policy = "Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateFlightStatus(int id, [FromBody] UpdateFlightStatusCommand command)
    {
        if (id != command.FlightId)
            return BadRequest("ID рейса в URL не совпадает с ID в теле запроса.");

        await _mediator.Send(command);
        return NoContent();
    }
}