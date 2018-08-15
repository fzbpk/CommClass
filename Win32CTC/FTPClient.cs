using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Permissions;
using System.Security;
using System.Security.Cryptography;

namespace Win32CTC
{
    public class FTPClient
    {
        #region 定义
        public struct FtpFileInfo
        {
            public bool IsDirectory;
            public string FileName;
            public DateTime EditDate;
            public string Role;
            public long Size;
        }

        private NetworkCredential account = new NetworkCredential();
        private string Url;
        private int TOut;
        private bool supportAnyonemous = false;
        private bool IsMD5 = false;
        private bool IsChk = false;
        private string SafeKey = "";
        private string ErrMsg = "";
        #endregion

        #region 结构体
        /// <summary>
        /// 新建FTP客户端
        /// </summary>
        public FTPClient()
        {
            account.UserName = "";
            account.Password = "";
            TOut = 60 * 1000;
        }

        /// <summary>
        /// 新建FTP客户端
        /// </summary>
        /// <param name="FTPIP">地址</param>
        /// <param name="FTPPort">端口</param>
        /// <param name="UrlRef">地址参数</param>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="timeout">超时</param>
        public FTPClient(string FTPIP, int FTPPort,string UrlRef="", int timeout = 60)
        {
            Url = String.Format("ftp://{0}:{1}", FTPIP, FTPPort.ToString());
            if (UrlRef.Trim() != "")
            { Url = Url + "/" + UrlRef; }
            TOut = timeout * 1000;
        }
        #endregion

        #region 设置参数

        /// <summary>
        /// 获取FTP服务器IP
        /// </summary>
        public string IP
        {
            get 
            {
                if (Url.ToLower().Contains("ftp://"))
                {
                    return Url.Substring(Url.IndexOf(":") + 3, Url.LastIndexOf(":"));
                }
                else
                { return ""; }
            }
        }

        /// <summary>
        /// 获取FTP服务器端口
        /// </summary>
        public int Port
        {
            get
            {
                if (Url.ToLower().Contains("ftp://"))
                {
                    string ports = "";
                    if (Url.LastIndexOf("/") > Url.LastIndexOf(":"))
                    { ports = Url.Substring(Url.LastIndexOf(":") + 1, Url.LastIndexOf("/")); }
                    else
                    { ports = Url.Substring(Url.LastIndexOf(":") + 1 ); }
                    int res=0;
                    int.TryParse(ports,out res );
                    return res;
                }
                else
                { return 0; }
            }
        }

        /// <summary>
        /// 获取地址参数
        /// </summary>
        public string UrlRef
        {
            get
            {
                if (Url.ToLower().Contains("ftp://"))
                {
                    if (Url.LastIndexOf("/") > Url.LastIndexOf(":"))
                    { return Url.Substring(Url.LastIndexOf("/") + 1); }
                    else
                    { return ""; }
                }
                else
                { return ""; }
            }
        }

        /// <summary>
        /// 获取或设置超时
        /// </summary>
        public int TimeOut
        {
            get
            { return TOut / 1000; }
            set
            { TOut = value * 1000; }
        }

        /// <summary>
        /// 获取用户账户
        /// </summary>
        public string User
        {
            get
            { return account.UserName; }
        }

        /// <summary>
        /// 获取或设置是否使用匿名登录
        /// </summary>
        public bool UseAnyonemous
        {
            get
            { return supportAnyonemous; }
            set
            { supportAnyonemous = value; }
        }

        /// <summary>
        /// 获取或设置是否使用MD5加密
        /// </summary>
        public bool UseMD5
        {
            get { return IsMD5; }
            set { IsMD5 = value; }
        }

        /// <summary>
        /// 获取或设置密码是否已加密
        /// </summary>
        public bool HadMD5
        {
            get { return IsChk; }
            set { IsChk = value; }
        }

        /// <summary>
        /// 获取或设置密匙
        /// </summary>
        public string Key
        {
            get { return SafeKey; }
            set { SafeKey = value.ToString(); }
        }

