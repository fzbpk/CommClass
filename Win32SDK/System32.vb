Imports System.Management
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports IWshRuntimeLibrary
Imports Microsoft.Win32
Public Class System32

    Private Shared ErrMsg As String = ""

    Private Const SE_PRIVILEGE_ENABLED As Integer = &H2
    Private Const TOKEN_QUERY As Integer = &H8
    Private Const TOKEN_ADJUST_PRIVILEGES As Integer = &H20
    Private Const SE_SHUTDOWN_NAME As String = "SeShutdownPrivilege"
    Private Const EWX_LOGOFF As Integer = &H0 '注销计算机
    Private Const EWX_SHUTDOWN As Integer = &H1 '关闭计算机
    Private Const EWX_REBOOT As Integer = &H2 '重新启动计算机
    Private Const EWX_FORCE As Integer = &H4 '关闭所有进程，注销计算机
    Private Const EWX_POWEROFF As Integer = &H8
    Private Const EWX_FORCEIFHUNG As Integer = &H10
    Private Const CCDEVICENAME As Short = 32
    Private Const CCFORMNAME As Short = 32
    Private Const DM_PELSWIDTH As Integer = &H80000
    Private Const DM_PELSHEIGHT As Integer = &H100000
    Private Const DM_DISPLAYFREQUENCY As Integer = &H400000
    Private Const DM_BITSPERPEL As Integer = &H40000
    Private Const DM_DISPLAYORIENTATION As Integer = &H80
    Private Const ENUM_CURRENT_SETTINGS As Integer = -1
    Private Const WM_SYSCOMMAND As Integer = &H112
    Private Const SC_MONITORPOWER As Integer = &HF170
    Private Const WM_APPCOMMAND As Long = &H319
    Private Const MAXPNAMELEN As Integer = 32
    Private Const MIXER_LONG_NAME_CHARS As Integer = 64
    Private Const MIXER_SHORT_NAME_CHARS As Integer = 16
    Private Const MIXER_GETLINEINFOF_COMPONENTTYPE As Integer = &H3
    Private Const MIXER_GETCONTROLDETAILSF_VALUE As Integer = &H0
    Private Const MIXER_GETLINECONTROLSF_ONEBYTYPE As Integer = &H2
    Private Const MIXER_SETCONTROLDETAILSF_VALUE As Integer = &H0
    Private Const MIXERLINE_COMPONENTTYPE_DST_FIRST As Integer = &H0
    Private Const MIXERLINE_COMPONENTTYPE_SRC_FIRST As Integer = &H1000
    Private Const MIXERLINE_COMPONENTTYPE_DST_SPEAKERS As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 4)
    Private Const MIXERLINE_COMPONENTTYPE_SRC_MICROPHONE As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 3)
    Private Const MIXERLINE_COMPONENTTYPE_SRC_LINE As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 2)
    Private Const MIXERCONTROL_CT_CLASS_FADER As Integer = &H50000000
    Private Const MIXERCONTROL_CT_UNITS_UNSIGNED As Integer = &H30000
    Private Const MIXERCONTROL_CONTROLTYPE_FADER As Integer = (MIXERCONTROL_CT_CLASS_FADER Or MIXERCONTROL_CT_UNITS_UNSIGNED)
    Private Const MIXERCONTROL_CONTROLTYPE_VOLUME As Integer = (MIXERCONTROL_CONTROLTYPE_FADER + 1)
    Private Const MMSYSERR_NOERROR As Integer = 0
    Private Const GMEM_ZEROINIT = &H40

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


    Private Structure SYSTEMTIME
        Dim wYear As Integer
        Dim wMonth As Integer
        Dim wDayOfWeek As Integer
        Dim wDay As Integer
        Dim wHour As Integer
        Dim wMinute As Integer
        Dim wSecond As Integer
        Dim wMilliseconds As Integer
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Private Structure TokPriv1Luid
        Public Count As Integer
        Public Luid As Long
        Public Attr As Integer
    End Structure

    Private Structure DEVMODE
        <VBFixedString(CCDEVICENAME), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=CCDEVICENAME)> Public dmDeviceName As String
        Dim dmSpecVersion As Short
        Dim dmDriverVersion As Short
        Dim dmSize As Short
        Dim dmDriverExtra As Short
        Dim dmFields As Integer
        Dim dmOrientation As Short
        Dim dmPaperSize As Short
        Dim dmPaperLength As Short
        Dim dmPaperWidth As Short
        Dim dmScale As Short
        Dim dmCopies As Short
        Dim dmDefaultSource As Short
        Dim dmPrintQuality As Short
        Dim dmColor As Short
        Dim dmDuplex As Short
        Dim dmYResolution As Short
        Dim dmTTOption As Short
        Dim dmCollate As Short
        <VBFixedString(CCFORMNAME), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=CCFORMNAME)> Public dmFormName As String
        Dim dmUnusedPadding As Short
        Dim dmBitsPerPel As Short
        Dim dmPelsWidth As Integer
        Dim dmPelsHeight As Integer
        Dim dmDisplayFlags As Integer
        Dim dmDisplayFrequency As Integer
        Dim dmDisplayOrientation As Integer
    End Structure

    Private Structure _SYSTEM_POWER_STATUS
        Public ACLineStatus As PowerStatus
        Public BatteryFlag As Byte
        Public BatteryLifePercent As Byte
        Public Reserved As Byte
        Public BatteryLifeTime As Integer
        Public BatteryFullLifeTime As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
    Private Structure RAMP
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> _
        Public Red As UInt16()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> _
        Public Green As UInt16()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> _
        Public Blue As UInt16()
    End Structure

    Private Structure MIXERCAPS
        Public wMid As Integer
        Public wPid As Integer
        Public vDriverVersion As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)>
        Public szPname As String
        Public fdwSupport As Integer
        Public cDestinations As Integer
    End Structure

    Private Structure MIXERCONTROL
        Public cbStruct As Integer
        Public dwControlID As Integer
        Public dwControlType As Integer
        Public fdwControl As Integer
        Public cMultipleItems As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_SHORT_NAME_CHARS)>
        Public szShortName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_LONG_NAME_CHARS)>
        Public szName As String
        Public lMinimum As Integer
        Public lMaximum As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=10)>
        Public Reserved As Integer()
    End Structure

    Private Structure MIXERCONTROLDETAILS
        Public cbStruct As Integer
        Public dwControlID As Integer
        Public cChannels As Integer
        Public item As Integer
        Public cbDetails As Integer
        Public paDetails As IntPtr
    End Structure
    Private Structure MIXERCONTROLDETAILS_SIGNED
        Public lValue As Integer
    End Structure
    Private Structure MIXERCONTROLDETAILS_UNSIGNED
        Public dwValue As Integer
    End Structure
    Private Structure MIXERLINE
        Public cbStruct As Integer
        Public dwDestination As Integer
        Public dwSource As Integer
        Public dwLineID As Integer
        Public fdwLine As Integer
        Public dwUser As Integer
        Public dwComponentType As Integer
        Public cChannels As Integer
        Public cConnections As Integer
        Public cControls As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_SHORT_NAME_CHARS)>
        Public szShortName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_LONG_NAME_CHARS)>
        Public szName As String
        Public dwType As Integer
        Public dwDeviceID As Integer
        Public wMid As Integer
        Public wPid As Integer
        Public vDriverVersion As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)>
        Public szPname As String
    End Structure
    Private Structure MIXERLINECONTROLS
        Public cbStruct As Integer
        Public dwLineID As Integer
        Public dwControl As Integer
        Public cControls As Integer
        Public cbmxctrl As Integer
        Public pamxctrl As IntPtr
    End Structure

    Private Structure MCI_OPEN_PARMS
        Public dwCallback As Integer
        Public wDeviceID As IntPtr
        Public lpstrDeviceType As String
        Public lpstrElementName As String
        Public lpstrAlias As String
    End Structure

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

    Public Enum DISPLAYORIENTATION
        DMDO_0 = 0
        DMDO_90 = 1
        DMDO_180 = 2
        DMDO_270 = 3
    End Enum

    Public Enum PowerStatus As Byte
        Battery = 0
        AC = 1
        Unknow = 255
    End Enum

    Private Declare Function SetLocalTime Lib "Kernel32" (ByRef sysTime As SYSTEMTIME) As Boolean
    Private Declare Function SetSystemTime Lib "kernel32" (ByVal lpSystemTime As SYSTEMTIME) As Long
    Private Declare Function GetCurrentProcess Lib "kernel32" () As IntPtr
    Private Declare Function LoadLibrary Lib "kernel32" Alias "LoadLibraryA" (ByVal lpLibFileName As String) As IntPtr
    Private Declare Function FreeLibrary Lib "kernel32" (ByVal hLibModule As IntPtr) As Integer
    Private Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As IntPtr, ByVal lpProcName As String) As IntPtr
    Private Declare Function GlobalUnlock Lib "kernel32" (ByVal hMem As Integer) As Integer
    Private Declare Function GlobalAlloc Lib "kernel32" (ByVal wFlags As Integer, ByVal dwBytes As Integer) As Integer
    Private Declare Function GlobalLock Lib "kernel32" (ByVal hMem As Integer) As Integer
    Private Declare Function GlobalFree Lib "kernel32" (ByVal hMem As Integer) As Integer

    Private Declare Function SetDeviceGammaRamp Lib "gdi32" (ByVal hDC As IntPtr, ByRef lpRamp As RAMP) As Boolean
    Private Declare Function GetDC Lib "user32" (ByVal hWnd As IntPtr) As IntPtr
    Private Declare Function GetDeviceGammaRamp Lib "gdi32" (ByVal hdc As Int32, ByVal lpv As Int32) As Int32

    Private Declare Function OpenProcessToken Lib "advapi32" (ByVal h As IntPtr, ByVal acc As Integer, ByRef phtok As IntPtr) As Boolean
    Private Declare Function LookupPrivilegeValue Lib "advapi32" Alias "LookupPrivilegeValueA" (ByVal host As String, ByVal name As String, ByRef pluid As Long) As Boolean
    Private Declare Function AdjustTokenPrivileges Lib "advapi32" (ByVal htok As IntPtr, ByVal disall As Boolean, ByRef newst As TokPriv1Luid, ByVal len As Integer, ByVal prev As IntPtr, ByVal relen As IntPtr) As Boolean
    Private Declare Function ExitWindows Lib "user32" (ByVal flg As Integer, ByVal rea As Integer) As Boolean
    Private Declare Function ExitWindowsEx Lib "user32" (ByVal uFlags As Integer, ByVal dwReserved As Integer) As Boolean
    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    Private Declare Function PostMessage Lib "user32.dll" Alias "PostMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByRef wParam As IntPtr, ByVal lParam As String) As Integer
    Private Declare Function SetSuspendState Lib "Powrprof" (ByVal Hibernate As Integer, ByVal ForceCritical As Integer, ByVal DisableWakeEvent As Integer) As Integer
    Private Declare Function ChangeDisplaySettings Lib "user32" Alias "ChangeDisplaySettingsA" (ByRef lpDevMode As DEVMODE, ByVal dwflags As Integer) As Integer
    Private Declare Function EnumDisplaySettings Lib "user32" Alias "EnumDisplaySettingsA" (ByVal lpszDeviceName As Integer, ByVal iModeNum As Integer, ByRef lpDevMode As DEVMODE) As Boolean
    Private Declare Function mixerClose Lib "winmm.dll" (ByVal hmx As Integer) As Integer
    Private Declare Function mixerGetControlDetails Lib "winmm.dll" Alias "mixerGetControlDetailsA" (ByVal hmxobj As Integer, ByRef pmxcd As MIXERCONTROLDETAILS, ByVal fdwDetails As Integer) As Integer
    Private Declare Function mixerGetDevCaps Lib "winmm.dll" Alias "mixerGetDevCapsA" (ByVal uMxId As Integer, ByRef pmxcaps As MIXERCAPS, ByVal cbmxcaps As Integer) As Integer
    Private Declare Function mixerGetID Lib "winmm.dll" (ByVal hmxobj As Integer, ByVal pumxID As Integer, ByVal fdwId As Integer) As Integer
    Private Declare Function mixerGetLineInfo Lib "winmm.dll" Alias "mixerGetLineInfoA" (ByVal hmxobj As Integer, ByRef pmxl As MIXERLINE, ByVal fdwInfo As Integer) As Integer
    Private Declare Function mixerGetLineControls Lib "winmm.dll" Alias "mixerGetLineControlsA" (ByVal hmxobj As Integer, ByRef pmxlc As MIXERLINECONTROLS, ByVal fdwControls As Integer) As Integer
    Private Declare Function mixerOpen Lib "winmm.dll" (ByRef phmx As Integer, ByVal uMxId As Integer, ByVal dwCallback As Integer, ByVal dwInstance As Integer, ByVal fdwOpen As Integer) As Integer

    Private Declare Function waveOutSetVolume Lib "Winmm" (ByVal hwo As Integer, ByVal pdwVolume As System.UInt32) As Integer
    Private Declare Function waveOutGetVolume Lib "Winmm" (ByVal hwo As Integer, ByRef pdwVolume As System.UInt32) As Integer
    Private Declare Function mciSendCommand Lib "winmm" Alias "mciSendCommandA" (ByVal wDeviceID As Integer, ByVal uMessage As Integer, ByVal dwParam1 As Integer, ByRef dwParam2 As MCI_OPEN_PARMS) As Integer

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto), System.Security.SuppressUnmanagedCodeSecurity()> _
    Private Shared Function GetSystemPowerStatus(ByRef status As _SYSTEM_POWER_STATUS) As Boolean
    End Function


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
    Private Declare Function CM_Locate_DevNodeA Lib "setupapi.dll" (ByRef pdnDevInst As IntPtr, ByVal DeviceInstanceId As String, ByVal ulFlags As Integer) As Integer

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



    Private Shared Function DoExitWin(ByVal flg As Integer) As Boolean
        Try
            Dim xc As Boolean '判断语句
            Dim tp As TokPriv1Luid
            Dim hproc As IntPtr = GetCurrentProcess()
            '调用进程值
            Dim htok As IntPtr = IntPtr.Zero
            xc = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES Or TOKEN_QUERY, htok)
            tp.Count = 1
            tp.Luid = 0
            tp.Attr = SE_PRIVILEGE_ENABLED
            xc = LookupPrivilegeValue(Nothing, SE_SHUTDOWN_NAME, tp.Luid)
            xc = AdjustTokenPrivileges(htok, False, tp, 0, IntPtr.Zero, IntPtr.Zero)
            xc = ExitWindowsEx(flg, 0)
            Return xc
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

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

    Private Shared Function GetVolumeControl(ByVal hmixer As Integer, ByVal componentType As Integer, ByVal ctrlType As Integer, ByRef mxc As MIXERCONTROL, ByRef vCurrentVol As Integer) As Boolean
        Dim mxlc As MIXERLINECONTROLS = New MIXERLINECONTROLS()
        Dim mxl As MIXERLINE = New MIXERLINE()
        Dim pmxcd As MIXERCONTROLDETAILS = New MIXERCONTROLDETAILS()
        Dim du As MIXERCONTROLDETAILS_UNSIGNED = New MIXERCONTROLDETAILS_UNSIGNED()
        mxc = New MIXERCONTROL()
        Dim rc As Integer = 0
        Dim retValue As Boolean = False
        vCurrentVol = -1
        mxl.cbStruct = Marshal.SizeOf(mxl)
        mxl.dwComponentType = componentType
        mxl.szName = ""
        mxl.szPname = ""
        mxl.szShortName = ""
        rc = mixerGetLineInfo(hmixer, mxl, MIXER_GETLINEINFOF_COMPONENTTYPE)
        If MMSYSERR_NOERROR = rc Then
            Dim sizeofMIXERCONTROL As Integer = 152
            Dim ctrl As Integer = Marshal.SizeOf(GetType(MIXERCONTROL))
            mxlc.pamxctrl = Marshal.AllocCoTaskMem(sizeofMIXERCONTROL)
            mxlc.cbStruct = Marshal.SizeOf(mxlc)
            mxlc.dwLineID = mxl.dwLineID
            mxlc.dwControl = ctrlType
            mxlc.cControls = 1
            mxlc.cbmxctrl = sizeofMIXERCONTROL
            rc = mixerGetLineControls(hmixer, mxlc, MIXER_GETLINECONTROLSF_ONEBYTYPE)
            If MMSYSERR_NOERROR = rc Then
                retValue = True
                mxc = CType(Marshal.PtrToStructure(mxlc.pamxctrl, GetType(MIXERCONTROL)), MIXERCONTROL)
            Else
                retValue = False
            End If
            Dim sizeofMIXERCONTROLDETAILS As Integer = Marshal.SizeOf(GetType(MIXERCONTROLDETAILS))
            Dim sizeofMIXERCONTROLDETAILS_UNSIGNED As Integer = Marshal.SizeOf(GetType(MIXERCONTROLDETAILS_UNSIGNED))
            pmxcd.cbStruct = sizeofMIXERCONTROLDETAILS
            pmxcd.dwControlID = mxc.dwControlID
            pmxcd.cChannels = 1
            pmxcd.item = 0
            pmxcd.cbDetails = Len(du.dwValue)
            pmxcd.paDetails = Marshal.AllocCoTaskMem(sizeofMIXERCONTROLDETAILS_UNSIGNED)
            rc = mixerGetControlDetails(hmixer, pmxcd, MIXER_GETCONTROLDETAILSF_VALUE)
            du = CType(Marshal.PtrToStructure(pmxcd.paDetails, GetType(MIXERCONTROLDETAILS_UNSIGNED)), MIXERCONTROLDETAILS_UNSIGNED)
            vCurrentVol = du.dwValue
            Return retValue
        End If
        retValue = False
        Return retValue
    End Function

    Private Shared Function SetVolumeControl(ByVal hmixer As Integer, ByVal mxc As MIXERCONTROL, ByVal volume As Integer) As Boolean
        Dim rc As Integer = 0
        Dim mxcd As MIXERCONTROLDETAILS = New MIXERCONTROLDETAILS()
        Dim vol As MIXERCONTROLDETAILS_UNSIGNED = New MIXERCONTROLDETAILS_UNSIGNED()
        mxcd.item = 0
        mxcd.dwControlID = mxc.dwControlID
        mxcd.cbStruct = Marshal.SizeOf(mxcd)
        mxcd.cbDetails = Marshal.SizeOf(vol)
        mxcd.cChannels = 1
        vol.dwValue = volume
        mxcd.paDetails = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(MIXERCONTROLDETAILS_UNSIGNED)))
        Marshal.StructureToPtr(vol, mxcd.paDetails, False)
        rc = mixerGetControlDetails(hmixer, mxcd, MIXER_SETCONTROLDETAILSF_VALUE)
        If MMSYSERR_NOERROR = rc Then
            Return True
        Else
            Return False
        End If
    End Function

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
    ''' 设置时间
    ''' </summary>
    ''' <param name="NewTime">设置的时间</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SetTime(ByVal NewTime As Date) As Boolean
        Try
            Dim time As SYSTEMTIME
            With time
                .wYear = NewTime.Year
                .wMonth = NewTime.Month
                .wDay = NewTime.Day
                .wHour = NewTime.Hour
                .wMinute = NewTime.Minute
                .wSecond = NewTime.Second
                .wMilliseconds = NewTime.Millisecond
            End With
            SetSystemTime(time)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    Public Shared Function SetRTCTime(ByVal NewTime As Date) As Boolean
        Try
            Dim time As SYSTEMTIME
            With time
                .wYear = NewTime.Year
                .wMonth = NewTime.Month
                .wDay = NewTime.Day
                .wHour = NewTime.Hour
                .wMinute = NewTime.Minute
                .wSecond = NewTime.Second
                .wMilliseconds = NewTime.Millisecond
            End With
            Return SetLocalTime(time)

        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 重启
    ''' </summary>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function Reboot() As Boolean
        Return DoExitWin((EWX_FORCE Or EWX_REBOOT)) '重新启动计算机
    End Function

    ''' <summary>
    ''' 关机
    ''' </summary>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function PowerOff() As Boolean
        Return DoExitWin((EWX_FORCE Or EWX_POWEROFF)) '关闭计算机
    End Function

    ''' <summary>
    ''' 注销
    ''' </summary>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function LogoOff() As Boolean
        Return DoExitWin((EWX_FORCE Or EWX_LOGOFF)) '注销计算机
    End Function

    ''' <summary>
    ''' 睡眠
    ''' </summary>
    ''' <param name="force">强制关机，TRUE是，FALSE否</param>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function Suspend(Optional ByVal force As Boolean = True) As Boolean
        Try
            If Not CheckEntryPoint("powrprof.dll", "SetSuspendState") Then
                ErrMsg = "Not Supported"
                Return False
            End If
            SetSuspendState(0, Convert.ToInt32(IIf(force, 1, 0)), 0)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 休眠
    ''' </summary>
    ''' <param name="force">强制关机，TRUE是，FALSE否</param>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function Hibernate(Optional ByVal force As Boolean = True) As Boolean
        Try
            If Not CheckEntryPoint("powrprof.dll", "SetSuspendState") Then
                ErrMsg = "Not Supported"
                Return False
            End If
            SetSuspendState(1, Convert.ToInt32(IIf(force, 1, 0)), 0)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取电源状态
    ''' </summary>
    ''' <param name="BatteryLeft">返回电池剩余百分比</param>
    ''' <returns>返回电源状态，使用AC电源，使用电池</returns>
    ''' <remarks></remarks>
    Public Shared Function Power(ByRef BatteryLeft As Byte) As PowerStatus
        Try
            Dim status As _SYSTEM_POWER_STATUS
            status.ACLineStatus = PowerStatus.Unknow
            status.BatteryFlag = 0
            status.BatteryFullLifeTime = 0
            status.BatteryLifePercent = 0
            status.BatteryLifeTime = 0
            status.Reserved = 0
            If GetSystemPowerStatus(status) Then
                BatteryLeft = status.BatteryLifePercent
                Return status.ACLineStatus
            Else
                Return PowerStatus.Unknow
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return PowerStatus.Unknow
        End Try
    End Function

    ''' <summary>
    ''' 创建快捷方式
    ''' </summary>
    ''' <param name="AppSrc">应用程序路径</param>
    ''' <param name="TarSrc">目标路径</param>
    ''' <param name="Arg">参数</param>
    ''' <param name="Memo">注释</param>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function CreatLnk(ByVal AppSrc As String, ByVal TarSrc As String, ByVal Arg As String, ByVal Memo As String) As Boolean
        Try
            Dim wsh As New IWshShell_Class
            If AppSrc.ToString.Trim <> "" Then
                Dim RootPath As String = ""
                Dim AppName As String = ""
                If IO.File.Exists(AppSrc) Then
                    Dim fi As New IO.FileInfo(AppSrc)
                    RootPath = fi.DirectoryName
                    AppName = Replace(fi.Name.ToLower, fi.Extension.ToLower, "")
                    If TarSrc.ToString.Trim <> "" Then
                        If TarSrc.EndsWith("\") Then
                            TarSrc = Left(TarSrc, TarSrc.Length - 1)
                        End If
                    Else
                        TarSrc = wsh.SpecialFolders.Item("Desktop")
                    End If
                    Dim desk As String = TarSrc  '从SHELL枚举中获取桌面路径
                    Dim lnk As IWshShortcut = wsh.CreateShortcut(desk & "\" + AppName + ".lnk") '在桌面上创建说明文件的路径，注意扩展名为 .lnk
                    With lnk
                        .Arguments = Arg '传递参数
                        .Description = Memo
                        .TargetPath = AppSrc '目标文件路径
                        .WindowStyle = 1 '打开窗体的风格
                        .WorkingDirectory = RootPath '工作路径
                        .Save() '保存快捷方式
                    End With
                    Return True
                Else
                    ErrMsg = "程序不存在"
                    Return False
                End If
            Else
                ErrMsg = "文件路径为空"
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    '''  判断快捷方式是否存在
    ''' </summary>
    ''' <param name="AppSrc">应用程序路径</param>
    ''' <param name="TarSrc">目标路径</param>
    ''' <returns>TRUE成功，FALSE失败，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function ExitLnk(ByVal AppSrc As String, ByVal TarSrc As String) As Boolean
        Try
            Dim wsh As New IWshShell_Class
            If AppSrc.ToString.Trim <> "" Then
                Dim RootPath As String = ""
                Dim AppName As String = ""
                If IO.File.Exists(AppSrc) Then
                    Dim fi As New IO.FileInfo(AppSrc)
                    RootPath = fi.DirectoryName
                    AppName = Replace(fi.Name.ToLower, fi.Extension.ToLower, "")
                    If TarSrc.ToString.Trim <> "" Then
                        If TarSrc.EndsWith("\") Then
                            TarSrc = Left(TarSrc, TarSrc.Length - 1)
                        End If
                    Else
                        TarSrc = wsh.SpecialFolders.Item("Desktop")
                    End If
                    If IO.File.Exists(TarSrc + "\" + AppName + ".lnk") Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    ErrMsg = "程序不存在"
                    Return False
                End If
            Else
                ErrMsg = "文件路径为空"
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 命令行
    ''' </summary>
    ''' <param name="PathName">执行命令</param>
    ''' <param name="style">窗口调用风格</param>
    ''' <param name="TimeOut">超时时间，为0则使用异步方式不等待超时</param>
    ''' <returns>返回的调用句柄</returns>
    ''' <remarks></remarks>
    Public Shared Function Shell(ByVal PathName As String, Optional ByVal style As System.Diagnostics.ProcessWindowStyle = ProcessWindowStyle.Hidden, Optional ByVal TimeOut As Integer = 0) As IntPtr
        Dim winstlye As Microsoft.VisualBasic.AppWinStyle = AppWinStyle.Hide
        Select Case style
            Case ProcessWindowStyle.Hidden
                winstlye = AppWinStyle.Hide
            Case ProcessWindowStyle.Maximized
                winstlye = AppWinStyle.MaximizedFocus
            Case ProcessWindowStyle.Minimized
                winstlye = AppWinStyle.MinimizedNoFocus
            Case ProcessWindowStyle.Normal
                winstlye = AppWinStyle.NormalFocus
            Case Else
                winstlye = AppWinStyle.Hide
        End Select
        If TimeOut > 0 Then
            Return Microsoft.VisualBasic.Interaction.Shell(PathName, style, True, TimeOut)
        Else
            Return Microsoft.VisualBasic.Interaction.Shell(PathName, style)
        End If
    End Function

    ''' <summary>
    ''' 获取串口名称
    ''' </summary>
    ''' <returns>串口名称数组</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property SerialComList() As String()
        Get
            Dim results As String() = {}
            Try
                Dim keycom As RegistryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\DEVICEMAP\SERIALCOMM")
                Dim str As String
                If Not keycom Is Nothing Then
                    Dim sSubKeys() As String = keycom.GetValueNames()
                    For Each str In sSubKeys
                        ReDim Preserve results(results.Length())
                        results(results.Length() - 1) = keycom.GetValue(str).ToString.Trim
                    Next
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
            End Try
            Return results
        End Get
    End Property

    ''' <summary>
    ''' 获取并口名称
    ''' </summary>
    ''' <returns>并口名称数组</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ParallelComList() As String()
        Get
            Dim results As String() = {}
            Try
                Dim keycom As RegistryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\DEVICEMAP\PARALLEL PORTS")
                Dim str As String
                If Not keycom Is Nothing Then
                    Dim sSubKeys() As String = keycom.GetValueNames()
                    For Each str In sSubKeys
                        Dim strtmp As String = keycom.GetValue(str).ToString.Trim
                        If strtmp.ToLower.Contains("lpt") Then
                            ReDim Preserve results(results.Length())
                            If strtmp.ToLower.Contains("\") Then
                                Dim temp As String() = strtmp.Split("\")
                                results(results.Length() - 1) = temp(UBound(temp)).ToUpper
                            End If
                        Else
                            results(results.Length() - 1) = strtmp.ToUpper
                        End If
                    Next
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
            End Try
            Return results
        End Get
    End Property

    ''' <summary>
    ''' 更改屏幕分辨率
    ''' </summary>
    ''' <param name="iWidth">长</param>
    ''' <param name="iHeight">宽</param>
    ''' <param name="frq">刷新率</param>
    ''' <returns>TRUE成功，FALSE失败，，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function ChangeResolution(ByVal iWidth As UInteger, ByVal iHeight As UInteger, ByVal frq As UShort, ByVal ColorDeapth As UShort) As Boolean
        Try
            Const DISP_CHANGE_SUCCESSFUL As Integer = 0
            Const DISP_CHANGE_RESTART As Integer = 1
            Const DISP_CHANGE_FAILED As Integer = -1
            Const DISP_CHANGE_BADMODE As Integer = -2
            Const DISP_CHANGE_NOTUPDATED As Integer = -3
            Const DISP_CHANGE_BADFLAGS As Integer = -4
            Const DISP_CHANGE_BADPARAM As Integer = -5
            Const DISP_CHANGE_BADDUALVIEW As Integer = -6
            If Not CheckEntryPoint("user32.dll", "EnumDisplaySettingsA") Or Not CheckEntryPoint("user32.dll", "ChangeDisplaySettingsA") Then
                ErrMsg = "Not Supported"
                Return False
            End If
            Dim blnworked As Boolean
            Dim devm As DEVMODE
            blnworked = EnumDisplaySettings(Nothing, ENUM_CURRENT_SETTINGS, devm)
            With devm
                .dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))
                .dmFields = DM_PELSWIDTH Or DM_PELSHEIGHT Or DM_DISPLAYFREQUENCY Or DM_BITSPERPEL
                .dmPelsWidth = iWidth
                .dmPelsHeight = iHeight
                .dmDisplayFrequency = frq
                .dmBitsPerPel = ColorDeapth
            End With
            Dim res As Integer = ChangeDisplaySettings(devm, 0)
            Select Case res
                Case DISP_CHANGE_SUCCESSFUL
                    Return True
                Case DISP_CHANGE_RESTART
                    Return True
                Case DISP_CHANGE_FAILED
                    ErrMsg = "指定图形模式的显示驱动失效"
                    Return False
                Case DISP_CHANGE_BADFLAGS
                    ErrMsg = "标志的无效设置被传送"
                    Return False
                Case DISP_CHANGE_NOTUPDATED
                    ErrMsg = "在WindowsNT中不能把设置写入注册表"
                    Return False
                Case DISP_CHANGE_FAILED
                    ErrMsg = "一个无效的参数被传递。它可以包括一个无效的标志或标志的组合"
                    Return False
                Case DISP_CHANGE_BADMODE
                    ErrMsg = "不支持图形模式"
                    Return False
                Case Else
                    ErrMsg = "未知错误：" + res.ToString
                    Return False
            End Select
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    Public Shared Function ReadResolution(ByRef iWidth As Single, ByRef iHeight As Single, ByRef frq As UShort, ByRef ColorDeapth As UShort) As Boolean
        Try


            Dim DevM As New DEVMODE()
            DevM.dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))
            Dim mybool As Boolean
            mybool = EnumDisplaySettings(Nothing, 0, DevM)
            iWidth = DevM.dmPelsWidth  '宽 
            iHeight = DevM.dmPelsHeight '高  
            frq = DevM.dmDisplayFrequency  '刷新频率  
            ColorDeapth = DevM.dmBitsPerPel  '颜色象素
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 显示器电源
    ''' </summary>
    ''' <param name="SW">开关，TRUE开，FALSE关</param>
    ''' <returns>TRUE成功，FALSE失败，，可通过错误查询信息</returns>
    ''' <remarks></remarks>
    Public Shared Function MoniterPower(ByVal SW As Boolean) As Boolean
        Try
            Dim HWND_BROADCAST As IntPtr = New IntPtr(&HFFFF)
            Const MONITOR_OFF = 2
            Const MONITOR_ON = -1
            Dim res As Integer = 0
            If SW Then
                res = SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_ON)
            Else
                res = SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF)
            End If
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 设置伽马值
    ''' </summary>
    ''' <param name="gamma">值，5-35，默认10</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SetGamma(ByVal gamma As Integer) As Boolean
        Try
            Dim s_ramp As New RAMP()
            s_ramp.Red = New UShort(255) {}
            s_ramp.Green = New UShort(255) {}
            s_ramp.Blue = New UShort(255) {}
            For i As Integer = 1 To 255
                ' gamma is a value between 3 and 44 
                s_ramp.Red(i) = InlineAssignHelper(s_ramp.Green(i), InlineAssignHelper(s_ramp.Blue(i), CUShort((Math.Min(65535, Math.Max(0, Math.Pow((i + 1) / 256.0R, gamma * 0.1) * 65535 + 0.5))))))
            Next
            ' Now set the value. 
            SetDeviceGammaRamp(GetDC(IntPtr.Zero), s_ramp)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 改变显示方向
    ''' </summary>
    ''' <param name="Angle">方向</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ChangeDirection(ByVal Angle As DISPLAYORIENTATION) As Boolean
        Try
            Const DISP_CHANGE_SUCCESSFUL As Integer = 0
            Const DISP_CHANGE_RESTART As Integer = 1
            Const DISP_CHANGE_FAILED As Integer = -1
            Const DISP_CHANGE_BADMODE As Integer = -2
            Const DISP_CHANGE_NOTUPDATED As Integer = -3
            Const DISP_CHANGE_BADFLAGS As Integer = -4
            Const DISP_CHANGE_BADPARAM As Integer = -5
            Const DISP_CHANGE_BADDUALVIEW As Integer = -6
            If Not CheckEntryPoint("user32.dll", "EnumDisplaySettingsA") Or Not CheckEntryPoint("user32.dll", "ChangeDisplaySettingsA") Then
                ErrMsg = "Not Supported"
                Return False
            End If
            Dim blnworked As Boolean
            Dim devm As DEVMODE
            blnworked = EnumDisplaySettings(Nothing, ENUM_CURRENT_SETTINGS, devm)
            With devm
                .dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))
                .dmFields = DM_DISPLAYORIENTATION
                .dmDisplayOrientation = Angle
            End With
            Dim res As Integer = ChangeDisplaySettings(devm, 0)
            Select Case res
                Case DISP_CHANGE_SUCCESSFUL
                    Return True
                Case DISP_CHANGE_RESTART
                    Return True
                Case DISP_CHANGE_FAILED
                    ErrMsg = "指定图形模式的显示驱动失效"
                    Return False
                Case DISP_CHANGE_BADFLAGS
                    ErrMsg = "标志的无效设置被传送"
                    Return False
                Case DISP_CHANGE_NOTUPDATED
                    ErrMsg = "在WindowsNT中不能把设置写入注册表"
                    Return False
                Case DISP_CHANGE_FAILED
                    ErrMsg = "一个无效的参数被传递。它可以包括一个无效的标志或标志的组合"
                    Return False
                Case DISP_CHANGE_BADMODE
                    ErrMsg = "不支持图形模式"
                    Return False
                Case Else
                    ErrMsg = "未知错误：" + res.ToString
                    Return False
            End Select
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    Private Shared Property Volume() As Integer
        Get
            Dim mixer As Integer = 0
            Dim volCtrl As MIXERCONTROL = New MIXERCONTROL()
            Dim currentVol As Integer = 0
            mixerOpen(mixer, 0, 0, 0, 0)
            Dim type As Integer = MIXERCONTROL_CONTROLTYPE_VOLUME
            GetVolumeControl(mixer, MIXERLINE_COMPONENTTYPE_DST_SPEAKERS, type, volCtrl, currentVol)
            mixerClose(mixer)
            Return currentVol
        End Get
        Set(ByVal value As Integer)
            Dim mixer As Integer = 0
            Dim volCtrl As MIXERCONTROL = New MIXERCONTROL()
            Dim currentVol As Integer = 0
            mixerOpen(mixer, 0, 0, 0, 0)
            Dim type As Integer = MIXERCONTROL_CONTROLTYPE_VOLUME
            GetVolumeControl(mixer, MIXERLINE_COMPONENTTYPE_DST_SPEAKERS, type, volCtrl, currentVol)
            If (value > volCtrl.lMaximum) Then value = volCtrl.lMaximum
            If (value < volCtrl.lMinimum) Then value = volCtrl.lMinimum
            SetVolumeControl(mixer, volCtrl, value)
            GetVolumeControl(mixer, MIXERLINE_COMPONENTTYPE_DST_SPEAKERS, type, volCtrl, currentVol)
            If value <> currentVol Then
                ErrMsg = "设置失败"
            End If
            mixerClose(mixer)
        End Set
    End Property


    ''' <summary>
    ''' 调大音量
    ''' </summary>
    ''' <param name="hwd">当前句柄</param>
    ''' <remarks></remarks>
    Public Shared Sub VolumeUp(ByVal hwd As IntPtr)
        Try
            Const APPCOMMAND_VOLUME_UP As Long = 10
            Dim res As Integer = 0
            res = SendMessage(hwd, WM_APPCOMMAND, &H30292, APPCOMMAND_VOLUME_UP * &H10000)
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 调小音量
    ''' </summary>
    ''' <param name="hwd">当前句柄</param>
    ''' <remarks></remarks>
    Public Shared Sub VolumeDown(ByVal hwd As IntPtr)
        Try
            Const APPCOMMAND_VOLUME_DOWN As Long = 9
            Dim res As Integer = 0
            res = SendMessage(hwd, WM_APPCOMMAND, &H30292, APPCOMMAND_VOLUME_DOWN * &H10000)
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 静音
    ''' </summary>
    ''' <param name="hwd">当前句柄</param>
    ''' <remarks></remarks>
    Public Shared Sub Mute(ByVal hwd As IntPtr)
        Try
            Const APPCOMMAND_VOLUME_MUTE As Long = 8
            Dim res As Integer = 0
            res = SendMessage(hwd, WM_APPCOMMAND, &H200EB0, APPCOMMAND_VOLUME_MUTE * &H10000)
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 弹出U盘
    ''' </summary>
    ''' <param name="DriveLetter">盘符</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function EjectUsbRemoveDisk(ByVal DriveLetter As String) As Boolean
        Try
            If DriveLetter Is Nothing Then
                Return False
            ElseIf DriveLetter.Trim.Length = 0 Then
                Return False
            Else
                DriveLetter = Left(DriveLetter, 1) + ":\"
            End If
            Dim di As System.IO.DriveInfo = New System.IO.DriveInfo(DriveLetter)
            Dim LogicalDiskToPartitions As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition ")
            If di.DriveType = IO.DriveType.Removable And di.IsReady Then
                Dim res As Boolean = False
                For Each obj As ManagementObject In LogicalDiskToPartitions.Get
                    If obj("Dependent").ToString.ToUpper.Contains(DriveLetter.ToUpper.Replace("\", "")) Then
                        Dim inValue As String = obj("Antecedent").ToString
                        Dim parsedValue As String = inValue.Substring(inValue.IndexOf(Chr(34)) + 1, (inValue.LastIndexOf(Chr(34)) - (inValue.IndexOf(Chr(34)))) - 1)
                        parsedValue = Left(parsedValue, parsedValue.IndexOf(",")).ToLower.Replace("disk #", "")
                        Dim DiskDrives As New System.Management.ManagementObjectSearcher("Select * from Win32_DiskDrive where Name like ""%PHYSICALDRIVE" + parsedValue + "%"" and InterfaceType like ""%usb%"" ")
                        For Each obj2 As ManagementObject In DiskDrives.Get
                            Dim PNPDeviceID As String = obj2("PNPDeviceID").ToString
                            PNPDeviceID = Left(Right(PNPDeviceID, PNPDeviceID.Length - PNPDeviceID.LastIndexOf("\") - 1), Right(PNPDeviceID, PNPDeviceID.Length - PNPDeviceID.LastIndexOf("\") - 1).IndexOf("&"))
                            Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_USBHub where PNPDeviceID like ""%" + PNPDeviceID + "%"" ")
                            For Each obj3 As System.Management.ManagementObject In opt.Get
                                Dim DeviceID As String = obj3("DeviceID").ToString
                                Dim dwDevInst As IntPtr = IntPtr.Zero
                                Dim lngRet As Integer = CM_Locate_DevNodeA(dwDevInst, DeviceID, 0)
                                If lngRet = 0 Then
                                    lngRet = CM_Request_Device_Eject(dwDevInst, 0, 0, 0, 0)
                                    If lngRet = 0 Then
                                        res = True
                                    Else
                                        res = False
                                    End If
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                Next
                Return res
            Else
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function


    ''' <summary>
    ''' 弹出光盘
    ''' </summary>
    ''' <param name="DriveLetter">光驱盘符</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function EjectCDRom(ByVal DriveLetter As String) As Boolean
        Try
            If DriveLetter Is Nothing Then
                Return False
            ElseIf DriveLetter.Trim.Length = 0 Then
                Return False
            Else
                DriveLetter = Left(DriveLetter, 1) + ":\"
            End If
            Dim OpenParms As New MCI_OPEN_PARMS
            OpenParms.lpstrDeviceType = "CDAUDIO".ToLower
            OpenParms.lpstrElementName = DriveLetter.ToUpper
            Dim res As String = mciSendCommand(0, MCI_OPEN, MCI_OPEN_ELEMENT Or MCI_OPEN_TYPE Or MCI_OPEN_SHAREABLE, OpenParms)
            If res = 0 Then
                res = mciSendCommand(OpenParms.wDeviceID, MCI_SET, MCI_WAIT Or MCI_SET_DOOR_OPEN, Nothing)
                If res = 0 Then
                    Return True
                Else
                    ErrMsg = "mciSendCommand:" + res.ToString
                    Return False
                End If
            Else
                ErrMsg = "mciSendCommand:" + res.ToString
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 关闭光盘，手提式不支持
    ''' </summary>
    ''' <param name="DriveLetter">光驱盘符</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function CloseCDRom(ByVal DriveLetter As String) As Boolean
        Try
            If DriveLetter Is Nothing Then
                Return False
            ElseIf DriveLetter.Trim.Length = 0 Then
                Return False
            Else
                DriveLetter = Left(DriveLetter, 1) + ":\"
            End If
            Dim OpenParms As New MCI_OPEN_PARMS
            OpenParms.lpstrDeviceType = "CDAUDIO".ToLower
            OpenParms.lpstrElementName = DriveLetter.ToUpper
            Dim res As String = mciSendCommand(0, MCI_OPEN, MCI_OPEN_ELEMENT Or MCI_OPEN_TYPE Or MCI_OPEN_SHAREABLE, OpenParms)
            If res = 0 Then
                res = mciSendCommand(OpenParms.wDeviceID, MCI_SET, MCI_WAIT Or MCI_SET_DOOR_CLOSED, Nothing)
                If res = 0 Then
                    Return True
                Else
                    ErrMsg = "mciSendCommand:" + res.ToString
                    Return False
                End If
            Else
                ErrMsg = "mciSendCommand:" + res.ToString
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

     


End Class