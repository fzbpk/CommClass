using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace System.Communicate
{
    /// <summary>
    /// TCPIP通讯类
    /// </summary>
    public class SocketSDK : IDisposable
   {
       #region 定义事件

       /// <summary>
       /// 异步接收事件
       /// </summary>
       /// <param name="socket">Socket连接</param>
       public delegate void Socket_ASyncRecvedEven(ref Socket socket);

       #endregion

       #region 事件
       /// <summary>
       /// 异步接收事件
       /// </summary>
         public event Socket_ASyncRecvedEven ASyncRecved = null;
       #endregion

       #region 定义
       private bool m_disposed;
       Socket SocketRS = null;
       IPEndPoint IPAddr = null;
       private Socket_ASyncSend ASYWork = null;
       private Socket_ASyncRecv ASYRecv = null;
       string ErrMsg = "";
       int ConnPool = 0;
       int ConnectTOut = 1000;
       int AutoCoefficient = 0;
       int SleepTime = 1000;
       int retry = 0;
       AddressFamily Address_Family;
       SocketType Socket_Type;
       ProtocolType Protocol_Type;
       bool SendEn = false;

       #endregion

       #region 构造

       /// <summary>
       ///  TCPIP通讯类
       /// </summary>
       public SocketSDK()
       {
           Address_Family = AddressFamily.InterNetwork;
           Socket_Type = SocketType.Stream;
           Protocol_Type = ProtocolType.Tcp;
           SocketRS = new Socket(Address_Family, Socket_Type, Protocol_Type);
           PingCheck = false;
           ReSendTime = 0;
           AsyncSendWait = 0;
           PingTimes = 4;
       }

       /// <summary>
       ///  TCPIP通讯类
       /// </summary>
       /// <param name="address_family">地址类型</param>
       /// <param name="socket_type">SOCKET类型</param>
       /// <param name="protocol_type">协议类型</param>
       public SocketSDK(AddressFamily address_family, SocketType socket_type, ProtocolType protocol_type)
       {
           Address_Family = address_family;
           Socket_Type = socket_type;
           Protocol_Type = protocol_type;
           SocketRS = new Socket(Address_Family, Socket_Type, Protocol_Type);
           PingCheck = false;
           ReSendTime = 0;
           AsyncSendWait = 0;
           PingTimes = 4;
       }

        /// <summary>
        /// 释放资源
        /// </summary>
       ~SocketSDK()
      {
        Dispose(false);
      }

        /// <summary>
       /// 释放资源
        /// </summary>
        public void Dispose()
       { 
         Dispose(true);
         GC.SuppressFinalize(this);
       }

        /// <summary>
        /// 释放连接
        /// </summary>
        /// <param name="disposing">是否释放</param>
       protected virtual void Dispose(bool disposing)
       {
           lock (this)
           {
               if (disposing && !m_disposed  )
              {
                  if (ASYWork != null)
                  {
                      ASYWork.SendEnA = false;
                      ASYWork.SendEnB = false;
                      Thread.Sleep(1000);
                      ASYWork = null;
                  }
                  if (ASYRecv != null)
                  {
                      ASYRecv.Listen = false;
                      Socket socks = new Socket(Address_Family, Socket_Type, Protocol_Type);
                      try
                      { socks.Connect(IPAddr); }
                      catch
                      {
                          try
                          { socks.Connect(IPAddress.Parse("127.0.0.1"), IPAddr.Port); }
                          catch { }
                      }
                      socks.Close();
                      Thread.Sleep(1000);
                      ASYRecv = null;
                  }
                  if (SocketRS != null)
                  {
                      try
                      {
                          SocketRS.Shutdown(SocketShutdown.Both);
                          System.Threading.Thread.Sleep(100);
                          SocketRS.Close();
                      }
                      catch { }
                  }
                  m_disposed = true; 
               }         
           }
       }


       #endregion

       #region 属性

       /// <summary>
       /// 是否启用连接前PING
       /// </summary>
       public bool PingCheck { get; set; }

       /// <summary>
       /// PING次数
       /// </summary>
       public int PingTimes { get; set; }

       /// <summary>
       /// 重发时间
       /// </summary>
       public int ReSendTime { get; set; }

       /// <summary>
       /// 异步发送等待时间
       /// </summary>
       public int AsyncSendWait { get; set; }

       /// <summary>
       /// 转换Socket
       /// </summary>
       public Socket Socket
       {
           get
           { return SocketRS; }
           set
           {
               if (value != null)
               {
                   if (SocketRS != null)
                   {
                       if (SocketRS.IsBound || SocketRS.Connected)
                       { SocketRS.Close(); }
                       SocketRS = null;
                   }
                   SocketRS = value;
                   Address_Family = SocketRS.AddressFamily;
                   Socket_Type = SocketRS.SocketType;
                   Protocol_Type = SocketRS.ProtocolType;
                   IPAddr = (IPEndPoint)SocketRS.LocalEndPoint;
                   PingCheck = false;
                   ReSendTime = 0;
                   AsyncSendWait = 0;
                   PingTimes = 4;
               }

           }
       }

       /// <summary>
       /// 错误信息，读一次后自动清除
       /// </summary>
       public string Error
       {
           get
           {
               string errmmsg = ErrMsg;
               ErrMsg = "";
               return errmmsg;
           }
       }

       /// <summary>
       /// 重试次数
       /// </summary>
       public int ReTry
       {
           get
           { return retry; }
           set
           { retry = value; }
       }

       /// <summary>
       /// 目标IP
       /// </summary>
       public string IP
       {
           get
           {
               try
               {
                   if (IPAddr == null)
                   { return "127.0.0.1"; }
                   else
                   { return IPAddr.Address.ToString(); }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return "127.0.0.1";
               }
           }
       }

       /// <summary>
       /// 目标端口
       /// </summary>
       public int Port
       {
           get
           {
               try
               {
                   if (IPAddr == null)
                   { return 0; }
                   else
                   { return IPAddr.Port; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
       }

       /// <summary>
       /// 异步接收是否启用
       /// </summary>
       public bool ASyncRecvState
       {
           get
           {
               try
               {
                   if (ASYRecv == null)
                       return false;
                   return ASYRecv.Listen;
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
       }

       /// <summary>
       /// 异步发送队列剩余数
       /// </summary>
       public int ASyncListCount
       {
           get
           {
               try
               {
                   if (ASYWork == null)
                       return -1;
                   int res = 0;
                   if (ASYWork.SendLsA != null)
                       res += ASYWork.SendLsA.Count;
                   if (ASYWork.SendLsB != null)
                       res += ASYWork.SendLsB.Count;
                   return res;
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return -2;
               }
           }
       }

       /// <summary>
       /// 异步发送状态
       /// </summary>
       public bool ASyncState
       {
           get
           {
               try
               {
                   if (ASYWork == null)
                       return false;
                   return ASYWork.SendEnA || ASYWork.SendEnB;
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
       }

       /// <summary>
       /// 异步通信错误
       /// </summary>
       public string ASyncErrMessage
       {
           get
           {
               try
               {
                   if (ASYWork == null)
                       return "未开启异步连接";
                   return ASYWork.ErrMessage;
               }
               catch (Exception ex)
               {
                   return ex.Message;
               }
           }
       }

       /// <summary>
       /// 是否已连接
       /// </summary>
       public bool Connected
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return false; }
                   else
                   { return SocketRS.Connected; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
       }

       /// <summary>
       /// 是否已绑定
       /// </summary>
       public bool IsBound
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return false; }
                   else
                   { return SocketRS.IsBound; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
       }

       /// <summary>
       /// 获取可接收字节数
       /// </summary>
       public int Available
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return -1; }
                   else
                   { return SocketRS.Available; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return -2;
               }
           }
       }

        /// <summary>
        /// 获取或设置阻塞状态
        /// </summary>
       public bool Blocking
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return false; }
                   else
                   { return SocketRS.Blocking; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.Blocking = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

       /// <summary>
       /// 获取或设置数据是否分包
       /// </summary>
       public bool DontFragment
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return false; }
                   else
                   { return SocketRS.DontFragment; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.DontFragment = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置发送超时，ms
        /// </summary>
       public int SendTimeout
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return SocketRS.SendTimeout; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.SendTimeout = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
        /// 获取或设置发送缓存区，B
        /// </summary>
       public int SendBufferSize
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return SocketRS.SendBufferSize; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.SendBufferSize = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置接收超时
        /// </summary>
       public int ReceiveTimeout
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return SocketRS.ReceiveTimeout; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.ReceiveTimeout = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置接收缓存区，B
        /// </summary>
       public int ReceiveBufferSize
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return SocketRS.ReceiveBufferSize; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.ReceiveBufferSize = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置最大连接数
        /// </summary>
       public int MaxConnect
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return ConnPool; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { ConnPool = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置连接超时
        /// </summary>
       public int ConnectTimeOut
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return ConnectTOut; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { ConnectTOut = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置等待超时
        /// </summary>
       public int WaitTime
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return 0; }
                   else
                   { return SleepTime; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return 0;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SleepTime = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
       /// 获取或设置是否允许多播
        /// </summary>
       public bool ExclusiveAddressUse
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return false; }
                   else
                   { return SocketRS.ExclusiveAddressUse; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SocketRS.ExclusiveAddressUse = value; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
       }

        /// <summary>
        /// 获取是否可以发送
        /// </summary>
       public bool SendEnable
       {
           get
           {
               try
               {
                   if (SocketRS == null)
                   { return false; }
                   else
                   { return SendEn; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
           set
           {
               try
               {
                   if (SocketRS != null)
                   { SendEn = value; }
               }
               catch (Exception ex)
               { ErrMsg = ex.Message; }
           }
       }

        /// <summary>
       /// 获取或设置启用保持连接
        /// </summary>
       public bool KeepAlive
       {
           set
           {
               try
               {
                   if (SocketRS != null)
                   {
                       if (value)
                       {
                           byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };
                           SocketRS.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, true);
                           SocketRS.IOControl(IOControlCode.KeepAliveValues, BitConverter.GetBytes(1), null);
                       }
                       else
                       {
                           byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };
                           SocketRS.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, false);
                       }
                   }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
               }
           }
           get
           {
               try
               {
                   if (SocketRS != null)
                   {
                       return (bool)SocketRS.GetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive);
                   }
                   else
                   { return false; }
               }
               catch (Exception ex)
               {
                   ErrMsg = ex.Message;
                   return false;
               }
           }
       }

      /// <summary>
      /// 获取本机所有IP地址
      /// </summary>
      /// <returns></returns>
       public List<string> LocalIPAddress()
       {
           List<string> res=new List<string>();
           try
           {
               string strHostName = Dns.GetHostName(); //得到本机的主机名 
               IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP 
               for (int i = 0; i < ipEntry.AddressList.Length; i++)
                   res.Add(ipEntry.AddressList[i].ToString());
           }
           catch (Exception ex)
           { ErrMsg = ex.Message;  }
           return res;
       }

      /// <summary>
      /// 获取远端连接
      /// </summary>
       public IPEndPoint RemoteEndPoint
       {
           get
           {
               try
               {
                   if (SocketRS != null)
                       return (IPEndPoint)SocketRS.RemoteEndPoint;
               }
               catch (Exception ex)
               {  ErrMsg = ex.Message;  }
               return null;
           }
       }

       #endregion

       #region 高级设置

       public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
       {
           try
           {
               if (SocketRS != null)
               {
                   SocketRS.SetSocketOption(optionLevel, optionName, optionValue);
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

       public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
       {
           try
           {
               if (SocketRS != null)
               {
                   SocketRS.SetSocketOption(optionLevel, optionName, optionValue);
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

       public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
       {
           try
           {
               if (SocketRS != null)
               {
                   SocketRS.SetSocketOption(optionLevel, optionName, optionValue);
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

       public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
       {
           try
           {
               if (SocketRS != null)
               {
                   SocketRS.SetSocketOption(optionLevel, optionName, optionValue);
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

       public void IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
       {
           try
           {
               if (SocketRS != null)
               {
                   SocketRS.IOControl(ioControlCode, optionInValue, optionOutValue);
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

       public long Ping(int PackSize=0)
       {
           try
           {
               if (IPAddr == null)
               { return -1; }
               else
               {
                   if (PackSize <= 0)
                   {
                       System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                       System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                       return Res.RoundtripTime;
                   }
                   else
                   {
                       byte[] buf = new byte[(int)PackSize];
                       for (int i = 0; i < buf.Length; i++)
                       { buf[i] = 0; }
                       System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                       System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut, buf);
                       return Res.RoundtripTime;
                   }
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return -2;
           }
       }

       public bool AutoSet()
       {

           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   if (!SocketRS.Connected && !SocketRS.IsBound)
                   {
                       string IP = "";
                       if (IPAddr.Address == IPAddress.Any)
                       { IP = "127.0.0.1"; }
                       else
                       { IP = IPAddr.Address.ToString(); }
                       int BitSize = 0;
                       int[] Pack = new int[] { 4096, 2048, 1024, 512, 256, 128, 64, 32 };
                       for (int i = 0; i < Pack.Length; i++)
                       {
                           byte[] buf = new byte[Pack[i]];
                           System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                           System.Net.NetworkInformation.PingReply Res = ping.Send(IP, 1000, buf);
                           if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                           {
                               if (Res.RoundtripTime < 10)
                               {
                                   if (i == Pack.Length - 1)
                                   { BitSize = Pack[i]; }
                                   else
                                   { BitSize = Pack[i + 1]; }
                                   break;
                               }
                           }
                       }
                       if (BitSize == 0)
                       {
                           for (int i = 0; i < Pack.Length; i++)
                           {
                               byte[] buf = new byte[Pack[i]];
                               System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                               System.Net.NetworkInformation.PingReply Res = ping.Send(IP, 1000, buf);
                               if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                               {
                                   if (Res.RoundtripTime < 100)
                                   {
                                       if (i == Pack.Length - 1)
                                       { BitSize = Pack[i]; }
                                       else
                                       { BitSize = Pack[i + 1]; }
                                       break;
                                   }
                               }
                           }
                       }
                       if (BitSize == 0)
                       {
                           for (int i = 0; i < Pack.Length; i++)
                           {
                               byte[] buf = new byte[Pack[i]];
                               System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                               System.Net.NetworkInformation.PingReply Res = ping.Send(IP, 1000, buf);
                               if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                               {
                                   if (Res.RoundtripTime < 1000)
                                   {
                                       if (i == Pack.Length - 1)
                                       { BitSize = Pack[i]; }
                                       else
                                       { BitSize = Pack[i + 1]; }
                                       break;
                                   }
                               }
                           }
                       }
                       switch (BitSize)
                       {
                           case 4096:
                               SocketRS.SendBufferSize = 16 * 1024;
                               SocketRS.SendTimeout = 1000;
                               SocketRS.ReceiveBufferSize = 16 * 1024;
                               SocketRS.ReceiveTimeout = 1000;
                               ConnPool = 200;
                               AutoCoefficient = 1;
                               ConnectTOut = 300;
                               return true;
                           case 2048:
                               SocketRS.SendBufferSize = 8 * 1024;
                               SocketRS.SendTimeout = 1500;
                               SocketRS.ReceiveBufferSize = 8 * 1024;
                               SocketRS.ReceiveTimeout = 1500;
                               ConnPool = 150;
                               AutoCoefficient = 1;
                               ConnectTOut = 500;
                               return true;
                           case 1024:
                               SocketRS.SendBufferSize = 2 * 1024;
                               SocketRS.SendTimeout = 2000;
                               SocketRS.ReceiveBufferSize = 2 * 1024;
                               SocketRS.ReceiveTimeout = 2000;
                               ConnPool = 100;
                               AutoCoefficient = 1;
                               ConnectTOut = 600;
                               return true;
                           case 512:
                               SocketRS.SendBufferSize = 1024;
                               SocketRS.SendTimeout = 2500;
                               SocketRS.ReceiveBufferSize = 1024;
                               SocketRS.ReceiveTimeout = 2500;
                               ConnPool = 10;
                               AutoCoefficient = 2;
                               ConnectTOut = 800;
                               return true;
                           case 256:
                               SocketRS.SendBufferSize = 512;
                               SocketRS.SendTimeout = 3000;
                               SocketRS.ReceiveBufferSize = 512;
                               SocketRS.ReceiveTimeout = 3000;
                               ConnPool = 100;
                               AutoCoefficient = 2;
                               ConnectTOut = 800;
                               return true;
                           case 128:
                               SocketRS.SendBufferSize = 256;
                               SocketRS.SendTimeout = 3500;
                               SocketRS.ReceiveBufferSize = 512;
                               SocketRS.ReceiveTimeout = 3500;
                               ConnPool = 150;
                               AutoCoefficient = 3;
                               ConnectTOut = 1000;
                               return true;
                           case 64:
                               SocketRS.SendBufferSize = 128;
                               SocketRS.SendTimeout = 4000;
                               SocketRS.ReceiveBufferSize = 512;
                               SocketRS.ReceiveTimeout = 4000;
                               ConnPool = 150;
                               AutoCoefficient = 5;
                               ConnectTOut = 2000;
                               return true;
                           case 32:
                               SocketRS.SendBufferSize = 100;
                               SocketRS.SendTimeout = 5000;
                               SocketRS.ReceiveBufferSize = 512;
                               SocketRS.ReceiveTimeout = 0;
                               ConnPool = 150;
                               AutoCoefficient = 6;
                               ConnectTOut = 5000;
                               return true;
                           default:
                               AutoCoefficient = 0;
                               return false;
                       }
                   }
                   else
                   { return false; }
               }
               else
               { return false; }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return false;
           }
       }


       #endregion

       #region 基本方法

       /// <summary>
       /// 设置IP和端口
       /// </summary>
       /// <param name="IP">IP，空为0.0.0.0</param>
       /// <param name="Port">端口</param>
       public void SetIPAddress(string IP, int Port)
       {
           try
           {
               IP = (IP == null ? "" : IP.Trim());
               if (IP.Trim() == "")
                   IPAddr = new IPEndPoint(IPAddress.Any, Port);
               else
                   IPAddr = new IPEndPoint(IPAddress.Parse(IP), Port);
           }
           catch
           { IPAddr = new IPEndPoint(IPAddress.Any, Port); }
       }

      /// <summary>
      /// 打开远程连接
      /// </summary>
       public void Open()
       {
           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   if (!SocketRS.Connected)
                   {
                       ASYWork = null;
                       ASYRecv = null;
                       if (PingCheck)
                       {
                           for (int counts = 0; counts < PingTimes; counts++)
                           {
                               System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                               System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                               if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                               { SocketRS.Connect(IPAddr); break; }
                           }
                       }
                       else
                       { SocketRS.Connect(IPAddr); }
                   }
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

       /// <summary>
       /// 重连接
       /// </summary>
       /// <param name="Auto">是否开启自动配置</param>
       public void ReOpen(bool Auto = false)
       {
           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   SocketRS.Close();
                   ASYRecv = null;
                   ASYWork = null;
                   SocketRS = new Socket(Address_Family, Socket_Type, Protocol_Type);
                   if (Auto)
                   { AutoSet(); }
                   if (PingCheck)
                   {
                       for (int counts = 0; counts < PingTimes; counts++)
                       {
                           System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                           System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                           if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                           { SocketRS.Connect(IPAddr); break; }
                       }
                   }
                   else
                   { SocketRS.Connect(IPAddr); }
               }
           }
           catch (Exception ex)
           { ErrMsg = ex.Message; }
       }

        /// <summary>
        /// 打开目标连接
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="Port">端口</param>
       public void Open(string IP, int Port)
       {
           try
           {
               if (SocketRS != null)
               {
                   if (!SocketRS.Connected)
                   {
                       IPAddr = new IPEndPoint(IPAddress.Parse(IP), Port);
                       if (PingCheck)
                       {
                           for (int counts = 0; counts < PingTimes; counts++)
                           {
                               System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                               System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                               if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                               { SocketRS.Connect(IPAddr); break; }
                           }
                       }
                       else
                       { SocketRS.Connect(IPAddr); }
                   }
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
        /// 绑定并监听端口
        /// </summary>
       public void Bind()
       {
           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   if (!SocketRS.IsBound)
                   {
                       SocketRS.Bind(IPAddr);
                       SocketRS.Listen(ConnPool);
                   }
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
       /// 绑定并监听端口
        /// </summary>
       /// <param name="Auto">是否开启自动配置</param>
       public void ReBind(bool Auto = false)
       {
           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   SocketRS.Close();
                   ASYRecv = null;
                   ASYWork = null;
                   SocketRS = new Socket(Address_Family, Socket_Type, Protocol_Type);
                   if (Auto)
                   { AutoSet(); }
                   SocketRS.Bind(IPAddr);
                   SocketRS.Listen(ConnPool);
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
       /// 绑定并监听端口
        /// </summary>
        /// <param name="IP">IP</param>
       /// <param name="Port">端口</param>
       public void Bind(string IP, int Port)
       {
           if (IP == null)
           { IP = ""; }
           try
           {
               if (SocketRS != null)
               {
                   if (!SocketRS.IsBound)
                   {
                       if (IP.ToString().Trim() == "")
                       {
                           IPAddr = new IPEndPoint(IPAddress.Any, Port);
                           SocketRS.Bind(IPAddr);
                           SocketRS.Listen(ConnPool);
                       }
                       else
                       {
                           IPAddr = new IPEndPoint(IPAddress.Parse(IP), Port);
                           SocketRS.Bind(IPAddr);
                           SocketRS.Listen(ConnPool);
                       }
                   }
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
        /// 关闭连接
        /// </summary>
       public void Close()
       {
           try
           {
               if (SocketRS != null)
               {
                   try
                   {
                       SocketRS.Shutdown(SocketShutdown.Both);
                   }
                   catch { }
                   System.Threading.Thread.Sleep(100);
                   SocketRS.Close();
                   ASYRecv = null;
                   ASYWork = null;
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
        /// 接收到新连接
        /// </summary>
       /// <returns>Socket</returns>
       public Socket AcceptAs()
       {

           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   if (SocketRS.IsBound)
                   {
                       return SocketRS.Accept();
                   }
                   else
                   { return null; }
               }
               else
               { return null; }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return null;
           }
       }

       /// <summary>
       /// 接收到新连接
       /// </summary>
       /// <returns>SocketSDK</returns>
       public SocketSDK Accept()
       {

           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   if (SocketRS.IsBound)
                   {
                       SocketSDK ss= new SocketSDK();
                       ss.Socket = SocketRS.Accept();
                       return ss;
                   }
                   else
                   { return null; }
               }
               else
               { return null; }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return null;
           }
       }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Data">数据</param>
        /// <returns>发送量</returns>
       public int SendBytes(byte[] Data)
       {
           try
           {
               if (SocketRS != null && IPAddr != null)
               {
                   bool senden = true;
                   if (PingCheck)
                   {
                       senden = false;
                       for (int counts = 0; counts < PingTimes; counts++)
                       {
                           System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                           System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                           if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                           { senden = true; break; }
                       }
                   }
                   if (senden)
                   {
                       int len = SocketRS.Send(Data);
                       return ( len);
                   }
                   else
                       return -1;
               }
               else
               { return -2; }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return -3;
           }
       }
       
        /// <summary>
        /// 发送并接收数据
        /// </summary>
       /// <param name="Data">发送数据</param>
        /// <returns>接收数据</returns>
       public byte[] SendBytesReply(byte[] Data, bool legacy = false)
       {
           int len = SendBytes(Data);
           if (len <= 0)
               return null;
           if (AutoCoefficient == 0)
           { System.Threading.Thread.Sleep(SleepTime); }
           else
           {
               int Delay = 1;
               if (PingCheck)
               {
                   for (int counts = 0; counts < PingTimes; counts++)
                   {
                       System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                       System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                       if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                       {Delay = Res.RoundtripTime == 0 ? 1 : (int)Res.RoundtripTime; break; }
                   }
               }
               System.Threading.Thread.Sleep(Delay * AutoCoefficient * 100);
           }
           len = Receive(out Data,legacy);
           if (len > 0)
               return Data;
           else
               return null;
       }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer">接收数据</param>
       /// <param name="legacy">传统接收</param>
        /// <returns>数据量</returns>
       public int Receive(out byte[] buffer, bool legacy=false)
       {
           try
           {
               if (SocketRS != null)
               {
                   int len = 0;
                   byte[] buf = new byte[SocketRS.ReceiveBufferSize];
                   if (legacy)
                   {
                       len = SocketRS.Receive(buf);
                       buffer = new byte[len];
                       Array.Copy(buf, buffer, len);
                       return len;
                   }
                   else 
                   {
                       List<byte> Recv = new List<byte>();
                       int Connecttt = SocketRS.ReceiveTimeout;
                       if (Connecttt == 0)
                       {
                           try
                           { len = SocketRS.Receive(buf); }
                           catch { len = 0; }
                           if (len > 0)
                           {
                               buffer = new byte[len];
                               Array.Copy(buf, buffer, len);
                               return len;
                           }
                           else
                           {
                               buffer = null;
                               return 0;
                           }
                       }
                       else
                       {
                           SocketRS.ReceiveTimeout = 100;
                           len = 0;
                           if (len <= 0)
                           {
                               for (int i = 0; i <= (Connecttt / SocketRS.ReceiveTimeout); i++)
                               {
                                   try
                                   { len = SocketRS.Receive(buf); }
                                   catch(Exception exx)
                                   { 
                                       ErrMsg = exx.Message;
                                       len = 0;
                                   }
                                   if (len > 0)
                                       break;
                               }
                           }
                           while (len > 0)
                           {
                               for (int i = 0; i < len; i++) Recv.Add(buf[i]);
                               buf = new byte[SocketRS.ReceiveBufferSize];
                               try
                               { len = SocketRS.Receive(buf); }
                               catch { len = 0; }
                           }
                           SocketRS.ReceiveTimeout = Connecttt;
                           if (Recv.Count > 0)
                           {
                               ErrMsg = "";
                               buffer = new byte[Recv.Count];
                               Recv.CopyTo(buffer);
                               return Recv.Count;
                           }
                           else
                           {
                               buffer = null;
                               return 0;
                           }
                       }
                   }
               }
               else
               {
                   buffer = null;
                   return -2;
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               buffer = null;
               return -3;
           }
       }

       /// <summary>
       /// 发送字符串
       /// </summary>
       /// <param name="Data">字符串</param>
       /// <param name="NewLine">是否回车换行</param>
       /// <param name="Encode">文字编码</param>
       /// <returns>发送数量</returns>
       public int SendString(string Data, bool NewLine = true, string Encode = "")
       {
           try
           {
               if (Data == null)
                   return -1;
               if (!Data.Contains("\r\n"))
                   Data += "\r\n";
               Encode = Encode == null ? "" : Encode.Trim();
               Encoding encode = Encoding.ASCII;
               if (Encode != "")
                   encode = Encoding.GetEncoding(Encode);
               byte[] data = encode.GetBytes(Data);
               return SendBytes(data);
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
               return -3;
           }
       }

       /// <summary>
       /// 接收字符串
       /// </summary>
       /// <returns>字符串</returns>
       public string Receive( string Encode = "",bool legacy=false)
       {
           byte[] data=null;
           int len = Receive(out data, legacy);
           if (len <= 0)
               return "";
           Encode = Encode == null ? "" : Encode.Trim();
           Encoding encode = Encoding.ASCII;
           if (Encode != "")
               encode = Encoding.GetEncoding(Encode);
           return encode.GetString(data);
       }


       #endregion 
 
       #region 异步方法

       /// <summary>
       /// 异步发送
       /// </summary>
       /// <param name="Data">发送数据</param>
       public void ASyncSendBytes(byte[] Data)
       {
           try
           {
               if (ASYWork == null && SocketRS.Connected)
               {
                   ASYWork = new Socket_ASyncSend();
                   ASYWork.SendEnA = true;
                   ASYWork.SendEnB = false;
                   ASYWork.ReSendTime = ReSendTime;
                   ASYWork.SocketRS = SocketRS;
                   ASYWork.AsyncSendWait = AsyncSendWait;
                   ASYWork.ConnectTOut = ConnectTOut;
                   ASYWork.PingCheck = PingCheck;
                   Thread tw = new Thread(new ThreadStart(ASYWork.main));
                   tw.Start();
               }
               if (ASYWork.SendEnA)
                   ASYWork.SendLsA.Add(Data);
               else if (ASYWork.SendEnB)
                   ASYWork.SendLsB.Add(Data);
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
        /// 异步发送停止
        /// </summary>
       public void ASyncSendStop()
       {
           try
           {
               if (ASYWork != null)
               {
                   ASYWork.SendEnA = false;
                   ASYWork.SendEnB = false;
                   Thread.Sleep(1000);
                   ASYWork = null;
               }
           }
           catch (Exception ex)
           {
               ErrMsg = ex.Message;
           }
       }

        /// <summary>
        /// 异步接收开启
        /// </summary>
       public void ASyncRecvStart()
       {

           try
           {
               if (SocketRS != null && IPAddr != null && ASYRecv == null)
               {
                   if (!SocketRS.IsBound)
                   {
                       SocketRS.Bind(IPAddr);
                       SocketRS.Listen(ConnPool);
                       ASYRecv = new  Socket_ASyncRecv();
                       ASYRecv.SocketRS = SocketRS;
                       ASYRecv.Listen = true;
                       if (ASyncRecved != null)
                           ASYRecv.ASyncRecved = ASyncRecved;
                       Thread tw = new Thread(new ThreadStart(ASYRecv.ListenIng));
                       tw.Start();
                   }
               }
           }
           catch (Exception ex)
           { ErrMsg = ex.Message; }
       }

        /// <summary>
       /// 异步接收停止
        /// </summary>
       public void ASyncRecvStop()
       {

           try
           {
               if (ASYRecv != null)
               {
                   ASYRecv.Listen = false;

                   Socket socks = new Socket(Address_Family, Socket_Type, Protocol_Type);
                   try
                   { socks.Connect(IPAddr); }
                   catch
                   {
                       try
                       {
                           socks.Connect(IPAddress.Parse("127.0.0.1"), IPAddr.Port);
                       }
                       catch { }
                   }

                   socks.Close();
                   Thread.Sleep(1000);
                   ASYRecv = null;
               }
           }
           catch (Exception ex)
           { ErrMsg = ex.Message; }
       }

       #endregion

    }

    /// <summary>
    /// 异步处理发送类
    /// </summary>
    internal class Socket_ASyncSend
    {
        public Socket SocketRS { get; set; }
        public int ReSendTime { get; set; }
        public string ErrMessage { get; private set; }
        public bool SendEnA { get; set; }
        public bool SendEnB { get; set; }
        public List<byte[]> SendLsA { get; set; }
        public List<byte[]> SendLsB { get; set; }
        public int AsyncSendWait { get; set; }
        public bool PingCheck { get; set; }
        public int ConnectTOut { get; set; }
        public Socket_ASyncSend()
        {
            SendEnA = SendEnB = false;
            SendLsA = new List<byte[]>();
            SendLsB = new List<byte[]>();
            ErrMessage = "";
            ReSendTime = 0;
            ConnectTOut = 1000;
            PingCheck = false;
        }
        public void main()
        {
            if (SocketRS == null)
                return;
            if (SendLsA == null || SendLsB == null)
                return;
            if (!SocketRS.Connected)
                return;
            try
            {
                while (SendEnA || SendEnB)
                {
                    bool senden = true;
                    if (PingCheck)
                    {
                        System.Net.IPEndPoint ips = (System.Net.IPEndPoint)SocketRS.RemoteEndPoint;
                        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                        System.Net.NetworkInformation.PingReply Res = ping.Send(ips.Address.ToString(), ConnectTOut);
                        if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                            senden = true;
                        else
                            senden = false;
                    }
                    if (!senden)
                        continue;

                    SendEnA = false;
                    SendEnB = true;
                    if (SendLsA.Count > 0)
                    {
                        foreach (byte[] Data in SendLsA)
                        {
                            SocketRS.Send(Data);
                            if (AsyncSendWait > 0)
                                System.Threading.Thread.Sleep(AsyncSendWait);
                        }
                    }
                    if (SendLsA.Count > 0)
                        SendLsA.Clear();
                    Thread.Sleep(100);

                    SendEnA = true;
                    SendEnB = false;
                    if (SendLsB.Count > 0)
                    {
                        foreach (byte[] Data in SendLsB)
                        {
                            SocketRS.Send(Data);
                            if (AsyncSendWait > 0)
                                System.Threading.Thread.Sleep(AsyncSendWait);
                        }
                    }
                    if (SendLsB.Count > 0)
                        SendLsB.Clear();
                    Thread.Sleep(100);

                    if (!SocketRS.Connected)
                    {
                        SendEnA = false;
                        SendEnB = false;
                        if (SendLsA.Count > 0)
                            SendLsA.Clear();
                        if (SendLsB.Count > 0)
                            SendLsB.Clear();
                    }
                }

            }
            catch (Exception ex)
            {
                ErrMessage = ex.Message;
                SendEnA = false;
                SendEnB = false;
                if (SendLsA.Count > 0)
                    SendLsA.Clear();
                if (SendLsB.Count > 0)
                    SendLsB.Clear();
            }
        }
    }

    /// <summary>
    /// 异常处理接收类
    /// </summary>
    internal class Socket_ASyncRecv
    {
        public Socket SocketRS { get; set; }
        public bool Listen { get; set; }
        public string Errmessage { get; private set; }
        public SocketSDK.Socket_ASyncRecvedEven ASyncRecved = null;

        private struct ListenDeal
        {
            public SocketSDK.Socket_ASyncRecvedEven ASyncRecved;
            public Socket sock;
            public void main()
            {
                if (sock == null)
                    return;
                if (ASyncRecved != null)
                    ASyncRecved(ref sock);

            }

        }

        public void ListenIng()
        {
            try
            {
                while (Listen)
                {
                    ListenDeal dl = new ListenDeal();
                    dl.ASyncRecved = ASyncRecved;
                    dl.sock = SocketRS.Accept();
                    Thread tw = new Thread(new ThreadStart(dl.main));
                    tw.Start();
                }
                SocketRS.Close();
                SocketRS.Dispose();
            }
            catch (Exception ex)
            { Errmessage = ex.Message; }
        }
    }

}
