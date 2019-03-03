using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;

namespace Steganografia
{
    class DataHide
    {
        public static ArrayList ImageToBits(Image oldImage)
        {
            Bitmap newBitmap = new Bitmap(oldImage);
            ArrayList imageInColors = new ArrayList();
            Color tmpColor;
            for (int i = 0; i < oldImage.Width; i++)
            {
                for (int j = 0; j < oldImage.Height; j++)
                {
                    tmpColor = newBitmap.GetPixel(i, j);
                    imageInColors.Add(Convert.ToString(tmpColor.R, 2));
                    imageInColors.Add(Convert.ToString(tmpColor.G, 2));
                    imageInColors.Add(Convert.ToString(tmpColor.B, 2));
                }
            }
            return imageInColors;
        }
        public static Bitmap BitsToBmp(Image image, ArrayList bitsRGBA)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height, format: PixelFormat.Format24bppRgb);
            int pixR, pixG, pixB, pixPos = 0;
            for(int i = 0; i< image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    pixR = Convert.ToInt32(bitsRGBA[pixPos + 0].ToString(), 2);
                    pixG = Convert.ToInt32(bitsRGBA[pixPos + 1].ToString(), 2);
                    pixB = Convert.ToInt32(bitsRGBA[pixPos + 2].ToString(), 2);

                    bmp.SetPixel(i, j, Color.FromArgb(pixR, pixG, pixB));
                    pixPos += 3;
                }
            }
            return bmp;
        }

        public static ArrayList InsretTextLenght(ArrayList imageInColors, String textToInsert)
        {
            ArrayList newImageInColors = new ArrayList();

            for (int i = 0; i < 12; i++)
            {
                newImageInColors.Add(imageInColors[i].ToString().Remove(imageInColors[i].ToString().Length - 1, 1) + textToInsert[i]);
            }
            return newImageInColors;
        }

        public static String OutputTextLenght(ArrayList imageInColors)
        {
            String lenght = "", tmpLenght = "";
            for (int i = 0; i < 12; i++)
            {
                tmpLenght = imageInColors.ToString();
                lenght += tmpLenght[tmpLenght.Length - 1];    
            }
            return lenght;
        }

        public static String lenghtInBits(String s)
        {
            String lenght = Convert.ToString(s.Count(), 2);
            while(lenght.Count() < 12)
            {
                lenght += "0"; 
            }
            return lenght;
        }

        public static int lenghtInInt(String s)
        {

        }

        public static Image hideInformationNoWork(Bitmap oldImage, string data, string password)
        {
            Bitmap encryptedImage = new Bitmap(oldImage);
            Byte[] encryptedInformation = encrypt(data, password);
            BitArray encryptedBits = new BitArray(encryptedInformation);
            Int32 informationSize = encryptedInformation.Length;

            int counter = 0;
            for (int i = 0; i < encryptedImage.Width; i++)
            {
                for (int j = 0; j < encryptedImage.Height; j++)
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

        public static Image hideInformation(Bitmap oldImage, string data, string password)
        {
            Bitmap encryptedImage = new Bitmap(oldImage);
            Byte[] encryptedInformation = Encoding.ASCII.GetBytes(data); //encrypt(data, password);
            BitArray encryptedBits = new BitArray(encryptedInformation);
            Int32 informationSize = encryptedBits.Length;

            int counter = 0;
            for (int i = 0; i < encryptedImage.Width; i++)
            {
                for (int j = 0; j < encryptedImage.Height; j++)
                {
                    byte[] rgb = new byte[3];
                    rgb[0] = oldImage.GetPixel(i, j).R;
                    rgb[1] = oldImage.GetPixel(i, j).G;
                    rgb[2] = oldImage.GetPixel(i, j).B;

                    for (int k = 0; k < 3; k++)
                    {
                        if (counter >= encryptedBits.Length + 32)
                            break;

                        bool x1;

                        //message size
                        if (counter < 32)
                        {
                            x1 = (informationSize & (0x1 << (31 - counter))) != 0;
                        }
                        else
                        {
                            //data
                            x1 = encryptedBits[counter - 32];
                        }

                        byte result = rgb[k];
                        if (x1) // set LSB to 1
                            result = (byte)(result | 1);
                        else // 0
                            result = (byte)(result & ~1);

                        rgb[k] = result;

                        counter++;
                    }

                    encryptedImage.SetPixel(i, j, Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
                }
            }

            return encryptedImage;
        }

        public static string showInformation(Bitmap encryptedImage, string password)
        {
            Int32 informationSize = 0;
            List<bool> encryptedBits = new List<bool>();

            int counter = 0;
            for (int i = 0; (i < encryptedImage.Width) && ((counter < 32) || (counter < informationSize + 32)); i++)
            {
                for (int j = 0; (j < encryptedImage.Height) && ((counter < 32) || (counter < informationSize + 32)); j++)
                {
                    byte[] rgb = new byte[3];
                    rgb[0] = encryptedImage.GetPixel(i, j).R;
                    rgb[1] = encryptedImage.GetPixel(i, j).G;
                    rgb[2] = encryptedImage.GetPixel(i, j).B;

                    for (int k = 0; (k < 3) && ((counter < 32) || (counter < informationSize + 32)); k++)
                    {
                        bool x1 = ((rgb[k] & 0x1) != 0);

                        // read message size
                        if (counter < 32)
                        {
                            informationSize <<= 1;
                            informationSize |= ((x1 == true) ? 1 : 0);
                        }
                        else
                        {
                            //data
                            encryptedBits.Add(x1);
                        }

                        counter++;
                    }
                }
            }

            if (encryptedBits.Count != informationSize) throw new Exception("oi");
            BitArray messageBits = new BitArray(encryptedBits.ToArray());
            Byte[] messageBytes = BitArrayToByteArray(messageBits);
            string message = Encoding.ASCII.GetString(messageBytes);

            return message;// Encoding.ASCII.GetString(decrypt(message, password));
        }


        static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        static Byte[] encrypt(string data, string password)
        {
            SHA512 sha = SHA512.Create();
            Byte[] key = sha.ComputeHash(Encoding.ASCII.GetBytes(password)).Take(32).ToArray();
            AesManaged aes = new AesManaged();
            //aes.Mode = CipherMode.CBC;
            aes.IV = new Byte[16];
            Console.WriteLine(string.Join(" ", aes.IV));
            aes.Key = key;
            aes.Padding = PaddingMode.PKCS7;
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
            Byte[] key = sha.ComputeHash(Encoding.ASCII.GetBytes(password)).Take(32).ToArray();
            AesManaged aes = new AesManaged();
            //aes.Mode = CipherMode.CBC;
            aes.IV = new Byte[16];
            Console.WriteLine(string.Join(" ", aes.IV));
            aes.Key = key;
            aes.Padding = PaddingMode.None;
            ICryptoTransform encryptPlatform = aes.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, encryptPlatform, CryptoStreamMode.Write))
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

