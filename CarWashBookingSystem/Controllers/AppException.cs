namespace CarWashBookingSystem.Controllers
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message)
        {
        }
    }
}
