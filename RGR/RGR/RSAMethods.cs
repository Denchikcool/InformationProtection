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
        internal BigInteger C;
        internal ulong D;
        internal BigInteger phi;
        private static Random rnd = new Random();
        public RSAMethods()
        {
            
            ulong P = MainOperations.GenerateModule(1000000, 1000000000, rnd);
            ulong Q = MainOperations.GenerateModule(1000000, 1000000000, rnd);

            N = P * Q;
            phi = (P - 1) * (Q - 1);
            D = MainOperations.GenerateCoprime(phi);

            C = MainOperations.EvklidSolve1(D, phi).Item2;

            if(C < 0)
            {
                C += phi;
            }
        }
    }
}
