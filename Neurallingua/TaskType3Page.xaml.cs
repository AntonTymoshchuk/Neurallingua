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
    /// Interaction logic for TaskType3Page.xaml
    /// </summary>
    public partial class TaskType3Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private Button correctButton;
        private bool gotVariant = false;

        public TaskType3Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            Random random = new Random();
            int correctButtonId = random.Next(0, 4);
            switch (correctButtonId)
            {
                case 0:
                    variant1TextBlock.Text = phrasePair.OriginPhrase;
                    correctButton = variant1Button;
                    break;
                case 1:
                    variant2TextBlock.Text = phrasePair.OriginPhrase;
                    correctButton = variant2Button;
                    break;
                case 2:
                    variant3TextBlock.Text = phrasePair.OriginPhrase;
                    correctButton = variant3Button;
                    break;
                case 3:
                    variant4TextBlock.Text = phrasePair.OriginPhrase;
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
                        variant1TextBlock.Text = testingEngine.RandomOriginPhrase(phrasePair.OriginPhrase);
                        break;
                    case 1:
                        variant2TextBlock.Text = testingEngine.RandomOriginPhrase(phrasePair.OriginPhrase);
                        break;
                    case 2:
                        variant3TextBlock.Text = testingEngine.RandomOriginPhrase(phrasePair.OriginPhrase);
                        break;
                    case 3:
                        variant4TextBlock.Text = testingEngine.RandomOriginPhrase(phrasePair.OriginPhrase);
                        break;
                }
            }
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
        }

        private void listenButton_Click(object sender, RoutedEventArgs e)
        {
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase);
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
