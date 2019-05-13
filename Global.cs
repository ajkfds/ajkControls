using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
    public static class Global
    {

        private static float dpi = -1;
        public static float Dpi
        {
            get
            {
                if (dpi == -1)
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(4, 4);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                    dpi = g.DpiX;
                }
                return dpi;
            }
        }

        private const float mainMenuIconSizeFor144Dpi = 24;
        private static int mainMenuIconSize = -1;
        public static int MainMenuIconSize
        {
            get
            {
                if (mainMenuIconSize == -1)
                {
                    mainMenuIconSize = (int)(Dpi / 144 * mainMenuIconSizeFor144Dpi);
                }
                return mainMenuIconSize;
            }
        }

        private const float toolStripIconSizeFor144Dpi = 16;
        private static int toolStripIconSize = -1;
        public static int ToolStripIconSize
        {
            get
            {
                if (toolStripIconSize == -1)
                {
                    toolStripIconSize = (int)(Dpi / 144 * toolStripIconSizeFor144Dpi);
                }
                return toolStripIconSize;
            }
        }

        private const float scrollBarWidthFor144Dpi = 20;
        private static int scrollBarWidth = -1;
        public static int ScrollBarWidth
        {
            get
            {
                if(scrollBarWidth == -1)
                {
                    scrollBarWidth = (int)(Dpi/144*scrollBarWidthFor144Dpi);
                }
                return scrollBarWidth;
            }
        }

        private const float defaultFontSizeFor144Dpi = 32;
        private static System.Drawing.Font uiDefaultFont = null;
        public static System.Drawing.Font UiDefaultFont
        {
            get
            {
                if(uiDefaultFont == null)
                {
                    uiDefaultFont = new System.Drawing.Font("Meiryo UI", Dpi/144* defaultFontSizeFor144Dpi, System.Drawing.GraphicsUnit.Pixel);
                }
                return uiDefaultFont;
            }
        }
        private static System.Drawing.Font uiFixedSizeFont = null;
        public static System.Drawing.Font UiFixedSizeFont
        {
            get
            {
                if (uiFixedSizeFont == null)
                {
                    uiFixedSizeFont = new System.Drawing.Font("Consolas", Dpi / 144 * defaultFontSizeFor144Dpi, System.Drawing.GraphicsUnit.Pixel);
                }
                return uiFixedSizeFont;
            }
        }

        public static float WheelSensitivity
        {
            get
            {
                return (float)0.01;
            }
        }

        private static Icon icon = null;
        public static Icon Icon
        {
            get
            {
                if(icon == null)
                {
                    icon = IconImages.Question.GetSystemDrawingIcon(48, IconImage.ColorStyle.White);
                }
                return icon;
            }
            set
            {
                icon = value;
            }
        }

        public static class ColorMap
        {
            public static Color ControlBackGround = Color.FromArgb(32, 56, 100);
        }

        public static class IconImages
        {
            public static IconImage Question = new IconImage(Properties.Resources.question);
        }


    }
}
