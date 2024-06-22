using Microsoft.EntityFrameworkCore;
using Qwiik.Models;

namespace Qwiik
{

    public class QwiikDbContext : DbContext
    {
        public QwiikDbContext(DbContextOptions<QwiikDbContext> options) : base(options)
        {
        }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<OffDay> OffDays { get; set; }
        public DbSet<AppointmentSetting> AppointmentSetting { get; set; }

    }
}
