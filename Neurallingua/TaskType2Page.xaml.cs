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
    /// Interaction logic for TaskType2Page.xaml
    /// </summary>
    public partial class TaskType2Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private bool answerChecked = false;

        public TaskType2Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            phraseTextBlock.Text = phrasePair.OriginPhrase;
            sessionProgressBar.Maximum = testingEngine.Total;
            sessionProgressBar.Value = testingEngine.Progress;
            translationTextBox.Focus();
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
                testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
            }
            else if (answerChecked == true)
                testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
        }

        private void translationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (translationTextBox.Text == phrasePair.ForeignPhrase)
            {
                continueButton.Content = "Продолжить";
                translationTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                phrasePair.IncreaseTimesTested();
                testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
            }
        }
    }
}
