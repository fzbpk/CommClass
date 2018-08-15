using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace System.Drawing.MediaFactory
{
    /// <summary>
    /// 影像与文件相转换
    /// </summary>
    public static  class ImageFile
    {
        /// <summary>
        /// 影像转流
        /// </summary>
        /// <param name="image">影像</param>
        /// <param name="type">类型</param>
        /// <returns>流</returns>
        public static MemoryStream ImageToMemoryStream(this Image image, ImageType type = ImageType.Png)
        {
            if (image == null)
                return null;
            MemoryStream data = new MemoryStream();
            switch (type)
            { 
                case ImageType.Png :
                     image.Save(data, ImageFormat.Png);
                     return data;
                case ImageType.Bmp :
                     image.Save(data, ImageFormat.Bmp);
                     return data;
                case ImageType.Emf :
                     image.Save(data, ImageFormat.Emf);
                     return data;
                case ImageType.Exif:
                     image.Save(data, ImageFormat.Exif);
                     return data;
                case ImageType.Gif:
                     image.Save(data, ImageFormat.Gif);
                     return data;
                case ImageType.Icon:
                     image.Save(data, ImageFormat.Icon);
                     return data;
                case ImageType.Jpeg:
                     image.Save(data, ImageFormat.Jpeg);
                     return data;
                case ImageType.MemoryBmp:
                     image.Save(data, ImageFormat.MemoryBmp);
                     return data;
                case ImageType.Tiff:
                     image.Save(data, ImageFormat.Tiff);
                     return data;
                case ImageType.Wmf:
                     image.Save(data, ImageFormat.Wmf);
                     return data;
                default :
                     return null;
            }
        }

        /// <summary>
        /// 影像转数组
        /// </summary>
        /// <param name="image">影像</param>
        /// <param name="type">类型</param>
        /// <returns>数组</returns>
        public static byte[] ImageToBytes(this Image image, ImageType type = ImageType.Png)
        {
            if (image == null)
                return null;
            MemoryStream data = new MemoryStream();
            byte[] byteImage = null;
            switch (type)
            {
                case ImageType.Png:
                    image.Save(data, ImageFormat.Png);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Bmp:
                    image.Save(data, ImageFormat.Bmp);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Emf:
                    image.Save(data, ImageFormat.Emf);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Exif:
                    image.Save(data, ImageFormat.Exif);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Gif:
                    image.Save(data, ImageFormat.Gif);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Icon:
                    image.Save(data, ImageFormat.Icon);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Jpeg:
                    image.Save(data, ImageFormat.Jpeg);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.MemoryBmp:
                    image.Save(data, ImageFormat.MemoryBmp);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Tiff:
                    image.Save(data, ImageFormat.Tiff);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                case ImageType.Wmf:
                    image.Save(data, ImageFormat.Wmf);
                    byteImage = new Byte[data.Length];
                    byteImage = data.ToArray();
                    return byteImage;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 影像转文件
        /// </summary>
        /// <param name="image">影像</param>
        /// <param name="FilePath">文件路径</param>
        public static void  ImageToFile(this Image image, string FilePath)
        {
            if (image == null || string.IsNullOrEmpty(FilePath))
                return ;
            string extension = Path.GetExtension(FilePath).ToLower();
            switch (extension)
            { 
                case ".png":
                case ".pns":
                    image.Save(FilePath, ImageFormat.Png);
                    break; 
                case ".bmp":
                    image.Save(FilePath, ImageFormat.Bmp);
                    break;
                case ".emf":
                    image.Save(FilePath, ImageFormat.Emf);
                    break;
                case ".exif":
                    image.Save(FilePath, ImageFormat.Exif);
                    break;
                case ".gif":
                    image.Save(FilePath, ImageFormat.Gif);
                    break;
                case ".ico":
                    image.Save(FilePath, ImageFormat.Icon);
                    break;
                case ".jpeg":
                case ".jpe":
                case ".jpg":
                    image.Save(FilePath, ImageFormat.Jpeg);
                    break;
                case ".tif":
                case ".tiff":
                    image.Save(FilePath, ImageFormat.Tiff);
                    break;
                case ".wmf":
                    image.Save(FilePath, ImageFormat.Wmf);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 流转影像
        /// </summary>
        /// <param name="Stream">流，注意用完关闭</param>
        /// <returns>影像</returns>
        public static Image ImageFromMemoryStream(this MemoryStream Stream)
        {
            try
            {
                Image img = Image.FromStream(Stream);
                return img;  
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// 数组转影像
        /// </summary>
        /// <param name="buffer">数组</param>
        /// <returns>影像</returns>
        public static Image ImageFromBytes(this byte[] buffer)
        {
            try
            {
                MemoryStream ms = new MemoryStream(buffer);
                Image img = Image.FromStream(ms);
                ms.Close();
                return img;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// 文件转影像
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns>影像</returns>
        public static Image ImageFromFile(this string FilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath))
                    return null;
                FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                int byteLength = (int)fileStream.Length;
                byte[] buffer = new byte[byteLength];
                fileStream.Read(buffer, 0, byteLength);
                fileStream.Close();
                MemoryStream ms = new MemoryStream(buffer);
                Image img = Image.FromStream(ms);
                ms.Close();
                return img;
            }
            catch (Exception ex)
            { throw ex; }
        }


    }
}
