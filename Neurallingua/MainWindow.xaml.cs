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
using System.Windows.Threading;

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

    public enum TaskFamily
    {
        Testing = 0,
        Writing = 1,
        Speaking = 2
    }

    public class PhrasePair
    {
        private string foreignPhrase;
        private string originPhrase;
        private int timesTested;
        private int testing;
        private int writing;
        private int speaking;
        private DateTime dateTime;
        private int index;
        private TaskFamily taskFamily;
        private bool toRepeat = false;

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

        public int Index
        {
            get { return index; }
        }

        public TaskFamily TaskFamily
        {
            get { return taskFamily; }
        }

        public bool ToRepeat
        {
            get { return toRepeat; }
            set { toRepeat = value; }
        }

        public PhrasePair(string foreignPhrase, string originPhrase, int timesTested,
            int testing, int writing, int speaking, DateTime dateTime)
        {
            this.foreignPhrase = foreignPhrase;
            this.originPhrase = originPhrase;
            this.timesTested = timesTested;
            this.testing = testing;
            this.writing = writing;
            this.speaking = speaking;
            this.dateTime = dateTime;
            index = timesTested - (DateTime.Now - dateTime).Days * 2;
        }

        public bool CheckIfIsNew()
        {
            if (timesTested == 0 && testing == 0 && writing == 0 && speaking == 0)
                return true;
            return false;
        }

        public TaskFamily SelectTaskFamily()
        {
            if (CheckIfIsNew() == true)
                return TaskFamily.Testing;
            int times = 0;
            bool selected = false;
            List<TaskFamily> families = new List<TaskFamily>();
            while (selected == false)
            {
                if (testing == times)
                {
                    selected = true;
                    families.Add(TaskFamily.Testing);
                }
                if (writing == times)
                {
                    selected = true;
                    families.Add(TaskFamily.Writing);
                }
                if (speaking == times)
                {
                    selected = true;
                    families.Add(TaskFamily.Speaking);
                }
                times++;
            }
            Random random = new Random();
            int familyId = random.Next(0, families.Count);
            taskFamily = families[familyId];
            return taskFamily;
        }

        public void IncreaseTimesTested()
        {
            timesTested++;
            dateTime = DateTime.Now;
            EditTaskFamilyValue(1);
            toRepeat = false;
        }

        public void EditTaskFamilyValue(int value)
        {
            switch (taskFamily)
            {
                case TaskFamily.Testing:
                    testing += value;
                    break;
                case TaskFamily.Writing:
                    writing += value;
                    break;
                case TaskFamily.Speaking:
                    speaking += value;
                    break;
            }
        }

        public override string ToString()
        {
            string s = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                foreignPhrase, originPhrase, timesTested, testing,
                writing, speaking, dateTime);
            return s;
        }
    }

    public class TestingEngine
    {
        private const string phrasesPath = "phrases.csv";
        private int testsCount;
        private int totalTestsCount;
        private int lessTestedValue;
        private int midMemoryIndex;
        private List<PhrasePair> allPhrasePairs;
        private List<PhrasePair> testingPhrasePairs;
        private List<PhrasePair> testedPhrasePairs;
        private List<PhrasePair> randPhrasePairs;
        private PhrasePair currentPhrasePair;
        private bool goToStudyingPage = false;
        SpeechSynthesizer synthesizer;

        public int Total
        {
            get { return totalTestsCount; }
        }

        public int Progress
        {
            get { return totalTestsCount - testsCount; }
        }

        public PhrasePair CurrentPhrasePair
        {
            get { return currentPhrasePair; }
        }

        public bool GoToStudyingPage
        {
            get { return goToStudyingPage; }
        }

        public TestingEngine()
        {
            allPhrasePairs = new List<PhrasePair>();
            testingPhrasePairs = new List<PhrasePair>();
            testedPhrasePairs = new List<PhrasePair>();
            randPhrasePairs = new List<PhrasePair>();
            synthesizer = new SpeechSynthesizer();
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
            int timesTested, testing, writing, speaking;
            DateTime dateTime;
            double sum = 0; int count = 0;
            foreach (string s in strings)
            {
                items = s.Split('|');
                try
                {
                    foreignPhrase = items[0];
                    originPhrase = items[1];
                    timesTested = 0;
                    testing = 0;
                    writing = 0;
                    speaking = 0;
                    dateTime = DateTime.Now;
                    if (items.Length == 7)
                    {
                        timesTested = Convert.ToInt32(items[2]);
                        testing = Convert.ToInt32(items[3]);
                        writing = Convert.ToInt32(items[4]);
                        speaking = Convert.ToInt32(items[5]);
                        dateTime = Convert.ToDateTime(items[6]);
                    }
                    if (timesTested >= 3)
                    {
                        sum += timesTested;
                        count++;
                    }
                    PhrasePair phrasePair = new PhrasePair(
                        foreignPhrase, originPhrase, timesTested,
                        testing, writing, speaking, dateTime);
                    allPhrasePairs.Add(phrasePair);
                    randPhrasePairs.Add(phrasePair);
                }
                catch
                {
                    MessageBox.Show(s);
                    return false;
                }
            }
            midMemoryIndex = Convert.ToInt32(Math.Round(
                sum / count / 2, 0, MidpointRounding.AwayFromZero));
            if (midMemoryIndex < 3)
                midMemoryIndex = 3;
            if (this.testsCount > allPhrasePairs.Count)
                this.testsCount = allPhrasePairs.Count;
            totalTestsCount = this.testsCount;

            lessTestedValue = allPhrasePairs[0].TimesTested;
            PhrasePair lessIndexPair;
            int iteration = 0;
            while (iteration < this.testsCount)
            {
                if (allPhrasePairs.Count == 0)
                    break;
                lessIndexPair = allPhrasePairs[0];
                foreach (PhrasePair phrasePair in allPhrasePairs)
                {
                    if (phrasePair != lessIndexPair &&
                        phrasePair.Index < lessIndexPair.Index)
                        lessIndexPair = phrasePair;
                    if (phrasePair.TimesTested < lessTestedValue)
                        lessTestedValue = phrasePair.TimesTested;
                }
                int distance = midMemoryIndex - lessIndexPair.TimesTested;
                if (distance <= 0)
                    distance = 1;
                while (distance > 0 && iteration < this.testsCount)
                {
                    testingPhrasePairs.Add(lessIndexPair);
                    allPhrasePairs.Remove(lessIndexPair);
                    distance--;
                    iteration++;
                }
            }
            return true;
        }

        private PhrasePair GetNextTestingPair()
        {
            Random random = new Random();
            int index = random.Next(0, testingPhrasePairs.Count);
            PhrasePair testingPair = testingPhrasePairs[index];
            testingPhrasePairs.Remove(testingPair);
            if (testedPhrasePairs.Contains(testingPair) == false)
                testedPhrasePairs.Add(testingPair);
            testsCount--;
            return testingPair;
        }

        private TaskFamily GetNextTaskFamily()
        {
            if (currentPhrasePair.ToRepeat)
                return currentPhrasePair.TaskFamily;
            return currentPhrasePair.SelectTaskFamily();
        }

        public void AddPhrasePairToRepeat(PhrasePair phrasePair)
        {
            if (testedPhrasePairs.Contains(phrasePair))
            {
                phrasePair.ToRepeat = true;
                phrasePair.TimesTested = lessTestedValue - 1;
                phrasePair.EditTaskFamilyValue(-1);
                testingPhrasePairs.Add(phrasePair);
                testedPhrasePairs.Remove(phrasePair);
                goToStudyingPage = true;
                testsCount++;
            }
        }

        private void SaveTestedPhrases()
        {
            allPhrasePairs.AddRange(testedPhrasePairs);
            // allPhrasePairs.AddRange(testingPhrasePairs);
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
            if (goToStudyingPage == true)
            {
                navigationService.Navigate(new StudyingPage(this));
                goToStudyingPage = false;
                return;
            }
            currentPhrasePair = GetNextTestingPair();
            TaskFamily family = GetNextTaskFamily();
            Random random = new Random();
            switch (family)
            {
                case TaskFamily.Testing:
                    switch (random.Next(0, 4))
                    {
                        case 0:
                            navigationService.Navigate(new TaskType1Page(this));
                            break;
                        case 1:
                            navigationService.Navigate(new TaskType3Page(this));
                            break;
                        case 2:
                            navigationService.Navigate(new TaskType4Page(this));
                            break;
                        case 3:
                            navigationService.Navigate(new TaskType5Page(this));
                            break;
                    }
                    break;
                case TaskFamily.Writing:
                    switch (random.Next(0, 2))
                    {
                        case 0:
                            navigationService.Navigate(new TaskType2Page(this));
                            break;
                        case 1:
                            navigationService.Navigate(new TaskType6Page(this));
                            break;
                    }
                    break;
                case TaskFamily.Speaking:
                    navigationService.Navigate(new TaskType9Page(this));
                    break;
            }
        }

        public void EndUpWithTaskPage(Dispatcher dispatcher, NavigationService navigationService)
        {
            Thread thread = new(() =>
            {
                ReadPhrase(currentPhrasePair.ForeignPhrase, false);
                dispatcher.Invoke(() =>
                {
                    GoToNextTaskPage(navigationService);
                });
            });
            thread.IsBackground = true;
            thread.Start();
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

            List<VoiceInfo> voiceInfos = GetAvailableVoices("fr-FR");
            Random random = new Random();
            synthesizer.SelectVoice(voiceInfos[random.Next(0, voiceInfos.Count)].Name);
            if (speakAsync == true)
                synthesizer.SpeakAsync(phrase);
            else synthesizer.Speak(phrase);
        }

        public void ReadPhraseByCertainVoice(string phrase, int voiceIndex)
        {
            synthesizer.Pause();
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            List<VoiceInfo> voiceInfos = GetAvailableVoices("fr-FR");
            synthesizer.SelectVoice(voiceInfos[voiceIndex].Name);
            synthesizer.SpeakAsync(phrase);
        }

        private List<VoiceInfo> GetAvailableVoices(string cultureCode)
        {
            List<VoiceInfo> voiceInfos = new List<VoiceInfo>();
            foreach (InstalledVoice installed in synthesizer.GetInstalledVoices())
            {
                if (installed.VoiceInfo.Culture.Name == cultureCode &&
                    installed.VoiceInfo.Id.StartsWith("MSTTS") == true)
                    voiceInfos.Add(installed.VoiceInfo);
            }
            return voiceInfos;
        }
    }
}