using Chip8.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
        private readonly CPU myChip8;

        private bool quit = false;
        private bool debugKeys = false;
        private bool debugPixels = false;
        private float zoom = 9.5f;

        // For timing..
        readonly Stopwatch stopWatch = Stopwatch.StartNew();
        readonly TimeSpan targetElapsedTime60Hz = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        readonly TimeSpan targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 1000);
        TimeSpan lastTime;

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

            // Load (copy) the game into the memory
            myChip8.LoadGame($"progs{Path.DirectorySeparatorChar}demo.c8");
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Console.WriteLine("ESCAPE");
                    Console.WriteLine("Quitting...");
                    quit = true;
                    Close();
                    break;

                case Keys.Up:
                    Console.WriteLine("UP");
                    break;
                case Keys.Down:
                    Console.WriteLine("DOWN");
                    break;
                case Keys.Left:
                    Console.WriteLine("LEFT");
                    break;
                case Keys.Right:
                    Console.WriteLine("RIGHT");
                    break;

                case Keys.D1:
                    keys[0x0] = 1;
                    Console.WriteLine("1");
                    break;
                case Keys.D2:
                    keys[0x1] = 1;
                    Console.WriteLine("2");
                    break;
                case Keys.D3:
                    keys[0x2] = 1;
                    Console.WriteLine("3");
                    break;
                case Keys.D4:
                    keys[0x3] = 1;
                    Console.WriteLine("4");
                    break;

                case Keys.Q:
                    keys[0x4] = 1;
                    Console.WriteLine("q");
                    break;
                case Keys.W:
                    keys[0x5] = 1;
                    Console.WriteLine("w");
                    break;
                case Keys.E:
                    keys[0x6] = 1;
                    Console.WriteLine("e");
                    break;
                case Keys.R:
                    keys[0x7] = 1;
                    Console.WriteLine("r");
                    break;

                case Keys.A:
                    keys[0x8] = 1;
                    Console.WriteLine("a");
                    break;
                case Keys.S:
                    keys[0x9] = 1;
                    Console.WriteLine("s");
                    break;
                case Keys.D:
                    keys[0xA] = 1;
                    Console.WriteLine("d");
                    break;
                case Keys.F:
                    keys[0xA] = 1;
                    Console.WriteLine("f");
                    break;

                case Keys.Z:
                    keys[0xB] = 1;
                    Console.WriteLine("z");
                    break;
                case Keys.Y:
                    keys[0xB] = 1;
                    Console.WriteLine("y");
                    break;
                case Keys.X:
                    keys[0xC] = 1;
                    Console.WriteLine("x");
                    break;
                case Keys.C:
                    keys[0xD] = 1;
                    Console.WriteLine("c");
                    break;
                case Keys.V:
                    keys[0xE] = 1;
                    Console.WriteLine("v");
                    break;

                case Keys.Back:
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
                case Keys.Enter:
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

                case Keys.Oemplus:
                case Keys.Add:
                    zoom += 0.5f;
                    Console.WriteLine($"Zoom level: {zoom}x");
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    zoom -= 0.5f;
                    Console.WriteLine($"Zoom level: {zoom}x");
                    break;
                case Keys.D0:
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
                    Console.WriteLine("1");
                    break;
                case Keys.D2:
                    keys[0x1] = 0;
                    Console.WriteLine("2");
                    break;
                case Keys.D3:
                    keys[0x2] = 0;
                    Console.WriteLine("3");
                    break;
                case Keys.D4:
                    keys[0x3] = 0;
                    Console.WriteLine("4");
                    break;

                case Keys.Q:
                    keys[0x4] = 0;
                    Console.WriteLine("q");
                    break;
                case Keys.W:
                    keys[0x5] = 0;
                    Console.WriteLine("w");
                    break;
                case Keys.E:
                    keys[0x6] = 0;
                    Console.WriteLine("e");
                    break;
                case Keys.R:
                    keys[0x7] = 0;
                    Console.WriteLine("r");
                    break;

                case Keys.A:
                    keys[0x8] = 0;
                    Console.WriteLine("a");
                    break;
                case Keys.S:
                    keys[0x9] = 0;
                    Console.WriteLine("s");
                    break;
                case Keys.D:
                    keys[0xA] = 0;
                    Console.WriteLine("d");
                    break;
                case Keys.F:
                    keys[0xA] = 0;
                    Console.WriteLine("f");
                    break;

                case Keys.Z:
                    keys[0xB] = 0;
                    Console.WriteLine("z");
                    break;
                case Keys.Y:
                    keys[0xB] = 0;
                    Console.WriteLine("y");
                    break;
                case Keys.X:
                    keys[0xC] = 0;
                    Console.WriteLine("x");
                    break;
                case Keys.C:
                    keys[0xD] = 0;
                    Console.WriteLine("c");
                    break;
                case Keys.V:
                    keys[0xE] = 0;
                    Console.WriteLine("v");
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
                    this.Invoke((Action)Tick60Hz);
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
            myChip8.EmulateCycle();

            if (myChip8.DrawFlag)
            {
                Draw(zoom);
            }

            pbScreen.Refresh();
        }

        void Draw(double zoom)
        {
            var bits = screenImage.LockBits(new Rectangle(0, 0, screenImage.Width, screenImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* pointer = (byte*)bits.Scan0;

                foreach (byte pixel in myChip8.Graphics)
                {
                    var color = (pixel > 0) ? appleIIcGreen : Color.Black;

                    pointer[0] = color.B;
                    pointer[1] = color.G;
                    pointer[2] = color.R;
                    pointer[3] = 255; // Alpha

                    pointer += 4; // 4 bytes per pixel
                }
            }

            screenImage.UnlockBits(bits);
        }
    }
}
