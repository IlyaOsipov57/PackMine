using PackMine.Geometry;
using PackMine.Puzzle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PackMine.Utility
{
    class Loader
    {
        public static Room LoadRoom (IntPoint z)
        {
            var data = roomdata[z.X][z.Y];
            return ParseRoom(data);
        }
        private static byte[][][]roomdata
        {
            get
            {
                if(_roomdata ==null)
                {
                    _roomdata = new byte[][][] {
                    new byte[][]{Properties.Resources.room00,Properties.Resources.room01,Properties.Resources.room02,Properties.Resources.room03},
                    new byte[][]{Properties.Resources.room10,Properties.Resources.room11,Properties.Resources.room12,Properties.Resources.room13},
                    new byte[][]{Properties.Resources.room20,Properties.Resources.room21,Properties.Resources.room22,Properties.Resources.room23},
                    new byte[][]{Properties.Resources.room30,Properties.Resources.room31,Properties.Resources.room32,Properties.Resources.room33}};
                }
                return _roomdata;
            }
        }
        private static byte[][][] _roomdata = null;
        private static Room ParseRoom(byte[] data)
        {
            var map = new IntMap(5, 5, (i, j) => (int)data[i * 5 + j], CellValue.Wall);
            var solutionsCount = data.Length / 25 -1;
            var solutions = new IntMap[solutionsCount];
            for (int k = 0; k < solutionsCount; k++)
            {
                solutions[k] = new IntMap(5, 5, (i, j) => (int)data[(k+1)*25 + i * 5 + j], 0);
            }
            return new Room(map, solutions);
        }
        public static String DefaultFileName = "save";
        public static int PlayerCoordinatesOffset = 128;
        public static GameState LoadOrCreate ()
        {
            GameState gameState = null;
            try
            {
                if(File.Exists(DefaultFileName))
                {
                    gameState = LoadFromFile(DefaultFileName);
                }
            }
            catch{}
            if (gameState == null)
                gameState =  new GameState();
            return gameState;
        }
        public static void TrySave(GameState gameState)
        {
            try
            {
                SaveToFile(gameState, DefaultFileName);
            }
            catch { }
        }
        private static GameState LoadFromFile (String fileName)
        {
            var data = File.ReadAllBytes(fileName);
            return ParseGameState(data);
        }
        private static void SaveToFile(GameState gameState, String fileName)
        {
            var data = GetData(gameState);
            File.WriteAllBytes(fileName, data);
        }
        private static byte[] GetData (GameState gameState)
        {
            var dataList = new List<byte>();
            dataList.Add((byte)gameState.version);

            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 23; j++)
                {
                    dataList.Add((byte)(gameState.map.Get(i, j)+2));
                }
            }
            for(int i = 0; i<4;i++)
            {
                for(int j=0;j<4;j++)
                {
                    var p = new IntPoint(i,j);
                    dataList.Add((byte)(gameState.solvedRooms.Contains(p) ? 1 : 0));
                }
            }
            dataList.Add((byte)(gameState.playerSavedPosition.X + PlayerCoordinatesOffset));
            dataList.Add((byte)(gameState.playerSavedPosition.Y + PlayerCoordinatesOffset));
            dataList.Add((byte)(gameState.playerSavedDirection));
            dataList.Add((byte)(gameState.playerLastStablePosition.X + PlayerCoordinatesOffset));
            dataList.Add((byte)(gameState.playerLastStablePosition.Y + PlayerCoordinatesOffset));
            dataList.Add((byte)(gameState.playerDirection));

            dataList.Add((byte)(gameState.CornerstoneVisited ? 1 : 0));
            dataList.Add((byte)(gameState.challengeState.challengeDoneOOB ? 1 : 0));
            dataList.Add((byte)(gameState.challengeState.challengeDoneComplete ? 1 : 0));
            dataList.Add((byte)(gameState.challengeState.challengeDoneLost ? 1 : 0));
            dataList.Add((byte)(gameState.challengeState.challengeDoneExpert ? 1 : 0));

            return dataList.ToArray();
        }
        private static GameState ParseGameState (byte[] data)
        {
            var gameState = new GameState();

            gameState.version = (int)data[0];
            gameState.roomsPrepared = gameState.version == GameState.CurrentVersion;

            gameState.map = new IntMap(23, 23, (i, j) => ((int)data[i * 23 + j + 1]) - 2, CellValue.Wall);

            var offset = 23*23 + 1;
            gameState.solvedRooms = new HashSet<IntPoint>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if(data[offset + i*4 + j] != (byte)0)
                    {
                        var p = new IntPoint(i, j);
                        gameState.solvedRooms.Add(p);
                    }
                }
            }

            offset += 4 * 4;
            gameState.playerSavedPosition = new IntPoint(data[offset] - PlayerCoordinatesOffset, data[offset + 1] - PlayerCoordinatesOffset);
            gameState.playerSavedDirection = data[offset + 2];
            gameState.playerLastStablePosition = new IntPoint(data[offset + 3] - PlayerCoordinatesOffset, data[offset + 4] - PlayerCoordinatesOffset);
            gameState.playerDirection = data[offset + 5];
            gameState.CornerstoneVisited = data[offset + 6] != (byte)0;
            gameState.challengeState.challengeDoneOOB = data[offset + 7] != (byte)0;
            gameState.challengeState.challengeDoneComplete = data[offset + 8] != (byte)0;
            gameState.challengeState.challengeDoneLost = data[offset + 9] != (byte)0;
            gameState.challengeState.challengeDoneExpert = data[offset + 10] != (byte)0;

            return gameState;
        }

#if EDITOR
        public static Room LoadRoomFromFile(String fileName)
        {
            var data = File.ReadAllBytes(fileName);
            return ParseRoom(data);
        }
        public static void SaveToFile(IntMap map, IntMap[]solutions, String fileName)
        {
            var data = GetData(map,solutions);
            File.WriteAllBytes(fileName, data);
        }
        private static byte[] GetData(IntMap map, IntMap[] solutions)
        {
            var dataList = new List<byte>();

            for(int i = 0;i <5; i++)
            {
                for(int j = 0;j <5; j++)
                {
                    dataList.Add((byte)map.Get(i,j));
                }
            }

            foreach (var solution in solutions)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        dataList.Add((byte)solution.Get(i, j));
                    }
                }
            }

            return dataList.ToArray();
        }
#endif
    }
}
