using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganografia.Modle
{
    class ImageHolder
    {
        private Image image;

        public ImageHolder(string path)
        {
            setImage(path);
        }

        void setImage(string path)
        {
            image = Image.FromFile(path);
        }
        Image getImage()
        {

        }
    }
}
