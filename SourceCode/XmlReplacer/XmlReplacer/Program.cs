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
                if (args[3] == "GenerateRandom")
                {
                    Random random = new Random();
                    node.Attributes[args[2]].Value = string.Format("{0}.{1}.{2}.{3}",
                                                                   random.Next(1, 20).ToString(),
                                                                   random.Next(0, 20).ToString(),
                                                                   random.Next(0, 100).ToString(),
                                                                   random.Next(0, 100).ToString());
                }
                else
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
