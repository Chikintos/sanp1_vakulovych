using System.IO.MemoryMappedFiles;

namespace DemonstratorArray
{
    public partial class Form1 : Form
    {
        private static MemoryMappedFile memoryMappedFile = null;

        private const string ArrayMutexName = "Global\\Array";
        private static Mutex ArrayMutex;

        private const string MemoryMappedFileName = "Array";

        private static bool _state;

        private int[] _savedArray = new int[1];

        public Form1()
        {
            InitializeComponent();
            demonstratorTimer.Start();
        }

        private void HandleTick(object sender, EventArgs e)
        {
            if (!_state)
            {
                _state = true;
                demonstratorWorker.RunWorkerAsync();
            }
        }

        private void StartDemo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                ArrayMutex = Mutex.OpenExisting(ArrayMutexName);
                if (ArrayMutex == null)
                    return;

                var mutexIsReleased = ArrayMutex.WaitOne(3000);
                if (!mutexIsReleased)
                {
                    return;
                }

                memoryMappedFile = MemoryMappedFile.OpenExisting(MemoryMappedFileName, MemoryMappedFileRights.ReadWrite);

                int length;
                using (var reader = memoryMappedFile.CreateViewAccessor(0, 5))
                {
                    length = reader.ReadInt32(0);
                }

                var array = new int[length];
                using (var reader = memoryMappedFile.CreateViewAccessor(5, length * 5))
                {
                    reader.ReadArray(0, array, 0, length);

                    if (!array.SequenceEqual(_savedArray))
                    {
                        demonstratorListBox.Items.Clear();
                        foreach (var item in array)
                        {
                            demonstratorListBox.Items.Add(NumberToDots(item));
                        }
                    }
                    _savedArray = array;
                }
            }
            catch
            {
                if (demonstratorListBox.Items[0].ToString() != "(empty)")
                {
                    demonstratorListBox.Items.Clear();
                    demonstratorListBox.Items.Add("(empty)");
                }
            }
            finally
            {
                ArrayMutex.ReleaseMutex();
            }
        }

        private void StopDemo(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            _state = false;
        }

        private string NumberToDots(int number)
        {
            var result = "";
            for (var i = 0; i < number; i++)
            {
                result += "•";
            }

            return result;
        }
    }
}