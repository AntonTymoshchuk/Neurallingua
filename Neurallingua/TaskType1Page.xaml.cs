using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Synthesis;
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
    /// Interaction logic for TaskType1Page.xaml
    /// </summary>
    public partial class TaskType1Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private Button correctButton;
        private bool gotVariant = false;

        public TaskType1Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            phraseTextBlock.Text = phrasePair.OriginPhrase;
            Random random = new Random();
            int correctButtonId = random.Next(0, 4);
            switch (correctButtonId)
            {
                case 0:
                    variant1TextBlock.Text = phrasePair.ForeignPhrase;
                    correctButton = variant1Button;
                    break;
                case 1:
                    variant2TextBlock.Text = phrasePair.ForeignPhrase;
                    correctButton = variant2Button;
                    break;
                case 2:
                    variant3TextBlock.Text = phrasePair.ForeignPhrase;
                    correctButton = variant3Button;
                    break;
                case 3:
                    variant4TextBlock.Text = phrasePair.ForeignPhrase;
                    correctButton = variant4Button;
                    break;
            }
            for (int i = 0; i < 4; i++)
            {
                if (i == correctButtonId)
                    continue;
                switch (i)
                {
                    case 0:
                        variant1TextBlock.Text = testingEngine.RandomForeignPhrase(phrasePair.ForeignPhrase);
                        break;
                    case 1:
                        variant2TextBlock.Text = testingEngine.RandomForeignPhrase(phrasePair.ForeignPhrase);
                        break;
                    case 2:
                        variant3TextBlock.Text = testingEngine.RandomForeignPhrase(phrasePair.ForeignPhrase);
                        break;
                    case 3:
                        variant4TextBlock.Text = testingEngine.RandomForeignPhrase(phrasePair.ForeignPhrase);
                        break;
                }
            }
        }

        private void variantButton_Click(object sender, RoutedEventArgs e)
        {
            if (gotVariant == true)
                return;
            gotVariant = true;
            Button variantButton = sender as Button;
            if (variantButton == correctButton)
            {
                variantButton.Background = new SolidColorBrush(Colors.LightGreen);
                phrasePair.IncreaseTimesTested();
            }
            else if (variantButton != correctButton)
            {
                variantButton.Background = new SolidColorBrush(Colors.Pink);
                correctButton.Background = new SolidColorBrush(Colors.LightGreen);
                testingEngine.AddPhrasePairToRepeat(phrasePair);
            }
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            if (gotVariant == false)
                return;
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase, false);
            testingEngine.GoToNextTaskPage(NavigationService);
        }
    }
}
