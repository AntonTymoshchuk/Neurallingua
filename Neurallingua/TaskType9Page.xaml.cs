using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Speech.Recognition;
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
    /// Interaction logic for TaskType9Page.xaml
    /// </summary>
    public partial class TaskType9Page : Page
    {
        private TestingEngine testingEngine;
        private PhrasePair phrasePair;
        private CultureInfo culture;
        private SpeechRecognitionEngine recognitionEngine;
        private bool recognitionFinished = false;

        public TaskType9Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.CurrentPhrasePair;
            phraseTextBlock.Text = phrasePair.OriginPhrase;
            sessionProgressBar.Maximum = testingEngine.Total;
            sessionProgressBar.Value = testingEngine.Progress;
            culture = CultureInfo.GetCultureInfo("fr-FR");
        }

        private void speakButton_Click(object sender, RoutedEventArgs e)
        {
            if (recognitionEngine != null)
                recognitionEngine.RecognizeAsyncCancel();

            recognitionEngine = new SpeechRecognitionEngine(culture);
            recognitionEngine.SetInputToDefaultAudioDevice();

            GrammarBuilder builder = new GrammarBuilder(GetRecognizablePhrase());
            builder.Culture = culture;
            Grammar grammar = new Grammar(builder);
            recognitionEngine.LoadGrammar(grammar);

            recognitionEngine.SpeechRecognized += engine_SpeechRecognized;
            recognitionEngine.SpeechRecognitionRejected += engine_SpeechRecognitionRejected;
            recognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            if (recognitionFinished == true)
                phrasePair.IncreaseTimesTested();
            else if (recognitionFinished == false)
            {
                phraseTextBlock.Text = string.Format("{0}\n\n{1}",
                    phrasePair.OriginPhrase, phrasePair.ForeignPhrase);
                testingEngine.AddPhrasePairToRepeat(phrasePair);
            }
            testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
        }

        private void engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            recognitionFinished = true;
            phrasePair.IncreaseTimesTested();
            speakButton.Background = new SolidColorBrush(Colors.LightGreen);
            phraseTextBlock.Text += "\n\n" + phrasePair.ForeignPhrase;
            testingEngine.EndUpWithTaskPage(Dispatcher, NavigationService);
        }

        private void engine_SpeechRecognitionRejected(object? sender, SpeechRecognitionRejectedEventArgs e)
        {
            speakButton.Background = new SolidColorBrush(Colors.Pink);
            phraseTextBlock.Text += "\n\n" + phrasePair.ForeignPhrase;
        }

        private string GetRecognizablePhrase()
        {
            string phrase = phrasePair.ForeignPhrase;
            string[] items = phrase.Split(
                [';', ',', ':', '«', '»', '(', ')', '?', '!']);
            string recognizablePhrase = string.Empty;
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = items[i].Trim();
                recognizablePhrase += string.Format("{0} ", items[i]);
            }
            recognizablePhrase = recognizablePhrase.Trim();
            return recognizablePhrase;
        }
    }
}
