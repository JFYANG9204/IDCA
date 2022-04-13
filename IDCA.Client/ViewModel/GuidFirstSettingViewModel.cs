
using IDCA.Client.Singleton;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Forms;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class GuidFirstSettingViewModel : ObservableObject
    {
        public GuidFirstSettingViewModel() { }

        string _fileName = string.Empty;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                SetProperty(ref _fileName, value);
                GlobalConfig.Instance.FileName = value;
            }
        }

        string _projectRootPath = GlobalConfig.Instance.ProjectRootPath;
        public string ProjectRootPath
        {
            get { return _projectRootPath; }
            set
            {
                SetProperty(ref _projectRootPath, value);
                GlobalConfig.Instance.ProjectRootPath = value;
            }
        }

        void SelectProjectPath()
        {
            FolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectRootPath = dialog.SelectedPath;
            }
        }

        public ICommand SelectProjectPathCommand => new RelayCommand(SelectProjectPath);

        string _excelSettingFilePath = GlobalConfig.Instance.ExcelSettingFilePath;
        public string ExcelSettingFilePath
        {
            get { return _excelSettingFilePath; }
            set
            {
                SetProperty(ref _excelSettingFilePath, value);
                GlobalConfig.Instance.ExcelSettingFilePath = value;
            }
        }

        void SelectExcelSettingFilePath()
        {
            FileDialog dialog = new OpenFileDialog { 
                Multiselect = false,
                Filter = "Excel File|*.xlsx"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExcelSettingFilePath = dialog.FileName;
            }
        }

        public ICommand SelectExcelSettingFilePathCommand => new RelayCommand(SelectExcelSettingFilePath);

    }
}
