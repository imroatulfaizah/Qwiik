using Microsoft.EntityFrameworkCore;
using Qwiik;
using Qwiik.Models;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly QwiikDbContext _context;

    public AppointmentRepository(QwiikDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Appointment> GetAppointments(DateTime date)
    {
        return _context.Appointment
                       .Where(a => a.Date.Date == date.Date)
                       .ToList();
    }

    public async Task<Appointment> BookAppointmentAsync(Appointment appointment)
    {
        var currentDate = appointment.Date.Date;
        var dailyAppointments = await _context.Appointment.CountAsync(a => a.Date.Date == currentDate);

        var maxAppointmentsPerDaySetting = await _context.AppointmentSetting.FirstOrDefaultAsync();
        int maxAppointmentsPerDay = maxAppointmentsPerDaySetting != null ? maxAppointmentsPerDaySetting.MaxAppointmentsPerDay : 0; // Default to 0 if not set

        if (dailyAppointments >= maxAppointmentsPerDay)
        {
            appointment.Date = await FindNextAvailableDay(_context, currentDate);
        }

        appointment.Token = Guid.NewGuid().ToString();
        _context.Appointment.Add(appointment);
        await _context.SaveChangesAsync();

        return appointment;
    }

    private async Task<DateTime> FindNextAvailableDay(QwiikDbContext _context, DateTime startDate)
    {
        DateTime nextAvailableDate = startDate.AddDays(1);

        var maxAppointmentsPerDaySetting = await _context.AppointmentSetting.FirstOrDefaultAsync();
        int maxAppointmentsPerDay = maxAppointmentsPerDaySetting?.MaxAppointmentsPerDay ?? 0;

        while (true)
        {
            bool isOffDay = await _context.OffDays.AnyAsync(od => od.Date.Date == nextAvailableDate);
            if (isOffDay)
            {
                nextAvailableDate = nextAvailableDate.AddDays(1);
                continue;
            }

            var dailyAppointmentsCount = await _context.Appointment
                .CountAsync(a => a.Date.Date == nextAvailableDate);

            if (dailyAppointmentsCount < maxAppointmentsPerDay)
            {
                return nextAvailableDate;
            }

            nextAvailableDate = nextAvailableDate.AddDays(1);
        }
    }

    public string SetOffDay(DateTime date)
    {
        if (_context.OffDays.Any(od => od.Date.Date == date.Date))
        {
            return $"Off day with date {date.Date.ToShortDateString()} has already been set.";
        }

        var offDay = new OffDay { Date = date.Date };
        _context.OffDays.Add(offDay);
        _context.SaveChanges();

        return $"Off day on {date.Date.ToShortDateString()} has been successfully set.";
    }

    public void SetMaxAppointmentsPerDay(int maxAppointments)
    {
        var setting = _context.AppointmentSetting.FirstOrDefault();

        if (setting == null)
        {
            setting = new AppointmentSetting { MaxAppointmentsPerDay = maxAppointments };
            _context.AppointmentSetting.Add(setting);
        }
        else
        {
            setting.MaxAppointmentsPerDay = maxAppointments;
        }

        _context.SaveChanges();
    }

    public int GetMaxAppointmentsPerDay()
    {
        var setting = _context.AppointmentSetting.FirstOrDefault();
        if (setting != null)
        {
            return setting.MaxAppointmentsPerDay;
        }
        return 1;
    }
}
