using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeleted
{
    public static class Crypting
    {
        public static string LastMessage = "";

        public static string publickey ;
        public static string privatekey;
        public static RSACryptoServiceProvider RsaKey;

        private static StringBuilder stringBuilder;

        public static void Init()
        {
            RsaKey = new RSACryptoServiceProvider();
            publickey = RsaKey.ToXmlString(false);
            privatekey = RsaKey.ToXmlString(true);
            stringBuilder = new StringBuilder();
        }

        public static void PrintKeys()
        {
            Console.WriteLine("publickey: " + publickey);
            Console.WriteLine("privatekey: " + privatekey);
        }
        public static byte[] CryptMessage(string message)
        {
            string test = "";
            LastMessage = message;
            stringBuilder.Clear();
            byte[] EncryptedData;
            byte[] DecryptedData;
            byte[] data ;
            data = Encoding.UTF8.GetBytes(message);
            EncryptedData = RsaKey.Encrypt(data, false);
            test = Convert.ToBase64String(EncryptedData);
            Console.WriteLine("EncryptedData: " + test);

            data = Convert.FromBase64String(test);
            
            DecryptedData = RsaKey.Decrypt(data, false);
    
            stringBuilder.Clear();
            stringBuilder.Append(Encoding.UTF8.GetString(DecryptedData));
            Console.WriteLine("DecryptedData: " + stringBuilder.ToString());

            return EncryptedData;
        }

        const string ContainerName = "MyContainer";
        public static void AssignNewKey()
        {
            CspParameters cspParams = new CspParameters(1);
            cspParams.KeyContainerName = ContainerName;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            var rsa = new RSACryptoServiceProvider(cspParams) { PersistKeyInCsp = true };
        }
        public static void DeleteKeyInCsp()
        {
            var cspParams = new CspParameters { KeyContainerName = ContainerName };
            var rsa = new RSACryptoServiceProvider(cspParams) { PersistKeyInCsp = false };
            rsa.Clear();
        }
        public static byte[] EncryptData(byte[] dataToEncrypt)
        {
            byte[] cipherbytes;
            var cspParams = new CspParameters { KeyContainerName = ContainerName };
            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                cipherbytes = rsa.Encrypt(dataToEncrypt, false);
            }
            return cipherbytes;
        }
        public static byte[] DecryptData(byte[] dataToDecrypt)
        {
            byte[] plain;
            var cspParams = new CspParameters { KeyContainerName = ContainerName };
            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                plain = rsa.Decrypt(dataToDecrypt, false);
            }
            return plain;
        }
    }
}
