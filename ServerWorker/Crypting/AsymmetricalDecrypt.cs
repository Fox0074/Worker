using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.Crypting
{
    public class AsymmetricalDecrypt
    {
        public RSAParameters RsaDecryptParametrs = new RSAParameters();
        private byte[] decryptedData;
        private RSACryptoServiceProvider RSA;
        private UnicodeEncoding ByteConverter;


        public AsymmetricalDecrypt(RSAParameters decryptParametrs)
        {
            ByteConverter = new UnicodeEncoding();
            RSA = new RSACryptoServiceProvider();
            SetKey(decryptParametrs);
        }

        public string DecryptMessage(byte[] decryptedData)
        {
            string result = "";

            try
            {               
                decryptedData = RSA.Decrypt(decryptedData, false);
                result = ByteConverter.GetString(decryptedData);
            }
            catch(CryptographicException e)
            {
                Log.Send("Ошибка дешифрования ключа: " + e.Message);
            }

            return result;
            
        }

        public void SetKey (RSAParameters decryptParametrs)
        {
            RsaDecryptParametrs = decryptParametrs;
            RSA.ImportParameters(decryptParametrs);
            
        }
    }
}
