using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Win32;
using System.Security.Permissions;
using System.Security;
using System.Security.Cryptography;

namespace Win32CTC
{
    public enum Language
    {
        fr,
        en
    }

    public enum ReadReplyCode
    {
        Ok = 0,
        TimeOut = 1,
        UnknownError = 2
    }

    public class ReadException : System.Exception 
    {
        private ReadReplyCode m_ReadReplyCode;


        public ReadException(ReadReplyCode code, string message)
            : base(message)
        {

            m_ReadReplyCode = code;
        }

        public ReadReplyCode ReadReplyCode
        {
            get { return m_ReadReplyCode; }
        }
    }

    public class ConnectionEventArgs : EventArgs
    {
        string m_ip;


        public string Ip
        {
            get { return m_ip; }
        }


        public ConnectionEventArgs(string ip)
        {
            this.m_ip = ip;
        }
    }

    public class FtpSession
    {
        #region 定义
        private Socket m_pClientSocket = null;  // Socket du client
        private FtpServer m_pServer = null;  // Server parent
        private long m_SessionID;				// Identificateur de la session
        private string m_UserName = "";    // Nom de l'utilisateur
        private string m_CurrentDir = "/";	// R閜ertoire courrant
        private string m_renameFrom = "";
        private bool m_PassiveMode = false;	// Flag mode passif
        private TcpListener m_pPassiveListener = null;	// Listener en cas de mode passif
        private IPEndPoint m_pDataConEndPoint = null;	// "EndPoint" pour la connection de donn閑s
        private bool m_Authenticated = false; // Flag utilisateur authentifi?
        private int m_BadCmdCount = 0;     // Nombre de commandes erron閑s
        private DateTime m_SessionStartTime;			// Date de d閎ut de session
        private EndPoint m_rmtEndPoint;				// adresse du client
        private int m_offset = 0;		// offset utile en cas d'usage de l'option "resume"
        private Hashtable m_vPaths;					// Liste des r閜ertoire virtuels appliqu閟 ?l'utilisateur
        private string m_ftproot = "";
        private int SessionTout = 30;
        private bool IsMD5=false ;
        private bool needMD5 = false;
        private bool IsChk = false;
        private bool supportanonymous = false;
        private string anonymousftproot = "";
        private string SafeKey = "";
        private List<Account> Accountlist=new List<Account>();

        public struct Account
        {
            public string UserName; 
            public string Password;
            public string RootDir;
            public long SessionID;
        }
        #endregion

        #region 属性
        public List<Account> User
        {
            get
            { return Accountlist; }
            set
            { Accountlist = value; }
        }

        public long SessionID
        {
            get { return m_SessionID; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return this.m_rmtEndPoint; }
        }

        public Socket ClientSocket
        {
            get { return this.m_pClientSocket; }
        }

        public int SessionTimeout
        {
            get { return SessionTout; }
            set { SessionTout = value; }
        }

        #endregion

        #region 主函数
        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="clientSocket">传入Socket/param>
        /// <param name="server">传递类地址</param>
        /// <param name="sessionID">会话ID</param>
        public FtpSession(Socket clientSocket, FtpServer server, long sessionID)
        {
            this.m_pClientSocket = clientSocket;
            this.m_rmtEndPoint = clientSocket.RemoteEndPoint;
            this.m_pServer = server;
            this.m_SessionID = sessionID;
            this.m_SessionStartTime = System.DateTime.Now;
        }

        /// <summary>
        ///启动函数
        /// </summary>
        public void StartProcessing()
        {
            this.SendData(Messages.MessReady(m_pServer.Language));
            long lastCmdTime = DateTime.Now.Ticks;
            string lastCmd = "";
            try
            {
                while (true)
                {
                    //Si des donn閑s arrivent, on commence ?les lire.
                    if (this.m_pClientSocket.Available > 0)
                    {
                        try
                        {
                            lastCmd = this.ReadLine(m_pClientSocket, this.m_pServer.SessionIdleTimeOut);//Lecture de la Commande
                            if (!SwitchCommand(lastCmd))//Si la commande est "QUIT" on quitte la boucle de traitement
                            {
                                break;
                            }
                        }
                        catch (ReadException rX)
                        {
                            //On verifie que le nombre maximum d'erreurs de commandes n'est pas atteint
                            if (m_BadCmdCount > m_pServer.MaxBadCommands - 1)
                            {
                                SendData(Messages.MessTooManyBadCmds(this.m_pServer.Language));//On envoie un message
                                break;//et on quitte la boucle de traitement
                            }
                            m_BadCmdCount++;
                            switch (rX.ReadReplyCode)//En fonction du type d'ereur (pour l'instant TimeOut et Unknown) on envoie un message
                            {
                                case ReadReplyCode.TimeOut:
                                    SendData(Messages.CmdTimeOut(this.m_pServer.Language));
                                    break;

                                case ReadReplyCode.UnknownError:
                                    SendData(Messages.UnknownError(this.m_pServer.Language));
                                    break;
                            }

                        }
                        catch
                        {
                            if (!this.m_pClientSocket.Connected) break;//Si le socket a 閠?d閏onnect? on quitte la boucle
                            SendData(Messages.UnknownError(this.m_pServer.Language));//Sinon on envoie un message d'erreur inconnu
                        }
                        //On update la date de la derni鑢e commande
                        lastCmdTime = DateTime.Now.Ticks;
                    }

                    else
                    {
                        if (DateTime.Now.Ticks > lastCmdTime + ((long)(m_pServer.SessionIdleTimeOut)) * 10000000)
                        {
                            SendData(Messages.TimeOut(this.m_pServer.Language));
                            break;
                        }
                        Thread.Sleep(100);
                    }
                }

                //Fermeture du Socket
                if (m_pClientSocket.Connected)
                {
                    m_pClientSocket.Close();
                }
                //Destruction de la session
                this.m_pServer.RemoveSession(this.m_SessionID);

            }
            catch
            {
                this.m_pServer.RemoveSession(this.m_SessionID);
            }

        }

        /// <summary>
        /// 关闭函数
        /// </summary>
        public void Close()
        {
            try { this.m_pClientSocket.Close(); }
            catch { }
            try { this.m_pPassiveListener.Stop(); }
            catch { }
        }

        /// <summary>
        /// 安全模式
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="SourceSafe"></param>
        public void SafeMode(bool usemd5,string Key,   bool SourceSafe)
        {
            IsMD5 = usemd5;
            IsChk = SourceSafe;
            SafeKey = Key;
        }

