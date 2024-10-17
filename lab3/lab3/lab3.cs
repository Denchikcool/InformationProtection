using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    class lab3
    {
        private static string _filename = "input";
        private static string _extension = "png";

        private static Dictionary<string, BigInteger> _keysGamal = new Dictionary<string, BigInteger>();
        private static Dictionary<string, BigInteger> _keysRSA = new Dictionary<string, BigInteger>();
        private static Dictionary<string, BigInteger> _keysGOST = new Dictionary<string, BigInteger>();

        static void Main(string[] args)
        {
            byte[] m = ReadFile(_filename, _extension);
            Console.WriteLine("Исходные данные:");
            Console.WriteLine(BitConverter.ToString(m));
            Console.WriteLine("============================El-Gamal===========================");

            IList<long> elGamalSign = ElgamalSign(m);
            Console.WriteLine("Подпись El-Gamal:");
            Console.WriteLine(string.Join(", ", elGamalSign));

            File.WriteAllText("elgamal_sign.png", string.Join(", ", elGamalSign));
            ElGamal_signcheck(m, elGamalSign);
            Console.WriteLine("==============================================================\n");

            Console.WriteLine("============================RSA===========================");

            IList<BigInteger> RSAsign = RSASign(m);
            Console.WriteLine("Подпись RSA:");
            Console.WriteLine(string.Join(", ", RSAsign));

            File.WriteAllText("RSA_sign.png", string.Join(", ", RSAsign));
            RSA_SignCheck(m, RSAsign);
            Console.WriteLine("==============================================================\n");

            Console.WriteLine("============================GOST===========================");

            BigInteger GOSTsign = GOSTSign(m);
            Console.WriteLine("Подпись GOST:");
            Console.WriteLine(GOSTsign);

            File.WriteAllText("GOST_sign.png", string.Join(", ", GOSTsign));
            GOST_SignCheck(m, GOSTsign);
            Console.WriteLine("==============================================================\n");
        }

        private static byte[] ReadFile(string fileName, string extension)
        {
            using (var originFile = File.OpenRead(fileName + "." + extension))
            {
                var buffer = new byte[originFile.Length];
                originFile.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private static IList<long> ElgamalSign(byte[] m)
        {
            long g = 0, p = 0, q = 0;
            Random rnd = new Random();

            while (true)
            {
                q = MainOperations.GenerateModule(1000000, 1000000000, rnd);
                p = 2 * q + 1;
                if (MainOperations.IsPrime(p))
                {
                    break;
                }
            }

            while(MainOperations.FastPow(g, q, p) != 1)
            {
                g = rnd.Next(2, (int)p - 1);
            }

            long x = MainOperations.GenerateModule(1, p - 1, rnd);

            long y = MainOperations.FastPow(g, x, p);

            long k = MainOperations.GenerateCoprime(p - 1);

            long r = MainOperations.FastPow(g, k, p);

            Console.WriteLine($"Получены следующие параметры:\np = {p}\ng = {g}\nc = {x}\nd = {y}\nk = {k}\nr = {r}\n");

            _keysGamal = new Dictionary<string, BigInteger>
            {
                {"p", p },
                {"g", g },
                {"y", y },
                {"r", r }
            };

            //string h = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(m)).Replace("-", "").ToLower();
            string h = ComputeMD5Hash(m);
            Console.WriteLine($"Подпись:\nEl-Gamal md5-хеш: {h}");

            
            string h_b = string.Concat(h.Select(i => MainOperations.FastPow(g, Convert.ToInt32(i.ToString(), 16), p).ToString()));
            Console.WriteLine($"Подпись: El-Gamal md5-хеш (числовое представление хеш-функции): {h_b}");

            var u = h.Select(i => (Convert.ToInt32(i.ToString(), 16) - x * r) % (p - 1)).ToList();
            int j = 0;
            List<long> s = new List<long>();
            foreach (long i in u)
            {
                if (j == 2)
                {
                        s.Add((MainOperations.EvklidSolve(k, p - 1).Item2 * (i + 1)));
                }
                else
                {
                        s.Add((MainOperations.EvklidSolve(k, p - 1).Item2 * i));
                }
                j++;
            }
            //var s = u.Select(i => (MainOperations.EvklidSolve(k, p - 1).Item2 * i) % (p - 1)).ToList();

            return s;
        }

        private static void ElGamal_signcheck(byte[] m, IList<long> s)
        {
            BigInteger p = _keysGamal["p"];
            BigInteger y = _keysGamal["y"];
            BigInteger r = _keysGamal["r"];
            BigInteger g = _keysGamal["g"];

            string h = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(m)).Replace("-", "").ToLower();
            Console.WriteLine("Расподпись:");
            Console.WriteLine($"El-Gamal md5-хеш: {h}");

            string h_b = string.Concat(h.Select(i => MainOperations.FastPow1(g, Convert.ToInt32(i.ToString(), 16), p).ToString()));
            Console.WriteLine($"El-Gamal md5-хеш (числовое представление хеш-функции): {h_b}");

            string res = string.Concat(s.Select(i => (MainOperations.FastPow1(y, r, p) * MainOperations.FastPow1(r, i, p) % p).ToString()));
            Console.WriteLine($"El-Gamal проверка аутенфикации: {res}");

            Console.WriteLine("Результат соответствия подписей: " + ((res == h_b) ? "Та самая подпись" : "Подделка"));
        }

        private static IList<BigInteger> RSASign(byte[] m)
        {
            Random rnd = new Random();
            long P = MainOperations.GenerateModule(1000000, 1000000000, rnd);
            Console.WriteLine($"P = {P}");

            long Q = MainOperations.GenerateModule(1000000, 1000000000, rnd);
            Console.WriteLine($"Q = {Q}");

            BigInteger N = P * Q;
            Console.WriteLine($"N = {N}");

            long Phi = (P - 1) * (Q - 1);
            Console.WriteLine($"Phi = {Phi}");

            long d = MainOperations.GenerateCoprime(Phi);
            Console.WriteLine($"d = {d}");

            long c = MainOperations.EvklidSolve(d, Phi).Item2;
            if (c < 0) c += Phi;
            Console.WriteLine($"c = {c}");

            _keysRSA = new Dictionary<string, BigInteger>
            {
                { "N", N },
                { "d", d }
            };

            var h = ComputeMD5Hash(m);
            Console.WriteLine("Подпись:");
            Console.WriteLine($"RSA md5-хеш: {h}");

            string h_b = string.Concat(h.Select(i => int.Parse(i.ToString(), System.Globalization.NumberStyles.HexNumber).ToString()));
            Console.WriteLine($"RSA md5-хеш (числовое представление хеш-функции): {h_b}");

            IList<BigInteger> s = h.Select(i => MainOperations.FastPow1(BigInteger.Parse(i.ToString(), System.Globalization.NumberStyles.HexNumber), c, N)).ToList();

            return s;
        }

        private static void RSA_SignCheck(byte[] m, IList<BigInteger> s)
        {
            BigInteger d = _keysRSA["d"];
            BigInteger N = _keysRSA["N"];

            var h = ComputeMD5Hash(m);
            Console.WriteLine("Расподпись:");
            Console.WriteLine($"RSA md5-хеш: {h}");

            string h_b = string.Concat(h.Select(i => int.Parse(i.ToString(), System.Globalization.NumberStyles.HexNumber).ToString()));
            Console.WriteLine($"RSA md5-хеш (числовое представление хеш-функции): {h_b}");

            StringBuilder eBuilder = new StringBuilder();
            foreach (BigInteger sig in s)
            {
                BigInteger decryptedValue = MainOperations.FastPow1(sig, d, N);
                eBuilder.Append(decryptedValue.ToString("x"));
            }
            string e = eBuilder.ToString();
            Console.WriteLine($"RSA проверка аутенфикации: {e}");

            StringBuilder decimalEBuilder = new StringBuilder();
            foreach (char hexDigit in e)
            {
                int decimalValue = int.Parse(hexDigit.ToString(), System.Globalization.NumberStyles.HexNumber);
                decimalEBuilder.Append(decimalValue.ToString());
            }
            string decimalE = decimalEBuilder.ToString();

            Console.WriteLine(decimalE);

            Console.WriteLine("Результат проверки аутенфикации: " + (decimalE == h_b ? "Та самая подпись" : "Подделка"));
        }


        private static BigInteger GOSTSign(byte[] m)
        {
            Random rnd = new Random();

            BigInteger q = 0;
            while (true)
            {
                if ((!MainOperations.IsPrime1(q)) || (MainOperations.ToBinary1(q).Count != 16))
                {
                    q = new BigInteger(rnd.Next(1, (int)Math.Pow(2, 16)));
                }
                else
                {
                    Console.WriteLine($"Количество бит в числе q = {MainOperations.ToBinary1(q).Count}");
                    break;
                }
            }
            BigInteger b = new BigInteger(rnd.Next(1, (int)Math.Pow(2, 16)));
            BigInteger p = 0;
            
            do {
                if((!MainOperations.CheckPrime(p)) || (MainOperations.ToBinary1(p).Count != 31))
                {
                    b = new BigInteger(rnd.Next(1, (int)Math.Pow(2, 16)));
                    p = q * b + 1;
                }
                else
                {
                    Console.WriteLine($"Количество бит в числе p = {MainOperations.ToBinary1(q * b + 1).Count}");
                    break;
                }
            } while (true);

            Console.WriteLine("p = " + p);
            Console.WriteLine("q = " + q);
            Console.WriteLine("b = " + b);

            BigInteger g = new BigInteger(rnd.Next(2, (int)p - 1));
            BigInteger a = MainOperations.FastPow1(g, b, p);
            while (a <= 1)
            {
                g = new BigInteger(rnd.Next(1, (int)p - 2));
                a = MainOperations.FastPow1(g, b, p);
            }
            Console.WriteLine($"a = {a}\ng = {g}");
            BigInteger x = new BigInteger(rnd.Next(1, (int)q));
            Console.WriteLine("x = " + x);
            BigInteger y = MainOperations.FastPow1(a, x, p);
            Console.WriteLine("y = " + y);

            var h = ComputeMD5Hash(m);
            Console.WriteLine("Подпись:");
            Console.WriteLine($"ГОСТ md5-хеш: {h}");

            string h_b = string.Concat(h.Select(i => int.Parse(i.ToString(), System.Globalization.NumberStyles.HexNumber).ToString()));
            BigInteger hInt = BigInteger.Parse(h_b);

            Console.WriteLine($"ГОСТ md5-хеш (числовое представление хеш-функции): {hInt}");

            BigInteger r = 0;
            BigInteger s = 0;
            BigInteger k = 0;

            while (s == 0)
            {
                while (r == 0)
                {
                    k = new BigInteger(rnd.Next(1, (int)q - 1));
                    r = MainOperations.FastPow1(a, k, p) % q;
                }
                s = (k * hInt + x * r) % q;  
            }
            //Console.WriteLine($"r = {r}\ns = {s}\nk = {k}");
            _keysGOST = new Dictionary<string, BigInteger>
            {
                {"q", q },
                {"a", a },
                {"y", y },
                {"p", p },
                {"r", r }
            };

            return s;
        }

        private static void GOST_SignCheck(byte[] m, BigInteger s)
        {
            BigInteger q = _keysGOST["q"];
            BigInteger y = _keysGOST["y"];
            BigInteger r = _keysGOST["r"];
            BigInteger a = _keysGOST["a"];
            BigInteger p = _keysGOST["p"];

            string hString = ComputeMD5Hash(m);
            if(r > q || s > q)
            {
                return;
            }
            string h_b = string.Concat(hString.Select(i => int.Parse(i.ToString(), System.Globalization.NumberStyles.HexNumber).ToString()));
            BigInteger hInt = BigInteger.Parse(h_b);

            BigInteger temp = MainOperations.EvklidSolve1(hInt, q).Item2;
            //Console.WriteLine($"Возвращаемый значения Евклида:\nItem1 = {MainOperations.EvklidSolve1(hInt, q).Item1}\nItem2 = {MainOperations.EvklidSolve1(hInt, q).Item2}\nItem3 = {MainOperations.EvklidSolve1(hInt, q).Item3}");
            if (temp < 1)
            {
                temp += q;
            }
            //Console.WriteLine($"temp = {temp}");
            BigInteger u1 = (s * temp) % q;
            BigInteger u2 = (-r * temp) % q;
            if(u1 < 0)
            {
                u1 += q;
            }
            if(u2 < 0)
            {
                u2 += q;
            }
            BigInteger v = (MainOperations.FastPow1(a, u1, p) * MainOperations.FastPow1(y, u2, p) % p) % q;
            Console.WriteLine($"v = {v}");
            Console.WriteLine($"r = {r}");
            Console.WriteLine("Результат проверки аутенфикации: " + (v == r ? "Та самая подпись" : "Подделка"));
        }

        private static string ComputeMD5Hash(byte[] input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(input);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
