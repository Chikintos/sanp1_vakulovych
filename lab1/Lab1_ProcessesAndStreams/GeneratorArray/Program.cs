using System.IO.MemoryMappedFiles;

namespace GeneratorArray;

internal class Program
{
    private static readonly int[] LengthLimits = { 20, 30 };
    private static readonly int[] RangeLimits = { 10, 100 };

    private const string FilePath = "data.dat";
    private const string MemoryMappedFileName = "Array";

    private static readonly Random Random = new();

    private const string ArrayMutexName = "Global\\Array";
    private static readonly Mutex ArrayMutex = new(false, ArrayMutexName);

    private static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;

        Console.WriteLine("Program for generating an array of numbers");
        Console.WriteLine($"(Length = {LengthLimits[0]}-{LengthLimits[1]}, Range = {RangeLimits[0]}-{RangeLimits[1]})");

        for (var i = 0; ; i++)
        {
            Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.DarkMagenta : ConsoleColor.DarkGreen;

            Console.WriteLine($"\nPlease press the \"G\" key to generate numbers and write them to a file \"{FilePath}\" or \"Esc\" to close the generator");
            var inputKey = Console.ReadKey();

            if (inputKey.Key == ConsoleKey.G)
            {
                var generatedArray = GenerateArray();
                var generatedArrayString = ArrayToString(generatedArray);

                Console.WriteLine($"\n\n{generatedArrayString}");

                Console.WriteLine("\nPlease press the \"Escape\" key to close the program or another key to continue.");
                inputKey = Console.ReadKey();
                if (inputKey.Key == ConsoleKey.Escape)
                {
                    break;
                }

                Console.WriteLine();
            }
            else if (inputKey.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    private static IEnumerable<int>? GenerateArray()
    {
        try
        {
            ArrayMutex.WaitOne();

            var size = Random.Next(LengthLimits[0], LengthLimits[1]);

            var array = new int[size];
            for (var i = 0; i < size; i++)
            {
                array[i] = Random.Next(RangeLimits[0], RangeLimits[1]);
            }

            MemoryMappedFile? memoryMappedFile = null;
            try
            {
                memoryMappedFile =
                    MemoryMappedFile.OpenExisting(MemoryMappedFileName, MemoryMappedFileRights.ReadWrite);
            }
            catch
            {
                memoryMappedFile ??= MemoryMappedFile.CreateFromFile(FilePath, FileMode.Create, MemoryMappedFileName,
                    size * 5 + 5,
                    MemoryMappedFileAccess.ReadWrite);
            }

            using (var writer = memoryMappedFile.CreateViewAccessor(0, size * 5 + 5))
            {
                writer.Write(0, size);
                writer.WriteArray(5, array, 0, array.Length);
            }

            return array;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nError: {exception.Message}");
            return null;
        }
        finally
        {
            ArrayMutex.ReleaseMutex();
            Thread.Sleep(1000);
        }
    }

    private static string ArrayToString(IEnumerable<int>? array)
    {
        if (array == null)
            return "";

        var result = array.Aggregate("Array = {  ", (current, item) => current + $"{item}  ");
        result += "}";

        return result;
    }
}