using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab5
{
    class MainOperations
    {
        public static BigInteger GeneratePrime(int bits)
        {
            while (true)
            {
                int bytesCount = (bits + 7) / 8;
                byte[] randomBytes = new byte[bytesCount];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }

                //randomBytes[randomBytes.Length - 1] &= 0x7F;

                BigInteger candidate = new BigInteger(randomBytes);
                if (CheckPrime(candidate))
                {
                    return candidate;
                }
            }
        }
        public static bool CheckPrime(BigInteger n)
        {
            if (n <= 1) return false;
            if (n <= 3) return true;

            if (n % 2 == 0 || n % 3 == 0) return false;

            for (int i = 0; i < 10; i++)
            {
                int lenght = n.ToString().Length;
                BigInteger a = GenerateRandomBigInteger(lenght);
                if (PowModule(a, n - 1, n) != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public static BigInteger GenerateRandomBigInteger(int bits)
        {
            int bytesCount = (bits + 7) / 8;
            byte[] randomBytes = new byte[bytesCount];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            //randomBytes[randomBytes.Length - 1] &= 0x7F;

            return new BigInteger(randomBytes);
        }

        public static BigInteger GenerateCoprime(BigInteger n)
        {
            while (true)
            {
                BigInteger r = GeneratePrime(1024);
                if (Gcd(r, n) == 1)
                {
                    return r;
                }
            }
        }
        public static (BigInteger d, BigInteger x, BigInteger y) EvklidSolve(BigInteger a, BigInteger b)
        {
            if (b == 0)
            {
                return (a, 1, 0);
            }
            else
            {
                (BigInteger d, BigInteger x1, BigInteger y1) = EvklidSolve(b, a % b);
                return (d, y1, x1 - (a / b) * y1);
            }
        }

        private static BigInteger Gcd(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        public static BigInteger PowModule(BigInteger baseValue, BigInteger exponent, BigInteger modulus)
        {
            BigInteger result = 1;
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

        public static BigInteger MySha(BigInteger n)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = n.ToByteArray();
                byte[] hash = sha256.ComputeHash(bytes);
                return new BigInteger(hash);
            }
        }

        public static BigInteger Inverse(BigInteger n, BigInteger p)
        {
            BigInteger inv = EvklidSolve(n, p).Item2;
            if (inv < 0)
            {
                inv += p;
            }
            return inv;
        }
    }
}
