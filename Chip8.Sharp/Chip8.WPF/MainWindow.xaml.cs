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
using System.Globalization;

namespace Chip8.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
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
            myChip8 = new CPU((uint)((appleIIcGreen.A << 24) | (appleIIcGreen.R << 16) | (appleIIcGreen.G << 8) | appleIIcGreen.B));
            myChip8.OnDraw += OnDraw;
            myChip8.OnStartSound += OnStartSound;
            myChip8.OnEndSound += OnEndSound;

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
                myChip8.LoadGame($"progs{Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture)}{filename}.c8");
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
                    Debug.Print("1");
                    break;
                case Key.D2:
                    keys[0x1] = 0;
                    Debug.Print("2");
                    break;
                case Key.D3:
                    keys[0x2] = 0;
                    Debug.Print("3");
                    break;
                case Key.D4:
                    keys[0x3] = 0;
                    Debug.Print("4");
                    break;

                case Key.Q:
                    keys[0x4] = 0;
                    Debug.Print("q");
                    break;
                case Key.W:
                    keys[0x5] = 0;
                    Debug.Print("w");
                    break;
                case Key.E:
                    keys[0x6] = 0;
                    Debug.Print("e");
                    break;
                case Key.R:
                    keys[0x7] = 0;
                    Debug.Print("r");
                    break;

                case Key.A:
                    keys[0x8] = 0;
                    Debug.Print("a");
                    break;
                case Key.S:
                    keys[0x9] = 0;
                    Debug.Print("s");
                    break;
                case Key.D:
                    keys[0xA] = 0;
                    Debug.Print("d");
                    break;
                case Key.F:
                    keys[0xA] = 0;
                    Debug.Print("f");
                    break;

                case Key.Z:
                    keys[0xB] = 0;
                    Debug.Print("z");
                    break;
                case Key.Y:
                    keys[0xB] = 0;
                    Debug.Print("y");
                    break;
                case Key.X:
                    keys[0xC] = 0;
                    Debug.Print("x");
                    break;
                case Key.C:
                    keys[0xD] = 0;
                    Debug.Print("c");
                    break;
                case Key.V:
                    keys[0xE] = 0;
                    Debug.Print("v");
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
                    Debug.Print("ESCAPE");
                    Debug.Print("Quitting...");
                    quit = true;
                    Close();
                    break;

                case Key.Up:
                    Debug.Print("UP");
                    break;
                case Key.Down:
                    Debug.Print("DOWN");
                    break;
                case Key.Left:
                    Debug.Print("LEFT");
                    break;
                case Key.Right:
                    Debug.Print("RIGHT");
                    break;

                case Key.D1:
                    keys[0x0] = 1;
                    Debug.Print("1");
                    break;
                case Key.D2:
                    keys[0x1] = 1;
                    Debug.Print("2");
                    break;
                case Key.D3:
                    keys[0x2] = 1;
                    Debug.Print("3");
                    break;
                case Key.D4:
                    keys[0x3] = 1;
                    Debug.Print("4");
                    break;

                case Key.Q:
                    keys[0x4] = 1;
                    Debug.Print("q");
                    break;
                case Key.W:
                    keys[0x5] = 1;
                    Debug.Print("w");
                    break;
                case Key.E:
                    keys[0x6] = 1;
                    Debug.Print("e");
                    break;
                case Key.R:
                    keys[0x7] = 1;
                    Debug.Print("r");
                    break;

                case Key.A:
                    keys[0x8] = 1;
                    Debug.Print("a");
                    break;
                case Key.S:
                    keys[0x9] = 1;
                    Debug.Print("s");
                    break;
                case Key.D:
                    keys[0xA] = 1;
                    Debug.Print("d");
                    break;
                case Key.F:
                    keys[0xA] = 1;
                    Debug.Print("f");
                    break;

                case Key.Z:
                    keys[0xB] = 1;
                    Debug.Print("z");
                    break;
                case Key.Y:
                    keys[0xB] = 1;
                    Debug.Print("y");
                    break;
                case Key.X:
                    keys[0xC] = 1;
                    Debug.Print("x");
                    break;
                case Key.C:
                    keys[0xD] = 1;
                    Debug.Print("c");
                    break;
                case Key.V:
                    keys[0xE] = 1;
                    Debug.Print("v");
                    break;

                case Key.Back:
                    debugKeys = false;
                    debugPixels = !debugPixels;

                    if (debugPixels)
                    {
                        Debug.Print("Entering debug pixel mode");
                    }
                    else
                    {
                        Debug.Print("Leaving debug pixel mode");
                    }
                    break;
                case Key.Enter:
                    debugKeys = !debugKeys;
                    debugPixels = false;

                    if (debugKeys)
                    {
                        Debug.Print("Entering debug keys mode");
                    }
                    else
                    {
                        Debug.Print("Leaving debug keys mode");
                    }
                    break;

                case Key.OemPlus:
                case Key.Add:
                    zoom += 0.5f;
                    Debug.Print($"Zoom level: {zoom.ToString(NumberFormatInfo.CurrentInfo)}x");
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    zoom -= 0.5f;
                    Debug.Print($"Zoom level: {zoom.ToString(NumberFormatInfo.CurrentInfo)}x");
                    break;
                case Key.D0:
                    zoom = 1.0f;
                    Debug.Print($"Zoom level: {zoom.ToString(NumberFormatInfo.CurrentInfo)}x");
                    break;
                default:
                    Debug.Print("Default Key Press");
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
                clockRateStatusLabel.Content = $"{myChip8.ClockRate.ToString("F1")} Hz (avg: {(myChip8.ProcessingTime / 10.0).ToString("#,##0.00", NumberFormatInfo.CurrentInfo)} ns)";
                frameRateStatusLabel.Content = $"{myChip8.FrameRate.ToString("F1")} FPS (avg: {(myChip8.RenderingTime / 10.0).ToString("#,##0.00", NumberFormatInfo.CurrentInfo)} ns)";
            }
        }

        void OnDraw(byte[] graphics)
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

            var screenImage = BitmapSource.Create(width, height, 96, 96, pf, null, graphics, rawStride);
            imgScreen.Source = screenImage;
        }

        private static void OnStartSound(int channel, int frequency, int duration)
        {
            Console.Beep(frequency, duration);
        }

        private static void OnEndSound(int channel)
        {
            Debug.Print("BEEP!");
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadProgram((string)cbPrograms.SelectedItem);
            imgScreen.Focus();
        }

        public void Dispose()
        {
            myChip8?.Dispose();
        }
    }
}
