
namespace IDCA.Model.Spec
{
    public class AxisParser
    {

        public AxisParser(SpecObject parent)
        {
            _axis = new Axis(parent, AxisType.Normal);
        }

        public AxisParser(Axis axis)
        {
            _axis = axis;
        }

        readonly Axis _axis;

        int _pos = 0;
        string _current = string.Empty;
        string _exp = string.Empty;
        int _length = 0;
        ParserElementType _type = ParserElementType.None;

        bool _exclude = false;

        enum ParserElementType
        {
            None,
            ElementName,
            ElementLabel,
            ElementTemplateName,
            ElementSuffixName,

            LeftCurlyBrace,
            RightCurlyBrace,
            LeftBracket,
            RightBracket,
            LeftParenthesis,
            RightParenthesis,

            Comma,
            Equal,

            Dotdot,

            Caret
        }

        void SkipSpace()
        {
            while (_pos < _length && IsSpace(_exp[_pos]))
            {
                _pos++;
            }
        }

        void SkipUntil(ParserElementType type)
        {
            if (_type == type)
            {
                return;
            }

            var t = Next();
            while (t != ParserElementType.None && t != type)
            {
                t = Next();
            }
        }

        bool Expect(ParserElementType type, bool lookAhead = true)
        {
            return (lookAhead ? LookAhead() : Next()) == type;
        }

        string ReadWord()
        {
            int length = 0;
            int start = _pos;
            while (_pos < _length && IsValidCharacter(_exp[_pos]))
            {
                length++;
                _pos++;
            }
            _current = _exp.Substring(start, length);
            _type = ParserElementType.ElementName;
            return _current;
        }

        string ReadLabel()
        {
            // 跳过开头的单引号
            _pos++;
            int length = 0;
            int start = _pos;
            while (_pos < _length && _exp[_pos] != '\'')
            {
                length++;
                _pos++;
            }
            _current = _exp.Substring(start, length);
            _type = ParserElementType.ElementLabel;
            // 跳过结尾的单引号
            _pos++;
            return _current;
        }

        char LookAheadChar()
        {
            int backPos = _pos;
            char result;
            SkipSpace();
            if (_pos < _length)
            {
                result = _exp[_pos];
            }
            else
            {
                result = ' ';
            }
            _pos = backPos;
            return result;
        }

        ParserElementType Next()
        {
            SkipSpace();
            if (_pos >= _length)
            {
                return ParserElementType.None;
            }

            char c = _exp[_pos];
            if (IsValidCharacter(c))
            {
                ReadWord();
                char n = LookAheadChar();
                if (n == '(')
                {
                    _type = ParserElementType.ElementTemplateName;
                }
                else if (n == '=')
                {
                    _type = ParserElementType.ElementSuffixName;
                }
                else
                {
                    _type = ParserElementType.ElementName;
                }
            }
            else if (c == '\'')
            {
                ReadLabel();
                _type = ParserElementType.ElementLabel;
            }
            else if (c == '=')
            {
                _pos++;
                _current = "=";
                _type = ParserElementType.Equal;
            }
            else if (c == '[')
            {
                _pos++;
                _current = "[";
                _type = ParserElementType.LeftBracket;
            }
            else if (c == ']')
            {
                _pos++;
                _current = "]";
                _type = ParserElementType.RightBracket;
            }
            else if (c == '{')
            {
                _pos++;
                _current = "{";
                _type = ParserElementType.LeftCurlyBrace;
            }
            else if (c == '}')
            {
                _pos++;
                _current = "}";
                _type = ParserElementType.RightCurlyBrace;
            }
            else if (c == '(')
            {
                _pos++;
                _current = "(";
                _type = ParserElementType.LeftParenthesis;
            }
            else if (c == ')')
            {
                _pos++;
                _current = ")";
                _type = ParserElementType.RightParenthesis;
            }
            else if (c == ',')
            {
                _pos++;
                _current = ",";
                _type = ParserElementType.Comma;
            }
            else if (c == '.' && _pos < _length - 1 && _exp[_pos + 1] == '.')
            {
                _pos += 2;
                _current = "..";
                _type = ParserElementType.Dotdot;
            }
            else if (c == '^')
            {
                _pos++;
                _current = "^";
                _type = ParserElementType.Caret;
            }
            else
            {
                _pos++;
                _current = string.Empty;
                _type = ParserElementType.None;
            }

            return _type;
        }

        ParserElementType LookAhead()
        {
            var backType = _type;
            var backValue = _current;
            var backPos = _pos;
            var result = Next();
            _type = backType;
            _current = backValue;
            _pos = backPos;
            return result;
        }

