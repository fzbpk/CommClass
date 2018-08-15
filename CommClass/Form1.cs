using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.MediaFactory;
using System.Communicate;
using System.Text.RegularExpressions; 
namespace CommClass
{
    public partial class Form1 : Form
    {
        SocketSDK sock = new SocketSDK();


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sock = new SocketSDK();
            sock.PingCheck = true;
            sock.SetIPAddress("127.0.0.1", 5000);
            sock.Open();
           
           // sock.ASyncStopWrite();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            sock.SetIPAddress("", 5000);
         

        }

        private void button3_Click(object sender, EventArgs e)
        {
          //  sock.ASyncStopSend();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sock.ASyncSendBytes(new byte[] { 0x65, 0x66, 0x0d, 0x0a });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            sock.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            byte[] buf = null;
            int len = sock.Receive(out buf);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            sock.SendBytes(new byte[] { 0x65, 0x66, 0x0d, 0x0a });
        }

        private void button8_Click(object sender, EventArgs e)
        {
           // DataDriver.SQLite db = new DataDriver.SQLite("123",60);
            //db.TableIsExist("");
            System.Threading.Thread th1 = new System.Threading.Thread(msgxx);
            th1.Start();
        }

        void msgxx()
        {

            try
            {
                SocketSDK sock = new SocketSDK();
                sock.ReceiveBufferSize = 1024;
                sock.SendBufferSize = 1024;
                sock.ReceiveTimeout = 1000;
                sock.SendTimeout = 1000;
                sock.SetIPAddress("", 5000);
                sock.KeepAlive = true;
                sock.MaxConnect = 150;
                sock.Bind();
                if (sock.IsBound)
                {
                    while (true)
                    {
                        try
                        {
                            testcall call = new testcall();
                            call .sock= sock.Accept();
                            System.Threading.Thread th1 = new System.Threading.Thread(call.main );
                            th1.Start();
                        }
                        catch (Exception exx)
                        {
                            
                        }
                    }
                }
                else
                {
                    
                }
                sock.Close();
            }
            catch (Exception ex)
            {
               
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Image img = ScreenCapture.FullScreen(0);
            string ss = img.ImageToBytes(ImageType.Jpeg).ToBase64();
            MessageBox.Show(ss);
            img.ImageToFile("H:\\临时文件\\xx.jpg");
        }

        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("///Date({0}+0800)///", ts.TotalMilliseconds);
            return result;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string res = System.ClassTransform.JSON.ToJson<DateTime>(DateTime.Now);
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            res = reg.Replace(res, matchEvaluator);  
        }

    }

    public class testcall
    {
        public SocketSDK sock = null;
        public void main()
        {
            while (true)
            {
                byte[] recv = null;
                int len = sock.Available;
                if (len > 0)
                {
                    len=sock.Receive(out recv);
                    int data = recv.Length;
                }
            }
        }
    }

}
