using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
namespace Win32CTC
{
    public  class SerialPortSDK
    {

        #region 定义
        SerialPort  SocketRS = null;
        string ErrMsg = "";
        #endregion

        #region 构造

        public SerialPortSDK()
        {
            SocketRS = new SerialPort();
        }

        public SerialPortSDK(int ComPort, int ComRate, int Databits, StopBits Stopbits, Parity Parity, Handshake ctrl)
        {
            SocketRS = new SerialPort();
            this.Port = ComPort;
            this.Rate = ComRate;
            this.DataBit = Databits;
            this.Stopbit = Stopbits;
            this.Parity = Parity;
            this.Ctrl = ctrl;
        }


        #endregion

        #region 属性
        /// <summary>
        /// 串口号
        /// </summary>
        public SerialPort  Uart {
            get
            { return SocketRS; }
            set
            {
                if (value != null)
                {
                    if (SocketRS != null)
                    {
                        try
                        {
                            if (SocketRS.IsOpen)
                                SocketRS.Close();
                        }
                        catch { }
                        SocketRS = null;
                    }
                    SocketRS = value;
                }
            }
        }
        /// <summary>
        /// 串口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBit { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits Stopbit { get; set; }
        /// <summary>
        /// 校验
        /// </summary>
        public Parity  Parity { get;set;}
        /// <summary>
        /// 流控
        /// </summary>
        public Handshake Ctrl { get; set; }
        /// <summary>
        /// 重试次数
        /// </summary>
        public int ReTry { get; set; }
        /// <summary>
        /// 重发次数
        /// </summary>
        public int ReSendTime { get; set; } 
        /// <summary>
        /// 连接超时时间，毫秒
        /// </summary>
        public int ConnectTimeOut { get; set; }
        /// <summary>
        /// 等待时间，毫秒
        /// </summary>
        public int WaitTime { get; set; }
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
        /// 是否打开串口
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
                    { return SocketRS.IsOpen ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 发送超时，毫秒
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
                    { return SocketRS.WriteTimeout ; }
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
                    { SocketRS.WriteTimeout = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 发送缓存区
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
                    { return SocketRS.WriteBufferSize ; }
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
                    { SocketRS.WriteBufferSize = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 读取超时，毫秒
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
                    { return SocketRS.ReadTimeout ; }
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
                    { SocketRS.ReadTimeout = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 接收缓存区
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
                    { return SocketRS.ReadBufferSize ; }
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
                    { SocketRS.ReadBufferSize = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 获取基础对象流
        /// </summary>
        public Stream BaseStream
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return null; }
                    else
                    { return SocketRS.BaseStream ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return null ;
                }
            }
        }
        /// <summary>
        /// 获取或设置中断信号
        /// </summary>
        public bool  BreakState
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return false ; }
                    else
                    { return SocketRS.BreakState ; }
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
                    { SocketRS.BreakState = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 获取接收缓存区还有多少数据未读
        /// </summary>
        public int BytesToRead
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return 0; }
                    else
                    { return SocketRS.BytesToRead ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取发送缓存区还有多少数据未发
        /// </summary>
        public int BytesToWrite
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return 0; }
                    else
                    { return SocketRS.BytesToWrite ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取载波检测行状态
        /// </summary>
        public bool CDHolding
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return false; }
                    else
                    { return SocketRS.CDHolding ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取是否可以发送
        /// </summary>
        public bool CtsHolding
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return false; }
                    else
                    { return SocketRS.CtsHolding ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取或设置忽略NULL
        /// </summary>
        public bool DiscardNull
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return false; }
                    else
                    { return SocketRS.DiscardNull ; }
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
                    { SocketRS.DiscardNull = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 获取就绪信号
        /// </summary>
        public bool DsrHolding
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return false; }
                    else
                    { return SocketRS.DsrHolding ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取或设置触发时间的数据量
        /// </summary>
        public int ReceivedBytesThreshold
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return -1; }
                    else
                    { return SocketRS.ReceivedBytesThreshold ; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    return -2;
                }
            }
            set
            {
                try
                {
                    if (SocketRS != null)
                    { SocketRS.ReceivedBytesThreshold = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// 获取或设置是否启用发送请求信号
        /// </summary>
        public bool RtsEnable
        {
            get
            {
                try
                {
                    if (SocketRS == null)
                    { return false ; }
                    else
                    { return SocketRS.RtsEnable ; }
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
                    { SocketRS.RtsEnable = value; }
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                }
            }
        }
      
        #endregion

        #region 基本方法

        public void Open()
        {
            try
            {
                if (SocketRS != null && Port > 0)
                {
                    if (!SocketRS.IsOpen)
                    {
                        SocketRS.PortName = "Com" + Port.ToString();
                        SocketRS.BaudRate = Rate;
                        SocketRS.DataBits = DataBit;
                        SocketRS.StopBits = Stopbit;
                        SocketRS.Parity = Parity;
                        SocketRS.Handshake = Ctrl;
                        SocketRS.Open(); 
                    }
                }
                else
                    ErrMsg = "端口号不能为0";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }

        public void ReOpen()
        {
            try
            {
                if (SocketRS != null && Port > 0)
                {
                    SocketRS.Close();
                    SocketRS.PortName = "Com" + Port.ToString();
                    SocketRS.BaudRate = Rate;
                    SocketRS.DataBits = DataBit;
                    SocketRS.StopBits = Stopbit;
                    SocketRS.Parity = Parity;
                    SocketRS.Handshake = Ctrl;
                    if (!SocketRS.IsOpen)
                       SocketRS.Open();
                }
            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }
        }


        public void Open(int ComPort,int ComRate,int Databits,StopBits Stopbits,Parity Parity,Handshake ctrl)
        {
            try
            {
                if (SocketRS != null && ComPort > 0)
                {
                    if (!SocketRS.IsOpen)
                    {
                        this.Port = ComPort;
                        this.Rate = ComRate;
                        this.DataBit = Databits;
                        this.Stopbit = Stopbits;
                        this.Parity = Parity;
                        this.Ctrl = ctrl;
                        SocketRS.PortName = "Com" + Port.ToString();
                        SocketRS.BaudRate = Rate;
                        SocketRS.DataBits = DataBit;
                        SocketRS.StopBits = Stopbit;
                        SocketRS.Parity = Parity;
                        SocketRS.Handshake = Ctrl;
                        SocketRS.Open();
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
                    if (SocketRS .IsOpen )
                    SocketRS.Close();
                   
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }

        public int SendBytes(byte[] Data)
        {
            try
            {
                if (SocketRS != null && Port > 0)
                {
                    if (!SocketRS.IsOpen)
                        SocketRS.Open();
                    SocketRS.Write(Data,0,Data.Length );
                    if (ReSendTime > 0)
                    {
                        System.Threading.Thread.Sleep(ReSendTime);
                        SocketRS.Write(Data, 0, Data.Length);
                    }
                    return (SocketRS.BytesToWrite > 0 ? Data.Length - SocketRS.BytesToWrite : Data.Length);
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
                if (SocketRS != null && Port > 0)
                {
                    if (!SocketRS.IsOpen)
                        SocketRS.Open();
                    SocketRS.Write(Data, 0, Data.Length);
                    if (ReSendTime > 0)
                    {
                        System.Threading.Thread.Sleep(ReSendTime);
                        SocketRS.Write(Data, 0, Data.Length);
                    }
                    try
                    {
                        System.Threading.Thread.Sleep(WaitTime);
                        byte[] buf = new byte[SocketRS.ReadBufferSize];
                        int len = SocketRS.Read(buf, 0, buf.Length);
                        byte[] buffer = new byte[len];
                        Array.Copy(buf, buffer, buffer.Length);
                        return buffer;
                    
                    }
                    catch { return null; }
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
                    byte[] buf = new byte[SocketRS.ReadBufferSize ];
                    int len = SocketRS.Read(buf, 0, buf.Length);
                    buffer = new byte[len]; 
                    Array.Copy(buf, buffer, buffer.Length);
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

        public int SendString(string Data,bool NewLine=true,string Encode="" )
        {
            try
            {
                if (Data == null)
                    return -1;
                if (!Data.Contains("\r\n"))
                    Data += "\r\n";
                Encode=Encode==null ?"":Encode.Trim ();
                Encoding encode=Encoding.ASCII ;
                if(Encode!="")
                    encode=Encoding.GetEncoding(Encode);
                if (SocketRS != null && Port > 0)
                {
                    if (!SocketRS.IsOpen)
                        SocketRS.Open();
                    byte[] data = encode.GetBytes(Data);
                    SocketRS.Write(data, 0, data.Length);
                    if (ReSendTime > 0)
                    {
                        System.Threading.Thread.Sleep(ReSendTime);
                        SocketRS.Write(data, 0, data.Length);
                    }
                    return (SocketRS.BytesToWrite > 0 ? Data.Length - SocketRS.BytesToWrite : Data.Length);
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

        public string Receive()
        {
            try
            {
                if (SocketRS != null)
                {
                    return SocketRS.ReadExisting();
                }
                 
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                 
            }
            return "";
        }

        public void Clear()
        {
            try
            {
                SocketRS.DiscardInBuffer();
                SocketRS.DiscardOutBuffer();
            }
            catch { }
           
        }

        #endregion

    }
}
