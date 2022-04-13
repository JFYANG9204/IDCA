
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
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
                return;
            }
            _pageIndex++;
            FramePageSource = _pages[_pageIndex];
        }

        public ICommand LastStepCommand => new RelayCommand(LastStep);
        public ICommand NextStepCommand => new RelayCommand(NextStep);

        bool _lastStepEnabled = false;
        public bool LastStepEnabled
        {
            get { return _lastStepEnabled; }
            set { SetProperty(ref _lastStepEnabled, value); }
        }

    }
}
