
using IDCA.Client.ViewModel.Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            _nextButtonText = _nextButtonTexts[0];
        }

        string _projectPath = string.Empty;
        /// <summary>
        /// 当前项目的根目录完整路径
        /// </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set { SetProperty(ref _projectPath, value); }
        }

        readonly static string[] _pages =
        {
            "Guid/GuidStartPage.xaml",
            "Guid/GuidFirstSettingPage.xaml"
        };

        readonly static string[] _nextButtonTexts =
        {
            "下一步",
            "完成"
        };

        string _nextButtonText;
        public string NextButtonText
        {
            get { return _nextButtonText; }
            set { SetProperty(ref _nextButtonText, value); }
        }

        int _pageIndex = 0;

        string _framePageSource = _pages[0];
        public string FramePageSource
        {
            get { return _framePageSource; }
            set { SetProperty(ref _framePageSource, value); }
        }

        void LastStep()
        {
            if (_pageIndex <= 0)
            {
                return;
            }
            _pageIndex--;
            FramePageSource = _pages[_pageIndex];
            NextButtonText = _nextButtonTexts[0];
            if (_pageIndex == 0)
            {
                LastStepEnabled = false;
            }
        }

        void NextStep()
        {
            LastStepEnabled = true;
            if (_pageIndex >= _pages.Length - 1)
            {
                WindowManager.Show("TableMainWindow");
                MainWindowToClose = true;
                return;
            }
            _pageIndex++;
            FramePageSource = _pages[_pageIndex];
            if (_pageIndex == _pages.Length - 1)
            {
                NextButtonText = _nextButtonTexts[1];
            }
        }

        public ICommand LastStepCommand => new RelayCommand(LastStep);
        public ICommand NextStepCommand => new RelayCommand(NextStep);

        bool _lastStepEnabled = false;
        public bool LastStepEnabled
        {
            get { return _lastStepEnabled; }
            set { SetProperty(ref _lastStepEnabled, value); }
        }


        bool _mainWindowToClose = false;
        public bool MainWindowToClose
        {
            get { return _mainWindowToClose; }
            set { SetProperty(ref _mainWindowToClose, value); }
        }

    }
}
