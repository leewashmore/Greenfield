using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XmlReplacer
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(args[0]);

            var node = doc.SelectSingleNode(args[1]);
            if (node.Attributes[args[2]] != null)
            {
                node.Attributes[args[2]].Value = args[3];
            }
            else
            {
                if (args[2] == "innerText")
                {
                    node.InnerText = args[3];
                }
            }

            doc.Save(args[0]);
        }
    }
}
