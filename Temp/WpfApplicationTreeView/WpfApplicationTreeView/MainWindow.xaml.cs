﻿using System.Windows;
using System.Collections.ObjectModel;

namespace WpfApplicationTreeView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<CustomItem> OC { get; set; }
        ObservableCollection<CustomItem> ChildOC { get; set; }
        public MainWindow()
        {
            OC = new ObservableCollection<CustomItem>()
            {
                new CustomItem()
                {
                    Name = "Item1",
                    Checked = true,
                    Children = new ObservableCollection<CustomItem>()
                    {
                        new CustomItem() {Name = "SubItem11", Checked = false},
                        new CustomItem() {Name = "SubItem12", Checked = false},
                        new CustomItem() {Name = "SubItem13", Checked = false}
                    }
                },
                new CustomItem()
                {
                    Name = "Item2",
                    Checked = true,
                    Children = new ObservableCollection<CustomItem>()
                    {
                        new CustomItem() {Name = "SubItem21", Checked = true},
                        new CustomItem() {Name = "SubItem22", Checked = true},
                        new CustomItem() {Name = "SubItem23", Checked = true}
                    }
                },
                new CustomItem()
                {
                    Name = "Item3",
                    Checked = true,
                    Children = new ObservableCollection<CustomItem>()
                    {
                        new CustomItem() {Name = "SubItem31", Checked = false},
                        new CustomItem() {Name = "SubItem32", Checked = false},
                        new CustomItem() {Name = "SubItem33", Checked = false}
                    }
                },
                new CustomItem()
                {
                    Name = "Item4",
                    Checked = true,
                    Children = new ObservableCollection<CustomItem>()
                    {
                        new CustomItem() {Name = "SubItem41", Checked = false},
                        new CustomItem() {Name = "SubItem42", Checked = false},
                        new CustomItem() {Name = "SubItem43", Checked = false}
                    }
                }
            };

            InitializeComponent();

            DataContext = OC;
        }

        public void OnCheck()
        {
            ChildOC = new ObservableCollection<CustomItem>() { };
            foreach (CustomItem item in OC)
            {
                if (item.Checked == true)
                {
                    ChildOC.Add(item);
                    foreach (CustomItem subitem in item.Children)
                    {
                        if (subitem.Checked == true)
                        {
                            ChildOC.Add(subitem);
                        }
                    }
                }
            }
            listbox.ItemsSource = ChildOC;
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) { OnCheck(); }
        private void CheckBox_Loaded(object sender, RoutedEventArgs e) { OnCheck(); }
    }

    public class CustomItem
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
        public ObservableCollection<CustomItem> Children { get; set; }
    }
}
