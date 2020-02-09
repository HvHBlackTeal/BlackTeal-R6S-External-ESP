using static BlackTeal_R6S_External_ESP.Imports.User32;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using BlackTeal_R6S_External_ESP.Data;

namespace BlackTeal_R6S_External_ESP
{
    public partial class Overlay : Form
    {
        public static RECT rect;

        private int screenX = 1920;
        private int screenY = 1080;

        private WindowRenderTarget _Device;
        private HwndRenderTargetProperties _RenderProperties;
        private SolidColorBrush _SolidColorBrush;
        private Factory _Factory;

        //Colors
        private SolidColorBrush BoxColorBrush;
        private SolidColorBrush CircleColorBrush;
        private SolidColorBrush SkeletonColorBrush;
        private SolidColorBrush SnaplineColorBrush;
        private SolidColorBrush TextColorBrush;
        private SolidColorBrush HealthColorBrush1;
        private SolidColorBrush HealthColorBrush2;

        // Fonts
        private FontFactory _FontFactory;
        private const string _FontFamily = "Arial";
        private const float FontSizeSmall = 10.0f;

        private readonly IntPtr _Handle;
        private Thread _ThreadDX = null;

        private bool _Running = false;

        public Overlay()
        {
            _Handle = Handle;
            InitializeComponent();

            int initialStyle = GetWindowLong(Handle, -20);
            SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        private void LoadOverlay(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            _Factory = new Factory();
            _FontFactory = new FontFactory();
            _RenderProperties = new HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new SharpDX.Size2(Size.Width, Size.Height),
                PresentOptions = PresentOptions.None
            };

            // Initialize DirectX
            _Device = new WindowRenderTarget(_Factory, new RenderTargetProperties(new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)),
                _RenderProperties);
            _SolidColorBrush = new SolidColorBrush(_Device, new RawColor4(Color.White.R, Color.White.G, Color.White.B, Color.White.A));

            _ThreadDX = new Thread(new ParameterizedThreadStart(DirectXThread))
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };

