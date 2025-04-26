using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerGraphicsLab3Paint
{
    public class Polygon : Shape
    {
        public List<Line> Lines { get; set;} = new();
        private int _Thickness;
        public int Thickness {
            get => _Thickness;
            set{
                _Thickness = value;
                if (Lines != null)
                {
                    foreach(var line in Lines){
                        line.Thickness = _Thickness;
                    }
                }
            }
        }
        public Polygon() : base(new MyColor(0,0,0))
        {}
        public Polygon(Polygon p) : base(p.Color)
        {
            foreach (var line in p.Lines)
            {
                Lines.Add(new Line(line));
            }
            Thickness = p.Thickness;
        }
        public Polygon(List<Point> points, MyColor c, int t) : base(c)
        {
            Thickness = t;
            for(int i = 0; i < points.Count() - 1; i++){
                Point p1 = points[i];
                Point p2 = points[i + 1];
                Lines.Add(new Line(p1, p2, c, Thickness));//all lines share corol ref
            }
            Lines.Add(new Line(points[0], points[points.Count() - 1], c, Thickness));
        }
        public override void Draw(byte[] buffer, int Width, int Height){
            foreach (var line in Lines)
            {
                line.Draw(buffer, Width, Height);
            }
        }
        public void GuptaSproullsDraw(byte[] buffer, int Width, int Height, MyColor bg){
            foreach (var line in Lines)
            {
                line.GuptaSproullsDraw(buffer, Width, Height, bg);
            }
        }
        public override bool isHovered(Point clickPoint){
            foreach(var line in Lines) {
                if (line.isHovered(clickPoint)) return true;
            }
            return false;
        }
        public void setColor(MyColor c){
            Color = c;
            foreach (var line in Lines)
            {
                line.Color = new MyColor(c.R, c.G, c.B);
            }
        }
        public override void Move(int dx, int dy){
            foreach (var line in Lines)
            {
                line.Move(dx, dy);
            }
        }
        public override void Strech(Point start, Point end){
            int errorMargin = 14;
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            for (int i = 0; i < Lines.Count(); i++){
                if (Lines[i].isHovered(start)){
                    Lines[i].Strech(start, end);
                }
            }

        }
    }
}