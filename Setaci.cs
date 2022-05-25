using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace OTTER
{
    abstract public class Setaci:Sprite
    {
        protected int bodovi;
        protected int brzina;

        public int Bodovi
        {
            get { return bodovi; }
            set { bodovi = value; }
        }

        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        public Setaci(string image, int x, int y, int bodovi, int brzina) : base(image, x, y)
        {
            this.Bodovi = bodovi;
            this.Brzina = brzina;
            this.Heigth = 150;
            this.Width = 100;
        }
    }

    public class SetaciNoci : Setaci
    {
        public SetaciNoci(string image, int x, int y, int bodovi=200, int brzina=5) : base(image, x, y, bodovi, brzina)
        {

        }
    }

    public class NapredniSetac : Setaci
    {
        public NapredniSetac(string image, int x, int y, int bodovi, int brzina) : base(image, x, y, bodovi, brzina)
        {

        }
    }
}
