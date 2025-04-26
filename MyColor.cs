using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerGraphicsLab3Paint
{
    public class MyColor
    {
        public byte R {get; set;} = 0;
        public byte G {get; set;} = 0;
        public byte B {get; set;} = 0;
        public MyColor(byte r, byte g, byte b){
            R = r; G = g; B = b;
        }
    }
}