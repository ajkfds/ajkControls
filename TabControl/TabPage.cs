using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class TabPage : System.Windows.Forms.TabPage
    {
        private IconImage iconImage;
        public virtual IconImage IconImage
        {
            get
            {
                return iconImage;
            }
            set
            {
                iconImage = value;
                resizeTab();
            }
        }

        private IconImage.ColorStyle iconColor = IconImage.ColorStyle.White;
        public virtual IconImage.ColorStyle IconColor
        {
            get
            {
                return iconColor;
            }
            set
            {
                iconColor = value;
            }
        }

        private bool closeButtonEnable = false;
        public virtual bool CloseButtonEnable
        {
            get
            {
                return closeButtonEnable;
            }
            set
            {
                closeButtonEnable = value;
                resizeTab();
            }
        }

        private void resizeTab()
        {
            if (closeButtonEnable || IconImage != null)
            {
                ImageIndex = 0;
            }
            else
            {
                ImageIndex = -1;
            }
        }


        public virtual void CloseButtonClicked()
        {

        }


    }
}
