using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace lab5
{
    class lab5
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Init();

            server.Vote("Николай", "Воздерживаюсь");
            server.Vote("Денис", "Нет");
            server.Vote("Кирилл", "Да");
            server.Vote("Елизавета", "Нет");
            server.Vote("Яна", "Да");
            server.Vote("Даниил", "Воздерживаюсь");
            server.Vote("Виталий", "Да");
            server.Vote("Ирина", "Нет");
            server.Vote("Петр", "Воздерживаюсь");
            server.Vote("Даниил", "Да");

            server.PrintResults();
        }
    }
}
