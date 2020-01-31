using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Protocols
{
    public sealed class DataType
    {
        public DataTypes Type { get; } = DataTypes.UNKNOWN;

        public string Name { get; } = "UNKNOWN";

        public bool IsDirty = true;

        public DataType(DataTypes type, string name)
        {
            Type = type;
            Name = name;
        }

        private (XElement type, XElement name) xml;
        public (XElement type, XElement name) XML
        {
            get
            {
                if (!IsDirty)
                    return xml;

                xml.name = new XElement("Name", Name);
                xml.type = new XElement("Type", Type.ToString());

                return xml;
            }
        }
    }
}
