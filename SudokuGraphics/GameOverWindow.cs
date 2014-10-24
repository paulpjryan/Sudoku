using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SudokuGraphics
{
    public class GameOverWindow : Form
    {
        int hours;
        int minutes;
        int seconds;
        Button buttonQuit;
        Button buttonNG;

	    public GameOverWindow( int hrs, int mins, int secs )
	    {

            this.seconds = secs;
            this.hours = hrs;
            this.minutes = mins;

            this.Size = new Size(250, 350);
            this.Text = "Game Over.";
            
            this.buttonQuit = new Button();
            buttonQuit.Size = new Size(100, 20);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Text = "Quit";
            
            this.buttonNG = new Button();
            buttonNG.Size = new Size(100, 20);
            buttonNG.Name = "buttonNG";
            buttonNG.Text = "New Game";

            buttonQuit.Location = new Point(ClientRectangle.Width / 2 - buttonQuit.Width / 2 - buttonNG.Width / 2 - 5, ClientRectangle.Height - 40);
            buttonNG.Location = new Point( ClientRectangle.Width / 2 - buttonQuit.Width / 2 + buttonNG.Width / 2 + 5, ClientRectangle.Height - 40 );


            this.Controls.AddRange(new Control[] { buttonQuit, buttonNG });
            buttonQuit.Show();
            buttonNG.Show();


            buttonQuit.Click += new EventHandler(buttonQuit_Click);
            buttonNG.Click += new EventHandler(buttonNG_Click);
            this.FormClosed += new FormClosedEventHandler(GameOverWindow_FormClosed);
            this.Paint += new PaintEventHandler(GameOverWindow_Paint);
	    }

        void buttonNG_Click(object sender, EventArgs e)
        {
            this.Hide();
            Sudoku s = new Sudoku();
            s.Show();
        }

        void buttonQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void GameOverWindow_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            string results = string.Format("You did it!\nTime: {0:00}:{1:00}:{2:00}\nPress \"New Game\" to start again.\nPress \"Quit\" to quit.", hours, minutes, seconds);
            g.DrawString(results, new Font("Arial", 10), Brushes.Black, new RectangleF(0, 0, ClientRectangle.Width, ClientRectangle.Height), sf);

        }

        void GameOverWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}