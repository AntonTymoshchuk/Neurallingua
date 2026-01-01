using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
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
    /// Interaction logic for StudyingPage.xaml
    /// </summary>
    public partial class StudyingPage : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private Thread thread;

        public StudyingPage(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.LastPhrasePair;
            foreignPhraseTextBlock.Text = phrasePair.ForeignPhrase;
            originPhraseTextBlock.Text = phrasePair.OriginPhrase;
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
        }

        private void listenButton_Click(object sender, RoutedEventArgs e)
        {
            Button listenButton = sender as Button;
            string phrase = phrasePair.ForeignPhrase;
            string text = listenButton.Content.ToString();
            int index = Convert.ToInt32(text.Split(' ')[1]) - 1;
            System.Diagnostics.Debug.Print(index.ToString());
            testingEngine.ReadPhraseByCertainVoice(phrase, index);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
        }
    }
}
