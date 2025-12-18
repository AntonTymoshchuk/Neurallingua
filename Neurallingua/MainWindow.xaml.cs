using System.Globalization;
using System.IO;
using System.Speech.Synthesis;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestingEngine testingEngine;

        public TestingEngine TestingEngine
        {  get { return testingEngine; } }

        public MainWindow()
        {
            InitializeComponent();
            testingEngine = new TestingEngine();
            mainFrame.Content = new StartPage(testingEngine);
        }
    }

    public class PhrasePair
    {
        private string foreignPhrase;
        private string originPhrase;
        private int timesTested;
        private DateTime testedDate;

        public string ForeignPhrase
        {
            get { return foreignPhrase; }
        }

        public string OriginPhrase
        {
            get { return originPhrase; }
        }

        public int TimesTested
        {
            get { return timesTested; }
            set { timesTested = value; }
        }

        public DateTime TestedDate
        {
            get { return testedDate; }
            set { testedDate = value; }
        }

        public PhrasePair(string foreignPhrase, string originPhrase, int timesTested)
        {
            this.foreignPhrase = foreignPhrase;
            this.originPhrase = originPhrase;
            this.timesTested = timesTested;
        }

        public void IncreaseTimesTested()
        {
            timesTested++;
            testedDate = DateTime.Now;
        }

        public override string ToString()
        {
            string s = string.Format("{0}|{1}|{2}",
                foreignPhrase, originPhrase, timesTested);
            if (timesTested > 0)
                s = string.Format("{0}|{1}", s, testedDate.ToString());
            return s;
        }
    }

    public class TestingEngine
    {
        private const string phrasesPath = "phrases.csv";
        private int testsCount;
        private int totalTestsCount;
        private int lessTestedValue;
        private List<PhrasePair> allPhrasePairs;
        private List<PhrasePair> testingPhrasePairs;
        private List<PhrasePair> testedPhrasePairs;
        private List<PhrasePair> randPhrasePairs;
        SpeechSynthesizer synthesizer;

        private const string settingsPath = "settings.csv";
        private bool applyForgetting = true;
        private bool forgettingByTest = true;
        private bool forgettingByDay = false;

        public int Total
        { get { return totalTestsCount; } }

        public int Progress
        { get { return totalTestsCount - testsCount; } }

        public TestingEngine()
        {
            allPhrasePairs = new List<PhrasePair>();
            testingPhrasePairs = new List<PhrasePair>();
            testedPhrasePairs = new List<PhrasePair>();
            randPhrasePairs = new List<PhrasePair>();
            synthesizer = new SpeechSynthesizer();
            ReadAndApplySettings();
        }

        public bool DeterminePhrasePairs(int testsCount)
        {
            ClearPhrasePairsLists();
            this.testsCount = testsCount;

            if (File.Exists(phrasesPath) == false)
            {
                MessageBox.Show("Файл phrases.csv не существует");
                return false;
            }

            string[] strings = File.ReadAllLines(phrasesPath);
            string[] items;
            string foreignPhrase, originPhrase;
            int timesTested;
            foreach (string s in strings)
            {
                items = s.Split('|');
                try
                {
                    foreignPhrase = items[0];
                    originPhrase = items[1];
                    timesTested = Convert.ToInt32(items[2]);
                    PhrasePair phrasePair = new PhrasePair(
                        foreignPhrase, originPhrase, timesTested);
                    if (items.Length == 4)
                        phrasePair.TestedDate = DateTime.Parse(items[3]);
                    allPhrasePairs.Add(phrasePair);
                    randPhrasePairs.Add(phrasePair);
                }
                catch
                {
                    MessageBox.Show("Ошибка в DeterminePhrasePairs");
                    return false;
                }
            }
            if (this.testsCount > allPhrasePairs.Count)
                this.testsCount = allPhrasePairs.Count;
            totalTestsCount = this.testsCount;

            lessTestedValue = allPhrasePairs[0].TimesTested;
            PhrasePair lessTestedPair;
            int iteration = 0;
            while (iteration < this.testsCount)
            {
                if (allPhrasePairs.Count == 0)
                    break;
                lessTestedPair = allPhrasePairs[0];
                foreach (PhrasePair phrasePair in allPhrasePairs)
                {
                    if (phrasePair != lessTestedPair &&
                        phrasePair.TimesTested < lessTestedPair.TimesTested)
                        lessTestedPair = phrasePair;
                    if (phrasePair.TimesTested < lessTestedValue)
                        lessTestedValue = phrasePair.TimesTested;
                }
                testingPhrasePairs.Add(lessTestedPair);
                allPhrasePairs.Remove(lessTestedPair);
                iteration++;
            }
            if (applyForgetting == true)
            {
                foreach (PhrasePair phrasePair in allPhrasePairs)
                {
                    if (forgettingByTest == true && phrasePair.TimesTested > 0)
                        phrasePair.TimesTested--;
                    if (forgettingByDay == true && phrasePair.TimesTested > 0)
                        phrasePair.TimesTested -= (DateTime.Now - phrasePair.TestedDate).Days;
                }
            }

            return true;
        }

        public void ReadAndApplySettings()
        {
            if (File.Exists(settingsPath) == false)
            {
                MessageBox.Show("Файл settings.csv не существует");
                return;
            }

            string[] settings = File.ReadAllLines(settingsPath);
            try
            {
                applyForgetting = Convert.ToBoolean(settings[0].Split(' ')[1]);
                forgettingByTest = Convert.ToBoolean(settings[1].Split(' ')[1]);
                forgettingByDay = Convert.ToBoolean(settings[2].Split(' ')[1]);
            }
            catch
            {
                MessageBox.Show("Ошибка в ReadAndApplySettings");
            }
        }

        public PhrasePair GetNextTestingPair()
        {
            Random random = new Random();
            int index = random.Next(0, testingPhrasePairs.Count);
            PhrasePair testingPair = testingPhrasePairs[index];
            testingPhrasePairs.Remove(testingPair);
            testedPhrasePairs.Add(testingPair);
            testsCount--;
            return testingPair;
        }

        public void AddPhrasePairToRepeat(PhrasePair phrasePair)
        {
            phrasePair.TimesTested = lessTestedValue;
            testingPhrasePairs.Add(phrasePair);
            testedPhrasePairs.Remove(phrasePair);
            testsCount++;
        }

        private void SaveTestedPhrases()
        {
            allPhrasePairs.AddRange(testedPhrasePairs);
            allPhrasePairs.AddRange(testingPhrasePairs);
            List<string> strings = new List<string>();
            foreach (PhrasePair pair in allPhrasePairs)
                strings.Add(pair.ToString());
            File.WriteAllLines(phrasesPath, strings.ToArray());
        }

        private void ClearPhrasePairsLists()
        {
            allPhrasePairs.Clear();
            testingPhrasePairs.Clear();
            testedPhrasePairs.Clear();
            randPhrasePairs.Clear();
        }

        public void GoToNextTaskPage(NavigationService navigationService)
        {
            if (testsCount == 0)
            {
                SaveTestedPhrases();
                navigationService.Navigate(new StartPage(this));
                return;
            }
            Random random = new Random();
            int taskType = random.Next(0, 9);
            switch (taskType)
            {
                case 0:
                    navigationService.Navigate(new TaskType1Page(this));
                    break;
                case 1:
                    navigationService.Navigate(new TaskType2Page(this));
                    break;
                case 2:
                    navigationService.Navigate(new TaskType3Page(this));
                    break;
                case 3:
                    navigationService.Navigate(new TaskType4Page(this));
                    break;
                case 4:
                    navigationService.Navigate(new TaskType5Page(this));
                    break;
                case 5:
                    navigationService.Navigate(new TaskType6Page(this));
                    break;
                case 6:
                    navigationService.Navigate(new TaskType7Page(this));
                    break;
                case 7:
                    navigationService.Navigate(new TaskType8Page(this));
                    break;
                case 8:
                    navigationService.Navigate(new TaskType9Page(this));
                    break;
            }
        }

        public string RandomForeignPhrase(string exceptOf)
        {
            Random random = new Random();
            int index;
            while (true)
            {
                index = random.Next(0, randPhrasePairs.Count);
                if (randPhrasePairs[index].ForeignPhrase != exceptOf)
                    return randPhrasePairs[index].ForeignPhrase;
            }
        }

        public string RandomOriginPhrase(string exceptOf)
        {
            Random random = new Random();
            int index;
            while (true)
            {
                index = random.Next(0, randPhrasePairs.Count);
                if (randPhrasePairs[index].OriginPhrase != exceptOf)
                    return randPhrasePairs[index].OriginPhrase;
            }
        }

        public void ReadPhrase(string phrase, bool speakAsync = true)
        {
            synthesizer.Pause();
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            List<VoiceInfo> voiceInfos = new List<VoiceInfo>();
            foreach (InstalledVoice installed in synthesizer.GetInstalledVoices())
            {
                if (installed.VoiceInfo.Culture.Name == "fr-FR")
                    voiceInfos.Add(installed.VoiceInfo);
            }
            Random random = new Random();
            synthesizer.SelectVoice(voiceInfos[random.Next(0, voiceInfos.Count)].Name);
            if (speakAsync == true)
                synthesizer.SpeakAsync(phrase);
            else synthesizer.Speak(phrase);
        }
    }
}