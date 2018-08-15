using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.MediaFactory;
namespace CommClassService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Image img = ScreenCapture.FullScreen();
            string ss = img.ImageToBytes(ImageType.Jpeg).ToBase64();
            img.ImageToFile("H:\\临时文件\\xx.jpg");
        }

        protected override void OnStop()
        {
            this.ExitCode = 0;
        }
    }
}
