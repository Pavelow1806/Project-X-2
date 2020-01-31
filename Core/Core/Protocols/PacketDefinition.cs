using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Protocols
{
    public sealed class PacketDefinition
    {
        public int Number { get; } = -1;
        public string Name { get; } = "";

        public List<DataType> Types = new List<DataType>();

        private bool isDirty = true;
        public bool IsDirty { get { return (isDirty || Types.Any(x => x.IsDirty)); } }

        public PacketDefinition(int number, string name)
        {
            Number = number;
            Name = name;
        }

        private XElement xml;
        public XElement XML
        {
            get
            {
                if (!IsDirty)
                    return xml;

                XElement types = new XElement("Data-Types");
                foreach (DataType dt in Types)
                {
                    types.Add(dt.XML.name);
                    types.Add(dt.XML.type);
                }
                xml = new XElement("Definition", 
                        new XElement("Name", Name),
                        new XElement("Number", Number.ToString()),
                        types
                    );

                return xml;
            }
        }
    }
}
