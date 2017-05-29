using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Cell
    {
        public Point point { get; private set; }
        public Rectangle rectangle { get; private set; }
        public bool isAlive { get; set; }

        public static int CELLSIZE = 9;

        public Cell(int x, int y)
        {
            point = new Point(x, y);
            rectangle = new Rectangle(x, y, CELLSIZE, CELLSIZE);

            isAlive = false;
        }

        public void Draw(System.Drawing.Graphics graphics)
        {
            if(isAlive)
                graphics.FillRectangle(new SolidBrush(Color.White), rectangle);
            else
                graphics.FillRectangle(new SolidBrush(Color.Black), rectangle);
            
        }

        public void SwitchIsAlive()
        {
            if (isAlive)
                isAlive = false;
            else
                isAlive = true;
        }
    }
       
}
