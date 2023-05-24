using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls.Primitive
{
    public class IconImage
    {
        public IconImage(System.Drawing.Image image)
        {
            originalImage = image;
        }

        private System.Drawing.Image originalImage;

        public Image GetImage(int height,ColorStyle color)
        {
            int index = height << 4 + (int)color;
            if(!images.ContainsKey(index))
            {
                if (!coloredImages.ContainsKey(color))
                {
                    coloredImages.Add(color, CreateColorImage(color, originalImage));
                }
                images.Add(index, resizeImage(height,coloredImages[color]));
            }
            return images[index];
        }

        public int GetImageWidth(int height)
        {
            return getImageWidth(height, originalImage);
        }

        public System.Drawing.Icon GetSystemDrawingIcon(int height, ColorStyle color)
        {
            Image image = GetImage(height, color);
            return System.Drawing.Icon.FromHandle(((System.Drawing.Bitmap)image).GetHicon());
        }

        Dictionary<ColorStyle, Image> coloredImages = new Dictionary<ColorStyle, Image>();
        Dictionary<int, Image> images = new Dictionary<int, Image>();

        public enum ColorStyle : int
        {
            Original = 0,
            Gray = 1,
            Red = 2,
            Blue = 3,
            Green = 4,
            White = 5,
            Orange = 6,
        }

        private Image CreateColorImage(ColorStyle style,Image originalImage)
        {
            switch (style)
            {
                case ColorStyle.Gray:   return shiftImageColor(originalImage, Color.DarkGray,  Color.LightGray);
                case ColorStyle.Red:    return shiftImageColor(originalImage, Color.DarkRed,   Color.LightPink);
                case ColorStyle.Blue:   return shiftImageColor(originalImage, Color.DarkBlue, Color.FromArgb(220, 240, 255));
                case ColorStyle.Green:  return shiftImageColor(originalImage, Color.DarkGreen, Color.FromArgb(180, 255, 180) );
                case ColorStyle.White:  return shiftImageColor(originalImage, Color.FromArgb(16,16,16) , Color.White);
                case ColorStyle.Orange: return shiftImageColor(originalImage, Color.DarkOrange, Color.LemonChiffon);

                default:    return originalImage;
            }
        }

        private int getImageWidth(int height, Image originalImage)
        {
            return originalImage.Width * height / originalImage.Height;
        }

        private Image resizeImage(int height,Image originalImage)
        {
            int width = getImageWidth(height, originalImage);
            Bitmap resizeBmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(resizeBmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, width, height);
            g.Dispose();

            return resizeBmp;
        }

        // --- original image ---
        // 0 ->     0.25 -> 1
        // ------ after ---------
        // line ->  fill -> white

        // after = original * ratio + shift

        // line = 0 * ratio + shift
        // fill = 0.25 * ratio + shift
        // 1 <= 1*ratio + fill

        // shift = line
        // ratio = (fill-line)*4

       
        private Image shiftImageColor(Image sourceImage, Color lineColor,Color fillColor)
        {
            float rShift = (float)lineColor.R / 256;
            float gShift = (float)lineColor.G / 256;
            float bShift = (float)lineColor.B / 256;

            float rRatio = (float)(fillColor.R - lineColor.R) / 256*4;
            float gRatio = (float)(fillColor.G - lineColor.G) / 256*4;
            float bRatio = (float)(fillColor.B - lineColor.B) / 256*4;

            float[][] matrixElement =
            {
                new float[]{rRatio, 0,      0,      0,      0},
                new float[]{0,      gRatio, 0,      0,      0},
                new float[]{0,      0,      bRatio, 0,      0},
                new float[]{0,      0,      0,      1,      0},
                new float[]{rShift, gShift, bShift, 0,      1} // shift
            };

            var matrix = new System.Drawing.Imaging.ColorMatrix(matrixElement);

            var attr = new System.Drawing.Imaging.ImageAttributes();
            attr.SetColorMatrix(matrix);

            int imageWidth = sourceImage.Width;
            int imageHeight = sourceImage.Height;

            Bitmap changedImage = new Bitmap(imageWidth, imageHeight);

            Graphics gr = Graphics.FromImage(changedImage);

            gr.DrawImage(sourceImage,
                    new Rectangle(0, 0, imageWidth, imageHeight),
                    0, 0, imageWidth, imageHeight,
                    GraphicsUnit.Pixel,
                    attr);

            gr.Dispose();

            return changedImage;
        }

    }
}
