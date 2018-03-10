using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.Implements;
using UserActivity.Viewer.Patterns;
using UserActivity.Viewer.Services;

namespace UserActivity.Viewer.ViewModel
{
    /// <summary>
    /// Sequential pattern view model.
    /// </summary>
    public class SequentialPatternVM : ComponentVM
    {
        const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Событий: {2}";

        string _inputData;
        int _maxPatternLength;
        string _outputData;
        string _loadedDataInfo;
        DataImportService _import = DataImportService.Create();

        /// <summary>Ctor.</summary>
        public SequentialPatternVM()
        {
            Header = "Посл. Шаблоны";
            LoadedDataInfo = string.Format(DataStatusStringFormat, 0, 0, 0);
            MaxPatternLength = 4;

            InputData = "1,2,1,2" + Environment.NewLine + "2,2,3,1";
            ProcessCommand = new RelayCommand(ExecuteProcessCommand);

            ClassFunc.SelectedItemChanged += OnSelectedClassFuncChanged;
            ClassFunc.Add(ClassFuncByType, "По Типу");
            TimeFunc.SelectedItemChanged += OnSelectedTimeFuncChanged;
            TimeFunc.Add(TimeFuncKLM, "KLM");
            TimeFunc.Add(TimeFuncTLM, "TLM");
        }

        /// <summary>Import UAD from file(s) command.</summary>
        public RelayCommand ImportFileCommand => new RelayCommand(ExecuteImportFile);

        /// <summary>Import UAD from entered text command.</summary>
        public RelayCommand ImportTextCommand => new RelayCommand(ExecuteImportText);

        /// <summary>All loaded data.</summary>
        private List<SessionGroup> Files { get; } = new List<SessionGroup>();

        /// <summary>All data status string property.</summary>
        public string LoadedDataInfo
        {
            get { return _loadedDataInfo; }
            set { Set(ref _loadedDataInfo, value); }
        }

        /// <summary>Selected classification function.</summary>
        public SelectableCollection<CollectionItem<Func<Event, IEnumerable<string>>>> ClassFunc { get; }
            = new SelectableCollection<CollectionItem<Func<Event, IEnumerable<string>>>>();

        private IEnumerable<string> ClassFuncByType(Event @event)
        {
            yield return @event.Kind.ToString().Substring(0, 1);
        }

        /// <summary>Input data.</summary>
        public string InputData
        {
            get { return _inputData; }
            set { Set(ref _inputData, value); }
        }

        /// <summary>Sessions after classification.</summary>
        public ObservableCollection<string[]> InputSessions { get; } = new ObservableCollection<string[]>();

        /// <summary>Selected time calculation function.</summary>
        public SelectableCollection<CollectionItem<Func<string, int>>> TimeFunc { get; }
            = new SelectableCollection<CollectionItem<Func<string, int>>>();

        private int TimeFuncKLM(string @class)
        {
            return 10;
        }

        private int TimeFuncTLM(string @class)
        {
            return 20;
        }

        /// <summary>Max pattern length.</summary>
        public int MaxPatternLength
        {
            get { return _maxPatternLength; }
            set { Set(ref _maxPatternLength, value); }
        }

        /// <summary>Output data.</summary>
        public string OutputData
        {
            get { return _outputData; }
            set { Set(ref _outputData, value); }
        }

        /// <summary>Process command.</summary>
        public RelayCommand ProcessCommand { get; set; }

        /// <summary>
        /// Import UAD-file(s).
        /// </summary>
        private void ExecuteImportFile()
        {
            var groups = _import.ImportFile();
            Files.AddRange(groups);

            //var regions = Files.SelectMany(sg => sg.Sessions)
            //    .SelectMany(s => s.RegionCollection)
            //    .SelectMany(r => r.Images.Select(v => new { r, v }))
            //    .DistinctBy((r1, r2) => r1.r.Name == r2.r.Name && r1.v.Name == r2.v.Name);

            int fileCount = Files.Count;
            int sessionCount = Files.Sum(sg => sg.Sessions.Count);
            int eventCount = Files.Sum(sg => sg.Sessions.Sum(a => a.Events.Count));
            LoadedDataInfo = string.Format(DataStatusStringFormat, fileCount, sessionCount, eventCount);

            ClassFunc.SelectFirst();
        }

