using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace SqlReplacer
{
    class Program
    {
        private class Rule
        {
            [DebuggerStepThrough]
            public Rule(string from, string to)
            {
                this.From = from;
                this.To = to;
            }
        
            public String From { get; private set; }
            public String To { get; private set; }

            
        }

        static void Main(string[] args)
        {
            String ruleFilePath = null;
            String find = null;
            String replaceTo = null;
            String file = null;

            OptionSet options = new OptionSet();
            options.Add("r=", "path to xml file containing set of rules for replacement", value => ruleFilePath = value);
            options.Add("f=", "what needs to be replaced", value => find = value);
            options.Add("t=", "replacement value", value => replaceTo = value);
            options.Add("u=", "file for update", value => file = value);
            options.Parse(args);

            List<Rule> rules = new List<Rule>();
            if (file == null)
            {
               rules.Add(new Rule(find, replaceTo));
            }
            else
            {
                var xml = new XmlDocument();
                xml.Load(ruleFilePath);
                var nodes = xml.SelectNodes("//rule");
                foreach (XmlNode node in nodes)
                {
                    rules.Add(new Rule(node.Attributes["from"].Value, node.Attributes["to"].Value));
                }
            }

            string fileContent;
            using (var sr = new StreamReader(file))
            {
                fileContent = sr.ReadToEnd();
            }
            foreach (var rule in rules)
            {
                fileContent = Regex.Replace(fileContent, rule.From, rule.To, RegexOptions.IgnoreCase);
            }
            using (var sw = new StreamWriter(file))
            {
                sw.Write(fileContent);
            }
        }
    }
}
