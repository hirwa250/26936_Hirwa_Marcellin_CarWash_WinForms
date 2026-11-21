namespace CarWashBookingSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string PlateNumber { get; set; }
        public string VehicleType { get; set; }
    }
}
