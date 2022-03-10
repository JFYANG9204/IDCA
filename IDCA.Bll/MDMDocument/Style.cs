
namespace IDCA.Bll.MDMDocument
{
    public struct Style
    {
        public bool? UseCascadedStyles;
        public string? Color;
        public string? BgColor;
        public bool? Hidden;
        public Alignments? Align;
        public ElementAlignments? ElementAlign;
        public int? Indent;
        public int? ZIndex;
        public CursorType? Cursor;
        public string? Image;
        public ImagePosition? ImagePosition;
        public Orientation? Orientation;
        public int? Columns;
        public int? Rows;
        public string? Width;
        public string? Height;
        public CellStyle? Cell;
        public ControlStyle? Control;
        public FontStyle? Font;
        public AudioStyle? Audio;
    }

    public enum Alignments
    {
        Default,
        Left,
        Center,
        Right,
        Justify
    }

    public enum ElementAlignments
    {
        Default,
        Right,
        NewLine,
    }

    public enum CursorType
    {
        Auto,
        CrossHair,
        Default,
        Pointer,
        Move,
        EResize,
        NEResize,
        NResize,
        NWResize,
        WResize,
        SWResize,
        SResize,
        SEResize,
        Text,
        Wait,
        Help
    }

    public enum ImagePosition
    {
        Left,
        Right,
        Top,
        Bottom,
        ImageOnly,
        None
    }

    public enum Orientation
    {
        Default,
        Row,
        Column
    }

    public struct CellStyle
    {
        public string Width;
        public string Height;
        public string BorderColor;
        public string BorderTopColor;
        public string BorderRightColor;
        public string BorderBottomColor;
        public string BorderLeftColor;
        public BorderStyle BorderStyle;
        public BorderStyle BorderTopStyle;
        public BorderStyle BorderRightStyle;
        public BorderStyle BorderBottomStyle;
        public BorderStyle BorderLeftStyle;
        public int BorderWidth;
        public int BorderTopWidth;
        public int BorderRightWidth;
        public int BorderBottomWidth;
        public int BorderLeftWidth;
        public int Padding;
        public int PaddingTop;
        public int PaddingRight;
        public int PaddingBottom;
        public int PaddingLeft;
        public bool Wrap;
        public int ColSpan;
        public int RowSpan;
        public string BgColor;
        public int RepeatHeader;
        public int RepeatSideHeader;
    }

    public enum BorderStyle
    {
        None,
        Solid,
        Double,
        Groove,
        Ridge,
        Inset,
        Outset
    }

    public struct ControlStyle
    {
        public ControlType Type;
        public bool ReadOnly;
        public string Accelerator;
    }

    public enum ControlType
    {
        Static,
        Edit,
        SingleLineEdit,
        MultiLineEdit,
        DropList,
        ComboList,
        RadioButton,
        CheckButton,
        ListBox,
        ListControl,
        Button,
        Date,
        Time,
        DateTime,
        Password
    }

    public struct FontStyle
    {
        public string Family;
        public int Size;
        public int Effects;
    }

    public struct AudioStyle
    {
        public string Name;
        public AudioControlPosition PlayControlPosition;
        public AudioControlPosition RecordControlPosition;
        public RecordMode Record;
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name) &&
                    PlayControlPosition == AudioControlPosition.None &&
                    RecordControlPosition == AudioControlPosition.None &&
                    Record == RecordMode.None;
            }
        }
    }

    public enum AudioControlPosition
    {
        None = -1,
        Left,
        Right,
        Top,
        Bottom
    }

    public enum RecordMode
    {
        None,
        Manual,
        Auto,
        Prohibited
    }

}
