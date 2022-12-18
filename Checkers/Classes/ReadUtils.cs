using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    static class ReadUtils
    {
        public static string ReadSingleChar(string text, char[] chars_mask)
        {
input:
            Console.Write(text);
            string val = Console.ReadLine();

            if (val == null)
                return null;

            string tmp = val;
            val = "";
            for (int i = 0; i < tmp.Length; i++)
                if (tmp[i] != ' ')
                    val += tmp[i];

            val = val.ToUpper();

            bool valid = false;
            for (int i = 0; i < chars_mask.Length; i++)
            {
                if (val == chars_mask[i].ToString())
                {
                    valid = true; 
                    break;
                }
            }
            if (!valid)
            {
                Console.WriteLine("Неверное значение!");
                goto input;
            }

            return val;
        }

        public static int ReadClampedInt(string text, int min, int max)
        {
input:
            Console.Write(text);
            string val = Console.ReadLine();

            if (val == null)
                return min - 1;

            string tmp = val;
            val = "";
            for (int i = 0; i < tmp.Length; i++)
                if (tmp[i] != ' ')
                    val += tmp[i];

            int res;
            try
            {
                res = Convert.ToInt32(val);
            }
            catch (Exception)
            {
                res = min - 1;
            }
            if (res < min || res > max)
            {
                Console.WriteLine("Введено неверное значение!");
                goto input;
            }

            return res;
        }
    }
}
