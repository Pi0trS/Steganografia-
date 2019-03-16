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
        public static ArrayList imageToBits(Image oldImage)
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
        public static Bitmap bitsToBmp(Image image, ArrayList bitsRGBA)
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

        public static ArrayList insretTextLenght(ArrayList imageInColors, String textToInsert)
        {
            ArrayList newImageInColors = new ArrayList();

            for (int i = 0; i < 12; i++)
            {
                newImageInColors.Add(imageInColors[i].ToString().Remove(imageInColors[i].ToString().Length - 1, 1) + textToInsert[i]);
            }
            return newImageInColors;
        }

        public static String outputTextLenght(ArrayList imageInColors)
        {
            String lenght = "", tmpLenght = "";
            for (int i = 0; i < 12; i++)
            {
                tmpLenght = imageInColors[i].ToString();
                lenght += tmpLenght[tmpLenght.Length - 1];    
            }
            return lenght;
        }

        public static String lenghtInBits(String s)
        {
            String lenght = Convert.ToString(s.Length, 2);
            while(lenght.Length < 12)
            {
                lenght += "0"; 
            }
            return lenght;
        }

        public static int lenghtInInt(String s)
        {
            int lenght = Convert.ToInt32(s, 2);
            return lenght;
        }

        public static String mesageToBits(String mesage)
        {
            int tmpI;
            String tmpS, mesageInBits = "";
            foreach (var c in mesage)
            {
                tmpI = Convert.ToInt32(c);
                tmpS = Convert.ToString(tmpI, 2);

                if (tmpS.Length == 6)
                {
                    mesageInBits += ("0" + tmpS);
                }
                else if(tmpS.Length == 7)
                {
                    mesageInBits += tmpS;
                }
            }
            return mesageInBits;
        }

        public static String bitsToMessage(String messageInBits)
        {
            String decodeMessage = "", tmpBitsString = "";
            int tmpI;

            foreach (var c in messageInBits)
            {
                tmpBitsString += c;
                if(tmpBitsString.Length % 7 == 0)
                {
                    tmpI = Convert.ToInt32(tmpBitsString, 2);
                    decodeMessage += Convert.ToChar(tmpI);
                    tmpBitsString = "";

                }
            }
            return decodeMessage;
        }

        public static String encrypt(String mesage, String password)
        {
            var bytesToencrypt = Encoding.UTF8.GetBytes(mesage);
            var passwordInBytes = Encoding.UTF8.GetBytes(password);

            passwordInBytes = SHA256.Create().ComputeHash(passwordInBytes);
            var encryptedBytes = encryptB(bytesToencrypt, passwordInBytes);
            return Convert.ToBase64String(encryptedBytes);

        }

        public static String decrypt(String mesageToDecrypt, String password)
        {
            var bytesToDecrypt = Convert.FromBase64String(mesageToDecrypt);
            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            passwordInBytes = SHA256.Create().ComputeHash(passwordInBytes);

        var bytesDecrypted = decryptB(bytesToDecrypt, passwordInBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        public static byte[] encryptB(byte[] byteToEncrypt, byte[] password)
        {
            byte[] encryptedB = null;
            var saltB = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(password, saltB, 1000);
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(byteToEncrypt, 0, byteToEncrypt.Length);
                        cs.Close();
                    }
                    encryptedB = ms.ToArray();
                }
                return encryptedB;
            }
        }

        public static byte[] decryptB(byte[] byteToDecrypt, byte[] password)
        {
            byte[] decryptedB = null;
            var saltB = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(password, saltB, 1000);
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(byteToDecrypt, 0, byteToDecrypt.Length);
                        cs.Close();
                    }
                    decryptedB = ms.ToArray();
                }
                return decryptedB;
            }
        }

        public static ArrayList codingXor(ArrayList lenghtImage, ArrayList Image, String lenghtMesage)
        {
            ArrayList codedImage = new ArrayList();
            codedImage = lenghtImage;
            int countImage = Image.Count;
            String tmpS = lenghtMesage + "00000001";
            int repeat = lenghtMesage.Length;
            int j = 0;
            byte ab1, ab2, ab3;
            String a1, a2, a3;
            String x1, x2, tmpS1, tmpS2, tmpS3;

            for (int i = 12; i < countImage; i+=3)
            {
                tmpS1 = Image[i].ToString();
                tmpS2 = Image[i + 1].ToString();
                tmpS3 = Image[i + 2].ToString();

                a1 = tmpS1[tmpS1.Length - 1].ToString();
                a2 = tmpS2[tmpS2.Length - 1].ToString();
                a3 = tmpS3[tmpS3.Length - 1].ToString();

                x1 = tmpS[j].ToString();
                j++;
                if (j >= tmpS.Length) j = 0;
                x2 = tmpS[j].ToString();
                j++;
                if (j >= tmpS.Length) j = 0;

                ab1 = Convert.ToByte(a1);
                ab2 = Convert.ToByte(a2);
                ab3 = Convert.ToByte(a3);

                String result1 = Convert.ToString(ab1 ^ ab3, 2);
                String result2 = Convert.ToString(ab2 ^ ab3, 2);

                if (result1 != x1 && result2 == x2)
                {
                    if (ab1 == 1) ab1 = 0; else ab1 = 1;
                }
                else
                {
                    if (result1 == x1 && result2 != x2)
                    {
                        if (ab2 == 1) ab2 = 0; else ab2 = 1;
                    }
                    else
                    {
                        if (result1 != x1 && result2 != x2)
                        {
                            if (ab3 == 1) ab3 = 0; else ab3 = 1;
                        }
                    }
                }
                codedImage.Add(tmpS1.Remove(tmpS1.Length - 1, 1) + ab1.ToString());
                codedImage.Add(tmpS2.Remove(tmpS2.Length - 1, 1) + ab2.ToString());
                codedImage.Add(tmpS3.Remove(tmpS3.Length - 1, 1) + ab3.ToString());
            }
            return codedImage;
        }

        public static String decodingXor(ArrayList image, int lenghtMesage)
        {
            String textImage = "";
            int lenghtImage = image.Count;
            String  a1, a2, a3, x1, x2;
            for (int i = 12; i < lenghtImage; i +=3)
            {
                a1 = image[i].ToString()[image[i].ToString().Length - 1].ToString();
                a2 = image[i + 1].ToString()[image[i + 1].ToString().Length - 1].ToString();
                a3 = image[i + 2].ToString()[image[i + 2].ToString().Length - 1].ToString();

                x1 = (Convert.ToByte(a1) ^ Convert.ToByte(a3)).ToString();
                x2 = (Convert.ToByte(a2) ^ Convert.ToByte(a3)).ToString();

                textImage = textImage + x1 + x2;
                if (textImage.Contains("00000001"))
                {
                    i = lenghtImage + 5;
                    textImage = textImage.Remove(textImage.Length - 8, 8);
                }
            }
            return textImage;
        }

        public static Image hideInformation(Image oldImage, String message, String password)
        {
            ArrayList image = imageToBits(oldImage);
            String messageLenght = lenghtInBits(message);
            ArrayList lenghtImage = insretTextLenght(image, messageLenght);
            String encryptedMessage = encrypt(message, password);
            String messageBits = mesageToBits(encryptedMessage);
            ArrayList newImage = codingXor(lenghtImage, image, messageBits);
            Bitmap newBitmap = bitsToBmp(oldImage, newImage);
            return newBitmap;
        }

        public static string showInformation(Image encryptedImage, String text, String password)
        {
            ArrayList image = imageToBits(encryptedImage);
            String messageLenght = outputTextLenght(image);
            int messageLenghtinInt = lenghtInInt(messageLenght);
            String decodeXor = decodingXor(image, messageLenghtinInt);
            String readeMessage = bitsToMessage(decodeXor);
            String decryptMessage = decrypt(readeMessage, password);
            return decryptMessage;

        }
        /*
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
    */}
}

