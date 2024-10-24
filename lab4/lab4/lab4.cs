using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4
{
    class lab4
    {
        static void Main(string[] args)
        {
            Console.WriteLine("♠ ♣ ♥ ♦");
            MentalPoker(4);
        }

        static void MentalPoker(int playersNum)
        {
            long p, q, counter = 3;
            List<long> C = new List<long>();
            List<long> D = new List<long>();
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
            Console.WriteLine($"q = {q}");
            Console.WriteLine($"p = {p}");

            for (int i = 0; i < playersNum; i++)
            {
                long C_temp = MainOperations.GenerateCoprime(p - 1, counter);
                long D_temp = MainOperations.EvklidSolve(C_temp, p - 1).Item2;
                if (D_temp < 0)
                {
                    D_temp += (p - 1);
                }
                Console.WriteLine($"C_temp * D_temp mod (p-1) = {MainOperations.FastPow(C_temp * D_temp, 1, p - 1)}");
                C.Add(C_temp);
                D.Add(D_temp);
                counter += 5;
            }
            
            Console.WriteLine($"C = {string.Join(", ", C)}");
            Console.WriteLine($"D = {string.Join(", ", D)}");

            Dictionary<long, string> originDeck = GenDeck();
            Console.WriteLine($"Deck: {string.Join(", ", originDeck.Values)}");

            List<long> deckKeys = originDeck.Keys.ToList();
            Console.WriteLine($"Deck Keys: {string.Join(", ", deckKeys)}");
            Random random = new Random();
            for (int i = 0; i < playersNum; i++)
            {
                deckKeys = deckKeys.Select(K => MainOperations.FastPow(K, C[i], p)).ToList();
                Shuffle(deckKeys, random);
                Console.WriteLine($"Shuffle {i + 1}: ");
                Console.WriteLine($"{string.Join(", ", deckKeys)}");
            }

            List<List<long>> hands = new List<List<long>>();
            for (int i = 0; i < playersNum; i++)
            {
                hands.Add(new List<long>());
                for (int j = 0; j < 2; j++)
                {
                    long card = deckKeys[j];
                    deckKeys.Remove(card);
                    hands[i].Add(card);
                }
            }

            Console.WriteLine($"Hands: {string.Join(", ", hands.Select(hand => $"[{string.Join(", ", hand)}]"))}");

            List<long> table = deckKeys.Take(5).ToList();
            Console.WriteLine($"Table: {string.Join(", ", table)}");

            for (int i = 0; i < playersNum; i++)
            {
                table = table.Select(card => MainOperations.FastPow(card, D[i], p)).ToList();
            }
            Console.WriteLine($"Table decoded: {string.Join(", ", table)}");

            Dictionary<long, string> tableCards = table.ToDictionary(key => key, key => originDeck[key]);
            Console.WriteLine($"\nCards on the table: {string.Join(" ", tableCards.Values.Select(card => GetColoredCard(card)))}");

            for (int i = 0; i < playersNum; i++)
            {
                for (int j = 0; j < playersNum; j++)
                {
                    if (i != j)
                    {
                        for (int v = 0; v < hands[i].Count; v++)
                        {
                            hands[i][v] = MainOperations.FastPow(hands[i][v], D[j], p);
                        }
                    }
                }
                for (int v = 0; v < hands[i].Count; v++)
                {
                    hands[i][v] = MainOperations.FastPow(hands[i][v], D[i], p);
                }
            }

            List<Dictionary<long, string>> processedHands = hands.Select(hand => hand.ToDictionary(kvp => kvp, kvp => originDeck.ContainsKey(kvp) ? originDeck[kvp] : "Invalid card")).ToList();

            for (int i = 0; i < playersNum; i++)
            {
                Console.Write($"Player {i + 1} has cards: ");
                foreach (string card in processedHands[i].Values)
                {
                    Console.Write(card + " ");
                }
                Console.WriteLine();
            }
        }

        private static Dictionary<long, string> GenDeck()
        {
            string[] suits = { "♠", "♣", "♥", "♦" };
            string[] faces = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            Dictionary<long, string> deck = new Dictionary<long, string>();
            long index = 2;
            foreach (string suit in suits)
            {
                foreach (string face in faces)
                {
                    deck[index] = $"{suit}{face}";
                    index++;
                }
            }
            return deck;
        }

        private static string GetColoredCard(string card)
        {
            return $"{card}";
        }

        private static void Shuffle<T>(List<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
