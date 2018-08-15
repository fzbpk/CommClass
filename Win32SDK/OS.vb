Imports System.Management
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.IO
Imports Microsoft.Win32
Public Class OS

    Private Shared ErrMsg As String = ""

    Public Const WindowsLog As String = "System"
    Public Const AppLog As String = "Application"
    Public Const SafeLog As String = "Security"

    Public Enum WinsVers As Byte
        Windows95 = 1
        Windows98 = 2
        Windows982ndEdition = 3
        WindowsME = 4
        WindowsNT351 = 5
        WindowsNT40 = 6
        WindowsCE = 7
        Windows2000 = 8
        WindowsXP = 9
        Windows2003 = 10
        WindowsVista = 11
        Windows2008 = 12
        Windows7 = 13
        UnKnown = 0
    End Enum

    Public Enum DiskFormatType As Byte
        FAT = 1
        FAT32 = 2
        NTFS = 3
        None = 0
    End Enum

    Private Structure MEMORYSTATUS
        Public dwLength As UInt32
        Public dwMemoryLoad As UInt32
        Public dwTotalPhys As UInt32
        Public dwAvailPhys As UInt32
        Public dwTotalPageFile As UInt32
        Public dwAvailPageFile As UInt32
        Public dwTotalVirtual As UInt32
        Public dwAvailVirtual As UInt32
    End Structure

    Private Declare Function GlobalMemoryStatus Lib "Kernel32.Dll" (ByRef ms As MEMORYSTATUS) As Integer
    Private Declare Function LoadLibrary Lib "kernel32" Alias "LoadLibraryA" (ByVal lpLibFileName As String) As IntPtr
    Private Declare Function FreeLibrary Lib "kernel32" (ByVal hLibModule As IntPtr) As Integer
    Private Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As IntPtr, ByVal lpProcName As String) As IntPtr
    Private Declare Function GetDiskFreeSpaceEx Lib "kernel32" Alias "GetDiskFreeSpaceExA" (ByVal lpDirectoryName As String, ByRef lpFreeBytesAvailableToCaller As Long, ByRef lpTotalNumberOfBytes As Long, ByRef lpTotalNumberOfFreeBytes As Long) As Long
    Private Declare Function SHFormatDrive Lib "shell32" (ByVal Hend As IntPtr, ByVal Drive As Integer, ByVal FormatID As Integer, ByVal Options As Integer) As Integer

    Private Shared Function CheckEntryPoint(ByVal library As String, ByVal method As String) As Boolean
        Try
            Dim libPtr As IntPtr = LoadLibrary(library)
            If Not libPtr.Equals(IntPtr.Zero) Then
                If Not GetProcAddress(libPtr, method).Equals(IntPtr.Zero) Then
                    FreeLibrary(libPtr)
                    Return True
                End If
                FreeLibrary(libPtr)
            End If
            Return False
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

    <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Public Structure SYSEventLog
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
        Public Category As String
        Public CategoryNumber As Short
        Public EntryType As Integer
        Public Index As Integer
        Public InstanceId As Long
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public MachineName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=512)>
        Public Message As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public Source As String
        Public TimeGenerated As Date
        Public TimeWritten As Date
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public UserName As String
    End Structure

    ''' <summary>
    ''' 获取错误
    ''' </summary>
    ''' <value></value>
    ''' <returns>错误信息</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ReadError() As String
        Get
            Return ErrMsg
        End Get
    End Property

    ''' <summary>
    ''' 清除错误
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ClearError()
        ErrMsg = ""
    End Sub

    ''' <summary>
    ''' 获取当前系统版本
    ''' </summary>
    ''' <value></value>
    ''' <returns>操作系统版本</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property CurrentPlatform() As WinsVers
        Get
            Try
                '定义。
                Dim os As OperatingSystem = Environment.OSVersion
                '定义返回值。
                Dim Vers As New WinsVers
                '开始判断操作系统平台。
                Select Case os.Platform
                    Case PlatformID.Win32Windows    'WIN95、98、ME
                        '次要版本号判断操作系统。
                        Select Case os.Version.Minor
                            Case 0 '95
                                Vers = WinsVers.Windows95
                            Case 10 '98
                                If os.Version.Revision.ToString = "2222A" Then
                                    Vers = WinsVers.Windows982ndEdition
                                Else
                                    Vers = WinsVers.Windows98
                                End If
                            Case 90 'ME
                                Vers = WinsVers.WindowsME
                            Case Else '其它
                                Vers = WinsVers.UnKnown
                        End Select

                    Case PlatformID.Win32NT         'WIN2K、XP、2003、Vista、2008、Win7
                        Select Case os.Version.Major
                            Case 3
                                Vers = WinsVers.WindowsNT351
                            Case 4
                                Vers = WinsVers.WindowsNT40
                            Case 5
                                Select Case os.Version.Minor
                                    Case 0
                                        Vers = WinsVers.Windows2000
                                    Case 1
                                        Vers = WinsVers.WindowsXP
                                    Case 2
                                        Vers = WinsVers.Windows2003
                                    Case Else
                                        Vers = WinsVers.UnKnown
                                End Select
                            Case 6
                                Select Case os.Version.Minor
                                    Case 0
                                        Vers = WinsVers.WindowsVista
                                    Case 1
                                        Vers = WinsVers.Windows7
                                    Case Else
                                        Vers = WinsVers.UnKnown
                                End Select
                        End Select

                    Case PlatformID.WinCE           'WinCE
                        Vers = WinsVers.WindowsCE

                    Case Else
                        Vers = WinsVers.UnKnown
                End Select
                '返回值。
                Return Vers
            Catch ex As Exception
                ErrMsg = ex.Message
                Return WinsVers.UnKnown
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取计算机名
    ''' </summary>
    ''' <value></value>
    ''' <returns>计算机名</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property MachineName() As String
        Get
            Try
                Return Environment.MachineName
            Catch ex As Exception
                ErrMsg = ex.Message
                Return ""
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取当前账号
    ''' </summary>
    ''' <value></value>
    ''' <returns>当前账号</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property CurAccount() As String
        Get
            Try
                Return Environment.UserName
            Catch ex As Exception
                ErrMsg = ex.Message
                Return ""
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取剩余内存
    ''' </summary>
    ''' <value></value>
    ''' <returns>剩余内存，MB</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property LeftRamSize() As Long
        Get
            Try
                Dim M As Long = 0
                Dim strComputer As String = "."
                Dim objWMIService = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2")
                Dim objRefresher = CreateObject("WbemScripting.SWbemRefresher")
                Dim objMemory = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_PerfOS_Memory").objectSet
                objRefresher.Refresh()
                For Each intAvailableBytes As Object In objMemory
                    M = intAvailableBytes.AvailableMBytes
                Next
                Return M
            Catch ex As Exception
                ErrMsg = ex.Message
                Return -1
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 系统总可用内存
    ''' </summary>
    ''' <value></value>
    ''' <returns>内存，MB</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property TotalRamSize() As Long
        Get
            Try
                Dim memStatus As New MEMORYSTATUS
                GlobalMemoryStatus(memStatus)
                Return memStatus.dwTotalPhys / 1048576
            Catch ex As Exception
                ErrMsg = ex.Message
                Return 0
            End Try
        End Get
    End Property

    ''' <summary>
    ''' CPU占用率
    ''' </summary>
    ''' <value></value>
    ''' <returns>CPU占用率，百分比</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property UsedCpuPercent() As Integer
        Get
            Try
                Dim P As Integer = 0
                Dim strComputer As String = "."
                Dim objWMIService = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2")
                Dim objRefresher = CreateObject("WbemScripting.SWbemRefresher")
                Dim objProcessor = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_PerfOS_Processor").objectSet
                objRefresher.Refresh()
                For Each intProcessorUse As Object In objProcessor
                    P = intProcessorUse.PercentProcessorTime
                Next
                Return P
            Catch ex As Exception
                ErrMsg = ex.Message
                Return 0
            End Try
        End Get
    End Property

    ''' <summary>
    ''' DirectX
    ''' </summary>
    ''' <value></value>
    ''' <returns>返回DX版本后，DX9C以后均显示DX9C</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property DirectXVersion() As String
        Get
            Try
                Const HKEY_LOCAL_MACHINE = &H80000002
                Dim strComputer As String = "."
                Dim objRegistry As Object = GetObject("winmgmts:\\" & strComputer & "\root\default:StdRegProv")
                Dim strKeyPath As String = "Software\Microsoft\DirectX"
                Dim strValueName As String = "Version"
                Dim strValue As String = ""
                Dim strVersion As String = ""
                objRegistry.GetStringValue(HKEY_LOCAL_MACHINE, strKeyPath, strValueName, strValue)
                Select Case strValue
                    Case "4.02.0095"
                        strVersion = "1.0"
                    Case "4.03.00.1096"
                        strVersion = "2.0"
                    Case "4.04.0068"
                        strVersion = "3.0"
                    Case "4.04.0069"
                        strVersion = "3.0"
                    Case "4.05.00.0155"
                        strVersion = "5.0"
                    Case "4.05.01.1721"
                        strVersion = "5.0"
                    Case "4.05.01.1998"
                        strVersion = "5.0"
                    Case "4.06.02.0436"
                        strVersion = "6.0"
                    Case "4.07.00.0700"
                        strVersion = "7.0"
                    Case "4.07.00.0716"
                        strVersion = "7.0a"
                    Case "4.08.00.0400"
                        strVersion = "8.0"
                    Case "4.08.01.0881"
                        strVersion = "8.1"
                    Case "4.08.01.0810"
                        strVersion = "8.1"
                    Case "4.09.0000.0900"
                        strVersion = "9.0"
                    Case "4.09.00.0900"
                        strVersion = "9.0"
                    Case "4.09.0000.0901"
                        strVersion = "9.0a"
                    Case "4.09.00.0901"
                        strVersion = "9.0a"
                    Case "4.09.0000.0902"
                        strVersion = "9.0b"
                    Case "4.09.00.0902"
                        strVersion = "9.0b"
                    Case "4.09.00.0904"
                        strVersion = "9.0c"
                    Case "4.09.0000.0904"
                        strVersion = "9.0c"
                End Select
                Return strVersion
            Catch ex As Exception
                ErrMsg = ex.Message
                Return ""
            End Try
        End Get
    End Property

    ''' <summary>
    ''' .NET版本
    ''' </summary>
    ''' <value></value>
    ''' <returns>.NET版本</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property DotNetVersion() As String
        Get
            Try
                Return Environment.Version.Major.ToString + "." + Environment.Version.Minor.ToString + "." + Environment.Version.Build.ToString
            Catch ex As Exception
                ErrMsg = ex.Message
                Return ""
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 返回当前工作域
    ''' </summary>
    ''' <value></value>
    ''' <returns>工作域</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property UserDomain() As String
        Get
            Try
                Return Environment.UserDomainName
            Catch ex As Exception
                ErrMsg = ex.Message
                Return ""
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取逻辑驱动器
    ''' </summary>
    ''' <value></value>
    ''' <returns>驱动器字符串盘符数组，NOTHING为找不到</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property LogicDisk() As String()
        Get
            Try
                Dim drives As DriveInfo() = DriveInfo.GetDrives()
                Dim pack As List(Of String) = New List(Of String)
                For Each Dir As DriveInfo In drives
                    pack.Add(Dir.Name)
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As String
                    pack.CopyTo(res)
                    Return res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Get
    End Property

    Public Shared ReadOnly Property RemovableLogicDisk() As String()
        Get
            Try
                Dim drives As DriveInfo() = DriveInfo.GetDrives()
                Dim pack As List(Of String) = New List(Of String)
                For Each Dir As DriveInfo In drives
                    If Dir.DriveType = DriveType.Removable Then
                        pack.Add(Dir.Name)
                    End If
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As String
                    pack.CopyTo(res)
                    Return res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取逻辑盘空间大小
    ''' </summary>
    ''' <param name="DiskName">盘符</param>
    ''' <param name="TotalSpace">总大小，MB</param>
    ''' <param name="LeftSpace">剩余大小，MB</param>
    ''' <param name="UsedSpace">可用大小，MB</param>
    ''' <returns>TRUE为成功，FALSE为失败</returns>
    ''' <remarks></remarks>
    Public Shared Function LogicDiskSpace(ByVal DiskName As String, ByRef TotalSpace As Long, ByRef LeftSpace As Long, ByRef UsedSpace As Long) As Boolean
        Try
            Dim BytesFreeToCalller As Long = 0
            Dim TotalBytes As Long = 0
            Dim TotalFreeBytes As Long = 0
            Dim TotalBytesUsed As Long = 0
            If Replace(DiskName, ":\", ":").Contains(":") Then
                DiskName = Replace(DiskName, ":\", ":").Split(":")(0) + ":\"
                Call GetDiskFreeSpaceEx(DiskName, BytesFreeToCalller, TotalBytes, TotalFreeBytes)
                TotalSpace = (TotalBytes / 1024) / 1024
                LeftSpace = (TotalFreeBytes / 1024) / 1024
                UsedSpace = ((TotalBytes - TotalFreeBytes) / 1024) / 1024
                Return True
            ElseIf DiskName.Trim.Length = 1 Then
                DiskName = DiskName + ":\"
                Call GetDiskFreeSpaceEx(DiskName, BytesFreeToCalller, TotalBytes, TotalFreeBytes)
                TotalSpace = (TotalBytes / 1024) / 1024
                LeftSpace = (TotalFreeBytes / 1024) / 1024
                UsedSpace = ((TotalBytes - TotalFreeBytes) / 1024) / 1024
                Return True
            Else
                ErrMsg = "路径有误"
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取逻辑磁盘信息
    ''' </summary>
    ''' <param name="DiskName">盘符</param>
    ''' <param name="DriveFormat">磁盘格式</param>
    ''' <param name="VolumeLabel">卷标</param>
    ''' <returns>TRUE为成功，FALSE为失败</returns>
    ''' <remarks></remarks>
    Public Shared Function LogicDiskInfo(ByVal DiskName As String, ByRef DriveFormat As String, ByRef VolumeLabel As String) As Boolean
        Try
            Dim drives As DriveInfo() = DriveInfo.GetDrives()
            If DiskName.Replace(":\", ":").Contains(":") Then
                DiskName = DiskName.Replace(":\", ":").Split("0")(0).ToUpper
                For Each Dir As DriveInfo In drives
                    If Dir.Name.ToUpper.Contains(DiskName) Then
                        DriveFormat = Dir.DriveFormat
                        VolumeLabel = Dir.VolumeLabel
                        Return True
                    End If
                Next
                ErrMsg = "找不到驱动器"
                Return False
            ElseIf DiskName.Trim.Length = 1 Then
                DiskName = DiskName.ToUpper
                For Each Dir As DriveInfo In drives
                    If Dir.Name.ToUpper.Contains(DiskName) Then
                        DriveFormat = Dir.DriveFormat
                        VolumeLabel = Dir.VolumeLabel
                        Return True
                    End If
                Next
                ErrMsg = "找不到驱动器"
                Return False
            Else
                ErrMsg = "路径有误"
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 格式化硬盘，慎用
    ''' </summary>
    ''' <param name="Disk">分区索引</param>
    ''' <param name="Fast">是否快速格式化</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function FormatDisk(ByVal Disk As Integer, Optional ByVal Fast As Boolean = True) As Boolean
        Try
            Dim lngReturn As Integer = 0
            If (Fast) Then
                lngReturn = SHFormatDrive(IntPtr.Zero, Disk, &HFFFF, 1&)
            Else
                lngReturn = SHFormatDrive(IntPtr.Zero, Disk, &HFFFF, 2&)
            End If
            Select Case lngReturn
                Case -2
                    Return True
                Case -3
                    ErrMsg = "Cannot format a read only drive"
                    Return False
                Case Else
                    ErrMsg = "未知错误:" + lngReturn.ToString
                    Return False
            End Select
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 格式化硬盘，慎用
    ''' </summary>
    ''' <param name="DriveLetter">盘符</param>
    ''' <param name="FormatType">格式化类型</param>
    ''' <param name="VolumeName">卷标</param>
    ''' <param name="Fast">快速格式化</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FormatDisk(ByVal DriveLetter As String, ByVal FormatType As DiskFormatType, Optional ByVal VolumeName As String = "", Optional ByVal Fast As Boolean = True) As Boolean
        Try
            If DriveLetter Is Nothing Then
                Return False
            ElseIf DriveLetter.Trim.Length = 0 Then
                Return False
            Else
                DriveLetter = Left(DriveLetter, 1) + ":"
            End If
            Dim cmd As String = "format " + DriveLetter + " "
            Select Case FormatType
                Case DiskFormatType.FAT
                    cmd += " /fs:FAT "
                Case DiskFormatType.FAT32
                    cmd += " /fs:FAT32 "
                Case DiskFormatType.NTFS
                    cmd += " /fs:NTFS "
            End Select
            If Fast Then
                cmd += " /q "
            End If
            If VolumeName.ToString.Trim <> "" Then
                cmd += " /v:" + VolumeName
            End If
            cmd += " "
            Shell(cmd)
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 写系统日志
    ''' </summary>
    ''' <param name="LogSource">日志路径</param>
    ''' <param name="LogMessage">日志内容</param>
    ''' <param name="Type">日志类型</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function WriteEventLog(ByVal LogSource As String, ByVal LogMessage As String, Optional ByVal Type As EventLogEntryType = EventLogEntryType.Information) As Boolean
        Try
            If LogSource Is Nothing Then
                Return False
            ElseIf LogSource.Trim = "" Then
                Return False
            End If
            If LogMessage Is Nothing Then
                LogMessage = ""
            End If
            If Not EventLog.SourceExists(LogSource) Then
                EventLog.CreateEventSource(LogSource, "Application")
            End If
            Dim myLog As New EventLog()
            myLog.Source = LogSource
            myLog.WriteEntry(LogMessage, Type)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function





    ''' <summary>
    ''' 读取系统日志
    ''' </summary>
    ''' <param name="LogSource">日志产生来源</param>
    ''' <returns>日志信息</returns>
    ''' <remarks></remarks>
    Public Shared Function ReadEventLog(Optional ByVal LogSource As String = "") As EventLogEntryCollection
        Try
            If LogSource Is Nothing Then
                Return Nothing
            ElseIf LogSource.Trim = "" Then
                Return Nothing
            End If
            Dim myLog As EventLog = New EventLog()
            myLog.Source = LogSource
            Return myLog.Entries
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 清除系统日志
    ''' </summary>
    ''' <param name="LogSource">日志路径,默认系统日志</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function ClearLog(Optional ByVal LogSource As String = "") As Boolean
        Try
            If LogSource Is Nothing Then
                LogSource = WindowsLog
            ElseIf LogSource.Trim = "" Then
                LogSource = WindowsLog
            End If
            If EventLog.Exists(LogSource) Then
                EventLog.Delete(LogSource)
            End If
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 结构体转数组
    ''' </summary>
    ''' <param name="structObj">结构体</param>
    ''' <returns>数组</returns>
    ''' <remarks></remarks>
    Public Shared Function StructToBytes(ByVal structObj As Object) As Byte()
        Try
            Dim size As Integer = Marshal.SizeOf(structObj)
            Dim _bytes(size - 1) As Byte
            Dim structPtr As IntPtr = Marshal.AllocHGlobal(size)
            Marshal.StructureToPtr(structObj, structPtr, False)
            Marshal.Copy(structPtr, _bytes, 0, size)
            Marshal.FreeHGlobal(structPtr)
            Return _bytes
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

End Class