using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Svg2Xaml
{
    class SvgViewbox
    {
        public Rect Value { private set; get; }

        public static SvgViewbox Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            value = value.Trim();

            if (value == "")
                throw new ArgumentException("value must not be empty.");

            return new SvgViewbox(Rect.Parse(value));
        }

        public SvgViewbox(Rect value)
        {
            this.Value = value;
        }

        public GeometryDrawing Process()
        {
            GeometryDrawing drawing = new GeometryDrawing();
            drawing.Brush = Brushes.Transparent;
            drawing.Geometry = new RectangleGeometry(this.Value);

            return drawing;
        }
    }
}
