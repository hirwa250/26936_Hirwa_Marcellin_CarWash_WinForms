using CarWashBookingSystem.Data;
using CarWashBookingSystem.Helpers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Controllers
{
    public class NotificationController
    {
        private readonly NotificationRepository repository = new NotificationRepository();

        public List<Notification> GetUnread()
        {
            return repository.GetForUser(Session.CurrentUser.Id, true);
        }

        public List<Notification> GetAll()
        {
            return repository.GetForUser(Session.CurrentUser.Id, false);
        }

        public void MarkAllRead()
        {
            repository.MarkAllRead(Session.CurrentUser.Id);
        }
    }
}
