using PackMine.Geometry;
using PackMine.Puzzle;
using PackMine.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackMine
{
    public partial class GameForm : Form
    {
        internal static IntPoint startPosition = new IntPoint(2, 2);
        internal static RPoint centerShift = new RPoint(0.5, 0.5);
        private static IntPoint endPosition = new IntPoint(20, 2);
        internal static double roomFadingSpeed = 2;
        internal static double speed = 2;
        internal static double growthSpeed = 5;
        private bool flagsOn;

        #region GameState
        private GameState gameState;
        #endregion

        #region Reinitable
        Dictionary<IntPoint, Room> rooms;
        bool acting;
        IntPoint playerMovement;
        bool canPrevent;
        bool dead;
        double roomFailed;
        double roomSolved;
        bool roomRepaired;
        bool roomCompleted;
        double delayed;
        bool canMove;
        bool glassesOn;
        bool godModOn;
        double growth;
        double glassesTimer;
        double finalCountdown;
        bool showRoomTitle;
        bool showChallengeSplash;
        String splashText;
        bool menuEscape;
        int menuIndex;
        bool mouseMenuClick;

        RPoint playerPosition;
        IntPoint playerTarget;
        IntPoint currentRoom;
        private void InitUpdate ()
        {
            rooms = new Dictionary<IntPoint, Room>();
            acting = false;
            playerMovement = IntPoint.Zero;
            canPrevent = true;
            dead = false;
            roomFailed = 0;
            roomSolved = 0;
            roomRepaired = false;
            roomCompleted = false;
            canMove = true;
            glassesOn = false;
            godModOn = false;
            growth = 0;
            glassesTimer = 0;
            finalCountdown = 0;
            showRoomTitle = true;
            showChallengeSplash = false;
            splashText = "";
            menuEscape = false;
            menuIndex = 0;
            mouseMenuClick = false;

            playerPosition = (RPoint)gameState.playerLastStablePosition + centerShift;
            playerTarget = new IntPoint(gameState.playerLastStablePosition);
            currentRoom = (gameState.playerLastStablePosition + directions[gameState.playerDirection]) / 6;
            currentRoom = Room.FixRoomIndex(currentRoom);
        }
        #endregion


        private void GameUpdate()
        {
            Update_Editor();
            var _key = axis.Read();

            var escapePressed = escape.Read(deltaTime);
            var acceptPressed = accept.Read(deltaTime) || mouseMenuClick;
            mouseMenuClick = false;
            var upPressed = up.Read(deltaTime);
            var downPressed = down.Read(deltaTime);

            if(escapePressed)
            {
                if (finalCountdown <= 0)
                {
                    menuEscape = !menuEscape;
                    menuIndex = 0;
                }
            }

            if(menuEscape)
            {
                if(acceptPressed)
                {
                    switch(menuIndex)
                    {
                        case 0:
                            break;
                        case 1:
                            var c = cameraPosition;
                            var p = zoom;
                            Init(false);
                            SavePlayerImmediately();
                            cameraPosition = c;
                            zoom = p;
                            break;
                        case 2:
                            flagsOn = !flagsOn;
                            return;
                        case 3:
                            mainLoop.Abort();
                            return;
                    }
                    menuEscape = false;
                    return;
                }
                if (_key.HasValue)
                {
                    if (downPressed)
                        menuIndex++;
                    if (upPressed)
                        menuIndex--;
                    menuIndex = (menuIndex + 4) % 4;
                }
                return;
            }

            if (godModOn)
            {
                growth += deltaTime;
                if (growth > 5)
                {
                    growth = 5;
                    canMove = true;
                }
                if (!glassesOn && !acting)
                    glassesTimer += deltaTime;
                else
                    glassesTimer = 0;

                if (glassesTimer > 0.5)
                    glassesOn = true;
            }

            if (roomSolved > 0)
            {
                roomSolved -= deltaTime * roomFadingSpeed;
                if (roomSolved < 1 && !roomCompleted)
                {
                    CompleteRoom(currentRoom);
                    roomCompleted = true;
                }
                if (roomSolved < 0)
                {
                    roomSolved = 0;
                    roomCompleted = false;
                    if(!godModOn)
                        canMove = true;
                }
            }

            if (dead)
            {
                if (godModOn)
                {
                    finalCountdown += deltaTime;
                    if(finalCountdown < 1)
                    {
                        showChallengeSplash = true;
                        splashText = "The End";
                    }
                    if (finalCountdown > 1)
                        shakeFactor = 0.5;
                    if (finalCountdown > 2)
                        shakeFactor = 1;
                    if (finalCountdown > 6)
                        shakeFactor = 0;
                    if (finalCountdown > 10)
                    {
                        mainLoop.Abort();
                    }
                    return;
                }
                roomFailed += deltaTime * roomFadingSpeed;
                if(roomFailed > 1 && !roomRepaired)
                {
                    RepairRoom(currentRoom);
                    playerTarget = new IntPoint(gameState.playerSavedPosition);
                    playerPosition = (RPoint)(gameState.playerSavedPosition) + centerShift;
                    gameState.playerLastStablePosition = gameState.playerSavedPosition;
                    gameState.playerDirection = gameState.playerSavedDirection;
                    if ((playerTarget == new IntPoint(14, 11) && gameState.map.Get(14, 5) == CellValue.Door && gameState.map.Get(11, 14) == CellValue.Door) ||
                        (playerTarget == new IntPoint(14, 5) && gameState.map.Get(14, 11) == CellValue.Door && gameState.map.Get(11, 2) == CellValue.Door) ||
                        ((playerTarget == new IntPoint(14, 11) || playerTarget == new IntPoint(14, 5)) && gameState.map.Get(11, 2) == CellValue.Door && gameState.map.Get(11, 14) == CellValue.Door))
                    {
                        if(!gameState.challengeState.challengeDoneLost)
                        {
                            gameState.challengeState.challengeDoneLost = true;
                            showChallengeSplash = true;
                            splashText = "You lose!";
                        }
                    }
                    acting = false;
                    roomRepaired = true;
                }
                if (roomFailed < 2)
                    return;
                roomRepaired = false;
                roomFailed = 0;
                dead = false;
            }

            if (delayed > 0)
            {
                delayed -= deltaTime;
                if(delayed <= 0)
                {
                    canMove = true;
                }
            }

            if (canMove)
            {
                if (_key.HasValue)
                {
                    var key = _key.Value;
                    if (!acting || (canPrevent && (playerMovement + keysInUse[key] == IntPoint.Zero || playerMovement + keysInUse[key] * 6 == IntPoint.Zero)))
                    {
                        playerMovement = keysInUse[key];
                        gameState.playerDirection = directions.IndexOf(playerMovement);
                        if (godModOn)
                        {
                            playerMovement *= 6;
                        }
                        var t = playerTarget + playerMovement;
                        var v = gameState.map.Get(t);

                        if(t.x == -1 && t.y == 16)
                        {
                            playerTarget += playerMovement;
                            acting = true;
                            canPrevent = true;
                        }
                        else if (IsOOB())
                        {
                            if (v == CellValue.Wall || (playerTarget.x == -1 && playerTarget.y == 16) || isEditor)
                            {
                                if (t.x >= -1 &&
                                    t.x <= 23 &&
                                    t.y >= -1 &&
                                    t.y <= 23)
                                {
                                    playerTarget += playerMovement;
                                    acting = true;
                                    canPrevent = true;
                                }
                            }
                        }
                        else
                        {
                            if (CellValue.IsWalkable(v) || isEditor)
                            {

                                playerTarget += playerMovement;
                                acting = true;
                                canPrevent = true;
                            }
                        }
                    }
                }
            }

            if (acting)
            {
                glassesOn = false;
                var speedMultiplier = IsOOB() ? 3 : 1;
                playerPosition += deltaTime * speed * speedMultiplier * (RPoint)playerMovement;
                var dist = ((RPoint)playerTarget + centerShift - playerPosition) * (RPoint)playerMovement;
                if(canPrevent && ((dist <0.5 && !godModOn) || (dist < 2 && godModOn)))
                {
                    if (EatCell())
                    {
                        canPrevent = false;
                    }
                    else
                    {
                        playerPosition = (RPoint)playerTarget + centerShift - (RPoint)playerMovement;
                        playerTarget -= playerMovement;
                        acting = false;
                        canMove = false;
                        delayed = 0.25;
                    }
                }
                if(dist<0)
                {
                    playerPosition = (RPoint)playerTarget + centerShift;
                    acting = false;
                }
            }
        }
        internal bool EatCell ()
        {
            if(isEditor) return true;

            if (playerTarget.x % 6 == 5 || playerTarget.y % 6 == 5)
            {
                SavePlayer(playerTarget);
            }
            else
            {
                var lastVisited = playerTarget - playerMovement;
                if (lastVisited.x % 6 == 5 || lastVisited.y % 6 == 5)
                {
                    SetRespawnPosition(lastVisited);
                }
            }

            var px = playerTarget.x;
            if (px % 6 == 5)
            {
                px = (playerTarget + playerMovement).x;
            }
            if (px % 6 == 5)
            {
                px = currentRoom.x;
            }
            else
            {
                px /= 6;
            }

            var py = playerTarget.y;
            if (py % 6 == 5)
            {
                py = (playerTarget + playerMovement).y;
            }
            if (py % 6 == 5)
            {
                py = currentRoom.y;
            }
            else
            {
                py /= 6;
            }

            var p = new IntPoint(px, py);
            p = Room.FixRoomIndex(p);

            if (currentRoom != p)
            {
                showRoomTitle = true;
                roomTitleFading = 0;
            }
            currentRoom = p;

            if (godModOn)
            {
                gameState.playerLastStablePosition = playerTarget;
                if (rooms[currentRoom].Clear(gameState.map, currentRoom * 6))
                {
                    dead = true;
                }
                return true;
            }

            var v = gameState.map.Get(playerTarget);
            if (v == CellValue.Mine)
            {
                FailRoom();
                return true;
            }
            if (v == CellValue.Open || v == CellValue.Flag)
            {
                gameState.map.Set(playerTarget, CellValue.Zero);
                if (rooms.ContainsKey(currentRoom))
                {
                    switch (rooms[currentRoom].GetRoomState(gameState.map, currentRoom * 6))
                    {
                        case Room.RoomState.Solved:
                            roomSolved = 2;
                            gameState.playerLastStablePosition = playerTarget;
                            rooms[currentRoom].UpdateFlags(gameState.map, currentRoom * 6);
                            canMove = false;
                            break;
                        case Room.RoomState.Solvable:
                            rooms[currentRoom].UpdateFlags(gameState.map, currentRoom * 6);
                            break;
                        case Room.RoomState.Unsolvable:
                            FailRoom();
                            break;
                    }
                }
            }
            if(playerTarget == new IntPoint(4,20))
            {
                gameState.CornerstoneVisited = true;
            }
            if(playerTarget == new IntPoint(2,18))
            {
                gameState.CornerstoneVisited = true;
            }
            if (playerTarget == new IntPoint(20, 5))
            {
                if(!gameState.CornerstoneVisited)
                {
                    if(!gameState.challengeState.challengeDoneExpert)
                    {
                        gameState.challengeState.challengeDoneExpert = true;
                        showChallengeSplash = true;
                        splashText = "Sequence break!";
                    }
                }
            }
            if(v == CellValue.Wall)
            {
                if(!gameState.challengeState.challengeDoneOOB)
                {
                    gameState.challengeState.challengeDoneOOB = true;
                    splashText = "Secret found!";
                    showChallengeSplash = true;
                }
            }
            return true;
        }
        private void SavePlayerImmediately ()
        {
            Loader.TrySave(gameState);
        }
        private void SavePlayer(IntPoint savePoint)
        {
            SetRespawnPosition(savePoint);
            SavePlayerImmediately();
        }

        private void SetRespawnPosition(IntPoint savePoint)
        {
            gameState.playerSavedPosition = new IntPoint(savePoint);
            gameState.playerLastStablePosition = new IntPoint(savePoint);
            gameState.playerSavedDirection = gameState.playerDirection;
        }
        internal void FailRoom ()
        {
            gameState.map.Set(playerTarget, CellValue.Mine);
            dead = true;
            shakeFactor = 1;
        }
        internal void CompleteRoom (IntPoint room)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    var p = room * 6 + new IntPoint(i, j);
                    if (gameState.map.Get(p) == CellValue.Open || gameState.map.Get(p) == CellValue.Flag)
                        gameState.map.Set(p, CellValue.Mine);
                }
            gameState.solvedRooms.Add(room);
            UpdateLocks(room);
            if (room == new IntPoint(3, 0))
            {
                if (gameState.solvedRooms.Count == 16)
                {
                    if (!gameState.challengeState.challengeDoneComplete)
                    {
                        gameState.challengeState.challengeDoneComplete = true;
                        splashText = "All clear!";
                        growth = -4;
                        showChallengeSplash = true;

                        var gs = Loader.LoadOrCreate();
                        gs.challengeState = gameState.challengeState;
                        Loader.TrySave(gs);
                    }
                }
                godModOn = true;
                glassesOn = true;
            }
            else
            {
                SavePlayerImmediately();
            }
        }
        internal void RepairRoom (IntPoint room)
        {
            if (!rooms.ContainsKey(room))
                return;
            var source = rooms[room];

            source.Repair(gameState.map, room * 6);
            
            gameState.solvedRooms.Remove(room);
            UpdateLocks(room);
        }
        internal void UpdateLocks(IntPoint room)
        {
            var roomIsSolved = gameState.solvedRooms.Contains(room);
            var roomCenter = room * 6 + new IntPoint(2, 2);
            foreach( var dir in new IntPoint[] {new IntPoint(0,1),new IntPoint(0,-1),new IntPoint(1,0),new IntPoint(-1,0)})
            {
                var doorPosition = roomCenter + dir * 3;
                if (gameState.map.Get(doorPosition) != CellValue.Wall)
                {
                    if (roomIsSolved || gameState.solvedRooms.Contains(room + dir) || doorPosition == gameState.playerSavedPosition)
                    {
                        gameState.map.Set(doorPosition, CellValue.Zero);
                    }
                    else
                    {
                        gameState.map.Set(doorPosition, CellValue.Door);
                    }
                }
            }
        }
        private void InitRooms()
        {
            if (isEditor)
            {
                InitRooms_Editor();
                return;
            }

            for(int i =0;i<4;i++)
            {
                for(int j =0;j<4;j++)
                {
                    var p = new IntPoint(i, j);
                    var room = Loader.LoadRoom(p);

                    rooms.Add(p, room);

                    if (!gameState.roomsPrepared)
                    {
                        if (!room.Verify(gameState.map, p * 6))
                            RepairRoom(p);
                        else
                        {
                            room.UpdateFlags(gameState.map, p * 6);
                        }
                    }
                }
            }
            gameState.roomsPrepared = true;
        }
        internal void InitRooms_Editor()
        {
#if EDITOR
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var p = new IntPoint(i, j);
                    var room = Loader.LoadRoom(p);

                    var filename = "room" + i + "" + j;
                    if (File.Exists(filename))
                    {
                        room = Loader.LoadRoomFromFile(filename);
                    }

                    rooms.Add(p, room);

                    RepairRoom(p);
                }
            }
            gameState.roomsPrepared = true;
