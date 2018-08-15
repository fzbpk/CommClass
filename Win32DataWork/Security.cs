using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
namespace Win32DataWork
{
    public class Security
    {
        #region 定义
        private static string CharSet = "";
        private static string ErrMsg = "";
      
        /// <summary>
        /// 获取错误
        /// </summary>
        public static string Error
        {
            get
            {
                string ess = ErrMsg;
                ErrMsg = "";
                return ess;
            }
        }

        #endregion

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>加密字符串</returns>
        public static string MD5(string code, string Key = "")
        {
            try
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                if (CharSet.Trim() == "")
                { return BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(code + Key))).Replace("-", ""); }
                else
                { return BitConverter.ToString(hashmd5.ComputeHash(Encoding.GetEncoding(CharSet).GetBytes(code + Key))).Replace("-", ""); }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>加密字符串</returns>
        public static string DesEncrypt(string code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = null;
                if (CharSet.Trim() == "")
                {
                    inputByteArray = Encoding.Default.GetBytes(code);
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    inputByteArray = Encoding.GetEncoding(CharSet).GetBytes(code);
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                des.Clear();
                ms.Close();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return ret.ToString();
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }



        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>加密字符串</returns>
        public static byte[] DesEncryptByByte(string code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = null;
                if (CharSet.Trim() == "")
                {
                    inputByteArray = Encoding.Default.GetBytes(code);
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    inputByteArray = Encoding.GetEncoding(CharSet).GetBytes(code);
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>加密字符串</returns>
        public static byte[] DesEncryptByte(byte[] code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = code;
                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>被加密字符串</returns>
        public static string DesDecrypt(string code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[code.Length / 2];
                for (int x = 0; x < code.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(code.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                des.Clear();
                ms.Close();
                string res = "";
                if (CharSet.Trim() == "")
                { res = System.Text.Encoding.Default.GetString(ms.ToArray()); }
                else
                { res = System.Text.Encoding.GetEncoding(CharSet).GetString(ms.ToArray()); }
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>被加密字符串</returns>
        public static byte[] DesDecrypt(byte[] code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = code;
                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
                cs.Read(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>被加密字符串</returns>
        public static string DesDecryptFromByte(byte[] code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = code;

                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                des.Clear();
                ms.Close();
                string res = "";
                if (CharSet.Trim() == "")
                { res = System.Text.Encoding.Default.GetString(ms.ToArray()); }
                else
                { res = System.Text.Encoding.GetEncoding(CharSet).GetString(ms.ToArray()); }
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>被加密字符串</returns>
        public static byte[] DesDecryptByByte(byte[] code, string Key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = code;

                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// TripeDes加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <param name="mode">块密码模式</param>
        /// <param name="type">填充</param>
        /// <returns>加密字符串</returns>
        public static string TripeDesEncrypt(string code, string Key, CipherMode mode = CipherMode.CBC, PaddingMode type = PaddingMode.PKCS7)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                byte[] inputByteArray = null;
                if (CharSet.Trim() == "")
                {
                    inputByteArray = Encoding.Default.GetBytes(code);
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    inputByteArray = Encoding.GetEncoding(CharSet).GetBytes(code);
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                des.Mode = mode;
                des.Padding = type;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                des.Clear();
                ms.Close();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return ret.ToString();
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// TripeDes加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <param name="mode">块密码模式</param>
        /// <param name="type">填充</param>
        /// <returns>加密字符串</returns>
        public static byte[] TripeDesEncryptByByte(string code, string Key, CipherMode mode = CipherMode.CBC, PaddingMode type = PaddingMode.PKCS7)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                byte[] inputByteArray = null;
                if (CharSet.Trim() == "")
                {
                    inputByteArray = Encoding.Default.GetBytes(code);
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    inputByteArray = Encoding.GetEncoding(CharSet).GetBytes(code);
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                des.Mode = mode;
                des.Padding = type;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// TripeDes加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <param name="mode">块密码模式</param>
        /// <param name="type">填充</param>
        /// <returns>加密字符串</returns>
        public static byte[] TripeDesEncryptByte(byte[] code, string Key, CipherMode mode = CipherMode.CBC, PaddingMode type = PaddingMode.PKCS7)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                byte[] inputByteArray = null;
                if (CharSet.Trim() == "")
                {
                    inputByteArray = code;
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    inputByteArray = code;
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                des.Mode = mode;
                des.Padding = type;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }


        /// <summary>
        /// TripeDes解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <param name="mode">块密码模式</param>
        /// <param name="type">填充</param>
        /// <returns>被加密字符串</returns>
        public static string TripeDesDecrypt(string code, string Key, CipherMode mode = CipherMode.CBC, PaddingMode type = PaddingMode.PKCS7)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                byte[] inputByteArray = new byte[code.Length / 2];
                for (int x = 0; x < code.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(code.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                des.Mode = mode;
                des.Padding = type;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                cs.Close();
                des.Clear();
                ms.Close();
                string res = "";
                if (CharSet.Trim() == "")
                { res = System.Text.Encoding.Default.GetString(ms.ToArray()); }
                else
                { res = System.Text.Encoding.GetEncoding(CharSet).GetString(ms.ToArray()); }
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// TripeDes解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <param name="mode">块密码模式</param>
        /// <param name="type">填充</param>
        /// <returns>被加密字符串</returns>
        public static byte[] TripeDesDecrypt(byte[] code, string Key, CipherMode mode = CipherMode.CBC, PaddingMode type = PaddingMode.PKCS7)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                byte[] inputByteArray = code;
                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                des.Mode = mode;
                des.Padding = type;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] res = ms.ToArray();
                cs.Close();
                des.Clear();
                ms.Close();
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }


        /// <summary>
        /// TripeDes解密
        /// </summary>
        /// <param name="code">加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <param name="mode">块密码模式</param>
        /// <param name="type">填充</param>
        /// <returns>被加密字符串</returns>
        public static string TripeDesDecryptByByte(byte[] code, string Key, CipherMode mode = CipherMode.CBC, PaddingMode type = PaddingMode.PKCS7)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                byte[] inputByteArray = code;
                if (CharSet.Trim() == "")
                {
                    des.Key = ASCIIEncoding.Default.GetBytes(Key);
                    des.IV = ASCIIEncoding.Default.GetBytes(Key);
                }
                else
                {
                    des.Key = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                    des.IV = ASCIIEncoding.GetEncoding(CharSet).GetBytes(Key);
                }
                des.Mode = mode;
                des.Padding = type;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                cs.Close();
                des.Clear();
                ms.Close();
                string res = "";
                if (CharSet.Trim() == "")
                { res = System.Text.Encoding.Default.GetString(ms.ToArray()); }
                else
                { res = System.Text.Encoding.GetEncoding(CharSet).GetString(ms.ToArray()); }
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return "";
            }
        }

    }
}
