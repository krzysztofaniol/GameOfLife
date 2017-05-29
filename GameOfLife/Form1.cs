using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(true);
        //private Graphics graphics;
        private bool isRunning = false;
        private bool isStarted = false;
        private Thread thread;
        private Grid grid;
        private int trackValue;
        private const int xMargin = 50;
        private const int yMargin = 125;
        private BufferedGraphics bGraphics;

        public Form1()
        {
            InitializeComponent();

            trackValue = trackBar1.Value;
            
            this.MinimumSize = new Size(550, 200);
            thread = new Thread(new ThreadStart(this.Game));

            YText.KeyDown += Text_KeyDown;
            XText.KeyDown += Text_KeyDown;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                startButton.Text = "Start";
                isRunning = false;
                _manualResetEvent.Reset();
                drawButton.Enabled = true;
                
            }
            else
            {
                startButton.Text = "Stop";
                isRunning = true;
                drawButton.Enabled = false;
                if (!isStarted)
                {
                    thread.Start();
                    isStarted = true;
                }
                else
                {
                    _manualResetEvent.Set();
                }
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            createButton.Enabled = false;

            cellSizeText.Enabled = false;
            XText.Enabled = false;
            YText.Enabled = false;

            pictureBox1.Size = new Size(int.Parse(XText.Text), int.Parse(YText.Text));
            //graphics = pictureBox1.CreateGraphics();

            UpdateBufferedGraphics();
            

            grid = new Grid(int.Parse(XText.Text), int.Parse(YText.Text));

            grid.Draw(bGraphics.Graphics);
            bGraphics.Render();
            pictureBox1.Refresh();

            pictureBox1.MouseClick += PictureBox1_MouseClick;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            startButton.Enabled = true;
            drawButton.Enabled = true;
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            grid.RandomBoard();
            grid.Draw(bGraphics.Graphics);
            bGraphics.Render();
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isRunning)
            {
                grid.ClickUpdate(new Point(e.X, e.Y), bGraphics.Graphics);
                bGraphics.Render();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackValue = trackBar1.Value;
        }
  
        private void Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ChangeSize();
        }

        protected override void OnClosed(EventArgs e)
        {
            thread.Abort();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!isStarted)
            {
                pictureBox1.Size = new Size(this.Size.Width - xMargin, this.Size.Height - yMargin);
                UpdateBufferedGraphics();

                XText.Text = (this.Size.Width - xMargin).ToString();
                YText.Text = (this.Size.Height - yMargin).ToString();
            }
        }

        private void Game()
        {
            while (true)
            {
                grid.Update(bGraphics.Graphics);
                bGraphics.Render();
                Thread.Sleep(trackValue);
                _manualResetEvent.WaitOne(Timeout.Infinite);
            }
        }
        
        private void ChangeSize()
        {
            int a, b;
            if (int.TryParse(XText.Text, out a) && int.TryParse(YText.Text, out b))
            {
                this.Size = new Size(a + xMargin, b + yMargin);
            }
        }

        private void UpdateBufferedGraphics()
        {
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            bGraphics = context.Allocate(pictureBox1.CreateGraphics(), pictureBox1.DisplayRectangle);
        }

        private void textCellSize_TextChanged(object sender, EventArgs e)
        {
            int size;
            if (int.TryParse(cellSizeText.Text, out size))
            {
                if (size > 0)
                    Cell.CELLSIZE = size;
                else
                {
                    const string message = "Cell size have to be greater than 0";
                    MessageBox.Show(message);
                }
            }
        }
    }
}
