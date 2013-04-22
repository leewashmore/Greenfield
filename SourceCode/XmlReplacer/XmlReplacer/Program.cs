using System;
using System.Globalization;
using System.Xml;

namespace XmlReplacer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var doc = new XmlDocument();
            doc.Load(args[0]);

            XmlNode node = doc.SelectSingleNode(args[1]);
            if (node.Attributes[args[2]] != null)
            {
                if (args[3] == "GenerateRandom")
                {
                    var random = new Random();
                    node.Attributes[args[2]].Value = string.Format("{0}.{1}.{2}.{3}",
                                                                   random.Next(1, 20).ToString(CultureInfo.InvariantCulture),
                                                                   random.Next(0, 20).ToString(CultureInfo.InvariantCulture),
                                                                   random.Next(0, 100).ToString(CultureInfo.InvariantCulture),
                                                                   random.Next(0, 100).ToString(CultureInfo.InvariantCulture));
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