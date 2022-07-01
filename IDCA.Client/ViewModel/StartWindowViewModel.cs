
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
            GlobalConfig.Instance.SettingWindowViewModel.TemplateRootPathChanged += s => UpdateTemplateInformation();
        }

        readonly Config _config;
        readonly TemplateDictionary _templateDictionary;

        bool _mainWindowToClose = false;
        /// <summary>
        /// 用于控制开始窗口是否关闭
        /// </summary>
        public bool MainWindowToClose
        {
            get { return _mainWindowToClose; }
            set { SetProperty(ref _mainWindowToClose, value); }
        }

        ObservableCollection<TemplateElementViewModel> _templateItems;
        /// <summary>
        /// 当前读取到的模板配置
        /// </summary>
        public ObservableCollection<TemplateElementViewModel> TemplateItems
        {
            get { return _templateItems; }
            set { SetProperty(ref _templateItems, value); }
        }

        int _templateSelectedIndex = -1;
        /// <summary>
        /// 模板列表选中索引
        /// </summary>
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
        /// <summary>
        /// 确认按钮是否可用。如果不填写所有必须的配置，需要禁用确认按钮，完成必须内容的填写才能进行下一步
        /// </summary>
        public bool IsConfirmButtonEnable
        {
            get { return _isConfirmButtonEnable; }
            set { SetProperty(ref _isConfirmButtonEnable, value); }
        }

        /// <summary>
        /// 检查当前的必须参数是否都已有值，如果都已赋值，将确认按钮改为可用
        /// </summary>
        void CheckConfirmEnable()
        {
            IsConfirmButtonEnable = _templateSelectedIndex >= 0 && _templateSelectedIndex < _templateItems.Count;
        }

        string _projectName = string.Empty;
        /// <summary>
        /// 项目名称
        /// </summary>
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
        /// <summary>
        /// 项目的根目录
        /// </summary>
        public string ProjectRootPath
        {
            get { return _projectRootPath; }
            set
            {
                SetProperty(ref _projectRootPath, value);
                GlobalConfig.Instance.ProjectRootPath = value;
            }
        }

        /// <summary>
        /// 弹出文件夹选择界面并选择项目的根目录。
        /// </summary>
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
        /// <summary>
        /// Excel配置文件的路径，可选参数。
        /// </summary>
        public string ExcelSettingFilePath
        {
            get { return _excelSettingFilePath; }
            set
            {
                SetProperty(ref _excelSettingFilePath, value);
                GlobalConfig.Instance.ExcelSettingFilePath = value;
            }
        }

        /// <summary>
        /// 弹出文件选取界面并选取EXCEL配置文件。
        /// </summary>
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
        /// <summary>
        /// 扩展名为.mdd的MDM文档路径，可以在表格配置界面修改。
        /// </summary>
        public string MdmDocumentPath
        {
            get { return _mdmDocumentPath; }
            set
            {
                SetProperty(ref _mdmDocumentPath, value);
                GlobalConfig.Instance.MdmDocumentPath = value;
            }
        }

        /// <summary>
        /// 弹出选择.mdd类型文件的文件选取窗口。
        /// </summary>
        void SelectMdmDocumentPath()
        {
            object? dialogResult = WindowManager.ShowOpenFileDialog("MDM Document|*.mdd");
            if (dialogResult != null && dialogResult is string filePath)
            {
                MdmDocumentPath = filePath;
            }
        }

        public ICommand SelectMdmDocumentPathCommand => new RelayCommand(SelectMdmDocumentPath);

        /// <summary>
        /// 确认按钮相应方法，需要隐藏当前开始窗口并打开新的表格配置窗口。
        /// </summary>
        /// <param name="sender"></param>
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

        /// <summary>
        /// 更新当前已经载入的模板列表
        /// </summary>
        void UpdateTemplateInformation()
        {
            _templateItems.Clear();
            foreach (var item in GlobalConfig.Instance.TemplateDictionary.GetTemplates())
            {
                var templateElement = new TemplateElementViewModel(item);
                _templateItems.Add(templateElement);
            }
        }

        /// <summary>
        /// 弹出文件夹选取窗口并更新模板列表。
        /// </summary>
        void ShowSettingWindow()
        {
            WindowManager.ShowWindow("SettingWindow", GlobalConfig.Instance.SettingWindowViewModel);
        }
        public ICommand ShowSettingWindowCommand => new RelayCommand(ShowSettingWindow);
    }
}
