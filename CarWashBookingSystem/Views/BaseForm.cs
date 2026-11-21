using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class BaseForm : Form
    {
        public static readonly Color MainColor = Color.FromArgb(0, 84, 147);
        public static readonly Color BackColorValue = Color.White;
        public static readonly Color TextColor = Color.Black;

        public BaseForm(string title, int width, int height)
        {
            Text = "Car Wash Booking System - " + title;
            ClientSize = new Size(width, height);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = BackColorValue;
            ForeColor = TextColor;
            Font = new Font("Segoe UI", 10);
        }

        protected Label AddTitle(string text)
        {
            var label = new Label { Text = text, Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = MainColor };
            Controls.Add(label);
            return label;
        }

        protected Label AddLabel(string text, int x, int y)
        {
            var label = new Label { Text = text, Location = new Point(x, y + 3), AutoSize = true };
            Controls.Add(label);
            return label;
        }

        protected TextBox AddTextBox(int x, int y, int width = 220, bool password = false)
        {
            var box = new TextBox { Location = new Point(x, y), Width = width, UseSystemPasswordChar = password };
            Controls.Add(box);
            return box;
        }

        protected ComboBox AddComboBox(int x, int y, int width = 220)
        {
            var box = new ComboBox { Location = new Point(x, y), Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(box);
            return box;
        }

        protected Button AddButton(string text, int x, int y, EventHandler click, int width = 120)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 32),
                BackColor = MainColor,
                ForeColor = BackColorValue,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += click;
            Controls.Add(button);
            return button;
        }

        protected DataGridView AddGrid(int x, int y, int width, int height)
        {
            var grid = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = BackColorValue,
                EnableHeadersVisualStyles = false
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = MainColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = BackColorValue;
            grid.DefaultCellStyle.SelectionBackColor = MainColor;
            grid.DefaultCellStyle.SelectionForeColor = BackColorValue;
            Controls.Add(grid);
            return grid;
        }

        protected void AddBackButton()
        {
            AddButton("Back", ClientSize.Width - 140, 15, (s, e) => Close());
        }

        protected void OpenChild(Form child, Action onReturn = null)
        {
            Hide();
            child.FormClosed += (s, e) =>
            {
                if (IsDisposed) return;
                Show();
                onReturn?.Invoke();
            };
            child.Show();
        }

        protected bool Run(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        protected void Success(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected bool Confirm(string message)
        {
            return MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        protected int SelectedId(DataGridView grid, string column = "Booking ID")
        {
            if (grid.CurrentRow == null)
                throw new AppException("Please select a row first.");
            return Convert.ToInt32(grid.CurrentRow.Cells[column].Value);
        }
    }
}
