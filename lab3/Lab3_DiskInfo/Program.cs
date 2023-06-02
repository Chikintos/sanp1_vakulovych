using System.Management;
using System.Text;

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;
Console.ForegroundColor = ConsoleColor.Green;

var menu = 0;
do
{
    Console.WriteLine("\t\t\t\t\t\t\tПрограма \"DiskInfo\"");
    Console.WriteLine("\nМеню:");
    Console.WriteLine("1) Список усіх логічних дисків в системі");
    Console.WriteLine("2) Тип кожного диску присутнього в системі");
    Console.WriteLine("3) Інформація про файлові системи, які використовують диски та про зайняте та вільне місце на кожному з дисків");
    Console.WriteLine("4) Інформація про системну пам'ять");
    Console.WriteLine("5) Інформація про назву комп'ютера");
    Console.WriteLine("6) Назва поточного користувача");
    Console.WriteLine("7) Інформація про поточний системний каталог, тимчасовий каталог, поточний робочий каталог");
    Console.WriteLine("8) Спостерігати за змінами для обраного каталогу");

    Console.Write("\nОберіть пункт меню або 0, щоб завершити роботу програми: ");
    bool flag;
    do
    {
        flag = int.TryParse(Console.ReadLine(), out menu);
        if (!flag || menu is < 0 or > 8)
        {
            Console.WriteLine("\nУпс, помилка введення! Перевірте коректність введеного значення та повторіть введення іще раз!");
        }
    } while (!flag || menu is < 0 or > 8);

    var allDrives = DriveInfo.GetDrives();

    switch (menu)
    {
        case 1:
            foreach (var drive in allDrives)
            {
                Console.WriteLine($"Диск {drive.Name}");
            }
            break;
        case 2:
            foreach (var drive in allDrives)
            {
                switch (drive.DriveType)
                {
                    case DriveType.CDRom:
                        Console.WriteLine($"{drive.Name} - оптичний диск (CD-ROM)");
                        break;
                    case DriveType.Fixed:
                        Console.WriteLine($"{drive.Name} - фіксований диск");
                        break;
                    case DriveType.Network:
                        Console.WriteLine($"{drive.Name} - мережевий диск");
                        break;
                    case DriveType.Removable:
                        Console.WriteLine($"{drive.Name} - знімний диск");
                        break;
                    case DriveType.Ram:
                        Console.WriteLine($"{drive.Name} - RAM диск");
                        break;
                    case DriveType.Unknown:
                    case DriveType.NoRootDirectory:
                    default:
                        Console.WriteLine($"{drive.Name} - невідомий тип диску");
                        break;
                }
            }
            break;
        case 3:
            foreach (var drive in allDrives)
            {
                Console.WriteLine($"Диск {drive.Name}");
                Console.WriteLine($"Доступний: {(drive.IsReady ? "Так" : "Ні")}");

                if (drive.IsReady)
                {
                    Console.WriteLine($"Мітка: {drive.VolumeLabel}");
                    Console.WriteLine($"Файлова система: {drive.DriveFormat}");
                    Console.WriteLine($"Вільне місце: {drive.AvailableFreeSpace / 1024 / 1024} МБ");
                    Console.WriteLine($"Зайняте місце: {drive.TotalSize / 1024 / 1024} МБ\n");
                }
            }
            break;
        case 4:
            Console.WriteLine($"Операційна система: {Environment.OSVersion}");

            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (var obj in searcher.Get())
            {
                var maxPhysicalMemory = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                var freePhysicalMemory = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                var freeVirtualMemory = Convert.ToUInt64(obj["FreeVirtualMemory"]);

                Console.WriteLine($"Максимальна фізична пам'ять: {maxPhysicalMemory / 1024f} МБ");
                Console.WriteLine($"Вільна фізична пам'ять: {freePhysicalMemory / 1024f} МБ");
                Console.WriteLine($"Вільна кількість віртуальної пам'яті: {freeVirtualMemory / 1024f} МБ");
            }
            break;
        case 5:
            var computerName = Environment.MachineName;
            Console.WriteLine($"Назва комп'ютера: {computerName}");
            break;
        case 6:
            var userName = Environment.UserName;
            Console.WriteLine($"Назва поточного користувача: {userName}");
            break;
        case 7:
            var systemFolder = Environment.SystemDirectory;
            var tempFolder = Environment.GetEnvironmentVariable("TEMP");
            var currentFolder = Environment.CurrentDirectory;

            Console.WriteLine($"Поточний системний каталог: {systemFolder}");
            Console.WriteLine($"Тимчасовий каталог: {tempFolder}");
            Console.WriteLine($"Поточний робочий каталог: {currentFolder}");
            break;
        case 8:
            Console.Write("Введіть повний шлях до обраного каталогу для відслідковування змін: ");

            string directoryPath;
            do
            {
                directoryPath = @Console.ReadLine();
                flag = Directory.Exists(directoryPath);

                if (!flag)
                {
                    Console.WriteLine("\nУпс, помилка введення! Перевірте коректність введеного шляху та повторіть введення нижче іще раз!");
                }
            } while (!flag);

            var watcher = new FileSystemWatcher(directoryPath);

            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                                   NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                                   NotifyFilters.Security | NotifyFilters.Size;

            watcher.Filter = "*.*";

            watcher.Changed += (_, e) =>
            {
                if (!File.Exists("log.txt"))
                {
                    using var createLogFileWriter = File.CreateText("log.txt");
                    createLogFileWriter.WriteLine($"Лог-файл створено: {DateTime.Now}");
                }

                Console.WriteLine($"\nФайл або каталог: {e.FullPath} змінено, Час: {DateTime.Now}");

                using var writer = new StreamWriter("log.txt", true);
                writer.WriteLine($"Файл або каталог: {e.FullPath} змінено, Час: {DateTime.Now}");
            };

            watcher.Created += (_, e) =>
            {
                if (!File.Exists("log.txt"))
                {
                    using var createLogFileWriter = File.CreateText("log.txt");
                    createLogFileWriter.WriteLine($"Лог-файл створено: {DateTime.Now}");
                }

                Console.WriteLine($"\nФайл або каталог: {e.FullPath} створено, Час: {DateTime.Now}");

                using var writer = new StreamWriter("log.txt", true);
                writer.WriteLine($"Файл або каталог: {e.FullPath} створено, Час: {DateTime.Now}");
            };

            watcher.Renamed += (_, e) =>
            {
                if (!File.Exists("log.txt"))
                {
                    using var createLogFileWriter = File.CreateText("log.txt");
                    createLogFileWriter.WriteLine($"Лог-файл створено: {DateTime.Now}");
                }

                Console.WriteLine($"\nФайл або каталог: {e.OldFullPath} -> {e.FullPath} перейменовано, Час: {DateTime.Now}");

                using var writer = new StreamWriter("log.txt", true);
                writer.WriteLine($"Файл або каталог: {e.OldFullPath} -> {e.FullPath} перейменовано, Час: {DateTime.Now}");
            };

            watcher.Deleted += (_, e) =>
            {
                if (!File.Exists("log.txt"))
                {
                    using var createLogFileWriter = File.CreateText("log.txt");
                    createLogFileWriter.WriteLine($"Лог-файл створено: {DateTime.Now}");
                }

                Console.WriteLine($"\nФайл або каталог: {e.FullPath} видалено, Час: {DateTime.Now}");

                using var writer = new StreamWriter("log.txt", true);
                writer.WriteLine($"Файл або каталог: {e.FullPath} видалено, Час: {DateTime.Now}");
            };

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("\nСпостереження за змінами включено. Натисніть будь-яку клавішу для виходу.");
            Console.ReadKey();
            break;
    }

    Console.WriteLine("\nНатисніть на будь-яку кнопку, щоб продовжити");
    Console.ReadKey();
    Console.Clear();
} while (menu != 0);