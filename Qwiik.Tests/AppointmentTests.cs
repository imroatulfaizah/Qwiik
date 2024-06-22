using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Qwiik.Models;
using Qwiik.Repositories;
using Xunit;

namespace Qwiik.Tests
{
    public class AppointmentTests
    {
        [Fact]
        public void GetAppointments_ReturnsAppointmentsForGivenDate()
        {
            // Arrange
            var repository = new AppointmentRepository();
            var date = new DateTime(2024, 6, 25);
            var appointment1 = new Appointment { Id = 1, Date = date, CustomerName = "John Doe" };
            var appointment2 = new Appointment { Id = 2, Date = date, CustomerName = "Jane Smith" };
            repository.BookAppointment(appointment1);
            repository.BookAppointment(appointment2);

            // Act
            var appointments = repository.GetAppointments(date);

            // Assert
            Assert.Equal(2, appointments.Count());
            Assert.Contains(appointment1, appointments);
            Assert.Contains(appointment2, appointments);
        }

        [Fact]
        public void BookAppointment_AddsAppointment()
        {
            // Arrange
            var repository = new AppointmentRepository();
            var appointment = new Appointment { Id = 1, Date = new DateTime(2024, 6, 25), CustomerName = "John Doe" };

            // Act
            var bookedAppointment = repository.BookAppointment(appointment);

            // Assert
            Assert.NotNull(bookedAppointment);
            Assert.Equal(appointment, bookedAppointment);
            Assert.Contains(appointment, repository.GetAppointments(appointment.Date));
        }

        [Fact]
        public void SetOffDay_MarksDateAsOffDay()
        {
            // Arrange
            var repository = new AppointmentRepository();
            var date = new DateTime(2024, 7, 4);

            // Act
            repository.SetOffDay(date);

            // Assert
            Assert.True(repository.IsOffDay(date));
        }

        [Fact]
        public void IsOffDay_ReturnsTrueForOffDay()
        {
            // Arrange
            var repository = new AppointmentRepository();
            var date = new DateTime(2024, 12, 25);
            repository.SetOffDay(date);

            // Act
            var isOffDay = repository.IsOffDay(date);

            // Assert
            Assert.True(isOffDay);
        }

        [Fact]
        public void SetMaxAppointmentsPerDay_SetsMaxAppointments()
        {
            // Arrange
            var repository = new AppointmentRepository();
            var maxAppointments = 5;

            // Act
            repository.SetMaxAppointmentsPerDay(maxAppointments);

            // Assert
            Assert.Equal(maxAppointments, repository.GetMaxAppointmentsPerDay());
        }

        [Fact]
        public void BookAppointment_AddsAppointmentOnNextAvailableDay()
        {
            // Arrange
            var repository = new AppointmentRepository();
            var date = new DateTime(2024, 6, 25);
            var appointment1 = new Appointment { Id = 1, Date = date, CustomerName = "John Doe" };
            var appointment2 = new Appointment { Id = 2, Date = date, CustomerName = "Jane Smith" };
            repository.BookAppointment(appointment1);
            repository.BookAppointment(appointment2);

            // Set max appointments per day to 2 to trigger overflow to the next day
            repository.SetMaxAppointmentsPerDay(2);

            var nextDay = date.AddDays(1);
            var appointment3 = new Appointment { Id = 3, Date = date, CustomerName = "Alice Johnson" };

            // Act
            var bookedAppointment = repository.BookAppointment(appointment3);

            // Assert
            Assert.Equal(nextDay, bookedAppointment.Date);
            Assert.Contains(appointment3, repository.GetAppointments(nextDay));
        }
    }
}
