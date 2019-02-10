using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Collections;

namespace Steganografia
{
    class DataHide
    {
        public static Image hideInformation(Bitmap oldImage, string data, string password)
        {
            Bitmap encryptedImage = new Bitmap(oldImage);
            Byte[] encryptedInformation = encrypt(data, password);
            BitArray encryptedBits = new BitArray(encryptedInformation);
            Int32 informationSize = encryptedInformation.Length;

            int counter = 0;
            for (int i = 0; i < encryptedImage.Width; i++)
            {
                for (int j = 0; i < encryptedImage.Height; j++)
                {
                    byte[] rgb = new byte[3];
                    rgb[0] = oldImage.GetPixel(i, j).R;
                    rgb[1] = oldImage.GetPixel(i, j).G;
                    rgb[2] = oldImage.GetPixel(i, j).B;

                    for (int k = 0; k < 3; k++)
                    {
                        if (counter >= encryptedBits.Length + 32)
                            break;

                        bool a1 = (rgb[k] & (0x1 << 0)) == 1;
                        bool a2 = (rgb[k] & (0x1 << 1)) == 1;
                        bool a3 = (rgb[k] & (0x1 << 2)) == 1;

                        bool x1, x2;

                        //message size
                        if (counter < 32)
                        {
                            x1 = (informationSize & (0x1 << counter)) == 1;
                            x2 = (informationSize & (0x1 << (counter + 1))) == 1;
                        }
                        else
                        {
                            //data
                            x1 = encryptedBits[counter - 32];
                            //padding
                            x2 = (((counter - 32 + 1) == encryptedBits.Length) ? false : encryptedBits[counter - 32 + 1]);
                        }


                        if (x1 == (a1 ^ a3) && x2 == (a2 ^ a3))
                            ; //nothing
                        if (x1 != (a1 ^ a3) && x2 == (a2 ^ a3))
                        {
                            a1 = !a1;
                        }
                        if (x1 == (a1 ^ a3) && x2 != (a2 ^ a3))
                        {
                            a2 = !a2;
                        }
                        if (x1 != (a1 ^ a3) && x2 != (a2 ^ a3))
                        {
                            a3 = !a3;
                        }

                        byte result = rgb[0];
                        result &= (0 << 0); //reset (0) last bit
                        result |= (byte)((a1 == true ? 1 : 0) << 0);// set last bit to a1

                        result &= (0 << 1); //reset second (0) last bit
                        result |= (byte)((a2 == true ? 1 : 0) << 1);// set second last bit to a2

                        result &= (0 << 2); //reset third (2) last bit
                        result |= (byte)((a3 == true ? 1 : 0) << 2);// set third last bit to a3

                        rgb[k] = result;

                        counter += 2;
                    }

                    encryptedImage.SetPixel(i, j, Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
                }
            }

            return encryptedImage;
        }



        static Byte[] encrypt(string data, string password)
        {   
            SHA512 sha = SHA512.Create();
            Byte[] key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            AesManaged aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.IV = new Byte[16];
            Console.WriteLine(string.Join(" ", aes.IV));
            aes.Key = key;
            aes.Padding = PaddingMode.None;
            ICryptoTransform encryptPlatform = aes.CreateEncryptor();
            
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptPlatform, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                   return msEncrypt.ToArray();
                }
            }
        }

        static Byte[] decrypt(string data, string password)
        {
            SHA512 sha = SHA512.Create();
            Byte[] key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            AesManaged aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.IV = new Byte[16];
            Console.WriteLine(string.Join(" ", aes.IV));
            aes.Key = key;
            aes.Padding = PaddingMode.None;
            ICryptoTransform encryptPlatform = aes.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, encryptPlatform, CryptoStreamMode.Read))
                {
                    using (StreamWriter swDecrypt = new StreamWriter(csDecrypt))
                    {
                        swDecrypt.Write(data);
                    }
                    return msDecrypt.ToArray();
                }
            }
        }
    }

    }
}