        void ReadSuffixElement(AxisElementSuffixCollection suffixes)
        {
            if (_type != ParserElementType.ElementSuffixName)
            {
                SkipUntil(ParserElementType.None);
                return;
            }

            var suffix = suffixes.NewObject();
            suffix.Type = Converter.ConvertToAxisElementSuffixType(_current);
            // 跳过 '='
            Next();
            if (Expect(ParserElementType.ElementName))
            {
                Next();
                suffix.Value = _current;
            }
            suffixes.Add(suffix);
        }

        void ReadSuffix(AxisElementSuffixCollection suffixes)
        {
            var t = Next();
            while (t != ParserElementType.None &&
                   t != ParserElementType.RightBracket)
            {
                ReadSuffixElement(suffixes);
                t = Next();
                // 跳过','
                if (t == ParserElementType.Comma)
                {
                    t = Next();
                }
            }
            // 跳过']'
            Next();
        }

        AxisElement ReadElement(SpecObject parent)
        {

            if (_type == ParserElementType.Caret)
            {
                _exclude = true;
                Next();
            }

            var element = new AxisElement(parent) { Exclude = _exclude };
            _exclude = false;
            // ..[Upper]
            if (_type == ParserElementType.Dotdot)
            {
                element.Template.ElementType = AxisElementType.CategoryRange;
                Next();
                if (_type == ParserElementType.ElementName)
                {
                    var parameter = element.Template.NewParameter();
                    parameter.SetValue(_current);
                    element.Template.PushParameter(parameter);
                    Next();
                }
                return element;
            }

            if (_type != ParserElementType.ElementName)
            {
                SkipUntil(ParserElementType.None);
                return element;
            }
            // Lower .. [Upper]
            if (LookAhead() == ParserElementType.Dotdot)
            {
                element.Template.ElementType = AxisElementType.CategoryRange;
                var parameter = element.Template.NewParameter();
                parameter.SetValue(_current);
                element.Template.PushParameter(parameter);
                // 跳过'..'
                Next();
                if (LookAhead() == ParserElementType.ElementName)
                {
                    Next();
                    parameter = element.Template.NewParameter();
                    parameter.SetValue(_current);
                    element.Template.PushParameter(parameter);
                    Next();
                }
                return element;
            }

            element.Name = _current;

            // 读取标签
            if (Expect(ParserElementType.ElementLabel))
            {
                Next();
                element.Description = _current;
            }

            // 读取Template
            if (Expect(ParserElementType.ElementTemplateName))
            {
                Next();
                element.Template.ElementType = Converter.ConvertToAxisElementType(_current);
                Next();
                ReadTemplate(element);
            }
            else
            {
                element.Template.ElementType = AxisElementType.Category;
                Next();
            }

            if (_type == ParserElementType.LeftBracket)
            {
                ReadSuffix(element.Suffix);
            }

            return element;
        }

        void ReadTemplate(AxisElement element)
        {
            if (_type != ParserElementType.LeftParenthesis)
            {
                SkipUntil(ParserElementType.None);
                return;
            }

            // 跳过'('
            var t = Next();
            while (t != ParserElementType.None &&
                   t != ParserElementType.RightParenthesis)
            {
                if (_type == ParserElementType.LeftCurlyBrace)
                {
                    ReadCategories(element);
                }
                else
                {
                    var p = element.Template.NewParameter();
                    p.SetValue(_current);
                    element.Template.PushParameter(p);
                    t = Next();
                }
            }
            // 跳过')'
            Next();
        }

        void ReadCategories(AxisElement element)
        {
            if (_type != ParserElementType.LeftCurlyBrace)
            {
                SkipUntil(ParserElementType.None);
                return;
            }
            // 跳过'{'
            var t = Next();
            while (t != ParserElementType.None &&
                   t != ParserElementType.RightCurlyBrace)
            {
                var parameter = element.Template.NewParameter();
                parameter.SetValue(ReadElement(element.Template));
                element.Template.PushParameter(parameter);
                t = Next();
                if (_type == ParserElementType.Comma)
                {
                    t = Next();
                }
            }
            // 跳过'}'
            Next();
        }

        public Axis Parse(string expression)
        {
            if (!(expression.StartsWith('{') && expression.EndsWith('}')))
            {
                return _axis;
            }
            _exp = expression[1..^1];
            _length = _exp.Length;

            var t = Next();
            while (t != ParserElementType.None)
            {
                _axis.Add(ReadElement(_axis));
                t = Next();
            }

            return _axis;
        }

        static bool IsSpace(char character)
        {
            return character == 32  || 
                   character == 160 || 
                   character == 9;
        }

        static bool IsValidCharacter(char character)
        {
            return ('a' <= character && character <= 'z') ||
                   ('A' <= character && character <= 'Z') ||
                   ('0' <= character && character <= '9') ||
                   character == '@' || 
                   character == '#' ||
                   character == '_' ||
                   character == '$';
        }



    }
}
