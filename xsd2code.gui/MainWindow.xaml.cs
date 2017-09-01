using ReactiveUI;
using System.Windows;
using xsd2code.gui.ViewModels;

namespace xsd2code.gui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();

            this.WhenActivated(d =>
            {
                #region Bind properties

                d(this.Bind(ViewModel, vm => vm.SourcePath, v => v.Source.Text));
                d(this.Bind(ViewModel, vm => vm.SpecifyTarget, v => v.SpecifyTarget.IsChecked));
                d(this.Bind(ViewModel, vm => vm.TargetPath, v => v.Target.Text));
                d(this.Bind(ViewModel, vm => vm.SpecifyTarget, v => v.Target.IsEnabled));
                d(this.Bind(ViewModel, vm => vm.Namespace, v => v.Namespace.Text));
                d(this.Bind(ViewModel, vm => vm.SpecifiedPattern, v => v.SpecifiedPattern.IsChecked));
                d(this.OneWayBind(ViewModel, vm => vm.ScannedFiles, v => v.Scanned.ItemsSource));
                d(this.OneWayBind(ViewModel, vm => vm.GeneratedClasses, v => v.GeneratedClasses.ItemsSource));
                d(this.OneWayBind(ViewModel, vm => vm.BusyIndicatorVisibility, v => v.BusyIndicator.Visibility));

                #endregion

                #region Bind commands

                /*
                 * I've solved my problem I believe. I was running Visual Studio as Administrator.
                 * When it launched my application, it didn't recognize drags from Explorer because Explorer was running in User mode.
                 * Hope this bonehead move helps someone else out.
                 */
                d(Source.Events().PreviewDragOver.InvokeCommand(ViewModel.PreviewDragOver));
                d(Source.Events().Drop.InvokeCommand(ViewModel.Drop));
                d(this.BindCommand(ViewModel, vm => vm.Scan, v => v.DoScan));
                d(this.BindCommand(ViewModel, vm => vm.Generate, v => v.DoGenerate));
                d(this.BindCommand(ViewModel, vm => vm.ToFiles, v => v.DoToFile));

                #endregion
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }

        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainWindow));
    }
}
