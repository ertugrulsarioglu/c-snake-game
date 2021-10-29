using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace YılanOyunu
{
    public partial class Form1 : Form
    {
        

        public enum Direction //tuş atamaları için enum yapısını oluşturduk
        {
            Up,
            Down,
            Left,
            Right
        }

        public int skor = 0;
        public int X = 1, Y = 1;
        public Direction dr = Direction.Right;
        public Location food = new Location(-1, -1);
        public List<Location> tales = new List<Location>();
        public bool Game = false;
        public Thread oyunThread;
        public int hız = 100 ;

        


        public void HerSeyiSıfırla()
        {
            
            skor = 0; X = 1; Y = 1;
            dr = Direction.Right;
            food = new Location(-1, -1);
            tales = new List<Location>();
            tales.Add(new YılanOyunu.Location(0, 0));
            
        }
        public Form1()
        {
            InitializeComponent();


            tales.Add(new YılanOyunu.Location(0, 0));
            CalcTable();

            oyunThread = new Thread(new ThreadStart(new Action(() =>
           {
               
                   while (Game)
                   {
                       if (dr == Direction.Right) X++; //eğer direction(yön) sağa doğruysa x'i arttır.
                       else if (dr == Direction.Down) Y++;//eğer direction(yön) aşağı doğruysa y'yi arttır.
                       else if (dr == Direction.Up) Y--; // eğer direction(yön) yukarı doğruysa y'yi azalt.
                       else if (dr == Direction.Left) X--; //eğer direction(yön) sola doğruysa x'i azalt.

                       //x yatay ekseni y dikey ekseni temsil ediyor

                       CalcTable();
                       Thread.Sleep(hız); //yılanımızın hızını düşürdük ki hareketini gözle görebilelim.

                   }
               
           })));
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "S") dr = Direction.Down; 
            if (e.KeyCode.ToString() == "W") dr = Direction.Up;
            if (e.KeyCode.ToString() == "A") dr = Direction.Left;
            if (e.KeyCode.ToString() == "D") dr = Direction.Right;
            if(e.KeyCode.ToString() == "Escape")
            {
                MenuGoster(0);
            }
            //Tuşlaarımızı atadık
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = ((Button)sender).Text;
            Menu.Visible = false;
            this.Focus();
            Game = true;
            oyunThread.Resume();

            if (str == "Yeniden Oyna") HerSeyiSıfırla();
            
        }

        public void MenuGoster(int durum)
        {
            if(durum == 0)
            {
                lblBaslık.Text = "Pause";
                button1.Text = "Devam";
                Menu.Visible = true;
                oyunThread.Suspend();
                timer1.Start();
                Game = false;
            }
            else if(durum == 1)
            {   
                Invoke(new Action(() => {
                    lblBaslık.Text = "Oyunu Kaybettin";
                    button1.Text = "Yeniden Oyna";
                    timer1.Stop();
                    oyunsuresi = 0;
                    label3.Text ="";
                    Menu.Visible = true;
                    oyunThread.Suspend();
                    Game = false;
                }));
                    
               
                
            }
        }

        private void btnBasla_Click(object sender, EventArgs e)
        {
            timer1.Start();
            timer1.Interval = 1000;
            
            
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Zorluk Seçiniz !", "Bilgilendirme Penceresi");
                
                AnaMenu.Visible = true;
                Menu.Visible = false;
               
                Game = false;

            }

            else if(comboBox1.Text == "Kolay")
            {
                hız = 100;
                AnaMenu.Visible = false;
                Game = true;
                this.Focus();
                HerSeyiSıfırla();
                if (oyunThread.ThreadState == ThreadState.Unstarted) oyunThread.Start();
                if (oyunThread.ThreadState == ThreadState.Suspended) oyunThread.Resume();
            }
            else if (comboBox1.Text == "Zor")
            {
                hız = 20;
                AnaMenu.Visible = false;
                Game = true;
                this.Focus();
                HerSeyiSıfırla();
                if (oyunThread.ThreadState == ThreadState.Unstarted) oyunThread.Start();
                if (oyunThread.ThreadState == ThreadState.Suspended) oyunThread.Resume();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AnaMenu.Visible = true;
            Menu.Visible = false;
            oyunThread.Suspend();
            Game = false;
        }

        private void btnCıkıs_Click(object sender, EventArgs e)
        {
            if (oyunThread.ThreadState == ThreadState.Suspended) oyunThread.Resume();
            oyunThread.Abort();

            Application.Exit();
        }

      
        
       private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Kolay") hız = 100;
            else hız = 10;
        }
        int gecenzaman;
        int oyunsuresi;
        private void timer1_Tick(object sender, EventArgs e)
        {
            oyunsuresi += 1;
            label3.Text = oyunsuresi.ToString();
            label6.Text = skor.ToString();
            gecenzaman += 1;
        }

        private void AnaMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        public void CalcTable()
        {
            try { Invoke(new Action(() => label1.Text = "Skor: " + skor)); }
            catch { }

            Random rnd = new Random();
            Bitmap bitmap = new Bitmap(500, 500);

            //Kuyruk Grubu
            if (tales.Count != 1)
            {
                for (int i = 1; i < tales.Count; i++)
                {
                    if (tales[i].x == X && tales[i].y == Y)
                    {
                        MenuGoster(1);
                        

                    }
                }
            }

                //Yemeği alma
                if (X == food.x && Y == food.y)
            {
                if (gecenzaman <= 100) {
                    skor += 100 / gecenzaman;
                    gecenzaman = 0;
                }
                else
                {
                    skor += 0;
                }
                
                tales.Add(new YılanOyunu.Location(food.x, food.y));
                food = new Location(-1, -1);
            }
            

            //Alan dışına çıkma kontrolü
            if (X <= 0 || Y <= 0 || X == 51 || Y == 51)
            {
                MenuGoster(1);

            }
            else
            {

                //Yılan
                for (int i = (X - 1) * 10; i < X * 10; i++)
                {
                    for (int j = (Y - 1) * 10; j < Y * 10; j++)
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }

                if (tales.Count != 1)
                {
                    for (int k = 0; k < tales.Count; k++)
                    {
                        for (int i = (tales[k].x - 1) * 10; i < tales[k].x * 10; i++)
                        {
                            for (int j = (tales[k].y - 1) * 10; j < tales[k].y * 10; j++)
                            {
                                bitmap.SetPixel(i, j, Color.Black);
                            }
                        }
                    }
                }
            }
            tales[0] = new Location(X,Y);
            for (int i = tales.Count - 1; i > 0; i--)
            {
                tales[i] = tales[i - 1];
            }


            //Yemek
            if (food.x == -1)
            {
                food = new Location(rnd.Next(1, 50), rnd.Next(1, 50));
            }
            for (int i = (food.x - 1) * 10; i < food.x * 10; i++)
            {
                for (int j = (food.y - 1) * 10; j < food.y * 10; j++)
                {
                    bitmap.SetPixel(i, j, Color.Red);
                }
            }
            
            Oyun.Image = bitmap;
        }
    }
    public class Location //Yemimiz için sınıf tanımlıyoruz
    {
        public int x, y;
        public Location(int x,int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
