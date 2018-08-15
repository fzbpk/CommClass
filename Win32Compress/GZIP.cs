using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
namespace Win32Compress
{
   public  class GZIP
    {

       public struct EntryContent
       {
           public string Name;
           public bool IsFile;
           public bool IsDirectory;
           public long CompressedSize;
           public long Size;
           public System.DateTime DateTime;
           public long Crc;
       }

        private static string ErrMsg = "";

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public static string Err
        { get { return ErrMsg; } }

        private static Boolean IsMatch(String input, String pattern)
        {
            Boolean matched = false;
            Int32 inputIndex = 0;
            Int32 patternIndex = 0;
            //无通配符 * 时，比较算法（）
            while (inputIndex < input.Length && patternIndex < pattern.Length && (pattern[patternIndex] != '*'))
            {
                if ((pattern[patternIndex] != '?') && (input[inputIndex] != pattern[patternIndex]))
                {//如果模式字符不是通配符，且输入字符与模式字符不相等，则可判定整个输入字串与模式不匹配
                    return matched;
                }
                patternIndex++;
                inputIndex++;
                if (patternIndex == pattern.Length && inputIndex < input.Length)
                {
                    return matched;
                }
                if (inputIndex == input.Length && patternIndex < pattern.Length)
                {
                    return matched;
                }
                if (patternIndex == pattern.Length && inputIndex == input.Length)
                {
                    matched = true;
                    return matched;
                }
            }
            //有通配符 * 时，比较算法
            Int32 mp = 0;
            Int32 cp = 0;
            while (inputIndex < input.Length)
            {
                if (patternIndex < pattern.Length && pattern[patternIndex] == '*')
                {
                    if (++patternIndex >= pattern.Length)
                    {
                        matched = true;
                        return matched;
                    }
                    mp = patternIndex;
                    cp = inputIndex + 1;
                }
                else if (patternIndex < pattern.Length && ((pattern[patternIndex] == input[inputIndex]) || (pattern[patternIndex] == '?')))
                {
                    patternIndex++;
                    inputIndex++;
                }
                else
                {
                    patternIndex = mp;
                    inputIndex = cp++;
                }
            }
            //当输入字符为空且模式为*时
            while (patternIndex < pattern.Length && pattern[patternIndex] == '*')
            {
                patternIndex++;
            }
            return patternIndex >= pattern.Length ? true : false;

        } 

