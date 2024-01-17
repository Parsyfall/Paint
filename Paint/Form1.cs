using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        //TODO: Implement fill color and load image
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen pencil = new Pen(Color.Black, 3f);
        Pen eraser = new Pen(Color.White, 10);
        int option;
        // TODO: Convert ints to Points and adapt program
        int x, y, sx, sy, cx, cy;
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
                        g.DrawEllipse(pencil, cx, cy, sx, sy);
                        break;
                    case 4:
                        g.DrawRectangle(pencil, cx, cy, sx, sy);
                        break;
                    case 5:
                        g.DrawLine(pencil, cx, cy, x, y);
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
            py = e.Location;

            cx = e.X;
            cy = e.Y;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                switch (option)
                {
                    case 1:
                        px = e.Location;
                        g.DrawLine(pencil, px, py);
                        py = px;
                        break;
                    case 2:
                        px = e.Location;
                        g.DrawLine(eraser, px, py);
                        py = px;
                        break;
                }
            }
            Canvas.Refresh();

            x = e.X;
            y = e.Y;
            sx = e.X - cx;
            sy = e.Y - cy;
        }

        private void eraserColor_MouseClick(object sender, MouseEventArgs e)
        {
            changeColor(eraser, eraserColor);
        }

        private void activeColor_MouseClick(object sender, MouseEventArgs e)
        {
            changeColor(pencil, activeColor);
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
            paint = false;

            sx = x - cx;
            sy = y - cy;

            switch (option)
            {
                case 3:
                    g.DrawEllipse(pencil, cx, cy, sx, sy);
                    break;
                case 4:
                    g.DrawRectangle(pencil, cx, cy, sx, sy);
                    break;
                case 5:
                    g.DrawLine(pencil, cx, cy, x, y);
                    break;
            }
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
            if(openFileDialog1.FileName != "")
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

        private void changeColor(Pen pen, Panel panel)
        {
            colorDialog.ShowDialog();
            newColor = colorDialog.Color;
            pen.Color = newColor;
            panel.BackColor = newColor;
        }
    }
}
