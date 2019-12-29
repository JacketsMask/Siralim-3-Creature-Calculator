using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Siralim3CreatureCalculator
{
    class Program
    {
        public static List<Recipe> GetRecipes(SQLiteConnection conn)
        {
            List<Recipe> recipies = new List<Recipe>();
            SQLiteCommand command = new SQLiteCommand(conn);
            command.CommandText = "SELECT Type, Species, Combos, Product, FirstMate, SecondMate, Notes FROM Recipes";

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int index = 0;
                string type = reader.GetString(index++);
                string species = reader.GetString(index++);
                string combos = reader.GetString(index++);
                string product = reader.GetString(index++);
                string firstMate = reader.GetString(index++);
                string secondMate = reader.GetString(index++);
                string notes = null;
                if (!reader.IsDBNull(index))
                {
                    notes = reader.GetString(index++);
                }
                recipies.Add(new Recipe()
                {
                    Type = type,
                    Species = species,
                    Combos = combos,
                    Product = product,
                    FirstMate = firstMate,
                    SecondMate = secondMate,
                    Notes = notes
                });
            }
            return recipies;
        }

        public static List<AvailableMonster> GetAvailableMonsters(SQLiteConnection conn)
        {
            List<AvailableMonster> availableMonsters = new List<AvailableMonster>();
            SQLiteCommand command = new SQLiteCommand(conn);
            command.CommandText = "SELECT Species, Name FROM AvailableMonsters";

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int index = 0;
                string species = reader.GetString(index++);
                string name = reader.GetString(index++);
                availableMonsters.Add(new AvailableMonster()
                {
                    Species = species,
                    Name = name,
                });
            }
            return availableMonsters;
        }

        static void Main(string[] args)
        {
            string dbPath = @"Siralim3.db";
            var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();

            var availableMonsters = GetAvailableMonsters(connection);
            var recipes = GetRecipes(connection);

            foreach (var recipe in recipes)
            {
                string firstMate = recipe.FirstMate;
                bool firstCompareSpecies = false;
                if (recipe.FirstMate.Contains("Any"))
                { //generic creature, check type instead of name
                    firstMate = recipe.FirstMate.Replace("Any ", "");
                    firstCompareSpecies = true;
                }

                string secondMate = recipe.SecondMate;
                bool secondCompareSpecies = false;
                if (recipe.SecondMate.Contains("Any"))
                { //generic creature, check type instead of name
                    secondMate = recipe.SecondMate.Replace("Any ", "");
                    secondCompareSpecies = true;
                }

                AvailableMonster firstMatch;
                if (firstCompareSpecies)
                {
                    firstMatch = availableMonsters.Where(x => x.Species.Equals(firstMate)).FirstOrDefault();
                }
                else
                {
                    firstMatch = availableMonsters.Where(x => x.Name.Equals(firstMate)).FirstOrDefault();
                }

                AvailableMonster secondMatch;
                if (secondCompareSpecies)
                {
                    secondMatch = availableMonsters.Where(x => x.Species.Equals(secondMate)).FirstOrDefault();
                }
                else
                {
                    secondMatch = availableMonsters.Where(x => x.Name.Equals(secondMate)).FirstOrDefault();
                }
                if (firstMatch != null && secondMatch != null)
                {
                    Console.WriteLine($"[{recipe.Type}][{recipe.Species}] {recipe.Product} can be created with {firstMatch.Name} and {secondMatch.Name})");
                }
            }

            Console.ReadLine();
        }
    }
}
