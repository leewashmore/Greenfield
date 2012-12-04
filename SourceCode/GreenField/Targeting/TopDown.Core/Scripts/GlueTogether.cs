using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TopDown.Core.Scripts
{
    public class GlueTogether
    {

        public void Preset()
        {
            var folderpath = @"..\..\Scripts\PreloadedData";
            var files = Directory.EnumerateFiles(folderpath, "*.sql").OrderBy(x => x);
            foreach (var file in files)
            {
                Trace.WriteLine(File.ReadAllText(file));
            }
        }


        public void Glue()
        {
            var folderpath = @"..\..\Scripts";
            var files = Directory.EnumerateFiles(folderpath, "*.sql").OrderBy(x => x);
            foreach (var file in files)
            {
                Trace.WriteLine(File.ReadAllText(file));
            }
        }


        public void Generate()
        {
            var filepath = @"..\..\Scripts\042-BASKET_PORTFOLIO_SECURITY_TARGET.sql";
            var lines = File.ReadAllLines(filepath);
            var index = 0;
            foreach (var line in lines)
            {
                var replaced = line.Replace("###", index.ToString("000"));
                if (replaced != line)
                {
                    index ++;
                }
                Trace.WriteLine(replaced);
            }
        }
    }
}
