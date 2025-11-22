using ApathyAndMania.DayStats;

namespace ApathyAndMania;
using System.Text.Json;

public static class JsonManager
{
    public static async Task SaveDay(Day day)
    {
        DirectoryInfo dirInfo = new DirectoryInfo("Days/");
        if (!dirInfo.Exists)
            dirInfo.Create();
        
        using (FileStream fs = new FileStream("Days/" + day.Date+ ".json", FileMode.Create))
        {
            await JsonSerializer.SerializeAsync<Day>(fs, day);
            Console.WriteLine("День сохранен");
        }
    }

    public static async Task<Day> LoadTotalDay() => await LoadDay(DateTime.Today.ToShortDateString());
    
    public static async Task<Day> LoadDay(string date) 
    {
        if (!File.Exists("Days/" + date + ".json"))
            await SaveDay(new Day());
        
        using (FileStream fs = new FileStream("Days/" + date + ".json", FileMode.Open))
        {
            Day totalDay = await JsonSerializer.DeserializeAsync<Day>(fs);
            return totalDay;
        }
    }
}