using CarWashBookingSystem.Data;
using CarWashBookingSystem.Helpers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Controllers
{
    public class ServiceTypeController
    {
        private readonly ServiceTypeRepository repository = new ServiceTypeRepository();

        public List<ServiceType> GetAll()
        {
            return repository.GetAll();
        }

        private ServiceType Build(int id, string name, string description, string price, string duration)
        {
            if (Validator.IsEmpty(name, price, duration))
                throw new AppException("Name, price and duration are required.");
            if (!decimal.TryParse(price, out decimal p) || p <= 0)
                throw new AppException("The price must be a number greater than zero.");
            if (!int.TryParse(duration, out int d) || d <= 0)
                throw new AppException("The duration must be a whole number of minutes greater than zero.");
            return new ServiceType { Id = id, Name = name.Trim(), Description = (description ?? "").Trim(), Price = p, DurationMinutes = d };
        }

        public void Add(string name, string description, string price, string duration)
        {
            repository.Add(Build(0, name, description, price, duration));
        }

        public void Update(int id, string name, string description, string price, string duration)
        {
            repository.Update(Build(id, name, description, price, duration));
        }

        public void Delete(int id)
        {
            if (repository.CountActiveBookings(id) > 0)
                throw new AppException("This service has pending or in-progress bookings and cannot be deleted.");
            if (repository.CountAllBookings(id) > 0)
                throw new AppException("This service is used in past bookings and cannot be deleted. You can edit it instead.");
            repository.Delete(id);
        }
    }
}
