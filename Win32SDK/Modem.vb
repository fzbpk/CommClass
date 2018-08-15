Imports System.Management
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports IWshRuntimeLibrary
Imports Microsoft.Win32
Imports System.IO
Imports System.Text

Public Class Modem
    Private Shared ErrMsg As String = ""
    Private Shared Seeltime As Integer = 800
    Private Shared BuffSize As Integer = 1024
    Private Shared TOut As Integer = 1500
    Private Shared Country As Integer = 86
    Public Enum Modem_Status
        Other = 1
        Unknown
        Enabled
        Disabled
        NotApplicable
    End Enum

    Public Structure Modem_Info
        Public Index As UInteger
        Public Name As String
        Public Caption As String
        Public Description As String
        Public ComPort As String
        Public MaxBaudRate As UInteger
        Public Model As String
        Public DeviceType As String
        Public DevID As String
        Public SpeakerVolumeInfo As UShort
        Public Status As Modem_Status
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
    ''' 获取或设置等待时间
    ''' </summary>
    ''' <value>等待时间</value>
    ''' <returns>等待时间</returns>
    ''' <remarks></remarks>
    Public Shared Property WaitTime() As Integer
        Get
            Return Seeltime
        End Get
        Set(ByVal value As Integer)
            Seeltime = value
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置缓存大小
    ''' </summary>
    ''' <value>缓存大小</value>
    ''' <returns>缓存大小</returns>
    ''' <remarks></remarks>
    Public Shared Property Cache() As Integer
        Get
            Return BuffSize
        End Get
        Set(ByVal value As Integer)
            BuffSize = value
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置超时
    ''' </summary>
    ''' <value>超时</value>
    ''' <returns>超时</returns>
    ''' <remarks></remarks>
    Public Shared Property TimeOut() As Integer
        Get
            Return TOut
        End Get
        Set(ByVal value As Integer)
            TOut = value
        End Set
    End Property

    Public Shared Property CountryCode() As Integer
        Get
            Return Country
        End Get
        Set(ByVal value As Integer)
            Country = value
        End Set
    End Property

    ''' <summary>
    ''' 获取所有调制解调器信息
    ''' </summary>
    ''' <returns>调制解调器信息</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ModemList As Modem_Info()
        Get
            Try
                Dim pack As List(Of Modem_Info) = New List(Of Modem_Info)
                Dim realmodem As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_POTSModem ")
                Dim queryrealmodem As ManagementObjectCollection = realmodem.Get()
                Dim Naems As String = ""
                For Each queryObj As ManagementObject In queryrealmodem
                    Dim DTU As New Modem_Info
                    DTU.Name = queryObj("Name")
                    DTU.Caption = queryObj("Caption")
                    DTU.Description = queryObj("Description")
                    DTU.Model = queryObj("Model")
                    DTU.DevID = queryObj("DeviceID")
                    DTU.ComPort = queryObj("AttachedTo")
                    DTU.MaxBaudRate = CUInt(queryObj("MaxBaudRateToSerialPort"))
                    DTU.DeviceType = queryObj("DeviceType")
                    DTU.SpeakerVolumeInfo = CUShort(queryObj("SpeakerVolumeInfo"))
                    DTU.Index = CUInt(queryObj("Index"))
                    DTU.Status = CUShort(queryObj("StatusInfo"))
                    pack.Add(DTU)
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As Modem_Info
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
    ''' 是否支持AT指令
    ''' </summary>
    ''' <param name="DTU">DTU参数</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SupportAT(ByVal DTU As Modem_Info) As Boolean
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As Boolean = False
                Try
                    ComPort.Write("at" + vbCrLf)
                    System.Threading.Thread.Sleep(Seeltime)
                    Dim recv As String = ComPort.ReadExisting
                    If recv.ToUpper.Contains("OK") Then
                        res = True
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                    Return False
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
                Return res
            Else
                ErrMsg = "找不到串口"
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 重启DTU
    ''' </summary>
    ''' <param name="DTU">DTU参数</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function ReSet(ByVal DTU As Modem_Info) As Boolean
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As Boolean = False
                Try
                    ComPort.Write("ate0" + vbCrLf)
                    System.Threading.Thread.Sleep(Seeltime)
                    Dim recv As String = ComPort.ReadExisting
                    If recv.ToUpper.Contains("OK") Then
                        res = True
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                    Return False
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
                Return res
            Else
                ErrMsg = "找不到串口"
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取信号强度
    ''' </summary>
    ''' <param name="DTU">DTU参数</param>
    ''' <returns>信号强度百分比，0-100%，255为获取失败</returns>
    ''' <remarks></remarks>
    Public Shared Function SignalStrength(ByVal DTU As Modem_Info) As Byte
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As Byte = 255
                Try
                    ComPort.Write("at+csq" + vbCrLf)
                    System.Threading.Thread.Sleep(Seeltime)
                    Dim recv As String = ComPort.ReadExisting
                    If recv.ToUpper.Contains("ERROR") Then
                        res = 255
                    ElseIf recv.ToUpper.Contains("+CSQ") Then
                        Dim I As Integer = recv.ToUpper.Trim.IndexOf("+CSQ:") + 6
                        Dim J As Integer = recv.ToUpper.Trim.LastIndexOf(",")
                        If J >= I + 1 Then
                            Dim SI As Int16 = 0
                            Try
                                recv = recv.Substring(I + 1, J - I + 1).Trim
                                SI = Int16.Parse(recv)
                                res = CInt((CDbl(SI) / 32.0) * 100)
                            Catch

                            End Try
                        Else
                            res = 255
                        End If
                    Else
                        res = 255
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
                Return res
            Else
                ErrMsg = "找不到串口"
                Return 255
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return 255
        End Try
    End Function

    ''' <summary>
    ''' 获取信号增益
    ''' </summary>
    ''' <param name="DTU">DTU参数</param>
    ''' <returns>增益</returns>
    ''' <remarks></remarks>
    Public Shared Function SignaldB(ByVal DTU As Modem_Info) As Byte
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As Byte = 255
                Try
                    ComPort.Write("at+csq" + vbCrLf)
                    System.Threading.Thread.Sleep(Seeltime)
                    Dim recv As String = ComPort.ReadExisting
                    If recv.ToUpper.Contains("ERROR") Then
                        res = 255
                    ElseIf recv.ToUpper.Contains("+CSQ") Then
                        Dim I As Integer = recv.ToUpper.Trim.IndexOf("+CSQ:") + 6
                        Dim J As Integer = recv.ToUpper.Trim.LastIndexOf(",")
                        If J >= I + 1 Then
                            Try
                                recv = recv.Substring(I + 1, J - I + 1).Trim
                                res = Int16.Parse(recv)
                            Catch

                            End Try
                        Else
                            res = 255
                        End If
                    Else
                        res = 255
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
                Return res
            Else
                ErrMsg = "找不到串口"
                Return 255
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return 255
        End Try
    End Function

    ''' <summary>
    ''' 获取网络运营商
    ''' </summary>
    ''' <param name="DTU">DTU参数</param>
    ''' <returns>运营商资料</returns>
    ''' <remarks></remarks>
    Public Shared Function MobileNet(ByVal DTU As Modem_Info) As String
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As String = ""
                Try
                    ComPort.Write("at+cops?" + vbCrLf)
                    System.Threading.Thread.Sleep(Seeltime)
                    Dim recv As String = ComPort.ReadExisting
                    If recv.ToUpper.Contains("ERROR") Then
                        res = ""
                    ElseIf recv.ToUpper.Contains("+COPS:") Then
                        recv = Right(recv, recv.Length - recv.IndexOf(Chr(34)) - 1)
                        recv = Left(recv, recv.LastIndexOf(Chr(34)))
                        res = recv
                    Else
                        res = ""
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
                Return res
            Else
                ErrMsg = "找不到串口"
                Return ""
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 初始化DTU
    ''' </summary>
    ''' <param name="DTU">DTU</param>
    ''' <param name="CMDStr">初始化语句，多个命令使用回车换行</param>
    ''' <remarks></remarks>
    Public Shared Sub ModemInit(ByVal DTU As Modem_Info, ByVal CMDStr As String)
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As String = ""
                Try
                    If CMDStr Is Nothing Then

                    ElseIf CMDStr.Trim = "" Then

                    Else
                        If CMDStr.Contains(vbCrLf) Then
                            Dim ustr As String() = Split(CMDStr, vbCrLf)
                            For i As Integer = LBound(ustr) To UBound(ustr)
                                ComPort.Write(ustr(i) + vbCrLf)
                                System.Threading.Thread.Sleep(Seeltime)
                            Next
                        Else
                            ComPort.Write(CMDStr + vbCrLf)
                            System.Threading.Thread.Sleep(Seeltime)
                        End If
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
            Else
                ErrMsg = "找不到串口"
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 初始化DTU
    ''' </summary>
    ''' <param name="DTU">DTU</param>
    ''' <param name="CMDHex">初始化命令</param>
    ''' <remarks></remarks>
    Public Shared Sub ModemInit(ByVal DTU As Modem_Info, ByVal CMDHex As Byte())
        Try
            If DTU.ComPort.Trim <> "" Then
                Dim ComPort As New IO.Ports.SerialPort
                With ComPort
                    .PortName = DTU.ComPort
                    .BaudRate = DTU.MaxBaudRate
                    .StopBits = IO.Ports.StopBits.One
                    .DataBits = 8
                    .Parity = IO.Ports.Parity.None
                    .Handshake = IO.Ports.Handshake.None
                    .ReadBufferSize = BuffSize
                    .WriteBufferSize = BuffSize
                    .ReadTimeout = TOut
                    .WriteTimeout = TOut
                    .Open()
                End With
                Dim res As String = ""
                Try
                    If CMDHex Is Nothing Then

                    ElseIf CMDHex.Length = 0 Then

                    Else
                        ComPort.Write(CMDHex, 0, CMDHex.Length)
                        System.Threading.Thread.Sleep(Seeltime)
                    End If
                Catch ex As Exception
                    ErrMsg = ex.Message
                Finally
                    If ComPort.IsOpen Then
                        ComPort.Close()
                    End If
                End Try
            Else
                ErrMsg = "找不到串口"
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    <DllImport("kernel32.dll", CallingConvention:=CallingConvention.Winapi)>
    Private Shared Function GetVolumeNameForVolumeMountPoint(ByVal lpszVolumeMountPoint As String, ByVal lpszVolumeName As IntPtr, ByVal cchBufferLength As Integer) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function SetupDiGetClassDevs(ByRef ClassGuid As Guid, ByVal Enumerator As IntPtr, ByVal hWndParent As Integer, ByVal Flags As Integer) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function SetupDiDestroyDeviceInfoList(ByVal DeviceInfoSet As Integer) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function SetupDiEnumDeviceInterfaces(ByVal DeviceInfoSet As Integer, ByVal DeviceInfoData As IntPtr, ByRef Guid As Guid, ByVal MemberIndex As Integer, ByVal DeviceInterfaceData As IntPtr) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function SetupDiEnumDeviceInfo(ByVal DeviceInfoSet As Integer, ByVal MemberIndex As Integer, ByVal DeviceInterfaceData As IntPtr) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function SetupDiGetDeviceInstanceId(ByVal DeviceInfoSet As Integer, ByVal DeviceInfoData As IntPtr, ByVal DeviceInstanceID As IntPtr, ByVal DeviceInstanceIdSize As Integer, ByRef RequiredSize As IntPtr) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function SetupDiGetDeviceInterfaceDetail(ByVal DeviceInfoSet As Integer, ByVal DeviceInterfaceData As IntPtr, ByVal DeviceInterfaceDetailData As IntPtr, ByVal DeviceInterfaceDetailDataSize As Integer, ByVal RequiredSize As Integer, ByVal DeviceInfoData As IntPtr) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function CM_Get_Parent(ByRef pdnDevInst As Integer, ByVal dnDevInst As Integer, ByVal ulFlags As Integer) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function CM_Get_Device_ID_Size(ByRef pulLen As Integer, ByVal dnDevInst As Integer, ByVal ulFlags As Integer) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function CM_Get_Device_ID(ByVal dnDevInst As Integer, ByVal BufferPtr As IntPtr, ByVal BufferLen As String, ByVal ulFlags As Integer) As Integer
    End Function
    <DllImport("setupapi.dll", CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function CM_Request_Device_Eject(ByVal dnDevInst As Integer, ByVal pVetoType As IntPtr, ByVal pszVetoName As IntPtr, ByVal ulNameLength As Integer, ByVal ulFlags As Integer) As Integer
    End Function

    Private Declare Function mciSendCommand Lib "winmm" Alias "mciSendCommandA" (ByVal wDeviceID As Integer, ByVal uMessage As Integer, ByVal dwParam1 As Integer, ByRef dwParam2 As MCI_OPEN_PARMS) As Integer

    <StructLayout(LayoutKind.Sequential)>
    Private Class SP_DEVINFO_DATA
        Public cbSize As Integer
        Public InterfaceClassGUID As Guid
        Public DevInst As Integer
        Public Reserved As Integer
    End Class
    <StructLayout(LayoutKind.Sequential)>
    Private Structure SP_DEVICE_INTERFACE_DATA
        Public cbSize As Integer
        Public InterfaceClassGUID As Guid
        Public Flags As Integer
        Public Reserved As Integer
    End Structure

    Private Structure MCI_OPEN_PARMS
        Public dwCallback As Integer
        Public wDeviceID As IntPtr
        Public lpstrDeviceType As String
        Public lpstrElementName As String
        Public lpstrAlias As String
    End Structure

    Private Const DIGCF_PRESENT As Integer = &H2
    Private Const DIGCF_ALLCLASSES As Integer = &H4
    Private Const DIGCF_DEVICEINTERFACE As Integer = &H10
    Private Const INVALID_HANDLE_VALUE As Integer = -1

    Private Const MCI_WAIT As Integer = &H2
    Private Const MCI_SET_DOOR_CLOSED As Integer = &H200
    Private Const MCI_OPEN_ELEMENT As Integer = &H200
    Private Const MCI_OPEN = &H803
    Private Const MCI_OPEN_TYPE = &H2000&
    Private Const MCI_OPEN_SHAREABLE = &H100&
    Private Const MCI_SET = &H80D
    Private Const MCI_SET_DOOR_OPEN = &H100&
    Private Const MCI_CLOSE = &H804

    Private Shared Function GetDeviceID(ByVal hDevInfo As Integer, ByVal didPtr As IntPtr) As String
        Dim DeviceID As String = ""
        Dim idPtr As IntPtr = Marshal.AllocHGlobal(1024)
        If SetupDiGetDeviceInstanceId(hDevInfo, didPtr, idPtr, 1024, IntPtr.Zero) <> 0 Then
            Try
                DeviceID = Marshal.PtrToStringAnsi(idPtr, 1024)
                DeviceID = Left(DeviceID, InStr(DeviceID, Chr(0)) - 1)
            Catch ex As Exception
            End Try
        End If
        Marshal.FreeHGlobal(idPtr)
        Return DeviceID
    End Function
    Private Shared Function GetDeviceID(ByVal DevInst As Integer) As String
        Dim ReqLen As Integer
        Dim DeviceID As String = ""
        If CM_Get_Device_ID_Size(ReqLen, DevInst, 0) = 0 Then
            Dim idPtr As IntPtr = Marshal.AllocHGlobal(ReqLen + 1)
            If CM_Get_Device_ID(DevInst, idPtr, ReqLen + 1, 0) = 0 Then
                Try
                    DeviceID = Marshal.PtrToStringAnsi(idPtr, 1024)
                    DeviceID = Left(DeviceID, InStr(DeviceID, Chr(0)) - 1)
                Catch ex As Exception
                End Try
            End If
            Marshal.FreeHGlobal(idPtr)
        End If
        Return DeviceID
    End Function

    Private Shared Sub EjectUsbDrive(ByVal DriveLetter As String)
        Try
            Dim GUID_DEVINTERFACE_VOLUME As Guid = New Guid(&H53F5630D, &HB6BF, &H11D0, &H94, &HF2, &H0, &HA0, &HC9, &H1E, &HFB, &H8B)
            Dim hDevInfo As Integer = SetupDiGetClassDevs(GUID_DEVINTERFACE_VOLUME, IntPtr.Zero, 0, DIGCF_PRESENT + DIGCF_DEVICEINTERFACE)
            If hDevInfo <> INVALID_HANDLE_VALUE Then
                Dim MemberIndex As Integer = 0
                Dim did As New SP_DEVINFO_DATA
                did.cbSize = Marshal.SizeOf(GetType(SP_DEVINFO_DATA))
                Dim didPtr As IntPtr = Marshal.AllocHGlobal(did.cbSize)
                Marshal.StructureToPtr(did, didPtr, True)
                While SetupDiEnumDeviceInfo(hDevInfo, MemberIndex, didPtr) <> 0
                    Marshal.PtrToStructure(didPtr, did)
                    Dim pDevInst As Integer
                    CM_Get_Parent(pDevInst, did.DevInst, 0)
                    CM_Get_Parent(pDevInst, pDevInst, 0)
                    Dim BusDeviceID As String = GetDeviceID(pDevInst)
                    If BusDeviceID.StartsWith("USB\") Then
                        If CM_Request_Device_Eject(pDevInst, IntPtr.Zero, IntPtr.Zero, 0, 0) = 0 Then

                        End If
                    End If
                    MemberIndex += 1
                    Console.WriteLine()
                End While
                Marshal.FreeHGlobal(didPtr)
                SetupDiDestroyDeviceInfoList(hDevInfo)
            End If
        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' 初始化高通设备
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub Qualcomm_HSDPA_Init()
        Try
            Dim Dirvers As String() = System.Environment.GetLogicalDrives
            If Dirvers Is Nothing Then

            ElseIf Dirvers.Length = 0 Then

            Else
                For Each tmp As String In Dirvers
                    Dim di As DriveInfo = New DriveInfo(tmp)
                    If di.DriveType = DriveType.Removable Then
                        Dim LogicalDiskToPartitions As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition ")
                        For Each obj As ManagementObject In LogicalDiskToPartitions.Get
                            If obj("Dependent").ToString.ToUpper.Contains(tmp.ToUpper.Replace("\", "")) Then
                                Dim inValue As String = obj("Antecedent").ToString
                                Dim parsedValue As String = inValue.Substring(inValue.IndexOf(Chr(34)) + 1, (inValue.LastIndexOf(Chr(34)) - (inValue.IndexOf(Chr(34)))) - 1)
                                parsedValue = Left(parsedValue, parsedValue.IndexOf(",")).ToLower.Replace("disk #", "")
                                Dim DiskDrives As New System.Management.ManagementObjectSearcher("Select * from Win32_DiskDrive where Name like ""%PHYSICALDRIVE" + parsedValue + "%"" and InterfaceType like ""%usb%"" ")
                                For Each obj2 As ManagementObject In DiskDrives.Get
                                    Dim Caption As String = obj2("Caption").ToString.ToLower
                                    If Caption.Contains("qualcomm") Or Caption.Contains("高通") Then
                                        EjectUsbDrive(tmp.Replace("\", ""))
                                    End If
                                Next
                            End If
                        Next
                    ElseIf di.DriveType = DriveType.CDRom Then
                        Dim DiskCD As New System.Management.ManagementObjectSearcher("Select * from Win32_CDROMDrive where Drive like ""%" + tmp.Replace("\", "") + "%"" ")
                        For Each obj3 As ManagementObject In DiskCD.Get
                            Dim diskname As String = obj3("Name").ToString.ToLower
                            Dim DEVID As String = obj3("DeviceID").ToString.ToLower
                            Dim res As Integer
                            If diskname.Contains("qualcomm") Or diskname.Contains("高通") Then
                                Dim OpenParms As New MCI_OPEN_PARMS
                                OpenParms.lpstrDeviceType = "CDAUDIO".ToLower
                                OpenParms.lpstrElementName = tmp.ToUpper
                                res = mciSendCommand(0, MCI_OPEN, MCI_OPEN_ELEMENT Or MCI_OPEN_TYPE Or MCI_OPEN_SHAREABLE, OpenParms)
                                If res = 0 Then
                                    res = mciSendCommand(OpenParms.wDeviceID, MCI_SET, MCI_WAIT Or MCI_SET_DOOR_OPEN, Nothing)
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    '===============================================拨号用============================================'
    Private Const APINULL As Integer = 0&
    Private Const ET_None As Integer = 0    ' No encryption
    Private Const ET_Require As Integer = 1    ' Require Encryption
    Private Const ET_RequireMax As Integer = 2    ' Require max encryption
    Private Const ET_Optional As Integer = 3    ' Do encryption if possible. None Ok.
    Private Const VS_Default As Integer = 0    ' default (PPTP for now)
    Private Const VS_PptpOnly As Integer = 1    ' Only PPTP is attempted.
    Private Const VS_PptpFirst As Integer = 2    ' PPTP is tried first.
    Private Const VS_L2tpOnly As Integer = 3    ' Only L2TP is attempted.
    Private Const VS_L2tpFirst As Integer = 4    ' L2TP is tried first.
    Private Const RASET_Phone As Integer = 1  ' Phone lines: modem, ISDN, X.25, etc
    Private Const RASET_Vpn As Integer = 2  ' Virtual private network
    Private Const RASET_Direct As Integer = 3  ' Direct connect: serial, parallel
    Private Const RASET_Internet As Integer = 4    ' BaseCamp internet
    Private Const RASET_Broadband As Integer = 5  ' Broadband
    'RASENTRY  IP协议 
    Private Const RASNP_NetBEUI As Integer = 1 'NetBEUI
    Private Const RASNP_Ipx As Integer = 2 'ipv6
    Private Const RASNP_Ip As Integer = 4 'ipv4
    ' RASENTRY 'dwFramingProtocols' bit flags. 
    Private Const RASFP_Ppp As Integer = 1
    Private Const RASFP_Slip As Integer = 2
    Private Const RASFP_Ras As Integer = 4
    '
    Private Const RAS_MaxCallbackNumber As Integer = 48
    Private Const RAS_MaxEntryName As Integer = 256
    Private Const RAS_MaxDeviceName As Integer = 128
    Private Const RAS_MaxDeviceType As Integer = 16
    Private Const RAS_MaxPhoneNumber As Integer = 128
    Private Const MAX_PATH As Integer = 260
    Private Const UNLEN As Integer = 256
    Private Const DNLEN As Integer = 15
    Private Const PWLEN As Integer = 256
    Private Const WINVER As Integer = &H400

    Private Structure RAS_GUID
        Public Data1 As Integer
        Public Data2 As Short
        Public Data3 As Short
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
        Public Data4() As Byte
    End Structure

    Private Structure RASIPADDR
        Dim a As Byte
        Dim b As Byte
        Dim c As Byte
        Dim d As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure RASENTRY
        Public dwSize As Integer
        Public dwfOptions As Integer
        Public dwCountryID As Integer
        Public dwCountryCode As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=11)>
        Public szAreaCode() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=129)>
        Public szLocalPhoneNumber() As Byte
        Public dwAlternateOffset As Integer
        Public ipaddr As RASIPADDR
        Public ipaddrDns As RASIPADDR
        Public ipaddrDnsAlt As RASIPADDR
        Public ipaddrWins As RASIPADDR
        Public ipaddrWinsAlt As RASIPADDR
        Public dwFrameSize As Integer
        Public dwfNetProtocols As Integer
        Public dwFramingProtocol As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)>
        Public szScript() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)>
        Public szAutodialDll() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)>
        Public szAutodialFunc() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)>
        Public szDeviceType() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=129)>
        Public szDeviceName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=33)>
        Public szX25PadType() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=201)>
        Public szX25Address() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=201)>
        Public szX25Facilities() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=201)>
        Public szX25UserData() As Byte
        Public dwChannels As Integer
        Public dwReserved1 As Integer
        Public dwReserved2 As Integer
        Public dwSubEntries As Integer
        Public dwDialMode As Integer
        Public dwDialExtraPercent As Integer
        Public dwDialExtraSampleSeconds As Integer
        Public dwHangUpExtraPercent As Integer
        Public dwHangUpExtraSampleSeconds As Integer
        Public dwIdleDisconnectSeconds As Integer
        Public dwType As Integer
        Public dwEncryptionType As Integer
        Public dwCustomAuthKey As Integer
        Public guidId As RAS_GUID
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)>
        Public szCustomDialDll() As Byte
        Public dwVpnStrategy As Integer
        Public dwfOptions2 As Integer
        Public dwfOptions3 As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)>
        Public szDnsSuffix() As Byte
        Public dwTcpWindowsize As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)>
        Public szPrerequisitePbk() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=257)>
        Public szPrerequisiteEntry() As Byte
        Public dwRedialCount As Integer
        Public dwRedialPause As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure RASCREDENTIALS
        Public dwSize As Integer
        Public dwMask As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=UNLEN + 1)>
        Public szUserName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=PWLEN + 1)>
        Public szPassword() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=DNLEN + 1)>
        Public szDomain() As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure RASDEVINFO
        Public dwSize As Long
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)>
        Public szDeviceType() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=129)>
        Public szDeviceName() As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure RASDIALPARAMS
        Public dwSize As Int32
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=RAS_MaxEntryName + 1)>
        Public szEntryName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=RAS_MaxPhoneNumber + 1)>
        Public szPhoneNumber() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=RAS_MaxPhoneNumber + 1)>
        Public szCallbackNumber() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=UNLEN + 1)>
        Public szUserName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=PWLEN + 1)>
        Public szPassword() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=DNLEN + 1)>
        Public szDomain() As Byte

        Public dwSubEntry As Int32
        Public dwCallbackId As Int32
        'Public dwIfIndex As Integer
    End Structure

    Private Structure RASCONN
        'set dwsize to 412 
        Public dwSize As Integer
        Public hRasConn As IntPtr
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=RAS_MaxEntryName + 1)>
        Public szEntryName As String

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=RAS_MaxDeviceType + 1)>
        Public szDeviceType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=RAS_MaxDeviceName + 1)>
        Public szDeviceName As String
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure RasEntryName
        Public dwSize As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=RAS_MaxEntryName + 1)>
        Public szEntryName As String
    End Structure



    Private Declare Function RasSetEntryProperties Lib "rasapi32" Alias "RasSetEntryPropertiesA" (ByVal lpszPhonebook As String, ByVal lpszEntry As String, ByRef lpRasEntry As RASENTRY, ByVal dwEntryInfoSize As Int32, ByVal lpbDeviceInfo As Int32, ByVal dwDeviceInfoSize As Int32) As Int32
    Private Declare Function RasSetEntryProperties Lib "rasapi32" Alias "RasSetEntryPropertiesA" (ByVal lpszPhonebook As String, ByVal lpszEntry As String, ByRef lpRasEntry As RASENTRY, ByVal dwEntryInfoSize As Int32, ByVal lpbDeviceInfo() As Byte, ByVal dwDeviceInfoSize As Int32) As Int32
    Private Declare Function RasSetCredentials Lib "rasapi32" Alias "RasSetCredentialsA" (ByVal lpszPhonebook As String, ByVal lpszEntry As String, ByRef lpCredentials As RASCREDENTIALS, ByVal fClearCredentials As Boolean) As UShort
    Private Declare Function RasDeleteEntry Lib "rasapi32.dll" Alias "RasDeleteEntryA" (ByVal lpszPhonebook As String, ByVal lpszEntry As String) As Integer
    Private Declare Function RasHangUp Lib "RasApi32.DLL" Alias "RasHangUpA" (ByVal hRasConn As Integer) As Integer
    Private Declare Function RasEnumConnections Lib "RasApi32.DLL" Alias "RasEnumConnectionsA" (ByRef lprasconn As RASCONN, ByRef lpcb As Integer, ByRef lpcConnections As Integer) As Integer
    Private Declare Function RasGetEntryDialParams Lib "RasAPI32.dll" Alias "RasGetEntryDialParamsA" (ByVal PB As String, ByRef DialPara As Byte, ByRef Pswd As Integer) As Integer
    Private Declare Sub CopyMemory Lib "KERNEL32" Alias "RtlMoveMemory" (ByRef hpvDest As Byte, ByVal hpvSource As String, ByVal cbCopy As Int32)
    Private Declare Function RasDial Lib "RasApi32.DLL" Alias "RasDialA" (ByVal lpRasDialExtensions As String, ByVal lpszPhonebook As String, ByRef lprasdialparams As RASDIALPARAMS, ByVal dwNotifierType As Int32, ByVal lpvNotifier As Int32, ByRef lphRasConn As Int32) As Int32
    <DllImport("rasapi32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function RasEnumEntries(ByVal reserved As String, ByVal lpszPhonebook As String, <[In](), [Out]()> ByVal lprasentryname() As RasEntryName, ByRef lpcb As Integer, <Out()> ByRef lpcConnections As Integer) As UInteger
    End Function


    ''' <summary>
    ''' 创建VPN连接
    ''' </summary>
    ''' <param name="EntryName">拨号连接名</param>
    ''' <param name="VPNServer">VPN服务器地址</param>
    ''' <param name="Username">用户名</param>
    ''' <param name="Password">密码</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function CreateVPNConnection(ByVal EntryName As String, ByVal VPNServer As String, ByVal Username As String, ByVal Password As String) As Boolean
        Try
            If EntryName Is Nothing Then
                Return False
            ElseIf EntryName.Trim = "" Then
                Return False
            End If
            If Username Is Nothing Then
                Username = ""
            End If
            If Password Is Nothing Then
                Password = ""
            End If
            Dim re As New RASENTRY
            Dim sDeviceName As String, sDeviceType As String
            sDeviceName = "WAN 微型端口 (PPTP)"
            sDeviceType = "vpn"
            With re
                .dwSize = Marshal.SizeOf(GetType(RASENTRY))
                .dwCountryCode = Country
                .dwCountryID = Country
                .dwDialExtraPercent = 75
                .dwDialExtraSampleSeconds = 120
                .dwDialMode = 1
                .dwfNetProtocols = 4
                .dwfOptions = 1024262928
                .dwfOptions2 = 367
                .dwFramingProtocol = 1
                .dwHangUpExtraPercent = 10
                .dwHangUpExtraSampleSeconds = 120
                .dwRedialCount = 3
                .dwRedialPause = 60
                .dwType = RASET_Vpn
                ReDim .szDeviceName(128)
                Dim DeviceName As Byte() = System.Text.Encoding.Default.GetBytes(sDeviceName)
                For i As Integer = LBound(DeviceName) To UBound(DeviceName)
                    .szDeviceName(i) = DeviceName(i)
                Next
                ReDim .szDeviceType(16)
                Dim DeviceType As Byte() = System.Text.Encoding.Default.GetBytes(sDeviceType)
                For i As Integer = LBound(DeviceType) To UBound(DeviceType)
                    .szDeviceType(i) = DeviceType(i)
                Next
                ReDim .szLocalPhoneNumber(128)
                Dim PhoneNumber As Byte() = System.Text.Encoding.Default.GetBytes(VPNServer)
                For i As Integer = LBound(PhoneNumber) To UBound(PhoneNumber)
                    .szLocalPhoneNumber(i) = PhoneNumber(i)
                Next
                .dwVpnStrategy = VS_Default    'vpn类型
                .dwEncryptionType = ET_Optional '数据加密类型
            End With
            Dim EntryNames As String = EntryName
            Dim rtn As Integer = RasSetEntryProperties(vbNullString, EntryNames, re, Marshal.SizeOf(GetType(RASENTRY)), 0, 0)
            If rtn = 0 Then
                If Username.Trim <> "" Then
                    Dim rc As New RASCREDENTIALS
                    With rc
                        ReDim .szPassword(UNLEN)
                        ReDim .szUserName(PWLEN)
                        ReDim .szDomain(DNLEN)
                        .dwSize = Marshal.SizeOf(GetType(RASCREDENTIALS))
                        .dwMask = 11
                        Dim User As Byte() = System.Text.Encoding.Default.GetBytes(Username)
                        For i As Integer = LBound(User) To UBound(User)
                            .szUserName(i) = User(i)
                        Next
                        Dim Pwd As Byte() = System.Text.Encoding.Default.GetBytes(Password)
                        For i As Integer = LBound(Pwd) To UBound(Pwd)
                            .szPassword(i) = Pwd(i)
                        Next
                    End With
                    rtn = RasSetCredentials(vbNullString, EntryName, rc, False)
                    If rtn = 0 Then
                        Return True
                    Else
                        ErrMsg = "RasSetCredentials:" + rtn.ToString()
                        Return False
                    End If
                Else
                    Return True
                End If
            Else
                ErrMsg = "RasSetEntryProperties:" + rtn.ToString()
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 创建PPPoE连接
    ''' </summary>
    ''' <param name="EntryName">拨号连接名</param>
    ''' <param name="Username">用户名</param>
    ''' <param name="Password">密码</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function CreatePPPOEConnection(ByVal EntryName As String, ByVal Username As String, ByVal Password As String) As Boolean
        Try
            If EntryName Is Nothing Then
                Return False
            ElseIf EntryName.Trim = "" Then
                Return False
            End If
            If Username Is Nothing Then
                Username = ""
            End If
            If Password Is Nothing Then
                Password = ""
            End If
            Dim re As New RASENTRY
            Dim sDeviceName As String, sDeviceType As String
            sDeviceName = "WAN微型端口(PPPOE)"
            sDeviceType = "PPPoE"
            With re
                .dwSize = Marshal.SizeOf(GetType(RASENTRY))
                .dwCountryCode = Country
                .dwCountryID = Country
                .dwDialExtraPercent = 75
                .dwDialExtraSampleSeconds = 120
                .dwDialMode = 1
                .dwEncryptionType = 3
                .dwfNetProtocols = 4
                .dwfOptions = 1024262928
                .dwfOptions2 = 367
                .dwFramingProtocol = 1
                .dwHangUpExtraPercent = 10
                .dwHangUpExtraSampleSeconds = 120
                .dwRedialCount = 3
                .dwRedialPause = 60
                .dwType = RASET_Broadband
                ReDim .szDeviceName(128)
                Dim DeviceName As Byte() = System.Text.Encoding.Default.GetBytes(sDeviceName)
                For i As Integer = LBound(DeviceName) To UBound(DeviceName)
                    .szDeviceName(i) = DeviceName(i)
                Next
                ReDim .szDeviceType(16)
                Dim DeviceType As Byte() = System.Text.Encoding.Default.GetBytes(sDeviceType)
                For i As Integer = LBound(DeviceType) To UBound(DeviceType)
                    .szDeviceType(i) = DeviceType(i)
                Next
            End With
            Dim EntryNames As String = EntryName
            Dim rtn As Integer = RasSetEntryProperties(vbNullString, EntryNames, re, Marshal.SizeOf(GetType(RASENTRY)), 0, 0)
            If rtn = 0 Then
                If Username.Trim <> "" Then
                    Dim rc As New RASCREDENTIALS
                    With rc
                        ReDim .szPassword(UNLEN)
                        ReDim .szUserName(PWLEN)
                        ReDim .szDomain(DNLEN)
                        .dwSize = Marshal.SizeOf(GetType(RASCREDENTIALS))
                        .dwMask = 11
                        Dim User As Byte() = System.Text.Encoding.Default.GetBytes(Username)
                        For i As Integer = LBound(User) To UBound(User)
                            .szUserName(i) = User(i)
                        Next
                        Dim Pwd As Byte() = System.Text.Encoding.Default.GetBytes(Password)
                        For i As Integer = LBound(Pwd) To UBound(Pwd)
                            .szPassword(i) = Pwd(i)
                        Next
                    End With
                    rtn = RasSetCredentials(vbNullString, EntryName, rc, False)
                    If rtn = 0 Then
                        Return True
                    Else
                        ErrMsg = "RasSetCredentials:" + rtn.ToString()
                        Return False
                    End If
                Else
                    Return True
                End If
            Else
                ErrMsg = "RasSetEntryProperties:" + rtn.ToString()
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 创建GPRS,WCDMA拨号连接,CDMA待测
    ''' </summary>
    ''' <param name="EntryName">拨号连接名</param>
    ''' <param name="DTU">硬件设备</param>
    ''' <param name="DailPhoneNumber">号码</param>
    ''' <param name="APN">APN</param>
    ''' <param name="Username">用户名</param>
    ''' <param name="Password">密码</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function CreatePPPDConnection(ByVal EntryName As String, ByVal DTU As Modem_Info, ByVal DailPhoneNumber As String, ByVal APN As String, ByVal Username As String, ByVal Password As String) As Boolean
        Try
            If EntryName Is Nothing Then
                Return False
            ElseIf EntryName.Trim = "" Then
                Return False
            End If
            If Username Is Nothing Then
                Username = ""
            End If
            If Password Is Nothing Then
                Password = ""
            End If
            If DailPhoneNumber Is Nothing Then
                DailPhoneNumber = ""
            End If
            If APN Is Nothing Then
                APN = ""
            End If
            Dim re As New RASENTRY
            Dim sDeviceName As String, sDeviceType As String
            sDeviceName = DTU.Name
            sDeviceType = "modem"
            With re
                .dwSize = Marshal.SizeOf(GetType(RASENTRY))
                .dwCountryCode = Country
                .dwCountryID = Country
                .dwDialExtraPercent = 75
                .dwDialExtraSampleSeconds = 120
                .dwDialMode = 1
                .dwfOptions = &H400000 Or &H10
                .dwfNetProtocols = 4
                .dwFramingProtocol = 1
                .dwHangUpExtraPercent = 10
                .dwHangUpExtraSampleSeconds = 120
                .dwRedialCount = 3
                .dwRedialPause = 60
                ReDim .szDeviceName(128)
                Dim DeviceName As Byte() = System.Text.Encoding.Default.GetBytes(sDeviceName)
                For i As Integer = LBound(DeviceName) To UBound(DeviceName)
                    .szDeviceName(i) = DeviceName(i)
                Next
                ReDim .szDeviceType(16)
                Dim DeviceType As Byte() = System.Text.Encoding.Default.GetBytes(sDeviceType)
                For i As Integer = LBound(DeviceType) To UBound(DeviceType)
                    .szDeviceType(i) = DeviceType(i)
                Next
                ReDim .szLocalPhoneNumber(128)
                Dim PhoneNumber As Byte() = System.Text.Encoding.Default.GetBytes(DailPhoneNumber)
                For i As Integer = LBound(PhoneNumber) To UBound(PhoneNumber)
                    .szLocalPhoneNumber(i) = PhoneNumber(i)
                Next
            End With
            Dim rtn As Integer = -1
            Dim EntryNames As String = EntryName
            If APN.Trim = "" Then
                rtn = RasSetEntryProperties(vbNullString, EntryNames, re, Marshal.SizeOf(GetType(RASENTRY)), 0, 0)
            Else
                Dim lpb(435) As Byte
                lpb(0) = 180
                lpb(1) = 1
                lpb(4) = 180
                lpb(5) = 1
                lpb(8) = 180
                lpb(9) = 1
                lpb(12) = 1
                lpb(16) = 15
                lpb(20) = 1
                lpb(24) = 2
                lpb(76) = 1
                lpb(80) = 2
                lpb(84) = 2
                Dim n As Integer = 0
                For i As Integer = 0 To APN.Length - 1
                    lpb(88 + n) = System.Text.Encoding.ASCII.GetBytes(APN)(i)
                    n += 2
                Next
                lpb(344) = 1
                lpb(348) = 1
                rtn = RasSetEntryProperties(vbNullString, EntryNames, re, Marshal.SizeOf(GetType(RASENTRY)), lpb, lpb.Length)
            End If
            If rtn = 0 Then
                If Username.Trim = "" Then
                    Return True
                Else
                    Dim rc As New RASCREDENTIALS
                    With rc
                        ReDim .szPassword(UNLEN)
                        ReDim .szUserName(PWLEN)
                        ReDim .szDomain(DNLEN)
                        .dwSize = Marshal.SizeOf(GetType(RASCREDENTIALS))
                        .dwMask = 11
                        Dim User As Byte() = System.Text.Encoding.Default.GetBytes(Username)
                        For i As Integer = LBound(User) To UBound(User)
                            .szUserName(i) = User(i)
                        Next
                        Dim Pwd As Byte() = System.Text.Encoding.Default.GetBytes(Password)
                        For i As Integer = LBound(Pwd) To UBound(Pwd)
                            .szPassword(i) = Pwd(i)
                        Next
                    End With
                    rtn = RasSetCredentials(vbNullString, EntryName, rc, False)
                    If rtn = 0 Then
                        Return True
                    Else
                        ErrMsg = "RasSetCredentials:" + rtn.ToString()
                        Return False
                    End If
                End If
            Else
                ErrMsg = "RasSetEntryProperties:" + rtn.ToString()
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 删除拨号连接
    ''' </summary>
    ''' <param name="EntryName">拨号连接名</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function DeleteConnection(ByVal EntryName As String) As Boolean
        Try
            If EntryName Is Nothing Then
                Return False
            ElseIf EntryName.Trim = "" Then
                Return False
            End If
            Dim rtn As Integer = RasDeleteEntry(vbNullString, EntryName)
            If rtn = 0 Then
                Return True
            Else
                ErrMsg = "RasDeleteEntry:" + rtn.ToString()
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 连接
    ''' </summary>
    ''' <param name="EntryName">拨号连接名</param>
    ''' <param name="UserName">用户名</param>
    ''' <param name="PassWord">密码</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function Connection(ByVal EntryName As String, ByVal UserName As String, ByVal PassWord As String) As IntPtr
        Try
            If EntryName Is Nothing Then
                Return False
            ElseIf EntryName.Trim = "" Then
                Return False
            End If
            If UserName Is Nothing Then
                UserName = ""
            End If
            If PassWord Is Nothing Then
                PassWord = ""
            End If
            Dim hRasConn As IntPtr = IntPtr.Zero
            Dim lprasdialparams As New RASDIALPARAMS
            With lprasdialparams
                .dwSize = Marshal.SizeOf(GetType(RASDIALPARAMS))
                ReDim .szEntryName(RAS_MaxEntryName)
                CopyMemory(.szEntryName(0), EntryName, EntryName.Length)
                ReDim .szUserName(UNLEN)
                CopyMemory(.szUserName(0), UserName, UserName.Length)
                ReDim .szPassword(PWLEN)
                CopyMemory(.szPassword(0), PassWord, PassWord.Length)
            End With
            Dim res As Integer = RasDial(vbNullString, vbNullString, lprasdialparams, 0, 0, hRasConn)
            If res = 0 Then
                Return hRasConn
            Else
                ErrMsg = "RasDial:" + res.ToString()
                Return IntPtr.Zero
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return IntPtr.Zero
        End Try
    End Function

    ''' <summary>
    ''' 断开当前拨号连接
    ''' </summary>
    ''' <param name="hwnd">连接句柄</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function DisConnection(ByVal hwnd As IntPtr) As Boolean
        Try
            If hwnd <> IntPtr.Zero Then
                Dim res As Integer = RasHangUp(hwnd)
                If res = 0 Then
                    Return True
                Else
                    ErrMsg = "RasHangUp:" + res.ToString()
                    Return False
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 断开所有拨号连接
    ''' </summary>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function DisConnectionAll() As Boolean
        Try
            Dim lngRetCode As Long
            Dim lpcb As Long
            Dim lpcConnections As Integer
            Dim intArraySize As Integer = 256
            Dim lprasConn(intArraySize - 1) As RASCONN
            lprasConn(0).dwSize = Marshal.SizeOf(GetType(RASCONN))
            lprasConn(0).hRasConn = IntPtr.Zero
            lpcb = intArraySize * Marshal.SizeOf(GetType(RASCONN))
            lngRetCode = RasEnumConnections(lprasConn(0), lpcb, lpcConnections)
            If lngRetCode = 0 Then
                If lpcConnections > 0 Then
                    For intLooper = 0 To lpcConnections - 1
                        If lprasConn(intLooper).hRasConn <> IntPtr.Zero Then
                            Try
                                RasHangUp(lprasConn(intLooper).hRasConn)
                            Catch

                            End Try
                        End If
                    Next
                    Return True
                Else
                    Return False
                End If
            Else
                ErrMsg = "RasHangUp:" + lngRetCode.ToString()
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 显示当前获取
    ''' </summary>
    ''' <value></value>
    ''' <returns>当前拨号连接名称</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ActiveConnection() As String()
        Get
            Try
                Dim pack As List(Of String) = New List(Of String)
                Dim lngRetCode As Integer
                Dim lpcb As Integer
                Dim lpcConnections As Integer
                Dim intArraySize As Integer = 256
                Dim lprasConn(intArraySize - 1) As RASCONN
                lprasConn(0).dwSize = Marshal.SizeOf(GetType(RASCONN))
                lprasConn(0).hRasConn = IntPtr.Zero
                lpcb = intArraySize * Marshal.SizeOf(GetType(RASCONN))
                lngRetCode = RasEnumConnections(lprasConn(0), lpcb, lpcConnections)
                If lngRetCode = 0 Then
                    If lpcConnections > 0 Then
                        For intLooper As Integer = 0 To lpcConnections - 1
                            Dim temp As String = lprasConn(intLooper).szEntryName
                            If Not pack.Contains(temp) Then
                                pack.Add(temp)
                            End If
                        Next
                    End If
                    If pack.Count > 0 Then
                        Dim res(pack.Count - 1) As String
                        pack.CopyTo(res)
                        Return res
                    Else
                        Return Nothing
                    End If
                Else
                    ErrMsg = "RasEnumConnections:" + lngRetCode.ToString
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取所有拨号连接
    ''' </summary>
    ''' <value></value>
    ''' <returns>连接名</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property AllConnection() As String()
        Get
            Try
                Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim lpConn As Integer = 0
                Dim len As Integer = 0
                Dim lpSize As Integer = Marshal.SizeOf(GetType(RasEntryName))
                Dim chk(0) As RasEntryName
                chk(0).dwSize = Marshal.SizeOf(GetType(RasEntryName))
                Dim res As Integer = RasEnumEntries(Nothing, Nothing, chk, lpSize, lpConn)
                If lpConn > 0 Then
                    len = lpConn
                    Dim lpConns(len - 1) As RasEntryName
                    For i As Integer = LBound(lpConns) To UBound(lpConns)
                        lpConns(i).dwSize = Marshal.SizeOf(GetType(RasEntryName))
                    Next
                    res = RasEnumEntries(vbNullString, vbNullString, lpConns, lpSize, lpConn)
                    If res = 0 Then
                        For i As Integer = LBound(lpConns) To UBound(lpConns)
                            List.Add(lpConns(i).szEntryName)
                        Next
                        Dim Filelist As String() = Nothing
                        If List.Count > 0 Then
                            ReDim Filelist(List.Count - 1)
                            List.CopyTo(Filelist)
                        End If
                        Return Filelist
                    Else
                        ErrMsg = "RasEnumEntries:" + res.ToString
                        Return Nothing
                    End If
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Get
    End Property

End Class