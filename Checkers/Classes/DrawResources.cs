using Checkers.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    static class DrawResources
    {
        public static void WritePixel(List<IChecker> Checkers, int x, int y)
        {
            bool has_checker = false;
            int k = 0;
            for (; k < Checkers.Count; k++)
            {
                if (Checkers[k].EqualPos(x, y))
                {
                    has_checker = true;
                    break;
                }
            }

            if (Checkers.Count > k)
            {
                Console.ForegroundColor = Checkers[k].TeamColor;

                if (Checkers[k].IsDamka)
                {
                    if (Checkers[k].TeamColor == ConsoleColor.White)
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }
            }

            if (has_checker)
                Console.Write(" O ");
            else
                Console.Write("   ");

            Console.ResetColor();
        }
    }
}
