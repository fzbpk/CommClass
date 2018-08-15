using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace System.Drawing.MediaFactory
{
    /// <summary>
    /// 截屏,FORM模式下使用
    /// </summary>
    public class ScreenCapture
    {

        /// <summary>
        /// 获取屏幕数量
        /// </summary>
        /// <returns></returns>
        public static int ScreenCount ()
        {
            try
            {
                return Screen.AllScreens.Length;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// 屏幕大小
        /// </summary>
        /// <param name="ScreenIndex">屏幕索引</param>
        /// <returns>屏幕大小</returns>
        public static Size ScreenSize(int ScreenIndex = 0)
        {
            try
            {
                Screen choScreen = null;
                if (ScreenIndex == 0)
                    choScreen = Screen.PrimaryScreen;
                if (ScreenIndex > Screen.AllScreens.Length)
                    choScreen = Screen.PrimaryScreen;
                else
                    choScreen = Screen.AllScreens[ScreenIndex];
               return new Size(choScreen.Bounds.Width, choScreen.Bounds.Height);
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// 屏幕起始点
        /// </summary>
        /// <param name="ScreenIndex">屏幕索引</param>
        /// <returns>起始点</returns>
        public static Point ScreenPoint(int ScreenIndex = 0)
        {
            try
            {
                Screen choScreen = null;
                if (ScreenIndex == 0)
                    choScreen = Screen.PrimaryScreen;
                if (ScreenIndex > Screen.AllScreens.Length)
                    choScreen = Screen.PrimaryScreen;
                else
                    choScreen = Screen.AllScreens[ScreenIndex];
                return new Point(choScreen.Bounds.X, choScreen.Bounds.Y);
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// 全屏
        /// </summary>
        /// <returns>影像</returns>
        public static Image FullScreen(int ScreenIndex=0)
        {
            try
            {
                Screen choScreen = null;
                if (ScreenIndex == 0)
                    choScreen = Screen.PrimaryScreen;
                if(ScreenIndex>Screen.AllScreens.Length)
                    choScreen = Screen.PrimaryScreen;
                else
                    choScreen = Screen.AllScreens[ScreenIndex];
                Bitmap image = new Bitmap(choScreen.Bounds.Width, choScreen.Bounds.Height);
                Graphics imgGraphics = Graphics.FromImage(image);
                imgGraphics.CopyFromScreen(choScreen.Bounds.X, choScreen.Bounds.Y, 0, 0, new Size(choScreen.Bounds.Width, choScreen.Bounds.Height));
                imgGraphics.Dispose();
                return image;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// 抓取屏幕(层叠的窗口)
        /// </summary>
        /// <param name="x">左上角的横坐标</param>
        /// <param name="y">左上角的纵坐标</param>
        /// <param name="width">抓取宽度</param>
        /// <param name="height">抓取高度</param>
        /// <returns>影像</returns>
        public static Image FixScreen(int x, int y, int width, int height)
        {
            try
            {
                Bitmap image = new Bitmap(width, height);
                Graphics imgGraphics = Graphics.FromImage(image);
                imgGraphics.CopyFromScreen(new Point(x, y), new Point(0, 0), image.Size);
                imgGraphics.Dispose();
                return image;
            }
            catch (Exception ex)
            { throw ex; }
        }
 
    }
}
