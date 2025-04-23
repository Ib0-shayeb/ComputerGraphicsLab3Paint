using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Collections;
using Avalonia.Controls.Shapes;
using System.Linq;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Runtime.InteropServices;

namespace ComputerGraphicsLab3Paint
{
    public class CanvasBitmap : WriteableBitmap
    {
        public ObservableCollection<Line> Lines { get; } = new();
        int Height {get;}
        int Width {get;}
        MyColor backgroundColor {get; set;}

        public CanvasBitmap(int height, int width)
            : base(
            new PixelSize(width, height),      // Width x Height
            new Vector(96, 96),                // DPI
            PixelFormat.Bgra8888,              // Format
            Avalonia.Platform.AlphaFormat.Opaque              // Alpha
        )
        {   
            Height = height;
            Width = width;

            SetBackground(new MyColor(200, 200, 200));
        }

        public void SetBackground(MyColor color){
            backgroundColor = color;
            var buffer = new byte [Height * Width * 4];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = y * Width * 4 + x * 4;
                    buffer[index + 0] = color.B;
                    buffer[index + 1] = color.G;
                    buffer[index + 2] = color.R;
                    buffer[index + 3] = 255;
                }
            } 
            
            using (var fb = this.Lock())
            {
                Marshal.Copy(buffer, 0, fb.Address, buffer.Length);
            }
        }
        public void AddLine(Point p1, Point p2)
        {   
            var line = new Line
            {
                StartPoint = p1,
                EndPoint = p2,
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };
            Lines.Add(line);
            //UPDATE BITMAP
            Redraw();
        }
        private void Redraw(){//update canvas
            var buffer = new byte [Height * Width * 4];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = y * Width * 4 + x * 4;
                    buffer[index + 0] = backgroundColor.B;
                    buffer[index + 1] = backgroundColor.G;
                    buffer[index + 2] = backgroundColor.R;
                    buffer[index + 3] = 255;
                }
            } 

            MyColor c = new MyColor(0, 0, 0);
            foreach (var line in Lines)
            {
                DrawLine(line.StartPoint, line.EndPoint, buffer, c);
            }

            using (var fb = this.Lock())
            {
                Marshal.Copy(buffer, 0, fb.Address, buffer.Length);
            }
        }
        private void DrawCircle(Point p, double r, byte[] buffer, MyColor c){
            
        }
        private void DrawLine(Point p1, Point p2, byte[] buffer, MyColor c){
            double dy = p1.Y - p2.Y;
            double dx = p1.X - p2.X;

            //to avoid div by 0
            if(dx == 0){
                if(p1.Y < p2.Y){
                    for(int y = (int)p1.Y; y < p2.Y; y++){
                        DrawPixel((int)p1.X, (int)y, buffer, c);
                    }
                } else {
                    for(int y = (int)p2.Y; y < p1.Y; y++){
                        DrawPixel((int)p1.X, (int)y, buffer, c);
                    }
                }
            }
            
            double m = dy/dx;
            
            if(p1.X < p2.X){
                double y = p1.Y;
                for(int x = (int)p1.X; x < p2.X; x++){
                    //draw
                    DrawPixel(x, (int)y, buffer, c);
                    y += m;
                }
            } else {
                double y = p2.Y;
                for(int x = (int)p2.X; x < p1.X; x++){
                    //draw
                    DrawPixel(x, (int)y, buffer, c);
                    y += m;
                }
            }
        }
        private void DrawPixel(int x, int y, byte[] buffer, MyColor c){
            if(x < 0 || x >= Width || y < 0 || y >= Height) return;

            int index = y * Width * 4 + x * 4;
            buffer[index] = c.B;
            buffer[index + 1] = c.G;
            buffer[index + 2] = c.R;
        }
        // private void CreateHandles()
        // {
        //     foreach (var point in _polyline.Points)
        //     {
        //         var handle = new Ellipse
        //         {
        //             Width = 10,
        //             Height = 10,
        //             Fill = Brushes.Red,
        //             [Canvas.LeftProperty] = point.X - 5,
        //             [Canvas.TopProperty] = point.Y - 5
        //         };

        //         _handles.Add(handle);
        //         Children.Add(handle);
        //     }
        // }
    }
}