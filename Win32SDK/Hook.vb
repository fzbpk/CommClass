Imports System.io
Imports System.text
Imports System.Text.Encoding
Imports System.Windows.Forms
    Public Class Hook
        Private Delegate Function EnumCallBackDelegate(ByVal hwnd As Integer, ByVal lParam As Integer) As Integer
        Private Declare Function EnumChildWindows Lib "user32" (ByVal hWndParent As Integer, ByVal lpEnumFunc As EnumCallBackDelegate, ByVal lParam As Integer) As Integer
        Private Declare Auto Function FindWindow Lib "user32" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
        Declare Function FindWindowEx Lib "user32" Alias "FindWindowExA" (ByVal hWnd1 As Integer, ByVal hWnd2 As Integer, ByVal lpsz1 As String, ByVal lpsz2 As String) As IntPtr
        Declare Auto Function SendMessage Lib "user32.dll" _
                              (ByVal hwnd As IntPtr, ByVal wMsg As Integer, _
                              ByRef wParam As IntPtr, ByVal lParam As String) As Integer
        Declare Auto Function PostMessage Lib "user32.dll" _
                             (ByVal hwnd As IntPtr, ByVal wMsg As Integer, _
                             ByRef wParam As IntPtr, ByVal lParam As String) As Integer
        Private Declare Function PostMessageInt Lib "user32" Alias "PostMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
        Private Declare Function SendMessageint Lib "user32" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
        Private Declare Function SendMessagetitle Lib "user32" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As StringBuilder) As Integer
        Private Declare Function GetClassName Lib "user32" Alias "GetClassNameA" (ByVal hwnd As IntPtr, ByVal lpClassName As String, ByVal nMaxCount As Integer) As Integer '获取窗口类
        Private Shared ErrMsg As String = ""

        Private Shared hWnds() As IntPtr
        Private Shared j As Integer

        Public Shared Function GetMessageType(ByVal MsgType As String) As Integer
            Dim CMDType As Integer = 0
            Try
                Select Case MsgType
                    '系统
                    Case "WM_SETTEXT"
                        CMDType = &HC
                    Case "WM_KEYDOWN"
                        CMDType = &H100
                    Case "WM_KEYUP"
                        CMDType = &H101
                    Case "WM_CHAR"
                        CMDType = &H102
                    Case "WM_NULL"
                        CMDType = &H0
                    Case "WM_CREATE"
                        CMDType = &H1
                    Case "WM_DESTROY"
                        CMDType = &H2
                    Case "WM_MOVE"
                        CMDType = &H3
                    Case "WM_SIZE"
                        CMDType = &H5
                    Case "WM_ACTIVATE"
                        CMDType = &H6
                    Case "WA_INACTIVE"
                        CMDType = 0
                    Case "WA_ACTIVE"
                        CMDType = 1
                    Case "WA_CLICKACTIVE"
                        CMDType = 2
                    Case "WM_SETFOCUS"
                        CMDType = &H7
                    Case "WM_KILLFOCUS"
                        CMDType = &H8
                    Case "WM_ENABLE"
                        CMDType = &HA
                    Case "WM_SETREDRAW"
                        CMDType = &HB
                    Case "WM_GETTEXT"
                        CMDType = &HD
                    Case "WM_GETTEXTLENGTH"
                        CMDType = &HE
                    Case "WM_PAINT"
                        CMDType = &HF
                    Case "WM_CLOSE"
                        CMDType = &H10
                    Case "WM_QUERYENDSESSION"
                        CMDType = &H11
                    Case "WM_QUERYOPEN"
                        CMDType = &H13
                    Case "WM_ENDSESSION"
                        CMDType = &H16
                    Case "WM_QUIT"
                        CMDType = &H12
                    Case "WM_ERASEBKGND"
                        CMDType = &H14
                    Case "WM_SYSCOLORCHANGE"
                        CMDType = &H15
                    Case "WM_SHOWWINDOW"
                        CMDType = &H18
                    Case "WM_WININICHANGE"
                        CMDType = &H1A
                    Case "WM_DEVMODECHANGE"
                        CMDType = &H1B
                    Case "WM_ACTIVATEAPP"
                        CMDType = &H1C
                    Case "WM_FONTCHANGE"
                        CMDType = &H1D
                    Case "WM_TIMECHANGE"
                        CMDType = &H1E
                    Case "WM_CANCELMODE"
                        CMDType = &H1F
                    Case "WM_SETCURSOR"
                        CMDType = &H20
                    Case "WM_MOUSEACTIVATE"
                        CMDType = &H21
                    Case "WM_CHILDACTIVATE"
                        CMDType = &H22
                    Case "WM_QUEUESYNC"
                        CMDType = &H23
                    Case "WM_GETMINMAXINFO"
                        CMDType = &H24
                    Case "WM_LBUTTONDOWN"
                        CMDType = &H21
                    Case "WM_LBUTTONUP"
                        CMDType = &H22
                    Case "WM_LBUTTONDBLCLK"
                        CMDType = &H23
                    Case "WM_RBUTTONDOWN"
                        CMDType = &H24
                    Case "WM_RBUTTONUP"
                        CMDType = &H25
                    Case "WM_RBUTTONDBLCLK"
                        CMDType = &H26
                    Case "WM_COPY"
                        CMDType = &H31
                    Case "WM_PASTE"
                        CMDType = &H32
                    Case "WM_CLEAR"
                        CMDType = &H33
                    Case "WM_UNDO"
                        CMDType = &H34
                        '按键
                    Case "VK_F1"
                        CMDType = &H70
                    Case "VK_A"
                        CMDType = &H41
                        '按钮
                    Case "BM_CLICK"
                        CMDType = &HF5
                        'combobox
                    Case "CB_SHOWDROPDOWN"
                        CMDType = &H14F
                    Case "CB_GETCOUNT"
                        CMDType = &H146
                    Case "CB_FINDSTRING"
                        CMDType = &H14C
                    Case "CB_ADDSTRING"
                        CMDType = &H143
                    Case "CB_GETCURSEL"
                        CMDType = &H147
                    Case "CB_SELECTSTRING"
                        CMDType = &H14D
                    Case "CB_SHOWDROPDOWN"
                        CMDType = &H14F
                    Case "CB_GETEDITSEL"
                        CMDType = &H140
                    Case "CB_GETEXTENDEDUI"
                        CMDType = &H156
                    Case "CB_SETCURSEL"
                        CMDType = &H14E
                    Case "CB_SETEDITSEL"
                        CMDType = &H142
                    Case "CB_INSERTSTRING"
                        CMDType = &H14A
                    Case "CB_DELETESTRING"
                        CMDType = &H144
                        'listbox
                    Case "LB_FINDSTRING"
                        CMDType = &H18F
                    Case "LB_SETSEL"
                        CMDType = &H185 '用于单选ListBox 
                    Case "LB_SETTOPINDEX"
                        CMDType = &H197
                    Case "LB_SETCURSEL"
                        CMDType = &H186 '用于多选ListBox
                        '默认
                    Case Else
                        CMDType = MsgType
                End Select
            Catch ex As Exception
                CMDType = -1
                ErrMsg = ex.Message
            End Try
            Return CMDType
        End Function

        Public Shared Function GetPID(ByVal ProcessesName As String) As Integer()

            Try
                Dim Pid() As Integer
                Dim mProcessList As Process() = Process.GetProcessesByName(ProcessesName)
                ReDim Pid(UBound(mProcessList) + 1)
                Dim i As Integer = 0
                For Each tmpProcess As Process In mProcessList
                    Pid(i) = tmpProcess.Id
                    i += 1
                Next
                i = Nothing
                mProcessList = Nothing
                Return Pid
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function

        Public Shared Function ActiveWindow(ByVal ProcessesID As Integer) As Boolean
            Try
                AppActivate(ProcessesID)
                Return True
            Catch ex As Exception
                ErrMsg = ex.Message
                Return False
            End Try
        End Function

        Public Shared Function GethWndByPID(ByVal ProcessesID As Integer) As IntPtr
            Try
                Dim mProcess As Process = Process.GetProcessById(ProcessesID)
                GethWndByPID = mProcess.MainWindowHandle
            Catch ex As Exception
                GethWndByPID = 0
                ErrMsg = ex.Message
            End Try
            Return GethWndByPID
        End Function

        Public Shared Function GethWndByPName(ByVal ProcessesName As String) As IntPtr()
            Try
                Dim Pid() As IntPtr
                Dim mProcessList As Process() = Process.GetProcessesByName(ProcessesName)
                ReDim Pid(UBound(mProcessList) + 1)
                Dim i As Integer = 0
                For Each tmpProcess As Process In mProcessList
                    Pid(i) = tmpProcess.MainWindowHandle
                    i += 1
                Next
                i = Nothing
                mProcessList = Nothing
                Return Pid
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function

        Public Shared Function GethWndClassName(ByVal hWnd As IntPtr) As String
            Try
                If hWnd <> 0 Then
                    Dim ClassName As New String(ChrW(0), 255)
                    GetClassName(hWnd, ClassName, 255)
                    GethWndClassName = ClassName
                Else
                    GethWndClassName = ""
                End If
            Catch ex As Exception
                GethWndClassName = ""
                ErrMsg = ex.Message
            End Try
            Return GethWndClassName
        End Function

        Public Shared Function GethWndTitle(ByVal hWnd As IntPtr) As String
            Try
                If hWnd <> 0 Then
                    Const WM_GETTEXT As Int32 = &HD
                    Const WM_GETTEXTLENGTH As Int32 = &HE
                    Dim title As New StringBuilder()
                    Dim size As Int32 = SendMessage(hWnd, WM_GETTEXTLENGTH, 0, 0)
                    If size > 0 Then
                        title = New StringBuilder(size * 2 + 1)
                        SendMessagetitle(hWnd, WM_GETTEXT, title.Capacity, title)
                    End If
                    GethWndTitle = title.ToString
                    GethWndTitle = GethWndTitle.Trim
                    title = Nothing
                    size = Nothing
                Else
                    GethWndTitle = ""
                End If
            Catch ex As Exception
                GethWndTitle = ""
                ErrMsg = ex.Message
            End Try
            Return GethWndTitle
        End Function

        Public Shared Function GetHwndCount(ByVal hWnd As IntPtr) As Integer
            Try
                j = 0
                Dim em As New EnumCallBackDelegate(AddressOf EnumChildProcConut)
                EnumChildWindows(hWnd, em, 0)
                Return j
            Catch ex As Exception
                ErrMsg = ex.Message
                Return -2
            End Try
        End Function

        Public Shared Function GetHwnd(ByVal hWnd As IntPtr) As IntPtr()
            Try
                j = 0
                Dim Pid() As IntPtr
                Dim em As New EnumCallBackDelegate(AddressOf EnumChildProcConut)
                EnumChildWindows(hWnd, em, 0)
                ReDim hWnds(j - 1)
                j = 0
                Dim Sm As New EnumCallBackDelegate(AddressOf EnumChildProcs)
                EnumChildWindows(hWnd, Sm, 0)
                ReDim Pid(hWnds.Length)
                Pid = hWnds
                Return Pid
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function
        Private Shared Function EnumChildProcConut(ByVal hwnded As Integer, ByVal lParam As Integer) As Integer
            EnumChildProcConut = 0
            j += 1
            EnumChildProcConut = 1
        End Function
        Private Shared Function EnumChildProcs(ByVal hwnded As Integer, ByVal lParam As Integer) As Integer
            EnumChildProcs = 0
            hWnds(j) = hwnded
            j += 1
            EnumChildProcs = 1
        End Function

        Public Shared Function GetMainhWndBySearch(ByVal SearchString As String) As IntPtr
            Try
                If SearchString.Trim = "" Then
                    SearchString = vbNullString
                End If
                Return FindWindow(Nothing, SearchString)
            Catch ex As Exception
                ErrMsg = ex.Message
                Return IntPtr.Zero
            End Try
        End Function

        Public Shared Function GetMainhWndBySearch(ByVal ClassName As String, ByVal SearchString As String) As IntPtr
            Try
                If ClassName.Trim = "" Then
                    ClassName = vbNullString
                End If
                If SearchString.Trim = "" Then
                    SearchString = vbNullString
                End If
                Return FindWindow(ClassName, SearchString)
            Catch ex As Exception
                ErrMsg = ex.Message
                Return IntPtr.Zero
            End Try
        End Function

        Public Shared Function GetSubhWndBySearch(ByVal hWnd As IntPtr, ByVal ClassName As String) As IntPtr
            Try
                If ClassName.Trim = "" Then
                    ClassName = vbNullString
                End If
                Return FindWindowEx(hWnd, IntPtr.Zero, ClassName, vbNullString)
            Catch ex As Exception
                ErrMsg = ex.Message
                Return IntPtr.Zero
            End Try
        End Function

        Public Shared Function GetSubhWndBySearch(ByVal hWnd As IntPtr, ByVal ClassName As String, ByVal SearchString As String) As IntPtr
            Try
                If ClassName.Trim = "" Then
                    ClassName = vbNullString
                End If
                If SearchString.Trim = "" Then
                    SearchString = vbNullString
                End If
                Return FindWindowEx(hWnd, IntPtr.Zero, ClassName, SearchString)
            Catch ex As Exception
                ErrMsg = ex.Message
                Return IntPtr.Zero
            End Try
        End Function

        Public Shared Function SendMSG(ByVal hWnd As IntPtr, ByVal SendType As Int16, ByVal MsgType As Integer, ByVal Ref1 As String, ByVal Ref2 As String) As Integer
            Try
                If Ref1.Trim = "" Then
                    Ref1 = vbNullString
                End If
                If Ref2.Trim = "" Then
                    Ref2 = vbNullString
                End If
                Select Case SendType
                    Case 0 'false
                        SendMSG = SendMessage(hWnd, MsgType, False, 0)
                    Case 1 'true
                        SendMSG = SendMessage(hWnd, MsgType, True, 0)
                    Case 2 'Find String
                        SendMSG = SendMessage(hWnd, MsgType, -1, Ref1)
                    Case 3 'For Index
                        SendMSG = SendMessageint(hWnd, MsgType, Ref1, 0)
                    Case 4 'EVT
                        SendMSG = SendMessageint(hWnd, MsgType, 0, 0)
                    Case 5 'comm string
                        SendMSG = SendMessage(hWnd, MsgType, Ref1, Ref2)
                    Case 6 'comm INT
                        SendMSG = SendMessageint(hWnd, MsgType, Ref1, Ref2)
                    Case 7 'Normal String
                        SendMSG = SendMessage(hWnd, MsgType, IntPtr.Zero, Ref1)
                    Case 8 'Normal String
                        SendMSG = SendMessage(hWnd, MsgType, Ref1, IntPtr.Zero)
                    Case 9 'Normal int
                        SendMSG = SendMessageint(hWnd, MsgType, IntPtr.Zero, Ref1)
                    Case 10 'Normal int
                        SendMSG = SendMessageint(hWnd, MsgType, Ref1, IntPtr.Zero)
                    Case Else
                        SendMSG = -1
                End Select
            Catch ex As Exception
                SendMSG = -1
                ErrMsg = ex.Message
            End Try
            Return SendMSG
        End Function

        Public Shared Function PostMSG(ByVal hWnd As IntPtr, ByVal SendType As Int16, ByVal MsgType As Integer, ByVal Ref1 As String, ByVal Ref2 As String) As Integer
            Try
                If Ref1.Trim = "" Then
                    Ref1 = vbNullString
                End If
                If Ref2.Trim = "" Then
                    Ref2 = vbNullString
                End If
                Select Case SendType
                    Case 0 'false
                        PostMSG = PostMessage(hWnd, MsgType, False, 0)
                    Case 1 'true
                        PostMSG = PostMessage(hWnd, MsgType, True, 0)
                    Case 2 'Find String
                        PostMSG = PostMessage(hWnd, MsgType, -1, Ref1)
                    Case 3 'For Index
                        PostMSG = PostMessageInt(hWnd, MsgType, Ref1, 0)
                    Case 4 'EVT
                        PostMSG = PostMessageInt(hWnd, MsgType, 0, 0)
                    Case 5 'comm string
                        PostMSG = PostMessage(hWnd, MsgType, Ref1, Ref2)
                    Case 6 'comm INT
                        PostMSG = PostMessageInt(hWnd, MsgType, Ref1, Ref2)
                    Case 7 'Normal String
                        PostMSG = PostMessage(hWnd, MsgType, IntPtr.Zero, Ref1)
                    Case 8 'Normal String
                        PostMSG = PostMessage(hWnd, MsgType, Ref1, IntPtr.Zero)
                    Case 9 'Normal int
                        PostMSG = PostMessageInt(hWnd, MsgType, IntPtr.Zero, Ref1)
                    Case 10 'Normal int
                        PostMSG = PostMessageInt(hWnd, MsgType, Ref1, IntPtr.Zero)
                    Case Else
                        PostMSG = -1
                End Select
            Catch ex As Exception
                PostMSG = -1
                ErrMsg = ex.Message
            End Try
            Return PostMSG
        End Function

        Public Shared Function SyncSendKey(ByVal ProcessesID As Integer, ByVal KeyVal As String) As Boolean
            Try
                AppActivate(ProcessesID)
                Dim str As String = KeyVal
                If str.Trim = "" Then
                    str = vbNullString
                End If
                SendKeys.Send(str)
                SyncSendKey = True
            Catch ex As Exception
                SyncSendKey = False
                ErrMsg = ex.Message
            End Try
            Return SyncSendKey
        End Function

        Public Shared Function ASyncSendKey(ByVal ProcessesID As Integer, ByVal KeyVal As String) As Boolean
            Try
                AppActivate(ProcessesID)
                Dim str As String = KeyVal
                If str.Trim = "" Then
                    str = vbNullString
                End If
                SendKeys.SendWait(str)
                ASyncSendKey = True
            Catch ex As Exception
                ASyncSendKey = False
                ErrMsg = ex.Message
            End Try
            Return ASyncSendKey
        End Function

        Public Shared Function ResponseTEXT(ByVal hWnd As IntPtr) As String
            Try
                If hWnd <> 0 Then
                    Const WM_GETTEXT As Int32 = &HD
                    Const WM_GETTEXTLENGTH As Int32 = &HE
                    Dim title As New StringBuilder()
                    Dim size As Int32 = SendMessage(hWnd, WM_GETTEXTLENGTH, 0, 0)
                    If size > 0 Then
                        title = New StringBuilder(size * 2 + 1)
                        SendMessagetitle(hWnd, WM_GETTEXT, title.Capacity, title)
                    End If
                    ResponseTEXT = title.ToString
                    ResponseTEXT = ResponseTEXT.Trim
                    title = Nothing
                    size = Nothing
                Else
                    ResponseTEXT = ""
                End If
            Catch ex As Exception
                ResponseTEXT = ""
                ErrMsg = ex.Message
            End Try
            Return ResponseTEXT
        End Function

        Public Shared Function SendTEXT(ByVal hWnd As IntPtr, ByVal SendString As String) As Integer
            Try
                If hWnd <> 0 Then
                    Const WM_SETTEXT As Int32 = &HC
                    SendTEXT = SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, SendString)
                Else
                    SendTEXT = 0
                End If
            Catch ex As Exception
                SendTEXT = -1
                ErrMsg = ex.Message
            End Try
            Return SendTEXT
        End Function

        Public Shared Function ASendTEXT(ByVal hWnd As IntPtr, ByVal SendString As String) As Integer
            Try
                If hWnd <> 0 Then
                    Const WM_SETTEXT As Int32 = &HC
                    ASendTEXT = PostMessage(hWnd, WM_SETTEXT, IntPtr.Zero, SendString)
                Else
                    ASendTEXT = 0
                End If
            Catch ex As Exception
                ASendTEXT = -1
                ErrMsg = ex.Message = ex.Message
            End Try
            Return ASendTEXT
        End Function

        '高阶应用
        '同步模式
        Public Shared Function MouseLClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_LBUTTONDOWN As Int32 = &H21
                If SendMessageint(hWnd, WM_LBUTTONDOWN, 0, 0) = 1 Then
                    MouseLClick = True
                Else
                    MouseLClick = False
                End If
                MouseLClick = True
            Catch ex As Exception
                MouseLClick = False
                ErrMsg = ex.Message
            End Try
            Return MouseLClick
        End Function

        Public Shared Function MouseLDBClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_LBUTTONDBLCLK As Int32 = &H21
                If SendMessageint(hWnd, WM_LBUTTONDBLCLK, 0, 0) = 1 Then
                    MouseLDBClick = True
                Else
                    MouseLDBClick = False
                End If
            Catch ex As Exception
                MouseLDBClick = False
                ErrMsg = ex.Message
            End Try
            Return MouseLDBClick
        End Function

        Public Shared Function MouseRClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_RBUTTONDOWN As Int32 = &H24
                If SendMessageint(hWnd, WM_RBUTTONDOWN, 0, 0) = 1 Then
                    MouseRClick = True
                Else
                    MouseRClick = False
                End If
            Catch ex As Exception
                MouseRClick = False
                ErrMsg = ex.Message
            End Try
            Return MouseRClick
        End Function

        Public Shared Function MouseRDBClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_RBUTTONDBLCLK As Int32 = &H26
                If SendMessageint(hWnd, WM_RBUTTONDBLCLK, 0, 0) = 1 Then
                    MouseRDBClick = True
                Else
                    MouseRDBClick = False
                End If
            Catch ex As Exception
                MouseRDBClick = False
                ErrMsg = ex.Message
            End Try
            Return MouseRDBClick
        End Function

        Public Shared Function ButtonClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const BM_CLICK As Int32 = &HF5
                If SendMessageint(hWnd, BM_CLICK, 0, 0) Then
                    ButtonClick = True
                Else
                    ButtonClick = False
                End If
            Catch ex As Exception
                ButtonClick = False
                ErrMsg = ex.Message
            End Try
            Return ButtonClick
        End Function

        Public Shared Function ComboBoxONOFF(ByVal hWnd As IntPtr, ByVal ONOFF As Boolean) As Boolean
            Try
                Const CB_SHOWDROPDOWN As Int32 = &H14F
                If SendMessageint(hWnd, CB_SHOWDROPDOWN, ONOFF, 0) = 1 Then
                    ComboBoxONOFF = True
                Else
                    ComboBoxONOFF = False
                End If
            Catch ex As Exception
                ComboBoxONOFF = False
                ErrMsg = ex.Message
            End Try
            Return ComboBoxONOFF
        End Function

        Public Shared Function ComboBoxSEL(ByVal hWnd As IntPtr, ByVal SEL As Integer) As Boolean
            Try
                Const CB_SETCURSEL As Int32 = &H14E
                If SendMessageint(hWnd, CB_SETCURSEL, SEL, 0) = 1 Then
                    ComboBoxSEL = True
                Else
                    ComboBoxSEL = False
                End If
            Catch ex As Exception
                ComboBoxSEL = False
                ErrMsg = ex.Message
            End Try
            Return ComboBoxSEL
        End Function

        Public Shared Function ListBoxSEL(ByVal hWnd As IntPtr, ByVal SEL As Integer) As Boolean
            Try
                Const LB_SETSEL As Int32 = &H185
                If SendMessageint(hWnd, LB_SETSEL, SEL, 0) = 1 Then
                    ListBoxSEL = True
                Else
                    ListBoxSEL = False
                End If
            Catch ex As Exception
                ListBoxSEL = False
                ErrMsg = ex.Message
            End Try
            Return ListBoxSEL
        End Function

        Public Shared Function ListBoxMSEL(ByVal hWnd As IntPtr, ByVal SEL As Integer) As Boolean
            Try
                Const LB_SETCURSEL As Int32 = &H186
                If SendMessageint(hWnd, LB_SETCURSEL, SEL, 0) = 1 Then
                    ListBoxMSEL = True
                Else
                    ListBoxMSEL = False
                End If
            Catch ex As Exception
                ListBoxMSEL = False
                ErrMsg = ex.Message
            End Try
            Return ListBoxMSEL
        End Function

        '异步模式
        Public Shared Function AMouseLClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_LBUTTONDOWN As Int32 = &H21
                PostMessageInt(hWnd, WM_LBUTTONDOWN, 0, 0)
                AMouseLClick = True
            Catch ex As Exception
                AMouseLClick = False
                ErrMsg = ex.Message
            End Try
            Return AMouseLClick
        End Function

        Public Shared Function AMouseLDBClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_LBUTTONDBLCLK As Int32 = &H21
                PostMessageInt(hWnd, WM_LBUTTONDBLCLK, 0, 0)
                AMouseLDBClick = True
            Catch ex As Exception
                AMouseLDBClick = False
                ErrMsg = ex.Message
            End Try
            Return AMouseLDBClick
        End Function

        Public Shared Function AMouseRClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_RBUTTONDOWN As Int32 = &H24
                PostMessageInt(hWnd, WM_RBUTTONDOWN, 0, 0)
                AMouseRClick = True
            Catch ex As Exception
                AMouseRClick = False
                ErrMsg = ex.Message
            End Try
            Return AMouseRClick
        End Function

        Public Shared Function AMouseRDBClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const WM_RBUTTONDBLCLK As Int32 = &H26
                PostMessageInt(hWnd, WM_RBUTTONDBLCLK, 0, 0)
                AMouseRDBClick = True
            Catch ex As Exception
                AMouseRDBClick = False
                ErrMsg = ex.Message
            End Try
            Return AMouseRDBClick
        End Function

        Public Shared Function AButtonClick(ByVal hWnd As IntPtr) As Boolean
            Try
                Const BM_CLICK As Int32 = &HF5
                PostMessageInt(hWnd, BM_CLICK, 0, 0)
                AButtonClick = True
            Catch ex As Exception
                AButtonClick = False
                ErrMsg = ex.Message
            End Try
            Return AButtonClick
        End Function

        Public Shared Function AComboBoxONOFF(ByVal hWnd As IntPtr, ByVal ONOFF As Boolean) As Boolean
            Try
                Const CB_SHOWDROPDOWN As Int32 = &H14F
                PostMessageInt(hWnd, CB_SHOWDROPDOWN, ONOFF, 0)
                AComboBoxONOFF = True
            Catch ex As Exception
                AComboBoxONOFF = False
                ErrMsg = ex.Message
            End Try
            Return AComboBoxONOFF
        End Function

        Public Shared Function AComboBoxSEL(ByVal hWnd As IntPtr, ByVal SEL As Integer) As Boolean
            Try
                Const CB_SETCURSEL As Int32 = &H14E
                PostMessageInt(hWnd, CB_SETCURSEL, SEL, 0)
                AComboBoxSEL = True
            Catch ex As Exception
                AComboBoxSEL = False
                ErrMsg = ex.Message
            End Try
            Return AComboBoxSEL
        End Function

        Public Shared Function AListBoxSEL(ByVal hWnd As IntPtr, ByVal SEL As Integer) As Boolean
            Try
                Const LB_SETSEL As Int32 = &H185
                PostMessageInt(hWnd, LB_SETSEL, SEL, 0)
                AListBoxSEL = True
            Catch ex As Exception
                AListBoxSEL = False
                ErrMsg = ex.Message
            End Try
            Return AListBoxSEL
        End Function

        Public Shared Function AListBoxMSEL(ByVal hWnd As IntPtr, ByVal SEL As Integer) As Boolean
            Try
                Const LB_SETCURSEL As Int32 = &H186
                PostMessageInt(hWnd, LB_SETCURSEL, SEL, 0)
                AListBoxMSEL = True
            Catch ex As Exception
                AListBoxMSEL = False
                ErrMsg = ex.Message
            End Try
            Return AListBoxMSEL
        End Function

    End Class