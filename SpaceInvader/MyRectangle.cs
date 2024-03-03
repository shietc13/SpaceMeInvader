using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvader
{
    internal class MyRectangle
    {
        public Rectangle ActualRectangle { get; set; }
        public string ID { get; set; }

        public MyRectangle(Rectangle paraRec, string ID_p)
        {
            ActualRectangle = paraRec;
            ID = ID_p;
        }
    }
}
