using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;
namespace Win32CTC
{
    internal  class Security
    {
        #region 定义
        private static string CharSet = "";
        private static string ErrMsg = "";

        public enum Security_Mode: byte
        { 
           None=0,
           MD5=1,
           DES=2,
           TripeDes=3,
        }

        public enum CheckSum_Mode : byte
        {
            None = 0,
            CRC8 = 1,
            CRC16 = 2,
            CRC32 = 3,
            XOR = 4
        }

    
        /// <summary>
        /// 获取错误
        /// </summary>
        public static  string Error
        {
            get
            { return ErrMsg; }
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        public static void ClearError()
        { ErrMsg = ""; }
        #endregion
        
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="code">被加密字符串</param>
        /// <param name="Key">密匙</param>
        /// <returns>加密字符串</returns>
        public static  string MD5(string code,string Key="")
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
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write );
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                des.Clear();
                ms.Close();
                string res = "";
                if (CharSet.Trim() == "")
                { res= System.Text.Encoding.Default.GetString(ms.ToArray()); }
                else
                { res= System.Text.Encoding.GetEncoding(CharSet).GetString(ms.ToArray()); }
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
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write );
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
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write );
                cs.Write (inputByteArray, 0, inputByteArray.Length);
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
        public static string TripeDesEncrypt(string code, string Key, CipherMode mode= CipherMode.CBC, PaddingMode type= PaddingMode.PKCS7)
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
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(),  CryptoStreamMode.Write);
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
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write );
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
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write );
                cs.Write (inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                cs.Close();
                des.Clear();
                ms.Close();
                string res = "";
                if (CharSet.Trim() == "")
                { res=System.Text.Encoding.Default.GetString(ms.ToArray()); }
                else
                { res=System.Text.Encoding.GetEncoding(CharSet).GetString(ms.ToArray()); }
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
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write );
                cs.Write (inputByteArray, 0, inputByteArray.Length);
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
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write );
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
        /// CRC16 
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <param name="key">多项式</param>
        /// <returns>校验码</returns>
        public static ushort CRC16(byte[] data, ushort key)
        {
            try
            {
                byte CRC16Lo;
                byte CRC16Hi;
                byte CL; byte CH;
                byte SaveHi; byte SaveLo;
                byte[] tmpData;
                int I;
                int Flag;
                CRC16Lo = 0xFF;
                CRC16Hi = 0xFF;
                CL = (byte)(key % 256);
                CH = (byte)(key / 256);
                tmpData = data;
                for (int i = 0; i < tmpData.Length; i++)
                {
                    CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                    for (Flag = 0; Flag <= 7; Flag++)
                    {
                        SaveHi = CRC16Hi;
                        SaveLo = CRC16Lo;
                        CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                        CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                        if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                        {
                            CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                        }             //否则自动补0 
                        if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                        {
                            CRC16Hi = (byte)(CRC16Hi ^ CH);
                            CRC16Lo = (byte)(CRC16Lo ^ CL);
                        }
                    }
                }
                byte[] ReturnData = new byte[2];
                return (ushort)(CRC16Hi * 256 + CRC16Lo);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// CRC8
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <param name="key">多项式</param>
        /// <returns>校验码</returns>
        public static byte CRC8(byte[] data, byte key)
         {
             try
             {
                 byte CRCPoly = key;//CRC多项式，当做除数
                 byte[] Data = data;
                 byte CRCTempResult = 0x00;//CRC结果运算的得数，但不是最后的值
                 byte CRCResult = 0x00;//CRC结果运算最后的值 
                 CRCTempResult = (byte)(Data[0] ^ CRCPoly);
                 for (int arrayLength = 1; arrayLength <= (data.Length - 1); arrayLength++)
                 {
                     for (int i = 0; i < 8; i++)
                     {
                         if ((CRCTempResult & 0x80) == 0x00)
                         {
                             CRCTempResult = (byte)(CRCTempResult << 1);
                             CRCTempResult = (byte)(CRCTempResult | ((Data[arrayLength] & 0x80) == 0x80 ? 0x01 : 0x00));
                             Data[arrayLength] <<= 1;
                         }
                         else
                         {
                             CRCTempResult = (byte)(CRCTempResult ^ CRCPoly);
                             i--;
                         }
                     }
                 }
                 CRCResult = CRCTempResult;
                 return CRCResult;
             }
             catch (Exception ex)
             {
                 ErrMsg = ex.Message;
                 return 0;
             }
        }

        /// <summary>
        /// CRC32
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <param name="key">多项式</param>
        /// <returns>校验码</returns>
        public static uint CRC32(byte[] data, uint key)
        { 
            uint crc = 0xFFFFFFFF; // initial contents of LFBSR 
            for (int j =0;j<data.Length ;j++) 
            { 
                uint temp = (crc ^ data[j]) & 0xff; 
                for (int i = 0; i < 8; i++) 
                { 
                   if ((temp & 1) == 1) 
                       temp = (temp >>  1) ^ key; 
                   else 
                        temp = (temp >>  1); 
                } 
                crc = (crc >> 8 ^ temp); 
             } 
             crc = crc ^ 0xffffffff; 
             return crc; 
          }

        /// <summary>
        /// 文件CRC32
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="BufferSize">缓存大小</param>
        /// <returns>CRC值</returns>
        public static uint CRC32File(string FilePath, int BufferSize = 4096)
        {
            uint[] crcTable = {  
 0x0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,  
 0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,  
 0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,  
 0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,  
 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,  
 0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,  
 0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,  
 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,  
 0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433,  
 0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,  
 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,  
 0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,  
 0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,  
 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,  
 0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,  
 0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,  
 0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683,  
 0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,  
 0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,  
 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,  
 0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,  
 0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,  
 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,  
 0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,  
 0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713,  
 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,  
 0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,  
 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,  
 0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,  
 0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,  
 0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,  
 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,  
};
            try
            {
                           
                uint crc = 0xFFFFFFFF;
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                byte[] bytes = new byte[BufferSize];
                int iCount = bytes.Length;
                long upsize = 0;
                do
                {
                    fs.Seek(upsize, SeekOrigin.Begin);
                    iCount = fs.Read(bytes, 0, BufferSize);
                    for (int i = 0; i < iCount; i++)
                    {
                        crc = ((crc >> 8) & 0x00FFFFFF) ^ crcTable[(crc ^ bytes[i]) & 0xFF];
                    }
                    upsize += iCount;
                }
                while (!(iCount < BufferSize));
                fs.Close();
                uint temp = crc ^ 0xFFFFFFFF;
                uint t = temp;
                return (t);  
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return 0;
            }
           
        }


        /// <summary>
        /// 异或
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <returns>校验码</returns>
        public static byte Xor(byte[] data)  
        {
            try
            {
                byte XorByte = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (i == 0)
                    { XorByte = data[i]; }
                    else
                    { XorByte =(byte)(XorByte ^ data[i]); }
                }
                return XorByte;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return 0;
            }
        }

    }
}