        /// <summary>
        /// Import UAD from entered text.
        /// </summary>
        private void ExecuteImportText()
        {
            string[][] sessions = InputData?
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new char[] { '⟨', '⟩', '<', '>', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
            InputSessions.Clear();
            InputSessions.AddRange(sessions);

            var data = string.Join(Environment.NewLine,
                sessions.Select(s => string.Join(", ", s)));
            InputData = data;

            TimeFunc.SelectFirst();
        }

        /// <summary>
        /// Selected classification function changed.
        /// </summary>
        private void OnSelectedClassFuncChanged(object sender, EventArgs ev)
        {
            var func = ClassFunc.SelectedItem.Value;
            var sessions = Files.SelectMany(f => f.Sessions)
                .Select(s => s.Events
                    .SelectMany(e => func(e))
                    .ToArray());
            InputSessions.Clear();
            InputSessions.AddRange(sessions);

            var data = string.Join(Environment.NewLine,
                sessions.Select(s => string.Join(", ", s)));
            InputData = data;

            TimeFunc.SelectFirst();
        }

        /// <summary>
        /// Selected time calculation function changed.
        /// </summary>
        private void OnSelectedTimeFuncChanged(object sender, EventArgs ev)
        {
            var func = TimeFunc.SelectedItem.Value;
            var data = string.Join(Environment.NewLine,
                InputSessions.Select(s => s.Sum(e => func(e))));
            OutputData = data;
        }

        /// <summary>
        /// Process sequential pattern algorithm.
        /// </summary>
        private void ExecuteProcessCommand()
        {
            var sessions = InputSessions.ToArray();
            var classes = PatternHelper.SelectClasses(sessions);

            StringBuilder ob = new StringBuilder();

            ob.AppendLine($"Классы: {string.Join(", ", classes)}.");
            ob.AppendLine($"Длина: {sessions.Sum(s => s.Length)}.");

            var patterns = PatternHelper.GeneratePatterns(classes, 2, MaxPatternLength);
            var results = (
                from p in patterns
                let count = PatternHelper.CalculateCount(p, sessions)
                let support = PatternHelper.CalculateSupport(p, sessions)
                where support > 0
                select new { Pattern = p, Count = count, Support = support })
                .ToArray();

            var maxSupport = results.Length == 0 ? 0 : results.Select(r => r.Support).Max();

            ob.AppendLine("Шаблоны:");
            foreach (var result in results)
            {
                ob.Append("⟨");
                ob.Append(string.Join(",", result.Pattern));
                ob.Append("⟩");
                ob.Append(". μ = ");
                ob.Append(result.Count);
                ob.Append(". λ = ");
                ob.Append(Math.Round(result.Support, 5));
                ob.Append(".");
                if (result.Support == maxSupport)
                {
                    ob.Append(" - MAX");
                }
                ob.AppendLine();
            }
            double sumSup = results.Sum(_ => _.Support);
            ob.AppendLine("Сумма поддержки: " + Math.Round(sumSup, 5));
            double sumlengths = results.Sum(_ => _.Pattern.Length);
            double sumWeightedSup = results.Sum(_ => _.Support * ((double)_.Pattern.Length / sumlengths));
            ob.AppendLine("Взв.сумма поддержка: " + Math.Round(sumWeightedSup, 5));
            double sumAvgSup = results.Sum(_ => _.Support) / results.Where(p => p.Support > 0).Count();
            ob.AppendLine("Сред.сумма поддержка: " + Math.Round(sumAvgSup, 5));

            OutputData = ob.ToString();
        }
    }
}
