using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MapEditor.Model
{
    public class LocationModel
    {
        // mouse_x : point x
        // image_x : source.Width
        public double editor_x { get; private set; }
        public double editor_y { get; private set; }
        public double image_W { get; private set; }
        public double image_H { get; private set; }

        public LocationModel(double mx, double my, double ix, double iy) {
            // 이미지 크기에 따라 변경되도록...?
            this.editor_x = mx;
            this.editor_y = my;
            this.image_W = ix;
            this.image_H = iy;        
        }
    }
}
