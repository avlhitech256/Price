using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;


namespace GridAnimationDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();

        }

        bool bSingleImageMode = false;

        void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image != null)
            {                
                int col = Grid.GetColumn(image);
                int row = Grid.GetRow(image);

                for (int indexRow = 0; indexRow < mainGrid.RowDefinitions.Count; indexRow++)
                {
                    if (indexRow != row)
                    {
                        GridLengthAnimation gla = new GridLengthAnimation();
                        gla.From = new GridLength(bSingleImageMode ? 0 : 1, GridUnitType.Star);
                        gla.To = new GridLength(bSingleImageMode ? 1 : 0, GridUnitType.Star); ;
                        gla.Duration = new TimeSpan(0, 0, 2);
                        mainGrid.RowDefinitions[indexRow].BeginAnimation(RowDefinition.HeightProperty, gla);
                    }
                  
                }

                for (int indexCol = 0; indexCol < mainGrid.ColumnDefinitions.Count; indexCol++)
                {
                    if (indexCol != col)
                    {
                        GridLengthAnimation gla = new GridLengthAnimation();
                        gla.From = new GridLength(bSingleImageMode ? 0 : 1, GridUnitType.Star);
                        gla.To = new GridLength(bSingleImageMode ? 1 : 0, GridUnitType.Star);
                        gla.Duration = new TimeSpan(0, 0, 2);
                        mainGrid.ColumnDefinitions[indexCol].BeginAnimation(ColumnDefinition.WidthProperty, gla);
                    }                  
                }
            }
            bSingleImageMode = !bSingleImageMode;
        }

    }
}