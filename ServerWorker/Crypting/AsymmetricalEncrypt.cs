using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace ServerWorker.Crypting
{
    public class AsymmetricalEncrypt
    {
        public RSAParameters RsaEncryptParametrs = new RSAParameters();
        private byte[] encryptedData;
        private RSACryptoServiceProvider RSA;
        private UnicodeEncoding ByteConverter;
        public AsymmetricalEncrypt ()
        {

            RSA = new RSACryptoServiceProvider();
            RsaEncryptParametrs = RSA.ExportParameters(true);
            ByteConverter = new UnicodeEncoding();
        }


        public void GenerateNewASimKey()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RsaEncryptParametrs = RSA.ExportParameters(true);
        }
        public byte[] EncryptMessage(string message)
         {
            try
            {
                RSA.ImportParameters(RsaEncryptParametrs);

                encryptedData = RSA.Encrypt(ByteConverter.GetBytes(message), false);

                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Log.Send("Ошибка шифрования ключа" + e.ToString());

                return null;
            }
        }
       
    }
}
