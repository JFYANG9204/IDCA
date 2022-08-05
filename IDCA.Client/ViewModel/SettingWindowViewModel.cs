using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IDCA.Client.Singleton;
using IDCA.Client.ViewModel.Common;
using IDCA.Model;
using IDCA.Model.Template;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class SettingWindowViewModel : ObservableObject
    {

        public SettingWindowViewModel(Config config)
        {
            _settings = Properties.Settings.Default;
            _config = config;
            _templateInfos = new ObservableCollection<TemplateInfo>();
            // 初始化
            _templateRootPath = _config.Get(SpecConfigKeys.GLOBAL_TEMPLATE_ROOTPATH);
            _axisAddSigma = _config.Get(SpecConfigKeys.AXIS_ADD_SIGMA);
            _axisSigmaLabel = _config.Get(SpecConfigKeys.AXIS_SIGMA_LABEL);
            _axisNetAheadLabel = _config.Get(SpecConfigKeys.AXIS_NET_AHEAD_LABEL);
            _axisNetInsertEmptyLine = _config.Get(SpecConfigKeys.AXIS_NET_INSERT_EMPTYLINE);
            _axisNpsTopBox = _config.Get(SpecConfigKeys.AXIS_NPS_TOP_BOX).ToString();
            _axisNpsBottomBox = _config.Get(SpecConfigKeys.AXIS_NPS_BOTTOM_BOX).ToString();
            _axisMeanLabel = _config.Get(SpecConfigKeys.AXIS_MEAN_LABEL);
            _axisStddevLabel = _config.Get(SpecConfigKeys.AXIS_STDDEV_LABEL);
            _axisStderrLabel = _config.Get(SpecConfigKeys.AXIS_STDERR_LABEL);
            _axisTopBottomBoxPosition = _config.Get(SpecConfigKeys.AXIS_TOP_BOTTOM_BOX_POSITION);
            _axisNetType = _config.Get(SpecConfigKeys.AXIS_NET_TYPE);
            _axisAverageMentionLabel = _config.Get(SpecConfigKeys.AXIS_AVERAGE_MENTION_LABEL);
            _axisAverageMentionBlankLine = _config.Get(SpecConfigKeys.AXIS_AVERAGE_MENTION_BLANKLINE);
            _axisAverageMentionDecimals = _config.Get(SpecConfigKeys.AXIS_AVERAGE_MENTION_DECIMALS).ToString();
            _tableSummaryLabel = _config.Get(SpecConfigKeys.TABLE_SUMMARY_LABEL);
            _tableSettingNetLabelCodeSeparater = _config.Get(SpecConfigKeys.TABLE_SETTING_NET_LABEL_CODE_SEPARATER);
            _tableSettingNetCodeSeparater = _config.Get(SpecConfigKeys.TABLE_SETTING_NET_CODE_SEPARATER);
            _tableSettingNetRangeSeparater = _config.Get(SpecConfigKeys.TABLE_SETTING_NET_RANGE_SEPARATER);
        }

        readonly ApplicationSettingsBase _settings;
        readonly Config _config;

        void SynchronizeProperty<T>(ref T field, T newValue, ConfigInfo<T> info)
        {
            SetProperty(ref field, newValue);
            _config.Set(info, newValue);
            _config.Synchronize(_settings);
            _settings.Save();
        }

        // Global Configs

        Action<string>? _templateRootPathChanged;
        /// <summary>
        /// 当模板根目录修改时触发的事件。
        /// </summary>
        public event Action<string> TemplateRootPathChanged
        {
            add { _templateRootPathChanged += value; }
            remove { _templateRootPathChanged -= value; }
        }

        string _templateRootPath;
        /// <summary>
        /// 模板文件夹目录
        /// </summary>
        public string TemplateRootPath
        {
            get { return _templateRootPath; }
            set 
            { 
                if (Directory.Exists(value))
                {
                    SynchronizeProperty(ref _templateRootPath, value, SpecConfigKeys.GLOBAL_TEMPLATE_ROOTPATH);
                    GlobalConfig.Instance.TemplateDictionary.Clear();
                    GlobalConfig.Instance.TemplateDictionary.LoadFromFolder(value);
                    _templateRootPathChanged?.Invoke(value);
                    if (GlobalConfig.Instance != null)
                    {
                        LoadTemplateInfos(GlobalConfig.Instance.TemplateDictionary);
                    }
                }
            }
        }

        void SelectTemplateRootPath()
        {
            var dialogResult = WindowManager.ShowFolderBrowserDialog();
            if (dialogResult != null)
            {
                TemplateRootPath = dialogResult;
            }
        }
        /// <summary>
        /// 弹出文件夹选择窗口，选取模板根文件夹路径
        /// </summary>
        public ICommand SelectTemplateRootPathCommand => new RelayCommand(SelectTemplateRootPath);

        public class TemplateInfo : ObservableObject
        {
            public TemplateInfo(string name, string description, string xmlPath)
            {
                _name = name;
                _description = description;
                _xmlPath = xmlPath;
            }

            string _name;
            string _description;
            string _xmlPath;

            public string Name
            {
                get => _name;
                set => SetProperty(ref _name, value);
            }

            public string Description
            {
                get => _description;
                set => SetProperty(ref _description, value);
            }

            public string XmlPath
            {
                get => _xmlPath;
                set => SetProperty(ref _xmlPath, value);
            }

            void OpenXmlFolder()
            {
                if (Directory.Exists(Path.GetDirectoryName(_xmlPath)))
                {
                    Process.Start("explorer.exe", _xmlPath);
                }
                else
                {
                    Logger.Message($"文件夹{_xmlPath}不存在！");
                }
            }
            /// <summary>
            /// 打开xml文件所在文件夹
            /// </summary>
            public ICommand OpenXmlFolderCommand => new RelayCommand(OpenXmlFolder);
        }

        ObservableCollection<TemplateInfo> _templateInfos;
        /// <summary>
        /// 当前已载入的模板信息
        /// </summary>
        public ObservableCollection<TemplateInfo> TemplateInfos
        {
            get { return _templateInfos; }
            set { SetProperty(ref _templateInfos, value); }
        }

        /// <summary>
        /// 载入模板信息
        /// </summary>
        public void LoadTemplateInfos(TemplateDictionary templateDictionary)
        {
            _templateInfos.Clear();
            foreach (var temp in templateDictionary.GetTemplates())
            {
                _templateInfos.Add(new TemplateInfo(temp.Name, temp.Description, temp.XmlPath));
            }
        }

        // Axis Configs

        bool _axisAddSigma;
        /// <summary>
        /// 轴表达式配置是否默认在最后添加Sigma
        /// </summary>
        public bool AxisAddSigma
        {
            get => _axisAddSigma;
            set => SynchronizeProperty(ref _axisAddSigma, value, SpecConfigKeys.AXIS_ADD_SIGMA);
        }

        string _axisSigmaLabel;
        /// <summary>
        /// 轴表达式添加Sigma行时的行标题
        /// </summary>
        public string AxisSigmaLabel
        {
            get => _axisSigmaLabel;
            set => SynchronizeProperty(ref _axisSigmaLabel, value, SpecConfigKeys.AXIS_SIGMA_LABEL);
        }

        string _axisNetAheadLabel;
        /// <summary>
        /// 轴表达式Net元素标签的默认前缀
        /// </summary>
        public string AxisNetAheadLabel
        {
            get => _axisNetAheadLabel;
            set => SynchronizeProperty(ref _axisNetAheadLabel, value, SpecConfigKeys.AXIS_NET_AHEAD_LABEL);
        }

        bool _axisNetInsertEmptyLine;
        /// <summary>
        /// 是否默认在Net元素之间添加空白行
        /// </summary>
        public bool AxisNetInsertEmptyLine
        {
            get => _axisNetInsertEmptyLine;
            set => SynchronizeProperty(ref _axisNetInsertEmptyLine, value, SpecConfigKeys.AXIS_NET_INSERT_EMPTYLINE);
        }

        string _axisNpsTopBox;
        /// <summary>
        /// 轴表达式添加NPS时默认使用的Top Box数量。
        /// </summary>
        public string AxisNpsTopBox
        {
            get { return _axisNpsTopBox; }
            set
            {
                if (int.TryParse(value, out int i))
                {
                    SetProperty(ref _axisNpsTopBox, value);
                    _config.Set(SpecConfigKeys.AXIS_NPS_TOP_BOX, i);
                    _config.Synchronize(_settings);
                    _settings.Save();
                }
            }
        }

        string _axisNpsBottomBox;
        /// <summary>
        /// 轴表达式添加NPS时默认使用的Bottom Box数量。
        /// </summary>
        public string AxisNpsBottomBox
        {
            get { return _axisNpsBottomBox; }
            set
            {
                if (int.TryParse(value, out int i))
                {
                    SetProperty(ref _axisNpsBottomBox, value);
                    _config.Set(SpecConfigKeys.AXIS_NPS_BOTTOM_BOX, i);
                    _config.Synchronize(_settings);
                    _settings.Save();
                }
            }
        }

        string _axisMeanLabel;
        /// <summary>
        /// 轴表达式添加均值时Mean对象的默认标签。
        /// </summary>
        public string AxisMeanLabel
        {
            get { return _axisMeanLabel; }
            set { SynchronizeProperty(ref _axisMeanLabel, value, SpecConfigKeys.AXIS_MEAN_LABEL); }
        }

        string _axisStddevLabel;
        /// <summary>
        /// 轴表达式添加均值时Stddev对象的默认标签
        /// </summary>
        public string AxisStddevLabel
        {
            get { return _axisStddevLabel; }
            set { SynchronizeProperty(ref _axisStddevLabel, value, SpecConfigKeys.AXIS_STDDEV_LABEL); }
        }

        string _axisStderrLabel;
        /// <summary>
        /// 轴表达式添加均值时Stderr对象的默认标签
        /// </summary>
        public string AxisStderrLabel
        {
            get { return _axisStderrLabel; }
            set { SynchronizeProperty(ref _axisStderrLabel, value, SpecConfigKeys.AXIS_STDERR_LABEL); }
        }

        public static readonly string[] AxisTopBottomBoxPositionSelection =
        {
            "在具体选项之前",
            "在具体选项和Sigma之间",
            "在Sigma之后"
        };

        int _axisTopBottomBoxPosition;
        /// <summary>
        /// 轴表达式中Top/Bottom Box的位置配置，数值对应ComboBox中的选中索引和<see cref="Model.AxisTopBottomBoxPosition"/>枚举类的int值相同。
        /// </summary>
        public int AxisTopBottomBoxPosition
        {
            get { return _axisTopBottomBoxPosition; }
            set { SynchronizeProperty(ref _axisTopBottomBoxPosition, value, SpecConfigKeys.AXIS_TOP_BOTTOM_BOX_POSITION); }
        }

        public static readonly string[] AxisNetTypeSelection =
        {
            "标准Net",
            "在具体选项前的Combine",
            "在Subtotal和具体选项之间的Combine",
            "放在最后的Combine",
        };

        int _axisNetType;
        /// <summary>
        /// 轴表达式默认的Net类型，数值对应<see cref="Model.AxisNetType"/>对应的int值。
        /// </summary>
        public int AxisNetType
        {
            get { return _axisNetType; }
            set { SynchronizeProperty(ref _axisNetType, value, SpecConfigKeys.AXIS_NET_TYPE); }
        }

        string _axisAverageMentionLabel;
        /// <summary>
        /// 轴表达式添加平均提及时默认的行描述
        /// </summary>
        public string AxisAverageMentionLabel
        {
            get { return _axisAverageMentionLabel; }
            set { SynchronizeProperty(ref _axisAverageMentionLabel, value, SpecConfigKeys.AXIS_AVERAGE_MENTION_LABEL); }
        }

        bool _axisAverageMentionBlankLine;
        /// <summary>
        /// 轴表达式添加平均提及时是否在前面添加一行空白行
        /// </summary>
        public bool AxisAverageMentionBlankLine
        {
            get { return _axisAverageMentionBlankLine; }
            set { SynchronizeProperty(ref _axisAverageMentionBlankLine, value, SpecConfigKeys.AXIS_AVERAGE_MENTION_BLANKLINE); }
        }

        string _axisAverageMentionDecimals;
        /// <summary>
        /// 轴表达式添加平均提及时保留的小数位数
        /// </summary>
        public string AxisAverageMentionDecimals
        {
            get { return _axisAverageMentionDecimals; }
            set
            {
                if (int.TryParse(value, out int i))
                {
                    SetProperty(ref _axisAverageMentionDecimals, value);
                    _config.Set(SpecConfigKeys.AXIS_AVERAGE_MENTION_DECIMALS, i);
                    _config.Synchronize(_settings);
                    _settings.Save();
                }
            }
        }

        // Table Settings

        string _tableSummaryLabel;
        /// <summary>
        /// 添加Summary Table时默认添加的后缀描述
        /// </summary>
        public string TableSummaryLabel
        {
            get { return _tableSummaryLabel; }
            set { SynchronizeProperty(ref _tableSummaryLabel, value, SpecConfigKeys.TABLE_SUMMARY_LABEL); }
        }

        string _tableSettingNetLabelCodeSeparater;
        /// <summary>
        /// 载入EXCEL中net配置时标签和码号之间的分隔符号
        /// </summary>
        public string TableSettingNetLabelCodeSeparater
        {
            get { return _tableSettingNetLabelCodeSeparater; }
            set { SynchronizeProperty(ref _tableSettingNetLabelCodeSeparater, value, SpecConfigKeys.TABLE_SETTING_NET_LABEL_CODE_SEPARATER); }
        }

        string _tableSettingNetCodeSeparater;
        /// <summary>
        /// 载入EXCEL中net配置时码号之间的分隔符号
        /// </summary>
        public string TableSettingNetCodeSeparater
        {
            get { return _tableSettingNetCodeSeparater; }
            set { SynchronizeProperty(ref _tableSettingNetCodeSeparater, value, SpecConfigKeys.TABLE_SETTING_NET_CODE_SEPARATER); }
        }

        string _tableSettingNetRangeSeparater;
        /// <summary>
        /// 载入EXCEL中net配置时码号区间的分隔符号
        /// </summary>
        public string TableSettingNetRangeSeparater
        {
            get { return _tableSettingNetRangeSeparater; }
            set { SynchronizeProperty(ref _tableSettingNetRangeSeparater, value, SpecConfigKeys.TABLE_SETTING_NET_RANGE_SEPARATER); }
        }

    }
}
