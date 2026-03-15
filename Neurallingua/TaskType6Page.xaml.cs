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
    /// Interaction logic for TaskType6Page.xaml
    /// </summary>
    public partial class TaskType6Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private bool answerChecked = false;
        private bool answerCorrect = false;

        public TaskType6Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            sessionProgressBar.Maximum = testingEngine.Total;
            sessionProgressBar.Value = testingEngine.Progress;
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
            translationTextBox.Focus();
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
                if (text == phrasePair.ForeignPhrase)
                {
                    translationTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                    phrasePair.IncreaseTimesTested();
                }
                else if (text != phrasePair.ForeignPhrase)
                {
                    translationTextBox.Background = new SolidColorBrush(Colors.Pink);
                    answerTextBlock.Text = phrasePair.ForeignPhrase;
                    answerTextBlock.Background = new SolidColorBrush(Colors.LightGreen);
                    testingEngine.AddPhrasePairToRepeat(phrasePair);
                }
                listenButtonTextBlock.Text = phrasePair.OriginPhrase;
                testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
            }
            else if (answerChecked == true)
                testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
        }

        private void translationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (translationTextBox.Text == phrasePair.ForeignPhrase)
            {
                answerCorrect = true;
                continueButton.Content = "Продолжить";
                translationTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                listenButtonTextBlock.Text = phrasePair.OriginPhrase;
                phrasePair.IncreaseTimesTested();
                testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
            }
            if (answerCorrect == true)
                translationTextBox.Text = phrasePair.ForeignPhrase;
        }
    }
}
