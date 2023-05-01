using PackMine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine
{
    class GameState
    {
        public GameState(ChallengeState challengeState = null)
        {
            Init();

            if (challengeState == null)
            {
                challengeState = new ChallengeState();
            }
            this.challengeState = challengeState;

        }
        public static int CurrentVersion = 1;
        public int version;

        public IntMap map = new IntMap(23, 23, IntMap.Roomy, CellValue.Wall);

        public IntPoint playerSavedPosition;
        public int playerSavedDirection;
        public IntPoint playerLastStablePosition;
        public int playerDirection;
        public HashSet<IntPoint> solvedRooms;
        public bool roomsPrepared;
        public bool CornerstoneVisited;
        public ChallengeState challengeState;

        private void Init()
        {
            version = CurrentVersion;
            map = new IntMap(23, 23, IntMap.Roomy, CellValue.Wall);
            solvedRooms = new HashSet<IntPoint>();
            roomsPrepared = false;
            playerSavedPosition = new IntPoint(GameForm.startPosition);
            playerSavedDirection = 0;
            playerLastStablePosition = new IntPoint(GameForm.startPosition);
            playerDirection = 0;
            CornerstoneVisited = false;
        }
    }
    class ChallengeState
    {
        public bool challengeDoneOOB = false;
        public bool challengeDoneComplete = false;
        public bool challengeDoneLost = false;
        public bool challengeDoneExpert = false;

        public int Total
        {
            get
            {
                return (challengeDoneOOB ? 1 : 0) +
                    (challengeDoneComplete ? 1 : 0) +
                    (challengeDoneLost ? 1 : 0) +
                    (challengeDoneExpert ? 1 : 0);
            }
        }
    }
}
