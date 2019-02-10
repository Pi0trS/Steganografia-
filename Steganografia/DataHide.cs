using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Steganografia
{
    class DataHide
    {
        static Image hideInformation(Image oldImage, Byte[] data, string password)
        {
            Image encryptedImagae =new Bitmap(oldImage);
            SHA512 sha = SHA512.Create();
            Byte[] key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            return encryptedImagae;
        }
    }
}
