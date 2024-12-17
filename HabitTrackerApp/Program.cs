﻿using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Globalization;

namespace HabitTrackerApp
{
    internal class Program
    {
        static string connectionString = @"Data Source=habit-Tracker.db";

        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Quantity INTEGER
                        )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

            GetuserInput();
        }

        static void GetuserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Records.");
                Console.WriteLine("Type 2 to Insert Record.");
                Console.WriteLine("Type 3 to Delete Record.");
                Console.WriteLine("Type 4 to Update Record.");
                Console.WriteLine("------------------------------------------\n");

                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("Goodbye!");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;

                    case "2":
                        Insert();
                        break;

                    case "3":
                        Delete();
                        break;

                    case "4":
                        Update();
                        break;

                    default:
                        Console.WriteLine("Invalid Command. Type 0 - 4.");
                        break;
                }
            }
        }
        private static void Insert()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed). Type 0 to return to main menu. \n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"INSERT INTO drinking_water(Date, Quantity) VALUES('{date}', {quantity})";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu.\n\n");
            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetuserInput();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine($"\n\nInvalid date. Please use the format: (dd-mm-yy):\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }
        internal static int GetNumberInput(string msg)
        {
            Console.WriteLine(msg);
            string numberInput = Console.ReadLine();

            if (numberInput == "0") GetuserInput();

            while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number.");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }
        internal static void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                connection.Close();

                Console.WriteLine("-------------------------------");
                foreach (var item in tableData)
                {
                    Console.WriteLine($"{item.Id} - {item.Date.ToString("dd-MMM-yyyy")} - Quantity: {item.Quantity}");
                }
                Console.WriteLine("-------------------------------");
            }
        }
        internal static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput($"\n\nPlease type the Id of the record you want to delete or type 0 to return to the Main Menu\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"Delete from drinking_water WHERE Id = '{recordId}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"Record with Id {recordId} doesn't exist.");
                    Console.ReadKey();
                    Delete();
                }
            }

            Console.WriteLine($"Record with Id {recordId} was deleted.");
            Console.ReadKey();

            GetuserInput();
        }
        internal static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput($"\n\nPlease type the Id of the record you want to update or type 0 to return to the Main Menu\n\n");

            using(var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with ID {recordId} doesn't exist.\n\n");
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();

                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed). Type 0 to return to main menu. \n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText= $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
public class DrinkingWater
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
}