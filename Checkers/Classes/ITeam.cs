using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    abstract class ITeam
    {
        public ITeam(ConsoleColor color)
        {
            TeamColor = color;
        }

        public ConsoleColor TeamColor;
        public string Owner;
        public abstract void Move(ref List<IChecker> Checkers, Board board);
    }
}
