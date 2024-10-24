using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace lab4
{
    class MainOperations
    {
        public static long FastPow(long a, long x, long p)
        {
            long result = 1;
            List<long> temp = new List<long> { a % (long)p };
            long t = (long)Math.Floor(Math.Log(x, 2));

            List<long> binaryExponent = ToBinary(x);

            for (int i = 1; i <= t; i++)
            {
                temp.Add((temp[i - 1] * temp[i - 1]) % (long)p);
            }

            for (int i = 0; i <= t; i++)
            {
                if (binaryExponent[i] != 0)
                {
                    result = (result * temp[i]) % (long)p;
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

        public static long GenerateCoprime(long p, long start)
        {
            long result = 0;
            for (long i = start; i < p; i++)
            {
                if (Gcd(p, i) == 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}
