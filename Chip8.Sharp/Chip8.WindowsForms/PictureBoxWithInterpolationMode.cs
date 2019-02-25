using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Chip8.WindowsForms
{
    class PictureBoxWithInterpolationMode : PictureBox
    {
        // http://stackoverflow.com/a/13484101/25124
        public InterpolationMode InterpolationMode { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
        }
    }
}
