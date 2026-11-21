using System.Data;
using CarWashBookingSystem.Data;
using CarWashBookingSystem.Helpers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Controllers
{
    public class StaffController
    {
        private readonly StaffRepository repository = new StaffRepository();

        public List<Staff> GetAll()
        {
            return repository.GetAll();
        }

        private Staff Build(int id, string name, string phone, string specialization, bool available)
        {
            if (Validator.IsEmpty(name, phone, specialization))
                throw new AppException("Name, phone and specialization are required.");
            if (!Validator.IsPhone(phone.Trim()))
                throw new AppException("The phone number must be exactly 10 digits.");
            return new Staff { Id = id, FullName = name.Trim(), Phone = phone.Trim(), Specialization = specialization.Trim(), IsAvailable = available };
        }

        public void Add(string name, string phone, string specialization, bool available)
        {
            repository.Add(Build(0, name, phone, specialization, available));
        }

        public void Update(int id, string name, string phone, string specialization, bool available)
        {
            repository.Update(Build(id, name, phone, specialization, available));
        }

        public void Delete(int id)
        {
            repository.Delete(id);
        }

        public DataTable GetBookings(int staffId)
        {
            return repository.GetBookingsForStaff(staffId);
        }
    }
}
