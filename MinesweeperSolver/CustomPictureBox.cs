using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackMine
{
    class CustomPictureBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            var g = paintEventArgs.Graphics;

            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            base.OnPaint(paintEventArgs);
        }
    }
}
