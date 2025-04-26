using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerGraphicsLab3Paint
{
    public class Line : Shape
    {
        public Point p1 {get; set;}//these are not actual point coordinates
        public Point p2 {get; set;}// actual points are incremented by (0.5, 0.5) (pixelRadius)
                                    // same with pixels: indexed by top left of pixel but  
                                    // center = index + pixelRadius 

                                    // for gouptaSproul I will use actual coordinates(real and untrimmed)
                                    // I will use real points to calculate 4 corner points
                                    // treating the line like a rectangle
        public PointF r1 {get; set;}
        public PointF r2 {get; set;}
        // public PointF u1 {get; set;}// u = up  // d = down
        // public PointF u2 {get; set;}// ex p1 has u1 and d1 
        // public PointF d1 {get; set;}// calculated based on
        // public PointF d2 {get; set;}// gradient and thickness

        public int Thickness {get; set;}
        public Line(Line l)
            : this(l.p1, l.p2, new MyColor(l.Color.R, l.Color.G, l.Color.B), l.Thickness)
        {
            if (l == null)
                throw new ArgumentNullException(nameof(l), "Cannot copy from a null Line.");
        }
        public Line(Point _p1, Point _p2, MyColor c, int t) : base(c)
        {
            p1 = _p1;
            p2 = _p2;
            Thickness = t;

            r1 = new PointF(p1.X + 0.5f, p1.Y + 0.5f);//r = real
            r2 = new PointF(p2.X + 0.5f, p2.Y + 0.5f);
        }
        public Line() : base(new MyColor(0,0,0))
        {}
        public override void Draw(byte[] buffer, int Width, int Height){
            double dy = p1.Y - p2.Y;
            double dx = p1.X - p2.X;

            //to avoid div by 0
            if(dx == 0){
                if(p1.Y < p2.Y){
                    for(int y = (int)p1.Y; y < p2.Y; y++){
                        if(Thickness > 1) {
                            Circle.DrawCircle(buffer, Width, Height, Thickness/2, new Point((int)p1.X, (int)y), Color);
                        } else {
                            DrawPixel((int)p1.X, (int)y, buffer, Width, Height);
                        }
                    }
                } else {
                    for(int y = (int)p2.Y; y < p1.Y; y++){
                        if(Thickness > 1) {
                            Circle.DrawCircle(buffer, Width, Height, Thickness/2, new Point((int)p1.X, (int)y), Color);
                        } else {
                            DrawPixel((int)p1.X, (int)y, buffer, Width, Height);
                        }
                    }
                }
            }

            double m = dy/dx;
            if(p1.X < p2.X){
                double y = p1.Y;
                for(int x = (int)p1.X; x < p2.X; x++){
                    if(Thickness > 1) {
                        Circle.DrawCircle(buffer, Width, Height, Thickness/2, new Point(x, (int)y), Color);
                    } else {
                        this.DrawPixel(x, (int)y, buffer, Width, Height);
                    }
                    y += m;
                }
            } else {
                double y = p2.Y;
                for(int x = (int)p2.X; x < p1.X; x++){
                    if(Thickness > 1) {
                        Circle.DrawCircle(buffer, Width, Height, Thickness/2, new Point(x, (int)y), Color);
                    } else {
                        this.DrawPixel(x, (int)y, buffer, Width, Height);
                    }
                    y += m;
                }
            }
        }
        public void GuptaSproullsDraw(byte[] buffer, int Width, int Height, MyColor bg){
            float dy = p1.Y - p2.Y;
            float dx = p1.X - p2.X;
            int x, X, y, Y;
            //to avoid div by 0
            if(dx == 0){
                if(p1.Y < p2.Y){
                    for(y = (int)p1.Y; y < p2.Y; y++){
                        if(Thickness > 1) {
                            Circle.DrawCircle(buffer, Width, Height, Thickness/2, new Point((int)p1.X, (int)y), Color);
                        } else {
                            DrawPixel((int)p1.X, (int)y, buffer, Width, Height);
                        }
                    }
                } else {
                    for(y = (int)p2.Y; y < p1.Y; y++){
                        if(Thickness > 1) {
                            Circle.DrawCircle(buffer, Width, Height, Thickness/2, new Point((int)p1.X, (int)y), Color);
                        } else {
                            DrawPixel((int)p1.X, (int)y, buffer, Width, Height);
                        }
                    }
                }
            }
            
            float m = dy/dx;
            if(p1.X < p2.X){
                X = p2.X;
                x = p1.X;
            } else {
                x = p2.X;
                X = p1.X;
            }
            if(p1.Y < p2.Y){
                Y = p2.Y;
                y = p1.Y;
            } else {
                y = p2.Y;
                Y = p1.Y;
            }
            y -= 3* Thickness;
            x -= 3*Thickness;
            Y += 3*Thickness;
            X += 3*Thickness;
            // x = 0;
            // y = 0;
            int resety = y;
            // THIS METHOD WILL CUT 2 LINE CORNERS

            for(; x < X; x++){
                y = resety;
                for(; y < Y; y++){
                    PointF p = new PointF( x + 0.5f, y + 0.5f);
                    float A, B, C;
                    A = m;
                    B = -1;
                    C = r1.Y - m * r1.X;

                    float mp = -1/m;
                    float Cp = p.Y - mp * p.X;
                    float xo = (Cp - C)/(m - mp);
                    float yo = m * xo + C;
                    if((xo < r1.X && xo < r2.X) || (xo > r1.X && xo > r2.X)) continue;

                    float D = pointDistFromLine(p, A, B, C);
                    float intensityMultiplier = cov(D);

                    this.varIntensityDrawPixel(intensityMultiplier, bg, x, (int)y, buffer, Width, Height);
                }
            }
            
        }
        public float pointDistFromLine(PointF p, float A, float B, float C){//Line: Ax + By +C = 0
            return (float)Math.Abs(A * p.X + B * p.Y + C) / (float)Math.Sqrt(A*A + B*B);
        }
        public float cov(float D){//D: smallest distance from middle line
            float d = Math.Abs(D - (float)Thickness/2);
            float r = 0.5f;
            float cov;
            if(d >= r) cov = 0f;
            else {
                cov = (float)(Math.Acos(d/r)/Math.PI) - (float)(d * Math.Sqrt(r*r - d*d) / (Math.PI * r*r));
            }
            return Thickness/2 <= D ? cov : 1 - cov;
        }
        public override bool isHovered(Point clickPoint){
            int errorMargin = 14;
            bool hoversp1 = Math.Abs(clickPoint.X - p1.X) < errorMargin && Math.Abs(clickPoint.Y - p1.Y) < errorMargin;
            bool hoversp2 = Math.Abs(clickPoint.X - p2.X) < errorMargin && Math.Abs(clickPoint.Y - p2.Y) < errorMargin;
            return hoversp1 || hoversp2;
        }
        public override void Move(int dx, int dy){
            p1 = new Point(p1.X + dx, p1.Y + dy);
            p2 = new Point(p2.X + dx, p2.Y + dy);
            r1 = new PointF(r1.X + dx, r1.Y + dy);
            r2 = new PointF(r2.X + dx, r2.Y + dy);
        }
        public override void Strech(Point start, Point end){
            int errorMargin = 14;
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            if(Math.Abs(start.X - p1.X) < errorMargin && Math.Abs(start.Y - p1.Y) < errorMargin){
                p1 = new Point(p1.X + dx, p1.Y + dy);
                r1 = new PointF(r1.X + dx, r1.Y + dy);
            } else {
                p2 = new Point(p2.X + dx, p2.Y + dy);
                r2 = new PointF(r2.X + dx, r2.Y + dy);
            }
        }
    }
}