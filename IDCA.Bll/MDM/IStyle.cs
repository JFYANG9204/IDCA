
namespace IDCA.Bll.MddDocument
{
    public interface IStyle
    {
        IStyle Parent { get; }
        bool UseCascadedStyles { get; }
        string Color { get; }
        string BgColor { get; }
        bool Hidden { get; }
        Alignments Align { get; }
        ElementAlignments ElementAlign { get; }
        int Indent { get; }
        int ZIndex { get; }
        CursorType Cursor { get; }
        string Image { get; }
        ImagePosition ImagePosition { get; }
        Orientation Orientation { get; }
        int Columns { get; }
        int Rows { get; }
        string Width { get; }
        string Height { get; }
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
        public bool IsEmpty;
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

}
