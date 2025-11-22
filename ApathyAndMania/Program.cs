using ApathyAndMania.DayStats;
using System.Text;
using System.Text.Json;

namespace ApathyAndMania
{
    /// <summary>
    /// Точка входа + фронт
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        private static async Task Main(string[] args)
        {
            await DrawStartMenu();
        }

        /// <summary>
        /// Отрисовка стартового меню
        /// </summary>
        /// <param name="param"></param>
        private static async Task DrawStartMenu(string param = "")
        {
            Console.Clear();
            Console.WriteLine(param);

            Day totalDay = await JsonManager.LoadTotalDay();
            await GenerateDayInfo(totalDay);
        }

        /// <summary>
        /// Список всех записок за день
        /// </summary>
        /// <param name="day"></param>
        private static async Task ReadAllNotes(Day day)
        {
            Console.Clear();

            Console.WriteLine("Выберите цифру:\n(0) Назад\n");
            int idx = 0;
            List<NoteChange> newNotesChange = new List<NoteChange>(day.NoteChanges);
            newNotesChange.Reverse();
            foreach (NoteChange noteChange in newNotesChange)
            {
                idx++;
                Console.WriteLine($"({idx}) {noteChange.SaveTime} | {noteChange.Change} | {noteChange.TextNote}");
            }

            string? key = Console.ReadLine();

            if (key == "0")
                await DrawStartMenu();
        }

        /// <summary>
        /// Создаем новую записку для нового дня
        /// </summary>
        private static async Task CreateNewNote(Day day)
        {
            Console.Clear();
            Console.WriteLine("Создание записки для ТЕКУЩЕГО дня.\n(e) Выход");
            Console.WriteLine("Как ты себя чуствуешь?\n" +
                              "(1) Я ХОЧУ СДОХНУТЬ\n" +
                              "(2) Я потерян \n" +
                              "(3) Плохо\n" +
                              "(4) Хорошо\n" +
                              "(5) Очень хорошо\n" +
                              "(6) Меня разрывает!");
            string? mood = Console.ReadLine();

            if (mood == "e")
            {
                await DrawStartMenu();
                return;
            }

            float change = mood switch
            {
                "1" => -3,
                "2" => -2,
                "3" => -1,
                "4" => 1,
                "5" => 2,
                "6" => 3,
                _ => 0
            };

            Console.WriteLine("Почему?");
            string? description = Console.ReadLine();

            await day.AddNewNote(change, description);
            await DrawStartMenu();
        }
        
        /// <summary>
        /// Список всех дней
        /// </summary>
        private static async Task ReadAllDays()
        {
            Console.Clear();

            // Получаем все файлы (дни) из папки Days
            string[] dayFiles = Directory.GetFiles("Days", "*.json", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .ToArray();

            if (dayFiles.Length == 0)
            {
                Console.WriteLine("Нет сохраненных дней.\nНажмите что-нибудь для возврата.");
                Console.ReadLine();
                await DrawStartMenu();
                return;
            }

            Console.WriteLine("Выберите день:\n(0) Назад\n");

            List<Day> loadedDays = new List<Day>();

            for (int i = 0; i < dayFiles.Length; i++)
            {
                try
                {
                    // Читаем файл напрямую для отображения
                    using (FileStream fs = new FileStream(dayFiles[i], FileMode.Open))
                    {
                        Day? day = await JsonSerializer.DeserializeAsync<Day>(fs);
                        if (day != null)
                        {
                            loadedDays.Add(day);
                            Console.WriteLine($"({i + 1}) {day.Date} - Записок: {day.NoteChanges.Count}");
                        }
                    }
                }
                catch
                {
                }
            }

            string? key = Console.ReadLine();

            if (key == "0" || string.IsNullOrEmpty(key))
            {
                await DrawStartMenu();
                return;
            }

            try
            {
                int selectedIndex = int.Parse(key) - 1;
        
                if (selectedIndex >= 0 && selectedIndex < loadedDays.Count)
                    await GenerateDayInfo(loadedDays[selectedIndex]);
                else
                    await ReadAllDays();
            }
            catch
            {
                await ReadAllDays();
            }
        }

        /// <summary>
        /// Универсальное меню для чтения дня
        /// </summary>
        /// <param name="totalDay"></param>
        private static async Task GenerateDayInfo(Day totalDay)
        {
            Console.WriteLine("День: " + totalDay.Date);
            Console.WriteLine("Записок: " + totalDay.NoteChanges.Count);

            GenerateSlider(totalDay);

            Console.WriteLine("(1) Текущие записки | (2) Новая записка | (3) Другие дни");
            string? key = Console.ReadLine();

            if (string.IsNullOrEmpty(key))
            {
                await DrawStartMenu("Введите ключ.");
                return;
            }

            key = key.Normalize();

            switch (key)
            {
                case "1":
                    await ReadAllNotes(totalDay);
                    break;
                case "2":
                    await CreateNewNote(totalDay);
                    break;
                case "3":
                    await ReadAllDays();
                    break;
                default:
                    await DrawStartMenu("Введите ключ.");
                    break;
            }
        }

        /// <summary>
        /// Генерация слайдера
        /// </summary>
        /// <param name="day"></param>
        private static void GenerateSlider(Day day)
        {
            float step = 15;
            float totalValue = 0;
            int sliderLength = 31;

            foreach (NoteChange noteChange in day.NoteChanges)
                totalValue += noteChange.Change;
            step += totalValue;
            int stepInt = (int)MathF.Round(step);

            stepInt = Math.Clamp(stepInt, 0, sliderLength - 1);

            StringBuilder slider = new StringBuilder(new string('-', sliderLength));
            slider[stepInt] = '|';

            int leftValue = sliderLength - 1 - stepInt;
            int rightValue = stepInt;

            string leftNum = leftValue.ToString();
            string rightNum = rightValue.ToString();
            int spacing = sliderLength - leftNum.Length - rightNum.Length;

            Console.WriteLine(leftNum + new string(' ', spacing / 2) + totalValue + new string(' ', spacing / 2) +
                              rightNum);
            Console.WriteLine(slider.ToString());
            Console.WriteLine(new string(' ', spacing / 2) + totalValue + new string(' ', spacing / 2));
            int labelSpacing = sliderLength - "Апатия".Length - "Мания".Length;
            Console.WriteLine($"Апатия{new string(' ', labelSpacing)}Мания");
        }
    }
}