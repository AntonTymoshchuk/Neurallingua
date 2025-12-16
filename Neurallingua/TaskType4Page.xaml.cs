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
    /// Interaction logic for TaskType4Page.xaml
    /// </summary>
    public partial class TaskType4Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private Button correctButton;
        private Button answerButton;
        private string[] foreignPhrases;
        private bool answerChecked = false;

        public TaskType4Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            foreignPhrases = new string[4];
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            phraseTextBlock.Text = phrasePair.OriginPhrase;
            Random random = new Random();
            int correctButtonId = random.Next(0, 4);
            foreignPhrases[correctButtonId] = phrasePair.ForeignPhrase;
            switch (correctButtonId)
            {
                case 0:
                    correctButton = variant1Button;
                    break;
                case 1:
                    correctButton = variant2Button;
                    break;
                case 2:
                    correctButton = variant3Button;
                    break;
                case 3:
                    correctButton = variant4Button;
                    break;
            }
            for (int i = 0; i < 4; i++)
            {
                if (i == correctButtonId)
                    continue;
                foreignPhrases[i] = testingEngine.RandomForeignPhrase(phrasePair.ForeignPhrase);
            }
        }

        private void variantButton_Click(object sender, RoutedEventArgs e)
        {
            if (answerChecked == true)
                return;
            answerButton = sender as Button;
            string text = answerButton.Content.ToString();
            int index = Convert.ToInt32(text.Split(' ')[1]) - 1;
            testingEngine.ReadPhrase(foreignPhrases[index]);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            if (answerChecked == false && answerButton != null)
            {
                answerChecked = true;
                continueButton.Content = "Продолжить";
                if (answerButton == correctButton)
                {
                    answerButton.Background = new SolidColorBrush(Colors.LightGreen);
                    phrasePair.TimesTested++;
                }
                else if (answerButton != correctButton)
                {
                    answerButton.Background = new SolidColorBrush(Colors.Pink);
                    correctButton.Background = new SolidColorBrush(Colors.LightGreen);
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
