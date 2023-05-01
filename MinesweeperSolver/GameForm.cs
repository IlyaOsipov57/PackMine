using PackMine.Geometry;
using PackMine.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PackMine
{
    public partial class GameForm : Form
    {
        Thread mainLoop;
        bool hidden = false;
        bool resizing = false;
        bool started = false;
        Random R = new Random();
        internal delegate void UpdateEvent();
        internal delegate void UpdateImageEvent(Image image);
        internal static Dictionary<Keys, IntPoint> keysInUse = new Dictionary<Keys, IntPoint>(){
            {Keys.Right, new IntPoint(1,0)},
            {Keys.Down, new IntPoint(0,1)},
            {Keys.Left, new IntPoint(-1,0)},
            {Keys.Up, new IntPoint(0,-1)},
            {Keys.D, new IntPoint(1,0)},
            {Keys.S, new IntPoint(0,1)},
            {Keys.A, new IntPoint(-1,0)},
            {Keys.W, new IntPoint(0,-1)}};
        internal static List<IntPoint> directions = new List<IntPoint>() {
            new IntPoint(1,0),
            new IntPoint(0,1),
            new IntPoint(-1,0),
            new IntPoint(0,-1)};
        static bool isEditor = false;
        static int FPS = 0;

        #region Reinitable
        UAxis axis;
        USwitch escape;
        USwitch accept;
        UButton up;
        UButton down;
        internal DateTime lastTime;
        private double deltaTime;
        private void InitGameForm()
        {
            axis = new UAxis();
            escape = new USwitch();
            accept = new USwitch();
            up = new UButton();
            down = new UButton();
            lastTime = DateTime.Now;
            deltaTime = 0;
        }
        #endregion
        private void Init (bool tryLoad = true)
        {
            Init_Editor();
            if (tryLoad && !isEditor)
            {
                gameState = Loader.LoadOrCreate();
            }
            else
            {
                if (!isEditor)
                {
                    gameState = new GameState(gameState.challengeState);
                }
                else
                {
                    gameState = new GameState();
                }
            }
            InitUpdate();
            InitRooms();
            InitGameForm();
            InitDraw();
        }

        public GameForm()
        {
            Init();
            InitializeComponent();   
        }

        private void GameForm_Activated(object sender, EventArgs e)
        {
            ShowOnTab();
            //Cursor.Hide();
#if EDITOR
            return;
#endif
            //TopMost = true;
        }

        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            axis.Reset();
            HideOnTab();
            //Cursor.Show();
            //TopMost = false;
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            if(e.Modifiers.HasFlag(Keys.Alt) && key == Keys.Enter)
            {
                this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                return;
            }
            if(keysInUse.ContainsKey(key))
                axis.KeyDown(key);
            if (key == Keys.Escape)
                escape.KeyDown();
            if (key == Keys.Enter)
                accept.KeyDown();
            if (key == Keys.S || key == Keys.Down)
                down.KeyDown();
            if (key == Keys.W || key == Keys.Up)
                up.KeyDown();
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            if (keysInUse.ContainsKey(key))
                axis.KeyUp(key);
            if (key == Keys.Escape)
                escape.KeyUp();
            if (key == Keys.Enter)
                accept.KeyUp();
            if (key == Keys.S || key == Keys.Down)
                down.KeyUp();
            if (key == Keys.W || key == Keys.Up)
                up.KeyUp();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            InitForm();
            mainLoop = new Thread(Run);
            mainLoop.Start();
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainLoop.Abort();
        }
        internal void UpdateImage(Image image)
        {
            var oldImage = this.pictureBox1.Image;
            this.pictureBox1.Image = image;
            if(oldImage!=null)
            {
                oldImage.Dispose();
            }
        }
        internal Size GetImageSize()
        {
            var size = new Size(Math.Max(this.pictureBox1.Size.Width, 1),
                Math.Max(this.pictureBox1.Size.Height, 1));
            return size;
        }
        internal void Run()
        {
            var delegatedClosing = new UpdateEvent(this.Close);
            try
            {
                var delegatedUpdateImage = new UpdateImageEvent(UpdateImage);
                lastTime = DateTime.Now;
                while (true)
                {
                    Thread.Sleep(1);
                    try
                    {
                        deltaTime = (DateTime.Now - lastTime).TotalMilliseconds / 1000;
                        //if (deltaTime < 0.025)
                        //    continue;
                        if (deltaTime > 0.002)
                            if(R.Next(2) == 0)
                                FPS = Math.Min(FPS,(int)(1.0 / deltaTime));
                            else FPS = (int)(1.0 / deltaTime);
                        if(!started)
                        {
                            if (deltaTime > 0.1)
                            {
                                started = true;
                                lastTime = DateTime.Now;
                            }
                        }
                        else
                        {
                            lastTime = DateTime.Now;
                            if (!hidden)
                            {
                                GameUpdate();
                                var size = GetImageSize();
                                var image = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                                Draw(image);
                                this.Invoke(delegatedUpdateImage, image);
                            }
                            if (resizing)
                            {
                                resizing = false;
                                if(hidden)
                                {
                                    deltaTime = 0;
                                    var size = GetImageSize();
                                    var image = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                                    Draw(image);
                                    this.Invoke(delegatedUpdateImage, image);
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException e)
                    {
                        e.ToString();
                        break;
                    }
                }
            }
            catch(ThreadAbortException te)
            {
                te.ToString();
                try
                {
                    this.Invoke(delegatedClosing);
                }
                catch{}
            }
        }
        internal void InitForm()
        {
            hidden = false;
            this.WindowState = FormWindowState.Normal;
            // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            // this.Bounds = Screen.PrimaryScreen.Bounds;
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var initialSize = new IntPoint(800, 600);
            var position = (new IntPoint(screenBounds.Width, screenBounds.Height) - initialSize) / 2;
            this.Bounds = new Rectangle(position.x, position.y, initialSize.x,initialSize.y);
        }
        internal void HideOnTab()
        {
            hidden = true;
        }
        internal void ShowOnTab()
        {
            hidden = false;
        }
        internal void Init_Editor()
        {
#if EDITOR
            isEditor = true;
            this.KeyDown += GameForm_KeyDown_Editor;
            this.KeyUp += GameForm_KeyUp_Editor;
        }
        Keys? magic = null;
        private void GameForm_KeyDown_Editor(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            magic = key;
        }

        private void GameForm_KeyUp_Editor(object sender, KeyEventArgs e)
        {
            magic = null;
#endif
        }

        private int GetMenuIndex(RPoint position)
        {
            var p = position - MenuItemPositionsStart;
            if (p.x < -2 || 9 < p.x)
                return -1;
            for (int i = 0; i < 3; i++)
            {
                if (-1 < p.y && p.y < 2)
                    return i;
                p -= MenuItemPositionsStep;
            }
            return -1;
        }

        enum CursorAction
        {
            None,
            AddFlag,
            RemoveFlag
        };

        internal IntPoint CursorOnMap()
        {
            var clickPosition = this.pictureBox1.PointToClient(Cursor.Position);
            var screenSize = this.pictureBox1.Size;
            var positionFromCenter = new RPoint(clickPosition.X - screenSize.Width / 2, clickPosition.Y - screenSize.Height / 2);
            var mapPosition = (cameraPosition + positionFromCenter / zoom)/tileSize;
            var cellPosition = new IntPoint((int)Math.Floor(mapPosition.x), (int)Math.Floor(mapPosition.y));
            return cellPosition;
        }
        internal RPoint CursorOnMenu()
        {
            var clickPosition = this.pictureBox1.PointToClient(Cursor.Position);
            var screenSize = this.pictureBox1.Size;
            var positionFromCenter = new RPoint(clickPosition.X - screenSize.Width / 2, clickPosition.Y - screenSize.Height / 2);
            var mapPosition = (cameraPosition + positionFromCenter / zoom) / tileSize;
            return mapPosition;
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
            // window actually can stay deactivated when it is resized
            resizing = true;
        }
    }
}
