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

        private int menIndex = 0;
        private int selIndex = 0;
        private void clearEx()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 50; i++)
            {
                Console.WriteLine("\t\t\t\t\t\t\t\t\t\t");
            }

            Console.SetCursorPosition(0, 0);
        }

    private void MenuStart()
        {
            //Console.Clear();
            clearEx();
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 50; i++)
            {
                Console.WriteLine("\t\t\t\t\t\t\t\t\t\t");
            }

            Console.SetCursorPosition(0, 0);
            menIndex = 0;
            selIndex = 0;
        }
        private void MenuUpdate()
        {
            Console.ResetColor();
            Console.SetCursorPosition(0, 0);
            //Console.Clear();
            menIndex = 0;
        }
        private void MenuTitle(string title)
        {
            StringBuilder corner = new StringBuilder("=============================");

            //if (title.Length % 2 == 0) corner.Length--;
            if (title.Length % 2 == 0) title+=" ";


            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(corner);

            int all_eq = corner.Length - title.Length;

            for (int i = 0; i < all_eq / 2; i++) { Console.Write(' '); }
            Console.Write(title);
            for (int i = 0; i < all_eq / 2; i++) { Console.Write(' '); }

            Console.WriteLine();
            Console.WriteLine(corner);
            Console.ResetColor();
            Console.WriteLine();
        }

        private void MenuButton(string Text)
        {
            Console.ForegroundColor = (menIndex == selIndex) ? ConsoleColor.White : ConsoleColor.DarkGray;

            char c = (menIndex == selIndex) ? '>' : ' ';
            Console.WriteLine($"{c} {Text}\t\t\t\t"); ++menIndex;

        }

        public void MainMenu()
        {
            MenuStart();

            do
            {
                if (GameValid) break;
                MenuUpdate();

                MenuTitle("ШИШКИ");
                MenuButton("Играть");
                MenuButton("Загрузить сохранение");

                Console.ForegroundColor = ConsoleColor.Black;

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                switch (consoleKeyInfo.Key) 
                {
                    case ConsoleKey.DownArrow:
                        if (selIndex != menIndex-1) selIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selIndex != 0) selIndex--;
                        break;
                    case ConsoleKey.Enter:
                        switch (selIndex)
                        {
                            case 0:
                                Console.ResetColor();
                                CreateGame();
                                MenuStart();
                                break;
                            case 1:
                                //Console.ResetColor();
                                LoadGame();
                                MenuStart();
                                break;
                        }
                        break;
                }
            } while (true);
        }

        private void GetStepText(int step)
        {
            Console.ForegroundColor = (step == 0) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Цвет");
            Console.ForegroundColor = ConsoleColor.DarkGray;                                    Console.Write(">");
            Console.ForegroundColor = (step == 1) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Тип игрока 1");
            Console.ForegroundColor = ConsoleColor.DarkGray;                                    Console.Write(">");
            Console.ForegroundColor = (step == 2) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Тип игрока 2");
            Console.WriteLine("\n");
            Console.ResetColor();
            switch (step)
            {
                case 0:
                    Console.Write("Выберите цвет (Игрок №1)");
                    break;
                case 1:
                    Console.Write("Выберите тип игрока №1");
                    break;
                case 2:
                    Console.Write("Выберите тип игрока №2");
                    break;
            }
            Console.WriteLine("\t\t\t\n");
        }

        public void CreateGame()
        {
            pCheckers.Clear();
            GameValid = true;
            pTeams = new ITeam[2];
            //

            ConsoleColor first_color = new ConsoleColor();
            ConsoleColor second_color = new ConsoleColor();


            //
            MenuStart();

            int step = 0;

            do
            {
                MenuUpdate();

                MenuTitle("Создание игры");

                GetStepText(step);

                switch (step) {
                    case 0:
                        MenuButton("Чёрные");
                        MenuButton("Белые");

                        Console.WriteLine();
                        MenuButton("Назад");
                    break;
                    case 1:
                    case 2:
                        MenuButton("Игрок");
                        MenuButton("Бот");

                        Console.WriteLine();
                        MenuButton("Назад");
                        break;
                }
                Console.ForegroundColor = ConsoleColor.Black;

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (selIndex != menIndex - 1) selIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selIndex != 0) selIndex--;
                        break;
                    case ConsoleKey.Enter:
                        if (selIndex == 2 && step == 0) { GameValid = false; return; }
                        else if (selIndex == 2 && step != 0)
                            step--;
                        else if (selIndex != 2)
                        {
                            if (step == 0)
                            {
                                first_color = (selIndex == 0) ? ConsoleColor.Black : ConsoleColor.White;
                                second_color = (selIndex == 1) ? ConsoleColor.Black : ConsoleColor.White;
                            }
                            else if (step == 1 || step == 2)
                            {
                                if (selIndex == 0)
                                {
                                    if (step == 1)
                                    {
                                        pTeams[0] = new PlayerTeam(first_color);
                                        for (int i = 5; i <= 7; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new PlayerChecker(j + (i % 2 == 0 ? 1 : 0), i, first_color, false));
                                    }
                                    else
                                    {
                                        pTeams[1] = new PlayerTeam(second_color);
                                        for (int i = 0; i < 3; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new PlayerChecker(j + (i % 2 == 0 ? 1 : 0), i, second_color, true));
                                    }
                                }
                                else
                                {
                                    if (step == 1)
                                    {
                                        pTeams[0] = new BotTeam(first_color);
                                        for (int i = 5; i <= 7; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new BotChecker(j + (i % 2 == 0 ? 1 : 0), i, first_color, false));
                                    }
                                    else
                                    {
                                        pTeams[1] = new BotTeam(second_color);
                                        for (int i = 0; i < 3; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new BotChecker(j + (i % 2 == 0 ? 1 : 0), i, second_color, true));
                                    }
                                }
                            }
                            step++;
                        }
                        selIndex = 0;
                        if (step == 3)
                        {
                            if (first_color == ConsoleColor.White)
                                MoveSwitcher = true;
                            else
                                MoveSwitcher = false;

                            DateTime dateTimea = DateTime.Now;
                            SaveName = "save_" + dateTimea.ToString("HH.mm.ss.dd.MM") + ".txt";

                            return;
                        }

                        break;
                }
                Console.ResetColor();
            } while (true);
        }

        public void LoadGame()
        {
            string[] saves = Directory.GetFiles("saves");

            MenuStart();

            do
            {
                MenuUpdate();
                MenuTitle("Сохранения");

                for (int i = 0; i < saves.Length; i++)
                {
                    MenuButton(Path.GetFileName(saves[i]));
                }

                MenuButton("Назад");

                Console.ForegroundColor = ConsoleColor.Black;

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (selIndex != menIndex - 1) selIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selIndex != 0) selIndex--;
                        break;
                    case ConsoleKey.Enter:
                        int m = saves.Length;
                        switch (selIndex)
                        {
                            case var value when value == saves.Length:
                                return;
                            default:
                                GameValid = true;
                                Load(saves[selIndex]);
                                return;
                        }
                }

            } while (true);
        }

        public static void ExitGame()
        {
            Console.WriteLine("Выход из игры\t\t\t\t");
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
                Console.SetCursorPosition(0, 0);
                Frame(); // Костыль, повторная перерисовка для убитых шашек в конце игры
                Console.WriteLine("Чёрные победили!\t\t\t\t");
                GameValid = false;
                Console.ReadKey();
            }
            else if (GetCheckersCount(ConsoleColor.Black) == 0)
            {
                Console.SetCursorPosition(0, 0);
                Frame(); // Костыль, повторная перерисовка для убитых шашек в конце игры
                Console.WriteLine("Белые победили!\t\t\t\t");
                GameValid = false;
                Console.ReadKey();
            }

            Console.SetCursorPosition(0, 0);
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
            if (x < 0 || y < 0 || x > 7 || y > 7)
            {
                Console.WriteLine($"Неизвестная координата x{x} y{y}");
                return;
            }
            string alphabet = "АБВГДЕЖЗ";
            Console.WriteLine($"Пиксель: {alphabet[x]}{y + 1}");
        }

        public static void ShowPixelByPixel(IChecker checker)
        {
            string alphabet = "АБВГДЕЖЗ";
            Console.WriteLine($"Пиксель: {alphabet[checker.Pos[1]]}{checker.Pos[0] + 1}");
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
            Console.WriteLine("\nСтатистика:\t\t");
            Console.WriteLine($"Белых съедено:\t{12 - GetCheckersCount(ConsoleColor.White)}\t\t\nБелых осталось:\t{GetCheckersCount(ConsoleColor.White)}\t\t");
            Console.WriteLine($"Чёрных съедено:\t{12 - GetCheckersCount(ConsoleColor.Black)}\t\t\nЧёрных осталось:{GetCheckersCount(ConsoleColor.Black)}\t\t\n");
            Console.ResetColor();
        }

        public void Draw()
        {
            bool WhitePixel;

            Console.Write("  ");
            for (int i = 0; i < 8; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(GetColumnName(i));
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

                Console.Write(GetRowName(i));

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

        private string GetRowName(int idx)
        {
            return (idx + 1).ToString() + "";
        }

        private string GetColumnName(int idx)
        {
            string alphabet = "АБВГДЕЖЗ";
            return (alphabet[idx]).ToString() + "  ";
        }
    }
}
