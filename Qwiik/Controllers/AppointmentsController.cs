using Microsoft.AspNetCore.Mvc;
using Qwiik.Models;
using Qwiik.Repositories;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _repository;

    public AppointmentsController(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{date}")]
    public IActionResult GetAppointments(DateTime date)
    {
        var appointments = _repository.GetAppointments(date);
        return Ok(appointments);
    }

    [HttpPost]
    public IActionResult BookAppointment([FromBody] Appointment appointment)
    {
        var bookedAppointment = _repository.BookAppointment(appointment);
        return Ok(bookedAppointment);
    }

    [HttpPost("offday")]
    public IActionResult SetOffDay([FromBody] DateTime date)
    {
        _repository.SetOffDay(date);
        return Ok();
    }

    [HttpPost("maxappointments")]
    public IActionResult SetMaxAppointments([FromBody] int maxAppointments)
    {
        _repository.SetMaxAppointmentsPerDay(maxAppointments);
        return Ok();
    }
}
