using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    abstract class IChecker
    {
        public IChecker(int x, int y, ConsoleColor color, bool placeUD)
        {
            Position = new int[2] { y, x };
            IsDamka = false;
            TeamColor = color;
            PlaceUD = placeUD;
        }

        private int[] Position;
        public ConsoleColor TeamColor;
        public bool IsDamka;
        public bool PlaceUD;

        public int[] Pos
        {
            get { return Position; }
            set { Position = value; }
        }

        public bool EqualPos(int x, int y)
        {
            return (Pos[1] == x && Pos[0] == y);
        }

        public void Move(int x, int y)
        {
            Pos[0] = y;
            Pos[1] = x;
        }

        public abstract bool TryMove(int x, int y, ref List<IChecker> Checkers, Board board, bool move = true, bool sleep = false);
        public abstract void GetKillCoords(ref List<IChecker> Checkers, out List<int[]> coords);
    }
}
