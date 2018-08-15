Imports Microsoft.Win32
Imports System.IO
Imports System.Text
Imports System.Text.Encoding

Public Class Regedit

    Private Shared ErrMsg As String
    ''' <summary>
    ''' 获取错误
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property Err As String
        Get
            Return ErrMsg
        End Get
    End Property
    ''' <summary>
    ''' 清除错误信息
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ClearErr()
        ErrMsg = ""
    End Sub

    ''' <summary>
    ''' 获取ClassesRoot 
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <returns>值</returns>
    ''' <remarks></remarks>
    Public Shared Function GetClassesRoot(ByVal Path As String, ByVal ValueName As String) As String
        Try
            Dim res As String = ""
            Dim key As RegistryKey = Nothing
            key = Registry.ClassesRoot.OpenSubKey(Path, False)
            If Not key.GetValue(ValueName) Is Nothing Then
                res = key.GetValue(ValueName).ToString()
            Else
                res = ""
            End If
            key.Close()
            Return res
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 设置ClassesRoot
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <param name="NewValue">值</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetClassesRoot(ByVal Path As String, ByVal ValueName As String, ByVal NewValue As String) As Boolean
        Try
            Dim key As RegistryKey = Nothing
            key = Registry.ClassesRoot.OpenSubKey(Path, True)
            key.SetValue(ValueName, NewValue)
            key.Close()
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取CurrentUser 
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <returns>值</returns>
    ''' <remarks></remarks>
    Public Shared Function GetCurrentUser(ByVal Path As String, ByVal ValueName As String) As String
        Try
            Dim res As String = ""
            Dim key As RegistryKey = Nothing
            key = Registry.CurrentUser.OpenSubKey(Path, False)
            If Not key.GetValue(ValueName) Is Nothing Then
                res = key.GetValue(ValueName).ToString()
            Else
                res = ""
            End If
            key.Close()
            Return res
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 设置CurrentUser
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <param name="NewValue">值</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetCurrentUser(ByVal Path As String, ByVal ValueName As String, ByVal NewValue As String) As Boolean
        Try
            Dim key As RegistryKey = Nothing
            key = Registry.CurrentUser.OpenSubKey(Path, True)
            key.SetValue(ValueName, NewValue)
            key.Close()
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取LocalMachine 
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <returns>值</returns>
    ''' <remarks></remarks>
    Public Shared Function GetLocalMachine(ByVal Path As String, ByVal ValueName As String) As String
        Try
            Dim res As String = ""
            Dim key As RegistryKey = Nothing
            key = Registry.LocalMachine.OpenSubKey(Path, False)
            If Not key.GetValue(ValueName) Is Nothing Then
                res = key.GetValue(ValueName).ToString()
            Else
                res = ""
            End If
            key.Close()
            Return res
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 设置LocalMachine
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <param name="NewValue">值</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetLocalMachine(ByVal Path As String, ByVal ValueName As String, ByVal NewValue As String) As Boolean
        Try
            Dim key As RegistryKey = Nothing
            key = Registry.LocalMachine.OpenSubKey(Path, True)
            key.SetValue(ValueName, NewValue)
            key.Close()
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取Users 
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <returns>值</returns>
    ''' <remarks></remarks>
    Public Shared Function GetUsers(ByVal Path As String, ByVal ValueName As String) As String
        Try
            Dim res As String = ""
            Dim key As RegistryKey = Nothing
            key = Registry.Users.OpenSubKey(Path, False)
            If Not key.GetValue(ValueName) Is Nothing Then
                res = key.GetValue(ValueName).ToString()
            Else
                res = ""
            End If
            key.Close()
            Return res
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 设置Users
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <param name="NewValue">值</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetUsers(ByVal Path As String, ByVal ValueName As String, ByVal NewValue As String) As Boolean
        Try
            Dim key As RegistryKey = Nothing
            key = Registry.Users.OpenSubKey(Path, True)
            key.SetValue(ValueName, NewValue)
            key.Close()
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取PerformanceData 
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <returns>值</returns>
    ''' <remarks></remarks>
    Public Shared Function GetPerformanceData(ByVal Path As String, ByVal ValueName As String) As String
        Try
            Dim res As String = ""
            Dim key As RegistryKey = Nothing
            key = Registry.PerformanceData.OpenSubKey(Path, False)
            If Not key.GetValue(ValueName) Is Nothing Then
                res = key.GetValue(ValueName).ToString()
            Else
                res = ""
            End If
            key.Close()
            Return res
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 设置PerformanceData
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <param name="NewValue">值</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetPerformanceData(ByVal Path As String, ByVal ValueName As String, ByVal NewValue As String) As Boolean
        Try
            Dim key As RegistryKey = Nothing
            key = Registry.PerformanceData.OpenSubKey(Path, True)
            key.SetValue(ValueName, NewValue)
            key.Close()
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取CurrentConfig 
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <returns>值</returns>
    ''' <remarks></remarks>
    Public Shared Function GetCurrentConfig(ByVal Path As String, ByVal ValueName As String) As String
        Try
            Dim res As String = ""
            Dim key As RegistryKey = Nothing
            key = Registry.CurrentConfig.OpenSubKey(Path, False)
            If Not key.GetValue(ValueName) Is Nothing Then
                res = key.GetValue(ValueName).ToString()
            Else
                res = ""
            End If
            key.Close()
            Return res
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 设置CurrentConfig
    ''' </summary>
    ''' <param name="Path">路径</param>
    ''' <param name="ValueName">键值</param>
    ''' <param name="NewValue">值</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetCurrentConfig(ByVal Path As String, ByVal ValueName As String, ByVal NewValue As String) As Boolean
        Try
            Dim key As RegistryKey = Nothing
            key = Registry.CurrentConfig.OpenSubKey(Path, True)
            key.SetValue(ValueName, NewValue)
            key.Close()
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    Sub New()
      
    End Sub
 

End Class

