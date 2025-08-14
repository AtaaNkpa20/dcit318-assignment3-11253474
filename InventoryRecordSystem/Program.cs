using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// ===========================
// MARKER INTERFACE
// ===========================
public interface IInventoryEntity
{
    int Id { get; }
}

// ===========================
// IMMUTABLE RECORD
// ===========================
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// ===========================
// GENERIC INVENTORY LOGGER
// ===========================
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
            Console.WriteLine($"Data saved successfully to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Could not save data: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("[WARNING] No file found to load.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);

            if (items != null)
            {
                _log = items;
                Console.WriteLine("Data loaded successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Could not load data: {ex.Message}");
        }
    }
}

// ===========================
// INVENTORY APPLICATION
// ===========================
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Mouse", 20, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 10, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 7, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Printer", 3, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No items found in inventory.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"{item.Id} - {item.Name}, Qty: {item.Quantity}, Added: {item.DateAdded}");
        }
    }
}

// ===========================
// MAIN PROGRAM
// ===========================
public class Program
{
    public static void Main()
    {
        string filePath = "inventory.json";

        // First session
        var app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session (fresh memory)
        Console.WriteLine("\n--- Simulating new session ---\n");
        var newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}
