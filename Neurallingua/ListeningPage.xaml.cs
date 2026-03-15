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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Neurallingua
{
    /// <summary>
    /// Interaction logic for ListeningPage.xaml
    /// </summary>
    public partial class ListeningPage : Page
    {
        private TestingEngine testingEngine;

        public ListeningPage(TestingEngine testingEngine)
        {
            InitializeComponent();
            foreignPhraseTextBox.Focus();
            this.testingEngine = testingEngine;
        }

        private void listenButton_Click(object sender, RoutedEventArgs e)
        {
            Button listenButton = sender as Button;
            string phrase = foreignPhraseTextBox.Text;
            string text = listenButton.Content.ToString();
            int index = Convert.ToInt32(text.Split(' ')[1]) - 1;
            testingEngine.ReadPhraseByCertainVoice(phrase, index);
        }

        private void escapeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage(testingEngine));
        }
    }
}
