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
    /// Interaction logic for TaskType8Page.xaml
    /// </summary>
    public partial class TaskType8Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private bool answerChecked = false;

        public TaskType8Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
        }

        private void listenButton_Click(object sender, RoutedEventArgs e)
        {
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            if (answerChecked == false)
            {
                answerChecked = true;
                string text = translationTextBox.Text;
                continueButton.Content = "Продолжить";
                if (text == phrasePair.OriginPhrase)
                {
                    translationTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                    phrasePair.TimesTested++;
                }
                else if (text != phrasePair.OriginPhrase)
                {
                    translationTextBox.Background = new SolidColorBrush(Colors.Pink);
                    answerTextBlock.Text = phrasePair.OriginPhrase;
                    answerTextBlock.Background = new SolidColorBrush(Colors.LightGreen);
                    testingEngine.AddPhrasePairToRepeat(phrasePair);
                }
                testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
            }
            else if (answerChecked == true)
            {
                testingEngine.ReadPhrase(phrasePair.ForeignPhrase, false);
                testingEngine.GoToNextTaskPage(NavigationService);
            }
        }
    }
}
