using System.Drawing;
using System.Windows.Forms;

namespace Lab22
{
    public class FontDialog
    {
        private readonly System.Windows.Forms.FontDialog fontDialog = new System.Windows.Forms.FontDialog();

        public bool ShowDialog()
        {
            return fontDialog.ShowDialog() == DialogResult.OK;
        }

        public string FontName => fontDialog.Font.Name;
        public double FontSize => (double)fontDialog.Font.Size;
    }
}