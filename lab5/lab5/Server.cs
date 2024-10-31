using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab5
{
    public class Server
    {
        private BigInteger P { get; set; }
        private BigInteger Q { get; set; }
        public BigInteger N { get; private set; }
        public BigInteger D { get; private set; }
        private BigInteger C { get; set; }
        public HashSet<string> Voted { get; private set; } = new HashSet<string>();
        public List<(BigInteger, BigInteger)> Blanks { get; private set; } = new List<(BigInteger, BigInteger)>();

        public void Init()
        {
            P = MainOperations.GeneratePrime(1024);
            Console.WriteLine($"P = {P}\nДлина P = {P.ToByteArray().Length * 8} бит");

            Q = MainOperations.GeneratePrime(1024);
            Console.WriteLine($"Q = {Q}\nДлина Q = {Q.ToByteArray().Length * 8} бит");

            Console.WriteLine($"\nP и Q разные числа: " + ((P != Q)? "true\n" : "false\n"));

            N = P * Q;
            Console.WriteLine($"Открытый параметр N = {N}\nДлина N = {N.ToByteArray().Length * 8} бит");

            BigInteger phi = (P - 1) * (Q - 1);
            D = MainOperations.GenerateCoprime(phi);
            Console.WriteLine($"Открытый ключ D = {D}\nДлина D = {D.ToByteArray().Length * 8} бит");

            C = MainOperations.EvklidSolve(D, phi).Item2;
            while (C < 0)
            {
                C += phi;
            }
            Console.WriteLine($"Закрытый ключ C (хранится на сервере) = {C}\nДлина C = {C.ToByteArray().Length * 8} бит");
            Console.WriteLine($"Проверка параметров C и D:\n CD % phi = {MainOperations.PowModule(C * D, 1, phi)}");
        }

        public void Vote(string name, string choice)
        {
            Console.WriteLine($"\n==================Право голоса {name}==================");
            if (Voted.Contains(name))
            {
                Console.WriteLine($"{name} уже проголосовал! Не пытайтесь обмануть систему.");
                return;
            }

            Dictionary<string, int> votingOptions = new Dictionary<string, int>
            {
                {"Нет", 0},
                {"Да", 1},
                {"Воздерживаюсь", 2}
            };

            BigInteger rnd = MainOperations.GeneratePrime(512);
            int v = votingOptions[choice];
            BigInteger n = rnd << 511 | v;
            Console.WriteLine($"Длина n = {n.ToByteArray().Length * 8}");
            BigInteger r = MainOperations.GenerateCoprime(N);
            BigInteger h = MainOperations.MySha(n);
            Console.WriteLine($"h = {h}");
            BigInteger _h = h * MainOperations.PowModule(r, D, N);
            Console.WriteLine($"_h = {_h}");
            //отправление на сервер

            Voted.Add(name);
            //Вычисление и отправка Алисе
            BigInteger _s = MainOperations.PowModule(_h, C, N);
            //Алиса серваку
            //вычисление подписи своего биллютеня
            BigInteger s = (_s * MainOperations.Inverse(r, N)) % N;

            Console.WriteLine($"Идет проверка поддлинности бюллетеня...");
            if (MainOperations.MySha(n) == MainOperations.PowModule(s, D, N))
            {
                Console.WriteLine($"==================Бюллетень принят!==================");
                Blanks.Add((n, s));
            }
            else
            {
                Console.WriteLine($"==================Явно допущена ошибка!==================");
                Console.WriteLine(MainOperations.MySha(n));
                Console.WriteLine(MainOperations.PowModule(s, D, N));
            }
        }

        public void PrintResults()
        {
            Console.WriteLine("\nУчастники голосования: ");
            foreach (string vote in Voted)
            {
                Console.WriteLine(vote);
            }
            Dictionary<int, int> counter = new Dictionary<int, int>();
            foreach ((BigInteger blank, BigInteger _) in Blanks)
            {
                int choice = (int)(blank & 3);
                if (counter.ContainsKey(choice))
                {
                    counter[choice]++;
                }
                else
                {
                    counter.Add(choice, 1);
                }
            }

            Console.WriteLine("\nРезультаты: ");
            Console.WriteLine($"Против: {counter[0]} человек");
            Console.WriteLine($"За: {counter[1]} человек");
            Console.WriteLine($"Воздержались: {counter[2]} человек");
            if (counter[0] == counter[1] && counter[1] == counter[2])
                Console.WriteLine("Результаты не утешительные... У нас ничья.");
        }
    }
}
