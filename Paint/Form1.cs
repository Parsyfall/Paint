using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        //TODO: Implement fill color
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point topLeft, lastPoint;
        Pen pencil = new Pen(Color.Black, 3f);
        Pen eraser = new Pen(Color.White, 10);
        int option;
        int width, height;
        Color newColor;

        public Form1()
        {
            InitializeComponent();
            //Rectangle rectangle = Screen.PrimaryScreen.Bounds;  // For screen resolution
            bm = new Bitmap(800, 369);   // Bitmap to save the drawing
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            Canvas.Image = bm;

            // For smoother lines
            pencil.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pencil.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            eraser.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            eraser.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            // ComboBox
            comboBox1.Items.AddRange(new string[] { "1", "2", "3", "4", "5", "6", });
            comboBox1.SelectedIndex = 0;
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (paint)
            {
                switch (option)
                {
                    case 3:
                        g.DrawEllipse(pencil, lastPoint.X, lastPoint.Y, width, height);
                        break;
                    case 4:
                        g.DrawRectangle(pencil, lastPoint.X, lastPoint.Y, width, height);
                        break;
                    case 5:
                        g.DrawLine(pencil, lastPoint.X, lastPoint.Y, topLeft.X, topLeft.Y);
                        break;
                }
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            changeColor(pencil, activeColor);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (option == 1)
            {
                this.Cursor = Cursors.Cross;
            }

            paint = true;
            lastPoint = e.Location;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                switch (option)
                {
                    case 1:
                        g.DrawLine(pencil, e.Location, lastPoint);
                        lastPoint = e.Location;
                        break;
                    case 2:
                        g.DrawLine(eraser, e.Location, lastPoint);
                        lastPoint = e.Location;
                        break;
                }
            }
            Canvas.Refresh();

            topLeft = e.Location;
            width = topLeft.X - lastPoint.X;
            height = topLeft.Y - lastPoint.Y;
        }
        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
            paint = false;

            width = topLeft.X - lastPoint.X;
            height = topLeft.Y - lastPoint.Y;

            switch (option)
            {
                case 3:
                    g.DrawEllipse(pencil, lastPoint.X, lastPoint.Y, width, height);
                    break;
                case 4:
                    g.DrawRectangle(pencil, lastPoint.X, lastPoint.Y, width, height);
                    break;
                case 5:
                    g.DrawLine(pencil, lastPoint.X, lastPoint.Y, topLeft.X, topLeft.Y);
                    break;
                case 6:
                    FloodFill(bm, e.Location, bm.GetPixel(e.Location.X, e.Location.Y), activeColor.BackColor);
                    break;
            }
        }

        private void eraserColor_MouseClick(object sender, MouseEventArgs e)
        {
            changeColor(eraser, eraserColor);
        }

        private void activeColor_MouseClick(object sender, MouseEventArgs e)
        {
            changeColor(pencil, activeColor);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // TODO: Solve bluring on resize
            Bitmap b = new Bitmap(Canvas.Width, Canvas.Height);
            g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bm, 0, 0, b.Width, b.Height);
            bm = b;
            Canvas.Image = bm;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG images (*.jpg)|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK && sfd.FileName != "")
            {
                Canvas.Image.Save(sfd.FileName);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                bm = (Bitmap)Image.FromFile(openFileDialog1.FileName);
                g = Graphics.FromImage(bm);
                Canvas.Image = bm;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            Canvas.Image = bm;
            option = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pencil.Width = (comboBox1.SelectedIndex + 1) * 3f;
            eraser.Width = (comboBox1.SelectedIndex + 1) * 10f;
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            option = 1;
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            option = 2;
        }

        private void btnElipse_Click(object sender, EventArgs e)
        {
            option = 3;
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            option = 4;
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            option = 5;
        }

        private void btnPaintBucket_Click(object sender, EventArgs e)
        {
            option = 6;
        }

        private void changeColor(Pen pen, Panel panel)
        {
            colorDialog.ShowDialog();
            newColor = colorDialog.Color;
            pen.Color = newColor;
            panel.BackColor = newColor;
        }
        private void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Stack<Point> pixels = new Stack<Point>();
            targetColor = bmp.GetPixel(pt.X, pt.Y);
            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.X < bmp.Width && a.X > 0 &&
                        a.Y < bmp.Height && a.Y > 0)//make sure we stay within bounds
                {

                    if (bmp.GetPixel(a.X, a.Y) == targetColor)
                    {
                        bmp.SetPixel(a.X, a.Y, replacementColor);
                        pixels.Push(new Point(a.X - 1, a.Y));
                        pixels.Push(new Point(a.X + 1, a.Y));
                        pixels.Push(new Point(a.X, a.Y - 1));
                        pixels.Push(new Point(a.X, a.Y + 1));
                    }
                }
            }
            Canvas.Refresh(); //refresh our main picture box
            return;
        }
    }
}
