
using Microsoft.Toolkit.Mvvm.ComponentModel;

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


    }
}
