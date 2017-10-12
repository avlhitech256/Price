using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double widthFirstColumn = 150;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (Math.Abs(FirstColumn.Width.Value) <= 0)
            {
                var animation = new GridLengthAnimation
                {
                    From = new GridLength(FirstColumn.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(widthFirstColumn, GridUnitType.Pixel),
                    Duration = TimeSpan.FromMilliseconds(200)
                };

                FirstColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            }
            else
            {
                widthFirstColumn = FirstColumn.ActualWidth;

                GridLengthAnimation animation = new GridLengthAnimation
                {
                    From = new GridLength(FirstColumn.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Pixel),
                    Duration = TimeSpan.FromMilliseconds(200)
                };

                FirstColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            }
        }
    }
}
