
using IDCA.Client.Singleton;
using IDCA.Client.ViewModel.Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class StartWindowViewModel : ObservableObject
    {
        public StartWindowViewModel()
        {
            _templateItems = new ObservableCollection<TemplateElementViewModel>();
            var template = new TemplateElementViewModel
            {
                TemplateName = "默认模板",
                TemplateDescription = "普通的多期模板"
            };
            _templateItems.Add(template);
        }

        bool _mainWindowToClose = false;
        public bool MainWindowToClose
        {
            get { return _mainWindowToClose; }
            set { SetProperty(ref _mainWindowToClose, value); }
        }

        ObservableCollection<TemplateElementViewModel> _templateItems;
        public ObservableCollection<TemplateElementViewModel> TemplateItems
        {
            get { return _templateItems; }
            set { SetProperty(ref _templateItems, value); }
        }

        int _templateSelectedIndex = GlobalConfig.Instance.TemplateSelectIndex;
        public int TemplateSelectedIndex
        {
            get { return _templateSelectedIndex; }
            set
            {
                SetProperty(ref _templateSelectedIndex, value);
                GlobalConfig.Instance.TemplateSelectIndex = value;
            }
        }

        string _projectName = string.Empty;
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                SetProperty(ref _projectName, value);
                GlobalConfig.Instance.ProjectName = value;
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
            object? dialogResult = WindowManager.ShowFolderBrowserDialog();
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
            object? dialogResult = WindowManager.ShowOpenFileDialog("Excel File|*.xlsx");
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
            set
            {
                SetProperty(ref _mdmDocumentPath, value);
                GlobalConfig.Instance.MdmDocumentPath = value;
            }
        }

        void SelectMdmDocumentPath()
        {
            object? dialogResult = WindowManager.ShowOpenFileDialog("MDM Document|*.mdd");
            if (dialogResult != null && dialogResult is string filePath)
            {
                MdmDocumentPath = filePath;
            }
        }

        public ICommand SelectMdmDocumentPathCommand => new RelayCommand(SelectMdmDocumentPath);

        void Confirm(object? sender)
        {
            WindowManager.Show("TableSettingWindow");
            WindowManager.CloseWindow(sender);
        }
        public ICommand ConfirmCommand => new RelayCommand<object?>(Confirm);

    }
}
