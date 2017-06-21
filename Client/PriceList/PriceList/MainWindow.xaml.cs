using System;
using System.Collections.Generic;
using System.Windows;
using Common.Messenger;
using Domain.DomainContext;
using PriceList.ViewModel.MainWindow;

namespace PriceList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            List<string> picturesList = CreatePicturesList();
            SplashScreen splashScreen = CreateSplashScreen(picturesList);

            InitializeComponent();
            DomainContext = new DomainContext();
            DataContext = new MainWindowViewModel(DomainContext);

            SetDomainContext();

            if (splashScreen != null)
            {
                DateTime now = DateTime.Now.AddSeconds(3);
                splashScreen.Close(TimeSpan.FromSeconds(1));
                while (DateTime.Now < now) { }
            }
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }
        private IMessenger Messenger => DomainContext?.Messenger;

        #endregion

        #region Methods

        private void SetDomainContext()
        {
            TopMenuControl.DomainContext = DomainContext;
            FooterStatusBarControl.DomainContext = DomainContext;
        }

        private List<string> CreatePicturesList()
        {
            List<string> result = new List<string>();

            result.Add("Picture/images01.jpg");
            result.Add("Picture/images02.jpg");
            result.Add("Picture/images03.jpg");
            result.Add("Picture/images04.jpg");
            result.Add("Picture/images05.jpg");
            result.Add("Picture/images06.jpg");
            result.Add("Picture/images07.jpg");
            result.Add("Picture/images08.jpg");
            result.Add("Picture/images09.jpg");
            result.Add("Picture/images10.jpg");
            result.Add("Picture/images11.jpg");
            result.Add("Picture/images12.jpg");
            result.Add("Picture/images12.jpg");
            result.Add("Picture/images13.jpg");
            result.Add("Picture/images14.jpg");
            result.Add("Picture/images15.jpg");
            result.Add("Picture/images16.jpg");
            result.Add("Picture/images17.jpg");
            result.Add("Picture/images18.jpg");
            result.Add("Picture/images19.gif");
            result.Add("Picture/images20.png");
            result.Add("Picture/images21.png");
            result.Add("Picture/images22.png");
            result.Add("Picture/images23.png");
            result.Add("Picture/images24.png");
            result.Add("Picture/images25.png");
            result.Add("Picture/images26.png");
            result.Add("Picture/images27.png");
            result.Add("Picture/images28.png");
            result.Add("Picture/images29.png");
            result.Add("Picture/images30.png");
            result.Add("Picture/images31.png");
            result.Add("Picture/images32.png");
            result.Add("Picture/images33.png");
            result.Add("Picture/images34.png");
            result.Add("Picture/images35.png");
            result.Add("Picture/images36.png");
            result.Add("Picture/images37.png");
            result.Add("Picture/images38.png");
            result.Add("Picture/images39.png");
            result.Add("Picture/images40.png");
            result.Add("Picture/images41.png");
            result.Add("Picture/images42.png");
            result.Add("Picture/images43.jpg");
            result.Add("Picture/images44.jpg");
            result.Add("Picture/images45.png");
            result.Add("Picture/images46.png");
            result.Add("Picture/images47.png");
            result.Add("Picture/images48.png");
            result.Add("Picture/images49.png");
            result.Add("Picture/images50.jpg");
            result.Add("Picture/images50.png");
            result.Add("Picture/images51.png");
            result.Add("Picture/images52.png");
            result.Add("Picture/images53.png");
            result.Add("Picture/images54.png");
            result.Add("Picture/images55.png");
            result.Add("Picture/images56.png");
            result.Add("Picture/images57.jpg");
            result.Add("Picture/images58.png");

            return result;
        }

        private SplashScreen CreateSplashScreen(List<string> picturesList)
        {
            SplashScreen splashScreen = null;
            List<int> indexList = new List<int>();
            bool isNotCreatedSplashScreen = true;
            int currentSeconds = DateTime.Now.Second;

            while (isNotCreatedSplashScreen && indexList.Count < picturesList.Count)
            {
                currentSeconds = DateTime.Now.Second;
                int index = currentSeconds % picturesList.Count;

                if (!indexList.Contains(index))
                {
                    indexList.Add(index);
                    string pathPicture = picturesList[index];
                    splashScreen = new SplashScreen(pathPicture);

                    try
                    {
                        isNotCreatedSplashScreen = false;
                        splashScreen.Show(false);
                    }
                    catch (Exception)
                    {
                        isNotCreatedSplashScreen = true;
                    }

                }

            }

            return splashScreen;
        }
        #endregion
    }
}
