using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;

namespace NURBSpline
{
    public struct Point
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x;
        public float y;

    }

    public partial class Form1 : Form
    {

        float[] knots;
        Point[] points;
        int n = 6;
        int k = 3;
        bool MousePressed = false;
        int selectedPoint;
        float[] weights;
        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            InitKnots();
            points = new Point[n];
            points[0] = new Point((float)0.1, (float)0.8);
            points[1] = new Point((float)0.3, (float)-0.1);
            points[2] = new Point((float)0.4, (float)0.5);
            points[3] = new Point((float)0.5, (float)0.0);
            points[4] = new Point((float)0.7, (float)0.6);
            points[5] = new Point((float)0.8, (float)-0.4);
            weights = new float[n];
            for (int i = 0; i < n; i++)
                weights[i] = 1;
        }


        private float B(float x, int i, int k)
        {
            if (k == 0)
            {
                if (knots[i] <= x && x < knots[i + 1])
                    return 1;
                return 0;
            }

            float a = (x - knots[i]) / (knots[i + k] - knots[i]) * B(x, i, k - 1);
            float b = (knots[i + k + 1] - x) / (knots[i + k + 1] - knots[i + 1]) * B(x, i + 1, k - 1);
            return a + b;
        }

        private Point CalcPoint(float t)
        {
            Point p = new Point(0, 0);
            for (int i = 0; i < n; i++)
            {
                p.x += B(t, i, k) * points[i].x * weights[i];
                p.y += B(t, i, k) * points[i].y * weights[i];
            }
            return p;
        }

        private void InitKnots()
        {
            int size = n + k + 1;
            knots = new float[size];
            for (int i = 0; i < size; i++)
                knots[i] = i;
        }


        private void DrawPoints()
        {
            GL.PointSize(4f);
            GL.Color4(Color.Black);
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < n; i++)
            {
                if (i == selectedPoint)
                    GL.Color3(Color.Red);
                GL.Vertex2(points[i].x, points[i].y);
                GL.Color3(Color.Black);
            }
            GL.End();
            GL.Begin(PrimitiveType.LineStrip);
            for (int i = 0; i < n; i++)
            {
                GL.Vertex2(points[i].x, points[i].y);
            }
            GL.End();
        }


        private void render()
        {
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawPoints();
            GL.Color4(Color.FromArgb(50, 3, 252, 78));
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex2(points[0].x, points[0].y);
            for (float t = knots[k] - (float)0.5; t <= knots[n] + (float)0.4; t += (float)0.1)
            {
                Point p = CalcPoint(t);
                GL.Vertex2(p.x, p.y);
            }
            GL.Vertex2(points[5].x, points[5].y);
            GL.End();
            glControl1.SwapBuffers();
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            weights[selectedPoint] = (float)numericUpDown1.Value;
            render();
        }

        private void glControl1_MouseUp_1(object sender, MouseEventArgs e)
        {
            MousePressed = !MousePressed;
        }

        private void glControl1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (!MousePressed) return;
            points[selectedPoint].x = (float)e.X / (glControl1.Width / 2) - 1;
            points[selectedPoint].y = 1 - (float)e.Y / (glControl1.Height / 2);
            render();
        }

        private void glControl1_MouseClick_1(object sender, MouseEventArgs e)
        {
            int x, y;
            for (int i = 0; i < n; i++)
            {
                x = glControl1.Width / 2 + (int)(points[i].x * (float)glControl1.Width / 2);
                y = glControl1.Height / 2 - (int)(points[i].y * (float)glControl1.Height / 2);
                if (Math.Abs(x - e.X) < 50 && Math.Abs(y - e.Y) < 10)
                {
                    selectedPoint = i;
                    numericUpDown1.Value = (decimal)weights[selectedPoint];
                    render();
                    return;
                }
            }
        }

        private void glControl1_Paint_1(object sender, PaintEventArgs e)
        {
            render();
        }

        private void glControl1_MouseDown_1(object sender, MouseEventArgs e)
        {
            int x, y;
            for (int i = 0; i < n; i++)
            {
                x = glControl1.Width / 2 + (int)(points[i].x * (float)glControl1.Width / 2);
                y = glControl1.Height / 2 - (int)(points[i].y * (float)glControl1.Height / 2);
                if (Math.Abs(x - e.X) < 50 && Math.Abs(y - e.Y) < 10)
                {
                    MousePressed = true;
                    selectedPoint = i;
                    numericUpDown1.Value = (decimal)weights[selectedPoint];
                    return;
                }
            }
        }
    }
}