        private static bool ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName)
        {
            bool res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            try
            {

                //创建当前文件夹
                entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/")); //加上 “/” 才会当成是文件夹创建
                s.PutNextEntry(entry);
                s.Flush();


                //先压缩文件，再递归压缩文件夹 
                filenames = Directory.GetFiles(FolderToZip);
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/" + Path.GetFileName(file)));

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }


            folders = Directory.GetDirectories(FolderToZip);
            foreach (string folder in folders)
            {
                if (!ZipFileDictory(folder, s, Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip))))
                {
                    return false;
                }
            }

            return res;
        }

       /// <summary>
       /// 压缩文件夹
       /// </summary>
       /// <param name="OrgPath">源文件夹</param>
       /// <param name="DestPath">目标文件夹</param>
       /// <param name="FileName">文件名</param>
       /// <param name="Password">密码</param>
       /// <returns>执行结果</returns>
        public static bool ZipPath(string OrgPath, string DestPath, string FileName = "", string Password="")
        {
            ErrMsg = "";
            bool res;
            try
            {
                if (!Directory.Exists(OrgPath) || !Directory.Exists(DestPath))
                { return false; }
                if (DestPath.EndsWith("\\"))
                { DestPath = DestPath.Substring(0, DestPath.LastIndexOf("\\")); }
                if (FileName == null)
                { FileName = OrgPath.Substring(OrgPath.LastIndexOf("\\") + 1) + ".zip"; }
                else if (FileName.Trim() == "")
                { FileName = OrgPath.Substring(OrgPath.LastIndexOf("\\") + 1) + ".zip"; }
                if (File.Exists(DestPath + "\\" + FileName))
                { File.Delete(DestPath + "\\" + FileName); }
                ZipOutputStream s = new ZipOutputStream(File.Create(DestPath + "\\" + FileName));
                s.SetLevel(6);
                s.Password = Password;
                res = ZipFileDictory(OrgPath, s, "");
                s.Finish();
                s.Close();
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                res = false;
            }
            return res;
        }

       /// <summary>
       /// 压缩当前文件夹
       /// </summary>
        /// <param name="OrgPath">源文件夹</param>
        /// <param name="DestPath">目标文件夹</param>
        /// <param name="FileName">文件名</param>
        /// <param name="Password">密码</param>
        /// <returns>执行结果</returns>
        public static bool ZipCurPath(string OrgPath, string DestPath, string FileName = "", string Password = "")
        {
            ErrMsg = "";
            bool res;
            try
            {
                if (!Directory.Exists(OrgPath) || !Directory.Exists(DestPath))
                { return false; }
                if (DestPath.EndsWith("\\"))
                { DestPath = DestPath.Substring(0, DestPath.LastIndexOf("\\")); }
                if (FileName == null)
                { FileName = OrgPath.Substring(OrgPath.LastIndexOf("\\") + 1) + ".zip"; }
                else if (FileName.Trim() == "")
                { FileName = OrgPath.Substring(OrgPath.LastIndexOf("\\") + 1) + ".zip"; }
                if (File.Exists(DestPath + "\\" + FileName))
                { File.Delete(DestPath + "\\" + FileName); }
                ZipOutputStream s = new ZipOutputStream(File.Create(DestPath + "\\" + FileName));
                s.SetLevel(6);
                s.Password = Password;
                Crc32 crc = new Crc32();
                string[] filenames = Directory.GetFiles(OrgPath);
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    FileStream fs =  File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(Path.Combine("",  Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
                filenames = Directory.GetDirectories(OrgPath);
                foreach (string dir in filenames)
                { ZipFileDictory( dir, s, ""); }
                s.Finish();
                s.Close();
                res = true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                res = false;
            }
            return res;
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="FileToZip">源文件</param>
        /// <param name="ZipedFile">目标文件</param>
        /// <param name="Password">密码</param>
        /// <returns>执行结果</returns>
        public static bool ZipFile(string FileToZip, string ZipedFile, String Password)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + FileToZip + " 不存在!");
            }
            //FileStream fs = null;
            FileStream ZipFile = null;
            ZipOutputStream ZipStream = null;
            ZipEntry ZipEntry = null;

            bool res = true;
            try
            {
                ZipFile = File.OpenRead(FileToZip);
                byte[] buffer = new byte[ZipFile.Length];
                ZipFile.Read(buffer, 0, buffer.Length);
                ZipFile.Close();

                ZipFile = File.Create(ZipedFile);
                ZipStream = new ZipOutputStream(ZipFile);
                ZipStream.Password = Password;
                ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                ZipStream.PutNextEntry(ZipEntry);
                ZipStream.SetLevel(6);

                ZipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (ZipEntry != null)
                {
                    ZipEntry = null;
                }
                if (ZipStream != null)
                {
                    ZipStream.Finish();
                    ZipStream.Close();
                }
                if (ZipFile != null)
                {
                    ZipFile.Close();
                    ZipFile = null;
                }
                GC.Collect();
                GC.Collect(1);
            }

            return res;
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="OrgFile">ZIP文件</param>
        /// <param name="DestPath">目标文件夹</param>
        /// <param name="Password">密码</param>
        /// <returns>执行结果</returns>
        public static bool UnZip(string OrgFile, string DestPath, string Password = "")
        {
            try
            {
                if (!File.Exists(OrgFile))
                {  return false ;}
                if (!Directory.Exists(DestPath))
                { Directory.CreateDirectory(DestPath); }
                ZipInputStream s = null;
                ZipEntry theEntry = null;
                string fileName;
                FileStream streamWriter = null;
                try
                {
                    s = new ZipInputStream(File.OpenRead(OrgFile));
                    s.Password = Password;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        if (theEntry.Name != String.Empty)
                        {
                            fileName = Path.Combine(DestPath, theEntry.Name);
                            ///判断文件路径是否是文件夹
                            if (fileName.EndsWith("/") || fileName.EndsWith("\\"))
                            {
                                Directory.CreateDirectory(fileName);
                                continue;
                            }
                            streamWriter = File.Create(fileName);
                            if (theEntry.Size != 0)
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    { streamWriter.Write(data, 0, size); }
                                    else
                                    { break; }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                        streamWriter = null;
                    }
                    if (theEntry != null)
                    {  theEntry = null;}
                    if (s != null)
                    {
                        s.Close();
                        s = null;
                    }
                    GC.Collect();
                    GC.Collect(1);
                    System.Threading.Thread.Sleep(1 * 1000);
                }
                return true;
            }
            catch(Exception ex)
            {
              ErrMsg =ex.Message ;
              return false ;
            }
           
        }

        /// <summary>
        /// 解压缩单个文件
        /// </summary>
        /// <param name="OrgFile">zip文件</param>
        /// <param name="DestPath">目标文件夹</param>
        /// <param name="Entry">要解压的文件信息</param>
        /// <param name="Password">密码</param>
        /// <returns></returns>
        public static bool UnZipFile(string OrgFile, string DestPath, EntryContent Entry, string Password = "")
        {
            try
            {
                if (!File.Exists(OrgFile))
                { return false; }
                if (Entry.Name == null)
                { return false; }
                else if (Entry.Name.Trim() == "")
                { return false; }
                else if (!Entry.IsFile)
                { return false; }
                if (!Directory.Exists(DestPath))
                { Directory.CreateDirectory(DestPath); }
                ZipInputStream s = null;
                ZipEntry theEntry = null;
                string fileName;
                FileStream streamWriter = null;
                try
                {
                    s = new ZipInputStream(File.OpenRead(OrgFile));
                    s.Password = Password;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        if (theEntry.Name != String.Empty)
                        {
                            if (theEntry.Name == Entry.Name && theEntry.IsFile && theEntry.Size !=0)
                            {
                                fileName = Path.Combine(DestPath, theEntry.Name);
                                FileInfo fi = new FileInfo(fileName);
                                if (!System.IO.Directory.Exists(fi.DirectoryName))
                                { System.IO.Directory.CreateDirectory(fi.DirectoryName); }
                                streamWriter = File.Create(fileName);
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    { streamWriter.Write(data, 0, size); }
                                    else
                                    { break; }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                        streamWriter = null;
                    }
                    if (theEntry != null)
                    {
                        theEntry = null;
                    }
                    if (s != null)
                    {
                        s.Close();
                        s = null;
                    }
                    GC.Collect();
                    GC.Collect(1);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取ZIP文件下所有文件信息
        /// </summary>
        /// <param name="OrgFile">zip文件</param>
        /// <param name="Password">密码</param>
        /// <param name="Key">搜索条件</param>
        /// <returns>文件信息</returns>
        public static EntryContent[] ZipContent(string OrgFile, string Password = "", string Key="")
        {
            List<ZipEntry> EntryList = new List<ZipEntry>();
            try
            {
                if (File.Exists(OrgFile))
                {
                    ZipInputStream  s = new ZipInputStream(File.OpenRead(OrgFile));
                    s.Password = Password;
                    ZipEntry theEntry = null;
                    try
                    {
                        while ((theEntry = s.GetNextEntry()) != null)
                        {
                            if (theEntry.Name != String.Empty && theEntry.Name!=null)
                            {
                                if (Key.Trim() != "")
                                {
                                    if (IsMatch(theEntry.Name, Key))
                                    { EntryList.Add(theEntry); }

                                }
                                else
                                { EntryList.Add(theEntry); }
                            }
                        }
                    }
                    finally
                    {
                        if (s != null)
                        {
                            s.Close();
                            s = null;
                        }
                        GC.Collect();
                        GC.Collect(1);
                    }
                }
            }
            catch (Exception ex)
            {ErrMsg = ex.Message;}
            if (EntryList.Count > 0)
            {
                EntryContent[] res = new EntryContent[EntryList.Count];
                int index = 0;
                foreach (ZipEntry Entry in EntryList)
                {
                    res[index] = new EntryContent();
                    res[index].Name = Entry.Name;
                    res[index].Size = Entry.Size;
                    res[index].CompressedSize = Entry.CompressedSize;
                    res[index].IsFile = Entry.IsFile;
                    res[index].IsDirectory = Entry.IsDirectory;
                    res[index].DateTime = Entry.DateTime;
                    res[index].Crc = Entry.Crc;
                    index++;
                }
                return res;
            }
            else
            { return null; }
        }

        /// <summary>
        /// 获取ZIP文件下文件信息
        /// </summary>
        /// <param name="OrgFile">zip文件</param>
        /// <param name="Password">密码</param>
        /// <param name="Key">搜索条件</param>
        /// <returns>文件信息</returns>
        public static string[]  ZipFileList(string OrgFile, string Password = "",string Key="")
        {
            List<string> EntryList = new List<string>();
            try
            {
                if (File.Exists(OrgFile))
                {
                    ZipInputStream s = new ZipInputStream(File.OpenRead(OrgFile));
                    s.Password = Password;
                    ZipEntry theEntry = null;
                    try
                    {
                        while ((theEntry = s.GetNextEntry()) != null)
                        {
                            if (theEntry.Name != String.Empty && theEntry.Name != null)
                            {
                                if (Key.Trim() != "")
                                {
                                    if (IsMatch(theEntry.Name, Key))
                                    { EntryList.Add(theEntry.Name); }
                                }
                                else
                                { EntryList.Add(theEntry.Name); }
                            }
                        }
                    }
                    finally
                    {
                        if (s != null)
                        {
                            s.Close();
                            s = null;
                        }
                        GC.Collect();
                        GC.Collect(1);
                    }
                }
            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }
            if (EntryList.Count > 0)
            {
                string[] res = new string[EntryList.Count];
                EntryList.CopyTo(res);
                return res;
            }
            else
            { return null; }
        }

        /// <summary>
        /// 获取ZIP文件下所有目录信息
        /// </summary>
        /// <param name="OrgFile">zip文件</param>
        /// <param name="Password">密码</param>
        /// <param name="Key">搜索条件</param>
        /// <returns>目录信息</returns>
        public static string[] ZipDirectoryList(string OrgFile, string Password = "",string Key="")
        {
            List<string> EntryList = new List<string>();
            try
            {
                if (File.Exists(OrgFile))
                {
                    ZipInputStream s = new ZipInputStream(File.OpenRead(OrgFile));
                    s.Password = Password;
                    ZipEntry theEntry = null;
                    try
                    {
                        while ((theEntry = s.GetNextEntry()) != null)
                        {
                            if (theEntry.Name != String.Empty && theEntry.Name != null)
                            {
                                if (theEntry.IsDirectory)
                                {
                                    if (Key.Trim() != "")
                                    {
                                        if(IsMatch(theEntry.Name ,Key))
                                        { EntryList.Add(theEntry.Name); }

                                    }
                                    else
                                    { EntryList.Add(theEntry.Name); }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (s != null)
                        {
                            s.Close();
                            s = null;
                        }
                        GC.Collect();
                        GC.Collect(1);
                    }
                }
            }
            catch (Exception ex)
            { ErrMsg = ex.Message; }
            if (EntryList.Count > 0)
            {
                string[] res = new string[EntryList.Count];
                EntryList.CopyTo(res);
                return res;
            }
            else
            { return null; }
        }
 
    }

}
