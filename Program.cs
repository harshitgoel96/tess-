using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Tesseract;

namespace tessauto
{
    class Program
    {
        private string docPath = "G:\\tess-wrk\\input";
        private string savePath = "G:\\tess-wrk\\output";
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");
            Program p = new Program();
            p.StartProcess();
        }

        private void StartProcess()
        {
           var files = Directory.EnumerateFiles(docPath, "*.gif");
            foreach(var f in files)
            {
                Console.WriteLine("prcess "+f);
                Pix img = Pix.LoadFromFile(f);
                string enc = Process(img);
                saveToFile(f, enc);
            }

        }

        private string Process(Pix sourceImage)
        {
            //Tesseract. tess = new Tesseract();
            using (TesseractEngine engine = new TesseractEngine(@"G:\\Tess4J\\tessdata", "eng", EngineMode.Default))
            {
                using (var page = engine.Process(sourceImage))
                {
                    string ocrtext = page.GetText();
                    string enc = Encrypt(ocrtext);
                    return enc;
                }
            }
        }
        private void saveToFile(string file_name, string data)
        {
            FileStream fileStream = new FileStream(file_name + ".rtx", FileMode.Create, FileAccess.Write);
            fileStream.SetLength(0L);
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }
        private string Encrypt(string clearText)
        {
            string password = "9652AKO98CCR796";
            byte[] bytes = Encoding.Unicode.GetBytes(clearText);
            string encryped = string.Empty;
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, new byte[13]
                {
          (byte) 73,
          (byte) 118,
          (byte) 97,
          (byte) 110,
          (byte) 32,
          (byte) 77,
          (byte) 101,
          (byte) 100,
          (byte) 118,
          (byte) 101,
          (byte) 100,
          (byte) 101,
          (byte) 118
                });
                aes.Key = rfc2898DeriveBytes.GetBytes(32);
                aes.IV = rfc2898DeriveBytes.GetBytes(16);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.Close();
                    }
                    encryped = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return encryped;
        }
    }
}
