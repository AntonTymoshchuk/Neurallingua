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
        private bool recognitionStarted = false;
        private bool recognitionFinished = false;

        public TaskType9Page(TestingEngine testingEngine)
        {
            InitializeComponent();
            this.testingEngine = testingEngine;
            phrasePair = testingEngine.GetNextTestingPair();
            phraseTextBlock.Text = phrasePair.OriginPhrase;
        }

        private void speakButton_Click(object sender, RoutedEventArgs e)
        {
            if (recognitionStarted == true)
                return;
            recognitionStarted = true;

            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            SpeechRecognitionEngine engine = new SpeechRecognitionEngine(culture);
            engine.SetInputToDefaultAudioDevice();

            GrammarBuilder builder = new GrammarBuilder(GetRecognizablePhrase());
            builder.Culture = culture;
            Grammar grammar = new Grammar(builder);
            engine.LoadGrammar(grammar);

            engine.SpeechRecognized += engine_SpeechRecognized;
            engine.SpeechRecognitionRejected += engine_SpeechRecognitionRejected;
            engine.RecognizeAsync(RecognizeMode.Single);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            if (recognitionFinished == true)
                phrasePair.TimesTested++;
            else if (recognitionFinished == false)
                testingEngine.AddPhrasePairToRepeat(phrasePair);
            testingEngine.ReadPhrase(phrasePair.ForeignPhrase, false);
            testingEngine.GoToNextTaskPage(NavigationService);
        }

        private void engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            recognitionFinished = true;
            speakButton.Background = new SolidColorBrush(Colors.LightGreen);
        }

        private void engine_SpeechRecognitionRejected(object? sender, SpeechRecognitionRejectedEventArgs e)
        {
            speakButton.Background = new SolidColorBrush(Colors.Pink);
        }

        private string GetRecognizablePhrase()
        {
            string phrase = phrasePair.ForeignPhrase;
            string[] items = phrase.Split(new char[] { ';', ',', '(', ')' });
            string recognizablePhrase = string.Empty;
            foreach (string item in items)
                recognizablePhrase += item;
            System.Diagnostics.Debug.Print(recognizablePhrase);
            return recognizablePhrase;
        }
    }
}
