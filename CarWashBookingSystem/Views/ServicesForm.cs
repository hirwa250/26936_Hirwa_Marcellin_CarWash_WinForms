using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class ServicesForm : BaseForm
    {
        public ServicesForm() : base("Services", 640, 360)
        {
            AddTitle("Available Services");
            AddBackButton();
            var grid = AddGrid(20, 60, 600, 280);
            Run(() =>
            {
                grid.DataSource = new ServiceTypeController().GetAll()
                    .Select(s => new { s.Name, s.Description, Price = s.Price.ToString("N0") + " RWF", Duration = s.DurationMinutes + " min" })
                    .ToList();
            });
        }
    }
}
