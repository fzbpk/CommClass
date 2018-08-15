using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Win32CTC
{
    public class SocketSDK
    {

        #region 定义
        public delegate void ASyncRecvedEven(Socket socket, string IP, int Port);
        public event ASyncRecvedEven ASyncRecved = null;
        Socket SocketRS = null;
        IPEndPoint IPAddr = null;
        private ASyncSend ASYWork = null;
        private ASyncRecv ASYRecv = null;
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

        public enum PackSize : int
        {
            bit_Zero = 0,
            bit_2 = 2,
            bit_4 = 4,
            bit_8 = 8,
            bit_16 = 16,
            bit_32 = 32,
            bit_64 = 64,
            bit_128 = 128,
            bit_256 = 256,
            bit_512 = 512,
            bit_1K = 1024,
            bit_2K = 2048,
            bit_4K = 4096,
            bit_8K = 8192,
            bit_16K = 16384,
            bit_32K = 32768,
            bit_64K = 65536,
        };

        #endregion

        #region 构造

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

        public SocketSDK(Socket Socket)
        {
            SocketRS = Socket;
            if (SocketRS != null)
            {
                Address_Family = SocketRS.AddressFamily;
                Socket_Type = SocketRS.SocketType;
                Protocol_Type = SocketRS.ProtocolType;
                IPAddr = (IPEndPoint)SocketRS.LocalEndPoint;
            }
           
            PingCheck = false;
            ReSendTime = 0;
            AsyncSendWait = 0;
            PingTimes = 4;
        }

        #endregion

        #region 属性

        public bool PingCheck { get; set; }

        public int PingTimes { get; set; }

        public int ReSendTime { get; set; }

        public int AsyncSendWait { get; set; }

        public Socket Socket
        {
            get
            { return SocketRS; }
            set
            {
                if (value != null)
                {
                    if (SocketRS == null)
                    { SocketRS = value; }
                    else
                    {
                        if (SocketRS.IsBound || SocketRS.Connected)
                        { SocketRS.Close(); }
                        SocketRS = null;
                        SocketRS = value;
                    }
                }
              
            }
        }

        public void SetIPAddress(string IP, int Port)
        {
            try
            {
                IP = (IP == null ? "" : IP.Trim());
                if (IP.Trim ()=="")
                    IPAddr = new IPEndPoint(IPAddress.Any, Port);
                else
                    IPAddr = new IPEndPoint(IPAddress.Parse(IP), Port);
            }
            catch
            { IPAddr = new IPEndPoint(IPAddress.Any, Port); }

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

        public int ReTry
        {
            get
            { return retry; }
            set
            { retry = value; }
        }
 
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

        public string ASyncErrMessage
        {
            get
            {
                try
                {
                    return ASYWork.ErrMessage;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

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

        public long Ping()
        {
            try
            {
                if (IPAddr == null)
                { return -2; }
                else
                {
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                    if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                    { return Res.RoundtripTime; }
                    else
                    { return -1; }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return -3;
            }
        }

        public long Ping(PackSize PackSize)
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

        public string[] LocalIPAddress()
        {

            try
            {
                string strHostName = Dns.GetHostName(); //得到本机的主机名 
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP 
                string[] res = new string[ipEntry.AddressList.Length];
                for (int i = 0; i < ipEntry.AddressList.Length; i++)
                {
                    res[i] = ipEntry.AddressList[i].ToString();
                }
                return res;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return null;
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

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                try
                {
                    if (SocketRS != null)
                    {
                        return (IPEndPoint)SocketRS.RemoteEndPoint;
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

        #endregion

        #region 基本方法

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

        public void ReOpen(bool Auto=false)
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

        public SocketSDK Accept()
        {

            try
            {
                if (SocketRS != null && IPAddr != null)
                {
                    if (SocketRS.IsBound)
                    { return new SocketSDK ( SocketRS.Accept()); }
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
                    if (!SocketRS.Connected)
                    {
                        if (senden)
                            SocketRS.Connect(IPAddr);
                        else
                            return -1;
                    }
                    if (senden)
                    {
                        int len = SocketRS.Send(Data);
                        int relen = 0;
                        if (ReSendTime > 0)
                        {
                            System.Threading.Thread.Sleep(ReSendTime);
                            relen = SocketRS.Send(Data);
                        }
                        return (relen > 0 ? relen : len);
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

        public int SendBytes(byte[] Data, string IP, int Port)
        {
            try
            {
                if (SocketRS != null)
                {
                    IPAddr = new IPEndPoint(IPAddress.Parse(IP), Port);
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
                    if (!SocketRS.Connected)
                    {
                        if (senden)
                            SocketRS.Connect(IPAddr);
                        else
                            return -1;
                    }
                    else
                        return -2;
                    if (senden)
                    {
                        int len = SocketRS.Send(Data);
                        int relen = 0;
                        if (ReSendTime > 0)
                        {
                            System.Threading.Thread.Sleep(ReSendTime);
                            relen = SocketRS.Send(Data);
                        }
                        return (relen > 0 ? relen : len);
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

        public byte[] SendBytesReply(byte[] Data)
        {
            try
            {
                if (SocketRS != null && IPAddr != null)
                {
                    bool senden = true;
                    int Delay = 1;
                    if (PingCheck)
                    {
                        senden = false;
                        for (int counts = 0; counts < PingTimes; counts++)
                        {
                            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                            System.Net.NetworkInformation.PingReply Res = ping.Send(IPAddr.Address.ToString(), ConnectTOut);
                            if (Res.Status == System.Net.NetworkInformation.IPStatus.Success)
                            { senden = true; Delay = Res.RoundtripTime == 0 ? 1 : (int)Res.RoundtripTime; break; }
                        }
                    }
                    if (!SocketRS.Connected)
                    {
                        if (senden)
                            SocketRS.Connect(IPAddr);
                        else
                            return null;
                    }
                    if (senden)
                    {
                        byte[] buf = new byte[SocketRS.ReceiveBufferSize];
                        SocketRS.Send(Data);
                        if (ReSendTime > 0)
                        {
                            System.Threading.Thread.Sleep(ReSendTime * 1000);
                            SocketRS.Send(Data);
                        }
                        if (AutoCoefficient == 0)
                        { System.Threading.Thread.Sleep(SleepTime); }
                        else
                        { System.Threading.Thread.Sleep(Delay * AutoCoefficient * 100); }
                        int len = SocketRS.Receive(buf);
                        if (len > 0)
                        {
                            byte[] res = new byte[len];
                            Array.Copy(buf, 0, res, 0, len);
                            return res;
                        }
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

        public int Receive(out byte[] buffer)
        {
            try
            {
                if (SocketRS != null)
                {
                    byte[] buf = new byte[SocketRS.ReceiveBufferSize];
                    int len = SocketRS.Receive(buf);
                    buffer = new byte[len];
                    Array.Copy(buf, buffer, len);
                    return len;
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

        #endregion

        #region 异步方法

        public void ASyncSendBytes(byte[] Data)
        {
            try
            {
                if (ASYWork == null && SocketRS.Connected)
                {
                    ASYWork = new ASyncSend();
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

        public void ASyncStopSend()
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
                        ASYRecv = new ASyncRecv();
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
    internal class ASyncSend
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
        public ASyncSend()
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
                            if (ReSendTime > 0)
                            {
                                System.Threading.Thread.Sleep(ReSendTime);
                                SocketRS.Send(Data);
                            }
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
                            if (ReSendTime > 0)
                            {
                                System.Threading.Thread.Sleep(ReSendTime);
                                SocketRS.Send(Data);
                            }
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
    internal class ASyncRecv
    {
        public Socket SocketRS { get; set; }
        public bool Listen { get; set; }
        public string Errmessage { get; private set; }
        public SocketSDK.ASyncRecvedEven ASyncRecved = null;

        private struct ListenDeal
        {
            public SocketSDK.ASyncRecvedEven ASyncRecved;
            public Socket sock;
            public void main()
            {
                if (sock == null)
                    return;
                System.Net.IPEndPoint OrgIP = (System.Net.IPEndPoint)sock.RemoteEndPoint;
                if (ASyncRecved != null)
                    ASyncRecved(sock, OrgIP.Address.ToString(), OrgIP.Port);

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
