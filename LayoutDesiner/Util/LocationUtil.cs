using MapEditor.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapEditor.Util
{
    public static class LocationUtil
    {

        public static LocationModel SetLocationModel(Panel panel)
        {
            if(panel == null)
            {
                throw new Exception("Grid is Null");
            }            
           
            Point position = Mouse.GetPosition(panel);
            ImageBrush imageSource = (ImageBrush)panel.Background;
            ImageSource IS = imageSource.ImageSource;
            //var imageHeight = Math.Round(IS.Height,1);
            //var imageWidth = Math.Round(IS.Width,1);

            //double mx = Math.Round(position.X, 1);
            //double my = Math.Round(position.Y, 1);
            var imageHeight = Math.Round(IS.Height);
            var imageWidth = Math.Round(IS.Width);

            double mx = Math.Round(position.X);
            double my = Math.Round(position.Y);
            return new LocationModel(mx, my, imageWidth , imageHeight);
        }

        public static double[] CalOriginalPosition(Panel panel , LocationModel LM)
        {
            double[] result = new double[2];

            Point position = Mouse.GetPosition(panel);
            var actualHeight = panel.ActualHeight;
            var actualWidth = panel.ActualWidth;
            var xRatio = position.X / actualWidth;
            var yRatio = position.Y / actualHeight;
            
            double originalXPositon = xRatio * LM.image_W;
            double originalYPosition = yRatio * LM.image_H;
            //result[0] = Math.Round(originalXPositon,1);
            //result[1] = Math.Round(originalYPosition,1);
            result[0] = Math.Round(originalXPositon);
            result[1] = Math.Round(originalYPosition);

            return result;
        }

        public static double[] ReturnActualSize(Panel panel)
        {
            double[] result = new double[2];

            Point position = Mouse.GetPosition(panel);
            var actualHeight = panel.ActualHeight;
            var actualWidth = panel.ActualWidth;

            //result[0] = Math.Round(actualWidth,1);
            //result[1] = Math.Round(actualHeight, 1);
            result[0] = Math.Round(actualWidth);
            result[1] = Math.Round(actualHeight);

            return result;
        }
    }
}
