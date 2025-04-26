using System;
using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Collections;
using System.Linq;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Runtime.InteropServices;
using System.Drawing;
using Avalonia;
using System.Numerics;

namespace ComputerGraphicsLab3Paint
{
    public class CanvasBitmap : WriteableBitmap
    {
        public ObservableCollection<Line> Lines { get; } = new();
        public ObservableCollection<Circle> Circles { get; } = new();
        public ObservableCollection<Polygon> Polygons { get; } = new();
        public int Height {get;}
        public int Width {get;}
        public MyColor backgroundColor {get; set;} = new MyColor(220, 220, 220);
        public bool _AntiAliasingOn;
        public bool AntiAliasingOn {
            get => _AntiAliasingOn;
            set{
                _AntiAliasingOn = value;
                
            }
        }
        public CanvasBitmap(CanvasData cd)
            : base(
            new PixelSize(cd.Width, cd.Height),      // Width x Height
            new Avalonia.Vector(96, 96),                // DPI
            PixelFormat.Bgra8888,              // Format
            Avalonia.Platform.AlphaFormat.Opaque              // Alpha
        )
        {   
            Height = cd.Height;
            Width = cd.Width;
            AntiAliasingOn = cd.AntiAliasingOn;
            backgroundColor = new MyColor(cd.backgroundColor.R, cd.backgroundColor.G, cd.backgroundColor.B);
            foreach (var circle in cd.Circles)
            {
                Circles.Add(new Circle(circle));
            }
            foreach (var line in cd.Lines)
            {
                Lines.Add(new Line(line));
            }
            foreach (var pol in cd.Polygons)
            {
                Polygons.Add(new Polygon(pol));
            }

            Redraw();
        }
        public CanvasBitmap(int height, int width)
            : base(
            new PixelSize(width, height),      // Width x Height
            new Avalonia.Vector(96, 96),                // DPI
            PixelFormat.Bgra8888,              // Format
            Avalonia.Platform.AlphaFormat.Opaque              // Alpha
        )
        {   
            Height = height;
            Width = width;
            AntiAliasingOn = true;

            SetBackground(new MyColor(220, 220, 220));
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
        public void AddLine(System.Drawing.Point p1, System.Drawing.Point p2, int Thickness, MyColor c)
        {    
            Lines.Add(new Line(p1, p2, new MyColor(c.R, c.G, c.B), Thickness));
            Redraw();
        }
        public void AddPolygon(List<System.Drawing.Point> points, int Thickness, MyColor c)
        {    
            Polygons.Add(new Polygon(points, new MyColor(c.R, c.G, c.B), Thickness));
            Redraw();
        }
        public void AddCircle(System.Drawing.Point p1, System.Drawing.Point p2, MyColor c)
        {    
            Circles.Add(new Circle(p1, euclidianDistance(p1, p2), c));
            Redraw();
        }
        public void Redraw(){//update canvas
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

            foreach (var line in Lines)
            {
                if(AntiAliasingOn){
                    line.GuptaSproullsDraw(buffer, Width, Height, backgroundColor);
                } else {
                    line.Draw(buffer, Width, Height);
                }
            }
            foreach (var poly in Polygons)
            {
                if(AntiAliasingOn){
                    poly.GuptaSproullsDraw(buffer, Width, Height, backgroundColor);
                } else {
                    poly.Draw(buffer, Width, Height);
                }
            }
            foreach (var circle in Circles)
            {
                circle.Draw(buffer, Width, Height);
            }

            using (var fb = this.Lock())
            {
                Marshal.Copy(buffer, 0, fb.Address, buffer.Length);
            }
        }
        public (int index, int shape) whichShape(System.Drawing.Point clickPoint){//update canvas
            int shape;
            for (int k = 0; k < Lines.Count(); k++)
            {
                if(Lines[k].isHovered(clickPoint)){
                    shape = 0;
                    return (k, shape);
                }
            }
            for (int k = 0; k < Polygons.Count(); k++)
            {
                if(Polygons[k].isHovered(clickPoint)){
                    shape = 2;
                    return (k, shape);
                }
            }
            for (int k = 0; k < Circles.Count(); k++)
            {
                if(Circles[k].isHovered(clickPoint)){
                    shape = 1;
                    return (k, shape);
                }
            }
            return (-1, -1);

        }
        public static int euclidianDistance(System.Drawing.Point p1, System.Drawing.Point p2){
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        public bool DeleteShape(int index, int shape)
        {
            bool removed = false;
            switch (shape)
            {
                case 0:
                    if(index < 0 || index >= Lines.Count())
                        break;
                    Lines.RemoveAt(index);
                    removed = true; 
                    Redraw();
                    break;
                case 1:
                    if(index < 0 || index >= Circles.Count())
                        break;
                    Circles.RemoveAt(index); 
                    removed = true; 
                    Redraw();  
                    break;
                case 2:
                    if(index < 0 || index >= Polygons.Count())
                        break;
                    Polygons.RemoveAt(index); 
                    removed = true;   
                    Redraw();
                    break; 
            }
            return removed;
        }
        public bool ChangeThickness(int index, int shape, int t)
        {
            bool applied = false;
            switch (shape)
            {
                case 0:
                    if(index < 0 || index >= Lines.Count())
                        break;
                    Lines[index].Thickness = t;
                    applied = true; 
                    Redraw();
                    break;
                case 2:
                    if(index < 0 || index >= Polygons.Count())
                        break;
                    Polygons[index].Thickness = t; 
                    applied = true;   
                    Redraw();
                    break; 
            }
            return applied;
        }
        public bool ChangeColor(int index, int shape, MyColor c)
        {
            bool applied = false;
            switch (shape)
            {
                case 0:
                    if(index < 0 || index >= Lines.Count())
                        break;
                    Lines[index].Color = new MyColor(c.R, c.G, c.B);
                    applied = true; 
                    Redraw();
                    break;
                case 1:
                    if(index < 0 || index >= Circles.Count())
                        break;
                    Circles[index].Color = new MyColor(c.R, c.G, c.B);
                    applied = true; 
                    Redraw();  
                    break;
                case 2:
                    if(index < 0 || index >= Polygons.Count())
                        break;
                    Polygons[index].setColor(new MyColor(c.R, c.G, c.B));
                    applied = true;   
                    Redraw();
                    break; 
            }
            return applied;
        }
        public bool MoveShape(int index, int shape, System.Drawing.Point p1, System.Drawing.Point p2)
        {
            bool applied = false;
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            switch (shape)
            {
                case 0:
                    if(index < 0 || index >= Lines.Count())
                        break;
                    Lines[index].Move(dx, dy);
                    applied = true; 
                    Redraw();
                    break;
                case 1:
                    if(index < 0 || index >= Circles.Count())
                        break;
                    Circles[index].Move(dx, dy);
                    applied = true;   
                    Redraw();
                    break; 
                case 2:
                    if(index < 0 || index >= Polygons.Count())
                        break;
                    Polygons[index].Move(dx, dy);
                    applied = true;   
                    Redraw();
                    break; 
            }
            return applied;
        }
        public bool StrechShape(int index, int shape, System.Drawing.Point p1, System.Drawing.Point p2)
        {
            bool applied = false;
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            switch (shape)
            {
                case 0:
                    if(index < 0 || index >= Lines.Count())
                        break;
                    Lines[index].Strech(p1, p2);
                    applied = true; 
                    Redraw();
                    break;
                case 1:
                    if(index < 0 || index >= Circles.Count())
                        break;
                    Circles[index].Strech(p1, p2);
                    applied = true;   
                    Redraw();
                    break; 
                case 2:
                    if(index < 0 || index >= Polygons.Count())
                        break;
                    Polygons[index].Strech(p1, p2);
                    applied = true;   
                    Redraw();
                    break; 
            }
            return applied;
        }

        public void ClearAll()
        {
            Lines.Clear();
            Circles.Clear();
            Polygons.Clear();
        }
    }
}