using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace srg_ColorPicker
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KeyboardListener KListener = new KeyboardListener();
        bool leftCtrl = false;
        bool leftShift = false;

        public MainWindow()
        {
            InitializeComponent();
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListener_KeyUp);
            hex.IsReadOnly = true;
            rgb.IsReadOnly = true;
            Color color = Properties.Settings.Default.color;
            hex.Text = $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
            rgb.Text = $"{color.R}, {color.G}, {color.B}";
            preview.Background = new SolidColorBrush(color);
        }

        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            if (args.Key.ToString() == "Space" && leftCtrl && leftShift)
                GetColor();
            if (args.Key.ToString() == "LeftCtrl") leftCtrl = true;
            if (args.Key.ToString() == "LeftShift") leftShift = true;
        }

        void KListener_KeyUp(object sender, RawKeyEventArgs args)
        {
            if (args.Key.ToString() == "LeftCtrl") leftCtrl = false;
            if (args.Key.ToString() == "LeftShift") leftShift = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            KListener.Dispose();
        }

        void GetColor()
        {
            POINT cursor;
            GetCursorPos(out cursor);
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, (int)cursor.X, (int)cursor.Y);
            ReleaseDC(desk, dc);
            Color color = Color.FromArgb(255, (byte)(a & 0xff), (byte)((a >> 8) & 0xff), (byte)((a >> 16) & 0xff));
            hex.Text = $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
            rgb.Text = $"{color.R}, {color.G}, {color.B}";
            preview.Background = new SolidColorBrush(color);
            Properties.Settings.Default.color = color;
            Properties.Settings.Default.Save();
        }

        void Copy(string text)
        {
            Clipboard.SetText(hex.Text);
            copied.Content = $"{text} copied to clipboard";
        }

        private void Hex_Copy(object sender, MouseEventArgs e)
        {
            Copy(hex.Text);
        }

        private void Hex_Copy(object sender, StylusEventArgs e)
        {
            Copy(hex.Text);
        }

        private void Hex_Copy(object sender, TouchEventArgs e)
        {
            Copy(hex.Text);
        }

        private void RGB_Copy(object sender, MouseEventArgs e)
        {
            Copy(rgb.Text);
        }

        private void RGB_Copy(object sender, StylusEventArgs e)
        {
            Copy(rgb.Text);
        }

        private void RGB_Copy(object sender, TouchEventArgs e)
        {
            Copy(rgb.Text);
        }

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);
    }
}
