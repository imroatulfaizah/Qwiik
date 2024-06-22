using Qwiik.Models;


namespace Qwiik.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments = new List<Appointment>();
        private readonly List<OffDay> _offDays = new List<OffDay>();
        private int _maxAppointmentsPerDay = 10;

        public IEnumerable<Appointment> GetAppointments(DateTime date)
        {
            return _appointments.Where(a => a.Date.Date == date.Date);
        }

        public Appointment BookAppointment(Appointment appointment)
        {
            var nextAvailableDate = appointment.Date;

            while (true)
            {
                if (IsOffDay(nextAvailableDate) ||
                    GetAppointments(nextAvailableDate).Count() >= _maxAppointmentsPerDay)
                {
                    nextAvailableDate = nextAvailableDate.AddDays(1);
                    continue;
                }

                appointment.Date = nextAvailableDate;
                _appointments.Add(appointment);
                return appointment;
            }
        }

        public void SetOffDay(DateTime date)
        {
            if (!_offDays.Any(d => d.Date.Date == date.Date))
            {
                _offDays.Add(new OffDay { Date = date });
            }
        }

        public bool IsOffDay(DateTime date)
        {
            return _offDays.Any(d => d.Date.Date == date.Date);
        }

        public void SetMaxAppointmentsPerDay(int maxAppointments)
        {
            _maxAppointmentsPerDay = maxAppointments;
        }

        public int GetMaxAppointmentsPerDay()
        {
            return _maxAppointmentsPerDay;
        }
    }
}
