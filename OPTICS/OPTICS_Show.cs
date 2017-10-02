using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OPTICS.Clustering.Core;

namespace OPTICS_Clustering
{
    public partial class FormShow : Form
    {
        private Bitmap bitmap;
        private Graphics graphics;
        private List<HighDimPoint> points = new List<HighDimPoint>();
        private string buff = "";

        public FormShow()
        {
            InitializeComponent();
            bitmap = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(bitmap);

           /* var ar = new List<OPTICS_Object<HighDimPoint>>();

            using (StreamReader reader = new StreamReader("1.log"))
            {
                while (reader.EndOfStream == false)
                {
                    string str = reader.ReadLine();
                    string[] seg = str.Split(' ');
                    ar.Add(new OPTICS_Object<HighDimPoint>(
                        new HighDimPoint(Convert.ToInt32(seg[0]), Convert.ToInt32(seg[1]))));
                }
            }

            OPTICS_Runner rn = new OPTICS_Runner(50, 7);
            var a = rn.Cluster(ar);
            MessageBox.Show(a.Count.ToString());*/
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitmap, new Point(0, 0));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Clear();
        }

        private int interval = 50;
        private int radix = 50;
        private bool addingPoint = false;
        private Point curPt = new Point(0, 0);
        System.Timers.Timer timer = new System.Timers.Timer();

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            curPt = e.Location;
            addingPoint = true;

            timer.Interval = 20;
            timer.Enabled = true;
            timer.Elapsed += OnTimer;
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (addingPoint)
            {
                bool ok = false;

                while (ok == false)
                {
                    Random random = new Random();

                    int x = random.Next(curPt.X - radix, curPt.X + radix);
                    int y = random.Next(curPt.Y - radix, curPt.Y + radix);

                    if (radix > Math.Sqrt(Math.Pow(x - curPt.X, 2) + Math.Pow(y - curPt.Y, 2)))
                    {
                        ok = true;
                        AddPoint(x, y);
                    }
                }
                
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            curPt = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            addingPoint = false;
            timer.Enabled = false;
            timer.Elapsed -= OnTimer;
        }

        private int? GetInt()
        {
            int? res = null;

            try { int n = Convert.ToInt32(buff); res = n; }
            catch (Exception) { MessageBox.Show("Invalid Number!"); }
            finally { buff = ""; }

            return res;
        }

        private void FormShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'c': Clear(); break;
                case 'n':
                    {
                        int? n = GetInt();
                        if (n != null) GenerateNoise(n.Value);
                        break;
                    }
                case 'r':
                    radix = GetInt() ?? radix; break;
                case 'e':
                    radix = GetInt() ?? epsilon; break;
                case 'm':
                    radix = GetInt() ?? minPts; break;
                case '[':
                    if (radix > 0) radix--; break;
                case ']':
                    radix++; break;
                case '+':
                    if (interval > 0) interval--; break;
                case '-':
                    interval++; break;
                case '\r':
                    DoCluster();
                    break;
                case ' ':
                    ClearScreen();
                    break;
                default: buff += e.KeyChar; break;
            }
        }

        private int showingIndex = 0;
        private List<OPTICS_Object<HighDimPoint>> orderList;

        private void ShowResult(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (showingIndex < orderList.Count)
            {
                Point pt = new Point((int)orderList[showingIndex].Element[0], 
                    (int)orderList[showingIndex].Element[1]);
                graphics.FillEllipse(Brushes.Yellow, new Rectangle(pt.X, pt.Y, 3, 3));
                Invalidate();
                showingIndex++;
            }
            else
            {
                timer.Elapsed -= ShowResult;
                timer.Enabled = false;
                MessageBox.Show("end");
            }
        }

        int epsilon = 10;
        int minPts = 20;

        private void DoCluster()
        {
            using (StreamWriter writer = new StreamWriter("1.log"))
            {
                foreach (var x in points) writer.WriteLine($"{x[0]} {x[1]}");
            }

            OPTICS_Runner optics = new OPTICS_Runner(epsilon, minPts);
            orderList = optics.Cluster((from pt in points select new OPTICS_Object<HighDimPoint>(pt)).ToList());
            MessageBox.Show(orderList.Count.ToString());
            showingIndex = 0;
            timer.Elapsed += ShowResult;
            timer.Interval = 10;
            timer.Enabled = true;
        }

        private void ClearScreen()
        {
            timer = new System.Timers.Timer();
            timer.Interval = interval;
            timer.Enabled = false;
            bitmap = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));

            foreach (var pt in points)
            {
                graphics.FillEllipse(Brushes.Red, new Rectangle((int)pt[0], (int)pt[1], 3, 3));
            }

            Invalidate();
        }

        private void Clear()
        {
            points = new List<HighDimPoint>();
            timer = new System.Timers.Timer();
            timer.Interval = interval;
            timer.Enabled = false;
            bitmap = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));
            Invalidate();
        }

        private void GenerateNoise(int n)
        {
            Random rnd = new Random();

            for (int i = 0; i < n; i++)
            {
                int x = rnd.Next(0, Width - 1);
                int y = rnd.Next(0, Height - 1);

                AddPoint(x, y);
            }

            Invalidate();
        }

        private void AddPoint(int x, int y)
        {
            points.Add(new HighDimPoint(x, y));
            graphics.FillEllipse(Brushes.Red, new Rectangle(x - 1, y - 1, 3, 3));
        }
    }
}
