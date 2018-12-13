using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class TabPage : System.Windows.Forms.TabPage
    {
        public virtual Icon Icon
        {
            get;
            set;
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
                if (closeButtonEnable)
                {
                    ImageIndex = 0;
                }
                else
                {
                    ImageIndex = -1;
                }
            }
        }

        public virtual void CloseButtonClicked()
        {

        }


    }
}
