
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("===============Task1=====================\n");
        Console.WriteLine("Введите основание степени: ");
        long baseNumber = long.Parse(Console.ReadLine());
        //Console.WriteLine("Введите степень: ");
        //int exponent = int.Parse(Console.ReadLine());
        

        //Console.WriteLine("Введите модуль: ");
        //int module = int.Parse(Console.ReadLine());
        long module = Task1.GenerateModule(100000000, 100000000000);
        long exponent = Task1.GenerateExponent(1, module-1);
        long result1 = Task1.FastPow(baseNumber, exponent, module);
        Console.WriteLine($"Быстрое возведение для {baseNumber}^{exponent} mod {module} = {result1}");
        Console.WriteLine("\n=========================================");

        Console.WriteLine("\n===============Task2=====================\n");
        Console.WriteLine("Введите коэффицент перед X: ");
        int a = int.Parse(Console.ReadLine());
        Console.WriteLine("Введите коэффицент перед Y: ");
        int b = int.Parse(Console.ReadLine());
        var result2 = Task2.Solve(a, b);
        Console.WriteLine($"НОД({a}, {b}) = {result2.GCD}\nПри x = {result2.X}\nПри y = {result2.Y}");
        Console.WriteLine("\n=========================================");

        Console.WriteLine("\n===============Task3=====================\n");
        (long, long) result3 = Task3.MakeKey();
        Console.WriteLine($"Первый секретный ключ: {result3.Item1}\nВторой секретный ключ: {result3.Item2}");
        Console.WriteLine("\n=========================================");

        Console.WriteLine("\n===============Task4=====================\n");
        long result4 = Task4.GuessKey(baseNumber, module, result1);
        if (result4 != exponent)
        {
            Console.WriteLine($"Ключ найден, но он отличен от изначального x.\nНайденный ключ: {result4}\nИспользованный ключ: {exponent}");
        }
        else
        {
            Console.WriteLine($"Ключ найден и он совпадает с использованным ключом!\nТ.е. сгенерированный X = {exponent}, а наш ключ Y = {result4}");
        }
        Console.WriteLine("\n=========================================");
        Console.WriteLine($"x1 = {exponent}, x2 = {result4}");

    }  
}

public class Task1
{
    public static long FastPow(long a, long x, long p)
    {
        long result = 1;
        List<long> temp = new List<long> { a % p };
        int t = (int)Math.Floor(Math.Log(x, 2));

        List<long> binaryExponent = ToBinary(x);

        for(int i = 1; i <= t; i++)
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


    public static List<long> ToBinary(long x)
    {
        List<long> result = new List<long>();
        while(x != 0)
        {
            result.Add(x & 1);
            x = x >> 1;
        }
        return result;
    }

    public static long GenerateModule(long left, long right)
    {
        Random rnd = new();
        long p;
        while (true)
        {
            p = rnd.Next((int)left, (int)right);
            if (IsPrime(p))
            {
                return p;
            }
        }
    }
    //Ферма
    public static bool IsPrime(long number)
    {
        Random rnd = new();
        if (number <= 1) return false;
        else if (number == 2) return true;
        for(int i = 0; i < 100; i++)
        {
            long a = rnd.Next(2, (int)number - 1);
            if (FastPow(a, number - 1, number) != 1 || Gcd(number, a) != 1) return false;
        }
        return true;
    }

    private static long Gcd(long a, long b)
    {
        while(b != 0)
        {
            long r = a % b;
            a = b;
            b = r;
        }
        return a;
    }

    public static long GenerateExponent(long left, long right)
    {
        Random rnd = new();
        return rnd.Next((int)left, (int)right);
    }
}

public class EvklidGCDResult
{
    public int GCD { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

}

public static class Task2
{
    public static EvklidGCDResult Solve(int a, int b)
    {
        (int, int , int) U = (a, 1, 0);
        (int, int, int) V = (b, 0, 1);

        while(V.Item1 != 0)
        {
            int q = U.Item1 / V.Item1;
            (int, int, int) T = (U.Item1 % V.Item1, U.Item2 - q * V.Item2, U.Item3 - q * V.Item3);
            U = V;
            V = T;
        }
        return new EvklidGCDResult
        { 
            GCD = U.Item1,
            X = U.Item2,
            Y = U.Item3,
        };
    }
}

public class Task3
{
    static long p, q, Xa, Xb, Ya, Yb, Zab, Zba;
    static int g = 3;
    public static (long, long) MakeKey()
    {
        Random rnd = new Random();
        while(true)
        {
            q = Task1.GenerateModule(1000000, 1000000000);
            p = 2 * q + 1;
            if (Task1.IsPrime(p))
            {
                break;
            }
        }
        Console.WriteLine($"p = {p} и q = {q}");

        while(Task1.FastPow(g, q, p) == 1)
        {
            g = rnd.Next(2, (int)p - 1);
        }
        Console.WriteLine($"g = {g}");

        Xa = Task1.GenerateExponent(1, p - 1);
        Xb = Task1.GenerateExponent(1, p - 1);
        Console.WriteLine($"Закрытые ключи: Xa = {Xa} и Xb = {Xb}");

        Ya = Task1.FastPow(g, Xa, p);
        Yb = Task1.FastPow(g, Xb, p);
        Console.WriteLine($"Открытые ключи: Ya = {Ya} и Yb = {Yb}");

        Zab = Task1.FastPow(Yb, Xa, p);
        Zba = Task1.FastPow(Ya, Xb, p);

        return (Zab, Zba);
    }
}

public class Task4
{
    static long k, m;
    static Dictionary<long, long> baby = new Dictionary<long, long>();
    static List<long> gigant = new List<long>();
    public static long GuessKey(long a, long p, long y)
    {
        k = (long)Math.Ceiling(Math.Sqrt(p));
        m = k;

        for(long i = 0; i < m; i++)
        {
            baby[Task1.FastPow(a, i, p) * y % p] = i;
        }
        //Console.WriteLine("Словарь маленьких шагов: " + string.Join(", ", baby));

        for(long j = 1; j <= k; j++)
        {
            gigant.Add(Task1.FastPow(a, m * j, p));
        }
        //Console.WriteLine("Список гиганских шагов: " + string.Join(", ", gigant));

        for (long i = 1; i <= k; i++)
        {
            if (baby.TryGetValue(gigant[(int)i - 1], out long j))
            {
                return i * m - j;
            }  
        }

        return -1;
    }
}

