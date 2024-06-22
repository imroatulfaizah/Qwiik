namespace Qwiik.Models
{
    public class AppointmentSetting
    {
        public int Id { get; set; }
        // Assuming there's only one row in this table that holds the global setting
        public int MaxAppointmentsPerDay { get; set; }
    }
}
