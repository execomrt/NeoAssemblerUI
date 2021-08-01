using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeoGeo;
using V3X.WindowsAPICodePack;
using V3X.WindowsAPICodePack.Dialogs;
using V3X.WindowsAPICodePack.Taskbar;

namespace NeoAssemblerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NeoFile m_Editor;

        void Error(string value)
        {
            Dispatcher.Invoke(() =>
            {
                m_StatusLong.Text = value;
            });
        }
        class ViewControllerWpf : ViewController
        {
            MainWindow Owner;
            public ViewControllerWpf(MainWindow owner)
            {
                Owner = owner;
            }
            public override void CheckOut(string szInput)
            {
                throw new NotImplementedException();
            }
            public override void LogError(string value)
            {
                Owner.Dispatcher.Invoke(() =>
                {
                    Owner.m_StatusLong.Text = value;
                });
            }
            public override void LogPrintf(string value)
            {
                Owner.Dispatcher.Invoke(() =>
                {
                    Owner.m_StatusLong.Text = value;
                });
            }
            public override void LogSuccess(string value)
            {
                Owner.Dispatcher.Invoke(() =>
                {
                    Owner.m_StatusShort.Text = value;
                });
            }
            public override void ReportProgress(int value, int Maximum)
            {

                Owner.Dispatcher.Invoke(() =>
                {
                    var prog = TaskbarManager.Instance;
                    prog.SetProgressState(value == 0 ? TaskbarProgressBarState.NoProgress : TaskbarProgressBarState.Normal);
                    prog.SetProgressValue(value, 100);

                    Owner.m_ProgressBar.Maximum = Maximum;
                    Owner.m_ProgressBar.Value = value;

                });
            }

        }
            public class IRealCommand : ICommand
        {
            protected MainWindow Owner;
            protected ClickDelegate Click;
            public delegate void ClickDelegate(object sender, RoutedEventArgs e);
            public IRealCommand(Window owner, ClickDelegate click)
            {
                Owner = owner as MainWindow;
                Click = click;
            }
            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter)
            {
                return true; //  Owner.Document != null;
            }
            public void Execute(object parameter)
            {
                Click(null, null);
            }
        }
   
        public class IRealCommandContext
        {
            public IRealCommandContext(MainWindow owner)
            {
                
                _buildCommand = new IRealCommand(owner, owner.OnBuild);
                _newCommand = new IRealCommand(owner, owner.OnNew);
                _openCommand = new IRealCommand(owner, owner.OnOpen);
                _saveCommand = new IRealCommand(owner, owner.OnSave);
                _configureCommand = new IRealCommand(owner, owner.OnConfigure);
                _quitCommand = new IRealCommand(owner, owner.OnQuit);
                
            }

            ICommand _openCommand;
            ICommand _newCommand;
            ICommand _saveCommand;
            ICommand _quitCommand;
            ICommand _buildCommand;
            ICommand _configureCommand;
            ICommand _closeCommand;

            public ICommand NewCommand
            {
                get { return _newCommand; }
            }

            public ICommand OpenCommand
            {
                get { return _openCommand; }
            }

            public ICommand SaveCommand
            {
                get { return _saveCommand; }
            }

            public ICommand QuitCommand
            {
                get { return _quitCommand; }
            }

            public ICommand BuildCommand
            {
                get { return _buildCommand; }
            }

            public ICommand ConfigureCommand
            {
                get { return _configureCommand; }
            }

            public ICommand CloseCommand
            {
                get { return _closeCommand; }
            }
        }

        CommonSaveFileDialog saveFileDialog1 = new CommonSaveFileDialog();
        CommonOpenFileDialog openFileDialog1 = new CommonOpenFileDialog();



        private void OnOpen(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Filter = "NEO GEO file (*.neo)|*.neo|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == CommonFileDialogResult.Ok)
            {
                
                if (OnLoadFile(openFileDialog1.FileName))
                {
                    

                }
            }
        }

        private async void OnBuild(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => m_Editor.Build());
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            Error("Not implemented yet");
        }


        private void OnConfigure(object sender, RoutedEventArgs e)
        {
            if (m_Editor == null)
            {
                Error("You must load a project first");
                return;
            }
            var dialog = new HeaderEdit();
            dialog.Info = m_Editor.Info;
            if (dialog.ShowDialogAndAccepted(this))
            {
                UpdateTitle();
                this.m_List.DataContext = m_Editor;
            }
        }


        private void OnQuit(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void OnNew(object sender, RoutedEventArgs e)
        {
            m_Editor = new NeoFile(new ViewControllerWpf(this));
            OnConfigure(sender, e);
            
        }

        public MainWindow()
        {

            InitializeComponent();
            DataContext = new IRealCommandContext(this);
            
            

        }
        void UpdateTitle()
        {
            this.Title = "NeoAssemblerUI - " + m_Editor.Info.Name + " (" + m_Editor.Info.Manu + ", " + m_Editor.Info.Year + ", " + m_Editor.Info.Genre.ToString() + ")";
        }

        void NewFile()
        {
            m_Editor = new NeoFile(new ViewControllerWpf(this));
              
            this.m_List.DataContext = m_Editor;
            UpdateTitle();
        }
    
        bool OnLoadFile(string value)
        { 
            m_Editor = new NeoFile(new ViewControllerWpf(this));
            m_Editor.Load(value);            
            this.m_List.DataContext = m_Editor;
            UpdateTitle();
            return true;
        }


        private void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                m_Editor.AddFiles(files);
            }
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