            _Running = true;
            _ThreadDX.Start();
        }

        private void ClosedOverlay(object sender, FormClosedEventArgs e)
        {
            _Running = false;
        }

        private void DirectXThread(object sender)
        {
            MainForm.Mem = new MemoryHelper();

            HealthColorBrush1 = new SolidColorBrush(_Device, new RawColor4(0.003922f, 0.003922f, 0.003922f, 1));

            while (_Running)
            {
                //Calling Colors from MainForm
                BoxColorBrush = new SolidColorBrush(_Device, new RawColor4(MainForm.BoxColor.R / 255, MainForm.BoxColor.G / 255, MainForm.BoxColor.B / 255, 1));
                CircleColorBrush = new SolidColorBrush(_Device, new RawColor4(MainForm.CircleColor.R / 255, MainForm.CircleColor.G / 255, MainForm.CircleColor.B / 255, 1));
                SkeletonColorBrush = new SolidColorBrush(_Device, new RawColor4(MainForm.SkeletonColor.R / 255, MainForm.SkeletonColor.G / 255, MainForm.SkeletonColor.B / 255, 1));
                SnaplineColorBrush = new SolidColorBrush(_Device, new RawColor4(MainForm.SnaplineColor.R / 255, MainForm.SnaplineColor.G / 255, MainForm.SnaplineColor.B / 255, 1));
                TextColorBrush = new SolidColorBrush(_Device, new RawColor4(MainForm.TextColor.R / 255, MainForm.TextColor.G / 255, MainForm.TextColor.B / 255, 1));

                //Getting R6S Window RECT (Coordinates and Size)
                GetWindowRect(FindWindow(null, "Rainbow Six"), out rect);
                screenX = rect.Right;
                screenY = rect.Bottom;
                MainForm.Mem.displayWidth = rect.Right;
                MainForm.Mem.displayHeight = rect.Bottom;

                _Device.BeginDraw();
                _Device.Clear(new RawColor4(Color.Transparent.R, Color.Transparent.G, Color.Transparent.B, Color.Transparent.A));
                _Device.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Aliased;

                //For Distance
                long LocalPlayerBaseAddress = MainForm.Mem.GetEntity(0);
                PlayerInfo LocalPlayer = MainForm.Mem.GetAllEntityInfo(LocalPlayerBaseAddress);

                // Render our ESP here
                for (int i = 1/*0 is local player in situations and lw htunt, set to 0 if playing mp*/; i < 64; i++)
                {
                    long Entity = MainForm.Mem.GetEntity(i);

                    //Get a PlayerInfo struct with all of this entity's info
                    PlayerInfo Player = MainForm.Mem.GetAllEntityInfo(Entity);
                    Player.Health = MainForm.Mem.GetEntityHealth(MainForm.Mem.GetEntity2(i));
                    if (Player.Health > 0               //Health over 0
                        && Player.Health <= 100         //Health less than 200
                        && Player.w2sPos.z >= 0.1f      //Player is actually on the screen
                        && Player.w2sHead.z >= 0.1f     //Player is actually on the screen
                    )
                    {
                        HealthColorBrush2 = new SolidColorBrush(_Device, new RawColor4((100 - Player.Health) * 1 / 100, Player.Health * 1 / 100, 0, 1));

                        float BoxHeight = Player.w2sPos.y - Player.ScrenTop.y;
                        float BoxWidth = BoxHeight / 2.4f;

                        if (MainForm.Box)
                        {
                            //Draw player box
                            _Device.DrawRectangle(new RawRectangleF(Player.ScrenTop.x - BoxWidth / 2, Player.ScrenTop.y - BoxHeight / 12.5f,
                                Player.w2sHead.x + BoxWidth / 2, Player.w2sPos.y), BoxColorBrush);
                        }

                        if (MainForm.Circle)
                        {
                            //Draw a circle around the players head
                            _Device.DrawEllipse(new Ellipse
                            {
                                Point = new RawVector2(Player.w2sHead.x, Player.w2sHead.y),
                                RadiusX = BoxHeight / 12.5f,
                                RadiusY = BoxHeight / 12.5f
                            }, CircleColorBrush);
                        }

                        if (MainForm.Distance)
                        {
                            //Draw name
                            _Device.DrawText(Math.Round(Vector3.Get3DDistance(LocalPlayer.w2sPos, Player.w2sPos) / 1000, 2).ToString() + "m",
                                new TextFormat(_FontFactory, _FontFamily, FontSizeSmall),
                                new RawRectangleF(Player.w2sPos.x - (BoxWidth / 2), Player.w2sPos.y, screenX, Player.w2sPos.y), TextColorBrush);
                        }

                        if (MainForm.Healthbar)
                        {
                            //Draw healthbar
                            if (MainForm.bRight)
                            {
                                _Device.FillRectangle(new RawRectangleF(Player.w2sHead.x + (BoxWidth / 2) + 2, Player.ScrenTop.y - BoxHeight / 12.5f, Player.w2sHead.x + (BoxWidth / 2) + 8, Player.w2sPos.y), HealthColorBrush1);
                                _Device.FillRectangle(new RawRectangleF(Player.w2sHead.x + (BoxWidth / 2) + 4, Player.w2sPos.y - BoxHeight / 12.5f + 2 - (BoxHeight / 100 * Player.Health), Player.w2sHead.x + (BoxWidth / 2) + 6, Player.w2sPos.y - 2), HealthColorBrush2);
                            }
                            else
                            {
                                _Device.FillRectangle(new RawRectangleF(Player.w2sHead.x - (BoxWidth / 2) - 8, Player.ScrenTop.y - BoxHeight / 12.5f, Player.w2sHead.x - (BoxWidth / 2) - 2, Player.w2sPos.y), HealthColorBrush1);
                                _Device.FillRectangle(new RawRectangleF(Player.w2sHead.x - (BoxWidth / 2) - 6, Player.w2sPos.y - BoxHeight / 12.5f + 2 - (BoxHeight / 100 * Player.Health), Player.w2sHead.x - (BoxWidth / 2) - 4, Player.w2sPos.y - 2), HealthColorBrush2);
                            }
                        }

                        if (MainForm.Nametags)
                        {
                            //Draw name
                            _Device.DrawText(MainForm.NametagsText, new TextFormat(_FontFactory, _FontFamily, FontSizeSmall),
                                new RawRectangleF(Player.w2sHead.x - BoxWidth / 2, Player.ScrenTop.y, screenX, Player.ScrenTop.y - BoxHeight / 3),
                                TextColorBrush, DrawTextOptions.None);
                        }

                        if (MainForm.Skeleton)
                        {
                            //Draw bones
                            /*Head-Neck*/
                            _Device.DrawLine(new RawVector2(Player.w2sHead.x, Player.w2sHead.y), new RawVector2(Player.w2sNeck.x, Player.w2sNeck.y), SkeletonColorBrush);
                            /*Neck-Chest*/
                            _Device.DrawLine(new RawVector2(Player.w2sNeck.x, Player.w2sNeck.y), new RawVector2(Player.w2sChest.x, Player.w2sChest.y), SkeletonColorBrush);
                            /*Chest-Stomach*/
                            _Device.DrawLine(new RawVector2(Player.w2sChest.x, Player.w2sChest.y), new RawVector2(Player.w2sStomach.x, Player.w2sStomach.y), SkeletonColorBrush);
                            /*Stomach-Pelvis*/
                            _Device.DrawLine(new RawVector2(Player.w2sStomach.x, Player.w2sStomach.y), new RawVector2(Player.w2sPelvis.x, Player.w2sPelvis.y), SkeletonColorBrush);
                            /*Pevlis-Feet*/
                            _Device.DrawLine(new RawVector2(Player.w2sPelvis.x, Player.w2sPelvis.y), new RawVector2(Player.w2sPos.x, Player.w2sPos.y), SkeletonColorBrush);
                            /*Chest-RIGHT_HAND*/
                            _Device.DrawLine(new RawVector2(Player.w2sChest.x, Player.w2sChest.y), new RawVector2(Player.w2sRHand.x, Player.w2sRHand.y), SkeletonColorBrush);
                        }

                        if (MainForm.Snaplines)
                        {
                            //Draw snapline
                            if (MainForm.Crosshair)
                            {
                                _Device.DrawLine(new RawVector2(screenX / 2, screenY / 2), new RawVector2(Player.w2sPos.x, Player.w2sPos.y), SnaplineColorBrush, 1);
                            }
                            else
                            {
                                _Device.DrawLine(new RawVector2(screenX / 2, screenY), new RawVector2(Player.w2sPos.x, Player.w2sPos.y), SnaplineColorBrush, 1);
                            }
                        }
                    }
                }

                // End Render of our ESP
                _Device.EndDraw();
            }
        }

        protected override bool ShowWithoutActivation => true;

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= WS_EX_TOPMOST;
                param.ExStyle |= WS_EX_NOACTIVATE;
                return param;
            }
        }

        private const int WM_ACTIVATE = 6;
        private const int WA_INACTIVE = 0;

        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATEANDEAT = 0x0004;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
                return;
            }
            if (m.Msg == WM_ACTIVATE)
            {
                if (((int)m.WParam & 0xFFFF) != WA_INACTIVE)
                    if (m.LParam != IntPtr.Zero)
                        SetActiveWindow(m.LParam);
                    else
                        SetActiveWindow(IntPtr.Zero);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
