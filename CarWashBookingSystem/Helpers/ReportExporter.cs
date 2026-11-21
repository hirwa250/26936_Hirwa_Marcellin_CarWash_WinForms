using System.Data;
using System.Text;

namespace CarWashBookingSystem.Helpers
{
    public static class ReportExporter
    {
        public static void ToCsv(DataTable table, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", table.Columns.Cast<DataColumn>().Select(c => Escape(c.ColumnName))));
            foreach (DataRow row in table.Rows)
                sb.AppendLine(string.Join(",", row.ItemArray.Select(v => Escape(Convert.ToString(v)))));
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private static string Escape(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        public static void ToPdf(string title, DataTable table, string path)
        {
            var lines = new List<string>();
            lines.Add(title);
            lines.Add("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            lines.Add("");
            int[] widths = table.Columns.Cast<DataColumn>()
                .Select(c => Math.Max(c.ColumnName.Length, table.Rows.Cast<DataRow>()
                    .Select(r => Convert.ToString(r[c]).Length).DefaultIfEmpty(0).Max()) + 2)
                .ToArray();
            lines.Add(FormatRow(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), widths));
            lines.Add(new string('-', widths.Sum()));
            foreach (DataRow row in table.Rows)
                lines.Add(FormatRow(row.ItemArray.Select(v => Convert.ToString(v)).ToArray(), widths));
            WritePdf(lines, path);
        }

        private static string FormatRow(string[] values, int[] widths)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
                sb.Append(values[i].PadRight(widths[i]));
            return sb.ToString();
        }

        private static void WritePdf(List<string> lines, string path)
        {
            const int perPage = 50;
            var pages = new List<List<string>>();
            for (int i = 0; i < lines.Count; i += perPage)
                pages.Add(lines.Skip(i).Take(perPage).ToList());
            if (pages.Count == 0) pages.Add(new List<string>());

            var objects = new List<string>();
            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
            string kids = string.Join(" ", Enumerable.Range(0, pages.Count).Select(i => (4 + i * 2) + " 0 R"));
            objects.Add("<< /Type /Pages /Kids [" + kids + "] /Count " + pages.Count + " >>");
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>");
            foreach (var page in pages)
            {
                var content = new StringBuilder();
                content.Append("BT /F1 9 Tf 40 800 Td 14 TL\n");
                foreach (var line in page)
                    content.Append("(" + PdfText(line) + ") '\n");
                content.Append("ET");
                int contentId = objects.Count + 2;
                objects.Add("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 3 0 R >> >> /Contents " + contentId + " 0 R >>");
                objects.Add("<< /Length " + Encoding.ASCII.GetByteCount(content.ToString()) + " >>\nstream\n" + content + "\nendstream");
            }

            var output = new StringBuilder("%PDF-1.4\n");
            var offsets = new List<int>();
            for (int i = 0; i < objects.Count; i++)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(output.ToString()));
                output.Append((i + 1) + " 0 obj\n" + objects[i] + "\nendobj\n");
            }
            int xref = Encoding.ASCII.GetByteCount(output.ToString());
            output.Append("xref\n0 " + (objects.Count + 1) + "\n0000000000 65535 f \n");
            foreach (int offset in offsets)
                output.Append(offset.ToString("D10") + " 00000 n \n");
            output.Append("trailer\n<< /Size " + (objects.Count + 1) + " /Root 1 0 R >>\nstartxref\n" + xref + "\n%%EOF");
            File.WriteAllBytes(path, Encoding.ASCII.GetBytes(output.ToString()));
        }

        private static string PdfText(string value)
        {
            var clean = new string(value.Select(c => c < 128 ? c : '?').ToArray());
            return clean.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }
    }
}