        /// <summary>
        /// 设置FTP服务器地址
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="Ref">地址参数</param>
        public void SetUrl(string ip,int port,string Ref)
        {
            Url = String.Format("ftp://{0}:{1}", ip, port.ToString());
            if (Ref.Trim() != "")
            { Url = Url + "/" + Ref; }
        }

        /// <summary>
        /// 设置认证信息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <returns>错误代码，0成功，-1参数不正，-2密码不能为空</returns>
        public int SetAuthor(string UserName, string PassWord)
        {
            if (UserName == null)
            { UserName = ""; }
            if (PassWord == null)
            { PassWord = ""; }
            if (PassWord.Trim() != "" && UserName.Trim() == "")
            { return -1; }
            if (IsMD5 && PassWord.Trim() == "")
            { return -2; }
            account.UserName = UserName;
            if (IsMD5 && !IsChk)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                PassWord = BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(PassWord.ToLower().Trim()+SafeKey.Trim()))).Replace("-", "");
                account.Password = PassWord;
            }
            else
            { account.Password = PassWord; }
            account.Domain = "";
           return 0;
        }

        /// <summary>
        /// 重输入密码
        /// </summary>
        /// <param name="PassWord">密码</param>
        /// <returns>错误代码，0成功，-1参数不正，-2密码不能为空</returns>
        public int EditPassWord(string PassWord)
        {
            if (PassWord == null)
            { PassWord = ""; }
            if (IsMD5 && PassWord.Trim() == "")
            { return -2; }
            if (IsMD5 && !IsChk)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                PassWord = BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(PassWord.ToLower().Trim() + SafeKey.Trim()))).Replace("-", "");
                account.Password = PassWord;
            }
            else
            { account.Password = PassWord; }
            return 0;
        }

        /// <summary>
        /// 获取错误
        /// </summary>
        public string Error
        {
            get
            { return ErrMsg; }
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        public void ClearError()
        { ErrMsg = ""; }

        #endregion

        #region 方法
        /// <summary>
        /// 获取欢迎信息
        /// </summary>
        public string WellCome
        { 
            get
            {
                try
                {
                    if (this.Url.Trim() != "")
                    {
                        Uri uri = new Uri(this.Url);
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails ;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        string res = listResponse.WelcomeMessage;
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        return res;
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return "";
                }
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <returns>执行结果，FTP返回码</returns>
        public FtpStatusCode CreatDirectory(string FolderName)
       {
           try
           {
               if (FolderName == null)
               {
                   ErrMsg = "REF unvalid";
                   return FtpStatusCode.Undefined;
               }
               else if (FolderName.Trim() == "")
               {
                   ErrMsg = "REF unvalid";
                   return FtpStatusCode.Undefined;
               }
               else
               {
                   if (this.Url.Trim() != "")
                   {
                       string str = Url + "/" + FolderName.Trim();
                       Uri uri = new Uri(str);
                       FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                       listRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                       listRequest.Timeout = TimeOut;
                       if (account.UserName.Trim() != "")
                       { listRequest.Credentials = account; }
                       FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                       FtpStatusCode res = listResponse.StatusCode;
                       listResponse.Close();
                       listResponse = null;
                       listRequest = null;
                       return res;
                   }
                   return FtpStatusCode.Undefined;
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return FtpStatusCode.Undefined;
           }
       }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <returns>执行结果，FTP返回码</returns>
        public FtpStatusCode DeleteDirectory(string FolderName)
        {
            try
            {
                if (FolderName == null)
                {
                    ErrMsg = "REF unvalid";
                    return FtpStatusCode.Undefined;
                }
                else if (FolderName.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return FtpStatusCode.Undefined;
                }
                else
                {
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        Uri uri = new Uri(str);
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        FtpStatusCode res = listResponse.StatusCode;
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        return res;
                    }
                    return FtpStatusCode.Undefined;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return FtpStatusCode.Undefined;
            }
        }

        /// <summary>
        /// 重命名目录
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <param name="NewFolderName">新文件名</param>
        /// <returns>执行结果，FTP返回码</returns>
        public FtpStatusCode RenameDirectory(string FolderName, string NewFolderName)
        {
            try
            {
                if (FolderName == null || NewFolderName == null)
                {
                    ErrMsg = "REF unvalid";
                    return FtpStatusCode.Undefined;
                }
                else if (FolderName.Trim() == "" || NewFolderName.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return FtpStatusCode.Undefined;
                }
                else
                {
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        Uri uri = new Uri(str);
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.Rename ;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        listRequest.RenameTo = NewFolderName;
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        FtpStatusCode res = listResponse.StatusCode;
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        return res;
                    }
                    return FtpStatusCode.Undefined;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return FtpStatusCode.Undefined;
            }
        }

        /// <summary>
        /// 列出文件目录
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <returns>文件名列表</returns>
        public string[] ListDirectory(string FolderName)
        {
            try
            {
                if (FolderName == null)
                { FolderName = ""; }
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        Uri uri = new Uri(str);
                        List<string> list = new List<string>();
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        Stream responseStream = listResponse.GetResponseStream();
                        StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.Default);
                        while (readStream.Peek() >= 0)
                        { 
                           str = readStream.ReadLine();
                           list.Add(str);
                        }
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        if (list.Count > 0)
                        {
                            string[] res = new string[list.Count];
                            list.CopyTo(res);
                            return res;
                        }
                    }
                    return null;
                 
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 列出文件目录详细信息
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <returns>文件信息列表</returns>
        public FtpFileInfo[] ListDirectoryDetails(string FolderName)
        {
            try
            {
                if (FolderName == null)
                { FolderName = ""; }
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        Uri uri = new Uri(str);
                        List<FtpFileInfo> list = new List<FtpFileInfo>();
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails ;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        Stream responseStream = listResponse.GetResponseStream();
                        StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.Default);
                        while (readStream.Peek() >= 0)
                        {
                            str = readStream.ReadLine();
                            FtpFileInfo temp=new FtpFileInfo();
                            if (str.Substring(0, 1).ToUpper().Trim() == "D")
                            { temp.IsDirectory = true; }
                            else
                            { temp.IsDirectory = false; }
                            temp.FileName = str.Substring(55).Trim();
                            long ints=0;
                            long .TryParse(str.Substring(29, 12).Trim(),out ints);
                            temp.Size = ints;
                            DateTime DT;
                            DateTime.TryParse(str.Substring(42, 12).Trim(), out DT);
                            temp.EditDate = DT;
                            temp.Role = str.Substring(1, 9).Trim();
                            list.Add(temp); 
                        }
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        if (list.Count > 0)
                        {
                            FtpFileInfo[] res = new FtpFileInfo[list.Count];
                            list.CopyTo(res);
                            return res;
                        }
                    }
                    return null;
 
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取远程文件大小
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <param name="FileName">文件名</param>
        /// <returns>文件大小，字节</returns>
        public long FileSize(string FolderName, string FileName)
        {
            try
            {
                if (FolderName == null)
                { FolderName = ""; }
                if ( FileName == null)
                {
                    ErrMsg = "REF unvalid";
                    return -1;
                }
                else if (  FileName.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return -1;
                }
                else
                {
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        str = str + "/" + FileName;
                        Uri uri = new Uri(str);
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.GetFileSize ;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        long upsize = 0;
                        try
                        {
                            string[] res = listResponse.StatusDescription.Replace("\r\n", "").Split(' ');
                            upsize = long.Parse(res[1]);
                        }
                        catch
                        { }
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        return upsize;
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return -2;
            }
        }

        /// <summary>
        /// 删除远程文件
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <param name="FileName">文件名</param>
        /// <returns>执行结果，FTP返回码</returns>
        public FtpStatusCode DeleteFile(string FolderName, string FileName)
        {
            try
            {
                if (FolderName == null || FileName == null)
                {
                    ErrMsg = "REF unvalid";
                    return FtpStatusCode.Undefined;
                }
                else if (FolderName.Trim() == "" || FileName.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return FtpStatusCode.Undefined;
                }
                else
                {
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        str = str + "/" + FileName;
                        Uri uri = new Uri(str);
                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        FtpStatusCode res = listResponse.StatusCode;
                        listResponse.Close();
                        listResponse = null;
                        listRequest = null;
                        return res;
                    }
                    return FtpStatusCode.Undefined;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return FtpStatusCode.Undefined;
            }
        }

        /// <summary>
        /// 上传文件，带断点续传
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <param name="FileName">文件名</param>
        /// <param name="LocalPath">本地路径</param>
        /// <param name="LocalFile">本地文件名，为空则用FileName</param>
        /// <param name="Checked">是否对比大小</param>
        /// <param name="BufferSize">缓存大小</param>
        /// <returns>执行结果</returns>
        public bool UpLoadFile(string FolderName, string FileName, string LocalPath, string LocalFile = "", bool Checked = false, int BufferSize = 4096)
        {
            try
            {
                if (FolderName == null)
                { FolderName = ""; }
                if ( FileName == null || LocalPath==null)
                {
                    ErrMsg = "REF unvalid";
                    return false ;
                }
                else if ( FileName.Trim() == "" || LocalPath.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return false;
                }
                else
                {
                    FileInfo fi;
                    if (LocalFile.Trim() != "")
                    { fi = new FileInfo(LocalPath + "\\" + LocalFile); }
                    else
                    { fi = new FileInfo(LocalPath + "\\" + FileName); }               
                    if (!fi.Exists)
                    {
                        ErrMsg = "File NOT Exists";
                        return false;
                    }
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        str = str + "/" + FileName;
                        Uri uri = new Uri(str);

                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                        listRequest.Timeout = TimeOut;
                        listRequest.KeepAlive = true;
                        listRequest.UseBinary = false;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                        long upsize =0;
                        try
                        {
                            string[] res = listResponse.StatusDescription.Replace("\r\n", "").Split(' ');
                            upsize = long.Parse(res[1]);
                        }
                        catch
                        {}
                        listResponse.Close();
                        listResponse = null;
                        listRequest.Abort();

                        if (upsize > fi.Length)
                        {
                            listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                            listRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                            listRequest.Timeout = TimeOut;
                            if (account.UserName.Trim() != "")
                            { listRequest.Credentials = account; }
                            listResponse = (FtpWebResponse)listRequest.GetResponse();
                            FtpStatusCode res = listResponse.StatusCode;
                            listResponse.Close();
                            listResponse = null;
                            listRequest = null;
                        }

                        listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        listRequest.Method = WebRequestMethods.Ftp.AppendFile;
                        listRequest.ContentLength = fi.Length;
                        listRequest.Timeout = TimeOut;
                        listRequest.KeepAlive = true;
                        //使用ASCII模式
                        listRequest.UseBinary = true;
                        //使用主动(port)模式
                        listRequest.UsePassive = true;
                        byte[] content = new byte[BufferSize];
                        int dataRead=0;
                        using (FileStream fs = fi.OpenRead())
                        {
                            try
                            {
                                using (Stream rs = listRequest.GetRequestStream())
                                {
                                    do
                                    {
                                        fs.Seek(upsize, SeekOrigin.Begin);
                                        dataRead = fs.Read(content, 0, BufferSize);
                                        rs.Write(content, 0, dataRead);
                                        upsize += dataRead;
                                    }
                                    while (!(dataRead < BufferSize)); 
                                }
                            }
                            catch
                            {   }
                            finally
                            { fs.Close(); }
                        }
                        listRequest.Abort();
                        listRequest = null;

                        if (Checked)
                        {
                            listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                            listRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                            listRequest.Timeout = TimeOut;
                            listRequest.KeepAlive = true;
                            listRequest.UseBinary = false;
                            if (account.UserName.Trim() != "")
                            { listRequest.Credentials = account; }
                            listResponse = (FtpWebResponse)listRequest.GetResponse();
                            upsize = 0;
                            try
                            {
                                string[] res = listResponse.StatusDescription.Replace("\r\n", "").Split(' ');
                                upsize = long.Parse(res[1]);
                            }
                            catch
                            { }
                            listResponse.Close();
                            listResponse = null;
                            listRequest.Abort();
                            listRequest = null;

                            if (upsize == fi.Length)
                            { return true; }
                            else
                            { return false; }
                        }
                        else
                        { return true; }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 下载文件，带断点下载
        /// </summary>
        /// <param name="FolderName">目录名</param>
        /// <param name="FileName">文件名</param>
        /// <param name="LocalPath">本地路径</param>
        /// <param name="LocalFile">本地文件名，为空则用FileName</param>
        /// <param name="ReDownLoad">重新下载</param>
        /// <param name="Checked">是否对比大小</param>
        /// <param name="BufferSize">缓存大小</param>
        /// <returns>执行结果</returns>
        public bool DownLoadFile(string FolderName, string FileName, string LocalPath, string LocalFile = "", bool ReDownLoad = false, bool Checked = false, int BufferSize = 4096)
        {
            try
            {
                if (FolderName == null)
                { FolderName = ""; }
                if ( FileName == null || LocalPath == null)
                {
                    ErrMsg = "REF unvalid";
                    return false;
                }
                else if (  FileName.Trim() == "" || LocalPath.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return false;
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(LocalPath);
                    if (!di.Exists)
                    { di.Create(); }
                    FileInfo fi;
                    if (LocalFile.Trim() != "")
                    { fi = new FileInfo(LocalPath + "\\" + LocalFile); }
                    else
                    { fi = new FileInfo(LocalPath + "\\" + FileName); }        
                    if (this.Url.Trim() != "")
                    {
                        string str = Url + "/" + FolderName.Trim();
                        str = str + "/" + FileName;
                        Uri uri = new Uri(str);

                        FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.DownloadFile ;
                        listRequest.Timeout = TimeOut;
                        if (account.UserName.Trim() != "")
                        { listRequest.Credentials = account; }
                        listRequest.KeepAlive = true;
                        listRequest.UseBinary = true;
                        listRequest.UsePassive = true;
                        System.IO.FileStream fs;
                        if (fi.Exists)
                        {
                            if (ReDownLoad)
                            {
                                fi.Delete();
                                fs = new System.IO.FileStream(fi.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            }
                            else
                            {
                                listRequest.ContentOffset = fi.Length;
                                fs = new System.IO.FileStream(fi.FullName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                            }
                        }
                        else
                        { fs = new System.IO.FileStream(fi.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write);}
                        bool resdo = false;
                        try
                        {
                            FtpWebResponse ftpRes = (FtpWebResponse)listRequest.GetResponse();
                            System.IO.Stream resStrm = ftpRes.GetResponseStream();
                            byte[] buffer = new byte[BufferSize];
                            while (true)
                            {
                                int readSize = resStrm.Read(buffer, 0, buffer.Length);
                                if (readSize == 0)
                                    break;
                                fs.Write(buffer, 0, readSize);
                            }
                            resStrm.Close();
                            ftpRes.Close();
                            ftpRes = null;
                            resdo = true;
                        }
                        catch (Exception exx)
                        {
                            ErrMsg = exx.Message;
                            resdo = false; 
                        }
                        finally
                        {
                            fs.Close();
                            listRequest = null;
                        }
                        if (Checked)
                        {
                            listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                            listRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                            listRequest.Timeout = TimeOut;
                            listRequest.KeepAlive = true;
                            listRequest.UseBinary = false;
                            if (account.UserName.Trim() != "")
                            { listRequest.Credentials = account; }
                            FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                            long upsize = 0;
                            try
                            {
                                string[] res = listResponse.StatusDescription.Replace("\r\n", "").Split(' ');
                                upsize = long.Parse(res[1]);
                            }
                            catch
                            { }
                            listResponse.Close();
                            listResponse = null;
                            listRequest.Abort();
                            if (LocalFile.Trim() != "")
                            { fi = new FileInfo(LocalPath + "\\" + LocalFile); }
                            else
                            { fi = new FileInfo(LocalPath + "\\" + FileName); }  
                            if (fi.Length == upsize)
                            { return true; }
                            else
                            { return false; }
                        }
                        else
                        { return resdo; } 
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }

        #endregion
    }
}
