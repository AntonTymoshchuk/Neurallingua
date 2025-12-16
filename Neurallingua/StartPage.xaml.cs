using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Neurallingua
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private TestingEngine testingEngine;

        public StartPage(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "Neurallingua phrases.csv";
            string text = (sender as Button).Content.ToString();
            int testsCount = Convert.ToInt32(text.Split([' '])[1]);
            if (testingEngine.DeterminePhrasePairs(filePath, testsCount) == false)
                return;
            testingEngine.GoToNextTaskPage(NavigationService);
        }
    }
}
