using IDCA.Model.MDM;
using IDCA.Model.Spec;
using IDCA.Model.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDCA.Model
{
    /// <summary>
    /// 轴表达式操作类，用于生成各种样式的轴表达式
    /// </summary>
    public class AxisOperator
    {

        public const string AXIS_AVERAGE_BASE_NAME = "AverageBase";
        public const string AXIS_AVERAGE_SUBTOTAL_NAME = "AverageSubTotal";
        public const string AXIS_AVERAGE_DERIVED_NAME = "AverageMention";

        public const string AXIS_MEAN_MEANVALUE = "MeanValue";
        public const string AXIS_MEAN_STDDEVVALUE = "StddevValue";
        public const string AXIS_MEAN_STDERRVALUE = "StderrValue";

        public AxisOperator(Axis axis, Config config, TemplateCollection templates)
        {
            _axis = axis;
            _config = config;
            _templates = templates;
            LoadConfig();

            _appendMean = false;
            _appendAverage = false;
            _averageSkipCodes = string.Empty;

            _axisMeanFunction = _templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateAxisInsertMean);
            //_axisAverageFunction = _templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateAxisAverage);

            _meanVariable = string.Empty;
            _meanFilter = string.Empty;
        }

        readonly Axis _axis;
        readonly Config _config;
        readonly TemplateCollection _templates;

        bool _hasMean = false;
        bool _hasAxisAverage = false;

        /// <summary>
        /// 当前轴是否已经在末尾添加了均值元素
        /// </summary>
        public bool HasMean => _hasMean;
        /// <summary>
        /// 当前轴是否已经在末尾添加了平均提及计算的相关元素
        /// </summary>
        public bool HasAxisAverage => _hasAxisAverage;

        /// <summary>
        /// 当前修改器修改的轴表达式对象
        /// </summary>
        public Axis Axis => _axis;

        // 配置属性

        void LoadConfig()
        {
            _topBottomPosition = Converter.ConvertToAxisTopBottomBoxPosition(
                (AxisTopBottomBoxPosition)_config.Get(SpecConfigKeys.AXIS_TOP_BOTTOM_BOX_POSITION));
            _baseText = _config.Get(SpecConfigKeys.AXIS_BASE_LABEL);
            _addSigma = _config.Get(SpecConfigKeys.AXIS_ADD_SIGMA);
            _sigmaLabel = _config.Get(SpecConfigKeys.AXIS_SIGMA_LABEL);
            _averageMentionLabel = _config.Get(SpecConfigKeys.AXIS_AVERAGE_MENTION_LABEL);
            _averageMentionDecimals = _config.Get(SpecConfigKeys.AXIS_AVERAGE_MENTION_DECIMALS);
            _averageMentionBlanckRow = _config.Get(SpecConfigKeys.AXIS_AVERAGE_MENTION_BLANKLINE);
            _emptyLineSeparator = _config.Get(SpecConfigKeys.AXIS_NET_INSERT_EMPTYLINE);
            _netAheadLabel = _config.Get(SpecConfigKeys.AXIS_NET_AHEAD_LABEL);
            _npsTopBox = _config.Get(SpecConfigKeys.AXIS_NPS_TOP_BOX);
            _npsBottomBox = _config.Get(SpecConfigKeys.AXIS_NPS_BOTTOM_BOX);
            _netLabelSeparater = _config.Get(SpecConfigKeys.TABLE_SETTING_NET_LABEL_CODE_SEPARATER);
            _netCodeSeparater = _config.Get(SpecConfigKeys.TABLE_SETTING_NET_CODE_SEPARATER);
            _netCodeRangeSeparater = _config.Get(SpecConfigKeys.TABLE_SETTING_NET_RANGE_SEPARATER);
        }

        readonly FunctionTemplate? _axisMeanFunction;
        //readonly FunctionTemplate? _axisAverageFunction;

        AxisNetType _netType = AxisNetType.StandardNet;
        /// <summary>
        /// 轴表达式Net的类型
        /// </summary>
        public AxisNetType NetType
        {
            get { return _netType; }
            set { _netType = value; }
        }

        // Top Bottom Box 所在轴表达式的位置
        AxisTopBottomBoxPosition _topBottomPosition = AxisTopBottomBoxPosition.BeforeAllCategory;
        // 默认的Base行描述
        string _baseText = SpecConfigDefaultValue.AXIS_BASE_LABEL;
        // 是否添加Sigma行小计
        bool _addSigma;
        // Sigma行的默认描述
        string _sigmaLabel = SpecConfigDefaultValue.AXIS_SIGMA_LABEL;
        // 计算均值行的默认描述
        string _averageMentionLabel = SpecConfigDefaultValue.AXIS_AVERAGE_MENTION_LABEL;
        // 计算均值行的保留小数位数
        int _averageMentionDecimals;
        // 是否在均值行前插入空白行
        bool _averageMentionBlanckRow;
        // 是否在两个Net中间插入空白行
        bool _emptyLineSeparator;
        // Net行标签开头描述
        string _netAheadLabel = SpecConfigDefaultValue.AXIS_NET_AHEAD_LABEL;
        // 用于计算NPS的Top Box选项数量
        int _npsTopBox;
        // 用于计算NPS的Bottom Box选项数量
        int _npsBottomBox;
        // Net配置中描述和码号两部分的分隔符号
        string _netLabelSeparater = SpecConfigDefaultValue.TABLE_SETTING_NET_LABEL_CODE_SEPARATER;
        // 单独码号的分隔符号，例如"V1,V2,V3"中的','
        string _netCodeSeparater = SpecConfigDefaultValue.TABLE_SETTING_NET_CODE_SEPARATER;
        // 码号区间的上下限分隔符号，例如"V1-V4"中的'-'
        string _netCodeRangeSeparater = SpecConfigDefaultValue.TABLE_SETTING_NET_RANGE_SEPARATER;

        bool _isTopBottomBoxReversed = false;
        /// <summary>
        /// Top/Bottom Box码号顺序是否是反转的，以存储顺序从0开始为正序
        /// true  - 顺序为反序，索引越低，Factor越高
        /// false - 顺序为正序，索引越低，Factor越低
        /// </summary>
        public bool IsTopBottomBoxReversed
        {
            get { return _isTopBottomBoxReversed; }
            set { _isTopBottomBoxReversed = value; }
        }

        /// <summary>
        /// NPS使用的Top Box选项数量，如果不修改，默认为配置中的数值。
        /// </summary>
        public int NpsTopBox
        {
            get { return _npsTopBox; }
            set { _npsTopBox = value; }
        }
        /// <summary>
        /// NPS使用的Bottom Box选项数量，如果不修改，默认未配置中的数值。
        /// </summary>
        public int NpsBottomBox
        {
            get { return _npsBottomBox; }
            set { _npsBottomBox = value; }
        }

        bool _appendMean;
        /// <summary>
        /// 当前轴表达式是否添加均值，和AppendAverage不能同时为true
        /// </summary>
        public bool AppendMean
        {
            get { return _appendMean; }
            set 
            { 
                _appendMean = value;
                if (value)
                {
                    _appendAverage = false;
                }
            }
        }

        bool _appendAverage;
        /// <summary>
        /// 当前轴表达式是否添加平均提及数，和AppendMean不能同时为true
        /// </summary>
        public bool AppendAverage
        {
            get { return _appendAverage; }
            set 
            { 
                _appendAverage = value;
                if (value)
                {
                    _appendMean = false;
                }
            }
        }

        string _averageSkipCodes;
        /// <summary>
        /// 当前轴表达式添加平均提及数时跳过的码号
        /// </summary>
        public string AverageSkipCodes
        {
            get { return _averageSkipCodes; }
            set { _averageSkipCodes = value; }
        }

        string _meanVariable;
        /// <summary>
        /// 用于计算均值/标准差/标准误差的变量，可以是null
        /// </summary>
        public string MeanVariable
        {
            get { return _meanVariable; }
            set { _meanVariable = value; }
        }

        string _meanFilter;
        /// <summary>
        /// 计算均值/标准差/标准误差时使用的Filter表达式
        /// </summary>
        public string MeanFilter
        {
            get { return _meanFilter; }
            set { _meanFilter = value; }
        }


        Field? _field = null;
        /// <summary>
        /// 当前轴表达式对应变量的MDMField对象，用来查询使用的码号
        /// </summary>
        public Field? Field
        {
            get { return _field; }
            set { _field = value; }
        }

        /// <summary>
        /// 在当前表达式中查找第一个指定类型的下级元素，如果没找到，返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AxisElement? First(AxisElementType type)
        {
            return _axis.Find(e => e.Template.ElementType == type);
        }

        /// <summary>
        /// 在当前表达式中查找第一个指定类型的下级元素，如果没找到，返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public AxisElement? First(Predicate<AxisElement> predicate)
        {
            return _axis.Find(predicate);
        }

        /// <summary>
        /// 遍历轴表达式元素并执行回调函数
        /// </summary>
        /// <param name="callback"></param>
        public void All(Action<AxisElement> callback)
        {
            foreach (AxisElement element in _axis)
            {
                callback(element);
            }
        }

        /// <summary>
        /// 创建基础的轴表达式
        /// </summary>
        /// <param name="baseText">Base行描述</param>
        public void CreateBasicAxisExpression(string? baseText)
        {
            _axis.Clear();
            _axis.AppendText();
            _axis.AppendBase($"Base : {baseText ?? _baseText}");
            _axis.AppendText();
            _axis.AppendCategoryRange();
            _axis.AppendText();
            _axis.AppendSubTotal(_sigmaLabel);
            AppendMeanStdDevStdErr();
            AppendAverageMention();
        }

        /// <summary>
        /// 向当前的轴表达式中追加单个Top/Bottom Box Combine轴元素
        /// </summary>
        /// <param name="box">Box数量，不能为0，正数为TopBox，负数为BottomBox</param>
        /// <param name="afterAppend">在AxisElement创建完成后，对添加的对象执行的回调函数</param>
        void AppendTopBottomBox(int box, Action<AxisElement>? afterAppend = null)
        {
            // Top Bottom Box相关表达式必须使用Field对象，如果Field为null，将直接跳出
            if (_field == null || _field.Categories == null)
            {
                Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, $"{(box < 0 ? "Bottom" : "Top")} {Math.Abs(box)} Box");
                return;
            }

            int absBox = Math.Abs(box);
            // Top Bottom Box值必须不等于0且其绝对值小于等于Categorical列表总数
            if (box == 0 || absBox > _field.Categories.Count)
            {
                Logger.Warning("SettingError", ExceptionMessages.SettingTopBottomBoxInvalid);
                return;
            }

            var codes = new StringBuilder();

            // 正向选取的情况
            if ((box > 0 && !_isTopBottomBoxReversed) || 
                (box < 0 && _isTopBottomBoxReversed))
            {
                for (int i = 0; i < box; i++)
                {
                    codes.Append($"{(i == 0 ? "" : ", ")}{_field.Categories[i]?.Name}");
                }
            }
            // 反向选取的情况
            else if ((box > 0 && _isTopBottomBoxReversed) ||
                (box < 0 && !_isTopBottomBoxReversed))
            {
                for (int i = _field.Categories.Count - absBox; i < _field.Categories.Count; i++)
                {
                    codes.Append($"{(i == 0 ? "" : ", ")}{_field.Categories[i]?.Name}");
                }
            }

            var ele = _axis.AppendNamedCombine($"{(box > 0 ? "t" : "b")}{absBox}b", $"{_netAheadLabel}{(box > 0 ? "Top " : "Bottom ")}{absBox} Box", codes.ToString());
            afterAppend?.Invoke(ele);

            if (_emptyLineSeparator)
            {
                _axis.AppendText();
            }
        }

        void AppendNps()
        {
            // 检查NPS计算用的Top Box
            if (_axis.Find(e => e.Name.Equals($"t{_npsTopBox}b", StringComparison.OrdinalIgnoreCase)) == null)
            {
                AppendTopBottomBox(_npsTopBox, e =>
                {
                    e.Suffix.AppendIsHidden(true);
                    e.Suffix.AppendIsFixed(true);
                });
            }
            // 检查NPS计算用的Bottom Box
            if (_axis.Find(e => e.Name.Equals($"b{_npsBottomBox}b", StringComparison.OrdinalIgnoreCase)) == null)
            {
                AppendTopBottomBox(_npsBottomBox, e =>
                {
                    e.Suffix.AppendIsHidden(true);
                    e.Suffix.AppendIsFixed(true);
                });
            }

            _axis.AppendNamedDerived("nps", $"NPS(T{_npsTopBox}B-B{_npsBottomBox}B)", $"t{_npsTopBox}b-b{_npsBottomBox}b");

        }

        void AppendNetSeparater()
        {
            var subtotal = _axis.AppendSubTotal();
            subtotal.Suffix.AppendIsHidden(true);
            subtotal.Suffix.AppendIsFixed(true);
            if (_emptyLineSeparator)
            {
                _axis.AppendText();
            }
        }

        void AppendExactSubtotalSeparater()
        {
            _axis.AppendSubTotal().Suffix.AppendIsHidden(true);
            _axis.AppendNet("", "..").Suffix.AppendIsHidden(true);
        }

        /// <summary>
        /// 向当前轴末尾追加平均数/标准误差/标准差轴元素
        /// </summary>
        public void AppendMeanStdDevStdErr()
        {
            if (!_appendMean || _hasMean)
            {
                return;
            }

            // 优先添加已有的函数模板
            if (_axisMeanFunction != null && (string.IsNullOrEmpty(_meanFilter) || (!string.IsNullOrEmpty(_meanFilter) && _axisMeanFunction.Parameters[TemplateParameterUsage.MannipulateFilter] != null)))
            {
                _axisMeanFunction.SetFunctionParameterValue(_meanVariable, TemplateValueType.Variable, TemplateParameterUsage.ManipulateMeanVariable);
                if (!string.IsNullOrEmpty(_meanFilter))
                {
                    _axisMeanFunction.SetFunctionParameterValue(_meanFilter, TemplateValueType.String, TemplateParameterUsage.MannipulateFilter);
                }
                _axis.AppendInsertFunction(_axisMeanFunction);
            }
            else
            {
                if (_axis[^1].Template.ElementType != AxisElementType.Text)
                {
                    _axis.AppendText();
                }
                _axis.AppendNamedMean(AXIS_MEAN_MEANVALUE, SpecConfigDefaultValue.AXIS_MEAN_LABEL, _meanVariable, _meanFilter).Suffix.AppendDecimals(2);
                _axis.AppendNamedStdDev(AXIS_MEAN_STDDEVVALUE, SpecConfigDefaultValue.AXIS_STDDEV_LABEL, _meanVariable, _meanFilter).Suffix.AppendDecimals(2);
                _axis.AppendNamedStdErr(AXIS_MEAN_STDERRVALUE, SpecConfigDefaultValue.AXIS_STDERR_LABEL, _meanVariable, _meanFilter).Suffix.AppendDecimals(2);
            }

            _hasMean = true;
        }

        /// <summary>
        /// 移除当前轴中已经添加MEAN/STDDEV/STDERR类型的轴元素
        /// </summary>
        public void RemoveMeanStdDevStdErr()
        {
            if (!_hasMean)
            {
                return;
            }

            FunctionTemplate? param;

            _axis.RemoveIf(e => e.Name.Equals(AXIS_MEAN_MEANVALUE) ||
                                e.Name.Equals(AXIS_MEAN_STDDEVVALUE) ||
                                e.Name.Equals(AXIS_MEAN_STDERRVALUE) || (
                                e.Template.ElementType == AxisElementType.InsertFunctionOrVariable &&
                                (param = e.Template.GetParameter(0)?.GetValue() as FunctionTemplate) != null &&
                                param.Flag == FunctionTemplateFlags.ManipulateAxisInsertMean));
            
            if (_axis[^1].Template.ElementType == AxisElementType.Text)
            {
                _axis.RemoveAt(_axis.Count - 1);
            }

            _hasMean = false;
        }

        /// <summary>
        /// 向当前轴的末尾追加计算平均提及的相关元素
        /// </summary>
        public void AppendAverageMention()
        {
            if (!_appendAverage || _hasAxisAverage)
            {
                return;
            }

            //if (_axisAverageFunction != null)
            //{
            //    _axisAverageFunction.SetFunctionParameterValue(_field?.FullName ?? string.Empty, TemplateValueType.String, TemplateParameterUsage.ManipulateFieldName);
            //    _axisAverageFunction.SetFunctionParameterValue(_averageMentionLabel, TemplateValueType.String, TemplateParameterUsage.ManipulateLabelText);
            //    _axisAverageFunction.SetFunctionParameterValue(_averageMentionDecimals.ToString(), TemplateValueType.String, TemplateParameterUsage.ManipulateDecimals);
            //    _axisAverageFunction.SetFunctionParameterValue(_averageMentionBlanckRow.ToString(), TemplateValueType.Expression, TemplateParameterUsage.ManipulateBlanckRow);
            //    _axisAverageFunction.SetFunctionParameterValue("Null", TemplateValueType.Expression, TemplateParameterUsage.ManipulateExclude);
            //}
            //else
            //{
            _axis.AppendSubTotal().Suffix.AppendIsHidden(true);
            if (_averageMentionBlanckRow && _axis[^1].Template.ElementType != AxisElementType.Text)
            {
                _axis.AppendText();
            }
            _axis.AppendNamedNet(AXIS_AVERAGE_BASE_NAME, null, $"..{(string.IsNullOrEmpty(_averageSkipCodes) ? "" : $",^{_averageSkipCodes.Replace(",", ",^")}")}").Suffix.AppendIsHidden(true);
            _axis.AppendSubTotal(AXIS_AVERAGE_SUBTOTAL_NAME).Suffix.AppendIsHidden(true);
            _axis.AppendNamedDerived(AXIS_AVERAGE_DERIVED_NAME, _averageMentionLabel, "AverageSubTotal/AverageBase").Suffix.AppendDecimals(_averageMentionDecimals);
            _hasAxisAverage = true;
            //}
        }

        /// <summary>
        /// 移除当前已经添加在末尾的平均提及计算元素
        /// </summary>
        public void RemoveAverageMention()
        {
            if (!_hasAxisAverage)
            {
                return;
            }

            _axis.RemoveIf(e => e.Name.Equals(AXIS_AVERAGE_BASE_NAME) || 
                                e.Name.Equals(AXIS_AVERAGE_DERIVED_NAME) ||
                                e.Name.Equals(AXIS_AVERAGE_SUBTOTAL_NAME));

            if (_axis[^1].Template.ElementType == AxisElementType.Text)
            {
                _axis.RemoveAt(_axis.Count - 1);
            }

            _hasAxisAverage = false;
        }

        /// <summary>
        /// 创建基于MDMField对象分类选项列表的Top/Bottom Box轴表达式，允许添加NPS和多个Box
        /// </summary>
        /// <param name="baseText">Base行描述</param>
        /// <param name="addNps">是否添加NPS</param>
        /// <param name="boxes">任意多个Box值，正数为Top，负数为Bottom，不能为0</param>
        public void CreateTopBottomBoxAxisExpression(string? baseText, bool addNps, params int[] boxes)
        {
            // Top Bottom Box相关表达式必须使用Field对象，如果Field为null，将直接跳出
            if (_field == null)
            {
                return;
            }

            _axis.Clear();

            _axis.AppendText();
            _axis.AppendBase($"Base : {baseText ?? _baseText}");
            _axis.AppendText();

            if (_topBottomPosition == AxisTopBottomBoxPosition.BeforeAllCategory)
            {
                foreach (var box in boxes)
                {
                    AppendTopBottomBox(box);
                }
                if (addNps)
                {
                    AppendNps();
                }
                AppendNetSeparater();
            }

            _axis.AppendCategoryRange();

            if (_topBottomPosition == AxisTopBottomBoxPosition.BetweenAllCategoryAndSigma)
            {
                if (_emptyLineSeparator)
                {
                    _axis.AppendText();
                }
                foreach (var box in boxes)
                {
                    AppendTopBottomBox(box);
                }
                if (addNps)
                {
                    AppendNps();
                }
                AppendExactSubtotalSeparater();
            }

            if (_addSigma)
            {
                _axis.AppendText();
                _axis.AppendSubTotal(_sigmaLabel);
            }

            if (_topBottomPosition == AxisTopBottomBoxPosition.AfterSigma)
            {
                if (_emptyLineSeparator)
                {
                    _axis.AppendText();
                }
                foreach (var box in boxes)
                {
                    AppendTopBottomBox(box);
                }
                if (addNps)
                {
                    AppendNps();
                }
            }

            AppendMeanStdDevStdErr();

        }

        string FindCode(string code)
        {
            // 如果code参数不是整数，则认为此码号是完整码号，直接返回传入值
            if (!StringHelper.IsInteger(code) || 
                _field == null || 
                _field.Categories == null)
            {
                return code;
            }

            foreach (Element category in _field.Categories)
            {
                string number = StringHelper.NumberAtRight(category.Name);
                if (!string.IsNullOrEmpty(number) && number.Equals(code))
                {
                    return category.Name;
                }
            }

            return code;
        }

        bool MatchRangeCodes(List<string> range, string name)
        {
            if (_field == null || _field.Categories == null)
            {
                return false;
            }

            for (int i = 0; i < range.Count; i++)
            {
                var code = range[i];
                bool isInteger = StringHelper.IsInteger(code);
                if ((isInteger && StringHelper.NumberAtRight(name).Equals(code)) ||
                    (!isInteger && name.Equals(code, StringComparison.OrdinalIgnoreCase)))
                {
                    range.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        void FindCodeRange(ref string[] resultCodes, string[] range)
        {
            if (_field == null || _field.Categories == null || range.Length != 2)
            {
                return;
            }

            var rangeCodes = new List<string>(range);
            bool selected = false;
            foreach (Element element in _field.Categories)
            {
                bool matchRange = MatchRangeCodes(rangeCodes, element.Name);

                if (!selected && matchRange)
                {
                    selected = true;
                }

                if (selected)
                {
                    Array.Resize(ref resultCodes, resultCodes.Length + 1);
                    resultCodes[^1] = element.Name;
                }

                if (selected && matchRange)
                {
                    break;
                }

            }

        }

        string[] ReadCodes(string codes)
        {
            string[] result = Array.Empty<string>();

            foreach (var code in codes.Split(_netCodeSeparater))
            {
                var codeRange = code.Split(_netCodeRangeSeparater);

                if (codeRange.Length == 1)
                {
                    Array.Resize(ref result, result.Length + 1);
                    result[^1] = FindCode(code);
                }
                else if (codeRange.Length == 2)
                {
                    FindCodeRange(ref result, codeRange);
                }
                else
                {
                    Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, code);
                }
            }

            return result;
        }

        struct AxisNetElement
        {
            public string Label;
            public string Codes;
        }

        List<AxisNetElement> ReadNets(string config)
        {
            var result = new List<AxisNetElement>();

            foreach (var net in config.Split('\n'))
            {
                AxisNetElement element;
                var singleNet = net.Split(_netLabelSeparater);
                if (singleNet.Length == 1)
                {
                    element.Label = "";
                    element.Codes = string.Join(',', ReadCodes(singleNet[0]));
                    result.Add(element);
                }
                else if (singleNet.Length == 2)
                {
                    element.Label = singleNet[0];
                    element.Codes = string.Join(',', ReadCodes(singleNet[1]));
                    result.Add(element);
                }
                else
                {
                    Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, net);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建添加Net的轴表达式，轴表达式配置需要用'\n'分隔
        /// </summary>
        /// <param name="netSetting"></param>
        /// <param name="baseText"></param>
        public void CreateNettedAxisExpression(string netSetting, string? baseText = null)
        {

            if (_field == null || _field.Categories == null)
            {
                return;
            }

            var netElements = ReadNets(netSetting);

            _axis.Clear();
            _axis.AppendText();
            _axis.AppendBase(baseText ?? _baseText);
            _axis.AppendText();

            if (_netType == AxisNetType.StandardNet)
            {
                netElements.ForEach(e =>
                {
                    _axis.AppendNet(e.Label, e.Codes);
                    if (_emptyLineSeparator)
                    {
                        _axis.AppendText();
                    }
                });
            }

            if (_netType == AxisNetType.CombineBeforeAllCategory)
            {
                netElements.ForEach(e => _axis.AppendCombine(e.Label, e.Codes));
                AppendNetSeparater();
                _axis.AppendCategoryRange();
            }

            if (_netType == AxisNetType.CombineBetweenAllCategoryAndSigma)
            {
                _axis.AppendCategoryRange();
                _axis.AppendText();
                netElements.ForEach(e => _axis.AppendCombine(e.Label, e.Codes));
                AppendExactSubtotalSeparater();
            }

            if (_addSigma)
            {
                if (_axis[^1].Template.ElementType != AxisElementType.Text)
                {
                    _axis.AppendText();
                }
                _axis.AppendSubTotal(_sigmaLabel);
            }

            if (_netType == AxisNetType.CombineAfterSigma)
            {
                netElements.ForEach(e => _axis.AppendCombine(e.Label, e.Codes));
            }
            AppendMeanStdDevStdErr();
            AppendAverageMention();
        }

        /// <summary>
        /// 根据<seealso cref="AxisNetType"/>调整现有轴元素的顺序
        /// </summary>
        /// <param name="netType"></param>
        public void Update(AxisNetType netType)
        {
            var elements = _axis.Elements(e => e.Template.ElementType == AxisElementType.Net || e.Template.ElementType == AxisElementType.Combine);
            if (!elements.Any())
            {
                return;
            }

            AxisElement? sigma = null;
            var subtotals = _axis.Elements(e => e.Template.ElementType == AxisElementType.SubTotal);
            if (subtotals.Any())
            {
                sigma = subtotals.Last();
            }

            if (netType == AxisNetType.CombineAfterSigma || (netType == AxisNetType.CombineBetweenAllCategoryAndSigma && sigma == null))
            {
                foreach (var item in elements)
                {
                    _axis.Remove(item);
                    _axis.Add(item);
                }
            }
            else if (netType == AxisNetType.CombineBetweenAllCategoryAndSigma && sigma != null)
            {
                foreach (var item in elements)
                {
                    _axis.Remove(item);
                }

                int sigmaIndex = _axis.IndexOf(sigma);
                if (sigmaIndex <= 0)
                {
                    return;
                }

                int count = 0;
                foreach (var item in elements)
                {
                    _axis.Insert(sigmaIndex - 1 + count, item);
                    count++;
                }
            }
            else if (netType == AxisNetType.CombineBeforeAllCategory)
            {
                var bases = _axis.Elements(e => e.Template.ElementType == AxisElementType.Base);
                if (bases.Any())
                {
                    var firstBase = bases.First();
                    int insertStartIndex = _axis.IndexOf(firstBase) + 1;
                    if (_axis[insertStartIndex + 1]?.Template.ElementType == AxisElementType.Text)
                    {
                        insertStartIndex++;
                    }

                    int count = 0;
                    foreach (var item in elements)
                    {
                        _axis.Remove(item);
                        _axis.Insert(insertStartIndex + count, item);
                        count++;
                    }
                }
            }
        }


    }


}
