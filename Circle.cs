using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerGraphicsLab3Paint
{
    public class Circle : Shape
    {
        public Point Center {get; set;}
        public int Radius {get; set;}
        // fill color can add later
        public Circle(Circle cr)
            :this(cr.Center, cr.Radius, cr.Color)
        {
        }
        public Circle(Point cent, int r, MyColor c)
            :base(c)
        {
            Center = new Point(cent.X, cent.Y);
            Radius = r;
        }
        public Circle() : base(new MyColor(0,0,0))
        {}
        public override void Draw(byte[] buffer, int Width, int Height){
            int d = 1 - Radius;
            int x = 0;//(int)Center.X;
            int y = Radius;//(int)Center.Y + Radius;
            while (y > x)
            {
                DrawPixel(Center.X + x, Center.Y + y, buffer, Width, Height);
                DrawPixel(Center.X - x, Center.Y + y, buffer, Width, Height);
                DrawPixel(Center.X + x, Center.Y - y, buffer, Width, Height);
                DrawPixel(Center.X - x, Center.Y - y, buffer, Width, Height);
                DrawPixel(Center.X + y, Center.Y + x, buffer, Width, Height);
                DrawPixel(Center.X - y, Center.Y + x, buffer, Width, Height);
                DrawPixel(Center.X + y, Center.Y - x, buffer, Width, Height);
                DrawPixel(Center.X - y, Center.Y - x, buffer, Width, Height);
                if ( d < 0 ) //move to E
                    d += 2*x + 3;
                else //move to SE
                {
                    d += 2*x - 2*y + 5;
                    --y;
                }
                ++x;
            }
        }
        public static void DrawCircle(byte[] buffer, int Width, int Height, int Radius, Point Center, MyColor Color){
            int d = 1 - Radius;
            int x = 0;//(int)Center.X;
            int y = Radius;//(int)Center.Y + Radius;
            while (y > x)
            {
                DrawPixelStatic(Center.X + x, Center.Y + y, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X - x, Center.Y + y, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X + x, Center.Y - y, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X - x, Center.Y - y, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X + y, Center.Y + x, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X - y, Center.Y + x, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X + y, Center.Y - x, buffer, Width, Height, Color);
                DrawPixelStatic(Center.X - y, Center.Y - x, buffer, Width, Height, Color);
                if ( d < 0 ) //move to E
                    d += 2*x + 3;
                else //move to SE
                {
                    d += 2*x - 2*y + 5;
                    --y;
                }
                ++x;
            }
        }
        public override bool isHovered(Point clickPoint){
            return Math.Abs(CanvasBitmap.euclidianDistance(clickPoint, Center) - Radius) < 14;
        }
        public override void Move(int dx, int dy)
        {
            Center = new Point(Center.X + dx, Center.Y + dy);
        }
        public override void Strech(Point start, Point end){

            Radius += CanvasBitmap.euclidianDistance(Center, end) - CanvasBitmap.euclidianDistance(Center, start) ;
        }
    }
}