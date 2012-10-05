using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;

namespace ICPSystemAlert
{
    public static class PDFFontStyle
    {
        public static Font STYLE_0 = FontFactory.GetFont("Verdana", 18F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_11 = FontFactory.GetFont("Verdana", 20F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_1 = FontFactory.GetFont("Verdana", 12F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_8 = FontFactory.GetFont("Verdana", 16F, Font.NORMAL, BaseColor.BLACK);
        public static Font STYLE_9 = FontFactory.GetFont("Verdana", 16F, Font.BOLD, BaseColor.WHITE);
        public static Font STYLE_10 = FontFactory.GetFont("Verdana", 16F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_4 = FontFactory.GetFont("Verdana", 10F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_7 = FontFactory.GetFont("Verdana", 10F, Font.BOLD, BaseColor.WHITE);
        public static Font STYLE_6 = FontFactory.GetFont("Verdana", 10F, Font.NORMAL, BaseColor.BLACK);
        public static Font STYLE_5 = FontFactory.GetFont("Verdana", 8F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_2 = FontFactory.GetFont("Verdana", 7F, Font.BOLD, BaseColor.BLACK);
        public static Font STYLE_3 = FontFactory.GetFont("Verdana", 7F, Font.NORMAL, BaseColor.BLACK);
    }

    public static class PDFBorderType
    {
        public static int NONE = 0;
        public static int TOP = 1;
        public static int BOTTOM = 2;
        public static int TOP_BOTTOM = 3;
        public static int LEFT = 4;
        public static int LEFT_TOP = 5;
        public static int LEFT_BOTTOM = 6;
        public static int LEFT_TOP_BOTTOM = 7;
        public static int RIGHT = 8;
        public static int RIGHT_TOP = 9;
        public static int RIGHT_BOTTOM = 10;
        public static int RIGHT_TOP_BOTTOM = 11;
        public static int LEFT_RIGHT = 12;
        public static int LEFT_RIGHT_TOP = 13;
        public static int LEFT_RIGHT_BOTTOM = 14;
        public static int LEFT_RIGHT_TOP_BOTTOM = 15;
    }

}
