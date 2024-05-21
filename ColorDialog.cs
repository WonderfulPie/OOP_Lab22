using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;

namespace Lab22
{
    public class ColorDialog
    {
        private readonly System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

        public bool ShowDialog()
        {
            return colorDialog.ShowDialog() == DialogResult.OK;
        }

        public System.Windows.Media.Color SelectedColor => System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
    }
}