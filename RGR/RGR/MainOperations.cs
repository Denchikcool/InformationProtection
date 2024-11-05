using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RGR
{
    class MainOperations
    {
        public static ulong FastPow(ulong baseValue, ulong exponent, ulong modulus)
        {
            ulong result = 1;
            baseValue %= modulus;

            while (exponent > 0)
            {
                if (exponent % 2 == 1)
                {
                    result = (result * baseValue) % modulus;
                }

                baseValue = (baseValue * baseValue) % modulus;
                exponent /= 2;
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

        public static ulong GenerateModule(ulong left, ulong right, Random rnd)
        {
            //Random rnd = new Random();
            ulong p = 0;
            ulong range = right - left + 1;
            while (true)
            {
                //p = rnd.Next((int)left, (int)right);
                p = (ulong)(rnd.NextDouble() * range) + left;
                if (IsPrime(p))
                {
                    return p;
                }
            }
        }

        //Ферма
        public static bool IsPrime(ulong number)
        {
            Random rnd = new Random();
            if (number <= 1) return false;
            else if (number == 2) return true;
            for (ulong i = 0; i < 100; i++)
            {
                ulong a = (ulong)rnd.Next(2, (int)number - 1);
                if (FastPow(a, number - 1, number) != 1 || Gcd(number, a) != 1) return false;
            }
            return true;
        }

        public static ulong Gcd(ulong a, ulong b)
        {
            while (b != 0)
            {
                ulong r = a % b;
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

        public static ulong GenerateExponent(ulong left, ulong right, Random rnd)
        {
            return (ulong)rnd.Next((int)left, (int)right);
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

        public static ulong GenerateCoprime(BigInteger p)
        {
            ulong result = 0;
            for (ulong i = 2; i < p; i++)
            {
                if (MainOperations.Gcd1(p, i) == 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}
