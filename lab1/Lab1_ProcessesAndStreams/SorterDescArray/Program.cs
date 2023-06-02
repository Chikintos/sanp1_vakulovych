using System.IO.MemoryMappedFiles;

namespace SorterDescArray;

internal class Program
{
    private const string FilePath = "data.dat";

    private const string MemoryMappedFileName = "Array";
    private static Mutex ArrayMutex;

    private const string ArrayMutexName = "Global\\Array";

    private static void Main()
    {
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkBlue;

        Console.WriteLine($"Program for sorting an array of numbers from file \"{FilePath}\"");

        for (var i = 0; ; i++)
        {
            Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.DarkBlue : ConsoleColor.DarkGreen;

            Console.WriteLine("\nTo start sorting by desc, press the button \"D\" or \"Esc\" to close the sorter");

            var inputKey = Console.ReadKey();

            if (inputKey.Key == ConsoleKey.D)
            {
                SortArray();

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

    private static void SortArray()
    {
        try
        {
            var memoryMappedFile =
                MemoryMappedFile.OpenExisting(MemoryMappedFileName, MemoryMappedFileRights.ReadWrite);

            var length = 2;
            var savedArray = new int[1];

            for (var i = 1; i < length; i++)
            {
                ArrayMutex = Mutex.OpenExisting(ArrayMutexName);
                if (ArrayMutex == null)
                {
                    return;
                }

                var mutexIsReleased = ArrayMutex.WaitOne(3000);

                if (!mutexIsReleased)
                {
                    Console.WriteLine("\nCurrently, mutex is busy with another process.");

                    Thread.Sleep(2000);
                }
                else
                {
                    using (var reader = memoryMappedFile.CreateViewAccessor(0, 5))
                    {
                        length = reader.ReadInt32(0);
                    }

                    using (var readerWriter = memoryMappedFile.CreateViewAccessor(5, length * 5))
                    {
                        var array = new int[length];
                        readerWriter.ReadArray(0, array, 0, length);

                        if (!array.SequenceEqual(savedArray))
                        {
                            i = 1;
                        }

                        var key = array[i];
                        var j = i - 1;

                        while (j >= 0 && array[j] < key)
                        {
                            array[j + 1] = array[j];
                            j--;
                        }

                        array[j + 1] = key;

                        readerWriter.WriteArray(0, array, 0, length);
                        savedArray = array;
                    }

                    Console.WriteLine($"\nThe number of permutations: {i}");
                }

                ArrayMutex.ReleaseMutex();
                Thread.Sleep(1000);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nError: {exception.Message}");
        }
    }
}