        public void CanAnymous(string RPath)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(RPath.Trim());
                if (di.Exists)
                {
                    supportanonymous = true;
                    anonymousftproot = di.FullName;
                }
                else
                {
                    supportanonymous = false;
                    anonymousftproot = "";
                }
            }
            catch
            {
                supportanonymous = false;
                anonymousftproot = "";
            }
        }

        #endregion

        #region 事件

        /// <summary>
        ///   成功
        /// </summary>
        private void NOOP()
        {
            SendData(Messages.NOOPOK(this.m_pServer.Language));
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="argsText">目录名称</param>
        private void MKD(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            if (newCurdir.Split('/').Length <= 2)
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            try
            {
                new DirectoryInfo(file).Create();
                SendData(Messages.DirCreatedOK(this.m_pServer.Language ));
            }
            catch
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
        }

        /// <summary>
        /// Juste avant un RNTO permet de sp閏ifi?le nom du fichier ?renomer
        /// </summary>
        /// <param name="argsText"></param>
        private void RNFR(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir) || !(System.IO.File.Exists(file) || System.IO.Directory.Exists(file)))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }

            this.m_renameFrom = file;
            SendData(Messages.RNFRFaile(this.m_pServer.Language));

        }

        /// <summary>
        /// Etape suivant le RNFR
        /// </summary>
        /// <param name="argsText"></param>
        private void RNTO(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            if (this.m_renameFrom.Length < 1)
            {
                SendData(Messages.Badsequencecommands(this.m_pServer.Language));
                return;
            }

            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }

            try
            {

                if (System.IO.Directory.Exists(this.m_renameFrom))
                {
                    if (m_renameFrom.ToLower().Replace(m_ftproot.ToLower(), "").Replace("\\", "") == "upload")
                    { SendData(Messages.Errorrenameing(this.m_pServer.Language)); }
                    else
                    {
                        DirectoryInfo FS = new System.IO.DirectoryInfo(this.m_renameFrom);
                        FS.MoveTo(file);
                        SendData(Messages.Directoryrenamed(this.m_pServer.Language));
                    }
                }
                else if (System.IO.File.Exists(this.m_renameFrom))
                {
                    FileInfo FS = new System.IO.FileInfo(this.m_renameFrom);
                    FS.CopyTo(file, true);
                    SendData(Messages.Directoryrenamed(this.m_pServer.Language));
                }
                else
                { SendData(Messages.Errorrenameing(this.m_pServer.Language)); }

            }
            catch
            { SendData(Messages.Errorrenameing(this.m_pServer.Language)); }
            this.m_renameFrom = "";

        }

        /// <summary>
        /// 获取续传参数
        /// </summary>
        /// <param name="argsText">offset</param>
        private void REST(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            try
            {
                this.m_offset = Convert.ToInt32(argsText);
                SendData(Messages.argumentREST(this.m_pServer.Language, this.m_offset));
            }
            catch
            {
                SendData(Messages.badargumentREST(this.m_pServer.Language));
            }
        }

        /// <summary>
        /// 续传
        /// </summary>
        /// <param name="argsText">文件名</param>
        private void APPE(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }

            FileStream fs;
            try
            {
                fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Position = fs.Length;
            }
            catch
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            Socket socket = this.GetDataConnection();
            try
            {
                if (socket == null || fs == null)
                {
                    throw new Exception();
                }
                int readed = 1;
                byte[] buffer = new byte[4096];
                while (readed > 0)
                {
                    readed = socket.Receive(buffer);
                    fs.Write(buffer, 0, readed);
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                SendData(Messages.TrComplete(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.TrFailed(this.m_pServer.Language));
                socket.Close();
            }
            fs.Close();

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="argsText">文件名</param>
        private void DELE(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir) || !System.IO.File.Exists(file))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }

            try
            {
                new FileInfo(file).Delete();
                SendData(Messages.DeleOk(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
            }
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="argsText"></param>
        private void RMD(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir) || !System.IO.Directory.Exists(file))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            try
            {

                if (newCurdir.ToLower().Trim() == "/upload")
                {
                    new DirectoryInfo(file).Delete(true);
                    new DirectoryInfo(file).Create();
                }
                else
                { new DirectoryInfo(file).Delete(true); }
                SendData(Messages.DeleOk(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="argsText">文件名</param>
        private void STOR(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            if (m_CurrentDir.Trim() == "/" || m_CurrentDir.ToLower().StartsWith("upload/") || m_CurrentDir.ToLower().StartsWith("/upload/"))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            FileInfo fsinf = new FileInfo(file);
            FileStream fs;
            try
            {
                fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Position = this.m_offset;
                fs.SetLength(this.m_offset);
            }
            catch
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            Socket socket = this.GetDataConnection();
            try
            {
                if (socket == null || fs == null)
                {
                    throw new Exception();
                }
                int readed = 1;
                byte[] buffer = new byte[4096];
                while (readed > 0)
                {
                    readed = socket.Receive(buffer);
                    fs.Write(buffer, 0, readed);
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                SendData(Messages.TrComplete(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.TrFailed(this.m_pServer.Language));
                socket.Close();
            }
            fs.Close();

        }


        /// <summary>
        ///下载文件，带续传
        /// </summary>
        /// <param name="argsText">文件名</param>
        private void RETR(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, argsText, this.m_vPaths, out file, out newCurdir) || !System.IO.File.Exists(file))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }
            FileStream fs;
            try
            {
                Socket socket = this.GetDataConnection();
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                fs.Position = this.m_offset;
                if (socket == null || fs == null)
                {
                    throw new Exception();
                }
                int readed = 1;
                byte[] buffer = new byte[4096];
                while (readed > 0)
                {
                    readed = fs.Read(buffer, 0, buffer.Length);
                    socket.Send(buffer, readed, SocketFlags.None);
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                SendData(Messages.TrComplete(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.TrFailed(this.m_pServer.Language));
                return;
            }
            fs.Close();

        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private void QUIT()
        {
            SendData(Messages.SignOff(this.m_pServer.Language));
            for(int i=0;i<Accountlist.Count;i++)
            {
                if (Accountlist[i].UserName == m_UserName)
                {
                    Account acc=new Account();
                    acc.UserName = Accountlist[i].UserName;
                    acc.Password = Accountlist[i].Password;
                    acc.RootDir = Accountlist[i].RootDir;
                    acc.SessionID  =0;
                    Accountlist.Remove(Accountlist[i]);
                    Accountlist.Add(acc);
                }
            }
        }


        /// <summary>
        ///获取用户名
        /// </summary>
        /// <param name="argsText">login</param>
        private void USER(string argsText)
        {
            if (m_Authenticated)
            {
                SendData(Messages.AlreadyAuth(this.m_pServer.Language));
                return;
            }
            if (m_UserName.Length > 0)
            {
                SendData(Messages.UserButNotPass(this.m_pServer.Language));
                return;
            }

            string userName = argsText;

            SendData(Messages.PassReq(this.m_pServer.Language, argsText));
            m_UserName = userName;
        }

        /// <summary>
        /// 登陆验证
        /// </summary>
        /// <param name="argsText">Mot de passe</param>
        private void PASS(string argsText)
        {
            if (m_Authenticated)
            {
                SendData(Messages.AlreadyAuth(this.m_pServer.Language));
                return;
            }
            if (m_UserName.Length == 0)
            {
                SendData(Messages.EnterUser(this.m_pServer.Language));
                return;
            }
            if (Auth(ref m_UserName, argsText ))
            {
                m_Authenticated = true;
                SendData(Messages.PassOk(this.m_pServer.Language));
                try
                {
                    this.m_ftproot = UserRootPath(m_UserName, m_SessionID);
                     if (this.m_ftproot == string.Empty)
                    {
                        SendData(Messages.BadHome(this.m_pServer.Language));
                        return;
                    }
                }
                catch (Exception e)
                { SendData(Messages.AuthFailed(this.m_pServer.Language)); }
                this.m_vPaths = VPaths(m_UserName);
            }
            else
            {
                SendData(Messages.AuthFailed(this.m_pServer.Language));
                m_UserName = "";
                this.m_ftproot = "";
                this.m_vPaths = new Hashtable();
            }

        }


        /// <summary>
        /// 获取根目录
        /// </summary>
        private void PWD()
        {

            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            try
            {
                if (this.m_ftproot == null)
                { SendData(Messages.PwdFal(this.m_pServer.Language, m_CurrentDir)); }
                else if (this.m_ftproot.Trim() == "")
                { SendData(Messages.PwdFal(this.m_pServer.Language, m_CurrentDir)); }
                else
                {
                    if (Directory.Exists(this.m_ftproot))
                    { SendData(Messages.Pwd(this.m_pServer.Language, m_CurrentDir)); }
                    else
                    { SendData(Messages.PwdFal(this.m_pServer.Language, m_CurrentDir)); }
                }
            }
            catch
            { SendData(Messages.PwdFal(this.m_pServer.Language, m_CurrentDir)); }
        }

        /// <summary>
        ///服务器OS版本
        /// </summary>
        private void SYST()
        {

            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }

            SendData("215 " + Environment.OSVersion.VersionString + "\r\n");
        }

        /// <summary>
        ///传输方式 A为字符，I为二进制
        /// </summary>
        /// <param name="argsText">type</param>
        private void TYPE(string argsText)
        {

            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            if (argsText.Trim().ToUpper() == "A" || argsText.Trim().ToUpper() == "I")
            {
                SendData(Messages.TypeSet(this.m_pServer.Language, argsText));
            }
            else
            {
                SendData(Messages.InvalidType(this.m_pServer.Language, argsText));
            }
        }


        /// <summary>
        /// 主动模式
        /// </summary>
        /// <param name="argsText">suite de 6 octets (sous forme d閏imale) repr閟entant l'ip et le port. i1,i2,i3,i4,p1,p2  (avec p1 octet de poid fort et p2 octet de poid faible)</param>
        private void PORT(string argsText)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }

            //on s閜are d'abord les diff閞entes parties de l'adresse
            string[] parts = argsText.Split(',');

            //on v閞ifie la syntaxe
            if (parts.Length != 6)
            {
                SendData(Messages.SyntaxError(this.m_pServer.Language));
                return;
            }

            //On d閒init l'adresse ip
            string ip = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];


            //le port est d閒init par un calcul au niveau bit : la premi鑢e partie est d閏al閑 de 8bits vers l gauche et addition閑 avec la deuxi鑝e partie.
            int port = (Convert.ToInt32(parts[4]) << 8) | Convert.ToInt32(parts[5]);

            m_pDataConEndPoint = new IPEndPoint(System.Net.Dns.GetHostByAddress(ip).AddressList[0], port);

            this.m_PassiveMode = false;
            SendData(Messages.PortCmdSuccess(this.m_pServer.Language));
        }


        /// <summary>
        /// 被动模式
        /// </summary>
        private void PASV()
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }

            //On commence par chercher un port libre pour lancer l'閏oute
            int port = 1025;
            while (true)
            {
                try
                {
                    m_pPassiveListener = new TcpListener(IPAddress.Any, port);
                    m_pPassiveListener.Start();

                    //Si on arrive jusqu'ici, c'est que le port est libre
                    break;
                }
                catch
                {
                    port++;
                }


            }

            //A partir du socket, on recr?une chaine de type i1,i2,i3,i4,p1,p2
            string ip = m_pClientSocket.LocalEndPoint.ToString();
            ip = ip.Substring(0, ip.IndexOf(":"));
            ip = ip.Replace(".", ",");

            //la premi鑢e partie du port est cr殚e en d閜la鏰nt les 8 bits de poids fort vers la droite,
            //la deuxi鑝e est cr閑 en ne prenant que les 8 bits de poids faible (2^8=256)
            ip += "," + (port >> 8) + "," + (port & 255);

            SendData(Messages.PasvCmdSuccess(this.m_pServer.Language, ip));

            m_PassiveMode = true;

        }

        /// <summary>
        /// 列出文件位置信息
        /// </summary>
        private void LIST(string args)
        {
            if (!this.m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            try
            {

                string cdir;
                string newCurdir;
                if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, args, this.m_vPaths, out cdir, out newCurdir) || !System.IO.Directory.Exists(cdir))
                {
                    SendData(Messages.AccesDenied(this.m_pServer.Language));
                    return;
                }

                ArrayList files = new ArrayList();
                try
                {
                    files.AddRange(System.IO.Directory.GetDirectories(cdir));
                    files.AddRange(VPathsInDir(this.m_CurrentDir, this.m_vPaths));
                    files.AddRange(System.IO.Directory.GetFiles(cdir));
                }
                catch
                {
                    SendData(Messages.AccesDenied(this.m_pServer.Language));
                    return;
                }

                //返回
                Socket socket = this.GetDataConnection();
                string msg = "";
                foreach (string file in files)
                {

                    if (System.IO.File.Exists(file))
                    {
                        msg += "-rwxrwxrwx    0    0    0    ";
                        msg += new System.IO.FileInfo(file).Length.ToString() + " ";
                        msg += MonthFromInt(System.IO.File.GetLastWriteTime(file).Month) + " " + System.IO.File.GetLastWriteTime(file).ToString("dd  hh:mm") + " ";
                        msg += System.IO.Path.GetFileName(file) + "\r\n";
                    }
                    else if (System.IO.Directory.Exists(file))
                    {
                        msg += "drwxrwxrwx    0    0    0    ";
                        msg += " 4096 ";
                        msg += MonthFromInt(System.IO.Directory.GetLastWriteTime(file).Month) + " " + System.IO.Directory.GetLastWriteTime(file).ToString("dd  hh:mm") + " ";
                        msg += System.IO.Path.GetFileName(file) + "\r\n";
                    }
                    else
                    {
                        //msg+="drwxrwxrwx    0    0    0    ";
                        //msg+=" 4096 ";
                        //msg+=MonthFromInt(System.DateTime.Now.Month)+" "+System.DateTime.Now.ToString("dd  hh:mm")+" ";
                        //msg+=file+"\r\n";
                    }

                }
                byte[] toSend = System.Text.Encoding.Default.GetBytes(msg);

                socket.Send(toSend);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                SendData(Messages.TrComplete(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.TrFailed(this.m_pServer.Language));
            }

        }

        /// <summary>
        /// 列出文件大小
        /// </summary>
        /// <param name="args"></param>
        private void SIZE(string args)
        {
            if (!m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }
            string file;
            string newCurdir;
            if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, args, this.m_vPaths, out file, out newCurdir))
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
                return;
            }

            FileStream fs;
            try
            {
                fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
                 SendData("213 " + fs.Length + "\r\n");
               
                fs.Close();
            }
            catch
            {
                SendData("213 0\r\n");
            }
            
          

        }

        /// <summary>
        /// 列出文件名
        /// </summary>
        private void NLST(string args)
        {
            //l'utilisateur doit 阾re identifi?
            if (!this.m_Authenticated)
            {
                SendData(Messages.AuthReq(this.m_pServer.Language));
                return;
            }


            try
            {


                string cdir;
                string newCurdir;
                //on formate le r閜ertoire voulu

                if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, args, this.m_vPaths, out cdir, out newCurdir) || !System.IO.Directory.Exists(cdir))
                {
                    SendData(Messages.AccesDenied(this.m_pServer.Language));
                    return;
                }
                ArrayList files = new ArrayList();
                try
                {
                    files.AddRange(System.IO.Directory.GetDirectories(cdir));
                    files.AddRange(VPathsInDir(this.m_CurrentDir, this.m_vPaths));
                    files.AddRange(System.IO.Directory.GetFiles(cdir));
                }
                catch
                {
                    SendData(Messages.AccesDenied(this.m_pServer.Language));
                    return;
                }
                //On r閏up鑢e le socket de donn閑s

                Socket socket = this.GetDataConnection();
                string msg = "";
                foreach (string file in files)
                {
                    if (System.IO.File.Exists(file))
                    {
                        msg += System.IO.Path.GetFileName(file) + "\r\n";
                    }
                    else if (System.IO.Directory.Exists(file))
                    {

                        msg += System.IO.Path.GetFileName(file) + "\r\n";
                    }

                    else
                    {
                        msg += file + "\r\n";
                    }
                }

                byte[] toSend = System.Text.Encoding.Default.GetBytes(msg);

                socket.Send(toSend);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                SendData(Messages.TrComplete(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.TrFailed(this.m_pServer.Language));
            }

        }


        /// <summary>
        /// 定位
        /// </summary>
        /// <param name="args">R閜ertoire</param>
        private void CWD(string args)
        {
            try
            {
                if (!this.m_Authenticated)
                {
                    SendData(Messages.AuthReq(this.m_pServer.Language));
                    return;
                }
                string cdir;
                string newCurdir;
                if (!IsValidDir(this.m_ftproot, this.m_CurrentDir, args.Trim(), this.m_vPaths, out cdir, out newCurdir) ||
                    !System.IO.Directory.Exists(cdir))
                {
                    SendData(Messages.AccesDenied(this.m_pServer.Language));
                    return;
                }


                this.m_CurrentDir = newCurdir;
                SendData(Messages.CwdOk(this.m_pServer.Language));
            }
            catch
            {
                SendData(Messages.AccesDenied(this.m_pServer.Language));
            }
        }

        #endregion

        #region 子函数
        /// <summary>
        /// Fonction permettant d'obtenir le socket de donn閑
        /// </summary>
        /// <returns>Socket de donn閑s</returns>
        private Socket GetDataConnection()
        {
            Socket socket = null;
            try
            {
                //si on est en mode passif
                if (m_PassiveMode)
                {
                    long startTime = DateTime.Now.Ticks;
                    // On attend que le server est re鐄 une requ鑤e de connection
                    while (!m_pPassiveListener.Pending())
                    {
                        System.Threading.Thread.Sleep(50);

                        // Time out apr鑣 30 secondes
                        if ((DateTime.Now.Ticks - startTime) / 10000 > SessionTout * 1000)
                        {
                            //SendData(Messages.TimeOut(this.m_pServer.Language));
                            throw new Exception("Ftp server didn't respond !");
                        }
                    }

                    //On accepte le socket
                    socket = m_pPassiveListener.AcceptSocket();

                    SendData(Messages.DataOpen(this.m_pServer.Language));
                }
                else //Si en mode actif
                {

                    SendData(Messages.DataOpening(this.m_pServer.Language));

                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(m_pDataConEndPoint);
                }
            }
            catch
            {
                SendData(Messages.opendatafailed(this.m_pServer.Language));
                return null;
            }

            m_PassiveMode = false;

            return socket;
        }

        private string MonthFromInt(int month)
        {
            switch (month)
            {
                case 1:
                    return "jan";
                case 2:
                    return "feb";
                case 3:
                    return "mar";
                case 4:
                    return "apr";
                case 5:
                    return "may";
                case 6:
                    return "jun";
                case 7:
                    return "jul";
                case 8:
                    return "aug";
                case 9:
                    return "sep";
                case 10:
                    return "oct";
                case 11:
                    return "nov";
                case 12:
                    return "dec";
                default:
                    return "";
            }
        }

        /// <summary>
        /// M閠hode permettant d'envoyer un message au client
        /// </summary>
        /// <param name="data">Message ?envoyer</param>
        private void SendData(string data)
        {
            Byte[] byte_data = System.Text.Encoding.Default.GetBytes(data.ToCharArray());
            m_pClientSocket.Send(byte_data, byte_data.Length, 0);
        }

        /// <summary>
        /// Fonction permettant de lire une commande tap閑 par le client.
        /// </summary>
        /// <param name="clientSocket">Socket du client</param>
        /// <param name="timeOut">Nombre de secondes maximum pour la lecture</param>
        /// <returns></returns>
        private string ReadLine(Socket clientSocket, int timeOut)
        {
            long lastDataTime = DateTime.Now.Ticks; //date de derni鑢e r閏eption
            ArrayList lineBuf = new ArrayList(); //Buffer
            byte prevByte = 0;

            while (true)
            {
                if (clientSocket.Available > 0)
                {
                    // On lit un octet
                    byte[] currByte = new byte[1];
                    int countRecieved = clientSocket.Receive(currByte, 1, SocketFlags.None);
                    if (countRecieved == 1)
                    {
                        //on stocke l'octet dans le buffer
                        lineBuf.Add(currByte[0]);

                        // Si on arrive en fin de ligne, on renvoie la commande
                        if ((prevByte == (byte)'\r' && currByte[0] == (byte)'\n'))
                        {
                            byte[] retVal = new byte[lineBuf.Count - 2];    // On enl鑦e les caract鑢es de fin de ligne 
                            lineBuf.CopyTo(0, retVal, 0, lineBuf.Count - 2);

                            return System.Text.Encoding.Default.GetString(retVal).Trim();
                        }

                        // on update la valeur de prevByte
                        prevByte = currByte[0];

                        //On met ?jour la date de derni鑢e r閏eption
                        lastDataTime = DateTime.Now.Ticks;
                    }
                }
                else
                {
                    //Si le Timeout est atteint on lance une excepion
                    if (DateTime.Now.Ticks > lastDataTime + ((long)(SessionTout)) * 100000)
                    {
                        throw new ReadException(ReadReplyCode.TimeOut, "Read timeout");
                    }
                    System.Threading.Thread.Sleep(100);//N閏essaire pour des raisons 関identes de performance									

                }
            }
        }

        private bool Auth(ref string Account, string Pwd)
        {
            if (Pwd == null)
            { Pwd = ""; }
            if (Account == null)
            { return false; }
            else if (Account.Trim() == "" )
            { return false; }
            else
            {
                if (supportanonymous && Account.ToLower().Trim() == "anonymous")
                {return true;}
                for (int i = 0; i < Accountlist.Count; i++)
                {
                    if (Accountlist[i].UserName.ToLower().Trim() == Account.ToLower().Trim())
                    {
                        Account = Accountlist[i].UserName;
                        if (IsMD5 &&  Pwd.ToUpper().Trim()!="" )
                        {
                            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                            if (needMD5)
                            {
                                string nupwd = BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(Pwd.ToLower().Trim()+SafeKey.Trim()))).Replace("-", "");
                                Pwd = nupwd.ToUpper().Trim();
                            }
                            if (IsChk)
                            {

                                string npwd = BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(Accountlist[i].Password.ToLower().Trim() + SafeKey.Trim()))).Replace("-", "");
                                if (Pwd == npwd.ToUpper().Trim())
                                { return true; }
                                else
                                { return false; }
                            }
                            else
                            {
                                if (Pwd == Accountlist[i].Password.ToUpper().Trim())
                                { return true; }
                                else
                                { return false; }
                            }
                        }
                        else
                        {
                            if (Accountlist[i].Password.ToLower().Trim() == Pwd.ToLower().Trim())
                            { return true; }
                            else
                            { return false; }
                        }
                    }
                }
            }
            return false;
        }

        private string UserRootPath(string Account,long sessionid)
        {
            if (Account == "anonymous")
            { return anonymousftproot; }
            else
            {
                for (int i = 0; i < Accountlist.Count; i++)
                {
                    if (Accountlist[i].UserName == m_UserName)
                    {
                        Account acc = new Account();
                        acc.UserName = Accountlist[i].UserName;
                        acc.Password = Accountlist[i].Password;
                        acc.RootDir = Accountlist[i].RootDir;
                        acc.SessionID = sessionid;
                        Accountlist.Remove(Accountlist[i]);
                        Accountlist.Add(acc);
                        return acc.RootDir;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        ///命令转换为指令集
        /// </summary>
        /// <param name="commandTxt">文字指令</param>
        /// <returns>指令</returns>
        private bool SwitchCommand(string commandTxt)
        {
            string[] cmdParts = commandTxt.TrimStart().Split(new char[] { ' ' });
            string command = cmdParts[0].ToUpper().Trim();

            switch (command)
            {
                case "QUIT":
                    QUIT();
                    return false;
                case "USER":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            USER(args);
                        }
                        else
                        {
                            USER(cmdParts[1]);
                        }
                    }
                    break;
                case "TYPE":
                    if (cmdParts.Length != 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else TYPE(cmdParts[1]);
                    break;
                case "PASS":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            PASS(args);
                        }
                        else
                        {
                            PASS(cmdParts[1]);
                        }
                    }
                    break;
                case "PWD":
                    PWD();
                    break;

                case "XPWD":
                    PWD();
                    break;

                case "SYST":
                    SYST();
                    break;
                case "PORT":
                    if (cmdParts.Length != 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else PORT(cmdParts[1]);
                    break;
                case "PASV":
                    PASV();
                    break;
                case "LIST":
                    if (cmdParts.Length > 1)
                    {
                        if (cmdParts[1].Trim().ToLower() == "-a" || cmdParts[1].Trim().ToLower() == "-l" || cmdParts[1].Trim().ToLower() == "-al")
                        {
                            string[] parts = new string[cmdParts.Length - 1];
                            parts[0] = cmdParts[0];
                            for (int i = 2; i < cmdParts.Length; i++)
                                parts[i - 1] = cmdParts[i];
                            cmdParts = parts;
                        }
                    }
                    if (cmdParts.Length > 2)
                    {
                        string args = "";
                        for (int i = 1; i < cmdParts.Length; i++)
                        {
                            args += cmdParts[i] + " ";
                        }
                        LIST(args);
                    }
                    else if (cmdParts.Length == 2) LIST(cmdParts[1]);
                    else LIST(String.Empty);
                    break;
                case "NLST":
                    if (cmdParts.Length > 1)
                    {
                        if (cmdParts[1].Trim().ToLower() == "-a" || cmdParts[1].Trim().ToLower() == "-l" || cmdParts[1].Trim().ToLower() == "-al")
                        {
                            string[] parts = new string[cmdParts.Length - 1];
                            parts[0] = cmdParts[0];
                            for (int i = 2; i < cmdParts.Length; i++)
                                parts[i - 1] = cmdParts[i];
                            cmdParts = parts;
                        }
                    }
                    if (cmdParts.Length > 2)
                    {
                        string args = "";
                        for (int i = 1; i < cmdParts.Length; i++)
                        {
                            args += cmdParts[i] + " ";
                        }
                        LIST(args);
                    }
                    else if (cmdParts.Length == 2) NLST(cmdParts[1]);
                    else NLST(String.Empty);
                    break;

                case "CWD":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            CWD(args);
                        }
                        else
                        {
                            CWD(cmdParts[1]);
                        }
                    }
                    break;
                case "RETR":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            RETR(args);
                        }
                        else
                        {
                            RETR(cmdParts[1]);
                        }
                    }
                    break;
                case "STOR":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            STOR(args);
                        }
                        else
                        {
                            STOR(cmdParts[1]);
                        }
                    }
                    break;

                case "DELE":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            DELE(args);
                        }
                        else
                        {
                            DELE(cmdParts[1]);
                        }
                    }
                    break;

                case "RMD":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            RMD(args);
                        }
                        else
                        {
                            RMD(cmdParts[1]);
                        }
                    }
                    break;

                case "APPE":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            APPE(args);
                        }
                        else
                        {
                            APPE(cmdParts[1]);
                        }
                    }
                    break;

                case "RNFR":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            RNFR(args);
                        }
                        else
                        {
                            RNFR(cmdParts[1]);
                        }
                    }
                    break;

                case "RNTO":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            RNTO(args);
                        }
                        else
                        {
                            RNTO(cmdParts[1]);
                        }
                    }
                    break;

                case "MKD":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            MKD(args);
                        }
                        else
                        {
                            MKD(cmdParts[1]);
                        }
                    }
                    break;

                case "REST":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            REST(args);
                        }
                        else
                        {
                            REST(cmdParts[1]);
                        }
                    }
                    break;

                case "SIZE":
                    if (cmdParts.Length < 2) SendData(Messages.UnknownError(this.m_pServer.Language));
                    else
                    {
                        if (cmdParts.Length > 2)
                        {
                            string args = "";
                            for (int i = 1; i < cmdParts.Length; i++)
                            {
                                args += cmdParts[i] + " ";
                            }
                            SIZE(args);
                        }
                        else
                        {
                            SIZE(cmdParts[1]);
                        }
                    }
                    break;

                case "CDUP":
                    CWD("..");
                    break;
                case "NOOP":
                    NOOP();
                    break;

                case "CLNT":
                    SendData(Messages.Noted(this.m_pServer.Language));
                    break;
                case "OPTS":
                     SendData(Messages.CMDOK(this.m_pServer.Language));
                    break;

                default:
                    SendData(Messages.Invalidcommand(this.m_pServer.Language));
                    break;

            }

            return true;
        }

        private bool IsValidDir(string ftpRoot, string curDir, string path, Hashtable vpaths, out string physicalPath, out string newCurdir)
        {
            if (path.Length > 0)
            {

                if (path.StartsWith("/"))
                {
                    string[] parts = path.Split('/');
                    int depth = 0;
                    foreach (string p in parts)
                    {
                        if (p != "")
                        {
                            if (p == "..") depth--;
                            else depth++;
                            if (depth < 0)
                            {
                                newCurdir = ResolveComplexUrl(curDir);
                                physicalPath = String.Empty;
                                return false;
                            }
                        }

                    }

                }
                else
                {
                    if (!curDir.EndsWith("/")) curDir += "/";
                    path = curDir + path;
                    string[] parts = path.Split('/');
                    int depth = 0;
                    foreach (string p in parts)
                    {
                        if (p != "")
                        {

                            if (p == "..") depth--;
                            else depth++;
                            if (depth < 0)
                            {
                                newCurdir = ResolveComplexUrl(curDir);
                                physicalPath = String.Empty;
                                return false;
                            }
                        }
                    }
                }

                if (ftpRoot.EndsWith("\\"))
                {
                    ftpRoot.Remove(ftpRoot.Length - 1, 1);
                }
                newCurdir = ResolveComplexUrl(path);
                physicalPath = GetPhysicalDir(path, ftpRoot, vpaths);
                return true;




            }
            else
            {
                if (ftpRoot.EndsWith("\\"))
                {
                    ftpRoot.Remove(ftpRoot.Length - 1, 1);
                }
                newCurdir = path = ResolveComplexUrl(curDir);
                physicalPath = GetPhysicalDir(path, ftpRoot, vpaths);
                return true;
            }
        }

        private string ResolveComplexUrl(string url)
        {
            string[] parts = url.Split('/');
            string rslv = "/";
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i] != ".." && parts[i + 1] != ".." && parts[i] != "")
                {
                    rslv += parts[i] + "/";
                }
            }
            if (parts[parts.Length - 1] != ".." && parts[parts.Length - 1] != "")
                rslv += parts[parts.Length - 1];
            return rslv;
        }

        private string GetPhysicalDir(string path, string ftproot, Hashtable vpaths)
        {
            if (path.Contains("."))
            {
                if (path.IndexOf("/") == path.LastIndexOf("/"))
                {
                    string v = path.Replace("/", "").ToLower();
                    try
                    {
                        return vpaths[v].ToString();
                    }
                    catch
                    {
                        return ftproot + path.Replace("/", "\\");
                    }
                }
                else
                {
                    return ftproot + path.Replace("/", "\\");
                }
            }
            else
            {
                return ftproot + path.Replace("/", "\\");
            }
        }

        private ArrayList VPathsInDir(string curDir, Hashtable vpaths)
        {
            ArrayList vdirs = new ArrayList();
            if (curDir == "" || curDir == null || curDir == "/")
            {
                foreach (string virdir in vpaths.Values)
                {
                    string vdir = virdir.ToLower();
                    vdirs.Add(vdir);
                }
            }
            return vdirs;
        }
       
        private Hashtable VPaths(string username)
        {
            System.Collections.Hashtable vpaths = new Hashtable();
            try
            {
                    if (this.Accountlist.Count > 0)
                    {
                        for (int i = 0; i < this.Accountlist.Count; i++)
                        {
                            try
                            {
                                DirectoryInfo di = new DirectoryInfo(Accountlist[i].RootDir);
                                if (di.Exists)
                                {
                                    DirectoryInfo[] dilist = di.GetDirectories();
                                    if (dilist != null)
                                    {
                                        for (int n = 0; n < dilist.Length; n++)
                                        {
                                            try
                                            { vpaths.Add(dilist[i].Name.ToLower(), dilist[i].FullName); }
                                            catch
                                            { vpaths[dilist[i].Name.ToLower()] = dilist[i].FullName; }
                                        }
                                    }
                                    FileInfo[] diflist = di.GetFiles();
                                    if (dilist != null)
                                    {
                                        for (int n = 0; n < dilist.Length; n++)
                                        {
                                            try
                                            { vpaths.Add(dilist[i].Name.ToLower(), dilist[i].FullName); }
                                            catch
                                            { vpaths[dilist[i].Name.ToLower()] = dilist[i].FullName; }
                                        }
                                    }
                                }
                            }
                            catch
                            { }
                        }
                    }
            }
            catch
            { }
            return vpaths;
        }

        #endregion
    }

    public class Messages
    {
        public static string CMDOK(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "200 OK\r\n";

                case Language.en:
                    return "200 OK\r\n";

                default:
                    return "200 OK\r\n";


            }
        }
        public static string MessReady(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "220 Serveur FTP pret\r\n";

                case Language.en:
                    return "220 FTP Server Ready\r\n";

                default:
                    return "220 FTP Server Ready\r\n";


            }
        }
        public static string SignOff(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "221 Fermeture de la session\r\n";

                case Language.en:
                    return "221 FTP Server Signing off\r\n";

                default:
                    return "221 FTP Server Signing off\r\n";


            }
        }
        public static string MessTooManyBadCmds(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "421 Trop de commandes erronees\r\n";

                case Language.en:
                    return "421 Too many bad Commands\r\n";

                default:
                    return "421 Too many bad Commands\r\n";


            }
        }
        public static string CmdTimeOut(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "500 delai de reception de commande ecoule\r\n";

                case Language.en:
                    return "500 Command Timeout\r\n";

                default:
                    return "500 Command Timeout\r\n";


            }
        }
        public static string UnknownError(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "500 Erreur de type inconnu\r\n";

                case Language.en:
                    return "500 unkown error\r\n";

                default:
                    return "500 unkown error\r\n";


            }
        }
        public static string AlreadyAuth(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "500 Vous etes deja authentifie\r\n";

                case Language.en:
                    return "500 You are already authenticated\r\n";

                default:
                    return "500 You are already authenticated\r\n";


            }
        }
        public static string UserButNotPass(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "500 Nom d'utilisateur deja rentre, mais mot de passe requis\r\n";

                case Language.en:
                    return "500 username is already specified, please specify password\r\n";

                default:
                    return "500 username is already specified, please specify password\r\n";


            }
        }
        public static string EnterUser(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "503 Veuillez d'abord donner votre login\r\n";

                case Language.en:
                    return "503 please specify username first\r\n";

                default:
                    return "503 please specify username first\r\n";


            }
        }
        public static string SyntaxError(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "500 Erreur de syntaxe\r\n";

                case Language.en:
                    return "500 Syntax error\r\n";

                default:
                    return "500 Syntax error\r\n";


            }
        }
        public static string PassOk(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "230 Mot de passe correct\r\n";

                case Language.en:
                    return "230 Password ok\r\n";

                default:
                    return "230 Password ok\r\n";


            }
        }
        public static string CwdOk(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "250 Commande CWD executee.\r\n";

                case Language.en:
                    return "250 CWD command successful.\r\n";

                default:
                    return "250 CWD command successful.\r\n";


            }
        }
        public static string DeleOk(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "250 Commande DELE executee.\r\n";

                case Language.en:
                    return "250 DELE command successful.\r\n";

                default:
                    return "250 DELE command successful.\r\n";


            }
        }
        public static string AuthFailed(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "530 Nom d'utilisateur ou mot de passe incorrect\r\n";

                case Language.en:
                    return "530 UserName or Password is incorrect\r\n";

                default:
                    return "530 UserName or Password is incorrect\r\n";


            }
        }
        public static string logFailed(Language language,string msg)
        {
            switch (language)
            {
                case Language.fr:
                    return "530 incorrect " + msg + " \r\n";

                case Language.en:
                    return "530 " + msg + "\r\n";

                default:
                    return "530 " + msg + "\r\n";


            }
        }
        public static string PassReq(Language language, string userName)
        {
            switch (language)
            {
                case Language.fr:
                    return "331 Mot de passe requis pour l'utilisateur : " + userName + "\r\n";

                case Language.en:
                    return "331 Password required for user : '" + userName + "\r\n";

                default:
                    return "331 Password required for user : '" + userName + "\r\n";


            }
        }
        public static string Pwd(Language language, string curdir)
        {
            switch (language)
            {
                case Language.fr:
                    return "257 \"" + curdir + "\" est le repertoire courrant\r\n";

                case Language.en:
                    return "257 \"" + curdir + "\" is current directory.\r\n";

                default:
                    return "257 \"" + curdir + "\" is current directory.\r\n";


            }
        }
        public static string PwdFal(Language language, string curdir)
        {
            switch (language)
            {
                case Language.fr:
                    return "502 \"" + curdir + "\" est le  repertoire courrant\r\n";

                case Language.en:
                    return "502 \"" + curdir + "\" is not current directory.\r\n";

                default:
                    return "502 \"" + curdir + "\" is not current directory.\r\n";


            }
        }
        public static string TypeSet(Language language, string type)
        {
            switch (language)
            {
                case Language.fr:
                    return "200 Type de transfert : " + type + ".\r\n";

                case Language.en:
                    return "200 Type is set to " + type + ".\r\n";

                default:
                    return "200 Type is set to " + type + ".\r\n";


            }
        }
        public static string InvalidType(Language language, string type)
        {
            switch (language)
            {
                case Language.fr:
                    return "500 Type invalide : " + type + ".\r\n";

                case Language.en:
                    return "500 Invalid type : " + type + ".\r\n";

                default:
                    return "500 Invalid type : " + type + ".\r\n";


            }
        }
        public static string AuthReq(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "530 Authentification necessaire\r\n";

                case Language.en:
                    return "530 Please authenticate first\r\n";

                default:
                    return "530 Please authenticate first\r\n";


            }
        }
        public static string PortCmdSuccess(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "200 Commande PORT reussite\r\n";

                case Language.en:
                    return "200 PORT Command successful\r\n";

                default:
                    return "200 PORT Command successful\r\n";


            }
        }
        public static string DataOpen(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "125 Connection de donnees ouverte, debut du transfert.\r\n";

                case Language.en:
                    return "125 Data connection open, Transfer starting.\r\n";

                default:
                    return "125 Data connection open, Transfer starting.\r\n";


            }
        }
        public static string DataOpening(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "150 Ouverture de la connection de donnees.\r\n";

                case Language.en:
                    return "150 Opening data connection.\r\n";

                default:
                    return "150 Opening data connection.\r\n";


            }
        }
        public static string DataConFailed(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "425 Erreur d'ouverture de connection de donnees.\r\n";

                case Language.en:
                    return "425 Can't open data connection.\r\n";

                default:
                    return "425 Can't open data connection.\r\n";


            }
        }
        public static string TrComplete(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "226 Transfer termine.\r\n";

                case Language.en:
                    return "226 Transfer Complete.\r\n";

                default:
                    return "226 Transfer Complete.\r\n";


            }
        }
        public static string TrFailed(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "426 Connection perdue; transfer annule.\r\n";

                case Language.en:
                    return "426 Connection closed; transfer aborted.\r\n";

                default:
                    return "426 Connection closed; transfer aborted.\r\n";


            }
        }
        public static string AccesDenied(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "550 Acces refuse ou repertoire inexistant\r\n";

                case Language.en:
                    return "550 Access denied or directory dosen't exist !\r\n";

                default:
                    return "550 Access denied or directory dosen't exist !\r\n";

            }
        }
        public static string PasvCmdSuccess(Language language, string ip)
        {
            switch (language)
            {
                case Language.fr:
                    return "227 Entering Passive Mode (" + ip + ").\r\n";

                case Language.en:
                    return "227 Entering Passive Mode (" + ip + ").\r\n";

                default:
                    return "227 Entering Passive Mode (" + ip + ").\r\n";


            }
        }
        public static string TimeOut(Language language )
        {
            switch (language)
            {
                case Language.fr:
                    return "500  \r\n";

                case Language.en:
                    return "500 Session timeout, OK FTP server signing off\r\n";

                default:
                    return "500 Session timeout, OK FTP server signing off\r\n";


            }
        }
        public static string NOOPOK(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "200  \r\n";

                case Language.en:
                    return "200 OK \r\n";

                default:
                    return "200 OK \r\n";


            }
        }
        public static string DirCreatedOK(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "257  \r\n";

                case Language.en:
                    return "257 Directory Created. \r\n";

                default:
                    return "257 Directory Created. \r\n";
            }
        }
        public static string RNFRFaile(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "350  \r\n";

                case Language.en:
                    return "350 Please specify destination name.\r\n";

                default:
                    return "350 Please specify destination name.\r\n";
            }
        }
        public static string Badsequencecommands(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "503  \r\n";

                case Language.en:
                    return "503 Bad sequence of commands. \r\n";

                default:
                    return "503 Bad sequence of commands. \r\n";
            }
        }
        public static string Errorrenameing(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "550  \r\n";

                case Language.en:
                    return "550 Error renameing directory or file .\r\n";

                default:
                    return "550 Error renameing directory or file .\r\n";
            }
        }
        public static string Directoryrenamed(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "250  \r\n";

                case Language.en:
                    return "250 Directory renamed.\r\n";

                default:
                    return "250 Directory renamed.\r\n";
            }
        }
        public static string badargumentREST(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "554  \r\n";

                case Language.en:
                    return "554 bad argument for REST\r\n";

                default:
                    return "554 bad argument for REST\r\n";
            }
        }
        public static string argumentREST(Language language,int OffSet)
        {
            switch (language)
            {
                case Language.fr:
                    return "350 " + OffSet.ToString()+ " \r\n";

                case Language.en:
                    return "350 Restarting at " + OffSet.ToString() + "\r\n";

                default:
                    return "350 Restarting at " + OffSet.ToString() + "\r\n";
            }
        }
        public static string BadHome(Language language )
        {
            switch (language)
            {
                case Language.fr:
                    return "530 \r\n";

                case Language.en:
                    return "530 Bad Home \r\n";

                default:
                    return "530 Bad Home \r\n";
            }
        }
        public static string opendatafailed(Language language )
        {
            switch (language)
            {
                case Language.fr:
                    return "425 \r\n";

                case Language.en:
                    return "425 Can't open data connection.\r\n";

                default:
                    return "425 Can't open data connection.\r\n";
            }
        }
        public static string Invalidcommand(Language language )
        {
            switch (language)
            {
                case Language.fr:
                    return "502 \r\n";

                case Language.en:
                    return "502 Invalid command  \r\n";

                default:
                    return "502 Invalid command  \r\n";
            }
        }
        public static string Noted(Language language)
        {
            switch (language)
            {
                case Language.fr:
                    return "200 \r\n";

                case Language.en:
                    return "200 Noted\r\n";

                default:
                    return "200 Noted\r\n";
            }
        }
        
    }

    public class FtpServer
    {
        #region 定义
        private TcpListener FTP_Listener = null;	  // Listener 閏outant les tentatives de connection des clients
        private Hashtable m_sessionList = null;	  // Collection contenant toutes les sessions en cours
        private Hashtable m_sessionThreads = null;
        private Thread m_listeningThread = null;	  // Thread repr閟entant la boucle d'閏oute
        private List<FtpSession.Account> accountlist = new List<FtpSession.Account>();
        private bool IsMD5 = false;
        private bool IsChk = false;
        private string SafeKey = "";
        private string footpath = "";
        private bool m_listening = false;  // Le serveur 閏oute-t'il?
        private long m_sessID = 0;
        private string IP = "";
        private int m_Port = 21;
        private string lang = "en";
        private int MaxBadCommand = 30;
        private int m_MaxThreads = 100;
        private int m_SessionIdleTimeOut = 10;
        private int m_CommandIdleTimeOut = 60000;
        private string ErrMsg = "";
        #endregion

        #region 属性

        /// <summary>
        /// 绑定IP
        /// </summary>
        public string IPAddress
        {
            get
            {
                try
                {
                    return IP;
                }
                catch
                {
                    return "ALL";
                }
            }
            set { IP = value; }
        }

        /// <summary>
        /// 显示语言
        /// </summary>
        public Language Language
        {
            get
            {
                switch (lang.ToLower())
                {
                    case "fr":
                        return Language.fr;
                    case "en":
                        return Language.en;
                    default:
                        return Language.en;
                }
            }
            set
            {
                switch (value)
                {
                    case Language.fr:
                        lang = "fr";
                        break;
                    case Language.en:
                        lang = "en";
                        break;
                    default:
                        lang = "en";
                        break;
                }

            }
        }

        /// <summary>
        /// 最大错误数
        /// </summary>
        public int MaxBadCommands
        {
            get
            {
                try
                {
                    return MaxBadCommand;
                }
                catch
                {
                    return 30;
                }
            }
            set
            {
                try
                { MaxBadCommand = value; }
                catch
                { }
            }
        }

        /// <summary>
        /// 连接端口
        /// </summary>
        public int Port
        {
            get
            {
                try
                {
                    return m_Port;
                }
                catch
                {
                    return 21;
                }
            }
            set
            {
                try
                { m_Port = value; }
                catch
                { }
            }
        }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxThreads
        {
            get
            {
                try
                {
                    return m_MaxThreads;
                }
                catch
                {
                    return 100;
                }
            }
            set
            {
                try
                {
                    m_MaxThreads = value;
                }
                catch
                { }
            }
        }

        /// <summary>
        /// 会话超时时间
        /// </summary>
        public int SessionIdleTimeOut
        {
            get
            {
                try
                {
                    return m_SessionIdleTimeOut;
                }
                catch
                {
                    return 10;
                }
            }
            set
            {
                try
                { m_SessionIdleTimeOut = value; }
                catch
                { }
            }
        }

        /// <summary>
        /// 命令超时时间
        /// </summary>
        public int CommandIdleTimeOut
        {
            get
            {
                try
                {
                    return m_CommandIdleTimeOut / 1000;
                }
                catch
                {
                    return 60;
                }
            }
            set
            {
                try
                { m_CommandIdleTimeOut = value * 1000; }
                catch
                { }
            }
        }

        #endregion

        #region 事件
        public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event ConnectionEventHandler ClientConnected;
        public event ConnectionEventHandler ClientDisconected;
        #endregion

        #region 方法

        /// <summary>
        /// 构造
        /// </summary>
        public FtpServer()
        {

        }

        private void Listen()
        {
   
            if (this.IP.ToLower().IndexOf("all") > -1 || this.IP.Trim() == "")
            {
                FTP_Listener = new TcpListener(System.Net.IPAddress.Any, this.m_Port);
            }
            else
            {
                FTP_Listener = new TcpListener(System.Net.IPAddress.Parse(this.IP), this.m_Port);
            }
            FTP_Listener.Start();
            this.m_listening = true;
            while (true)
            {
                //有连接
                if (m_sessionList.Count < this.m_MaxThreads)
                {
                    // 传递线程
                    Socket clientSocket = FTP_Listener.AcceptSocket();
                    if (this.ClientConnected != null)
                    {this.ClientConnected(this, new ConnectionEventArgs(clientSocket.RemoteEndPoint.ToString()));}
                    FtpSession session = new FtpSession(clientSocket, this, this.m_sessID);
                    session.User = accountlist;
                    session.SafeMode(IsMD5, SafeKey, IsChk);
                    if (this.footpath.Trim() != "")
                    { session.CanAnymous(footpath); }
                    Thread clientThread = new Thread(new ThreadStart(session.StartProcessing));
                    this.m_sessionList.Add(m_sessID, session);
                    clientThread.Start();
                    this.m_sessionThreads.Add(m_sessID, clientThread);
                    m_sessID++;
                }
                else
                {Thread.Sleep(100);}
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            try
            {
                //Si le serveur n'閏oute pas d閖?..
                if (!this.m_listening)
                {
                    //on r閕nitialise la collection des sessions en cours.
                    this.m_sessionList = new Hashtable();
                    //ainsi que celle des threads de session
                    this.m_sessionThreads = new Hashtable();
                    //on initialise le thread
                    this.m_listeningThread = new Thread(new ThreadStart(Listen));
                    //et on d閙arre la boucle
                    this.m_listeningThread.Start();
                    //On d閏lenche aussi l'関鑞ement "Started"
                    if (this.Started != null)
                    {
                        Started(this, EventArgs.Empty);
                    }
                }

            }
            catch(Exception ex)
            { ErrMsg = ex.Message; }
        }


        /// <summary>
        ///停止
        /// </summary>
        public void Stop()
        {
            try
            {
                //on ne stop le thread que si il est en train de tourner
                if (this.m_listening)
                {
                    //on stop le thread
                    this.m_listeningThread.Abort();
                    //on change la variable en cons閝uence
                    this.m_listening = false;
                    //on stop le listener
                    FTP_Listener.Stop();
                    //et on le d閠ruit
                    FTP_Listener = null;
                    //On arr阾e aussi toutes les sessions en cours
                    for (int i = 0; i < m_sessID; i++)
                    {
                        this.RemoveSession(i);
                    }
                    //on d閏lenche l'関鑞ement Stopped
                    if (this.Stopped != null)
                    {
                        Stopped(this, EventArgs.Empty);
                    }
                }

            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }
        }

        /// <summary>
        /// 获取运行状态
        /// </summary>
        /// <returns>运行状态</returns>
        public bool Stat()
        { return this.m_listening; }

        /// <summary>
        ///用户退出
        /// </summary>
        /// <param name="session">Session ?supprimer</param>
        public void RemoveSession(long sessID)
        {

            try
            {
                //Evenement ClientDisconnected
                if (this.ClientDisconected != null) this.ClientDisconected(this, new ConnectionEventArgs(((FtpSession)this.m_sessionList[sessID]).RemoteEndPoint.ToString()));

                //Arr阾 du Thread correspondant
                ((Thread)this.m_sessionThreads[sessID]).Abort();
                //suppression du thread
                this.m_sessionThreads.Remove(sessID);
                //Cloture de la session
                ((FtpSession)this.m_sessionList[sessID]).Close();
                //suppression de la session
                this.m_sessionList[sessID] = null;
                this.m_sessionList.Remove(sessID);

            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }

        }

        /// <summary>
        /// 添加账号信息
        /// </summary>
        /// <param name="UserName">账号名</param>
        /// <param name="PassWord">密码</param>
        /// <param name="folder">目录</param>
        public void AddUser(string UserName, string PassWord, string folder)
        {
            if (UserName.Trim() != "")
            {
                for (int i = 0; i < accountlist.Count; i++)
                {
                    if (accountlist[i].UserName.ToLower().Trim() == UserName.ToLower().Trim())
                    { return; }
                }
                FtpSession.Account acc=new FtpSession.Account();
                acc.UserName = UserName;
                acc.Password = PassWord;
                acc.RootDir = folder;
                accountlist.Add(acc);
            }  
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="UserName">账号名</param>
        public void DeleUser(string UserName)
        {
            if (UserName.Trim() != "")
            {
                for (int i = 0; i < accountlist.Count; i++)
                {
                    if (accountlist[i].UserName.ToLower().Trim() == UserName.ToLower().Trim())
                    { accountlist.Remove(accountlist[i]); }
                }
            }  
        }

        /// <summary>
        /// 获取账号信息
        /// </summary>
        /// <returns>账号名</returns>
        public string[] UserList()
        {
            if (accountlist.Count > 0)
            {
                string[] res=new string[accountlist.Count];
                for (int i = 0; i < accountlist.Count; i++)
                { res[i] = accountlist[i].UserName; }
                return res;
            }
            else
            { return null; }
        }

        /// <summary>
        /// 获取当前用户会话ID
        /// </summary>
        /// <param name="UserName"账号名></param>
        /// <returns>用户会话ID</returns>
        public long UserSessionID(string UserName)
        {
            if (UserName.Trim() != "")
            {
                for (int i = 0; i < accountlist.Count; i++)
                {
                    if (accountlist[i].UserName.ToLower().Trim() == UserName.ToLower().Trim())
                    { return accountlist[i].SessionID; }
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取当前在线账号
        /// </summary>
        /// <returns>账号名</returns>
        public string[] OnlineUser()
        {
            if (accountlist.Count > 0)
            {
                List<string> pack = new List<string>();
                for (int i = 0; i < accountlist.Count; i++)
                {
                    if (accountlist[i].SessionID > 0)
                    { pack.Add(accountlist[i].UserName ); }
                }
                if (pack.Count > 0)
                {
                    string[] res = new string[pack.Count];
                    pack.CopyTo(res);
                    return res;
                }
                else
                { return null; }
            }
            else
            { return null; }
        }

        /// <summary>
        /// 开启安全模式
        /// </summary>
        /// <param name="Key">密钥</param>
        /// <param name="SourceSafe">是否对源加密</param>
        public void SafeModeOn(string Key,  bool SourceSafe)
        {
            IsMD5 = true;
            IsChk = SourceSafe;
            SafeKey = Key;
        }

        /// <summary>
        /// 关闭安全模式
        /// </summary>
        public void SafeModeOff()
        { IsMD5 = false; }

        /// <summary>
        /// 使用匿名模式
        /// </summary>
        /// <param name="RootPath"></param>
        public void UseAnyonemous(string RootPath)
        { 
             try
             {
                 footpath = "";
                 DirectoryInfo di = new DirectoryInfo(RootPath);
                 if(di.Exists)
                 {RootPath=di.FullName ;}
             }
             catch
              { footpath = ""; }
        
        }

        /// <summary>
        /// 禁用匿名模式
        /// </summary>
        public void disableAnyonemous()
        { footpath = ""; }

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
    }

}
