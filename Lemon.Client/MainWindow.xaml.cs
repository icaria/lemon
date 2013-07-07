using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Lemon.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool ignoreNextMouseMove;

        public MainWindow()
        {
            InitializeComponent();
        }

        void ToggleMaximized()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }


        void ButtonMinimiseOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void ButtonMaxRestoreOnClick(object sender, RoutedEventArgs e)
        {
            ToggleMaximized();
        }

        void TryClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //code for moving the windows - from MarkPad

        void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (WindowState == WindowState.Maximized && e.ClickCount != 2) return;

            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                ignoreNextMouseMove = true;
                return;
            }

            DragMove();
        }
        

        void MouseMoveWindow(object sender, MouseEventArgs e)
        {
            if (ignoreNextMouseMove)
            {
                ignoreNextMouseMove = false;
                return;
            }

            if (WindowState != WindowState.Maximized) return;

            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!Header.IsMouseOver) return;

            // Calculate correct left coordinate for multi-screen system
            var mouseX = PointToScreen(Mouse.GetPosition(this)).X;
            var width = RestoreBounds.Width;
            var left = mouseX - width / 2;
            if (left < 0) left = 0;

            // Align left edge to fit the screen
            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            if (left + width > virtualScreenWidth) left = virtualScreenWidth - width;

            Top = 0;
            Left = left;

            WindowState = WindowState.Normal;

            DragMove();
        }
    }
}
