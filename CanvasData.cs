using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerGraphicsLab3Paint
{
    public class CanvasData
    {
        public ObservableCollection<Line> Lines { get; set; } = new();
        public ObservableCollection<Circle> Circles { get; set; } = new();
        public ObservableCollection<Polygon> Polygons { get; set; } = new();
        public int Height { get; set; }
        public int Width { get; set; }
        public MyColor backgroundColor { get; set; }
        public bool AntiAliasingOn { get; set; }
        public CanvasData(CanvasBitmap canvas){
            Height = canvas.Height;
            Width = canvas.Width;
            AntiAliasingOn = canvas.AntiAliasingOn;
            backgroundColor = new MyColor(canvas.backgroundColor.R, canvas.backgroundColor.G, canvas.backgroundColor.B);
            foreach (var circle in canvas.Circles)
            {
                Circles.Add(new Circle(circle));
            }
            foreach (var line in canvas.Lines)
            {
                Lines.Add(new Line(line));
            }
            foreach (var pol in canvas.Polygons)
            {
                Polygons.Add(new Polygon(pol));
            }
        }
        public CanvasData(){}
    }

}