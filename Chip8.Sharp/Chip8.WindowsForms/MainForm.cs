using Chip8.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8.WindowsForms
{
    public partial class MainForm : Form
    {
        private const int WIDTH = 640;
        private const int HEIGHT = 480;

        private readonly Bitmap screenImage;
        private readonly ICPU myChip8;

        private bool quit = false;
        private bool pause = false;
        private bool debugKeys = false;
        private bool debugPixels = false;
        private float zoom = 9.5f;

        // For timing..
        readonly Stopwatch stopWatch = Stopwatch.StartNew();
        readonly TimeSpan targetElapsedTime60Hz = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        readonly TimeSpan targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 1000);
        TimeSpan lastTime;

        // frame rate
        private DateTime frameDateTime;
        private double averageDeltaTime;

        private static readonly Color ambar =  Color.FromArgb(0xFF, 0xFF, 0xB0, 0x00);
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

        public MainForm()
        {
            InitializeComponent();

            screenImage = new Bitmap(64, 32);
            pbScreen.Image = screenImage;

            // Initialize the CHIP - 8 system(Clear the memory, registers and screen)
            myChip8 = new CPU();
            myChip8.OnDraw += OnDraw;
            myChip8.OnStartSound += OnStartSound;
            myChip8.OnEndSound += OnEndSound;

            const string STARTUP_FILE = "demo";

            var files = Directory.EnumerateFiles("progs", "*.c8");
            
            foreach(var file in files)
            {
                cbPrograms.Items.Add(Path.GetFileNameWithoutExtension(file));
            }

            cbPrograms.SelectedIndex = cbPrograms.Items.IndexOf(STARTUP_FILE);
            LoadProgram(STARTUP_FILE);
        }

        void MyChip8_OnStartSound(int arg1, int arg2, int arg3)
        {
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

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Debug.Print("ESCAPE");
                    Debug.Print("Quitting...");
                    quit = true;
                    Close();
                    break;

                case Keys.Up:
                    Debug.Print("UP");
                    break;
                case Keys.Down:
                    Debug.Print("DOWN");
                    break;
                case Keys.Left:
                    Debug.Print("LEFT");
                    break;
                case Keys.Right:
                    Debug.Print("RIGHT");
                    break;

                case Keys.D1:
                    keys[0x0] = 1;
                    Debug.Print("1");
                    break;
                case Keys.D2:
                    keys[0x1] = 1;
                    Debug.Print("2");
                    break;
                case Keys.D3:
                    keys[0x2] = 1;
                    Debug.Print("3");
                    break;
                case Keys.D4:
                    keys[0x3] = 1;
                    Debug.Print("4");
                    break;

                case Keys.Q:
                    keys[0x4] = 1;
                    Debug.Print("q");
                    break;
                case Keys.W:
                    keys[0x5] = 1;
                    Debug.Print("w");
                    break;
                case Keys.E:
                    keys[0x6] = 1;
                    Debug.Print("e");
                    break;
                case Keys.R:
                    keys[0x7] = 1;
                    Debug.Print("r");
                    break;

                case Keys.A:
                    keys[0x8] = 1;
                    Debug.Print("a");
                    break;
                case Keys.S:
                    keys[0x9] = 1;
                    Debug.Print("s");
                    break;
                case Keys.D:
                    keys[0xA] = 1;
                    Debug.Print("d");
                    break;
                case Keys.F:
                    keys[0xA] = 1;
                    Debug.Print("f");
                    break;

                case Keys.Z:
                    keys[0xB] = 1;
                    Debug.Print("z");
                    break;
                case Keys.Y:
                    keys[0xB] = 1;
                    Debug.Print("y");
                    break;
                case Keys.X:
                    keys[0xC] = 1;
                    Debug.Print("x");
                    break;
                case Keys.C:
                    keys[0xD] = 1;
                    Debug.Print("c");
                    break;
                case Keys.V:
                    keys[0xE] = 1;
                    Debug.Print("v");
                    break;

                case Keys.Back:
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
                case Keys.Enter:
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

                case Keys.Oemplus:
                case Keys.Add:
                    zoom += 0.5f;
                    Debug.Print($"Zoom level: {zoom.ToString(NumberFormatInfo.CurrentInfo)}x");
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    zoom -= 0.5f;
                    Debug.Print($"Zoom level: {zoom.ToString(NumberFormatInfo.CurrentInfo)}x");
                    break;
                case Keys.D0:
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

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    break;
                case Keys.Down:
                    break;
                case Keys.Left:
                    break;
                case Keys.Right:
                    break;

                case Keys.D1:
                    keys[0x0] = 0;
                    Debug.Print("1");
                    break;
                case Keys.D2:
                    keys[0x1] = 0;
                    Debug.Print("2");
                    break;
                case Keys.D3:
                    keys[0x2] = 0;
                    Debug.Print("3");
                    break;
                case Keys.D4:
                    keys[0x3] = 0;
                    Debug.Print("4");
                    break;

                case Keys.Q:
                    keys[0x4] = 0;
                    Debug.Print("q");
                    break;
                case Keys.W:
                    keys[0x5] = 0;
                    Debug.Print("w");
                    break;
                case Keys.E:
                    keys[0x6] = 0;
                    Debug.Print("e");
                    break;
                case Keys.R:
                    keys[0x7] = 0;
                    Debug.Print("r");
                    break;

                case Keys.A:
                    keys[0x8] = 0;
                    Debug.Print("a");
                    break;
                case Keys.S:
                    keys[0x9] = 0;
                    Debug.Print("s");
                    break;
                case Keys.D:
                    keys[0xA] = 0;
                    Debug.Print("d");
                    break;
                case Keys.F:
                    keys[0xA] = 0;
                    Debug.Print("f");
                    break;

                case Keys.Z:
                    keys[0xB] = 0;
                    Debug.Print("z");
                    break;
                case Keys.Y:
                    keys[0xB] = 0;
                    Debug.Print("y");
                    break;
                case Keys.X:
                    keys[0xC] = 0;
                    Debug.Print("x");
                    break;
                case Keys.C:
                    keys[0xD] = 0;
                    Debug.Print("c");
                    break;
                case Keys.V:
                    keys[0xE] = 0;
                    Debug.Print("v");
                    break;

                default:
                    break;
            }

            myChip8.SetKeys(keys);

            e.Handled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
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
                    pbScreen.Invoke((Action)Tick60Hz);
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

        void OnDraw(byte[] graphics)
        {
            if (pbScreen.InvokeRequired)
            {
                pbScreen.Invoke((Action)(() =>
                {
                    Draw(graphics);
                    pbScreen.Refresh();

                    // frame rate
                    DateTime currentDateTime = DateTime.Now;
                    double currentDeltaTime = (currentDateTime - frameDateTime).TotalSeconds;
                    frameDateTime = currentDateTime;
                    averageDeltaTime = averageDeltaTime * 0.9 + currentDeltaTime * 0.1;
                    int frameRate = (int)(1.0 / averageDeltaTime);
                    frameRateStatusLabel.Text = frameRate + " FPS";
                }));
            }
            else
            {
                Draw(graphics);
                pbScreen.Refresh();

                // frame rate
                DateTime currentDateTime = DateTime.Now;
                double currentDeltaTime = (currentDateTime - frameDateTime).TotalSeconds;
                frameDateTime = currentDateTime;
                averageDeltaTime = averageDeltaTime * 0.9 + currentDeltaTime * 0.1;
                int frameRate = (int)(1.0 / averageDeltaTime);
                frameRateStatusLabel.Text = frameRate + " FPS";
            }
        }


        void Draw(byte[] graphics)
        {
            var bits = screenImage.LockBits(new Rectangle(0, 0, screenImage.Width, screenImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int* pointer = (int*)bits.Scan0;

                for (int i = 0; i < graphics.Length; i++)
                {
                    byte pixel = graphics[i];
                    var color = (pixel > 0) ? appleIIcGreen : Color.Black;

                    *pointer = color.ToArgb();
                    pointer++; // 4 bytes (1 int) per pixel
                }
            }

            screenImage.UnlockBits(bits);
        }


        private static void OnStartSound(int channel, int frequency, int duration)
        {
            Console.Beep(frequency, duration);
        }

        private static void OnEndSound(int channel)
        {
            Debug.Print("BEEP!");
        }

        private void cbPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProgram((string)cbPrograms.SelectedItem);
            pbScreen.Focus();
        }
    }
}
