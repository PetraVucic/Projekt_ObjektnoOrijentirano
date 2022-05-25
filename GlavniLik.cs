using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class GlavniLik:Sprite
    {
        private int bodovi;
        private int zivot;
        public int Bodovi
        {
            get { return bodovi; }
            set { bodovi = value; }
        }
        public int Zivot
        {
            get { return zivot; }
            set
            {
                try
                {
                    if (zivot >= 0)
                        zivot = value;
                }
                catch
                {
                    throw (new ArgumentException());
                }
            }
        }

        public GlavniLik(string image, int x, int y, int bodovi) : base(image, x, y)
        {
            this.bodovi = bodovi;
            this.Heigth = 150;
            this.Width = 150;
            this.zivot = 3;
        }
    }
}
