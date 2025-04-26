using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;


namespace ComputerGraphicsLab3Paint
{
    public abstract class Shape
    {
        // z-index maby
        public MyColor Color {get; set;}
        public abstract void Draw(byte[] buffer, int Width, int Height);
        public abstract bool isHovered(Point clickPoint);
        public abstract void Move(int dx, int dy);
        public abstract void Strech(Point start, Point end);
        public Shape(MyColor c){
            Color = new MyColor(c.R, c.G, c.B);
        }
        protected void DrawPixel(int x, int y, byte[] buffer, int Width, int Height){
            if(x < 0 || x >= Width || y < 0 || y >= Height) return;

            int index = y * Width * 4 + x * 4;
            buffer[index] = this.Color.B;
            buffer[index + 1] = this.Color.G;
            buffer[index + 2] = this.Color.R;
        }
        protected void varIntensityDrawPixel(float intensityMultiplier, MyColor bg, int x, int y, byte[] buffer, int Width, int Height){
            if(x < 0 || x >= Width || y < 0 || y >= Height) return;
            if(intensityMultiplier == 0) return;

            int index = y * Width * 4 + x * 4;
            if(intensityMultiplier < 1 && intensityMultiplier > 0){
                int itDrawsGray = 6;
            }

            byte r = (byte)(Math.Clamp((1 - intensityMultiplier) * bg.R + this.Color.R * intensityMultiplier, 0, 255));
            byte g = (byte)(Math.Clamp((1 - intensityMultiplier) * bg.G + this.Color.G * intensityMultiplier, 0, 255));
            byte b = (byte)(Math.Clamp((1 - intensityMultiplier) * bg.B + this.Color.B * intensityMultiplier, 0, 255));
            buffer[index] = b;
            buffer[index + 1] = g;
            buffer[index + 2] = r;
        }
        public static void DrawPixelStatic(int x, int y, byte[] buffer, int Width, int Height, MyColor Color){
            if(x < 0 || x >= Width || y < 0 || y >= Height) return;

            int index = y * Width * 4 + x * 4;
            buffer[index] = Color.B;
            buffer[index + 1] = Color.G;
            buffer[index + 2] = Color.R;
        }
    }
    
    
}