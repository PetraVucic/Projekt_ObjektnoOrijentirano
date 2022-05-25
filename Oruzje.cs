using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Oruzje:Sprite
    {
        private int brzina;
        private int snaga;
        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        public int Snaga
        {
            get { return snaga; }
            set { snaga = value; }
        }

        public Oruzje(string image, int x, int y, int brzina) : base(image, x, y)
        {
            this.brzina = brzina;
            this.snaga = 100;
        }
    }
}