#endif
        }
        internal void Update_Editor ()
        {
#if EDITOR
            speed = 5;
            var p = new IntPoint((int)playerPosition.x, (int)playerPosition.y);
            if (p.x % 6 != 5 && p.y % 6 != 5)
            {
                currentRoom = p / 6;
            }
            if (magic.HasValue)
            {
                switch (magic.Value)
                {
                    case Keys.D1:
                        gameState.map.Set(playerTarget, 1);
                        break;
                    case Keys.D2:
                        gameState.map.Set(playerTarget, 2);
                        break;
                    case Keys.D3:
                        gameState.map.Set(playerTarget, 3);
                        break;
                    case Keys.D4:
                        gameState.map.Set(playerTarget, 4);
                        break;
                    case Keys.D5:
                        gameState.map.Set(playerTarget, 5);
                        break;
                    case Keys.D6:
                        gameState.map.Set(playerTarget, 6);
                        break;
                    case Keys.D9:
                        gameState.map.Set(playerTarget, 9);
                        break;
                    case Keys.D0:
                        gameState.map.Set(playerTarget, 0);
                        break;
                    case Keys.F5:
                        for (int i = 0; i < 4; i++)
                            for (int j = 0; j < 4; j++)
                            {
                                var m = new IntMap(5, 5, (ii, jj) => gameState.map.Get(i * 6 + ii, j * 6 + jj), CellValue.Wall);
                                var r = new Room(m, new IntMap[0]);
                                r.Solve();
                                r.Save("room" + i + "" + j);
                            }
                        break;
                    case Keys.F6:
                        {
                            int i = currentRoom.x;
                            int j = currentRoom.y;
                            {
                                var m = new IntMap(5, 5, (ii, jj) => gameState.map.Get(i * 6 + ii, j * 6 + jj), CellValue.Wall);
                                var r = new Room(m, new IntMap[0]);
                                r.Solve();
                                r.Print();
                            }
                        }
                        break;
                }
            }
#endif
        }
    }
}
