using Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using System.Text;

namespace Service
{
    public class DPAPI
    {
        [DllImport("crypt32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern
            bool CryptUnprotectData(ref DATA_BLOB pCipherText, ref string pszDescription, ref DATA_BLOB pEntropy,
                  IntPtr pReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPrompt, int dwFlags, ref DATA_BLOB pPlainText);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }
        static private IntPtr NullPtr = ((IntPtr)((int)(0)));
        private const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;
        private static void InitPrompt(ref CRYPTPROTECT_PROMPTSTRUCT ps)
        {
            ps.cbSize = Marshal.SizeOf(
                                      typeof(CRYPTPROTECT_PROMPTSTRUCT));
            ps.dwPromptFlags = 0;
            ps.hwndApp = NullPtr;
            ps.szPrompt = null;
        }
        private static void InitBLOB(byte[] data, ref DATA_BLOB blob)
        {
            // Use empty array for null parameter.
            if (data == null)
                data = new byte[0];

            // Allocate memory for the BLOB data.
            blob.pbData = Marshal.AllocHGlobal(data.Length);

            // Make sure that memory allocation was successful.
            if (blob.pbData == IntPtr.Zero)
                throw new Exception(
                    "Unable to allocate data buffer for BLOB structure.");

            // Specify number of bytes in the BLOB.
            blob.cbData = data.Length;

            // Copy data from original source to the BLOB structure.
            Marshal.Copy(data, 0, blob.pbData, data.Length);
        }
        public static byte[] Decrypt(byte[] cipherTextBytes, byte[] entropyBytes, out string description)
        {
            // Create BLOBs to hold data.
            DATA_BLOB plainTextBlob = new DATA_BLOB();
            DATA_BLOB cipherTextBlob = new DATA_BLOB();
            DATA_BLOB entropyBlob = new DATA_BLOB();

            // We only need prompt structure because it is a required
            // parameter.
            CRYPTPROTECT_PROMPTSTRUCT prompt =
                                      new CRYPTPROTECT_PROMPTSTRUCT();
            InitPrompt(ref prompt);

            // Initialize description string.
            description = String.Empty;

            try
            {
                // Convert ciphertext bytes into a BLOB structure.
                try
                {
                    InitBLOB(cipherTextBytes, ref cipherTextBlob);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Cannot initialize ciphertext BLOB.", ex);
                }

                // Convert entropy bytes into a BLOB structure.
                try
                {
                    InitBLOB(entropyBytes, ref entropyBlob);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Cannot initialize entropy BLOB.", ex);
                }

                // Disable any types of UI. CryptUnprotectData does not
                // mention CRYPTPROTECT_LOCAL_MACHINE flag in the list of
                // supported flags so we will not set it up.
                int flags = CRYPTPROTECT_UI_FORBIDDEN;

                // Call DPAPI to decrypt data.
                bool success = CryptUnprotectData(ref cipherTextBlob,
                                                  ref description,
                                                  ref entropyBlob,
                                                      IntPtr.Zero,
                                                  ref prompt,
                                                      flags,
                                                  ref plainTextBlob);

                // Check the result.
                if (!success)
                {
                    // If operation failed, retrieve last Win32 error.
                    int errCode = Marshal.GetLastWin32Error();

                    // Win32Exception will contain error message corresponding
                    // to the Windows error code.
                    throw new Exception(
                        "CryptUnprotectData failed.", new Win32Exception(errCode));
                }

                // Allocate memory to hold plaintext.
                byte[] plainTextBytes = new byte[plainTextBlob.cbData];

                // Copy ciphertext from the BLOB to a byte array.
                Marshal.Copy(plainTextBlob.pbData,
                             plainTextBytes,
                             0,
                             plainTextBlob.cbData);

                // Return the result.
                return plainTextBytes;
            }
            catch (Exception ex)
            {
                throw new Exception("DPAPI was unable to decrypt data.", ex);
            }
            // Free all memory allocated for BLOBs.
            finally
            {
                if (plainTextBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(plainTextBlob.pbData);

                if (cipherTextBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(cipherTextBlob.pbData);

                if (entropyBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }

        public static List<LoginData> GetPasswords(string dataPath)
        {
            List<LoginData> loginData = new List<LoginData>();

            string connectionString = $"Data Source = {dataPath}";
            byte[] entropy = null;
            string description;

            DataTable db = new DataTable();
            string sql = $"SELECT * FROM {"logins"}";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString,true))
            {
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(db);
            }

            int rows = db.Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                string url = db.Rows[i][1].ToString();
                string login = db.Rows[i][3].ToString();
                byte[] byteArray = (byte[])db.Rows[i][5];
                byte[] decrypted = DPAPI.Decrypt(byteArray, entropy, out description);
                string password = new UTF8Encoding(true).GetString(decrypted);
                loginData.Add(new LoginData(url, login, password));
            }

            return loginData;
        }
    }
}
