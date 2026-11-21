using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class NotificationsForm : BaseForm
    {
        public NotificationsForm() : base("Notifications", 700, 400)
        {
            AddTitle("Notifications");
            AddBackButton();
            var grid = AddGrid(20, 60, 660, 320);
            var controller = new NotificationController();
            Run(() =>
            {
                grid.DataSource = controller.GetAll()
                    .Select(n => new { Date = n.CreatedAt.ToString("yyyy-MM-dd HH:mm"), n.Message, Status = n.IsRead ? "Read" : "New" })
                    .ToList();
                grid.Columns["Message"].FillWeight = 300;
                controller.MarkAllRead();
            });
        }
    }
}
