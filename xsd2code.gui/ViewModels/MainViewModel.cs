using System;
using ReactiveUI;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using xsd2code.v2;

namespace xsd2code.gui.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            // create guard
            var canScan = this.WhenAnyValue(x => x.SourcePath, (s) => !string.IsNullOrWhiteSpace(s) && PathExists(s));
            // create command
            PreviewDragOver = ReactiveCommand.Create<DragEventArgs>(eventArgs => DoPreviewDragOver(eventArgs));
            Drop = ReactiveCommand.Create<DragEventArgs>(eventArgs => DoDrop(eventArgs));
            Scan = ReactiveCommand.CreateFromObservable(() => _PerformScan(), canScan);
            // command excepation handling
            Scan.ThrownExceptions.Subscribe(ex =>
            {
                // TODO handle exception
            });
            // map command result
            _ScannedFiles = Scan.ToProperty(this, x => x.ScannedFiles, new List<string>());

            var canGenerate = this.WhenAnyValue(x => x.ScannedFiles, (s) => s.Any());
            //Generate = ReactiveCommand.CreateFromTask<string, string>(_ => PerformGenerate(), canGenerate);
            Generate = ReactiveCommand.CreateFromObservable(() => _PerformGenerate(), canGenerate);
            Generate.ThrownExceptions.Subscribe(ex => { });
            _GeneratedClasses = Generate.ToProperty(this, x => x.GeneratedClasses, new List<string>());

            ToFiles = ReactiveCommand.CreateFromTask(() => ClassesToFiles());

            // busy indicator
            _BusyIndicatorVisibility = Scan.IsExecuting
                .Merge(Generate.IsExecuting)
                .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, x => x.BusyIndicatorVisibility, Visibility.Hidden);

            this.WhenAnyValue(vm => vm.SourcePath)
                .Subscribe(s => TargetPath = s);
        }

        #region Properties

        private string _sourcePath;
        public string SourcePath
        {
            get { return _sourcePath; }
            set { this.RaiseAndSetIfChanged(ref _sourcePath, value); }
        }

        private bool _specifyTarget;
        public bool SpecifyTarget
        {
            get { return _specifyTarget; }
            set { this.RaiseAndSetIfChanged(ref _specifyTarget, value); }
        }

        private string _targetPath;
        public string TargetPath
        {
            get { return _targetPath; }
            set { this.RaiseAndSetIfChanged(ref _targetPath, value); }
        }

        private string _namespace;
        public string Namespace
        {
            get { return _namespace; }
            set { this.RaiseAndSetIfChanged(ref _namespace, value); }
        }

        private bool _specifiedPattern;
        public bool SpecifiedPattern
        {
            get { return _specifiedPattern; }
            set { this.RaiseAndSetIfChanged(ref _specifiedPattern, value); }
        }

        ObservableAsPropertyHelper<List<string>> _ScannedFiles;
        public List<string> ScannedFiles => _ScannedFiles.Value;

        ObservableAsPropertyHelper<List<string>> _GeneratedClasses;
        public List<string> GeneratedClasses => _GeneratedClasses.Value;

        ObservableAsPropertyHelper<Visibility> _BusyIndicatorVisibility;
        public Visibility BusyIndicatorVisibility => _BusyIndicatorVisibility.Value;

        #endregion

        #region Commands

        public ReactiveCommand PreviewDragOver { get; private set; }
        public ReactiveCommand Drop { get; private set; }
        public ReactiveCommand<Unit, List<string>> Scan { get; private set; }
        //public ReactiveCommand Generate { get; private set; }
        public ReactiveCommand<Unit, List<string>> Generate { get; private set; }
        public ReactiveCommand ToFiles { get; private set; }

        #endregion

        #region Helpers

        private bool PathExists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        #endregion

        #region Methods

        private void DoPreviewDragOver(DragEventArgs eventArgs)
        {
            if (eventArgs != null)
            {
                eventArgs.Effects = DragDropEffects.Copy;
                eventArgs.Handled = true;
            }
        }

        private void DoDrop(DragEventArgs eventArgs)
        {
            if (eventArgs != null && eventArgs.Data != null && eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])eventArgs.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    SourcePath = files[0];
                }
            }
        }
        
        private IObservable<List<string>> _PerformScan()
        {
            return Observable.Create<List<string>>(observer =>
            {
                var files = new List<string>();

                FileAttributes attrs = File.GetAttributes(SourcePath);
                if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    files.AddRange(Directory.GetFiles(SourcePath, "*.xsd", SearchOption.TopDirectoryOnly));
                }
                else
                {
                    files.Add(SourcePath);
                }

                observer.OnNext(files);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        private IObservable<List<string>> _PerformGenerate()
        {
            return new Processor(new ProcessorOptions
            {
                Files = ScannedFiles,
                Namespace = Namespace,
                TargetPath = TargetPath,
                SpecifiedPattern = SpecifiedPattern
            })
            .Process();
        }

        private async Task ClassesToFiles()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < GeneratedClasses.Count; i++)
                {
                    using (var fileWriter = new StreamWriter(Path.Combine(TargetPath, $"class_{i}.cs")))
                    {
                        try
                        {
                            fileWriter.Write(GeneratedClasses[i]);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.Message);
                        }
                    }
                }
            });
        }

        #endregion
    }
}
