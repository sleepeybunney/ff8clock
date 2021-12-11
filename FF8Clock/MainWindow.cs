using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using GameOverlay.Windows;
using GameOverlay.Drawing;

namespace FF8Clock
{
    public partial class MainWindow : Form
    {
        private SolidBrush whiteBrush;
        private SolidBrush blackBrush;
        private Font font;
        private Process process;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            whiteBrush.Dispose();
            blackBrush.Dispose();
            font.Dispose();
        }

        private void Window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            if (process == null || process.HasExited) Environment.Exit(0);

            var gfx = e.Graphics;
            gfx.ClearScene();
            gfx.DrawTextWithBackground(font, whiteBrush, blackBrush, gfx.Width / 2 - 28, 32, DateTime.Now.ToLongTimeString());
        }

        private void Window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (e.RecreateResources)
            {
                whiteBrush.Dispose();
                blackBrush.Dispose();
            }

            whiteBrush = gfx.CreateSolidBrush(255, 255, 255);
            blackBrush = gfx.CreateSolidBrush(0, 0, 0);

            if (e.RecreateResources) return;

            font = gfx.CreateFont("Consolas", 12);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            process = Process.GetProcessesByName("FF8_EN").FirstOrDefault();

            if (process == null)
            {
                MessageBox.Show("FF8 process not found");
                Application.Exit();
                return;
            }

            var gfx = new Graphics()
            {
                TextAntiAliasing = true
            };

            var window = new StickyWindow(process.MainWindowHandle, gfx)
            {
                FPS = 60,
                IsTopmost = true,
                IsVisible = true
            };

            window.SetupGraphics += Window_SetupGraphics;
            window.DrawGraphics += Window_DrawGraphics;
            window.DestroyGraphics += Window_DestroyGraphics;

            window.Create();
            window.Join();
        }
    }
}
