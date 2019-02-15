using Chip8.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chip8.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WIDTH = 640;
        private const int HEIGHT = 480;

        private readonly ICPU myChip8;

        private bool pause = false;
        private bool quit = false;
        private bool debugKeys = false;
        private bool debugPixels = false;
        private float zoom = 9.5f;

        // For timing..
        readonly Stopwatch stopWatch = Stopwatch.StartNew();
        readonly TimeSpan targetElapsedTime60Hz = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        readonly TimeSpan targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 1000);
        TimeSpan lastTime;

        private static readonly Color ambar = Color.FromArgb(0xFF, 0xFF, 0xB0, 0x00);
        private static readonly Color lightAmbar = Color.FromArgb(0xFF, 0xFF, 0xCC, 0x00);
        private static readonly Color green1 = Color.FromArgb(0xFF, 0x33, 0xFF, 0x00);
        private static readonly Color green2 = Color.FromArgb(0xFF, 0x00, 0xFF, 0x33);
        private static readonly Color green3 = Color.FromArgb(0xFF, 0x00, 0xFF, 0x66);
        private static readonly Color appleIIGreen = Color.FromArgb(0xFF, 0x33, 0xFF, 0x33);
        private static readonly Color appleIIcGreen = Color.FromArgb(0xFF, 0x66, 0xFF, 0x66);
        private static readonly Color gray = Color.FromArgb(0xFF, 0x28, 0x28, 0x28);

        private static readonly byte[] keys =
        {
             0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00
         };
        public MainWindow()
        {
            InitializeComponent();

            var pf = PixelFormats.Bgra32;
            int width = 64;
            int height = 32;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            var screenImage = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            imgScreen.Source = screenImage;

            // Initialize the CHIP - 8 system(Clear the memory, registers and screen)
            myChip8 = new CPU();
            myChip8.OnDraw += MyChip8_OnDraw;

            RenderOptions.SetBitmapScalingMode(imgScreen, BitmapScalingMode.NearestNeighbor);

            //EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);
            //EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);

            const string STARTUP_FILE = "demo";

            var files = Directory.EnumerateFiles("progs", "*.c8");

            foreach (var file in files)
            {
                cbPrograms.Items.Add(Path.GetFileNameWithoutExtension(file));
            }

            cbPrograms.SelectedIndex = cbPrograms.Items.IndexOf(STARTUP_FILE);
            LoadProgram(STARTUP_FILE);
        }

        private void LoadProgram(string filename)
        {
            try
            {
                pause = true;
                myChip8.Reset();

                // Load (copy) the game into the memory
                myChip8.LoadGame($"progs{Path.DirectorySeparatorChar}{filename}.c8");
            }
            finally
            {
                pause = false;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    break;
                case Key.Down:
                    break;
                case Key.Left:
                    break;
                case Key.Right:
                    break;

                case Key.D1:
                    keys[0x0] = 0;
                    Console.WriteLine("1");
                    break;
                case Key.D2:
                    keys[0x1] = 0;
                    Console.WriteLine("2");
                    break;
                case Key.D3:
                    keys[0x2] = 0;
                    Console.WriteLine("3");
                    break;
                case Key.D4:
                    keys[0x3] = 0;
                    Console.WriteLine("4");
                    break;

                case Key.Q:
                    keys[0x4] = 0;
                    Console.WriteLine("q");
                    break;
                case Key.W:
                    keys[0x5] = 0;
                    Console.WriteLine("w");
                    break;
                case Key.E:
                    keys[0x6] = 0;
                    Console.WriteLine("e");
                    break;
                case Key.R:
                    keys[0x7] = 0;
                    Console.WriteLine("r");
                    break;

                case Key.A:
                    keys[0x8] = 0;
                    Console.WriteLine("a");
                    break;
                case Key.S:
                    keys[0x9] = 0;
                    Console.WriteLine("s");
                    break;
                case Key.D:
                    keys[0xA] = 0;
                    Console.WriteLine("d");
                    break;
                case Key.F:
                    keys[0xA] = 0;
                    Console.WriteLine("f");
                    break;

                case Key.Z:
                    keys[0xB] = 0;
                    Console.WriteLine("z");
                    break;
                case Key.Y:
                    keys[0xB] = 0;
                    Console.WriteLine("y");
                    break;
                case Key.X:
                    keys[0xC] = 0;
                    Console.WriteLine("x");
                    break;
                case Key.C:
                    keys[0xD] = 0;
                    Console.WriteLine("c");
                    break;
                case Key.V:
                    keys[0xE] = 0;
                    Console.WriteLine("v");
                    break;

                default:
                    break;
            }

            myChip8.SetKeys(keys);

            e.Handled = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Console.WriteLine("ESCAPE");
                    Console.WriteLine("Quitting...");
                    quit = true;
                    Close();
                    break;

                case Key.Up:
                    Console.WriteLine("UP");
                    break;
                case Key.Down:
                    Console.WriteLine("DOWN");
                    break;
                case Key.Left:
                    Console.WriteLine("LEFT");
                    break;
                case Key.Right:
                    Console.WriteLine("RIGHT");
                    break;

                case Key.D1:
                    keys[0x0] = 1;
                    Console.WriteLine("1");
                    break;
                case Key.D2:
                    keys[0x1] = 1;
                    Console.WriteLine("2");
                    break;
                case Key.D3:
                    keys[0x2] = 1;
                    Console.WriteLine("3");
                    break;
                case Key.D4:
                    keys[0x3] = 1;
                    Console.WriteLine("4");
                    break;

                case Key.Q:
                    keys[0x4] = 1;
                    Console.WriteLine("q");
                    break;
                case Key.W:
                    keys[0x5] = 1;
                    Console.WriteLine("w");
                    break;
                case Key.E:
                    keys[0x6] = 1;
                    Console.WriteLine("e");
                    break;
                case Key.R:
                    keys[0x7] = 1;
                    Console.WriteLine("r");
                    break;

                case Key.A:
                    keys[0x8] = 1;
                    Console.WriteLine("a");
                    break;
                case Key.S:
                    keys[0x9] = 1;
                    Console.WriteLine("s");
                    break;
                case Key.D:
                    keys[0xA] = 1;
                    Console.WriteLine("d");
                    break;
                case Key.F:
                    keys[0xA] = 1;
                    Console.WriteLine("f");
                    break;

                case Key.Z:
                    keys[0xB] = 1;
                    Console.WriteLine("z");
                    break;
                case Key.Y:
                    keys[0xB] = 1;
                    Console.WriteLine("y");
                    break;
                case Key.X:
                    keys[0xC] = 1;
                    Console.WriteLine("x");
                    break;
                case Key.C:
                    keys[0xD] = 1;
                    Console.WriteLine("c");
                    break;
                case Key.V:
                    keys[0xE] = 1;
                    Console.WriteLine("v");
                    break;

                case Key.Back:
                    debugKeys = false;
                    debugPixels = !debugPixels;

                    if (debugPixels)
                    {
                        Console.WriteLine("Entering debug pixel mode");
                    }
                    else
                    {
                        Console.WriteLine("Leaving debug pixel mode");
                    }
                    break;
                case Key.Enter:
                    debugKeys = !debugKeys;
                    debugPixels = false;

                    if (debugKeys)
                    {
                        Console.WriteLine("Entering debug keys mode");
                    }
                    else
                    {
                        Console.WriteLine("Leaving debug keys mode");
                    }
                    break;

                case Key.OemPlus:
                case Key.Add:
                    zoom += 0.5f;
                    Console.WriteLine($"Zoom level: {zoom}x");
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    zoom -= 0.5f;
                    Console.WriteLine($"Zoom level: {zoom}x");
                    break;
                case Key.D0:
                    zoom = 1.0f;
                    Console.WriteLine($"Zoom level: {zoom}x");
                    break;
                default:
                    Console.WriteLine("Default Key Press");
                    break;
            }

            myChip8.SetKeys(keys);

            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartGameLoop();
        }

        private void StartGameLoop()
        {
            Task.Run(GameLoop);
        }

        private Task GameLoop()
        {
            while (!quit)
            {
                var currentTime = stopWatch.Elapsed;
                var elapsedTime = currentTime - lastTime;

                while (elapsedTime >= targetElapsedTime60Hz)
                {
                    Dispatcher.Invoke(Tick60Hz);
                    elapsedTime -= targetElapsedTime60Hz;
                    lastTime += targetElapsedTime60Hz;
                }

                //this.Invoke((Action)Tick);

                Thread.Sleep(targetElapsedTime);
            }

            return Task.CompletedTask;
        }

        //private void Tick() => myChip8.Tick();

        private void Tick60Hz()
        {
            if (!pause)
            {
                myChip8.EmulateCycle();
            }
        }

        void MyChip8_OnDraw(byte[] graphics)
        {
            imgScreen.Dispatcher.Invoke(() =>
            {
                Draw(graphics);
                imgScreen.InvalidateVisual();
            });
        }

        void Draw(byte[] graphics)
        {
            var pf = PixelFormats.Bgra32;
            int width = 64;
            int height = 32;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            for (int index = 0; index < graphics.Length; index++)
            {
                var pixel = graphics[index];
                var color = (pixel > 0) ? appleIIcGreen : Colors.Black;

                rawImage[index * 4] = color.B;
                rawImage[index * 4 + 1] = color.G;
                rawImage[index * 4 + 2] = color.R;
                rawImage[index * 4 + 3] = 255; // Alpha
            }

            var screenImage = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            imgScreen.Source = screenImage;
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadProgram((string)cbPrograms.SelectedItem);
            imgScreen.Focus();
        }
    }
}
