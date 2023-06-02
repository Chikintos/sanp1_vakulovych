using System.IO.MemoryMappedFiles;

namespace SorterArray;

internal class Program
{
    private const string FilePath = "data.dat";

    private const string MemoryMappedFileName = "Array";
    private static Mutex ArrayMutex;

    private const string ArrayMutexName = "Global\\Array";

    private static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Cyan;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkBlue;

        Console.WriteLine($"Program for sorting an array of numbers from file \"{FilePath}\"");

        for (var i = 0;; i++)
        {
            Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.DarkBlue : ConsoleColor.DarkRed;

            Console.WriteLine("\nTo start sorting, press the button \"S\" or \"Esc\" to close the sorter");

            var inputKey = Console.ReadKey();

            if (inputKey.Key == ConsoleKey.S)
            {
                SortArray();

                Console.WriteLine($"\n\nThe array was sorted and written to the file \"{FilePath}\".");

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
                try
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

                            for (var j = 0; j < length - i; j++)
                            {
                                if (array[j] > array[j + 1])
                                {
                                    Swap(ref array[j], ref array[j + 1]);
                                }
                            }

                            readerWriter.WriteArray(0, array, 0, length);
                            savedArray = array;
                        }

                        Console.WriteLine($"\nThe number of permutations: {i}");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nError caught: {exception.Message}");
                }
                finally
                {
                    ArrayMutex.ReleaseMutex();
                    Thread.Sleep(1000);
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nError caught: {exception.Message}");
        }
    }

    private static void Swap(ref int first, ref int second)
    {
        (first, second) = (second, first);
    }
}