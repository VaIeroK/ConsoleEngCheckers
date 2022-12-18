using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding(1251); //russian in console (US Win)

            Board pBoard = new Board();
            pBoard.RunGame();

            Console.ReadKey();
        }
    }
}
