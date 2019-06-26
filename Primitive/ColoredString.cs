using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls.Primitive
{
    public class ColoredString
    {
        public ColoredString(string text)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder exsb = new StringBuilder();
            System.Drawing.Color defaultColor = System.Drawing.Color.Gray;
            System.Drawing.Color color = defaultColor;

            for (int i = 0; i < text.Length; i++)
            {
                if(text[i] == '\u001b')
                {
                    i++;
                    exsb.Clear();
                    if (i >= text.Length) break;
                    if (text[i] != '[') continue;
                    while (i < text.Length)
                    {
                        if (text[i] == 'm')
                        {
                            exsb.Append(text[i]);
                            switch (exsb.ToString())
                            {
                                case "[m":
                                    color = defaultColor;
                                    break;
                                case "[30m":
                                case "[1;30m":
                                    color = System.Drawing.Color.Black;
                                    break;
                                case "[31m":
                                case "[1;31m":
                                    color = System.Drawing.Color.Red;
                                    break;
                                case "[32m":
                                case "[1;32m":
                                    color = System.Drawing.Color.Green;
                                    break;
                                case "[33m":
                                case "[1;33m":
                                    color = System.Drawing.Color.Yellow;
                                    break;
                                case "[34m":
                                case "[1;34m":
                                    color = System.Drawing.Color.Blue;
                                    break;
                                case "[35m":
                                case "[1;35m":
                                    color = System.Drawing.Color.Magenta;
                                    break;
                                case "[36m":
                                case "[1;36m":
                                    color = System.Drawing.Color.Cyan;
                                    break;
                                case "[37m":
                                case "[1;37m":
                                    color = System.Drawing.Color.White;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                        else
                        {
                            exsb.Append(text[i]);
                            i++;
                        }
                    }
                }else
                // "\u001b[32m|\u001b[m\u001b[33m\\\u001b[m  "
                if (i < text.Length)
                {
                    sb.Append(text[i]);
                    Colors.Add(color);
                }
            }
            Text = sb.ToString();
        }

        public string Text = "";
        public List<System.Drawing.Color> Colors = new List<System.Drawing.Color>();

    }
}
