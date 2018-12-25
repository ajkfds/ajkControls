using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
    public class Icon
    {
        public Icon(System.Drawing.Image image)
        {
            originalImage = image;
        }

        private System.Drawing.Image originalImage;

        public Image GetImage(int size,ColorStyle color)
        {
            int index = size << 4 + (int)color;
            if(!images.ContainsKey(index))
            {
                if (!coloredImages.ContainsKey(color))
                {
                    coloredImages.Add(color, CreateColorImage(color, originalImage));
                }
                images.Add(index, resizeImage(size,coloredImages[color]));
            }
            return images[index];
        }

        public System.Drawing.Icon GetSystemDrawingIcon(int size, ColorStyle color)
        {
            Image image = GetImage(size, color);
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
            White = 5
        }

        private Image CreateColorImage(ColorStyle style,Image originalImage)
        {
            switch (style)
            {
                case ColorStyle.Blue: return shiftImageColor(originalImage, 2f, 2.5f, 5f);
                case ColorStyle.Red: return shiftImageColor(originalImage, 5f, 2f, 2f);
                case ColorStyle.Green: return shiftImageColor(originalImage, 5f, 2f, 2f);
                case ColorStyle.White: return shiftImageColor(originalImage, 3f, 3f, 3f);
                default:    return originalImage;
            }
        }


        private Image resizeImage(int size,Image originalImage)
        {
            Bitmap resizeBmp = new Bitmap(size, size);
            Graphics g = Graphics.FromImage(resizeBmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.DrawImage(originalImage, 0, 0, size, size);
            g.Dispose();

            return resizeBmp;
        }


        // value
        // value * ratio + shift

        // 0 * ratio + shift
        // 0.5 * ratio + shift
        // 1 * ratio + shift

        private Image shiftImageColor(Image sourceImage,float rShift,float gShift,float bShift)
        {
            // keep black & white
            // shift intermediate colors

/*            float[][] matrixElement =
            {
                new float[]{rShift, 0,      0,      0,      0},
                new float[]{0,      gShift, 0,      0,      0},
                new float[]{0,      0,      bShift, 0,      0},
                new float[]{0,      0,      0,      1,      0},
                new float[]{0,      0,      0,      0,      1} // shift
            };
            */
            float[][] matrixElement =
            {
                new float[]{rShift, 0,      0,      0,      0},
                new float[]{0,      gShift, 0,      0,      0},
                new float[]{0,      0,      bShift, 0,      0},
                new float[]{0,      0,      0,      1,      0},
                new float[]{rShift/10, gShift/10, bShift/10, 0,      1} // shift
            };

            var matrix = new System.Drawing.Imaging.ColorMatrix(matrixElement);

            var attr = new System.Drawing.Imaging.ImageAttributes();
            attr.SetColorMatrix(matrix);

            int imageWidth = sourceImage.Width;
            int imageHeight = sourceImage.Height;

            Bitmap changedImage = new Bitmap(imageWidth, imageHeight);

            Graphics g = Graphics.FromImage(changedImage);

            g.DrawImage(sourceImage,
                    new Rectangle(0, 0, imageWidth, imageHeight),
                    0, 0, imageWidth, imageHeight,
                    GraphicsUnit.Pixel,
                    attr);

            g.Dispose();

            return changedImage;
        }

    }
}
