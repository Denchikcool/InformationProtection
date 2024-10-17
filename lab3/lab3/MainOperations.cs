using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    class MainOperations
    {
        public static long FastPow(long a, long x, long p)
        {
            long result = 1;
            List<long> temp = new List<long> { a % p };
            long t = (long)Math.Floor(Math.Log(x, 2));

            List<long> binaryExponent = ToBinary(x);

            for (int i = 1; i <= t; i++)
            {
                temp.Add((temp[i - 1] * temp[i - 1]) % p);
            }

            for (int i = 0; i <= t; i++)
            {
                if (binaryExponent[i] != 0)
                {
                    result = (result * temp[i]) % p;
                }
            }
            return result;
        }

        /*public static ulong FastPow2(ulong a, ulong x, ulong p)
        {
            ulong result = 1;
            List<ulong> temp = new List<ulong> { a % p };
            long t = (long)Math.Floor(Math.Log(x, 2));

            List<ulong> binaryExponent = ToBinary1(x);

            for (int i = 1; i <= t; i++)
            {
                temp.Add((temp[i - 1] * temp[i - 1]) % p);
            }

            for (int i = 0; i <= t; i++)
            {
                if (binaryExponent[i] != 0)
                {
                    result = (result * temp[i]) % p;
                }
            }
            return result;
        }*/

        public static long GenerateCoprime(long p)
        {
            long result = 0;
            for (long i = 2; i < p; i++)
            {
                if (Gcd(p, i) == 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public static BigInteger FastPow1(BigInteger a, BigInteger x, BigInteger p)
        {
            BigInteger result = 1;
            List<BigInteger> temp = new List<BigInteger> { a % p };
            long t = (long)Math.Floor(Math.Log((double)x, 2));

            List<BigInteger> binaryExponent = ToBinary1(x);

            for (int i = 1; i <= t; i++)
            {
                temp.Add((temp[i - 1] * temp[i - 1]) % p);
            }

            for (int i = 0; i <= t; i++)
            {
                if (binaryExponent[i] != 0)
                {
                    result = (result * temp[i]) % p;
                }
            }
            return result;
        }

        private static List<long> ToBinary(long x)
        {
            List<long> result = new List<long>();
            while (x != 0)
            {
                result.Add(x & 1);
                x = x >> 1;
            }
            return result;
        }

        public static List<BigInteger> ToBinary1(BigInteger x)
        {
            List<BigInteger> result = new List<BigInteger>();
            while (x != 0)
            {
                result.Add(x & 1);
                x = x >> 1;
            }
            return result;
        }

        public static long GenerateModule(long left, long right, Random rnd)
        {
            //Random rnd = new Random();
            long p = 0;
            long range = right - left + 1;
            while (true)
            {
                //p = rnd.Next((int)left, (int)right);
                p = (long)(rnd.NextDouble() * range) + left;
                if (IsPrime(p))
                {
                    return p;
                }
            }
        }

        /*public static ulong GenerateModule1(ulong left, ulong right, Random rnd)
        {
            //Random rnd = new Random();
            ulong p = 0;
            ulong range = right - left + 1;
            while (true)
            {
                //p = rnd.Next((int)left, (int)right);
                p = (ulong)(rnd.NextDouble() * range) + left;
                if (IsPrime1(p))
                {
                    return p;
                }
            }
        }

        /*public static bool IsPrime1(ulong number)
        {
            Random rnd = new Random();
            ulong range = number - 2 + 1;
            if (number <= 1) return false;
            else if (number == 2) return true;
            for (long i = 0; i < 100; i++)
            {
                //ulong a = (ulong)rnd.Next(2, (int)number - 1);
                ulong a = (ulong)(rnd.NextDouble() * range) + 2;
                if (FastPow2(a, number - 1, number) != 1 || Gcd1(number, a) != 1) return false;
            }
            return true;
        }*/

        public static bool CheckPrime(BigInteger number)
        {
            if (number <= 1) return false;
            if (number <= 3) return true;
            if (number % 2 == 0 || number % 3 == 0) return false;

            for (BigInteger i = 5; i * i <= number; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        //Ферма
        public static bool IsPrime(long number)
        {
            Random rnd = new Random();
            if (number <= 1) return false;
            else if (number == 2) return true;
            for (long i = 0; i < 100; i++)
            {
                long a = (long)rnd.Next(2, (int)number - 1);
                if (FastPow(a, number - 1, number) != 1 || Gcd(number, a) != 1) return false;
            }
            return true;
        }

        public static bool IsPrime1(BigInteger number)
        {
            Random rnd = new Random();
            if (number <= 1) return false;
            else if (number == 2) return true;
            for (long i = 0; i < 100; i++)
            {
                BigInteger a = (long)rnd.Next(2, (int)number - 1);
                if (FastPow1(a, number - 1, number) != 1 || Gcd1(number, a) != 1) return false;
            }
            return true;
        }

        public static long Gcd(long a, long b)
        {
            while (b != 0)
            {
                long r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        public static BigInteger Gcd1(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        public static long GenerateExponent(long left, long right)
        {
            Random rnd = new Random();
            return rnd.Next((int)left, (int)right);
        }

        public static (long, long, long) EvklidSolve(long a, long b)
        {
            (long, long, long) U = (a, 1, 0);
            (long, long, long) V = (b, 0, 1);

            while (V.Item1 != 0)
            {
                long q = U.Item1 / V.Item1;
                (long, long, long) T = (U.Item1 % V.Item1, U.Item2 - q * V.Item2, U.Item3 - q * V.Item3);
                U = V;
                V = T;
            }

            return U;
        }

        public static (BigInteger, BigInteger, BigInteger) EvklidSolve1(BigInteger a, BigInteger b)
        {
            (BigInteger, BigInteger, BigInteger) U = (a, 1, 0);
            (BigInteger, BigInteger, BigInteger) V = (b, 0, 1);

            while (V.Item1 != 0)
            {
                BigInteger q = U.Item1 / V.Item1;
                (BigInteger, BigInteger, BigInteger) T = (U.Item1 % V.Item1, U.Item2 - q * V.Item2, U.Item3 - q * V.Item3);
                U = V;
                V = T;
            }

            return U;
        }
    }
}
