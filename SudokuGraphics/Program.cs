using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SudokuGraphics
{
    public class Sudoku : Form
    {
        int[,] gameBoard = new int[9, 9];
        int[,] defaults = new int[9, 9];
        bool gameComplete;
        int selX, selY, selCX, selCY;
        int secs, mins, hours, count;
        Form gameOverWindow;
        Timer time;
        bool timerStarted;

        string fname;

        public Sudoku()
        {
            //TIMER STUFF
            time = new Timer();
            time.Interval = 1000;
            time.Tick += new EventHandler(t_Tick);
            count = 0;
            hours = 0;
            mins = 0;
            secs = 0;

            //FORM STUFF
            this.Size = new Size(487, 520);
            this.Text = "Sudoku";
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Sudoku_Paint);
            this.MouseDown += new MouseEventHandler(Sudoku_MouseClick);
            this.KeyDown += new KeyEventHandler(Sudoku_KeyDown);
            this.KeyPress += new KeyPressEventHandler(Sudoku_KeyPress);
            this.FormClosed += new FormClosedEventHandler(Sudoku_FormClosed);

            //GAME STUFF
            timerStarted = false;
            gameComplete = false;
            selX = -1;
            selY = -1;

            //OPEN FILE DIALOG
            fname = "";
            System.Threading.Thread t = new System.Threading.Thread(openFile);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            
            while(t.ThreadState == System.Threading.ThreadState.Running){}

            int index = fname.LastIndexOf("\\");
            if(index >= 0)
            {
                fname = fname.Substring(index + 1);
                this.Text += " - " + fname;
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            count++;
            mins = count / 60;
            secs = count % 60;
            hours = mins / 60;
            mins = mins % 60;
            
            Invalidate();
        }

        void Sudoku_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            { 
            case Keys.Up:
                if(selY > 0)
                    selY--;
                break;
            case Keys.Down:
                if(selY < 8)  
                    selY++;
                break;
            case Keys.Left:
                if(selX > 0)
                    selX--;
                break;
            case Keys.Right:
                if(selX < 8) 
                    selX++;
                break;
            }
            Invalidate();
        }

        void Sudoku_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        void Sudoku_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!timerStarted)
            {
                time.Start();
                timerStarted = true;
            }
            if(gameComplete && e.KeyChar == 'q')
            {
                Application.Exit();
            }
            else if(selX >= 0 && selY >= 0 && selX < 9 && selY < 9)
            {
                if(e.KeyChar >= '0' && e.KeyChar <= '9')
                    makeMove(selX, selY, int.Parse("" + e.KeyChar));
            }
            Invalidate();
        }

        void openFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".txt";
            ofd.InitialDirectory = "/puzzles";
            DialogResult result = ofd.ShowDialog();
            string filename = "";
            if(result == DialogResult.OK)
            {
                filename = ofd.FileName;
                fname = filename;
                loadFile(filename);
                Invalidate();
            }
            else
            {
                Application.Exit();
            }
        }

        void Sudoku_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.X >= 10 && e.Y >= 10 && e.X <= 460 && e.Y <= 460)
            {
                selX = ((e.X - 10)/50) % 9;
                selY = ((e.Y - 10)/50) % 9;
            }
            Invalidate();
        }

        void Sudoku_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if(!gameComplete)
            {
                g.FillRectangle(Brushes.White, 10, 10, 450, 450);
                if(selX >= 0 && selY >= 0 && selX < 9 && selY < 9)
                {
                    selCX = 10 + 50 * selX;
                    selCY = 10 + 50 * selY;
                    g.FillRectangle(Brushes.LightGray, selCX, selCY, 50, 50);
                }

                for(int i = 0; i <= 9; i++)
                {
                    if(i == 3 || i == 6)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 3), 10 + i * 50, 10, 10 + i * 50, 460);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Brushes.Black), 10 + i * 50, 10, 10 + i * 50, 460);
                    }
                }

                for(int j = 0; j <= 9; j++)
                {
                    if(j == 3 || j == 6)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 3), 10, 10 + j * 50, 460, 10 + j * 50);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Brushes.Black), 10, 10 + j * 50, 460, 10 + j * 50);
                    }
                }

                for(int i = 0; i < 9; i++)
                {
                    for(int j = 0; j < 9; j++)
                    {
                        if(gameBoard[i, j] != 0)
                        {
                            StringFormat sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            Brush b = Brushes.Black;
                            if(defaults[i, j] > 0)
                                b = Brushes.DarkRed;
                            g.DrawString("" + gameBoard[i, j], new Font("Arial", 20), b, new RectangleF(10 + i * 50, 10 + j * 50, 50, 50), sf);
                        }
                    }
                }

                StringFormat sf2 = new StringFormat();
                sf2.Alignment = StringAlignment.Far;
                sf2.LineAlignment = StringAlignment.Center;
                string time = string.Format("Time: {0:00}:{1:00}:{2:00}", hours, mins, secs);
                g.DrawString(time, new Font("Arial", 10), Brushes.Black, new Rectangle(10, 460, 450, 20), sf2);
            }
            else
            {
                time.Stop();
                this.Hide();
                gameOverWindow = new GameOverWindow(hours, mins, secs);
                gameOverWindow.Show();
            }

        }

        static void Main()
        {
            Application.Run(new Sudoku());
        }

        public void loadFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            char[] c;
            int val;
            for(int i = 0; i < 9; i++)
            {
                c = lines[i].ToCharArray();
                for(int j = 0; j < 9; j++)
                {
                    val = int.Parse("" + c[j]);
                    if(val > 0)
                    {
                        addDefault(j + 1, i + 1, val);
                    }
                }
            }
        }

        public bool checkLegalMove(int x, int y)
        {
            if(x > 8 || y > 8 || x < 0 || y < 0 || defaults[x, y] > 0)
                return false;
            return true;
        }

        public bool checkRows()
        {
            for(int i = 0; i < 9; i++)
            {
                bool[] test = new bool[9];
                for(int j = 0; j < 9; j++)
                {
                    if(gameBoard[i, j] != 0)
                        test[gameBoard[i, j] - 1] = true;
                    else return false;
                }

                for(int x = 0; x < 9; x++)
                {
                    if(!test[x])
                        return false;
                }
            }
            return true;
        }

        public bool checkCols()
        {
            for(int i = 0; i < 9; i++)
            {
                bool[] test = new bool[9];
                for(int j = 0; j < 9; j++)
                {
                    if(gameBoard[j, i] != 0)
                        test[gameBoard[j, i] - 1] = true;
                    else return false;
                }

                for(int x = 0; x < 9; x++)
                {
                    if(!test[x])
                        return false;
                }
            }
            return true;
        }

        public void addDefault(int x, int y, int val)
        {
            defaults[x - 1, y - 1] = val;
            gameBoard[x - 1, y - 1] = val;
        }

        public void makeMove(int x, int y, int val)
        {
            if(checkLegalMove(x, y))
                gameBoard[x, y] = val;
            if(checkRows() && checkCols())
                gameComplete = true;
        }
    }
}