﻿using IDCA.Model;
using IDCA.Model.MDM;
using IDCA.Model.Spec;
using IDCA.Model.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Bll
{
    /// <summary>
    /// 轴表达式操作类，用于生成各种样式的轴表达式
    /// </summary>
    public class AxisOperator
    {

        public const string DefaultAxisBaseLabel = "Base : Total Respondent";
        public const string DefaultAxisSigmaLabel = "Sigma";
        public const string DefaultAxisNetAheadLabel = "Net.";
        public const string DefaultAxisNetLabelSeparater = ":";
        public const string DefaultAxisNetCodeSeparater = ",";
        public const string DefaultAxisNetCodeRangeSeparater = "-";
        public const string DefaultAxisMeanLabel = "AVERAGE";
        public const string DefaultAxisStdDevLabel = "STANDARD DEVIATION";
        public const string DefaultAxisStdErrLabel = "STANDARD ERROR";
        public const string DefaultAxisAverageMentionLabel = "AVERAGE MENTION";

        public AxisOperator(Axis axis, Config config, TemplateCollection templates)
        {
            _axis = axis;
            _config = config;
            _templates = templates;
            LoadConfig();

            _appendMean = false;
            _appendAverage = false;

            _axisMeanFunction = _templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateAxisMean);
            //_axisAverageFunction = _templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateAxisAverage);

            _meanVariable = string.Empty;
        }

        readonly Axis _axis;
        readonly Config _config;
        readonly TemplateCollection _templates;
        
        /// <summary>
        /// 当前修改器修改的轴表达式对象
        /// </summary>
        public Axis Axis => _axis;

        // 配置属性

        void LoadConfig()
        {
            _topBottomPosition = Converter.ConvertToAxisTopBottomBoxPosition(
                _config.TryGet<AxisTopBottomBoxPosition>(SpecConfigKeys.AxisTopBottomBoxPositon));
            _baseText = _config.TryGet<string>(SpecConfigKeys.AxisSigmaLabel) ?? DefaultAxisBaseLabel;
            _addSigma = _config.TryGet<bool>(SpecConfigKeys.AxisAddSigma);
            _sigmaLabel = _config.TryGet<string>(SpecConfigKeys.AxisSigmaLabel) ?? DefaultAxisSigmaLabel;
            _averageMentionLabel = _config.TryGet<string>(SpecConfigKeys.AxisAverageMentionLabel) ?? DefaultAxisAverageMentionLabel;
            _averageMentionDecimals = _config.TryGet<int>(SpecConfigKeys.AxisAverageMentionDecimals);
            _averageMentionBlanckRow = _config.TryGet<bool>(SpecConfigKeys.AxisAverageMentionBlankRow);
            _emptyLineSeparator = _config.TryGet<bool>(SpecConfigKeys.AxisNetInsertEmptyLine);
            _netAheadLabel = _config.TryGet<string>(SpecConfigKeys.AxisNetAheadLabel) ?? DefaultAxisNetAheadLabel;
            _npsTopBox = _config.TryGet<int>(SpecConfigKeys.AxisNpsTopBox);
            _npsBottomBox = _config.TryGet<int>(SpecConfigKeys.AxisNpsBottomBox);
            _netLabelSeparater = _config.TryGet<string>(SpecConfigKeys.TableSettingNetLabelCodeSeparater) ?? DefaultAxisNetLabelSeparater;
            _netCodeSeparater = _config.TryGet<string>(SpecConfigKeys.TableSettingNetCodeSeparater) ?? DefaultAxisNetCodeSeparater;
            _netCodeRangeSeparater = _config.TryGet<string>(SpecConfigKeys.TableSettingNetCodeRangeSeparater) ?? DefaultAxisNetCodeRangeSeparater;
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
        string _baseText = DefaultAxisBaseLabel;
        // 是否添加Sigma行小计
        bool _addSigma = false;
        // Sigma行的默认描述
        string _sigmaLabel = DefaultAxisSigmaLabel;
        // 计算均值行的默认描述
        string _averageMentionLabel = DefaultAxisAverageMentionLabel;
        // 计算均值行的保留小数位数
        int _averageMentionDecimals = 2;
        // 是否在均值行前插入空白行
        bool _averageMentionBlanckRow = true;
        // 是否在两个Net中间插入空白行
        bool _emptyLineSeparator = false;
        // Net行标签开头描述
        string _netAheadLabel = DefaultAxisNetAheadLabel;
        // 用于计算NPS的Top Box选项数量
        int _npsTopBox = 2;
        // 用于计算NPS的Bottom Box选项数量
        int _npsBottomBox = 6;
        // Net配置中描述和码号两部分的分隔符号
        string _netLabelSeparater = DefaultAxisNetLabelSeparater;
        // 单独码号的分隔符号，例如"V1,V2,V3"中的','
        string _netCodeSeparater = DefaultAxisNetCodeSeparater;
        // 码号区间的上下限分隔符号，例如"V1-V4"中的'-'
        string _netCodeRangeSeparater = DefaultAxisNetCodeRangeSeparater;

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

        string _meanVariable;
        /// <summary>
        /// 用于计算均值/标准差/标准误差的变量，可以是null
        /// </summary>
        public string MeanVariable
        {
            get { return _meanVariable; }
            set { _meanVariable = value; }
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
            var subtotal = _axis.AppendSubTotal();
            subtotal.Suffix.AppendIsHidden(true);
            var total = _axis.AppendNet("", "..");
            total.Suffix.AppendIsHidden(true);
        }

        void AppendMeanStdDevStdErr()
        {
            if (!_appendMean)
            {
                return;
            }

            // 优先添加已有的函数模板
            if (_axisMeanFunction != null)
            {
                _axisMeanFunction.SetFunctionParameterValue(_meanVariable, TemplateValueType.Variable, TemplateParameterUsage.ManipulateFunctionMeanVariable);
                _axis.AppendInsertFunction(_axisMeanFunction);
            }
            else
            {
                _axis.AppendText();
                _axis.AppendMean(DefaultAxisMeanLabel, _meanVariable).Suffix.AppendDecimals(2);
                _axis.AppendStdDev(DefaultAxisStdDevLabel, _meanVariable).Suffix.AppendDecimals(2);
                _axis.AppendStdErr(DefaultAxisStdErrLabel, _meanVariable).Suffix.AppendDecimals(2);
            }
        }

        void AppendAverageMention()
        {
            if (!_appendAverage)
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
            if (_averageMentionBlanckRow)
            {
                _axis.AppendText();
            }
            _axis.AppendNamedNet("AverageBase").Suffix.AppendIsHidden(true);
            _axis.AppendSubTotal("AverageSubTotal").Suffix.AppendIsHidden(true);
            _axis.AppendNamedDerived("AverageMention", _averageMentionLabel, "AverageSubTotal/AverageBase").Suffix.AppendDecimals(_averageMentionDecimals);
            //}
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


    }


}
