using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Helpers;

namespace CarWashBookingSystem.Views
{
    public class CustomerMenuForm : BaseForm
    {
        private readonly NotificationController notifications = new NotificationController();
        private readonly Button btnNotifications;

        public CustomerMenuForm() : base("Customer Menu", 360, 420)
        {
            AddTitle("Welcome, " + Session.CurrentUser.FullName);
            AddButton("View Services", 60, 70, (s, e) => OpenChild(new ServicesForm()), 240);
            AddButton("Book a Car Wash", 60, 115, (s, e) => OpenChild(new BookingForm(), UpdateNotifications), 240);
            AddButton("My Bookings", 60, 160, (s, e) => OpenChild(new MyBookingsForm(), UpdateNotifications), 240);
            btnNotifications = AddButton("Notifications", 60, 205, (s, e) => OpenChild(new NotificationsForm(), UpdateNotifications), 240);
            AddButton("My Profile", 60, 250, (s, e) => OpenChild(new ProfileForm()), 240);
            AddButton("Logout", 60, 330, (s, e) => Close(), 240);
            Shown += (s, e) => ShowUnread();
            UpdateNotifications();
        }

        private void UpdateNotifications()
        {
            Run(() =>
            {
                int count = notifications.GetUnread().Count;
                btnNotifications.Text = count > 0 ? "Notifications (" + count + " new)" : "Notifications";
            });
        }

        private void ShowUnread()
        {
            Run(() =>
            {
                var unread = notifications.GetUnread();
                if (unread.Count == 0) return;
                string text = string.Join(Environment.NewLine, unread.Take(10).Select(n => n.CreatedAt.ToString("yyyy-MM-dd HH:mm") + "  " + n.Message));
                MessageBox.Show(text, "You have " + unread.Count + " new notification(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                notifications.MarkAllRead();
                UpdateNotifications();
            });
        }
    }
}
