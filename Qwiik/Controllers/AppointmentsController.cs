using Microsoft.AspNetCore.Mvc;
using Qwiik.Models;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _repository;

    public AppointmentsController(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    #region API Endpoints

    [HttpGet("{date}")]
    public IActionResult GetAppointments(DateTime date)
    {
        var appointments = _repository.GetAppointments(date);
        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> BookAppointment([FromBody] Appointment appointment)
    {
        var bookedAppointment = await _repository.BookAppointmentAsync(appointment);
        return Ok(bookedAppointment);
    }

    [HttpPost("offday")]
    public IActionResult SetOffDay([FromBody] DateTime date)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (date == DateTime.MinValue)
        {
            return BadRequest("Please provide a valid date.");
        }


        if (date.Date < DateTime.UtcNow.Date)
        {
            return BadRequest("The date cannot be in the past.");
        }
        var resultMessage = _repository.SetOffDay(date);
        if (resultMessage.Contains("already been set"))
        {
            return BadRequest(resultMessage);
        }

        return Ok(resultMessage);
    }

    [HttpPost("maxappointments")]
    public IActionResult SetMaxAppointments([FromBody] int maxAppointments)
    {
        _repository.SetMaxAppointmentsPerDay(maxAppointments);
        return Ok();
    }

    [HttpGet("maxappointments")]
    public IActionResult GetMaxAppointments()
    {
        var maxAppointments = _repository.GetMaxAppointmentsPerDay();
        return Ok(maxAppointments);
    }

    #endregion
}
