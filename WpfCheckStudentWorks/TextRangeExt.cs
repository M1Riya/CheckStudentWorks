using System;
using System.Windows.Documents;

namespace WpfCheckStudentWorks
{
    public static class TextRangeExt
    {
        public static TextRange FindText(this TextRange searchRange, string searchText)
        {
            TextRange result = null;
            int offset = searchRange.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (offset >= 0)
            {
                var start = searchRange.Start.GetTextPositionAtOffset(offset);
                result = new TextRange(start, start.GetTextPositionAtOffset(searchText.Length));
            }
            return result;
        }

        private static TextPointer GetTextPositionAtOffset(this TextPointer position, int offset)
        {
            for (TextPointer current = position; current != null; current = position.GetNextContextPosition(LogicalDirection.Forward))
            {
                position = current;
                var adjacent = position.GetAdjacentElement(LogicalDirection.Forward);
                var context = position.GetPointerContext(LogicalDirection.Forward);
                switch (context)
                {
                    case TextPointerContext.Text:
                        int count = position.GetTextRunLength(LogicalDirection.Forward);
                        if (offset <= count)
                        {
                            return position.GetPositionAtOffset(offset);
                        }
                        offset -= count;
                        break;
                    case TextPointerContext.ElementStart:
                        if (adjacent is InlineUIContainer)
                        {
                            offset--;
                        }
                        else if (adjacent is ListItem lsItem)
                        {
                            var trange = new TextRange(lsItem.ElementStart, lsItem.ElementEnd);
                            var index = trange.Text.IndexOf('\t');
                            if (index >= 0)
                            {
                                offset -= index + 1;
                            }
                        }
                        break;
                    case TextPointerContext.ElementEnd:
                        if (adjacent is Paragraph)
                            offset -= 2;
                        break;
                }
            }
            return position;
        }
    }
}
