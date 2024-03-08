using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Lab4
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows, cols;

        public Form1()
        {
            InitializeComponent();
        }
        private void StartGame()
        {
            resolution = (int)edResolution.Value; 

            rows = pictureBox1.Height / resolution; 
            cols = pictureBox1.Width / resolution;

            field = new bool[cols, rows];


            Random random = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)edDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);

            timer1.Start();

        }
        private int CountNeighbours(int x, int y)
        {
            int count=0;
            for (int i=-1;i<2;i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i+cols)%cols;
                    var row = (y + j+rows)%rows;

                    var isSelfCheching=col == x && row ==y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfCheching)
                        count++;
                }
            }
            return count;
        }
        private void GenerationAfter()
        {
            graphics.Clear(Color.Black);
            

            //так как мы каждый раз по сути рисуем новое поле, нам нужна еще переменная для рисования поля в новом поколении
            var newField = new bool[cols, rows];
            
            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                {
                    var neighborhood = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighborhood == 3)
                    {
                        newField[x, y] = true;
                    }
                    else if (hasLife && (neighborhood < 2 || neighborhood > 3))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (hasLife)
                    {
                        graphics.FillRectangle(Brushes.DeepSkyBlue, x * resolution, y * resolution, resolution - 1, resolution - 1);
                    }
                    //параметры(цвет, положение по x, положение по y, размер клетки из resolution(не в реальном времени) по x,  размер клетки из resolution(не в реальном времени) по y)
                    //resolution -1, чтобы была рамка в цвет фона
                }
            field = newField;

            pictureBox1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GenerationAfter();
        }

        private void btstart_Click(object sender, EventArgs e)
        {
           StartGame(); 
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
            { return; } 
            if (e.Button==MouseButtons.Left)
            {
                var x=e.Location.X/resolution;
                var y = e.Location.Y / resolution;
                var positionOk=ValidateMousePosition(x, y);
                if (positionOk)
                {
                    field[x, y] = true;
                }
                
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var positionOk = ValidateMousePosition(x, y);
                if (positionOk)
                {
                    field[x, y] = false;
                }
            }
            
        }
        private bool ValidateMousePosition(int x, int y)
        {
            return x>=0 && y>=0 && x<cols && y<rows;
        }


        private void btStop_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled) return;
            timer1.Stop();
        }
    }
}
