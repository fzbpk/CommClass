using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Win32CTC
{
    public class HttpSdk
    {
        #region 定义
        private CookieContainer session = null;
        private System.Net.HttpWebRequest myHttpWebRequest;
        private System.Net.HttpWebResponse myHttpWebResponse;
        private string TransferEncoding;
        private string ConnType = "";
        private string RConnType = "";
        private string ConnCharSet = "";
        private string ConnEncode = "";
        private string refer = "";
        private int iStatCode = -1;
        private int TOut = 0;
        private bool sendchunked = false;
        private bool keepalive = false;
        private string useragent = "";
        private string ErrMsg = "";
        #endregion

        #region 构造函数
        public HttpSdk()
        {
            TOut = 60000;
            ConnType = "text/HTML";
        }

        public HttpSdk(string ConType, int timeout=60000, string refer="", string agent="")
        {
            TOut = timeout;
            ConnType = ConType;
            if (agent.Trim() != "")
            { myHttpWebRequest.UserAgent = agent; }
            if (refer.Trim() != "")
            {
                if (refer.ToLower().Contains("http://"))
                { myHttpWebRequest.Referer = refer; }
                else
                { myHttpWebRequest.Referer = "http://" + refer; }
            }
        }
        #endregion

        #region 属性

        public int HtmlStatus
        {get{ return iStatCode; } }

        public string HtmlCode 
        {
            get{ return ConnEncode; }
            set 
            { ConnEncode = value; }
        }

        public void SetHtmlCode(string code)
        {
            if (code.Trim() == "")
            { sendchunked = false; }
            else
            {
                TransferEncoding = code.Trim();
                sendchunked = true;
            }
        }

        public string CharSet 
        {get{ return ConnCharSet; }  }

        public string HtmlConnType
        {
            get { return RConnType; }
            set { ConnType = value; }
        }

        public string Refer 
        {
            get { return refer; }
            set
            {
                if (value.Trim() != "")
                {
                    if (value.ToLower().Contains("http://"))
                    { refer = value; }
                    else
                    { refer = "http://" + value; }
                }
            }
        }

        public bool KeepAlive
        {
            get { return keepalive; }
            set { keepalive = value; }
        }

        public string UserAgent 
        {
            get { return useragent; }
            set { useragent = value; }
        }

        public int TimeOut 
        {
            get { return TOut / 1000; }
            set { TOut = value * 1000; }            
        }

        public CookieContainer Session 
        {
            get { return session; }
            set { session = value; }
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
        /// 获取URL上的HTML页面
        /// </summary>
        /// <param name="URL">地址</param>
        /// <param name="Method">方式，使用POST或GET</param>
        /// <param name="usesession">是否需要SESSION</param>
        /// <returns>HTML代码</returns>
        public string GetHtml(string URL , bool usesession=false )
        {
            string Html = "";
            try
            {
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                myHttpWebRequest.ContentType = ConnType;
                myHttpWebRequest.KeepAlive =keepalive;
                myHttpWebRequest.Timeout = TOut ;
                myHttpWebRequest.Method = "GET";
                if (usesession)
                { myHttpWebRequest.CookieContainer = session;}
                if (useragent.ToString().Trim() != "")
                { myHttpWebRequest.UserAgent = useragent;}
                if (refer.ToString().Trim() != "")
                { myHttpWebRequest.Referer = refer; }
                if (sendchunked)
                {
                    myHttpWebRequest.SendChunked = true;
                    myHttpWebRequest.TransferEncoding = TransferEncoding;
                }
                //接收
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                iStatCode = (int)myHttpWebResponse.StatusCode;
                RConnType = myHttpWebResponse.ContentType;
                ConnCharSet = myHttpWebResponse.CharacterSet;
                ConnEncode = myHttpWebResponse.ContentEncoding;
                if (iStatCode == 200)
                {
                    StreamReader reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.GetEncoding(ConnCharSet));
                    Html = reader.ReadToEnd();
                    reader.Close();
                }
                else
                { ErrMsg = "HTTP ERR:" + iStatCode.ToString();}
                myHttpWebResponse.Close();
                myHttpWebRequest.Abort();
            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }
            return Html;
        }

        /// <summary>
        /// 提交表单并返回URL上的HTML页面
        /// </summary>
        /// <param name="URL">地址</param>
        /// <param name="Postdata">提交表单数据</param>
        /// <param name="usesession">是否需要SESSION</param>
        /// <returns>HTML代码</returns>
        public string PostHtml(string URL, String Postdata, bool usesession)
        {
            string Html = "";
            try
            {
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.KeepAlive = keepalive;
                myHttpWebRequest.Timeout = TOut;
                myHttpWebRequest.Method = "Post";
                if (usesession)
                { myHttpWebRequest.CookieContainer = session;}
                if (useragent.Trim() != "")
                { myHttpWebRequest.UserAgent = useragent; }
                if (refer.ToString().Trim() != "")
                { myHttpWebRequest.Referer = refer; }
                if (sendchunked)
                {
                    myHttpWebRequest.SendChunked = true;
                    myHttpWebRequest.TransferEncoding = TransferEncoding;
                }
                if (CharSet.Trim() != "")
                {
                    byte[] data = Encoding.GetEncoding(CharSet.Trim()).GetBytes(Postdata);
                    myHttpWebRequest.ContentLength = data.Length;
                    Stream newStream = myHttpWebRequest.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                else
                {
                    byte[] data = Encoding.ASCII.GetBytes(Postdata);
                    myHttpWebRequest.ContentLength = data.Length;
                    Stream newStream = myHttpWebRequest.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                //接收
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                iStatCode = (int)myHttpWebResponse.StatusCode;
                RConnType = myHttpWebResponse.ContentType;
                ConnCharSet = myHttpWebResponse.CharacterSet;
                ConnEncode = myHttpWebResponse.ContentEncoding;
                if (iStatCode == 200)
                {
                    StreamReader reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.GetEncoding(ConnCharSet));
                    Html = reader.ReadToEnd();
                    reader.Close();
                }
                else
                { ErrMsg = "HTTP ERR:" + iStatCode.ToString(); }
                myHttpWebResponse.Close();
                myHttpWebRequest.Abort();
            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }
            return Html;
        }

        /// <summary>
        /// HTTP文件下载
        /// </summary>
        /// <param name="URL">文件HTTP地址，HTTP地址不支持中文字符，中午字符需要做URL编码</param>
        /// <param name="SavePath">保存路径</param>
        /// <param name="FileName">保存文件名，为空则</param>
        /// <param name="usesession">是否需要SESSION</param>
        /// <param name="ReDownLoad">是否覆盖原文件</param>
        /// <param name="BufferSize">缓存大小</param>
        /// <returns>执行结果</returns>
        public bool DownLoad(string URL, string SavePath, string FileName = "", bool usesession = false,bool ReDownLoad=false, int BufferSize = 4096)
        {
            try
            {
                if (URL == null || FileName == null || SavePath == null)
                {
                    ErrMsg = "REF unvalid";
                    return false;
                }
                else if (URL.Trim() == "" || FileName.Trim() == "" || SavePath.Trim() == "")
                {
                    ErrMsg = "REF unvalid";
                    return false;
                }
                else
                {
                    bool res = false;
                    DirectoryInfo di = new DirectoryInfo(SavePath);
                    if (!di.Exists)
                    { di.Create(); }
                    FileInfo fi;
                    if (FileName.Trim() != "")
                    { fi = new FileInfo(SavePath + "\\" + FileName); }
                    else
                    {
                        FileName = URL.Substring(URL.LastIndexOf("/") + 1);
                        fi = new FileInfo(SavePath + "\\" + FileName); 
                    }
                    long index = 0;
                    System.IO.FileStream fs=null;
                    try
                    {
                        myHttpWebRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                        myHttpWebRequest.KeepAlive = keepalive;
                        myHttpWebRequest.Timeout = TOut;
                        if (usesession)
                        { myHttpWebRequest.CookieContainer = session; }
                        if (useragent.Trim() != "")
                        { myHttpWebRequest.UserAgent = useragent; }
                        if (refer.ToString().Trim() != "")
                        { myHttpWebRequest.Referer = refer; }
                        if (fi.Exists)
                        {
                            if (ReDownLoad)
                            {
                                fi.Delete();
                                fs = new System.IO.FileStream(fi.FullName, System.IO.FileMode.Create);
                            }
                            else
                            {
                                index = fi.Length;
                                fs = System.IO.File.OpenWrite(fi.FullName);
                                fs.Seek(index, System.IO.SeekOrigin.Begin);
                                myHttpWebRequest.AddRange((int)index);
                            }
                        }
                        else
                        { fs = new System.IO.FileStream(fi.FullName, System.IO.FileMode.Create); }
                        myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                        iStatCode = (int)myHttpWebResponse.StatusCode;
                        if (iStatCode == 200 || iStatCode == 206)
                        {
                            Stream myStream = myHttpWebResponse.GetResponseStream();
                            //定义一个字节数据
                            byte[] btContent = new byte[BufferSize];
                            int intSize = 0;
                            intSize = myStream.Read(btContent, 0, btContent.Length);
                            while (intSize > 0)
                            {
                                fs.Write(btContent, 0, intSize);
                                intSize = myStream.Read(btContent, 0, btContent.Length);
                            }
                            //关闭流
                          
                            myStream.Close();
                            myHttpWebResponse.Close();
                            myHttpWebRequest.Abort();
                            res= true;
                        }
                        else
                        {
                            myHttpWebResponse.Close();
                            myHttpWebRequest.Abort();
                            ErrMsg = "HTTP ERR:" + iStatCode.ToString();
                            res= false;
                        }            
                    }
                    catch (Exception ex)
                    { 
                        ErrMsg = ex.Message;
                        res = false;
                    }
                    finally
                    {
                        fs.Close();
                        fs = null;
                    }
                    return res;       
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// HTTP文件上传
        /// </summary>
        /// <param name="URL">HTTP上传地址</param>
        /// <param name="FileFullName">本地文件路径</param>
        /// <param name="FileName">服务器文件名</param>
        /// <param name="usesession">是否需要SESSION</param>
        /// <param name="BufferSize">缓存大小</param>
        /// <returns>执行结果</returns>
        public bool UpLoad(string URL, string FileFullName, string FileName = "", bool usesession = false, int BufferSize = 4096)
        {
            try
            {
                if (URL == null || FileFullName == null)
                {
                    ErrMsg = "REF unvalid";
                    return false;
                }
                else if (URL.Trim() == "" || FileFullName.Trim() == "")
              {
                  ErrMsg = "REF unvalid";
                  return false;
              }
              else
              {
                  FileInfo fi= new FileInfo(FileFullName.Trim());
                  if (fi.Exists)
                  {
                      //时间戳 
                      string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x"); 
                      byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
                      //请求头部信息 
                      StringBuilder sb = new StringBuilder(); 
                      sb.Append("--"); 
                      sb.Append(strBoundary); 
                      sb.Append("\r\n"); 
                      sb.Append("Content-Disposition: form-data; name=\""); 
                      sb.Append("file"); 
                      sb.Append("\"; filename=\""); 
                      if(FileName.Trim()=="")
                      {FileName=fi.Name;}
                      sb.Append(FileName); 
                      sb.Append("\""); 
                      sb.Append("\r\n"); 
                      sb.Append("Content-Type: application/octet-stream"); 
                      sb.Append("\r\n"); 
                      sb.Append("\r\n"); 
                      string strPostHeader = sb.ToString(); 
                      byte[] postHeaderBytes;
                      if (CharSet.Trim() != "")
                      { postHeaderBytes = Encoding.GetEncoding(CharSet.Trim()).GetBytes(strPostHeader); }
                      else
                      { postHeaderBytes = Encoding.ASCII.GetBytes(strPostHeader); }
                      myHttpWebRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                      myHttpWebRequest.KeepAlive = keepalive;
                      myHttpWebRequest.Timeout = TOut;
                      if (usesession)
                      { myHttpWebRequest.CookieContainer = session; }
                      if (useragent.Trim() != "")
                      { myHttpWebRequest.UserAgent = useragent; }
                      if (refer.ToString().Trim() != "")
                      { myHttpWebRequest.Referer = refer; }
                      myHttpWebRequest.Method = "POST";
                      myHttpWebRequest.AllowWriteStreamBuffering = false;
                      myHttpWebRequest.ContentType = "multipart/form-data; boundary=" + strBoundary;
                      long length = fi.Length + postHeaderBytes.Length + boundaryBytes.Length;
                      long fileLength = fi.Length;
                      myHttpWebRequest.ContentLength = length;
                      byte[] buffer = new byte[BufferSize];
                      int dataRead = 0;
                      using (FileStream fs = fi.OpenRead())
                      {
                          try
                          {
                              using (Stream rs = myHttpWebRequest.GetRequestStream())
                              {
                                  rs.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                                  do
                                  {
                                      dataRead = fs.Read(buffer, 0, BufferSize);
                                      rs.Write(buffer, 0, dataRead);
                                  }
                                  while (dataRead < BufferSize);
                              }
                          }
                          catch
                          { }
                          finally
                          { fs.Close(); }
                      }
                      myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                      iStatCode = (int)myHttpWebResponse.StatusCode;
                      if (iStatCode == 200)
                      {
                          myHttpWebResponse.Close();
                          myHttpWebRequest.Abort();
                          return true;
                      }
                      else
                      {
                          myHttpWebResponse.Close();
                          myHttpWebRequest.Abort();
                          ErrMsg = "HTTP ERR:" + iStatCode.ToString();
                          return false;
                      }
                  }
                  else
                  {
                      ErrMsg = "File Not Exists";
                      return false;
                  }
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
