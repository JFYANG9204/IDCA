
using IDCA.Client.Singleton;
using IDCA.Client.ViewModel.Common;
using IDCA.Model;
using IDCA.Model.MDM;
using IDCA.Model.Template;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class StartWindowViewModel : ObservableObject
    {
        public StartWindowViewModel()
        {
            _config = GlobalConfig.Instance.Config;
            _templateDictionary = GlobalConfig.Instance.TemplateDictionary;
            _templateItems = new ObservableCollection<TemplateElementViewModel>();
            UpdateTemplateInformation();
        }

        readonly Config _config;
        readonly TemplateDictionary _templateDictionary;

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

        int _templateSelectedIndex = -1;
        public int TemplateSelectedIndex
        {
            get { return _templateSelectedIndex; }
            set 
            { 
                SetProperty(ref _templateSelectedIndex, value);
                CheckConfirmEnable();
            }
        }

        bool _isConfirmButtonEnable = false;
        public bool IsConfirmButtonEnable
        {
            get { return _isConfirmButtonEnable; }
            set { SetProperty(ref _isConfirmButtonEnable, value); }
        }

        void CheckConfirmEnable()
        {
            IsConfirmButtonEnable = _templateSelectedIndex >= 0 && _templateSelectedIndex < _templateItems.Count;
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
            WindowManager.HideWindow(sender);
            var mdm = new MDMDocument();
            string path = GlobalConfig.Instance.MdmDocumentPath;
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                mdm.Open(path);
            }
            WindowManager.ShowWindow("TableSettingWindow", new TableSettingWindowViewModel(_templateItems[_templateSelectedIndex].Template, mdm));
            WindowManager.CloseWindow(sender);
        }
        public ICommand ConfirmCommand => new RelayCommand<object?>(Confirm);


        void UpdateTemplateInformation()
        {
            GlobalConfig.Instance.TemplateDictionary.Clear();
            _templateItems.Clear();
            _templateDictionary.LoadFromFolder(_config.TryGet<string>(SpecConfigKeys.TemplateRootPath) ?? "");
            foreach (var item in GlobalConfig.Instance.TemplateDictionary.GetTemplates())
            {
                var templateElement = new TemplateElementViewModel(item);
                _templateItems.Add(templateElement);
            }
        }

        void LoadTemplate()
        {
            string? folder = WindowManager.ShowFolderBrowserDialog();
            if (folder != null && Directory.Exists(folder))
            {
                _config.Set(SpecConfigKeys.TemplateRootPath, folder);
                _config.UpdateToSettings(Properties.Settings.Default);
                Properties.Settings.Default.Save();
                UpdateTemplateInformation();
            }
        }
        public ICommand LoadTemplateCommand => new RelayCommand(LoadTemplate);
    }
}
