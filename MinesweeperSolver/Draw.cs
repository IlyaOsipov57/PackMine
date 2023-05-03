using PackMine.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackMine
{

    public partial class GameForm : Form
    {
        internal static Brush[] ColorsByTheNumbers = { Brushes.Blue, Brushes.Green, Brushes.Red, Brushes.DarkBlue, Brushes.DarkRed, Brushes.DarkCyan, Brushes.Black, Brushes.Gray };
        internal static float tileSize = 12;
        internal static Color mixedBrown = Color.FromArgb(
            (Color.SandyBrown.R + Color.SaddleBrown.R) / 2,
            (Color.SandyBrown.G + Color.SaddleBrown.G) / 2,
            (Color.SandyBrown.B + Color.SaddleBrown.B) / 2);
        internal static double fruitAnimationSpeed = 0.2;
        internal static double roomTitleFadingSpeed = 0.4;
        internal static double challengeSplashSpeed = 1.0;
        internal static RealPoint MenuItemPositionsStart = new RealPoint(-10, 3.6);
        internal static RealPoint MenuItemPositionsStep = new RealPoint(0,5);


        #region Reinitable
        double shakeFactor;
        RealPoint cameraPosition;
        double zoom;
        double leftoverTime;
        double angle;
        double fruitAnimation;
        double roomTitleFading;
        double challengeSplashFading;
        private void InitDraw()
        {
            shakeFactor = 0;
            cameraPosition = new RealPoint(tileSize * 11.5, tileSize * 11.5);
            zoom = 3.5;
            leftoverTime = -0.2;
            angle = 0;
            fruitAnimation = 0;
            roomTitleFading = 0;
            challengeSplashFading = 0;
        }
        #endregion

        private void Draw(Image image)
        {
            var g = Graphics.FromImage(image);
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Color.AliceBlue);
            RealPoint wantedSize;
            RealPoint wantedPosition = GetTarget(out wantedSize);
            var wantedZoom = Math.Min(image.Size.Width / wantedSize.X, image.Size.Height / wantedSize.Y);

            fruitAnimation += deltaTime * fruitAnimationSpeed;
            if (fruitAnimation > 1)
                fruitAnimation = 0;

            leftoverTime += deltaTime;
            while (leftoverTime > 0)
            {
                leftoverTime -= 0.025;
                var deltaZoom = wantedZoom - zoom;
                deltaZoom += Math.Sign(deltaZoom) * 0.1;
                zoom += deltaZoom * 0.04;
                if ((wantedZoom - zoom) * deltaZoom < 0)
                    zoom = wantedZoom;

                var deltaPosition = wantedPosition - cameraPosition;
                if (deltaPosition.Length > 0.5)
                    cameraPosition += deltaPosition * 0.05;
            }
            shakeFactor = shakeFactor > 0 ? shakeFactor - deltaTime : 0;

            var s = shakeFactor * shakeFactor * 10;
            var r = (float)(s * (R.NextDouble() - 0.5));
            var x = (float)(s * (R.NextDouble() - 0.5));
            var y = (float)(s * (R.NextDouble() - 0.5));
            g.RotateTransform(r, MatrixOrder.Append);
            g.ScaleTransform((float)zoom, (float)zoom, MatrixOrder.Append);
            g.TranslateTransform(x, y, MatrixOrder.Append);
            g.TranslateTransform(image.Size.Width / 2, image.Size.Height/2, MatrixOrder.Append);
            
            if(finalCountdown > 7)
            {
                DrawFinalTitle(g);
            }
    
            for (int i = 0; i < 23;i++ )
                for (int j = 0; j < 23; j++)
                {
                    var p = new IntPoint(i, j);
                    if (gameState.map.Get(p) != CellValue.Wall && p != endPosition)
                        DrawSquare(g,p);
                }

            ////
            //g.SmoothingMode = SmoothingMode.HighSpeed;
            //for (int i = -1; i < 24; i++)
            //    for (int j = -1; j < 24; j++)
            //    {
            //        var p = new ZPoint(i, j);
            //        if (gameState.map.Get(p) == CellValue.Wall)
            //            DrawSquare(g, p);
            //    }
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            ////

            DrawSquare(g, endPosition);

            if(roomFailed>0)
            {
                DrawFog(g, currentRoom, Color.DarkRed, 1 - Math.Abs(roomFailed-1));
            }
            
            if (roomSolved > 0)
            {
                DrawFog(g, currentRoom, Color.LimeGreen, 1 - Math.Abs(roomSolved - 1));
            }

            if (!godModOn)
            {
                if (IsOOB())
                {
                    DrawRoomSecretTitle(g);
                    showRoomTitle = false;
                }
            }

            if (finalCountdown <= 5)
            {
                DrawWalls(g);
            }

            if(!dead || roomRepaired)
                DrawPlayer(g, playerPosition, gameState.playerDirection, 0.8 *(1+Math.Max(0,growth)));
            
            if(menuEscape)
            {
                DrawDecoratedLine(g, "Continue", 16, tileSize * MenuItemPositionsStart, false);
                DrawDecoratedLine(g, "New game", 16, tileSize * (MenuItemPositionsStart + MenuItemPositionsStep), false);
                DrawDecoratedLine(g, "Hints: " + (flagsOn ? "On" : "Off"), 16, tileSize * (MenuItemPositionsStart + MenuItemPositionsStep * 2), false);
                DrawDecoratedLine(g, "Exit", 16, tileSize * (MenuItemPositionsStart + MenuItemPositionsStep*3), false);
                DrawPlayer(g, MenuItemPositionsStart + new RealPoint(-1,0.7) + MenuItemPositionsStep *menuIndex, 0, 1.2, true);
            }


            if (showChallengeSplash)
            {
                if(finalCountdown > 0)
                {
                    g.ResetTransform();
                    g.ScaleTransform((float)zoom*3, (float)zoom*3, MatrixOrder.Append);
                    g.TranslateTransform(image.Size.Width / 2, image.Size.Height / 2, MatrixOrder.Append);
                }
                DrawChallengeText(g);
            }
            else
            {
                if (!godModOn)
                {
                    if (!IsOOB())
                    {
                        if (showRoomTitle)
                        {
                            DrawRoomTitle(g);
                        }
                    }
                }
            }

            // DrawFps(g);

            g.Dispose();
        }

        private void DrawFinalTitle(Graphics g)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            using (Font font = new System.Drawing.Font("Arial", 8))
            {
                g.DrawString("Mekagem 2018", font, Brushes.Black, (PointF)(new RealPoint(0, 0)), sf);
            }
        }

        private void DrawFps(Graphics g)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            using (Font font = new System.Drawing.Font("Arial", 8))
            {
                g.DrawString("FPS: " + FPS, font, Brushes.Black, (PointF)(new RealPoint(0, 0)), sf);
            }
        }

        private void DrawRoomTitle(Graphics g)
        {
            var roomName = Puzzle.Room.GetName(currentRoom);

            var k = 9;
            roomTitleFading += deltaTime * roomTitleFadingSpeed * k / 12;

            if (!godModOn && roomName != "")
            {
                var pos = roomTitleFading * 3 / 2;
                if (pos > 1)
                    pos -= 0.5;
                else if (pos > 0.5)
                    pos = 0.5;

                pos -= 0.5;
                var position = tileSize * ((RealPoint)currentRoom * 6 + new RealPoint(2.5 + pos * 100, 4));

                DrawDecoratedLine(g, roomName, k, position, true);
            }
            if (roomTitleFading > 1)
            {
                showRoomTitle = false;
                roomTitleFading = 0;
            }
        }
        private void DrawChallengeText(Graphics g)
        {
            challengeSplashFading += deltaTime * challengeSplashSpeed;
            if (challengeSplashFading >= 6)
            {
                showChallengeSplash = false;
                challengeSplashFading = 0;
            }
            var c = challengeSplashFading < 1 ? challengeSplashFading - 1 :
                challengeSplashFading < 5 ? 0 : challengeSplashFading - 5;
            c = c * c * c - 0.1 * c;
            c *= 40.0;
            var position = tileSize * (new RealPoint(0, c)) + cameraPosition;
            DrawBlueDecoratedLine(g, splashText, 16, position, true);
            DrawBlueDecoratedLine(g, "Secret challenges completed:\r\n" + gameState.challengeState.Total + " out of 4", 10, position + new RealPoint(0, 20), true);
        }
        private class Challenge
        {
            public String Title;
            public String Description;
            public IntPoint Room;
            public bool Done;
        }
        private void DrawRoomSecretTitle(Graphics g)
        {
            var challenges = new Challenge[]{
                new Challenge(){
                    Title = "Outside of the box",
                    Description = "Get out of bounds",
                    Room = new IntPoint(0,0),
                    Done = gameState.challengeState.challengeDoneOOB
                },
                new Challenge(){
                    Title = "Well done",
                    Description = "Solve it all\r\nand eat the cherry",
                    Room = new IntPoint(3,0),
                    Done = gameState.challengeState.challengeDoneComplete
                },
                new Challenge(){
                    Title = "Lockdown",
                    Description = "Isolate yourself\r\nfrom the cherry",
                    Room = new IntPoint(3,3),
                    Done = gameState.challengeState.challengeDoneLost
                },
                new Challenge(){
                    Title = "Overcomplication",
                    Description = "Do not enter\r\nthis room",
                    Room = new IntPoint(0,3),
                    Done = gameState.challengeState.challengeDoneExpert
                },
            };

            foreach(var challenge in challenges)
            {
                if(currentRoom == challenge.Room)
                {
                    var roomName = challenge.Title + ":\r\n\r\n\r\n\r\n\r\n\r\n" + (challenge.Done ? "DONE" : "NOT DONE");
                    var k = 6;
                    var position = tileSize * ((RealPoint)challenge.Room * 6 + new RealPoint(2.5, 2.5));
                    DrawFog(g, challenge.Room, challenge.Done ? Color.LimeGreen : Color.DarkGray, 0.9);
                    DrawBlackDecoratedLine(g, roomName, k, position, true);
                    DrawBlackDecoratedLine(g, challenge.Description, k, position, true);
                }
            }
        }

        private void DrawDecoratedLine(Graphics g, string text, int fontSize, RealPoint position, bool centered)
        {
            using (Font font = new System.Drawing.Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                StringFormat sf = new StringFormat();
                if (centered)
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                }

                var path = new GraphicsPath();

                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, (PointF)(position - cameraPosition), sf);
                g.FillPath(Brushes.Orange, path);
                using (var p = new Pen(Color.Black))
                {
                    p.Width = 0.2f;
                    p.LineJoin = LineJoin.Round;

                    g.DrawPath(p, path);
                }
            }
        }
        private void DrawBlueDecoratedLine(Graphics g, string text, int fontSize, RealPoint position, bool centered)
        {
            using (Font font = new System.Drawing.Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                StringFormat sf = new StringFormat();
                if (centered)
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                }

                var path = new GraphicsPath();

                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, (PointF)(position - cameraPosition), sf);
                g.FillPath(Brushes.LightGreen, path);
                using (var p = new Pen(Color.Black))
                {
                    p.Width = 0.5f;
                    p.LineJoin = LineJoin.Round;

                    g.DrawPath(p, path);
                }
            }
        }
        private void DrawBlackDecoratedLine(Graphics g, string text, int fontSize, RealPoint position, bool centered)
        {
            using (Font font = new System.Drawing.Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                StringFormat sf = new StringFormat();
                if (centered)
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                }

                var path = new GraphicsPath();

                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, (PointF)(position - cameraPosition), sf);
                g.FillPath(Brushes.Black, path);
            }
        }

        internal void DrawFog(Graphics g, IntPoint room, Color baseColor, double thickness)
        {
            var color = Color.FromArgb((int)(255*thickness),baseColor);
            using (var b = new SolidBrush(color))
            {
                var of = - tileSize / 32;
                var roomSize = tileSize * 5 - of*2;
                var positionF = (PointF)(tileSize * 6 * (RealPoint)room - cameraPosition + new RealPoint(of,of));
                g.FillRectangle(b, positionF.X, positionF.Y,roomSize,roomSize);
            }
        }
        internal void DrawPlayer(Graphics g, RealPoint coordinates, int direction, double sizeMultiplier, bool inMenu = false)
        {
            using (var p = new Pen(Color.Black))
            {
                p.Width = (float)(0.5 * sizeMultiplier);
                p.LineJoin = LineJoin.Round;
                var position = tileSize * (RealPoint)coordinates - cameraPosition;
                var size = sizeMultiplier * tileSize;
                var positionF = (PointF)(position - size * new RealPoint(0.5, 0.5));
                var halfPeriod = 0.1;
                if (glassesOn && !acting && !inMenu)
                {
                    g.FillEllipse(Brushes.Orange, positionF.X, positionF.Y, (float)size, (float)size);
                    g.DrawEllipse(p, positionF.X, positionF.Y, (float)size, (float)size);

                    var leftF =  (PointF)(position - size * new RealPoint(0.35, 0.15));
                    var rightF =  (PointF)(position - size * new RealPoint(-0.05, 0.15));
                    var fr = (float) size*0.3f;
                    g.FillEllipse(Brushes.Black,leftF.X,leftF.Y,fr,fr*0.75f);
                    g.FillEllipse(Brushes.Black, rightF.X, rightF.Y, fr, fr * 0.75f);

                    g.FillRectangle(Brushes.Black, leftF.X, leftF.Y - fr * 0.025f, fr, fr * 0.4f);
                    g.FillRectangle(Brushes.Black, rightF.X, rightF.Y - fr * 0.025f, fr, fr * 0.4f);
                    p.Width = fr/4;

                    var giantF = (PointF)(position - size*new RealPoint(2,0.12));
                    g.DrawArc(p, giantF.X, giantF.Y, (float)size * 4, (float)size * 4, 255, 30);

                    p.Width = fr / 8;
                    g.DrawArc(p, leftF.X, leftF.Y, fr * 7 / 3, fr * 1.5f, 40, 100);
                    return;
                }
                if (!menuEscape)
                {
                    angle += deltaTime;
                }
                if (angle > halfPeriod)
                    angle -= halfPeriod * 2;
                if (!acting)
                {
                    angle = 0;
                }
                var a = 15 - 15 * (float)Math.Abs(angle / halfPeriod);
                if(inMenu)
                {
                    a = 15;
                }
                g.FillPie(Brushes.Orange, positionF.X, positionF.Y, (float)size, (float)size, direction * 90 + a, 360 - 2 * a);

                g.DrawPie(p, positionF.X, positionF.Y, (float)size, (float)size, direction * 90 + a, 360 - 2 * a);
            }
        }

        internal void DrawWalls (Graphics g)
        {
            var fr = 0.125 / tileSize;
            var lt = new RealPoint(-fr, -fr);
            var rt = new RealPoint(fr, -fr);
            var lb = new RealPoint(-fr, fr);
            var rb = new RealPoint(fr, fr);

            var outerBorder = new RealPoint[]{
                new RealPoint(0,16) + lb,
                new RealPoint(0,12) + lt,
                new RealPoint(2,12) + lt,
                new RealPoint(2,11) + lb,
                new RealPoint(0,11) + lb,
                new RealPoint(0,6) + lt,
                new RealPoint(2,6) + lt,
                new RealPoint(2,5) + lb,
                new RealPoint(0,5) + lb,
                new RealPoint(0,0) + lt,
                new RealPoint(5,0) + rt,
                new RealPoint(5,2) + rt,
                new RealPoint(6,2) + lt,
                new RealPoint(6,0) + lt,
                new RealPoint(11,0) + rt,
                new RealPoint(11,2) + rt,
                new RealPoint(12,2) + lt,
                new RealPoint(12,0) + lt,
                new RealPoint(17,0) + rt,
                new RealPoint(17,2) + rt,
                new RealPoint(18,2) + lt,
                new RealPoint(18,0) + lt,
                new RealPoint(23,0) + rt,
                new RealPoint(23,5) + rb,
                new RealPoint(21,5) + rb,
                new RealPoint(21,6) + rt,
                new RealPoint(23,6) + rt,
                new RealPoint(23,11) + rb,
                new RealPoint(21,11) + rb,
                new RealPoint(21,12) + rt,
                new RealPoint(23,12) + rt,
                new RealPoint(23,17) + rb,
                new RealPoint(21,17) + rb,
                new RealPoint(21,18) + rt,
                new RealPoint(23,18) + rt,
                new RealPoint(23,23) + rb,
                new RealPoint(18,23) + lb,
                new RealPoint(18,21) + lb,
                new RealPoint(17,21) + rb,
                new RealPoint(17,23) + rb,
                new RealPoint(12,23) + lb,
                new RealPoint(12,21) + lb,
                new RealPoint(11,21) + rb,
                new RealPoint(11,23) + rb,
                new RealPoint(6,23) + lb,
                new RealPoint(6,21) + lb,
                new RealPoint(5,21) + rb,
                new RealPoint(5,23) + rb,
                new RealPoint(0,23) + lb,
                new RealPoint(0,18) + lt,
                new RealPoint(2,18) + lt,
                new RealPoint(2,17) + lb,
                new RealPoint(0,17) + lb,
                new RealPoint(0,17) + lt,
            };
            var innerBorder = new RealPoint[]{
                new RealPoint(5,5) + rb,
                new RealPoint(5,3) + rb,
                new RealPoint(6,3) + lb,
                new RealPoint(6,5) + lb,
                new RealPoint(8,5) + lb,
                new RealPoint(8,6) + lt,
                new RealPoint(6,6) + lt,
                new RealPoint(6,8) + lt,
                new RealPoint(5,8) + rt,
                new RealPoint(5,6) + rt,
                new RealPoint(3,6) + rt,
                new RealPoint(3,5) + rb,
            };
            var outerOuterBorder =  new RealPoint[]{
                new RealPoint(-1,-1) + rb,
                new RealPoint(24,-1) + lb,
                new RealPoint(24,24) + lt,
                new RealPoint(-1,24) + rt,
            };

            g.DrawPolygon(Pens.LightGray, outerOuterBorder.Select(p => (PointF)(p * tileSize - cameraPosition)).ToArray());
            g.DrawLine(Pens.Gray, (PointF)(outerBorder[0] * tileSize - cameraPosition), (PointF)(outerBorder[outerBorder.Length -1] * tileSize - cameraPosition));
            
            g.DrawLines(Pens.Black, outerBorder.Select(p => (PointF)(p * tileSize - cameraPosition)).ToArray());

            for(int i = 0; i<3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var s = new RealPoint(6 * i, 6 * j);
                    g.DrawPolygon(Pens.Black, innerBorder.Select(p => (PointF)((p + s) * tileSize - cameraPosition)).ToArray());
                }
            }
        }
        internal void DrawSquare (Graphics g, IntPoint coordinates)
        {
            var value = gameState.map.Get(coordinates);

            var position = tileSize * (RealPoint)coordinates - cameraPosition;

            if(finalCountdown > 2)
            {
                position += new RealPoint(R.NextDouble() * finalCountdown, R.NextDouble() * finalCountdown);
                if(finalCountdown > 5)
                {
                    var explosion = Math.Pow(2, finalCountdown * finalCountdown) / Math.Pow(2,25);
                    position *=explosion;

                    position += new RealPoint(
                        R.NextDouble() * finalCountdown * explosion * (position.X > 0 ? 1 : -1),
                        R.NextDouble() * finalCountdown * explosion * (position.Y > 0 ? 1 : -1));
                    
                    if (finalCountdown > 6)
                        return;
                }
            }

            var center = position + new RealPoint(tileSize / 2, tileSize / 2);


            var topLeft = (PointF)(position);
            var topRight = (PointF)(position + new RealPoint(tileSize, 0));
            var bottomLeft = (PointF)(position + new RealPoint(0, tileSize));
            var bottomRight = (PointF)(position + new RealPoint(tileSize, tileSize));


            if (value == CellValue.Wall)
            {
                return;
            }


            var positionF = (PointF)position;
            if (coordinates.X % 6 == 0 && coordinates.Y % 6 == 0)
            {
                g.FillRectangle(Brushes.Gray, positionF.X, positionF.Y, tileSize*5, tileSize*5);
            }
            if (coordinates.X % 6 == 5)
            {
                var of = tileSize / 32;
                g.FillRectangle(Brushes.Gray, positionF.X - of, positionF.Y, tileSize + 2 * of, tileSize);
            }
            if(coordinates.Y % 6 == 5)
            {
                var of = tileSize / 32;
                g.FillRectangle(Brushes.Gray, positionF.X, positionF.Y - of, tileSize, tileSize + 2 * of);
            }

            positionF = (PointF)(position + new RealPoint(tileSize / 32, tileSize / 32));
            if (value == CellValue.Door)
            {
                using (var b = new SolidBrush(mixedBrown))
                {
                    g.FillRectangle(b, positionF.X + tileSize / 32, positionF.Y + tileSize / 32, tileSize - tileSize / 8, tileSize - tileSize / 8);
                }
            }
            else
            {
                if (value == CellValue.Open || value == CellValue.Flag)
                {
                    g.FillRectangle(Brushes.LightGray, positionF.X + tileSize / 32, positionF.Y + tileSize / 32, tileSize - tileSize / 8, tileSize - tileSize / 8);
                }
                else
                {
                    g.FillRectangle(Brushes.LightGray, positionF.X, positionF.Y, tileSize - tileSize / 16, tileSize - tileSize / 16);
                }
            }
            

            if(coordinates == endPosition && growth < 3)
            {
                var cycle = (float)Math.Abs(fruitAnimation *2 -1);
                var col1 = Color.FromArgb((int)(100+ 100*cycle), Color.LightPink);
                var col2 = Color.FromArgb((int)(200 - 100*cycle), Color.LightSalmon);
                var col3 = Color.FromArgb((int)(100 + 100*Math.Abs(cycle - 0.5)), Color.LightCoral);
                var p1 = new Pen(col1);
                var p2 = new Pen(col2);
                var p3 = new Pen(col3);
                var fr1 = tileSize * 0.75f;
                var fr2 = tileSize;
                var fr3 = tileSize *0.875f;
                var c1F = (PointF)(center - new RealPoint(fr1,fr1));
                var c2F = (PointF)(center - new RealPoint(fr2,fr2));
                var c3F = (PointF)(center - new RealPoint(fr3,fr3));
                var rot = (float)fruitAnimation;
                g.DrawArc(p1, c1F.X, c1F.Y, fr1 * 2, fr1 * 2, rot * 360, 40);
                g.DrawArc(p1, c2F.X, c2F.Y, fr2 * 2, fr2 * 2, 120 + rot * 360, 40);
                g.DrawArc(p2, c2F.X, c2F.Y, fr2 * 2, fr2 * 2, 120 - rot * 360, 60);
                g.DrawArc(p2, c3F.X, c3F.Y, fr3 * 2, fr3 * 2, -rot * 360, 60);
                g.DrawArc(p3, c3F.X, c3F.Y, fr3 * 2, fr3 * 2, 240 + rot * 360, 30);
                g.DrawArc(p3, c1F.X, c1F.Y, fr1 * 2, fr1 * 2, 240 - rot * 360, 30);
            }

            if (value == CellValue.Open && coordinates == endPosition)
            {
                using (var p = new Pen(Color.Black))
                {
                    p.Width = 0.5f;
                    p.LineJoin = LineJoin.Round;

                    var fr = 5 * tileSize / 16;

                    var c1F = (PointF)(center - new RealPoint(fr/2, fr));
                    g.DrawArc(p, c1F.X, c1F.Y, fr * 3, fr * 3, 180, 80);

                    var c2F = (PointF)(center - new RealPoint(-fr / 2, 3*fr/2));
                    g.DrawArc(p, c2F.X, c2F.Y, fr * 2.5f, fr * 2.5f, 160, 57);

                    fr = 5f * tileSize / 16;
                    var posF = (PointF)(center - new RealPoint(fr, fr/8));
                    g.FillEllipse(Brushes.Red, posF.X, posF.Y, fr, fr);
                    g.DrawEllipse(p, posF.X, posF.Y, fr, fr);
                    posF = (PointF)(center - new RealPoint(0, 0));
                    g.FillEllipse(Brushes.Red, posF.X, posF.Y, fr, fr);
                    g.DrawEllipse(p, posF.X, posF.Y, fr, fr);
                }

                return;
            }

            if (value == CellValue.Open || value == CellValue.Door || value == CellValue.Flag)
            {
                var innerSize1 = tileSize / 32;
                var innerSize2 = tileSize / 8;
                var topLeft1 = (PointF)(position + new RealPoint(innerSize1, innerSize1));
                var topRight1 = (PointF)(position + new RealPoint(tileSize - innerSize1, innerSize1));
                var bottomLeft1 = (PointF)(position + new RealPoint(innerSize1, tileSize - innerSize1));
                var bottomRight1 = (PointF)(position + new RealPoint(tileSize - innerSize1, tileSize - innerSize1));
                var topLeft2 = (PointF)(position + new RealPoint(innerSize2, innerSize2));
                var topRight2 = (PointF)(position + new RealPoint(tileSize - innerSize2, innerSize2));
                var bottomLeft2 = (PointF)(position + new RealPoint(innerSize2, tileSize - innerSize2));
                var bottomRight2 = (PointF)(position + new RealPoint(tileSize - innerSize2, tileSize - innerSize2));

                if (value != CellValue.Door)
                {
                    g.FillPolygon(Brushes.White, new PointF[] { bottomLeft1, topLeft1, topRight1, topRight2, topLeft2, bottomLeft2 });
                    g.FillPolygon(Brushes.Gray, new PointF[] { bottomLeft1, bottomRight1, topRight1, topRight2, bottomRight2, bottomLeft2 });
                    if (value == CellValue.Flag && flagsOn)
                    {
                        var flagLeft = (PointF)(position + new RealPoint(tileSize * 0.25, tileSize * 0.375));
                        var flagTop = (PointF)(position + new RealPoint(tileSize * 0.625, tileSize * 0.25));
                        var flagBot = (PointF)(position + new RealPoint(tileSize * 0.625, tileSize * 0.5));
                        var poleTopLeft = (PointF)(position + new RealPoint(tileSize * 18f / 32, tileSize * 0.375));
                        var poleTopRight = (PointF)(position + new RealPoint(tileSize * 20f / 32, tileSize * 0.25));
                        var poleBottomLeft = (PointF)(position + new RealPoint(tileSize * 18f / 32, tileSize * 0.75));
                        var poleBottomRight = (PointF)(position + new RealPoint(tileSize * 20f / 32, tileSize * 0.75));


                        g.FillPolygon(Brushes.Black, new PointF[] { poleTopLeft, poleTopRight, poleBottomRight, poleBottomLeft});
                        g.FillPolygon(Brushes.Red, new PointF[] { flagLeft, flagTop, flagBot });
                    }
                }
                else
                {
                    g.FillPolygon(Brushes.SandyBrown, new PointF[] { bottomLeft1, topLeft1, topRight1, topRight2, topLeft2, bottomLeft2 });
                    g.FillPolygon(Brushes.SaddleBrown, new PointF[] { bottomLeft1, bottomRight1, topRight1, topRight2, bottomRight2, bottomLeft2 });

                    var centerF = (PointF)center;
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    using (Font font = new System.Drawing.Font("Arial", 6))
                    {
                        if (coordinates.Y % 6 == 5)
                        {
                            sf.FormatFlags = StringFormatFlags.DirectionVertical;
                            g.DrawString("§", font, Brushes.Black, centerF.X + 0.4f, centerF.Y, sf);
                        }
                        else
                        {
                            g.DrawString("§", font, Brushes.Black, centerF.X, centerF.Y, sf);
                        }
                    }
                }
            }

            if (CellValue.IsNumber(value))
            {
                var centerF = (PointF)center;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                using (Font font = new System.Drawing.Font("Arial", 8, FontStyle.Bold))
                {
                    g.DrawString(value + "", font, ColorsByTheNumbers[value - 1], centerF.X, centerF.Y + 1, sf);
                }
            }
            if (value == CellValue.Mine)
            {
                var fr = 4.5f * tileSize/16;
                var posF = (PointF)(center - new RealPoint(fr,fr));
                g.FillEllipse(Brushes.Black, posF.X, posF.Y, fr * 2, fr * 2);
                fr = 6f * tileSize/16;
                var topF = (PointF)(center + new RealPoint(0, -fr));
                var bottomF = (PointF)(center + new RealPoint(0, fr));
                var leftF = (PointF)(center + new RealPoint(-fr, 0));
                var rightF = (PointF)(center + new RealPoint(fr, 0));
                fr = 4f * tileSize / 16;
                var topleftF = (PointF)(center + new RealPoint(-fr, -fr));
                var bottomleftF = (PointF)(center + new RealPoint(-fr, fr));
                var bottomrightF = (PointF)(center + new RealPoint(fr, fr));
                var toprightF = (PointF)(center + new RealPoint(fr, -fr));
                g.DrawLine(Pens.Black, topF, bottomF);
                g.DrawLine(Pens.Black, leftF, rightF);
                g.DrawLine(Pens.Black, topleftF, bottomrightF);
                g.DrawLine(Pens.Black, bottomleftF, toprightF);
                fr = 2f * tileSize / 16;
                var glareF =  (PointF)(center + new RealPoint(-fr, -fr));
                g.FillRectangle(Brushes.White, glareF.X, glareF.Y, fr, fr);
            }
        }
        internal RealPoint GetTarget (out RealPoint size)
        {
            if(menuEscape)
            {
                size = new RealPoint(tileSize * 40, tileSize *30);
                return tileSize * new RealPoint(7.5, 11.5);
            }
            var s = tileSize*10 *Math.Min(3,(1+Math.Max(0,growth)));
            size = new RealPoint(s, s);
            if (godModOn && growth > 4)
                return tileSize * new RealPoint(11.5, 11.5);
            return tileSize * (6 * (RealPoint)currentRoom + new RealPoint(2.5, 2.5));
        }
        internal bool IsOOB ()
        {
            return gameState.map.Get(playerTarget) == CellValue.Wall;
        }
    }
}
