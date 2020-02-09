using SharpDX.Direct2D1;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlackTeal_R6S_External_ESP
{
    public partial class MainForm : Form
    {
        private Point mouseLocation = new Point();
        public static MemoryHelper Mem = new MemoryHelper();
        private Overlay Over;
        private bool ESP = false;
        public static bool Box = true;
        public static bool Circle = true;
        public static bool Distance = false;
        public static string NametagsText = "Terrorist";
        public static bool Healthbar = true;
        public static bool bRight = false;
        public static bool Nametags = true;
        public static bool Skeleton = true;
        public static bool Snaplines = false;
        public static bool Crosshair = false;

        //Colors
        public static Color BoxColor = new Color();
        public static Color CircleColor = new Color();
        public static Color SkeletonColor = new Color();
        public static Color SnaplineColor = new Color();
        public static Color TextColor = new Color();

        public MainForm()
        {
            //Initialize GUI and Update Addresses
            InitializeComponent();
            Mem.UpdateAddresses();

            //GUI Organizer v
            if (Width < 162)
                Width = MainWindowTitle.Location.X + MainWindowTitle.Width + 12 + MinimizeButton1.Width + 2 + CloseButton1.Width;

            Panel2.Width = Width - 2;
            CloseButton1.Location = new Point(Width - 42, 10);
            MinimizeButton1.Location = new Point(Width - 76, 10);
            //GUI Organizer ^

            //Set colors
            BoxColor = Color.FromArgb(255, 255, 255);
            CircleColor = Color.FromArgb(128, 255, 0);
            SkeletonColor = Color.FromArgb(128, 255, 0);
            SnaplineColor = Color.FromArgb(0, 148, 255);
            TextColor = Color.FromArgb(255, 255, 255);

            Console.WriteLine("Original source: jizzware R6 by SexOffenderSally#0660");
            Console.WriteLine("Offsets Updated by Ebra31321");
            Console.WriteLine("Translated to C# and SharpDX Overlay by me, BlackTeaL#3222");
        }

        private void ESPButton1_Click(object sender, EventArgs e)
        {
            //ESP Manager
            if (ESP)
            {
                ESPButton1.BackColor = Color.Black;
                ESPButton1.ForeColor = Color.Teal;

                Over.Close();
                ESP = false;
            }

            else
            {
                ESPButton1.BackColor = Color.Teal;
                ESPButton1.ForeColor = Color.Black;

                Over = new Overlay();
                Over.Show();
                ESP = true;
            }
        }

        private void VariablesUpdaterTimer1_Tick(object sender, EventArgs e)
        {
            //Sync Variables with GUI
            Box = BoxCheckBox1.Checked;
            Circle = CircleCheckBox1.Checked;
            Distance = DistanceCheckBox1.Checked;
            Healthbar = HealthbarCheckBox1.Checked;
            bRight = RightCheckBox1.Checked;
            Nametags = NametagsCheckBox1.Checked;
            NametagsText = ESPTextRichTextBox1.Text;
            Skeleton = SkeletonCheckBox1.Checked;
            Snaplines = SnaplinesCheckBox1.Checked;
            Crosshair = CrosshairCheckBox1.Checked;
        }

        //Below Stuff is for animating GUI, like moving it, closing, minimizing, etc...
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Teal, ButtonBorderStyle.Solid);
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X - 1, -e.Y - 1);
        }

        private void Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void pictureBoxIcon_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X - 10, -e.Y - 13);
        }

        private void pictureBoxIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void MainWindowTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X - 27, -e.Y - 13);
        }

        private void MainWindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void CloseButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MinimizeButton1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BoxColorButton1_Click(object sender, EventArgs e)
        {
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                BoxColor = ColorDialog1.Color;
            }
        }

        private void CircleColorButton1_Click(object sender, EventArgs e)
        {
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                CircleColor = ColorDialog1.Color;
            }
        }

        private void SkeletonColorButton1_Click(object sender, EventArgs e)
        {
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                SkeletonColor = ColorDialog1.Color;
            }
        }

        private void SnaplineColorButton1_Click(object sender, EventArgs e)
        {
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                SnaplineColor = ColorDialog1.Color;
            }
        }

        private void TextColorButton1_Click(object sender, EventArgs e)
        {
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                TextColor = ColorDialog1.Color;
            }
        }

        private void ResetButton1_Click(object sender, EventArgs e)
        {
            BoxColor = Color.FromArgb(255, 255, 255);
            CircleColor = Color.FromArgb(128, 255, 0);
            SkeletonColor = Color.FromArgb(128, 255, 0);
            SnaplineColor = Color.FromArgb(0, 148, 255);
            TextColor = Color.FromArgb(255, 255, 255);
        }
    }
}
