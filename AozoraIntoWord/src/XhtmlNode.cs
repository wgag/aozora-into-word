namespace AozoraIntoWord
{
    internal class XhtmlNode
    {
        public XhtmlNode(string name) : this(name, null, null) { }

        public XhtmlNode(string name, string _id, string _class)
        {
            Name = name;
            Id = _id;
            Class = _class;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string Class { get; set; }
    }
}
