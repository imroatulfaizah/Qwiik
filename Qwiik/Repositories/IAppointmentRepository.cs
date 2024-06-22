using Qwiik.Models;

public interface IAppointmentRepository
{
    IEnumerable<Appointment> GetAppointments(DateTime date);
    Task<Appointment> BookAppointmentAsync(Appointment appointment);
    string SetOffDay(DateTime date);
    void SetMaxAppointmentsPerDay(int maxAppointments);
    int GetMaxAppointmentsPerDay();
}
