using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    class Program
    {
        private static string _fileName = "input";
        private static string _extension = "txt";

        private static Dictionary<string, long> _keysShamir = new Dictionary<string, long>();
        private static Dictionary<string, long> _keysElGamal = new Dictionary<string, long>();
        private static Dictionary<string, byte[]> _keysVernam = new Dictionary<string, byte[]>();
        private static Dictionary<string, BigInteger> _keysRSA = new Dictionary<string, BigInteger>();
        static void Main(string[] args)
        {
            var m = ReadFile(_fileName, _extension);
            Console.WriteLine("Исходные данные:");
            Console.WriteLine(BitConverter.ToString(m));
            Console.WriteLine("============================Shamir============================");
            var shamEnc = ShamirEncode(m);
            Console.WriteLine("Зашифрованные данные:");
            Console.WriteLine(string.Join(", ", shamEnc));

            File.WriteAllText("shamir_encoded.txt", string.Join(",", shamEnc));

            var shamDec = ShamirDecode(shamEnc);
            Console.WriteLine("Расшифрованные данные:");
            Console.WriteLine(BitConverter.ToString(shamDec.Select(x => (byte)x).ToArray()));

            File.WriteAllBytes("shamir_decoded.txt", shamDec.Select(x => (byte)x).ToArray());
            Console.WriteLine("==============================================================\n");

            Console.WriteLine("===========================El-Gamal===========================");
            var elGamalEnc = ElGamal_Encode(m);
            Console.WriteLine("Зашифрованные данные:");
            Console.WriteLine(string.Join(", ", elGamalEnc));

            File.WriteAllText("elgamal_encoded.txt", string.Join(",", elGamalEnc));

            var elGamalDec = ElGamal_Decode(elGamalEnc);
            Console.WriteLine("Расшифрованные данные:");
            Console.WriteLine(BitConverter.ToString(elGamalDec.Select(x => (byte)x).ToArray()));

            File.WriteAllBytes("elgamal_decoded.txt", elGamalDec.Select(x => (byte)x).ToArray());
            Console.WriteLine("==============================================================\n");

            Console.WriteLine("============================Vernam============================");
            var vernamEnc = Vernam_Encode(m);
            Console.WriteLine("Зашифрованные данные:");
            Console.WriteLine(string.Join(", ", vernamEnc));

            File.WriteAllText("vernam_encoded.txt", string.Join(",", vernamEnc));

            var vernamDec = Vernam_Decode(vernamEnc);
            Console.WriteLine("Расшифрованные данные:");
            Console.WriteLine(BitConverter.ToString(vernamDec.Select(x => (byte)x).ToArray()));

            File.WriteAllBytes("vernam_decoded.txt", vernamDec.Select(x => (byte)x).ToArray());
            Console.WriteLine("==============================================================\n");

            Console.WriteLine("=============================RSA==============================");
            var rsaEnc = RSA_Encode(m);
            Console.WriteLine("Зашифрованные данные:");
            Console.WriteLine(string.Join(", ", rsaEnc));

            File.WriteAllText("rsa_encoded.txt", string.Join(",", rsaEnc));

            var rsaDec = RSA_Decode(rsaEnc);
            Console.WriteLine("Расшифрованные данные:");
            Console.WriteLine(BitConverter.ToString(rsaDec.Select(x => (byte)x).ToArray()));

            File.WriteAllBytes("rsa_decoded.txt", rsaDec.Select(x => (byte)x).ToArray());
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

        private static long GenerateCoprime(long p)
        {
            long result = 0;
            for (long i = 2; i < p; i++)
            {
                if (MainOperations.Gcd(p, i) == 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
            /*long result_number = 0;
            string temp = p.ToString();
            int lengh = temp.Length;
            var random = new Random();
            //long range = p - (p / 10) + 1;
            
            //long result = (long)(random.NextDouble() * range) + (p / 10);
            while (result_number == 0 && MainOperations.Gcd(p, result_number) != 1)
            {
                long result = random.Next(2, lengh);
                string result_string = "";
                for (int i = 0; i < result; i++)
                {
                    if (i == 0) result_string += random.Next(1, 9) + '0';
                    else result_string += random.Next(0, 9) + '0';
                }

                for (int i = 0; i < result_string.Length; i++)
                {
                    result_number = (result_number * 10) + (result_string[i] - '0');
                }
                //result = random.Next(2, (int)p);
                //result = (long)(random.NextDouble() * range) + (p / 10);
            }
            return result_number;*/
        }

        private static List<long> ShamirEncode(byte[] m)
        {
            var result = new List<long>();
            Random rnd = new Random();
            long p = MainOperations.GenerateModule(1000000, 1000000000, rnd);

            long Ca = GenerateCoprime(p - 1);

            long Da = MainOperations.EvklidSolve(p - 1, Ca).Item3;

            //Console.WriteLine($"item1 = {MainOperations.EvklidSolve(p-1, Ca).Item1}\nitem2 = {MainOperations.EvklidSolve(p - 1, Ca).Item2}\nitem3 = {MainOperations.EvklidSolve(p - 1, Ca).Item3}\n");
            if (Da < 0)
            {
                Da += p - 1;
            }

            long Cb = GenerateCoprime(p - 1);

            long Db = MainOperations.EvklidSolve(p - 1, Cb).Item3;
            if (Db < 0)
            {
                Db += p - 1;
            }

            Console.WriteLine($"Получены следующие параметры:\np = {p}\nCa = {Ca}\nDa = {Da}\nCb = {Cb}\nDb = {Db}\n");

            _keysShamir = new Dictionary<string, long>
            {
                {"p", p },
                {"Ca", Ca },
                {"Da", Da },
                {"Cb", Cb },
                {"Db", Db }
            };

            foreach (var part in m)
            {
                //x1 формирует A, передает B
                long x1 = MainOperations.FastPow(part, Ca, p);
                //x2 формирует B, передает A
                long x2 = MainOperations.FastPow(x1, Cb, p);
                //x3 формирует A, передает B
                long x3 = MainOperations.FastPow(x2, Da, p);

                result.Add(x3);

                //Console.WriteLine($"x1 = {x1}");
                //Console.WriteLine($"x2 = {x2}");
                //Console.WriteLine($"x3 = {x3}");
            }
            return result;
        }

        private static List<long> ShamirDecode(List<long> x3)
        {
            var result = new List<long>();
            long p = _keysShamir["p"];
            long Db = _keysShamir["Db"];

            foreach (var part in x3)
            {
                //Console.WriteLine(part);
                //B дешеврует
                long x4 = MainOperations.FastPow(part, Db, p);
                result.Add(x4);

                //Console.WriteLine($"x4 = {x4}");
            }
            return result;
        }

        private static List<long> ElGamal_Encode(byte[] m)
        {
            List<long> result = new List<long>();
            long q = 0, g = 0, p = 0;
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

            long c = MainOperations.GenerateModule(1, p - 1, rnd);

            long d = MainOperations.FastPow(g, c, p);

            long k = MainOperations.GenerateModule(1, p - 2, rnd);

            long r = MainOperations.FastPow(g, k, p);

            Console.WriteLine($"Получены следующие параметры:\np = {p}\ng = {g}\nc = {c}\nd = {d}\nk = {k}\nr = {r}\n");

            _keysElGamal = new Dictionary<string, long> 
            {
                {"p", p},
                {"g", g},
                {"c", c},
                {"d", d},
                {"k", k},
                {"r", r}
            };

            foreach(var part in m)
            {
                long e = (part * MainOperations.FastPow(d, k, p)) % p;
                result.Add(e);
            }
            return result;
        }

        private static List<long> ElGamal_Decode(List<long> e)
        {
            List<long> result = new List<long>();
            long p = _keysElGamal["p"];
            long c = _keysElGamal["c"];
            long r = _keysElGamal["r"];

            foreach(var part in e)
            {
                long m1 = (part * MainOperations.FastPow(r, p - 1 - c, p)) % p;
                result.Add(m1);
            }
            return result;
        }

        private static List<long> Vernam_Encode(byte[] m)
        {
            List<long> result = new List<long>();
            Random rnd = new Random();
            long e;

            long p = MainOperations.GenerateModule(1000000, 1000000000, rnd);

            byte[] key = Enumerable.Repeat(0, m.Length).Select(_ => (byte)rnd.Next(0, 255)).ToArray();

            _keysVernam = new Dictionary<string, byte[]>
            {
                { "key", key } 
            };

            for (int i = 0; i < m.Length; i++) 
            {
                e = m[i] ^ key[i];
                result.Add(e);
            }
            return result;
        }

        private static List<long> Vernam_Decode(List<long> e) 
        {
            List<long> result = new List<long>();
            byte[] key = _keysVernam["key"];

            for (int i = 0; i < e.Count; i++) 
            {
                byte m = (byte)(e[i] ^ key[i]);
                result.Add(m);
            }
            return result;
        }

        private static List<BigInteger> RSA_Encode(byte[] m)
        {
            List<BigInteger> result = new List<BigInteger>();
            Random rnd = new Random();
            long p = MainOperations.GenerateModule(1000000, 1000000000, rnd);
            Console.WriteLine($"p = {p}");

            long q = MainOperations.GenerateModule(1000000, 1000000000, rnd);
            Console.WriteLine($"q = {q}");

            long n = p * q;
            Console.WriteLine($"n = {n}");

            long phi = (p - 1) * (q - 1);
            Console.WriteLine($"phi = {phi}");

            long d = GenerateCoprime(phi);
            Console.WriteLine($"d = {d}");
            //Console.WriteLine($"NOD = {MainOperations.EvklidSolve(d, phi).Item1}\nx = {MainOperations.EvklidSolve(d, phi).Item2}\ny = {MainOperations.EvklidSolve(d, phi).Item3}");
            long c = MainOperations.EvklidSolve(d, phi).Item2;
            
            if(c < 0)
            {
                c += phi;
            }
            Console.WriteLine($"c = {c}");
            //Console.WriteLine($"Module = {MainOperations.FastPow(c * d, 1, phi)}");
            _keysRSA = new Dictionary<string, BigInteger>
            {
                {"c", c },
                {"n", n }
            };
            

            foreach(var part in m)
            {
                BigInteger e = 0;
                e = MainOperations.FastPow(part, d, n);
                result.Add(e);
            }
            return result;
        }

        private static List<BigInteger> RSA_Decode(List<BigInteger> e)
        {
            List<BigInteger> result = new List<BigInteger>();
            BigInteger c = _keysRSA["c"];
            BigInteger n = _keysRSA["n"];
            foreach (var part in e) 
            {
                BigInteger m1 = 0;
                m1 = MainOperations.FastPow1(part, c, n);
                result.Add(m1);
            }
            return result;
        }
    }
}
