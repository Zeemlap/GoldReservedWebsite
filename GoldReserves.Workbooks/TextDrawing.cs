using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoldReserves.Workbooks
{
    public class TextDrawing : Drawing
    {
        internal List<Paragraph> ParagraphsInternal;

        internal TextDrawing()
        {
            ParagraphsInternal = new List<Paragraph>();
            Paragraphs = new ReadOnlyCollection<Paragraph>(ParagraphsInternal);
        }

        public override DrawingType DrawingType
        {
            get
            {
                return DrawingType.TextBox;
            }
        }

        public IReadOnlyList<Paragraph> Paragraphs
        {
            get;
        }
    }
}
