using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ValveControlSystem.Classes
{
    public class GetBrushes
    {
        public List<Rectangle> GetRectangles(ObjectDataProvider PropertyName)
        {
            List<Rectangle> result = new List<Rectangle>();
            PropertyInfo[] pi = PropertyName.Data as PropertyInfo[];
            foreach (var item in pi)
            {
                Rectangle newRect = new Rectangle();
                newRect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(item.Name));
                newRect.Height = 20;
                newRect.Width = 60;
                newRect.Margin = new Thickness(0);
                result.Add(newRect);
            }
            return result;
        }
    }
}
