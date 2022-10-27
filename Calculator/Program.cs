using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Calculator
{
    public static class Number
    {
        private static Dictionary<int, char> alphabet;

        public static void CreateAlphabet()
        {
            alphabet = new Dictionary<int, char>();
            for (int i = 0; i < 62; i++)
            {
                if (i >= 0 && i <= 9)
                    alphabet.Add(i, (char)('0' + i));
                if (i >= 10 && i <= 35)
                    alphabet.Add(i, (char)('A' + i - 10));
                if (i >= 36 && i <= 62)
                    alphabet.Add(i, (char)('a' + i - 36));
            }
        }

        public static void PrintAlphabet()
        {
            foreach (var sym in alphabet)
            {
                Console.WriteLine(sym.Key + " = " + sym.Value);
            }
        }

        public static bool IsCorrectBase(int baseNum)
        {
            return baseNum >= 2 && baseNum <= alphabet.Count;
        }

        public static bool IsCorrectNum(string num, int baseNum)
        {
            foreach (var c in num)
            {
                try
                {
                    GetInt(c);
                }
                catch (Exception)
                {
                    return false;
                }
                if(GetInt(c) >= baseNum)
                {
                    return false;
                }
            }
            return true;
        }

        public static int GetInt(char c)
        {
            for (int i = 0; i < 62; i++)
            {
                if (alphabet[i] == c)
                {
                    return i;
                }
            }
            throw new ArgumentException();
        }

        public static char GetChar(int num)
        {
            return alphabet[num];
        }
    }
    class Program
    {
        static void Main(string[] args)
        { 
            Number.CreateAlphabet();
            Console.WriteLine("Калькулятор для самых маленьких");
            while(true)
            {
                Console.WriteLine("Выберите операцию");
                Console.WriteLine("0)Ознакомиться с алфавитом \n1)перевод(1-62) \n2)Перевод из римской \n3)Перевод в римскую \n4)- \n5)+ \n6)*");
                string operation = Console.ReadLine();
                Console.Clear();
                switch (operation)
                {
                    case "0":
                        {
                            Number.PrintAlphabet();
                            break;
                        }
                    case "1":
                        {
                            Console.WriteLine("Из какой системы счисления");
                            int fromBase = GetBase();

                            Console.WriteLine("В какую систему счислению");
                            int toBase = GetBase();

                            Console.WriteLine("Какое число?");
                            string num = GetNum(fromBase);

                            Console.WriteLine(ToBase(num, fromBase, toBase));
                            break;
                        }
                    case "2":
                        {
                            Console.WriteLine("Введите число в римской системе счисления");
                            RomeToDec(Console.ReadLine());
                            break;
                        }
                    case "3":
                        {
                            Console.WriteLine("Введите число в 10-ой до 5000");
                            int num = int.Parse(GetNum(10));
                            while(num > 5000)
                            {
                                Console.Clear();
                                Console.WriteLine("У тебя наверное был тяжелый день... или детсво, но число должно быть меньше 5000");
                                num = int.Parse(GetNum(10));
                            }
                            ToRoman(num);
                            break;
                        }
                    case "4":
                        {
                            Console.WriteLine("Введите систему счисления");
                            int baseNum = GetBase();

                            Console.WriteLine("Введите первое число");
                            string num1 = GetNum(baseNum);

                            Console.WriteLine("Введите второе число");
                            string num2 = GetNum(baseNum);

                            Minus(num1, num2, baseNum);
                        }
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Подумай еще раз");
                        continue;
                }
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static readonly Dictionary<char, int> romanDigits = new Dictionary<char, int> {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 }
        };
        private static long RomeToDec(string num)
        {
            int res = 0;
            int prev = 0; //MMVI
            foreach (char c in num)
            {
                int curr = romanDigits[c];
                Console.WriteLine($"{c} = {curr}");
                if (prev == 0)
                    Console.WriteLine($"Первое число просто считаем");
                else if (prev >= curr)
                    Console.WriteLine($"Так как предыдущее больше/равно то прибавляем его к {c}");
                else
                    Console.WriteLine($"Так как предыдущее меньше то дважды(чтобы исключить, что ранее мы его добавили) отнимаем его от {c}");
                res += prev < curr ? curr - prev * 2 : curr;
                prev = curr;
            }
            Console.WriteLine(res);
            return res;
        }

        private static string GetNum(int baseNum)
        {
            string num = Console.ReadLine();
            long dec;
            if (Number.IsCorrectNum(num, baseNum))
                dec = ToDecimalUncoment(num, baseNum);
            else
                dec = 0;
            while (!Number.IsCorrectNum(num, baseNum) || dec < 0)
            {
                Console.Clear();
                if(dec >= 0) 
                    Console.WriteLine("Ты облажался и не смог написать число, попрубуй еще раз");
                else
                    Console.WriteLine("Может число поменьше, а?");

                num = Console.ReadLine();
                if (Number.IsCorrectNum(num, baseNum))
                    dec = ToDecimalUncoment(num, baseNum);
                else
                    dec = 0;
            }
            return num;
        }

        private static int GetBase()
        {
            int toBase;
            while (!int.TryParse(Console.ReadLine(), out toBase) || !Number.IsCorrectBase(toBase))
            {
                Console.Clear();
                Console.WriteLine("Я не знаю такую систему счисления, может придумаешь другую?");
            }

            return toBase;
        }

        private static string ToBase(string num, int fromBase, int toBase)
        {
            Console.WriteLine("Для начала переведём в 10-ую");
            long decNum = ToDecimal(num, fromBase);
            string resNum = FromDecToAny(decNum, toBase);
            return resNum;
        }

        private static string FromDecToAny(long decNum, int toBase)
        {
            Console.WriteLine($"Теперь переведём в {toBase}-ую");
            StringBuilder res = new StringBuilder();
            while (decNum >= toBase)
            {
                int mod = (int)(decNum % toBase);
                char c = Number.GetChar(mod);
                res.Append(c);

                Console.Write($"Остаток деления {decNum} от {toBase} равен {mod}");
                if(mod >= 10)
                    Console.Write($" (записывается как {c})");
                Console.ReadKey();
                Console.WriteLine();

                decNum /= toBase;
            }
            if (decNum != 0)
            {
                Console.Write($"Остаток деления {decNum} от {toBase} равен {decNum}");
                if (decNum >= 10)
                    Console.Write($" (записывается как {Number.GetChar((int)decNum)})");
                Console.WriteLine();

                res.Append(Number.GetChar((int)decNum));
            }
            Console.WriteLine($"Соеденив остатки от деления получаем {res}, разворачиваем и получаем ответ");
            return new string(res.ToString().Reverse().ToArray());
        }

        private static long ToDecimal(string num, int baseNum)
        {
            long decNum = 0;
            for (int i = 0; i < num.Length; i++)
            {
                decNum += Number.GetInt(num[i]) * (long)Math.Pow(baseNum, num.Length - i - 1);

                Console.Write($"{num[i]} * {baseNum} ^ {num.Length - i - 1}");
                if (i != num.Length - 1)
                {
                    Console.Write(" + ");
                }
            }
            Console.WriteLine($" = {decNum}");

            if (decNum < 0)
                throw new ArgumentException("Число слишком большое");

            return decNum;
        }

        private static long ToDecimalUncoment(string num, int baseNum)
        {
            long decNum = 0;
            for (int i = 0; i < num.Length; i++)
            {
                decNum += Number.GetInt(num[i]) * (int)Math.Pow(baseNum, num.Length - i - 1);
            }
            return decNum;
        }

        private static int[] nums = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        private static string[] rums = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        private static string ToRoman(int number)
        {
            StringBuilder res = new StringBuilder();

            for (int i = 0; i < nums.Length && number != 0; i++)
            {
                int count = 0;
                while (number >= nums[i])
                {
                    number -= nums[i];
                    res.Append(rums[i]);
                    count++;
                }
                if(count != 0)
                {
                    Console.WriteLine($"Считаем количество {nums[i]}");
                    Console.WriteLine($"Итого {count}, записываем соответствующее количество {rums[i]}");
                    Console.WriteLine(res.ToString());
                    Console.WriteLine();
                }
            }

            return res.ToString();
        }

        public static string Minus(string number1, string number2, int baseNum)
        {
            List<int> num1 = new List<int>();
            List<int> num2 = new List<int>();

            int maxLength = Math.Max(number1.Length, number2.Length);
 
            bool isNegative = ToDecimalUncoment(number1, baseNum) < ToDecimalUncoment(number2, baseNum);
            if (isNegative)
            {
                Console.WriteLine($"Так как {number1} меньше {number2} разность будет отрицательной,\n" +
                    "чтобы ее посчитать, нужно вычесть из второго первое и добавить минус");

                foreach (var num in number2)
                    num1.Add(Number.GetInt(num));
                foreach (var num in number1)
                    num2.Add(Number.GetInt(num));
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(number2);
                Console.WriteLine(number1.PadLeft(maxLength, '0'));
            }
            else
            {
                foreach (var num in number1)
                    num1.Add(Number.GetInt(num));
                foreach (var num in number2)
                    num2.Add(Number.GetInt(num));

                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(number1);
                Console.WriteLine(number2.PadLeft(maxLength, ' '));
            }

            while(num2.Count < maxLength)
            {
                num2.Insert(0, 0);
            }

            for (int i = 0; i < maxLength; i++)
            {
                Console.Write("-");
            }

            int lineMessage = 6;
            StringBuilder sb = new StringBuilder();
            for (int i = maxLength - 1; i >= 0; i--)
            {
                Console.SetCursorPosition(0, lineMessage++);
                Console.WriteLine($"Считаем разряд {i + 1}");

                if (num1[i] < num2[i])
                {
                    if(num1[i] >= 0)
                    {
                        Console.SetCursorPosition(0, lineMessage++);
                        Console.WriteLine($"Так как {num1[i]} < {num2[i]}, то занимаем у девого разряда");
                        Console.SetCursorPosition(i - 1, 0);
                        Console.WriteLine("*");

                        Console.SetCursorPosition(0, lineMessage++);
                        Console.WriteLine($"В таком случае из {num1[i]} + {baseNum} вычетаем {num2[i]} получаем {num1[i] + baseNum - num2[i]}");
                    }
                    else
                    {
                        Console.SetCursorPosition(0, lineMessage++);
                        Console.WriteLine("Так как этот разряд первого числа равен нулю, а ранее из него занимали, то занимаем у левого разряда");
                        Console.SetCursorPosition(0, lineMessage++);
                        Console.WriteLine($"Из {baseNum - 1} вычетаем {num2[i]} получаем {baseNum - 1 - num2[i]}");

                        Console.SetCursorPosition(i - 1, 0);
                        Console.WriteLine("*");
                    }

                    num1[i - 1] = num1[i - 1] - 1;
                    num1[i] += baseNum;
                }
                else
                {
                    Console.SetCursorPosition(0, lineMessage++);
                    Console.WriteLine($"Из {num1[i]} вычетаем {num2[i]} получаем {num1[i] - num2[i]}");
                }
                char resSub = Number.GetChar(num1[i] - num2[i]);

                Console.SetCursorPosition(i, 4);
                Console.WriteLine(resSub);
                sb.Append(resSub);
                Console.ReadKey();
            }

            if (ToDecimalUncoment(number1, baseNum) < ToDecimalUncoment(number2, baseNum))
            {
                string ans = "-" + new string(sb.ToString().Reverse().ToArray());
                Console.SetCursorPosition(0, lineMessage++);
                Console.WriteLine($"Не забываем про минус! Ответ:{ans}");
                return ans;
            } 

            return new string(sb.ToString().Reverse().ToArray());
        }

        public static string Sum(string number1, string numer2, int baseNum)
        {
            return null;
        }


    }
}