Imports System
Imports System.Management
Imports System.Security
Imports System.Security.Cryptography
Imports Microsoft.Win32
Public Class Account

    Private Shared ErrMsg As String = ""

    Public Enum PowerCrtl As Integer
        Err = -2
        Null = -1
        None = 0
        Read = 1
        Write = 2
        RUN = 3
        All = 4
    End Enum

    Public Structure FilePower
        Public Account As String
        Public Power As PowerCrtl
    End Structure

    Public Structure FileACL
        Public Account As String
        Public ACLList As System.Security.AccessControl.FileSystemRights
        Public Action As Boolean
    End Structure

    Public Structure Account_Info
        Public AccountType As Integer
        Public Caption As String
        Public Description As String
        Public Disabled As Boolean
        Public Domain As String
        Public FullName As String
        Public InstallDate As Date
        Public LocalAccount As Boolean
        Public Lockout As Boolean
        Public Name As String
        Public PasswordChangeable As Boolean
        Public PasswordExpires As Boolean
        Public PasswordRequired As Boolean
        Public SID As String
        Public SIDType As Integer
        Public Status As String
    End Structure

    Public Structure Groups_Info
        Public Caption As String
        Public Description As String
        Public Domain As String
        Public InstallDate As Date
        Public LocalAccount As Boolean
        Public Name As String
        Public SID As String
        Public SIDType As Integer
        Public Status As String
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
    ''' 获取所有系统组
    ''' </summary>
    ''' <value></value>
    ''' <returns>账号组</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property UserGroups() As Groups_Info()
        Get
            Try
                Dim pack As List(Of Groups_Info) = New List(Of Groups_Info)
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_Group  ")
                For Each queryObj As ManagementObject In searcher.Get()
                    Dim Groups As Groups_Info
                    Groups.Caption = queryObj("Caption")
                    Groups.Description = queryObj("Description")
                    Groups.Domain = queryObj("Domain")
                    Try
                        Groups.InstallDate = queryObj("InstallDate")
                    Catch
                        Groups.InstallDate = Date.Now
                    End Try
                    Try
                        Groups.LocalAccount = queryObj("LocalAccount")
                    Catch
                        Groups.LocalAccount = True
                    End Try
                    Groups.Name = queryObj("Name")
                    Groups.SID = queryObj("SID")
                    Dim SIDType As Integer = 0
                    Try
                        Groups.SID = queryObj("SIDType")
                    Catch
                        Groups.SID = -1
                    End Try
                    Groups.Status = queryObj("Status")
                    pack.Add(Groups)
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As Groups_Info
                    pack.CopyTo(res)
                    Return res
                Else
                    Return Nothing
                End If
            Catch err As ManagementException
                ErrMsg = err.Message
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取本地账号组
    ''' </summary>
    ''' <value></value>
    ''' <returns>账号组</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property LocalUserGroups() As Groups_Info()
        Get
            Try
                Dim pack As List(Of Groups_Info) = New List(Of Groups_Info)
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_Group WHERE LocalAccount = True")
                For Each queryObj As ManagementObject In searcher.Get()
                    Dim Groups As Groups_Info
                    Groups.Caption = queryObj("Caption")
                    Groups.Description = queryObj("Description")
                    Groups.Domain = queryObj("Domain")
                    Try
                        Groups.InstallDate = queryObj("InstallDate")
                    Catch
                        Groups.InstallDate = Date.Now
                    End Try
                    Groups.LocalAccount = True
                    Groups.Name = queryObj("Name")
                    Groups.SID = queryObj("SID")
                    Dim SIDType As Integer = 0
                    Try
                        Groups.SID = queryObj("SIDType")
                    Catch
                        Groups.SID = -1
                    End Try
                    Groups.Status = queryObj("Status")
                    pack.Add(Groups)
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As Groups_Info
                    pack.CopyTo(res)
                    Return res
                Else
                    Return Nothing
                End If
            Catch err As ManagementException
                ErrMsg = err.Message
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 通过名称获取组信息
    ''' </summary>
    ''' <param name="Name">搜索的组名</param>
    ''' <returns>组信息</returns>
    ''' <remarks></remarks>
    Public Shared Function GetGroupsByName(ByVal Name As String) As Groups_Info
        Try
            Dim pack As List(Of Groups_Info) = New List(Of Groups_Info)
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_Group WHERE LocalAccount = True and Name like '%" + Name + "%' ")
            For Each queryObj As ManagementObject In searcher.Get()
                Dim Groups As Groups_Info
                Groups.Caption = queryObj("Caption")
                Groups.Description = queryObj("Description")
                Groups.Domain = queryObj("Domain")
                Try
                    Groups.InstallDate = queryObj("InstallDate")
                Catch
                    Groups.InstallDate = Date.Now
                End Try
                Groups.LocalAccount = True
                Groups.Name = queryObj("Name")
                Groups.SID = queryObj("SID")
                Dim SIDType As Integer = 0
                Try
                    Groups.SID = queryObj("SIDType")
                Catch
                    Groups.SID = -1
                End Try
                Groups.Status = queryObj("Status")
                pack.Add(Groups)
            Next
            If pack.Count > 0 Then
                Return pack(0)
            Else
                Return Nothing
            End If
        Catch err As ManagementException
            ErrMsg = err.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取本地账号
    ''' </summary>
    ''' <value></value>
    ''' <returns>账号信息</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property LocalAccount() As Account_Info()
        Get
            Try
                Dim pack As List(Of Account_Info) = New List(Of Account_Info)
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_UserAccount WHERE LocalAccount = True")
                For Each queryObj As ManagementObject In searcher.Get()
                    Dim user As Account_Info
                    Try
                        user.AccountType = queryObj("AccountType")
                    Catch
                        user.AccountType = -1
                    End Try
                    user.Caption = queryObj("Caption")
                    user.Description = queryObj("Description")
                    Try
                        user.Disabled = queryObj("Disabled")
                    Catch
                        user.Disabled = False
                    End Try
                    user.Domain = queryObj("Domain")
                    user.FullName = queryObj("FullName")
                    Try
                        user.InstallDate = queryObj("InstallDate")
                    Catch
                        user.InstallDate = Date.Now
                    End Try
                    Try
                        user.LocalAccount = queryObj("LocalAccount")
                    Catch
                        user.LocalAccount = False
                    End Try
                    Try
                        user.Lockout = queryObj("Lockout")
                    Catch
                        user.Lockout = False
                    End Try
                    user.Name = queryObj("Name")
                    Try
                        user.PasswordChangeable = queryObj("PasswordChangeable")
                    Catch
                        user.PasswordChangeable = False
                    End Try
                    Try
                        user.PasswordExpires = queryObj("PasswordExpires")
                    Catch
                        user.PasswordExpires = False
                    End Try
                    Try
                        user.PasswordRequired = queryObj("PasswordRequired")
                    Catch
                        user.PasswordRequired = False
                    End Try
                    user.SID = queryObj("SID")
                    Dim SIDType As Integer = 0
                    Try
                        user.SIDType = queryObj("SIDType")
                    Catch
                        user.SIDType = -1
                    End Try
                    user.Status = queryObj("Status")
                    pack.Add(user)
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As Account_Info
                    pack.CopyTo(res)
                    Return res
                Else
                    Return Nothing
                End If
            Catch err As ManagementException
                ErrMsg = err.Message
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 获取所有账号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property AllAccount() As Account_Info()
        Get
            Try
                Dim pack As List(Of Account_Info) = New List(Of Account_Info)
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_UserAccount ")
                For Each queryObj As ManagementObject In searcher.Get()
                    Dim user As Account_Info
                    Try
                        user.AccountType = queryObj("AccountType")
                    Catch
                        user.AccountType = -1
                    End Try
                    user.Caption = queryObj("Caption")
                    user.Description = queryObj("Description")
                    Try
                        user.Disabled = queryObj("Disabled")
                    Catch
                        user.Disabled = False
                    End Try
                    user.Domain = queryObj("Domain")
                    user.FullName = queryObj("FullName")
                    Try
                        user.InstallDate = queryObj("InstallDate")
                    Catch
                        user.InstallDate = Date.Now
                    End Try
                    Try
                        user.LocalAccount = queryObj("LocalAccount")
                    Catch
                        user.LocalAccount = False
                    End Try
                    Try
                        user.Lockout = queryObj("Lockout")
                    Catch
                        user.Lockout = False
                    End Try
                    user.Name = queryObj("Name")
                    Try
                        user.PasswordChangeable = queryObj("PasswordChangeable")
                    Catch
                        user.PasswordChangeable = False
                    End Try
                    Try
                        user.PasswordExpires = queryObj("PasswordExpires")
                    Catch
                        user.PasswordExpires = False
                    End Try
                    Try
                        user.PasswordRequired = queryObj("PasswordRequired")
                    Catch
                        user.PasswordRequired = False
                    End Try
                    user.SID = queryObj("SID")
                    Dim SIDType As Integer = 0
                    Try
                        user.SIDType = queryObj("SIDType")
                    Catch
                        user.SIDType = -1
                    End Try
                    user.Status = queryObj("Status")
                    pack.Add(user)
                Next
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As Account_Info
                    pack.CopyTo(res)
                    Return res
                Else
                    Return Nothing
                End If
            Catch err As ManagementException
                ErrMsg = err.Message
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' 通过名称获取账号信息
    ''' </summary>
    ''' <param name="Name">查询账号</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAccountByName(ByVal Name As String) As Account_Info
        Try
            Dim pack As List(Of Account_Info) = New List(Of Account_Info)
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_UserAccount WHERE LocalAccount = True and Name like '%" + Name + "%' ")
            For Each queryObj As ManagementObject In searcher.Get()
                Dim user As Account_Info
                Try
                    user.AccountType = queryObj("AccountType")
                Catch
                    user.AccountType = -1
                End Try
                user.Caption = queryObj("Caption")
                user.Description = queryObj("Description")
                Try
                    user.Disabled = queryObj("Disabled")
                Catch
                    user.Disabled = False
                End Try
                user.Domain = queryObj("Domain")
                user.FullName = queryObj("FullName")
                Try
                    user.InstallDate = queryObj("InstallDate")
                Catch
                    user.InstallDate = Date.Now
                End Try
                Try
                    user.LocalAccount = queryObj("LocalAccount")
                Catch
                    user.LocalAccount = False
                End Try
                Try
                    user.Lockout = queryObj("Lockout")
                Catch
                    user.Lockout = False
                End Try
                user.Name = queryObj("Name")
                Try
                    user.PasswordChangeable = queryObj("PasswordChangeable")
                Catch
                    user.PasswordChangeable = False
                End Try
                Try
                    user.PasswordExpires = queryObj("PasswordExpires")
                Catch
                    user.PasswordExpires = False
                End Try
                Try
                    user.PasswordRequired = queryObj("PasswordRequired")
                Catch
                    user.PasswordRequired = False
                End Try
                user.SID = queryObj("SID")
                Dim SIDType As Integer = 0
                Try
                    user.SIDType = queryObj("SIDType")
                Catch
                    user.SIDType = -1
                End Try
                user.Status = queryObj("Status")
                pack.Add(user)
            Next
            If pack.Count > 0 Then
                Return pack(0)
            Else
                Return Nothing
            End If
        Catch err As ManagementException
            ErrMsg = err.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 判断账号是否存在
    ''' </summary>
    ''' <param name="Name">账号名</param>
    ''' <returns>TRUE存在，FALSE不存在</returns>
    ''' <remarks></remarks>
    Public Shared Function AccountIsExist(ByVal Name As String) As Boolean
        Try
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_UserAccount WHERE   Name like '%" + Name.Trim + "%' ")
            For Each queryObj As ManagementObject In searcher.Get()
                Return True
            Next
            Return False
        Catch err As ManagementException
            ErrMsg = err.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 增加账号
    ''' </summary>
    ''' <param name="Account">账号</param>
    ''' <param name="Password">密码</param>
    ''' <param name="Group">用户组</param>
    ''' <param name="active">是否启用</param>
    ''' <param name="HomePath">主目录</param>
    ''' <param name="passwordchg">密码可否改</param>
    ''' <param name="passwordreq">是否要求密码</param>
    ''' <param name="Memo">备注</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function AddAccount(ByVal Account As String, ByVal Password As String, ByVal Group As String, Optional ByVal active As Boolean = True, Optional ByVal HomePath As String = "", Optional ByVal passwordchg As Boolean = True, Optional ByVal passwordreq As Boolean = True, Optional ByVal Memo As String = "") As Boolean
        Try
            If Password Is Nothing Then
                Password = ""
            End If
            If HomePath Is Nothing Then
                HomePath = ""
            End If
            If Memo Is Nothing Then
                Memo = ""
            End If
            If Group Is Nothing Then
                Group = ""
            End If
            If Account Is Nothing Then
                ErrMsg = "账号有误"
                Return False
            ElseIf Account.Trim = "" Then
                ErrMsg = "账号有误"
                Return False
            Else
                Dim cmd As String = "net user " + Account
                If Password.Trim <> "" Then
                    cmd = cmd + " " + Password
                End If
                cmd = cmd + " " + "/add"
                If Not active Then
                    cmd = cmd + " " + "/ACTIVE:NO"
                End If
                If passwordchg Then
                    cmd = cmd + " " + "/PASSWORDCHG:YES"
                Else
                    cmd = cmd + " " + "/PASSWORDCHG:NO"
                End If
                If passwordreq Then
                    cmd = cmd + " " + "/PASSWORDREQ:YES"
                Else
                    cmd = cmd + " " + "/PASSWORDREQ:NO"
                End If
                If HomePath.Trim <> "" Then
                    If IO.Directory.Exists(HomePath) Then
                        cmd = cmd + " " + "/HOMEDIR:" + HomePath
                    End If
                End If
                If Memo.Trim <> "" Then
                    cmd = cmd + " " + "/COMMENT:""" + Memo + """"
                End If
                If Not AccountIsExist(Account) Then
                    Shell(cmd)
                    If AccountIsExist(Account) Then
                        If Group.Trim <> "" Then
                            Try
                                DeleteGroupMember("Users", Account.Trim)
                                Shell("net localgroup """ + Group.Trim + """ " + Account.Trim + " /add ")
                                Shell("net localgroup ""Remote Desktop Users"" " + Account.Trim + " /add ")
                            Catch

                            End Try
                        End If
                        Return True
                    Else
                        ErrMsg = "添加失败"
                        Return False
                    End If
                Else
                    ErrMsg = "账号已存在"
                    Return False
                End If
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 改密码
    ''' </summary>
    ''' <param name="Account">账号</param>
    ''' <param name="Password">密码</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function EditPassword(ByVal Account As String, ByVal Password As String) As Boolean
        Try
            If Account Is Nothing Then
                ErrMsg = "账号有误"
                Return False
            ElseIf Account.Trim = "" Then
                ErrMsg = "账号有误"
                Return False
            Else
                Shell("net user " + Account + " " + Password)
                Return True
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 修改账号
    ''' </summary>
    ''' <param name="Account">要修改的账号</param>
    ''' <param name="Password">密码</param>
    ''' <param name="OrgGroup">原来用户组</param>
    ''' <param name="Group">修改的用户组</param>
    ''' <param name="active">是否启用</param>
    ''' <param name="HomePath">主目录</param>
    ''' <param name="passwordchg">密码是否可更改</param>
    ''' <param name="passwordreq">是否需要密码</param>
    ''' <param name="Memo">备注</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function EditAccount(ByVal Account As String, ByVal Password As String, ByVal OrgGroup As String, ByVal Group As String, Optional ByVal active As Boolean = True, Optional ByVal HomePath As String = "", Optional ByVal passwordchg As Boolean = True, Optional ByVal passwordreq As Boolean = True, Optional ByVal Memo As String = "") As Boolean
        Try
            If Password Is Nothing Then
                Password = ""
            End If
            If HomePath Is Nothing Then
                HomePath = ""
            End If
            If Memo Is Nothing Then
                Memo = ""
            End If
            If Group Is Nothing Then
                Group = ""
            End If
            If Account Is Nothing Then
                ErrMsg = "账号有误"
                Return False
            ElseIf Account.Trim = "" Then
                ErrMsg = "账号有误"
                Return False
            Else
                Dim cmd As String = "net user " + Account
                If Password.Trim <> "" Then
                    cmd = cmd + " " + Password
                End If
                If Not active Then
                    cmd = cmd + " " + "/ACTIVE:NO"
                End If
                If Not passwordchg Then
                    cmd = cmd + " " + "/PASSWORDCHG:NO"
                End If
                If Not passwordreq Then
                    cmd = cmd + " " + "/PASSWORDREQ:NO"
                End If
                If HomePath.Trim <> "" Then
                    If IO.Directory.Exists(HomePath) Then
                        cmd = cmd + " " + "/HOMEDIR:" + HomePath
                    End If
                End If
                If Memo.Trim <> "" Then
                    cmd = cmd + " " + "/COMMENT:""" + Memo + """"
                End If
                If AccountIsExist(Account) Then
                    Shell(cmd)
                    If Group.Trim <> "" And Group.Trim <> OrgGroup.Trim Then
                        Try
                            Shell("net localgroup " + Group.Trim + " " + Account.Trim + " /add ")
                            DeleteGroupMember(OrgGroup.Trim, Account.Trim)
                        Catch

                        End Try
                    End If
                    Return True
                Else
                    ErrMsg = "添加失败"
                    Return False
                End If
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 删除账号
    ''' </summary>
    ''' <param name="Account">账号名</param>
    ''' <returns>TRUE成功，FALSE失败</returns>
    ''' <remarks></remarks>
    Public Shared Function DeleteAccount(ByVal Account As String) As Boolean
        Try
            If Account Is Nothing Then
                ErrMsg = "账号有误"
                Return False
            ElseIf Account.Trim = "" Then
                ErrMsg = "账号有误"
                Return False
            Else
                Dim cmd As String = "net user " + Account + " /Delete"
                If AccountIsExist(Account) Then
                    Shell(cmd)
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 添加用户到组
    ''' </summary>
    ''' <param name="GroupName">组名</param>
    ''' <param name="UserName">用户名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AddGroupMember(ByVal GroupName As String, ByVal UserName As String) As Boolean
        Try
            Shell("net localgroup " + GroupName.Trim + " " + UserName.Trim + " /add ")
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 删除组用户
    ''' </summary>
    ''' <param name="GroupName">组名</param>
    ''' <param name="UserName">用户名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DeleteGroupMember(ByVal GroupName As String, ByVal UserName As String) As Boolean
        Try
            Shell("net localgroup " + GroupName.Trim + " " + UserName.Trim + " /delete ")
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 设置自动登录
    ''' </summary>
    ''' <param name="UserName">账户名</param>
    ''' <param name="PassWord">密码</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function AutoLoginOn(ByVal UserName As String, ByVal PassWord As String) As Boolean
        Try
            If UserName Is Nothing Then
                Return False
            ElseIf UserName.Trim = "" Then
                Return False
            End If
            If PassWord Is Nothing Then
                PassWord = ""
            End If
            Dim hklm As RegistryKey = Registry.LocalMachine
            Dim autol As RegistryKey = hklm.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", True)
            autol.SetValue("DefaultUserName", UserName)
            autol.SetValue("DefaultPassword", PassWord)
            autol.SetValue("AutoAdminLogon", 1)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 关闭自动登录
    ''' </summary>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function AutoLoginOff() As Boolean
        Try
            Dim hklm As RegistryKey = Registry.LocalMachine
            Dim autol As RegistryKey = hklm.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", True)
            autol.SetValue("AutoAdminLogon", 0)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function


    ''' <summary>
    ''' 添加文件夹，带ACL行为控制
    ''' </summary>
    ''' <param name="Directory">目录</param>
    ''' <param name="Power">权限</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreatDirectory(ByVal Directory As String, ByVal Power As FilePower) As Boolean
        Try
            Dim FS As IO.DirectoryInfo = New IO.DirectoryInfo(Directory)
            If Not FS.Exists Then
                FS.Create()
            End If
            Dim ACL As System.Security.AccessControl.DirectorySecurity = FS.GetAccessControl
            If Not Power.Account Is Nothing And Power.Account.Trim <> "" Then
                Select Case Power.Power
                    Case PowerCrtl.All
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.FullControl, AccessControl.AccessControlType.Allow))
                    Case PowerCrtl.Read
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Read, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.ListDirectory, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Write, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.CreateFiles, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Modify, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Delete, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.ExecuteFile, AccessControl.AccessControlType.Deny))
                    Case PowerCrtl.Write
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Read, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.ListDirectory, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Write, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.CreateFiles, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Modify, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Delete, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.ExecuteFile, AccessControl.AccessControlType.Deny))
                    Case PowerCrtl.RUN
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Read, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.ListDirectory, AccessControl.AccessControlType.Allow))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Write, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.CreateFiles, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Modify, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.Delete, AccessControl.AccessControlType.Deny))
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.ExecuteFile, AccessControl.AccessControlType.Allow))
                    Case PowerCrtl.None
                        ACL.AddAccessRule(New AccessControl.FileSystemAccessRule(Power.Account, AccessControl.FileSystemRights.FullControl, AccessControl.AccessControlType.Deny))
                    Case Else

                End Select
            End If
            FS.SetAccessControl(ACL)
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

End Class