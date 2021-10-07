using Jellyfin.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Jellyfin.Helpers
{
    public class StorageHelpers
    {
        #region Singleton members

        private static StorageHelpers _instance;
        public static StorageHelpers Instance => _instance ?? (_instance = new StorageHelpers());

        #endregion

        #region Instance members

        private readonly string _appDataFolder;
        private readonly byte[] _symmetricKey;
        private readonly byte[] _initializationVector;

        public StorageHelpers()
        {
            _appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //Note: Generate your own private encryption key instead of this sample one: DsFZ3gWtpP5
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes("DsFZ3gWtpP5", Encoding.ASCII.GetBytes("DsFZ3gWtpP5"));
            _symmetricKey = keyGenerator.GetBytes(32);
            _initializationVector = keyGenerator.GetBytes(16);
        }

        public string SaveImage(byte[] imageBytes, string fileName)
        {
            try
            {
                string filePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, fileName);
                File.WriteAllBytes(filePath, imageBytes);
                return filePath;
            }
            catch (Exception e)
            {
                e.LogException();
                Debug.WriteLine(e);
                return null;
            }
        }

        public bool StoreToken(string key, string value)
        {
            try
            {
                string filePath = Path.Combine(_appDataFolder, $"{key}.txt");
                string encryptedToken = EncryptString(value);

                File.WriteAllText(filePath, encryptedToken);

                return true;
            }
            catch (Exception e)
            {
                e.LogException();
                Debug.WriteLine($"StoreToken Exception: {e}");
                return false;
            }
        }

        public string LoadToken(string key)
        {
            try
            {
                string filePath = Path.Combine(_appDataFolder, $"{key}.txt");

                if (File.Exists(filePath))
                {
                    string storedValue = File.ReadAllText(filePath);
                    string decryptedToken = DecryptString(storedValue);
                    return decryptedToken;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                e.LogException();
                Debug.WriteLine($"LoadToken Exception: {e}");
                return null;
            }
        }

        public bool DeleteToken(string key)
        {
            try
            {
                string filePath = Path.Combine(_appDataFolder, $"{key}.txt");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return true;
            }
            catch (Exception e)
            {
                e.LogException();
                Debug.WriteLine($"DeleteToken Exception: {e}");
                return false;
            }
        }

        public bool SaveSetting(string key, string value, string settingsFileName)
        {
            try
            {
                string filePath = Path.Combine(_appDataFolder, settingsFileName);

                Dictionary<string, string> settings = null;

                if (File.Exists(filePath))
                {
                    string json = "";
                    json = File.ReadAllText(filePath);
                    settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
                else
                {
                    settings = new Dictionary<string, string>();
                }

                settings[key] = value;

                string updatedJson = JsonConvert.SerializeObject(settings);

                File.WriteAllText(filePath, updatedJson);
                return true;
            }
            catch (Exception e)
            {
                e.LogException();
                return false;
            }
        }

        public string LoadSetting(string key, string settingsFileName)
        {
            try
            {
                string filePath = Path.Combine(_appDataFolder, settingsFileName);

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Dictionary<string, string> settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    return settings.ContainsKey(key) ? settings[key] : null;
                }
                else
                {
                    Dictionary<string, string> settings = new Dictionary<string, string>();
                    string json = JsonConvert.SerializeObject(settings);
                    File.WriteAllText(filePath, json);
                    return null;
                }
            }
            catch (Exception e)
            {
                e.LogException();
                return null;
            }
        }

        public bool SaveEncrypted(string key, byte[] unencryptedBytes)
        {
            try
            {
                byte[] encryptedBytes = EncryptBytes(unencryptedBytes);

                string filePath = Path.Combine(_appDataFolder, $".{key}.bin");

                File.WriteAllBytes(filePath, encryptedBytes);

                return true;
            }
            catch (Exception e)
            {
                e.LogException();
                return false;
            }
        }

        public byte[] LoadDecrypted(string key)
        {
            try
            {
                string filePath = Path.Combine(_appDataFolder, $".{key}.bin");

                if (File.Exists(filePath))
                {
                    byte[] encryptedBytes = File.ReadAllBytes(filePath);
                    byte[] decryptedBytes = DecryptBytes(encryptedBytes);
                    return decryptedBytes;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                e.LogException();
                return null;
            }
        }

        public static bool AppendToLogFile(string logContent, string filePath)
        {
            try
            {
                string timestamp = $"{DateTime.Now:G}----------------------------------; \r\n";

                StringBuilder content = new StringBuilder(timestamp).Append(logContent);

                if (File.Exists(filePath))
                {
                    File.AppendAllText(filePath, content.ToString());
                }
                else
                {
                    File.WriteAllText(filePath, content.ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"StorageHelpers.AppendToLogFile Exception: {ex}");
                return false;
            }
        }

        #endregion

        #region Encryption methods

        private string EncryptString(string inputText)
        {
            byte[] textBytes = Encoding.Unicode.GetBytes(inputText);
            byte[] encryptedBytes = EncryptBytes(textBytes);

            Debug.WriteLine($"EncryptString complete: {encryptedBytes.Length} bytes");
            return Convert.ToBase64String(encryptedBytes);
        }

        private string DecryptString(string encryptedText)
        {
            // NOTE: This string is encrypted first, THEN converted to Base64 (not just obfuscated as Base64)
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = DecryptBytes(encryptedBytes);

            Debug.WriteLine($"DecryptString complete: {decryptedBytes.Length} bytes");
            return Encoding.Unicode.GetString(decryptedBytes, 0, decryptedBytes.Length);
        }

        private byte[] EncryptBytes(byte[] unencryptedData)
        {
            // I chose Rijndael instead of AES because of it's support for larger block size (AES only support 128)
            using (RijndaelManaged cipher = new RijndaelManaged { Key = _symmetricKey, IV = _initializationVector })
            using (ICryptoTransform cryptoTransform = cipher.CreateEncryptor())
            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(unencryptedData, 0, unencryptedData.Length);
                cryptoStream.FlushFinalBlock();
                byte[] encryptedBytes = memoryStream.ToArray();
                Debug.WriteLine($"EncryptBytes complete: {encryptedBytes.Length} bytes");
                return encryptedBytes;
            }
        }

        private byte[] DecryptBytes(byte[] encryptedBytes)
        {
            using (RijndaelManaged cipher = new RijndaelManaged())
            using (ICryptoTransform cryptoTransform = cipher.CreateDecryptor(_symmetricKey, _initializationVector))
            using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
            {
                byte[] decryptedBytes = new byte[encryptedBytes.Length];
                int bytesRead = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                // Note - I'm using Take() to clean up junk bytes at the end of the array
                return decryptedBytes.Take(bytesRead).ToArray();
            }
        }

        #endregion
    }
}
