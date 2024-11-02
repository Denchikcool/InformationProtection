using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RGR
{
    class RSAMethods
    {
        internal BigInteger N;
        internal ulong C;
        internal ulong D;
        private static Random rnd = new Random();
        public RSAMethods()
        {
            
            ulong P = MainOperations.GenerateModule(1000000, 1000000000, rnd);
            ulong Q = MainOperations.GenerateModule(1000000, 1000000000, rnd);

            N = P * Q;
            ulong phi = (P - 1) * (Q - 1);
            D = GenerateCoprime(phi);

            C = MainOperations.EvklidSolve(D, phi).Item2;

            if(C < 0)
            {
                C += phi;
            }
        }

        private static ulong GenerateCoprime(ulong p)
        {
            ulong result = 0;
            for (ulong i = 2; i < p; i++)
            {
                if (MainOperations.Gcd(p, i) == 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}
