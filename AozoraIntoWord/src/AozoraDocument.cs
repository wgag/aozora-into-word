using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace AozoraIntoWord
{
    public class AozoraDocument
    {
        public string FilePath { get; set; }
        public string TemplatePath { get; set; }

        public AozoraDocument(string path, string templatePath)
        {
            FilePath = path;
            TemplatePath = templatePath;
        }

        public void GenerateWordDocument()
        {
            using (XmlTextReader reader = new XmlTextReader(FilePath))
            {
                reader.DtdProcessing = System.Xml.DtdProcessing.Ignore;
                reader.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;
                AozoraWordProcessor proc = new AozoraWordProcessor(TemplatePath);
                proc.UseHorizInVert = true;

                Stack<XhtmlNode> nodeStack = new Stack<XhtmlNode>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ProcessElement(proc, reader, nodeStack);
                            break;

                        case XmlNodeType.EndElement:
                            ProcessEndElement(proc, reader, nodeStack);
                            break;

                        case XmlNodeType.Text:
                            ProcessText(proc, reader, nodeStack);
                            break;
                    }
                }
            }
        }

        private void ProcessElement(AozoraWordProcessor proc, XmlTextReader reader, Stack<XhtmlNode> nodeStack)
        {
            string _id = reader.GetAttribute("id");
            string _class = reader.GetAttribute("class");

            if (!reader.IsEmptyElement)
            {
                nodeStack.Push(new XhtmlNode(reader.Name, _id, _class));
            }

            switch (reader.Name)
            {
                case "h1":
                    if (_class == "title") { proc.SwitchDocumentPart(DocumentPart.Title); }
                    break;

                case "h2":
                    if (_class == "author") { proc.SwitchDocumentPart(DocumentPart.Author); }
                    break;

                case "div":
                    switch (_class)
                    {
                        case "main_text":
                            proc.SwitchDocumentPart(DocumentPart.MainText);
                            break;

                        case "bibliographical_information":
                            proc.SwitchDocumentPart(DocumentPart.BibInfo);
                            break;

                        default:
                            if (_class != null && _class.StartsWith("jisage_")) { proc.Indent = int.Parse(Regex.Replace(_class, @"jisage_(\d+)", "$1")); }
                            break;
                    }
                    break;

                case "strong":
                    proc.Emphasize = true;
                    break;

                case "br":
                    proc.WriteText("\r\n");
                    break;

                case "ruby":
                    proc.ResetRuby();
                    break;

                case "img":
                    if (_class == "gaiji")
                    {
                        string text = ConvertGaiji(reader.GetAttribute("alt"));
                        ProcessText(proc, text, nodeStack);
                    }
                    else
                    {
                        string text = reader.GetAttribute("alt");
                        ProcessText(proc, text, nodeStack);
                    }
                    break;

                default:
                    break;
            }
        }

        private void ProcessEndElement(AozoraWordProcessor proc, XmlTextReader reader, Stack<XhtmlNode> nodeStack)
        {
            XhtmlNode node = nodeStack.Pop();
            switch (node.Name)
            {
                case "h1":
                    if (node.Class == "title") { proc.SwitchDocumentPart(DocumentPart.None); }
                    break;

                case "h2":
                    if (node.Class == "author") { proc.SwitchDocumentPart(DocumentPart.None); }
                    break;

                case "div":
                    switch (node.Class)
                    {
                        case "main_text":
                            proc.SwitchDocumentPart(DocumentPart.None);
                            break;

                        case "bibliographical_information":
                            proc.SwitchDocumentPart(DocumentPart.None);
                            break;

                        default:
                            if (node.Class != null && node.Class.StartsWith("jisage_")) { proc.Indent = 0; }
                            break;
                    }
                    break;

                case "strong":
                    proc.Emphasize = false;
                    break;

                case "ruby":
                    proc.WriteRubiedText();
                    break;

                default:
                    break;
            }
        }

        private void ProcessText(AozoraWordProcessor proc, XmlTextReader reader, Stack<XhtmlNode> nodeStack)
        {
            ProcessText(proc, reader.Value, nodeStack);
        }

        private void ProcessText(AozoraWordProcessor proc, string text, Stack<XhtmlNode> nodeStack)
        {
            text = ReplaceSymbols(text);
            text = Regex.Replace(text, @"[\r\n]+", "");

            switch (nodeStack.Peek().Name)
            {
                case "rb":
                    proc.RubyRb += text;
                    break;

                case "rt":
                    proc.RubyRt += text;
                    break;

                case "rp":
                    break;

                default:
                    proc.WriteText(text);
                    break;
            }
        }

        /// <summary>
        /// Replaces embedded symbols such as "／＼".
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string ReplaceSymbols(string text)
        {
            text = text.Replace("／＼", "\u3033\u3035");
            text = text.Replace("／″＼", "\u3034\u3035");
            return text;
        }

        /// <summary>
        /// Converts special character descriptioned in img-alt attribute.
        /// </summary>
        /// <param name="alttext"></param>
        /// <returns></returns>
        private string ConvertGaiji(string alttext)
        {
            Regex regex = new Regex(@"※.*(?<p>\d+)-(?<r>\d+)-(?<c>\d+)");
            if (regex.IsMatch(alttext))
            {
                int p = int.Parse(regex.Match(alttext).Groups["p"].Value);
                int r = int.Parse(regex.Match(alttext).Groups["r"].Value);
                int c = int.Parse(regex.Match(alttext).Groups["c"].Value);
                return EncodingEx.KutenToString(p, r, c);
            }
            else
            {
                return alttext;
            }
        }
    }
}
