using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.Patterns;

namespace UserActivity.Viewer.ViewModel
{
    public class SequentialPatternVM : ComponentVM
    {
        private string _inputData;
        private int _maxPatternLength;
        private string _outputData;

        public SequentialPatternVM()
        {
            MaxPatternLength = 4;
        }

        public void Initialize(IEnumerable<Activity> activities)
        {
            Header = "Посл. Шаблоны";
            InputData = "1,2,1,2" + Environment.NewLine + "2,2,3,1";
            ProcessCommand = new RelayCommand(ExecuteProcessCommand);
        }

        private void ExecuteProcessCommand()
        {
            int[][] sessions = InputData?
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new char[] { '⟨', '⟩', '<', '>', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => int.Parse(c)).ToArray())
                .ToArray();
            int[] classes = PatternHelper.SelectClasses(sessions);

            StringBuilder ob = new StringBuilder();

            ob.AppendLine($"Классы: {string.Join(", ", classes)}.");
            ob.AppendLine($"Длина: {sessions.Sum(s => s.Length)}.");

            int[][] patterns = PatternHelper.GeneratePatterns(classes, 2, MaxPatternLength);
            var results =
                from p in patterns
                let count = PatternHelper.CalculateCount(p, sessions)
                let support = PatternHelper.CalculateSupport(p, sessions)
                where support > 0
                select new { Pattern = p, Count = count, Support = support };

            var maxSupport = results.Select(r => r.Support).Max();

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

        /// <summary>
        /// Input data.
        /// </summary>
        public string InputData
        {
            get { return _inputData; }
            set
            {
                if (_inputData == value)
                {
                    return;
                }
                _inputData = value;
                RaisePropertyChanged(() => InputData);
            }
        }

        /// <summary>
        /// Max pattern length.
        /// </summary>
        public int MaxPatternLength
        {
            get { return _maxPatternLength; }
            set
            {
                if (_maxPatternLength == value)
                {
                    return;
                }
                _maxPatternLength = value;
                RaisePropertyChanged(() => MaxPatternLength);
            }
        }

        /// <summary>
        /// Output data.
        /// </summary>
        public string OutputData
        {
            get { return _outputData; }
            set
            {
                if (_outputData == value)
                {
                    return;
                }
                _outputData = value;
                RaisePropertyChanged(() => OutputData);
            }
        }

        /// <summary>
        /// Process command.
        /// </summary>
        public RelayCommand ProcessCommand { get; set; }
    }
}
