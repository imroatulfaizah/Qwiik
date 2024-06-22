using System;
using System.Collections.Generic;
using Qwiik.Models;

namespace Qwiik.Repositories
{
    public interface IAppointmentRepository
    {
        IEnumerable<Appointment> GetAppointments(DateTime date);
        Appointment BookAppointment(Appointment appointment);
        void SetOffDay(DateTime date);
        bool IsOffDay(DateTime date);
        void SetMaxAppointmentsPerDay(int maxAppointments);
        int GetMaxAppointmentsPerDay();
    }
}
