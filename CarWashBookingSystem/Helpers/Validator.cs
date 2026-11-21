using System.Text.RegularExpressions;

namespace CarWashBookingSystem.Helpers
{
    public static class Validator
    {
        public static bool IsEmail(string value)
        {
            return Regex.IsMatch(value ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsPhone(string value)
        {
            return Regex.IsMatch(value ?? "", @"^\d{10}$");
        }

        public static bool IsPassword(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length >= 8;
        }

        public static bool IsEmpty(params string[] values)
        {
            return values.Any(string.IsNullOrWhiteSpace);
        }
    }
}
