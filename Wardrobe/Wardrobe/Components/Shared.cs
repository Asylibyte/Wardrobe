using System.CodeDom.Compiler;
using Net.ConnectCode.Barcode;
using System.Windows;
using System.ComponentModel.DataAnnotations;

/** File for all of the logic that is accessed by multiple pages */
public static class Shared {
    public static string dbStatus = "null";
    public static Dictionary<int, List<ClothingItem>> dict = new Dictionary<int, List<ClothingItem>>();
    public static List<ClothingItem> fileList = new List<ClothingItem>();

    public static void CheckDatabase() {
        string pathToDB = @"..\..\Database\Database.csv";
        if (File.Exists(pathToDB)) {
            Console.WriteLine("file exists");
            ReadFile(pathToDB);
        } else {
            Console.WriteLine("Error: Could not access Database");
        }
    }

    /** Read the file, one line at a time, skipping the header line,
    and assign each line to a ClothingItem that is added to the 'file' list */
    static void ReadFile(string path)
    {
        try
        {
            var stream = new StreamReader(path);
            string currLine = stream.ReadLine();
            currLine = stream.ReadLine();
            while (currLine != null)
            {
                Console.WriteLine($"currline = {currLine}");
                var ClothingItem = new ClothingItem(currLine);
                fileList.Add(ClothingItem);
                currLine = stream.ReadLine();
            }
            dbStatus = "initialized";
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    public static string AddNewItem(string itemtype, string itemsize, string itemgender)
    {
        try
        {
            DateTime timestamp = DateTime.Now;
            TimeSpan interval = DateTime.UnixEpoch - timestamp;
            int intervalAsSeconds = (int)(interval.TotalSeconds);
            string line = $"{timestamp},{itemtype},{itemsize},{itemgender},{true}\n";
            Shared.fileList.Add(new ClothingItem(line));
            var stream = new StreamWriter(@"..\..\Database\Database.csv", append: true);
            stream.Write(line);
            stream.Flush();
            stream.Close();
            return timestamp.ToString();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        return "";
    }
}

public class ClothingItem {
    private int column1 { get; set; } // Date the item was added to Database; doubles as unqiue ID for tagging purposes
    private string column2 { get; set; } // Item type (i.e. dress, suit, pants, etc)
    private string column3 { get; set; } // Item size (i.e. large, XL, 14, 32w40l)
    private string column4 { get; set; } // Item gender (mens, womens, neuter)
    private string column5 { get; set; } // Item status (true = in stock, false = has been checked out already)

    public ClothingItem(string line) {
        var temparray = new string[5];
        var cells = line.Split(",");
        for (int i = 0; i < cells.Length; i++) {
            if (cells[i] != null) {
                temparray[i] = cells[i];
            } else {
                temparray[i] = "";
            }
        }
        column1 = (int) ((DateTime.UnixEpoch - DateTime.Parse(temparray[0])).TotalSeconds);
        column2 = temparray[1];
        column3 = temparray[2];
        column4 = temparray[3];
        column5 = temparray[4];
    }

    public int GetInterval() { return column1; }
    public DateTime GetTimestamp() { return DateTime.UnixEpoch.AddSeconds(column1); }

    public string GetItemType() { return column2; }

    public string GetItemSize() { return column3; }
    public string GetItemGender() { return column4; }
    public string GetStatus() { return column5; }

    public override string ToString() {
        return $"{column1}, {column2}, {column3}, {column4}, {column5}";
    }
}