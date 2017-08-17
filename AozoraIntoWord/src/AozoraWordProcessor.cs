using System;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;

namespace AozoraIntoWord
{
    internal class AozoraWordProcessor : IDisposable
    {
        Word._Document document;
        Word.Range range;

        DocumentPart documentPart;

        public string RubyRb { get; set; }
        public string RubyRt { get; set; }

        // bookmarks in word document
        readonly object bmEndOfDoc = @"\endofdoc"; // predefined bookmark
        readonly object bmTitle = "title";
        readonly object bmAuthor = "author";
        readonly object bmMainText = "main_text";
        readonly object bmBibInfo = "bibliographical_information";

        public void SwitchDocumentPart(DocumentPart part)
        {
            documentPart = part;

            object bkmk = null;
            switch (part)
            {
                case DocumentPart.Author: bkmk = bmAuthor; break;
                case DocumentPart.Title: bkmk = bmTitle; break;
                case DocumentPart.MainText: bkmk = bmMainText; break;
                case DocumentPart.BibInfo: bkmk = bmBibInfo; break;
            }
            if (bkmk != null)
            {
                range = document.Bookmarks.get_Item(ref bkmk).Range;
            }
        }

        public bool Emphasize { get; set; }
        public int Indent { get; set; }
        public bool UseHorizInVert { get; set; }


        public string TemplatePath { get; set; }

        public AozoraWordProcessor(string templatePath)
        {
            TemplatePath = templatePath;
            object missing = System.Reflection.Missing.Value;
            object template = TemplatePath;
            Word._Application word = new Word.Application();
            word.Visible = true;
            document = word.Documents.Add(ref template, ref missing, ref missing, ref missing);
        }

        public void Dispose()
        {
            if (document != null) { document.Close(); }
        }

        public void ResetRuby()
        {
            RubyRb = "";
            RubyRt = "";
        }

        public void WriteRubiedText()
        {
            WriteTextSub(RubyRb, RubyRt);
        }

        public void WriteText(string text)
        {
            WriteText(text, null);
        }

        public void WriteText(string text, string ruby)
        {
            if (documentPart == DocumentPart.None) { return; }

            if (UseHorizInVert)
            {
                foreach (string splitted in Regex.Split(text, @"(\d+)"))
                {
                    WriteTextSub(splitted, ruby);
                }
            }
            else
            {
                WriteTextSub(text, ruby);
            }
        }

        private void WriteTextSub(string text, string ruby)
        {
            range.Text = text;

            if (Emphasize) { range.EmphasisMark = Word.WdEmphasisMark.wdEmphasisMarkOverComma; }
            else { range.EmphasisMark = Word.WdEmphasisMark.wdEmphasisMarkNone; }

            // ルビ
            if (!string.IsNullOrEmpty(ruby)) { range.PhoneticGuide(ruby); }

            // 縦中横
            if (UseHorizInVert && Regex.IsMatch(text, @"^\d{1,2}$")) { range.HorizontalInVertical = Word.WdHorizontalInVerticalType.wdHorizontalInVerticalFitInLine; }
            else { range.HorizontalInVertical = Word.WdHorizontalInVerticalType.wdHorizontalInVerticalNone; }

            if (!string.IsNullOrEmpty(text))
            {
                range.Move();
            }

            range.ParagraphFormat.CharacterUnitLeftIndent = Indent;
            if (documentPart == DocumentPart.MainText && Indent == 0) { range.ParagraphFormat.LeftIndent = 0; }
        }
    }
}

