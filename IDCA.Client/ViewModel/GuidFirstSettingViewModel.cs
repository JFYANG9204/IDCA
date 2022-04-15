
using IDCA.Client.Singleton;
using IDCA.Client.ViewModel.Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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
            object? dialogResult = WindowManager.ShowDialog("FolderBrowserDialog");
            if (dialogResult != null && dialogResult is string folderPath)
            {
                ProjectRootPath = folderPath;
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
            object? dialogResult = WindowManager.ShowDialog("FileDialog", "Excel File|*.xlsx");
            if (dialogResult != null && dialogResult is string filePath)
            {
                ExcelSettingFilePath = filePath;
            }
        }

        public ICommand SelectExcelSettingFilePathCommand => new RelayCommand(SelectExcelSettingFilePath);

        string _mdmDocumentPath = GlobalConfig.Instance.MdmDocumentPath;
        public string MdmDocumentPath
        {
            get { return _mdmDocumentPath; }
            set {
                SetProperty(ref _mdmDocumentPath, value);
                GlobalConfig.Instance.MdmDocumentPath = value;
            }
        }

        void SelectMdmDocumentPath()
        {
            object? dialogResult = WindowManager.ShowDialog("FileDialog", "MDM Document|*.mdd");
            if (dialogResult != null && dialogResult is string filePath)
            {
                MdmDocumentPath = filePath;
            }
        }

        public ICommand SelectMdmDocumentPathCommand => new RelayCommand(SelectMdmDocumentPath);

    }
}
