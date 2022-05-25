using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */


        /* Initialization */
        GlavniLik Heroj;
        Oruzje Metak1, Vuk;
        SetaciNoci Setac1, Setac2;
        NapredniSetac NocniKralj, NocniJahac, Zmaj;
        Sprite vucic1, vucic2, vucic3;

        Random r = new Random();

        SoundPlayer upgrade = new SoundPlayer("sound\\upgrade.wav");
        SoundPlayer zivot_manje = new SoundPlayer("sound\\lost_life.wav");
        SoundPlayer gameover = new SoundPlayer("sound\\game_over.wav");
        SoundPlayer setacismrt = new SoundPlayer("sound\\setaci_smrt.wav");

        //delegati
        public delegate void EventHandler(Setaci s);
        public static event EventHandler GubiZivot;
        public delegate void GameOver();
        public static event GameOver game_over;
        
        private void gubi_zivot(Setaci s)
        {
            Heroj.Zivot -= 1;
            ISPIS="BODOVI" +Heroj.Bodovi.ToString()+ "\n ŽIVOT: " + Heroj.Zivot.ToString();
            s.GotoXY(GameOptions.RightEdge, r.Next(0, 350));
            s.SetVisible(false);
            zivot_manje.PlaySync();
            Wait(3);
            s.SetVisible(true);
        }


        private void GAMEOVER()
        {
            START = false;
            Heroj.SetVisible(false);
            Metak1.SetVisible(false);
            Setac1.SetVisible(false);
            Setac2.SetVisible(false);
            NocniKralj.SetVisible(false);
            NocniJahac.SetVisible(false);
            Zmaj.SetVisible(false);
            vucic1.SetVisible(false);
            vucic2.SetVisible(false);
            vucic3.SetVisible(false);
            setBackgroundPicture("backgrounds\\gameover_back.jpg");
            ISPIS = "BODOVI: " + Heroj.Bodovi.ToString();
            gameover.PlaySync();
        }

        List<Sprite> k = new List<Sprite>();

        private void SetupGame()
        {
            ODABIR i = new ODABIR();
            i.ShowDialog();
            //1. setup stage
            SetStageTitle("Game of Thrones");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\background.jpg");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            Heroj = new GlavniLik(i.Putanja, 10, 50, 0);
            Heroj.AddCostumes("sprites//dany_zmaj.png");
            //x od metka je 90 zbog 10 kraj x od heroja i sirine heroja od 80
            Metak1 = new Oruzje("sprites//metak.png", 90, 0, 5);
            Metak1.AddCostumes("sprites//metak_vatreni.png");
            Metak1.Heigth = 30;
            Metak1.Width = 30;
            Metak1.SetVisible(false);

            //da se šetači ne bi slučajno sreli s vukovima dok nisu vidljivi
            Vuk = new Oruzje("sprites//vuk.png", GameOptions.LeftEdge - 400, 0, 10);
            Vuk.SetVisible(false);
            Vuk.Heigth = 100;
            Vuk.Width = 150;

            vucic1= new Sprite("sprites//mini_vucic.png", 150, 0);
            vucic2= new Sprite("sprites//mini_vucic.png", 180, 0);
            vucic3= new Sprite("sprites//mini_vucic.png", 210, 0);
            k.Add(vucic1);
            k.Add(vucic2);
            k.Add(vucic3);

            Setac1 = new SetaciNoci("sprites//setac.png", GameOptions.RightEdge, 0);
            Setac1.AddCostumes("sprites//ruka.png");
            Setac2 = new SetaciNoci("sprites//setac.png", GameOptions.RightEdge, 200);
            Setac2.AddCostumes("sprites//ruka.png");
            NocniKralj = new NapredniSetac("sprites//nocni_kralj.png", GameOptions.RightEdge, 350, 300, 5);
            NocniKralj.AddCostumes("sprites//ruka.png");
            NocniJahac = new NapredniSetac("sprites//nocni_jahac.png", GameOptions.RightEdge, 150, 300, 8);
            NocniJahac.AddCostumes("sprites//ruka.png");
            NocniKralj.SetVisible(false);
            NocniJahac.SetVisible(false);
            Zmaj = new NapredniSetac("sprites//zmaj.png", GameOptions.RightEdge, 250, 500, 6);
            Zmaj.AddCostumes("sprites//ruka.png");
            Zmaj.SetVisible(false);
            //2. add sprites
            Game.AddSprite(Heroj);
            Game.AddSprite(vucic1);
            Game.AddSprite(vucic2);
            Game.AddSprite(vucic3);
            Game.AddSprite(Setac1);
            Game.AddSprite(NocniKralj);
            Game.AddSprite(Setac2);
            Game.AddSprite(NocniJahac);
            Game.AddSprite(Zmaj);
            Game.AddSprite(Metak1);
            Game.AddSprite(Vuk);
            //delegat
            GubiZivot += gubi_zivot;
            game_over += GAMEOVER;

            //3. scripts that start
            Game.StartScript(KretanjeGlavnogLika);
            Game.StartScript(KretanjeMetka);
            Game.StartScript(KretanjeSetaca1);
            Game.StartScript(KretanjeSetaca2);
            Game.StartScript(KretanjeNocnogKralja);
            Game.StartScript(KretanjeNocnogJahaca);
            Game.StartScript(KretanjeVuka);
            ISPIS = "BODOVI: " + Heroj.Bodovi.ToString() + "\n ŽIVOT: " + Heroj.Zivot.ToString();
        }
        bool pogodens1 = false;
        bool pogodens2 = false;
        bool pogodennjahac = false;
        bool pogodennkralj = false;
        bool pogodenzmaj = false;

        private void SetacKraj(Setaci s)
        {
            Heroj.Bodovi += s.Bodovi;
            ISPIS = "BODOVI " + Heroj.Bodovi.ToString() + "\n ŽIVOT: " + Heroj.Zivot.ToString();
            s.NextCostume();
            s.Heigth = 100;
            setacismrt.PlaySync();
            Wait(0.5);
            s.SetVisible(false);
            s.GotoXY(GameOptions.RightEdge, r.Next(0, 350));
            Wait(2);
            s.NextCostume();
            s.Heigth = 150;
            s.SetVisible(true);
        }

        /* Scripts */

        private int KretanjeGlavnogLika()
        {
            while (Heroj.Bodovi < 2500 && START)
            {
                if (sensing.KeyPressed(Keys.Up))
                {
                    Heroj.Y -= 5;
                }
                if (sensing.KeyPressed(Keys.Down))
                {
                    Heroj.Y += 5;
                }
                if (Heroj.Y < 0)
                {
                    Heroj.Y = 0;
                }
                if (Heroj.Y > GameOptions.DownEdge - 100)
                {
                    Heroj.Y = GameOptions.DownEdge - 100;
                }
                Wait(0.001);
                if(Heroj.Zivot==0)
                {
                    game_over.Invoke();
                }



            }
            Heroj.NextCostume();
            ISPIS = "UPGRADE!!!";
            Game.StartScript(KretanjeGlavnogLika2);
            Game.StartScript(KretanjeZmaja);

            return 0;
        }

        private int KretanjeGlavnogLika2()
        {
            if (START)
                upgrade.PlaySync();
            Metak1.NextCostume();
            while(Heroj.Bodovi>=2500&&START)
            {
                Metak1.Brzina = 6;
                if (sensing.KeyPressed(Keys.Up))
                {
                    Heroj.Y -= 5;
                }
                if (sensing.KeyPressed(Keys.Down))
                {
                    Heroj.Y += 5;
                }
                if (Heroj.Y < 0)
                {
                    Heroj.Y = 0;
                }
                if (Heroj.Y > GameOptions.DownEdge - 100)
                {
                    Heroj.Y = GameOptions.DownEdge - 100;
                }
                Wait(0.0001);
                if (Heroj.Zivot == 0)
                {
                    game_over.Invoke();
                }
            }
            return 0;
        } 

        private int KretanjeMetka()
        {
            while (true)
            {
                if (sensing.KeyPressed(Keys.Space))
                {
                    Metak1.GotoXY(90, Heroj.Y + 15);
                    Metak1.SetVisible(true);
                    while (Metak1.X <= GameOptions.RightEdge)
                    {
                        Metak1.X += Metak1.Brzina;
                        if(Metak1.TouchingSprite(Setac1)&& pogodens1 == false)
                        {
                            Metak1.SetVisible(false);
                            Metak1.GotoXY(0, 0);
                            Setac1.Bodovi -= Metak1.Snaga;
                            break;
                        }
                        if (Metak1.TouchingSprite(Setac2) && pogodens2 == false)
                        {
                            Metak1.SetVisible(false);
                            Metak1.GotoXY(0, 0);
                            Setac2.Bodovi -= Metak1.Snaga;
                            break;
                        }
                        if (Metak1.TouchingSprite(NocniKralj) && pogodennkralj== false)
                        {
                            Metak1.SetVisible(false);
                            Metak1.GotoXY(0, 0);
                            NocniKralj.Bodovi -= Metak1.Snaga;
                            break;
                        }
                        if (Metak1.TouchingSprite(NocniJahac) && pogodennjahac == false)
                        {
                            Metak1.SetVisible(false);
                            Metak1.GotoXY(0, 0);
                            NocniJahac.Bodovi -= Metak1.Snaga;
                            break;
                        }
                        if (Metak1.TouchingSprite(Zmaj) && pogodenzmaj == false)
                        {
                            Metak1.SetVisible(false);
                            Metak1.GotoXY(0, 0);
                            Zmaj.Bodovi -= Metak1.Snaga;
                            break;
                        }
                        Wait(0.01);

                    }
                }
                if (Metak1.TouchingEdge())
                {
                    Metak1.SetVisible(false);
                }
                Wait(0.001);
            }
            return 0;
        }

        private int KretanjeSetaca1()
        {
            while (START)
            {
                Setac1.X -= Setac1.Brzina;
                if (Heroj.Bodovi >= 2500)
                {
                    Wait(0.05);
                }
                else
                    Wait(0.1);
                if (Setac1.Bodovi <= 0 || Setac1.TouchingSprite(Vuk))
                {
                    Setac1.Bodovi = 200;
                    pogodens1 = true;

                    SetacKraj(Setac1);
                    pogodens1 = false;
                }
                if (Setac1.X <= GameOptions.LeftEdge || Setac1.TouchingSprite(Heroj))
                {
                    GubiZivot.Invoke(Setac1);
                }
            }
            return 0;
        }

        private int KretanjeSetaca2()
        {
            while (START)
            {
                Setac2.X -= Setac2.Brzina;
                if (Heroj.Bodovi >= 2500)
                {
                    Wait(0.05);
                }
                else
                    Wait(0.1);
                if (Setac2.Bodovi <= 0 || Setac2.TouchingSprite(Vuk))
                {
                    Setac2.Bodovi = 200;
                    pogodens2 = true;
                    SetacKraj(Setac2);
                    pogodens2 = false;
                }
                if (Setac2.X <= GameOptions.LeftEdge || Setac2.TouchingSprite(Heroj))
                {
                    GubiZivot.Invoke(Setac2);
                }
            }
            return 0;
        }

        private int KretanjeNocnogKralja()
        {
            Wait(2);
            NocniKralj.SetVisible(true);
            while (START)
            {
                NocniKralj.X -= NocniKralj.Brzina;
                if (Heroj.Bodovi >= 2500)
                {
                    Wait(0.06);
                }
                else
                    Wait(0.1);
                if (NocniKralj.Bodovi <= 0 || NocniKralj.TouchingSprite(Vuk))
                {
                    NocniKralj.Bodovi = 300;
                    pogodennkralj = true;
                    SetacKraj(NocniKralj);
                    pogodennkralj = false;
                }
                if (NocniKralj.X <= GameOptions.LeftEdge || NocniKralj.TouchingSprite(Heroj))
                {
                    GubiZivot.Invoke(NocniKralj);
                }
            }
            return 0;
        }

        private int KretanjeNocnogJahaca()
        {
            Wait(5);
            NocniJahac.SetVisible(true);
            while (START)
            {
                NocniJahac.X -= NocniJahac.Brzina;
                if (Heroj.Bodovi >= 2500)
                {
                    Wait(0.05);
                }
                else
                    Wait(0.08);
                if (NocniJahac.Bodovi <= 0 || NocniJahac.TouchingSprite(Vuk))
                {
                    NocniJahac.Bodovi = 300;
                    pogodennjahac = true;
                    SetacKraj(NocniJahac);
                    pogodennjahac = false;
                }
                if (NocniJahac.X <= GameOptions.LeftEdge || NocniJahac.TouchingSprite(Heroj))
                {
                    GubiZivot.Invoke(NocniJahac);
                }
            }
            return 0;
        }

        private int KretanjeZmaja()
        {
            while (START && Heroj.Bodovi >= 2500)
            {
                Zmaj.SetVisible(true);
                Zmaj.X -= Zmaj.Brzina;
                Wait(0.08);
                if (Zmaj.Bodovi <= 0 || Zmaj.TouchingSprite(Vuk))
                {
                    Zmaj.Bodovi = 400;
                    Heroj.Bodovi += Zmaj.Bodovi;
                    ISPIS = "BODOVI: " + Heroj.Bodovi.ToString() + "\n ŽIVOT: " + Heroj.Zivot.ToString();
                    Zmaj.NextCostume();
                    Zmaj.Heigth = 100;
                    pogodenzmaj = true;
                    Wait(1);
                    Zmaj.SetVisible(false);
                    Zmaj.GotoXY(GameOptions.RightEdge, r.Next(0, 350));
                    Wait(8);
                    Zmaj.NextCostume();
                    Zmaj.Heigth = 150;
                    Zmaj.SetVisible(true);
                    pogodenzmaj = false;
                }
                if (Zmaj.X <= GameOptions.LeftEdge || Zmaj.TouchingSprite(Heroj))
                {
                    game_over.Invoke();
                    ISPIS = "BODOVI: " + Heroj.Bodovi.ToString();
                }
            }
            return 0;
        }
        int brojac = 0;
        private int KretanjeVuka()
        {
            while (START)
            {
                if (sensing.KeyPressed(Keys.Enter) && k.Count > 0)
                {
                    brojac++;
                    Vuk.GotoXY(GameOptions.LeftEdge - 150, Heroj.Y);
                    Vuk.SetVisible(true);
                    while (Vuk.X <= GameOptions.RightEdge)
                    {
                        Vuk.X += Vuk.Brzina;
                        Wait(0.1);

                    }
                }
                if (Vuk.X > GameOptions.RightEdge)
                {
                    Vuk.SetVisible(false);
                    Vuk.GotoXY(-400, 0);
                }
                if (brojac == 1)
                {
                    vucic3.SetVisible(false);
                    k.Remove(vucic3);
                }
                if (brojac == 2)
                {
                    vucic2.SetVisible(false);
                    k.Remove(vucic2);
                }
                if (brojac == 3)
                {
                    vucic1.SetVisible(false);
                    k.Remove(vucic1);
                    break;
                }
            }
            return 0;
        }
        private int Metoda()
        {
            while (START) //ili neki drugi uvjet
            {

                Wait(0.1);
            }
            return 0;
        }



        /* ------------ GAME CODE END ------------ */


    }
}
