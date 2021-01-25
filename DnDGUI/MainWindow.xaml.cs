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

namespace DnDGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> monsterButtons = new();

        public MainWindow()
        {
            InitializeComponent();
            monsterDisplay.ItemsSource = monsterButtons;
            DataReader.Load();


            foreach (var (name, data) in DataReader.EntityData)
            {
                var tempname = name.Split(" ");
                var listName = tempname.ToList();
                var indexName = listName.IndexOf(listName.Find(s => s.Contains(":"))) + 1;
                var beforename = String.Join(" ", tempname[..indexName]);
                var aftername = String.Join(" ", tempname[(indexName + 1)..]);
                Button tempButton = new() {Content = $"{beforename} [{tempname[indexName][0]}] {aftername.ToUpper()}"};

                tempButton.Click += (_, _) => new MonsterStats(data);
                monsterButtons.Add(tempButton);
            }
        }

        private void input_Click(object sender, RoutedEventArgs e)
        {
            DataReader.Save();
        }
    }
}