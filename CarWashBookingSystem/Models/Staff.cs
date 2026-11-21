namespace CarWashBookingSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public bool IsAvailable { get; set; }

        public override string ToString()
        {
            return FullName + " (" + Specialization + ")";
        }
    }
}
