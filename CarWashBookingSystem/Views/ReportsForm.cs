using System.Data;
using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class ReportsForm : BaseForm
    {
        private readonly ReportController controller = new ReportController();
        private readonly ComboBox cboReport;
        private readonly DateTimePicker dtpFrom, dtpTo;
        private readonly DataGridView grid;
        private DataTable current;
        private string currentTitle;

        public ReportsForm() : base("Reports", 820, 500)
        {
            AddTitle("Reports");
            AddBackButton();
            AddLabel("Report", 20, 65);
            cboReport = AddComboBox(80, 65, 220);
            cboReport.Items.AddRange(ReportController.Reports);
            cboReport.SelectedIndex = 0;
            AddLabel("From", 315, 65);
            dtpFrom = new DateTimePicker { Location = new Point(360, 65), Width = 120, Format = DateTimePickerFormat.Short, Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) };
            Controls.Add(dtpFrom);
            AddLabel("To", 495, 65);
            dtpTo = new DateTimePicker { Location = new Point(525, 65), Width = 120, Format = DateTimePickerFormat.Short };
            Controls.Add(dtpTo);
            AddButton("Generate", 660, 62, Generate_Click, 140);

            grid = AddGrid(20, 110, 780, 320);
            AddButton("Export PDF", 20, 450, (s, e) => Export("PDF file|*.pdf", ".pdf"), 140);
            AddButton("Export CSV", 170, 450, (s, e) => Export("CSV file|*.csv", ".csv"), 140);
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                current = controller.Generate(cboReport.Text, dtpFrom.Value.Date, dtpTo.Value.Date);
                currentTitle = cboReport.Text + " (" + dtpFrom.Value.ToString("yyyy-MM-dd") + " to " + dtpTo.Value.ToString("yyyy-MM-dd") + ")";
                grid.DataSource = current;
                if (current.Rows.Count == 0)
                    MessageBox.Show("No data found for this period.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void Export(string filter, string extension)
        {
            Run(() =>
            {
                if (current == null) throw new AppException("Generate a report first.");
                using var dialog = new SaveFileDialog { Filter = filter, FileName = cboReport.Text.Replace(" ", "_") + extension };
                if (dialog.ShowDialog() != DialogResult.OK) return;
                controller.Export(currentTitle, current, dialog.FileName);
                Success("Report exported to " + dialog.FileName);
            });
        }
    }
}
