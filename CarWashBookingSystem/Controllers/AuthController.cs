using CarWashBookingSystem.Data;
using CarWashBookingSystem.Helpers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Controllers
{
    public class AuthController
    {
        private readonly UserRepository users = new UserRepository();

        public User Login(string email, string password)
        {
            if (Validator.IsEmpty(email, password))
                throw new AppException("Please enter your email and password.");
            var user = users.GetByEmail(email.Trim());
            if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
                throw new AppException("Incorrect email or password.");
            Session.CurrentUser = user;
            return user;
        }

        public void Logout()
        {
            Session.CurrentUser = null;
        }

        public void Register(string name, string email, string phone, string password, string confirm, string plate, string vehicleType)
        {
            if (Validator.IsEmpty(name, email, phone, password, plate, vehicleType))
                throw new AppException("All fields are required.");
            email = email.Trim();
            if (!Validator.IsEmail(email))
                throw new AppException("Please enter a valid email, for example user@example.com.");
            if (!Validator.IsPhone(phone.Trim()))
                throw new AppException("The phone number must be exactly 10 digits.");
            if (!Validator.IsPassword(password))
                throw new AppException("The password must be at least 8 characters long.");
            if (password != confirm)
                throw new AppException("The passwords do not match.");
            if (users.EmailExists(email))
                throw new AppException("An account with this email already exists.");

            users.Add(new User
            {
                FullName = name.Trim(),
                Email = email,
                Phone = phone.Trim(),
                PasswordHash = PasswordHasher.Hash(password),
                Role = "Customer",
                PlateNumber = plate.Trim().ToUpper(),
                VehicleType = vehicleType
            });
        }

        public void ChangePassword(string current, string newPassword, string confirm)
        {
            var user = users.GetById(Session.CurrentUser.Id);
            if (Validator.IsEmpty(current, newPassword, confirm))
                throw new AppException("Please fill in all password fields.");
            if (!PasswordHasher.Verify(current, user.PasswordHash))
                throw new AppException("The current password is incorrect.");
            if (!Validator.IsPassword(newPassword))
                throw new AppException("The new password must be at least 8 characters long.");
            if (newPassword != confirm)
                throw new AppException("The new passwords do not match.");
            users.UpdatePassword(user.Id, PasswordHasher.Hash(newPassword));
        }

        public void UpdateProfile(string phone, string plate, string vehicleType)
        {
            if (Validator.IsEmpty(phone, plate, vehicleType))
                throw new AppException("Phone, plate number and vehicle type are required.");
            if (!Validator.IsPhone(phone.Trim()))
                throw new AppException("The phone number must be exactly 10 digits.");
            var user = Session.CurrentUser;
            user.Phone = phone.Trim();
            user.PlateNumber = plate.Trim().ToUpper();
            user.VehicleType = vehicleType;
            users.UpdateProfile(user);
        }
    }
}
