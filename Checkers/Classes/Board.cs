using Checkers.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checkers
{
    internal class Board
    {
        public List<IChecker> pCheckers;
        public ITeam[] pTeams;
        static public bool GameValid;
        private bool MoveSwitcher;
        private string SaveName;
        public static int BotWalkTime = 500;
        public Board()
        {
            pCheckers = new List<IChecker>();
            GameValid = false;
            MoveSwitcher = false;
            SaveName = "save.txt";
        }

        public void MainMenu()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("=============================");
            Console.WriteLine("============ШАШКИ============");
            Console.WriteLine("=============================");

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("=");
            Console.ResetColor();

            Console.Write("  1. Играть                ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("=\n=");
            Console.ResetColor();

            Console.Write("  2. Загрузить сохранение  ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("=\n");
            Console.WriteLine("=============================");
            Console.ResetColor();
input:
            Console.Write("Ввод: ");
            string line = Console.ReadLine();
            int mode;

            try
            {
                mode = Convert.ToInt32(line);
            }
            catch (Exception)
            {
                mode = 0;
            }
            if (mode < 1 || mode > 2)
            {
                Console.WriteLine("Введено неверное значение!");
                goto input;
            }

            switch (mode)
            {
                case 1:
                    CreateGame();
                    break;
                case 2:
                    LoadGame();
                    break;
            }
        }

        public void CreateGame()
        {
            pCheckers.Clear();
            GameValid = true;
            pTeams = new ITeam[2];

            // Нижняя команда
            Console.WriteLine("Команда снизу:");

            string sfirst_color = ReadUtils.ReadSingleChar("Выберете цвет Ч\\Б: ", new char[2] { 'Ч', 'Б' });
            if (sfirst_color == null)
            {
                MainMenu();
                return;
            }

            string down_player_type = ReadUtils.ReadSingleChar("Это бот или игрок? (Б\\И): ", new char[2] {'Б','И'});
            if (down_player_type == null)
            {
                MainMenu();
                return;
            }

            ConsoleColor first_color = (sfirst_color == "Ч" ? ConsoleColor.Black : ConsoleColor.White);
            ConsoleColor second_color = (first_color == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.White);
            if (down_player_type == "Б")
            {
                pTeams[0] = new BotTeam(first_color);
                for (int i = 5; i <= 7; i++)
                    for (int j = 0; j <= 7; j += 2)
                        pCheckers.Add(new BotChecker(j + (i % 2 == 0 ? 1 : 0), i, first_color, false));
            }
            else
            {
                pTeams[0] = new PlayerTeam(first_color);
                for (int i = 5; i <= 7; i++)
                    for (int j = 0; j <= 7; j += 2)
                        pCheckers.Add(new PlayerChecker(j + (i % 2 == 0 ? 1 : 0), i, first_color, false));
            }

            // Верхняя команда
            Console.WriteLine("Команда сверху:");
            string up_player_type = ReadUtils.ReadSingleChar("Это бот или игрок? (Б\\И): ", new char[2] { 'Б', 'И' });
            if (up_player_type == null)
            {
                MainMenu();
                return;
            }

            if (up_player_type == "Б")
            {
                pTeams[1] = new BotTeam(second_color);
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j <= 7; j += 2)
                        pCheckers.Add(new BotChecker(j + (i % 2 == 0 ? 1 : 0), i, second_color, true));
            }
            else
            {
                pTeams[1] = new PlayerTeam(second_color);
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j <= 7; j += 2)
                        pCheckers.Add(new PlayerChecker(j + (i % 2 == 0 ? 1 : 0), i, second_color, true));
            }

            if (first_color == ConsoleColor.White)
                MoveSwitcher = true;
            else
                MoveSwitcher = false;

            DateTime dateTime = DateTime.Now;
            SaveName = "save_" + dateTime.ToString("HH.mm.ss.dd.MM") + ".txt";

            Console.Clear();
        }

        public void LoadGame()
        {
            string[] saves = Directory.GetFiles("saves");
            Console.WriteLine("Сохранения:");
            for (int i = 0; i < saves.Length; i++)
            {
                Console.WriteLine((i + 1).ToString() + ". " + Path.GetFileName(saves[i]));
            }

            Console.Write("Ввод: ");
            int save = ReadUtils.ReadClampedInt("Ввод: ", 1, saves.Length);

            if (save == 0)
            {
                MainMenu();
                return;
            }

            GameValid = true;
            Load(saves[save - 1]);
            Console.Clear();
        }

        public static void ExitGame()
        {
            Console.WriteLine("Выход из игры");
            Thread.Sleep(500);
            GameValid = false;
        }

        public void RunGame()
        {
            MainMenu();

            while (GameValid)
            {
                Frame();
                Move();
                EndFrame();

                if (!GameValid)
                {
                    if (File.Exists("saves\\" + SaveName))
                        File.Delete("saves\\" + SaveName);
                    MainMenu();
                }
            }
        }

        public void Load(string filename)
        {
            StreamReader sr = new StreamReader(filename);

            pCheckers.Clear();

            MoveSwitcher = true;
            SaveName = sr.ReadLine();

            ConsoleColor player_color = ConvertColor(sr.ReadLine());
            ConsoleColor bot_color = (player_color == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.White);
            pTeams = new ITeam[2] { new PlayerTeam(player_color), new BotTeam(bot_color) };

            int CheckersCount = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < CheckersCount; i++)
            {
                int y = Convert.ToInt32(sr.ReadLine());
                int x = Convert.ToInt32(sr.ReadLine());
                ConsoleColor color = ConvertColor(sr.ReadLine());
                bool damka = Convert.ToBoolean(sr.ReadLine());
                IChecker checker;

                if (color == player_color)
                    checker = new PlayerChecker(x, y, color, false);
                else
                    checker = new BotChecker(x, y, color, true);

                checker.IsDamka = damka;
                pCheckers.Add(checker);
            }

            sr.Close();
        }

        private ConsoleColor ConvertColor(string color)
        {
            if (color == "White")
                return ConsoleColor.White;
            else
                return ConsoleColor.Black;
        }

        public void Save()
        {
            if (!Directory.Exists("saves"))
                Directory.CreateDirectory("saves");

            StreamWriter sw = new StreamWriter("saves\\" + SaveName);
            sw.WriteLine(SaveName);
            sw.WriteLine(pTeams[0].TeamColor.ToString());

            sw.WriteLine(pCheckers.Count.ToString());
            for (int i = 0; i < pCheckers.Count; i++)
            {
                sw.WriteLine(pCheckers[i].Pos[0].ToString());
                sw.WriteLine(pCheckers[i].Pos[1].ToString());
                sw.WriteLine(pCheckers[i].TeamColor.ToString());
                sw.WriteLine(pCheckers[i].IsDamka.ToString());
            }
            sw.Close();
        }

        public void Move()
        {
            if (MoveSwitcher)
                pTeams[0].Move(ref pCheckers, this);
            else
                pTeams[1].Move(ref pCheckers, this);

            MoveSwitcher = !MoveSwitcher;
        }

        public void Frame()
        {
            Draw();
            DrawStats();
        }

        public void EndFrame()
        {
            if (GetCheckersCount(ConsoleColor.White) == 0)
            {
                Console.Clear();
                Frame(); // Костыль, повторная перерисовка для убитых шашек в конце игры
                Console.WriteLine("Чёрные победили!");
                GameValid = false;
                Console.ReadKey();
            }
            else if (GetCheckersCount(ConsoleColor.Black) == 0)
            {
                Console.Clear();
                Frame(); // Костыль, повторная перерисовка для убитых шашек в конце игры
                Console.WriteLine("Белые победили!");
                GameValid = false;
                Console.ReadKey();
            }

            Console.Clear();

            Save();
        }

        public int GetCheckersCount(ConsoleColor color)
        {
            int count = 0;
            foreach (var ch in pCheckers)
                if (ch.TeamColor == color) 
                    count++;

            return count;
        }

        public static void ShowPixelByCoord(int x, int y)
        {
            Console.WriteLine($"Пиксель: {GetPixelByCoord(x, y)}");
        }

        public static void ShowPixelByPixel(IChecker checker)
        {
            Console.WriteLine($"Пиксель: {GetixelByPixel(checker)}");
        }

        public static string GetPixelByCoord(int x, int y)
        {
            if (x < 0 || y < 0 || x > 7 || y > 7)
            {
                return ($"x{x} y{y}");
            }
            string alphabet = "АБВГДЕЖЗ";
            return ($"{alphabet[x]}{y + 1}");
        }

        public static string GetixelByPixel(IChecker checker)
        {
            string alphabet = "АБВГДЕЖЗ";
            return ($"{alphabet[checker.Pos[1]]}{checker.Pos[0] + 1}");
        }

        public static IChecker GetPixel(int X, int Y, ref List<IChecker> Checkers)
        {
            foreach (var checker in Checkers)
            {
                if (checker.EqualPos(X, Y))
                    return checker;
            }

            return null;
        }

        public IChecker GetPixel(int X, int Y)
        {
            foreach (var checker in pCheckers)
            {
                if (checker.EqualPos(X, Y))
                    return checker;
            }

            return null;
        }

        public static void RemoveChecker(int X, int Y, ref List<IChecker> Checkers)
        {
            if (Checkers.Contains(GetPixel(X, Y, ref Checkers)))
                Checkers.Remove(GetPixel(X, Y, ref Checkers));
        }

        public void RemoveChecker(int X, int Y)
        {
            if (pCheckers.Contains(GetPixel(X, Y)))
                pCheckers.Remove(GetPixel(X, Y));
        }

        public void DrawStats()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nСтатистика:");
            Console.WriteLine($"Белых съедено: {12 - GetCheckersCount(ConsoleColor.White)}\nБелых осталось: {GetCheckersCount(ConsoleColor.White)}");
            Console.WriteLine($"Чёрных съедено: {12 - GetCheckersCount(ConsoleColor.Black)}\nЧёрных осталось: {GetCheckersCount(ConsoleColor.Black)}\n");
            Console.ResetColor();
        }

        public void Draw()
        {
            bool WhitePixel;
            string alphabet = "АБВГДЕЖЗ";

            Console.Write("  ");
            for (int i = 0; i < 8; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write((alphabet[i]).ToString() + "  ");
                Console.ResetColor();
            }
            Console.WriteLine();

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                    WhitePixel = true;
                else
                    WhitePixel = false;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write((i + 1).ToString());

                for (int j = 0; j < 8; j++)
                {
                    if (WhitePixel)
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkGray;

                    DrawResources.WritePixel(pCheckers, j, i);

                    WhitePixel = !WhitePixel;
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}
