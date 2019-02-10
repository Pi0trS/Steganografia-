using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Steganografia
{
    class DataHide
    {
        static Image hideInformation(Image oldImage, string data, string password)
        {
            Image encryptedImagae = new Bitmap(oldImage);
            SHA512 sha = SHA512.Create();
            Byte[] key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            AesManaged aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.IV = new Byte[16];
            Console.WriteLine(string.Join(" ", aes.IV));
            aes.Key = key;
            aes.Padding = PaddingMode.None;
            ICryptoTransform encryptPlatform = aes.CreateEncryptor();
            Byte[] encryptedData; 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptPlatform, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                    encryptedData = msEncrypt.ToArray();
                }
            }

            return encryptedImagae;
        }
    }
}
