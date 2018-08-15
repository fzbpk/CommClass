Imports System.Management
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Public Class Process32
    Shared ErrMsg As String = ""

    Public Structure Process_Info
        ''' <summary>
        ''' 进程名称
        ''' </summary>
        ''' <remarks></remarks>
        Public ProcessName As String
        ''' <summary>
        ''' 进程ID
        ''' </summary>
        ''' <remarks></remarks>
        Public ProcessID As Integer
        ''' <summary>
        ''' 进程句柄
        ''' </summary>
        ''' <remarks></remarks>
        Public Processhwnd As IntPtr
        ''' <summary>
        ''' 进程会话ID
        ''' </summary>
        ''' <remarks></remarks>
        Public ProcessSessionID As Integer
        ''' <summary>
        ''' 是否有响应，FALSE为无响应
        ''' </summary>
        ''' <remarks></remarks>
        Public Responding As Boolean
        ''' <summary>
        ''' 启动时间
        ''' </summary>
        ''' <remarks></remarks>
        Public StartTime As Date
        ''' <summary>
        ''' CPU使用率，%
        ''' </summary>
        ''' <remarks></remarks>
        Public CPU As Byte
        ''' <summary>
        ''' 内存使用率，MB
        ''' </summary>
        ''' <remarks></remarks>
        Public Memory As Long
        ''' <summary>
        ''' 用户
        ''' </summary>
        ''' <remarks></remarks>
        Public User As String
        ''' <summary>
        ''' 路径
        ''' </summary>
        ''' <remarks></remarks>
        Public FilePath As String
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
    ''' 通过进程名称关闭进程
    ''' </summary>
    ''' <param name="comObjProcessName">进程名称</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function KillProcessByName(ByVal comObjProcessName As String) As Boolean
        KillProcessByName = False
        Try
            If comObjProcessName.Contains(".") Then
                Dim str() As String = comObjProcessName.Split(".")
                comObjProcessName = comObjProcessName.Replace("." + str(UBound(str)), "")
            End If
            Dim mProcessList As Process()
            mProcessList = Process.GetProcessesByName(comObjProcessName)
            For Each tmpProcess As Process In mProcessList
                tmpProcess.Kill()
                KillProcessByName = True
            Next
        Catch ex As Exception
            KillProcessByName = False
            ErrMsg = ex.Message
        End Try
        Return KillProcessByName
    End Function

    ''' <summary>
    ''' 通过进程路径关闭进程
    ''' </summary>
    ''' <param name="Address">进程路径</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function KillProcessByPath(ByVal Address As String) As Boolean
        If Address Is Nothing Then
            ErrMsg = "路径有误"
            Return False
        ElseIf Address.Trim = "" Then
            ErrMsg = "路径有误"
            Return False
        Else
            Try
                If IO.File.Exists(Address) Then
                    Dim WorkSrc, ExeName As String
                    If Address.Contains("\") Then
                        WorkSrc = Strings.Left(Address, Address.LastIndexOf("\"))
                        If WorkSrc.EndsWith("\") Then
                            WorkSrc = Strings.Left(WorkSrc, WorkSrc.Length - 1)
                        End If
                        ExeName = Strings.Right(Address, Address.Length - Address.LastIndexOf("\"))
                        If ExeName.StartsWith("\") Then
                            ExeName = Strings.Right(ExeName, ExeName.Length - 1)
                        End If
                    Else
                        ExeName = Address
                        WorkSrc = System.Environment.CurrentDirectory
                    End If
                    If ExeName.Trim <> "" Then
                        Dim FExeName As String() = ExeName.Split(".")
                        Dim mProcessList As Process()
                        mProcessList = Process.GetProcessesByName(FExeName(0))
                        For Each tmpProcess As Process In mProcessList
                            tmpProcess.Kill()
                            Return True
                        Next
                        Return False
                    Else
                        ErrMsg = "路径有误"
                        Return False
                    End If
                Else
                    Return False
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return False
            End Try
        End If
    End Function

    ''' <summary>
    ''' 通过进程名查询进程是否存在
    ''' </summary>
    ''' <param name="comObjProcessName">进程名</param>
    ''' <returns>TRUE存在，FALSE不存在</returns>
    ''' <remarks></remarks>
    Public Shared Function FindProcessByName(ByVal comObjProcessName As String) As Boolean
        FindProcessByName = False
        Try
            If comObjProcessName.Contains(".") Then
                Dim str() As String = comObjProcessName.Split(".")
                comObjProcessName = comObjProcessName.Replace("." + str(UBound(str)), "")
            End If
            Dim result As Boolean = False
            Dim mProcessList As Process()
            mProcessList = Process.GetProcessesByName(comObjProcessName)
            For Each tmpProcess As Process In mProcessList
                FindProcessByName = True
                Exit For
            Next
        Catch ex As Exception
            FindProcessByName = False
            ErrMsg = ex.Message
        End Try
        Return FindProcessByName
    End Function

    ''' <summary>
    ''' 通过进程路径查询进程是否存在
    ''' </summary>
    ''' <param name="Address">进程路径</param>
    ''' <returns>TRUE存在，FALSE不存在</returns>
    ''' <remarks></remarks>
    Public Shared Function FindProcessByPath(ByVal Address As String) As Boolean
        If Address Is Nothing Then
            ErrMsg = "路径有误"
            Return False
        ElseIf Address.Trim = "" Then
            ErrMsg = "路径有误"
            Return False
        Else
            Try
                If IO.File.Exists(Address) Then
                    Dim WorkSrc, ExeName As String
                    If Address.Contains("\") Then
                        WorkSrc = Strings.Left(Address, Address.LastIndexOf("\"))
                        If WorkSrc.EndsWith("\") Then
                            WorkSrc = Strings.Left(WorkSrc, WorkSrc.Length - 1)
                        End If
                        ExeName = Strings.Right(Address, Address.Length - Address.LastIndexOf("\"))
                        If ExeName.StartsWith("\") Then
                            ExeName = Strings.Right(ExeName, ExeName.Length - 1)
                        End If
                    Else
                        ExeName = Address
                        WorkSrc = System.Environment.CurrentDirectory
                    End If
                    If ExeName.Trim <> "" Then
                        FindProcessByPath = False
                        Dim FExeName As String() = ExeName.Split(".")
                        Dim mProcessList As Process()
                        mProcessList = Process.GetProcessesByName(FExeName(0))
                        For Each tmpProcess As Process In mProcessList
                            FindProcessByPath = True
                            Exit For
                        Next
                    Else
                        FindProcessByPath = False
                    End If
                Else
                    ErrMsg = "路径有误"
                    FindProcessByPath = False
                End If
            Catch ex As Exception
                FindProcessByPath = False
                ErrMsg = ex.Message
            End Try
            Return FindProcessByPath
        End If
    End Function

    ''' <summary>
    ''' 通过进程名查询进程句柄
    ''' </summary>
    ''' <param name="comObjProcessName">进程名</param>
    ''' <returns>进程句柄</returns>
    ''' <remarks></remarks>
    Public Shared Function FindProcessHwdByName(ByVal comObjProcessName As String) As IntPtr
        Try
            If comObjProcessName.Contains(".") Then
                Dim str() As String = comObjProcessName.Split(".")
                comObjProcessName = comObjProcessName.Replace("." + str(UBound(str)), "")
            End If
            Dim result As Boolean = False
            Dim mProcessList As Process()
            mProcessList = Process.GetProcessesByName(comObjProcessName)
            For Each tmpProcess As Process In mProcessList
                Return tmpProcess.Handle
                Exit For
            Next
            Return IntPtr.Zero
        Catch ex As Exception
            ErrMsg = ex.Message
            Return IntPtr.Zero
        End Try
    End Function

    ''' <summary>
    ''' 通过进程路径查询进程句柄
    ''' </summary>
    ''' <param name="Address">进程路径</param>
    ''' <returns>进程句柄</returns>
    ''' <remarks></remarks>
    Public Shared Function FindProcessHwdByPath(ByVal Address As String) As IntPtr
        If Address Is Nothing Then
            ErrMsg = "路径有误"
            Return IntPtr.Zero
        ElseIf Address.Trim = "" Then
            ErrMsg = "路径有误"
            Return IntPtr.Zero
        Else
            Try
                If IO.File.Exists(Address) Then
                    Dim WorkSrc, ExeName As String
                    If Address.Contains("\") Then
                        WorkSrc = Strings.Left(Address, Address.LastIndexOf("\"))
                        If WorkSrc.EndsWith("\") Then
                            WorkSrc = Strings.Left(WorkSrc, WorkSrc.Length - 1)
                        End If
                        ExeName = Strings.Right(Address, Address.Length - Address.LastIndexOf("\"))
                        If ExeName.StartsWith("\") Then
                            ExeName = Strings.Right(ExeName, ExeName.Length - 1)
                        End If
                    Else
                        ExeName = Address
                        WorkSrc = System.Environment.CurrentDirectory
                    End If
                    If ExeName.Trim <> "" Then
                        Dim FExeName As String() = ExeName.Split(".")
                        Dim mProcessList As Process()
                        mProcessList = Process.GetProcessesByName(FExeName(0))
                        For Each tmpProcess As Process In mProcessList
                            Return tmpProcess.Handle
                            Exit For
                        Next
                        Return IntPtr.Zero
                    Else
                        Return IntPtr.Zero
                    End If
                Else
                    ErrMsg = "路径有误"
                    Return IntPtr.Zero
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return IntPtr.Zero
            End Try
        End If
    End Function

    ''' <summary>
    ''' 通过进程名查询进程ID
    ''' </summary>
    ''' <param name="comObjProcessName">进程名</param>
    ''' <returns>进程ID</returns>
    ''' <remarks></remarks>
    Public Shared Function FindProcessIDByName(ByVal comObjProcessName As String) As Integer
        Try
            If comObjProcessName.Contains(".") Then
                Dim str() As String = comObjProcessName.Split(".")
                comObjProcessName = comObjProcessName.Replace("." + str(UBound(str)), "")
            End If
            Dim result As Boolean = False
            Dim mProcessList As Process()
            mProcessList = Process.GetProcessesByName(comObjProcessName)
            For Each tmpProcess As Process In mProcessList
                Return tmpProcess.Id
                Exit For
            Next
            Return -1
        Catch ex As Exception
            ErrMsg = ex.Message
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' 通过进程路径查询进程ID
    ''' </summary>
    ''' <param name="Address">进程路径</param>
    ''' <returns>进程ID</returns>
    ''' <remarks></remarks>
    Public Shared Function FindProcessIDByPath(ByVal Address As String) As Integer
        If Address Is Nothing Then
            ErrMsg = "路径有误"
            Return -1
        ElseIf Address.Trim = "" Then
            ErrMsg = "路径有误"
            Return -1
        Else
            Try
                If IO.File.Exists(Address) Then
                    Dim WorkSrc, ExeName As String
                    If Address.Contains("\") Then
                        WorkSrc = Strings.Left(Address, Address.LastIndexOf("\"))
                        If WorkSrc.EndsWith("\") Then
                            WorkSrc = Strings.Left(WorkSrc, WorkSrc.Length - 1)
                        End If
                        ExeName = Strings.Right(Address, Address.Length - Address.LastIndexOf("\"))
                        If ExeName.StartsWith("\") Then
                            ExeName = Strings.Right(ExeName, ExeName.Length - 1)
                        End If
                    Else
                        ExeName = Address
                        WorkSrc = System.Environment.CurrentDirectory
                    End If
                    If ExeName.Trim <> "" Then
                        Dim FExeName As String() = ExeName.Split(".")
                        Dim mProcessList As Process()
                        mProcessList = Process.GetProcessesByName(FExeName(0))
                        For Each tmpProcess As Process In mProcessList
                            Return tmpProcess.Id
                            Exit For
                        Next
                        Return -1
                    Else
                        Return -1
                    End If
                Else
                    ErrMsg = "路径有误"
                    Return -1
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return -1
            End Try
        End If
    End Function

    ''' <summary>
    ''' 调用可执行程序
    ''' </summary>
    ''' <param name="WorkSrc">程序目录</param>
    ''' <param name="ExeName">程序名称</param>
    ''' <param name="Repeat">是否可用重复调用</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function CallEXE(ByVal WorkSrc As String, ByVal ExeName As String, Optional ByVal Repeat As Boolean = True) As Boolean
        CallEXE = False
        Try
            If WorkSrc Is Nothing Then
                WorkSrc = System.Environment.CurrentDirectory
            ElseIf WorkSrc.Trim = "" Then
                WorkSrc = System.Environment.CurrentDirectory
            Else
                If WorkSrc.EndsWith("\") Then
                    WorkSrc = Left(WorkSrc, WorkSrc.Length - 1)
                End If
            End If
            If ExeName Is Nothing Then
                CallEXE = False
            ElseIf ExeName.Trim <> "" Then
                If IO.File.Exists(WorkSrc & "\" & ExeName) Then
                    If Repeat Then
                        Dim start As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo
                        start.FileName = WorkSrc & "\" & ExeName
                        start.WorkingDirectory = WorkSrc
                        System.Diagnostics.Process.Start(start)
                        CallEXE = True
                    Else
                        Dim FExeName As String() = ExeName.Split(".")
                        If FindProcessByName(FExeName(0)) Then
                            CallEXE = False
                        Else
                            Dim start As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo
                            start.FileName = WorkSrc & "\" & ExeName
                            start.WorkingDirectory = WorkSrc
                            System.Diagnostics.Process.Start(start)
                            CallEXE = True
                        End If
                    End If
                Else
                    CallEXE = False
                End If
            Else
                CallEXE = False
            End If
        Catch ex As Exception
            CallEXE = False
            ErrMsg = ex.Message
        End Try
        Return CallEXE
    End Function

    ''' <summary>
    ''' 调用程序
    ''' </summary>
    ''' <param name="Address">程序路径</param>
    ''' <param name="Repeat">重复</param>
    ''' <param name="UserName">运行账号</param> 
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function CallEXE(ByVal Address As String, Optional ByVal Repeat As Boolean = True, Optional ByVal UserName As String = "") As Boolean
        If Address Is Nothing Then
            ErrMsg = "文件路径为空"
            Return False
        ElseIf Address.Trim = "" Then
            ErrMsg = "文件路径为空"
            Return False
        Else
            Try
                If UserName Is Nothing Then
                    UserName = ""
                End If
                If IO.File.Exists(Address) Then
                    Dim WorkSrc, ExeName As String
                    If Address.Contains("\") Then
                        WorkSrc = Strings.Left(Address, Address.LastIndexOf("\"))
                        If WorkSrc.EndsWith("\") Then
                            WorkSrc = Strings.Left(WorkSrc, WorkSrc.Length - 1)
                        End If
                        ExeName = Strings.Right(Address, Address.Length - Address.LastIndexOf("\"))
                        If ExeName.StartsWith("\") Then
                            ExeName = Strings.Right(ExeName, ExeName.Length - 1)
                        End If
                    Else
                        ExeName = Address
                        WorkSrc = System.Environment.CurrentDirectory
                    End If
                    If ExeName.Trim <> "" Then
                        If Repeat Then
                            Dim start As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo
                            start.FileName = WorkSrc & "\" & ExeName
                            start.WorkingDirectory = WorkSrc
                            If UserName.Trim <> "" Then
                                start.Verb = "runas"
                                start.UserName = UserName
                            End If
                            System.Diagnostics.Process.Start(start)
                            CallEXE = True
                            ErrMsg = ""
                        Else
                            Dim FExeName As String() = ExeName.Split(".")
                            If FindProcessByName(FExeName(0)) Then
                                CallEXE = False
                                ErrMsg = "程序已运行"
                            Else
                                Dim start As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo
                                start.FileName = WorkSrc & "\" & ExeName
                                start.WorkingDirectory = WorkSrc
                                If UserName.Trim <> "" Then
                                    start.Verb = "runas"
                                    start.UserName = UserName
                                End If
                                System.Diagnostics.Process.Start(start)
                                CallEXE = True
                                ErrMsg = ""
                            End If
                        End If
                    Else
                        ErrMsg = "执行文件不存在"
                        CallEXE = False
                    End If
                Else
                    ErrMsg = "执行文件路径不存在"
                    CallEXE = False
                End If
            Catch ex As Exception
                CallEXE = False
                ErrMsg = ex.Message
            End Try
            Return CallEXE
        End If
    End Function

    ''' <summary>
    ''' 命令行
    ''' </summary>
    ''' <param name="cmd">命令行</param>
    ''' <param name="style">窗口展示</param>
    ''' <returns>返回执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function CommandLine(ByVal cmd As String, Optional ByVal style As System.Diagnostics.ProcessWindowStyle = ProcessWindowStyle.Hidden) As String
        Try
            Dim pro As System.Diagnostics.Process = New System.Diagnostics.Process()
            pro.StartInfo.UseShellExecute = False
            '分离文件名和路径  
            '定位路径  
            Dim IndexA As Integer = cmd.LastIndexOf("\")
            If IndexA >= 0 Then
                '设定工作目录  
                pro.StartInfo.WorkingDirectory = cmd.Substring(0, IndexA)
            End If
            '定位文件名，判断是否带参数  
            IndexA += 1
            Dim IndexB As Integer = cmd.IndexOf(" ", IndexA)
            If IndexB >= 0 Then
                pro.StartInfo.FileName = cmd.Substring(IndexA, IndexB - IndexA)
                pro.StartInfo.Arguments = cmd.Substring(IndexB + 1)
            Else
                '不带参数  
                pro.StartInfo.FileName = cmd.Substring(IndexA)
            End If
            pro.StartInfo.RedirectStandardOutput = True
            pro.StartInfo.RedirectStandardError = True
            '设置cmd窗口不显示
            If style = ProcessWindowStyle.Hidden Then
                pro.StartInfo.CreateNoWindow = True
            Else
                pro.StartInfo.WindowStyle = style
            End If
            pro.Start()
            pro.WaitForExit()
            Return pro.StandardOutput.ReadToEnd()
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 获取进程信息
    ''' </summary>
    ''' <returns>进程信息</returns>
    ''' <remarks></remarks>
    Public Shared Function ProcessList() As Process_Info()
        Try
            Dim pack As List(Of Process_Info) = New List(Of Process_Info)
            Dim mProcessList As Process() = Process.GetProcesses()
            For Each pro As Process In mProcessList
                Dim ss As New Process_Info
                ss.ProcessName = pro.ProcessName
                ss.ProcessID = pro.Id
                ss.ProcessSessionID = pro.SessionId
                ss.Processhwnd = pro.Handle
                ss.Responding = pro.Responding
                ss.StartTime = pro.StartTime
                ss.Memory = pro.WorkingSet64 / 1000
                ss.User = pro.StartInfo.UserName
                ss.FilePath = pro.StartInfo.FileName
                ss.CPU = 0
                pack.Add(ss)
            Next
            If pack.Count > 0 Then
                Dim res(pack.Count - 1) As Process_Info
                pack.CopyTo(res)
                Return res
            Else
                Return Nothing
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 启动服务
    ''' </summary>
    ''' <param name="ServicesName">服务名</param>
    ''' <remarks></remarks>
    Public Shared Sub ServicesStart(ByVal ServicesName As String)
        Try
            If ServicesName Is Nothing Then

            ElseIf ServicesName.Trim() = "" Then

            Else
                Shell("net start """ + ServicesName.Trim() + """")
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 关闭服务
    ''' </summary>
    ''' <param name="ServicesName">服务名</param>
    ''' <remarks></remarks>
    Public Shared Sub ServicesStop(ByVal ServicesName As String)
        Try
            If ServicesName Is Nothing Then

            ElseIf ServicesName.Trim() = "" Then

            Else
                Shell("net stop """ + ServicesName.Trim() + """")
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 启动服务
    ''' </summary>
    ''' <param name="ServicesName">服务名</param>
    ''' <remarks></remarks>
    Public Shared Sub ServiceStart(ByVal ServicesName As String)
        Try
            If ServicesName Is Nothing Then

            ElseIf ServicesName.Trim() = "" Then

            Else
                Dim sc As System.ServiceProcess.ServiceController = New System.ServiceProcess.ServiceController()
                sc.ServiceName = ServicesName
                If sc.Status <> ServiceProcess.ServiceControllerStatus.Running Then
                    sc.Start()
                End If
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 关闭服务
    ''' </summary>
    ''' <param name="ServicesName">服务名</param>
    ''' <remarks></remarks>
    Public Shared Sub ServiceStop(ByVal ServicesName As String)
        Try
            If ServicesName Is Nothing Then

            ElseIf ServicesName.Trim() = "" Then

            Else
                Dim sc As System.ServiceProcess.ServiceController = New System.ServiceProcess.ServiceController()
                sc.ServiceName = ServicesName
                If sc.Status = ServiceProcess.ServiceControllerStatus.Running Then
                    sc.Stop()
                End If
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 获取服务状态
    ''' </summary>
    ''' <param name="ServicesName">服务名</param>
    ''' <returns>是否在运行</returns>
    ''' <remarks></remarks>
    Public Shared Function ServiceState(ByVal ServicesName As String) As Boolean
        Try
            If ServicesName Is Nothing Then
                Return False
            ElseIf ServicesName.Trim() = "" Then
                Return False
            Else
                Dim sc As System.ServiceProcess.ServiceController = New System.ServiceProcess.ServiceController()
                sc.ServiceName = ServicesName
                If sc.Status = ServiceProcess.ServiceControllerStatus.Running Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class