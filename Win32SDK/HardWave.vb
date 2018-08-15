Imports System.Management
Imports System.Runtime.InteropServices
    Public Class HardWave
        Private Shared ErrMsg As String = ""

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
        ''' CPU信息
        ''' </summary>
        ''' <remarks></remarks>
        <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
         Public Structure CPU_info
            ''' <summary>
            ''' 设备ID
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public DeviceID As String
            ''' <summary>
            ''' CPUID
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public ProcessorID As String
            ''' <summary>
            ''' 制造商
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public Manufacturer As String
            ''' <summary>
            ''' 设备名
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public Name As String
            ''' <summary>
            ''' 设备描述
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Description As String
            ''' <summary>
            ''' 唯一ID
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public UniqueId As String
            ''' <summary>
            ''' 连接插口
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public SocketDesignation As String
            ''' <summary>
            ''' 地址位宽
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public AddressWidth As String
            ''' <summary>
            ''' 数据位宽
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public DataWidth As String
            ''' <summary>
            ''' 家族系列
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Family As String
            ''' <summary>
            ''' 级别
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Level As String
            ''' <summary>
            ''' 步进
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Stepping As String
            ''' <summary>
            ''' 最高频率
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public MaxClockSpeed As String
            ''' <summary>
            ''' 当前频率
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public CurrentClockSpeed As String
            ''' <summary>
            ''' 安装日期
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public InstallDate As String
            ''' <summary>
            ''' PNP设备ID
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public PNPDeviceID As String
            ''' <summary>
            ''' 二级缓存大小
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public L2CacheSize As String
            ''' <summary>
            ''' 二级缓存速度
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public L2CacheSpeed As String
            ''' <summary>
            ''' 三级缓存大小
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public L3CacheSize As String
            ''' <summary>
            ''' 三级缓存速度
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public L3CacheSpeed As String
            ''' <summary>
            ''' 处理器架构
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Architecture As String
            ''' <summary>
            ''' 可用状态
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Availability As String
            ''' <summary>
            ''' 当前电压
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public CurrentVoltage As String
            ''' <summary>
            ''' 处理器体系
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public ProcessorType As String
            ''' <summary>
            ''' 状态
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public StatusInfo As String
            ''' <summary>
            ''' 升级方法
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public UpgradeMethod As String
            ''' <summary>
            ''' 核心电压
            ''' </summary>
            ''' <remarks></remarks>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public VoltageCaps As String
        End Structure
        ''' <summary>
        ''' 获取CPU信息
        ''' </summary>
        ''' <returns>CPU信息</returns>
        ''' <remarks></remarks>
        Public Shared Function Get_CPU_info() As CPU_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim cpu As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor")
                For Each obj1 As System.Management.ManagementObject In cpu.Get
                    Dim Mes As CPU_info
                    Try
                        Mes.DeviceID = obj1("DeviceID")
                    Catch
                        Mes.DeviceID = ""
                    End Try
                    Try
                        Mes.ProcessorID = obj1("ProcessorID").ToString.Trim
                    Catch
                        Mes.ProcessorID = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj1("Manufacturer").ToString.Trim
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.Name = obj1("Name").ToString.Trim
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Description = obj1("Description").ToString.Trim
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.UniqueId = obj1("UniqueId")
                    Catch
                        Mes.UniqueId = ""
                    End Try
                    Try
                        Mes.SocketDesignation = obj1("SocketDesignation")
                    Catch
                        Mes.SocketDesignation = ""
                    End Try
                    Try
                        Mes.AddressWidth = obj1("AddressWidth")
                    Catch
                        Mes.AddressWidth = ""
                    End Try
                    Try
                        Mes.DataWidth = obj1("DataWidth")
                    Catch
                        Mes.DataWidth = ""
                    End Try
                    Try
                        Mes.Family = obj1("Family")
                    Catch
                        Mes.Family = ""
                    End Try
                    Try
                        Mes.Level = obj1("Level")
                    Catch
                        Mes.Level = ""
                    End Try
                    Try
                        Mes.Stepping = obj1("Stepping")
                    Catch
                        Mes.Stepping = ""
                    End Try
                    Try
                        Mes.MaxClockSpeed = obj1("MaxClockSpeed")
                    Catch
                        Mes.MaxClockSpeed = ""
                    End Try
                    Try
                        Mes.CurrentClockSpeed = obj1("CurrentClockSpeed")
                    Catch
                        Mes.CurrentClockSpeed = ""
                    End Try
                    Try
                        Mes.InstallDate = obj1("InstallDate")
                    Catch
                        Mes.InstallDate = ""
                    End Try
                    Try
                        Mes.PNPDeviceID = obj1("PNPDeviceID")
                    Catch
                        Mes.PNPDeviceID = ""
                    End Try
                    Try
                        Mes.L2CacheSize = obj1("L2CacheSize")
                    Catch
                        Mes.L2CacheSize = ""
                    End Try
                    Try
                        Mes.L2CacheSpeed = obj1("L2CacheSpeed")
                    Catch
                        Mes.L2CacheSpeed = ""
                    End Try
                    Try
                        Mes.L3CacheSize = obj1("L3CacheSize")
                    Catch
                        Mes.L3CacheSize = ""
                    End Try
                    Try
                        Mes.L3CacheSpeed = obj1("L3CacheSpeed")
                    Catch
                        Mes.L3CacheSpeed = ""
                    End Try
                    Try
                        Select Case obj1("Architecture").ToString
                            Case "0"
                                Mes.Architecture = "x86"
                            Case "1"
                                Mes.Architecture = "MIPS"
                            Case "2"
                                Mes.Architecture = "Alpha"
                            Case "3"
                                Mes.Architecture = "PowerPC"
                            Case "6"
                                Mes.Architecture = "Intel Itanium Processor Family (IPF)"
                            Case "9"
                                Mes.Architecture = "x64"
                            Case Else
                                Mes.Architecture = obj1("Architecture").ToString
                        End Select
                    Catch
                        Mes.Architecture = ""
                    End Try
                    Try
                        Mes.Availability = obj1("Availability")
                    Catch
                        Mes.Availability = ""
                    End Try
                    Try
                        Mes.CurrentVoltage = obj1("CurrentVoltage")
                    Catch ex As Exception
                        Mes.CurrentVoltage = ""
                    End Try
                    Try
                        Select Case obj1("ProcessorType")
                            Case "1"
                                Mes.ProcessorType = "Other"
                            Case "2"
                                Mes.ProcessorType = "Unknown"
                            Case "3"
                                Mes.ProcessorType = "Central Processor"
                            Case "4"
                                Mes.ProcessorType = "Math Processor"
                            Case "5"
                                Mes.ProcessorType = "DSP Processor"
                            Case "6"
                                Mes.ProcessorType = "Video Processor"
                            Case Else
                                Mes.ProcessorType = obj1("ProcessorType")
                        End Select
                    Catch
                        Mes.ProcessorType = ""
                    End Try
                    Try
                        Select Case obj1("StatusInfo")
                            Case "1"
                                Mes.StatusInfo = "Other"
                            Case "2"
                                Mes.StatusInfo = "Unknown"
                            Case "3"
                                Mes.StatusInfo = "Enabled"
                            Case "4"
                                Mes.StatusInfo = "Disabled"
                            Case "5"
                                Mes.StatusInfo = "Not Applicable"
                            Case Else
                                Mes.StatusInfo = obj1("StatusInfo")
                        End Select
                    Catch
                        Mes.StatusInfo = ""
                    End Try
                    Try
                        Select Case obj1("UpgradeMethod").ToString
                            Case "1"
                                Mes.UpgradeMethod = "Other"
                            Case "2"
                                Mes.UpgradeMethod = "Unknown"
                            Case "3"
                                Mes.UpgradeMethod = "Daughter Board"
                            Case "4"
                                Mes.UpgradeMethod = "ZIF Socket"
                            Case "5"
                                Mes.UpgradeMethod = "Replacement/Piggy Back"
                            Case "6"
                                Mes.UpgradeMethod = "None"
                            Case "7"
                                Mes.UpgradeMethod = "LIF Socket"
                            Case "8"
                                Mes.UpgradeMethod = "Slot 1"
                            Case "9"
                                Mes.UpgradeMethod = "Slot 2"
                            Case "10"
                                Mes.UpgradeMethod = "370 Pin Socket"
                            Case "11"
                                Mes.UpgradeMethod = "Slot A"
                            Case "12"
                                Mes.UpgradeMethod = "Slot M"
                            Case "13"
                                Mes.UpgradeMethod = "Socket 423"
                            Case "14"
                                Mes.UpgradeMethod = "Socket A (Socket 462)"
                            Case "15"
                                Mes.UpgradeMethod = "Socket 478"
                            Case "16"
                                Mes.UpgradeMethod = "Socket 754"
                            Case "17"
                                Mes.UpgradeMethod = "Socket 940"
                            Case "18"
                                Mes.UpgradeMethod = "Socket 939"
                            Case Else
                                Mes.UpgradeMethod = obj1("UpgradeMethod").ToString
                        End Select
                    Catch
                        Mes.UpgradeMethod = ""
                    End Try
                    Dim tmp As UInt32
                    Try
                        tmp = obj1("VoltageCaps")
                    Catch
                        tmp = 0
                    End Try
                    Select Case tmp
                        Case 0
                            Mes.VoltageCaps = "Unknown"
                        Case 1
                            Mes.VoltageCaps = "5 volts"
                        Case 2
                            Mes.VoltageCaps = "3.3 volts"
                        Case 3
                            Mes.VoltageCaps = "2.9 volts"
                        Case Else
                            Mes.VoltageCaps = Hex$(tmp)
                    End Select
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As CPU_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 操作系统信息
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure OS_info
            ''' <summary>
            ''' 系统名称
            ''' </summary>
            ''' <remarks></remarks>
            Public CSName As String
            ''' <summary>
            ''' 描述
            ''' </summary>
            ''' <remarks></remarks>
            Public Caption As String
            ''' <summary>
            ''' 版本
            ''' </summary>
            ''' <remarks></remarks>
            Public Version As String
            ''' <summary>
            ''' 内部版本
            ''' </summary>
            ''' <remarks></remarks>
            Public BuildNumber As String
            ''' <summary>
            ''' 内部类型
            ''' </summary>
            ''' <remarks></remarks>
            Public BuildType As String
            ''' <summary>
            ''' 操作系统类型
            ''' </summary>
            ''' <remarks></remarks>
            Public OSType As String
            ''' <summary>
            ''' 操作系统套装
            ''' </summary>
            ''' <remarks></remarks>
            Public OSProductsuite As String
            ''' <summary>
            ''' 其他类型描述
            ''' </summary>
            ''' <remarks></remarks>
            Public OtherTypeDescription As String
        End Structure
        ''' <summary>
        ''' 获取系统信息
        ''' </summary>
        ''' <returns>操作系统信息</returns>
        ''' <remarks></remarks>
        Public Shared Function Get_OS_info() As OS_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_OperatingSystem")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As OS_info
                    Try
                        Mes.CSName = obj2("CSName")
                    Catch
                        Mes.CSName = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.Version = obj2("Version")
                    Catch
                        Mes.Version = ""
                    End Try
                    Try
                        Mes.BuildNumber = obj2("BuildNumber")
                    Catch
                        Mes.BuildNumber = ""
                    End Try
                    Try
                        Mes.OSType = obj2("OSType")
                    Catch
                        Mes.OSType = ""
                    End Try
                    Try
                        Mes.OSProductsuite = obj2("OSProductsuite")
                    Catch
                        Mes.OSProductsuite = ""
                    End Try
                    Try
                        Mes.OtherTypeDescription = obj2("OtherTypeDescription")
                    Catch
                        Mes.OtherTypeDescription = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As OS_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 网卡信息
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure NetApter_info
            ''' <summary>
            ''' 网卡索引号
            ''' </summary>
            ''' <remarks></remarks>
            Public Index As String
            ''' <summary>
            ''' 是否使用
            ''' </summary>
            ''' <remarks></remarks>
            Public IPEnabled As Boolean
            ''' <summary>
            ''' 名称
            ''' </summary>
            ''' <remarks></remarks>
            Public Caption As String
            ''' <summary>
            ''' MAC地址
            ''' </summary>
            ''' <remarks></remarks>
            Public MACAddress As String
            ''' <summary>
            ''' IP地址
            ''' </summary>
            ''' <remarks></remarks>
            Public IPAddress As String
            ''' <summary>
            ''' 默认网关
            ''' </summary>
            ''' <remarks></remarks>
            Public DefaultIPGateway As String
            ''' <summary>
            ''' 子网掩码
            ''' </summary>
            ''' <remarks></remarks>
            Public IPSubnet As String
            ''' <summary>
            ''' 路由
            ''' </summary>
            ''' <remarks></remarks>
            Public IPConnectionMetric As String
            ''' <summary>
            ''' 允许在 IP 上运行的协议构成的数组
            ''' </summary>
            ''' <remarks></remarks>
            Public IPSecPermitIPProtocols As String
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <remarks></remarks>
            Public IPSecPermitTCPPorts As String
            Public IPSecPermitUDPPorts As String
            Public IPUseZeroBroadcast As String
            Public IPFilterSecurityEnabled As String
            Public IPPortSecurityEnabled As String
            Public DefaultTOS As String
            ''' <summary>
            ''' TTL
            ''' </summary>
            ''' <remarks></remarks>
            Public DefaultTTL As String
            ''' <summary>
            ''' IPV6传输类型
            ''' </summary>
            ''' <remarks></remarks>
            Public IPXMediaType As String
            ''' <summary>
            ''' IPV6地址
            ''' </summary>
            ''' <remarks></remarks>
            Public IPXAddress As String
            ''' <summary>
            ''' IPV6祯类型
            ''' </summary>
            ''' <remarks></remarks>
            Public IPXFrameType As String
            ''' <summary>
            ''' IPV6网络号
            ''' </summary>
            ''' <remarks></remarks>
            Public IPXNetworkNumber As String
            ''' <summary>
            ''' IPV6虚拟网络号
            ''' </summary>
            ''' <remarks></remarks>
            Public IPXVirtualNetNumber As String
            ''' <summary>
            ''' 缓冲区
            ''' </summary>
            ''' <remarks></remarks>
            Public ForwardBufferMemory As String
            ''' <summary>
            ''' 服务名
            ''' </summary>
            ''' <remarks></remarks>
            Public ServiceName As String
            ''' <summary>
            ''' DHCP是否启用
            ''' </summary>
            ''' <remarks></remarks>
            Public DHCPEnabled As String
            ''' <summary>
            ''' DHCP有效期
            ''' </summary>
            ''' <remarks></remarks>
            Public DHCPLeaseExpires As String
            ''' <summary>
            ''' DHCP生存期
            ''' </summary>
            ''' <remarks></remarks>
            Public DHCPLeaseObtained As String
            ''' <summary>
            ''' DHCP服务
            ''' </summary>
            ''' <remarks></remarks>
            Public DHCPServer As String
            ''' <summary>
            ''' DHCP域
            ''' </summary>
            ''' <remarks></remarks>
            Public DNSDomain As String
            ''' <summary>
            ''' DNS主机名
            ''' </summary>
            ''' <remarks></remarks>
            Public DNSHostName As String
            ''' <summary>
            ''' DNS是否启用
            ''' </summary>
            ''' <remarks></remarks>
            Public DNSEnabledForWINSResolution As String
            ''' <summary>
            ''' DNS服务优先级
            ''' </summary>
            ''' <remarks></remarks>
            Public DNSServerSearchOrder As String
            ''' <summary>
            ''' IGMP级别
            ''' </summary>
            ''' <remarks></remarks>
            Public IGMPLevel As String
            Public TcpipNetbiosOptions As String
            Public TcpMaxConnectRetransmissions As String
            Public TcpMaxDataRetransmissions As String
            Public TcpNumConnections As String
            Public TcpUseRFC1122UrgentPointer As String
            Public TcpWindowSize As String
            Public WINSEnableLMHostsLookup As String
            Public WINSHostLookupFile As String
            Public WINSPrimaryServer As String
            Public WINSScopeID As String
            Public WINSSecondaryServer As String
        End Structure
        ''' <summary>
        ''' 获取网卡信息
        ''' </summary>
        ''' <returns>网卡信息</returns>
        ''' <remarks></remarks>
        Public Shared Function Get_NetApter_info() As NetApter_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_NetworkAdapterConfiguration")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As NetApter_info
                    Try
                        Mes.Index = obj2("Index")
                        If CBool(obj2("IPEnabled")) Then
                            Try
                                Mes.IPEnabled = True
                            Catch
                                Mes.IPEnabled = False
                            End Try
                            Try
                                Mes.Caption = obj2("Caption")
                            Catch
                                Mes.Caption = ""
                            End Try
                            Try
                                Mes.MACAddress = obj2("MACAddress")
                            Catch
                                Mes.MACAddress = ""
                            End Try
                            Try
                                Mes.IPAddress = obj2("IPAddress")(0)
                            Catch
                                Mes.IPAddress = ""
                            End Try
                            Try
                                Mes.DefaultIPGateway = obj2("DefaultIPGateway")(0)
                            Catch
                                Mes.DefaultIPGateway = ""
                            End Try
                            Try
                                Mes.IPSubnet = obj2("IPSubnet")(0)
                            Catch
                                Mes.IPSubnet = "255.255.255.0"
                            End Try
                            Try
                                Mes.DNSServerSearchOrder = obj2("DNSServerSearchOrder")(0)
                            Catch
                                Mes.DNSServerSearchOrder = ""
                            End Try
                            Try
                                Mes.IPConnectionMetric = obj2("IPConnectionMetric")
                            Catch
                                Mes.IPConnectionMetric = ""
                            End Try
                            Try
                                Mes.IPSecPermitIPProtocols = obj2("IPSecPermitIPProtocols")(0)
                            Catch
                                Mes.IPSecPermitIPProtocols = ""
                            End Try
                            Try
                                Mes.IPSecPermitTCPPorts = obj2("IPSecPermitTCPPorts")(0)
                            Catch
                                Mes.IPSecPermitTCPPorts = ""
                            End Try
                            Try
                                Mes.IPSecPermitUDPPorts = obj2("IPSecPermitUDPPorts")(0)
                            Catch
                                Mes.IPSecPermitUDPPorts = ""
                            End Try
                            Try
                                Mes.IPUseZeroBroadcast = obj2("IPUseZeroBroadcast")
                            Catch
                                Mes.IPUseZeroBroadcast = ""
                            End Try
                            Try
                                Mes.IPFilterSecurityEnabled = obj2("IPFilterSecurityEnabled")
                            Catch
                                Mes.IPFilterSecurityEnabled = ""
                            End Try
                            Try
                                Mes.IPPortSecurityEnabled = obj2("IPPortSecurityEnabled")
                            Catch
                                Mes.IPPortSecurityEnabled = ""
                            End Try
                            Try
                                Mes.DefaultTOS = obj2("DefaultTOS")
                            Catch
                                Mes.DefaultTOS = ""
                            End Try
                            Try
                                Mes.DefaultTTL = obj2("DefaultTTL")
                            Catch
                                Mes.DefaultTTL = ""
                            End Try
                            Try
                                Mes.IPXMediaType = obj2("IPXMediaType")
                            Catch
                                Mes.IPXMediaType = ""
                            End Try
                            Try
                                Mes.IPXAddress = obj2("IPXAddress")
                            Catch
                                Mes.IPXAddress = ""
                            End Try
                            Try
                                Mes.IPXFrameType = obj2("IPXFrameType")(0)
                            Catch
                                Mes.IPXFrameType = ""
                            End Try
                            Try
                                Mes.IPXNetworkNumber = obj2("IPXNetworkNumber")(0)
                            Catch
                                Mes.IPXNetworkNumber = ""
                            End Try
                            Try
                                Mes.IPXVirtualNetNumber = obj2("IPXVirtualNetNumber")
                            Catch
                                Mes.IPXVirtualNetNumber = ""
                            End Try
                            Try
                                Mes.ForwardBufferMemory = obj2("ForwardBufferMemory")
                            Catch
                                Mes.ForwardBufferMemory = ""
                            End Try
                            Try
                                Mes.ServiceName = obj2("ServiceName")
                            Catch
                                Mes.ServiceName = ""
                            End Try
                            Try
                                Mes.DHCPEnabled = obj2("DHCPEnabled")
                            Catch
                                Mes.DHCPEnabled = ""
                            End Try
                            Try
                                Mes.DHCPLeaseExpires = obj2("DHCPLeaseExpires").ToString
                            Catch
                                Mes.DHCPLeaseExpires = ""
                            End Try
                            Try
                                Mes.DHCPLeaseObtained = obj2(" DHCPLeaseObtained").ToString
                            Catch
                                Mes.DHCPLeaseObtained = ""
                            End Try
                            Try
                                Mes.DHCPServer = obj2(" DHCPServer").ToString
                            Catch
                                Mes.DHCPServer = ""
                            End Try
                            Try
                                Mes.DNSDomain = obj2(" DNSDomain").ToString
                            Catch
                                Mes.DNSDomain = ""
                            End Try
                            Try
                                Mes.DNSHostName = obj2(" DNSHostName").ToString
                            Catch
                                Mes.DNSHostName = ""
                            End Try
                            Try
                                Mes.DNSEnabledForWINSResolution = obj2(" DNSEnabledForWINSResolution").ToString
                            Catch
                                Mes.DNSEnabledForWINSResolution = ""
                            End Try
                            Try
                                Mes.IGMPLevel = obj2(" IGMPLevel")(0)
                            Catch
                                Mes.IGMPLevel = ""
                            End Try
                            Try
                                Mes.TcpipNetbiosOptions = obj2(" TcpipNetbiosOptions")(0)
                            Catch
                                Mes.TcpipNetbiosOptions = ""
                            End Try
                            Try
                                Mes.TcpMaxConnectRetransmissions = obj2(" TcpMaxConnectRetransmissions")(0)
                            Catch
                                Mes.TcpMaxConnectRetransmissions = ""
                            End Try
                            Try
                                Mes.TcpMaxDataRetransmissions = obj2(" TcpMaxDataRetransmissions")(0)
                            Catch
                                Mes.TcpMaxDataRetransmissions = ""
                            End Try
                            Try
                                Mes.TcpNumConnections = obj2(" TcpNumConnections")(0)
                            Catch
                                Mes.TcpNumConnections = ""
                            End Try
                            Try
                                Mes.TcpUseRFC1122UrgentPointer = obj2(" TcpUseRFC1122UrgentPointer")(0)
                            Catch
                                Mes.TcpUseRFC1122UrgentPointer = ""
                            End Try
                            Try
                                Mes.TcpWindowSize = obj2(" TcpWindowSize")(0)
                            Catch
                                Mes.TcpWindowSize = ""
                            End Try
                            Try
                                Mes.WINSEnableLMHostsLookup = obj2(" WINSEnableLMHostsLookup")(0)
                            Catch
                                Mes.WINSEnableLMHostsLookup = ""
                            End Try
                            Try
                                Mes.WINSHostLookupFile = obj2(" WINSHostLookupFile")(0)
                            Catch
                                Mes.WINSHostLookupFile = ""
                            End Try
                            Try
                                Mes.WINSPrimaryServer = obj2(" WINSPrimaryServer")(0)
                            Catch
                                Mes.WINSPrimaryServer = ""
                            End Try
                            Try
                                Mes.WINSScopeID = obj2(" WINSScopeID")(0)
                            Catch
                                Mes.WINSScopeID = ""
                            End Try
                            Try
                                Mes.WINSSecondaryServer = obj2(" WINSSecondaryServer")(0)
                            Catch
                                Mes.WINSSecondaryServer = ""
                            End Try
                        Else
                            Mes.IPEnabled = False
                        End If
                        Packect.Add(Mes)
                    Catch

                    End Try
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As NetApter_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
        Public Structure HardDisk_info
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)>
            Public Index As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public Caption As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Description As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public DeviceID As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Name As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public MediaType As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public Model As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Manufacturer As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public InterfaceType As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public CreationClassName As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public CompressionMethod As String
            Public Partitions As Long
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public NumberOfMediaSupported As String
            Public Size As Long
            Public MaxMediaSize As Long
            Public TotalHeads As Long
            Public TotalCylinders As Long
            Public TracksPerCylinder As Long
            Public TotalTracks As Long
            Public SectorsPerTrack As Long
            Public TotalSectors As Long
            Public BytesPerSector As Long
            Public MinBlockSize As Long
            Public MaxBlockSize As Long
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Availability As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Capabilities As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public CapabilityDescriptions As String
            Public DefaultBlockSize As Long
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public FirmwareRevision As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public SCSIBus As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public SCSILogicalUnit As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public SCSIPort As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public SCSITargetId As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public SerialNumber As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Signature As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Status As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public StatusInfo As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public SystemCreationClassName As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public SystemName As String
        End Structure
        ''' <summary>
        ''' 获取硬盘信息
        ''' </summary>
        ''' <returns>硬盘信息</returns>
        ''' <remarks></remarks>

        Public Shared Function Get_HardDisk_info() As HardDisk_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_DiskDrive")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As HardDisk_info
                    Try
                        Mes.Index = obj2("Index")
                    Catch
                        Mes.Index = "0"
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.DeviceID = obj2("DeviceID")
                    Catch
                        Mes.DeviceID = ""
                    End Try
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.MediaType = obj2("MediaType")
                    Catch
                        Mes.MediaType = ""
                    End Try
                    Try
                        Mes.Model = obj2("Model")
                    Catch
                        Mes.Model = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.InterfaceType = obj2("InterfaceType")
                    Catch
                        Mes.InterfaceType = ""
                    End Try
                    Try
                        Mes.CreationClassName = obj2("CreationClassName")
                    Catch
                        Mes.CreationClassName = ""
                    End Try
                    Try
                        Mes.CompressionMethod = obj2("CompressionMethod")
                    Catch
                        Mes.CompressionMethod = ""
                    End Try
                    Try
                        Mes.Partitions = obj2("Partitions")
                    Catch
                        Mes.Partitions = 0
                    End Try
                    Try
                        Mes.NumberOfMediaSupported = obj2("NumberOfMediaSupported")
                    Catch
                        Mes.NumberOfMediaSupported = ""
                    End Try
                    Try
                        Mes.Size = obj2("Size") / 1048576
                    Catch
                        Mes.Size = 0
                    End Try
                    Try
                        Mes.MaxMediaSize = obj2("MaxMediaSize")
                    Catch
                        Mes.MaxMediaSize = 0
                    End Try
                    Try
                        Mes.TotalHeads = obj2("TotalHeads")
                    Catch
                        Mes.TotalHeads = 0
                    End Try
                    Try
                        Mes.TotalCylinders = obj2("TotalCylinders")
                    Catch
                        Mes.TotalCylinders = 0
                    End Try
                    Try
                        Mes.TracksPerCylinder = obj2("TracksPerCylinder")
                    Catch
                        Mes.TracksPerCylinder = 0
                    End Try
                    Try
                        Mes.TotalTracks = obj2("TotalTracks")
                    Catch
                        Mes.TotalTracks = 0
                    End Try
                    Try
                        Mes.SectorsPerTrack = obj2("SectorsPerTrack")
                    Catch
                        Mes.SectorsPerTrack = 0
                    End Try
                    Try
                        Mes.TotalSectors = obj2("TotalSectors")
                    Catch
                        Mes.TotalSectors = 0
                    End Try
                    Try
                        Mes.BytesPerSector = obj2("BytesPerSector")
                    Catch
                        Mes.BytesPerSector = 0
                    End Try
                    Try
                        Mes.MinBlockSize = obj2("MinBlockSize")
                    Catch
                        Mes.MinBlockSize = 0
                    End Try
                    Try
                        Mes.MaxBlockSize = obj2("MaxBlockSize")
                    Catch
                        Mes.MaxBlockSize = 0
                    End Try
                    Try
                        Mes.Availability = obj2("Availability")
                    Catch
                        Mes.Availability = ""
                    End Try
                    Try
                        Mes.Capabilities = obj2("Capabilities")(0)
                    Catch
                        Mes.Capabilities = ""
                    End Try
                    Try
                        Mes.CapabilityDescriptions = obj2("CapabilityDescriptions")(0)
                    Catch
                        Mes.CapabilityDescriptions = ""
                    End Try
                    Try
                        Mes.DefaultBlockSize = obj2("DefaultBlockSize")
                    Catch
                        Mes.DefaultBlockSize = 0
                    End Try
                    Try
                        Mes.FirmwareRevision = obj2("FirmwareRevision")
                    Catch
                        Mes.FirmwareRevision = ""
                    End Try
                    Try
                        Mes.SCSIBus = obj2("SCSIBus")
                    Catch
                        Mes.SCSIBus = ""
                    End Try
                    Try
                        Mes.SCSILogicalUnit = obj2("SCSILogicalUnit")
                    Catch
                        Mes.SCSILogicalUnit = ""
                    End Try
                    Try
                        Mes.SCSIPort = obj2("SCSIPort")
                    Catch
                        Mes.SCSIPort = ""
                    End Try
                    Try
                        Mes.SCSITargetId = obj2("SCSITargetId")
                    Catch
                        Mes.SCSITargetId = ""
                    End Try
                    Try
                        Mes.SerialNumber = obj2("SerialNumber")
                    Catch
                        Mes.SerialNumber = ""
                    End Try
                    Try
                        Mes.Signature = obj2("Signature")
                    Catch
                        Mes.Signature = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.StatusInfo = obj2("StatusInfo")
                    Catch
                        Mes.StatusInfo = ""
                    End Try
                    Try
                        Mes.SystemCreationClassName = obj2("SystemCreationClassName")
                    Catch
                        Mes.SystemCreationClassName = ""
                    End Try
                    Try
                        Mes.SystemName = obj2("SystemName")
                    Catch
                        Mes.SystemName = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As HardDisk_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Structure CDRom_info
            Public ID As String
            Public Caption As String
            Public Description As String
            Public DeviceID As String
            Public Name As String
            Public Drive As String
            Public MediaType As String
            Public PNPDeviceID As String
            Public MaxMediaSize As String
            Public MaxBlockSize As String
            Public Manufacturer As String
            Public FileSystemFlags As String
            Public DefaultBlockSize As String
            Public FileSystemFlagsEx As String
            Public Availability As String
            Public CreationClassName As String
            Public MaximumComponentLength As String
            Public MfrAssignedRevisionLevel As String
            Public MinBlockSize As String
            Public NumberOfMediaSupported As String
            Public RevisionLevel As String
            Public VolumeSerialNumber As String
            Public VolumeName As String
            Public TransferRate As String
            Public SystemName As String
            Public SystemCreationClassName As String
            Public Size As Long
            Public StatusInfo As String
            Public Status As String
            Public SerialNumber As String
            Public SCSITargetId As String
            Public SCSIPort As String
            Public SCSIBus As String
            Public SCSILogicalUnit As String
        End Structure
        Public Shared Function Get_CDRom_info() As CDRom_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_CDROMDrive")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As CDRom_info
                    Try
                        Mes.ID = obj2("Id")
                    Catch
                        Mes.ID = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.DeviceID = obj2("DeviceID")
                    Catch
                        Mes.DeviceID = ""
                    End Try
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Drive = obj2("Drive")
                    Catch
                        Mes.Drive = ""
                    End Try
                    Try
                        Mes.MediaType = obj2("MediaType")
                    Catch
                        Mes.MediaType = ""
                    End Try
                    Try
                        Mes.PNPDeviceID = obj2("PNPDeviceID")
                    Catch
                        Mes.PNPDeviceID = ""
                    End Try
                    Try
                        Mes.MaxMediaSize = obj2("MaxMediaSize")
                    Catch
                        Mes.MaxMediaSize = ""
                    End Try
                    Try
                        Mes.MaxBlockSize = obj2("MaxBlockSize")
                    Catch
                        Mes.MaxBlockSize = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.FileSystemFlags = obj2("FileSystemFlags")
                    Catch
                        Mes.FileSystemFlags = ""
                    End Try
                    Try
                        Mes.DefaultBlockSize = obj2("DefaultBlockSize")
                    Catch
                        Mes.DefaultBlockSize = ""
                    End Try
                    Try
                        Mes.FileSystemFlagsEx = obj2("FileSystemFlagsEx")
                    Catch
                        Mes.FileSystemFlagsEx = ""
                    End Try
                    Try
                        Mes.Availability = obj2("Availability")
                    Catch
                        Mes.Availability = ""
                    End Try
                    Try
                        Mes.CreationClassName = obj2("CreationClassName")
                    Catch
                        Mes.CreationClassName = ""
                    End Try
                    Try
                        Mes.MaximumComponentLength = obj2("MaximumComponentLength")
                    Catch
                        Mes.MaximumComponentLength = ""
                    End Try
                    Try
                        Mes.MfrAssignedRevisionLevel = obj2("MfrAssignedRevisionLevel")
                    Catch
                        Mes.MfrAssignedRevisionLevel = ""
                    End Try
                    Try
                        Mes.MinBlockSize = obj2("MinBlockSize")
                    Catch
                        Mes.MinBlockSize = ""
                    End Try
                    Try
                        Mes.NumberOfMediaSupported = obj2("NumberOfMediaSupported")
                    Catch
                        Mes.NumberOfMediaSupported = ""
                    End Try
                    Try
                        Mes.RevisionLevel = obj2("RevisionLevel")
                    Catch
                        Mes.RevisionLevel = ""
                    End Try
                    Try
                        Mes.VolumeSerialNumber = obj2("VolumeSerialNumber")
                    Catch
                        Mes.VolumeSerialNumber = ""
                    End Try
                    Try
                        Mes.VolumeName = obj2("VolumeName")
                    Catch
                        Mes.VolumeName = ""
                    End Try
                    Try
                        Mes.TransferRate = obj2("TransferRate")
                    Catch
                        Mes.TransferRate = ""
                    End Try
                    Try
                        Mes.SystemName = obj2("SystemName")
                    Catch
                        Mes.SystemName = ""
                    End Try
                    Try
                        Mes.SystemCreationClassName = obj2("SystemCreationClassName")
                    Catch
                        Mes.SystemCreationClassName = ""
                    End Try
                    Try
                        Mes.Size = obj2("Size") / 1048576
                    Catch
                        Mes.Size = 0
                    End Try
                    Try
                        Mes.StatusInfo = obj2("StatusInfo")
                    Catch
                        Mes.StatusInfo = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.SerialNumber = obj2("SerialNumber")
                    Catch
                        Mes.SerialNumber = ""
                    End Try
                    Try
                        Mes.SCSITargetId = obj2("SCSITargetId")
                    Catch
                        Mes.SCSITargetId = ""
                    End Try
                    Try
                        Mes.SCSIPort = obj2("SCSIPort")
                    Catch
                        Mes.SCSIPort = ""
                    End Try
                    Try
                        Mes.SCSIBus = obj2("SCSIBus")
                    Catch
                        Mes.SCSIBus = ""
                    End Try
                    Try
                        Mes.SCSILogicalUnit = obj2("SCSILogicalUnit")
                    Catch
                        Mes.SCSILogicalUnit = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As CDRom_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
         Public Structure Display_info
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Dim Name As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Dim Caption As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Dim ColorPlanes As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Dim Description As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Dim RefreshRate As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Dim SettingID As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Dim VideoMode As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Dim VerticalResolution As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Dim SystemPaletteEntries As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Dim ReservedSystemPaletteEntries As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Dim BitsPerPixel As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Dim DeviceEntriesInAColorTable As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Dim DeviceSpecificPens As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Dim HorizontalResolution As String
        End Structure
        Public Shared Function Get_Display_info() As Display_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_DisplayControllerConfiguration")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As Display_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.ColorPlanes = obj2("ColorPlanes")
                    Catch
                        Mes.ColorPlanes = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.RefreshRate = obj2("RefreshRate")
                    Catch
                        Mes.RefreshRate = ""
                    End Try
                    Try
                        Mes.SettingID = obj2("SettingID")
                    Catch
                        Mes.SettingID = ""
                    End Try
                    Try
                        Mes.VideoMode = obj2("VideoMode")
                    Catch
                        Mes.VideoMode = ""
                    End Try
                    Try
                        Mes.VerticalResolution = obj2("VerticalResolution")
                    Catch
                        Mes.VerticalResolution = ""
                    End Try
                    Try
                        Mes.SystemPaletteEntries = obj2("SystemPaletteEntries")
                    Catch
                        Mes.SystemPaletteEntries = ""
                    End Try
                    Try
                        Mes.ReservedSystemPaletteEntries = obj2("ReservedSystemPaletteEntries")
                    Catch
                        Mes.ReservedSystemPaletteEntries = ""
                    End Try
                    Try
                        Mes.BitsPerPixel = obj2("BitsPerPixel")
                    Catch
                        Mes.BitsPerPixel = ""
                    End Try
                    Try
                        Mes.DeviceEntriesInAColorTable = obj2("DeviceEntriesInAColorTable")
                    Catch
                        Mes.DeviceEntriesInAColorTable = ""
                    End Try
                    Try
                        Mes.DeviceSpecificPens = obj2("DeviceSpecificPens")
                    Catch
                        Mes.DeviceSpecificPens = ""
                    End Try
                    Try
                        Mes.HorizontalResolution = obj2("HorizontalResolution")
                    Catch
                        Mes.HorizontalResolution = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As Display_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Structure BIOS_info
            Public Name As String
            Public Caption As String
            Public SerialNumber As String
            Public Manufacturer As String
            Public Description As String
            Public PrimaryBIOS As String
            Public Version As String
            Public Status As String
            Public BuildNumber As String
            Public CodeSet As String
            Public CurrentLanguage As String
            Public IdentificationCode As String
            Public InstallableLanguages As String
            Public InstallDate As String
            Public LanguageEdition As String
            Public OtherTargetOS As String
            Public ReleaseDate As String
            Public SMBIOSBIOSVersion As String
            Public SMBIOSMajorVersion As String
            Public SMBIOSMinorVersion As String
            Public SMBIOSPresent As String
            Public SoftwareElementID As String
            Public SoftwareElementState As String
            Public TargetOperatingSystem As String
            Public ListOfLanguages As String
            Public BIOSVersion As String
            Public BiosCharacteristics As String
        End Structure
        Public Shared Function Get_BIOS_info() As BIOS_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim k As Integer = 0
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_BIOS")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As BIOS_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.SerialNumber = obj2("SerialNumber")
                    Catch ex As Exception
                        Mes.SerialNumber = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.PrimaryBIOS = obj2("PrimaryBIOS")
                    Catch
                        Mes.PrimaryBIOS = ""
                    End Try
                    Try
                        Mes.Version = obj2("Version")
                    Catch
                        Mes.Version = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.BuildNumber = obj2("BuildNumber")
                    Catch
                        Mes.BuildNumber = ""
                    End Try
                    Try
                        Mes.CodeSet = obj2("CodeSet")
                    Catch
                        Mes.CodeSet = ""
                    End Try
                    Try
                        Mes.CurrentLanguage = obj2("CurrentLanguage")
                    Catch
                        Mes.CurrentLanguage = ""
                    End Try
                    Try
                        Mes.IdentificationCode = obj2("IdentificationCode")
                    Catch
                        Mes.IdentificationCode = ""
                    End Try
                    Try
                        Mes.InstallableLanguages = obj2("InstallableLanguages")
                    Catch
                        Mes.InstallableLanguages = ""
                    End Try
                    Try
                        Mes.InstallDate = obj2("InstallDate")
                    Catch
                        Mes.InstallDate = ""
                    End Try
                    Try
                        Mes.LanguageEdition = obj2("LanguageEdition")
                    Catch
                        Mes.LanguageEdition = ""
                    End Try
                    Try
                        Mes.OtherTargetOS = obj2("OtherTargetOS")
                    Catch
                        Mes.OtherTargetOS = ""
                    End Try
                    Try
                        Mes.ReleaseDate = obj2("ReleaseDate")
                    Catch
                        Mes.ReleaseDate = ""
                    End Try
                    Try
                        Mes.SMBIOSBIOSVersion = obj2("SMBIOSBIOSVersion")
                    Catch
                        Mes.SMBIOSBIOSVersion = ""
                    End Try
                    Try
                        Mes.SMBIOSMajorVersion = obj2("SMBIOSMajorVersion")
                    Catch
                        Mes.SMBIOSMajorVersion = ""
                    End Try
                    Try
                        Mes.SMBIOSMinorVersion = obj2("SMBIOSMinorVersion")
                    Catch
                        Mes.SMBIOSMinorVersion = ""
                    End Try
                    Try
                        Mes.SMBIOSPresent = obj2("SMBIOSPresent")
                    Catch
                        Mes.SMBIOSPresent = ""
                    End Try
                    Try
                        Mes.SoftwareElementID = obj2("SoftwareElementID")
                    Catch
                        Mes.SoftwareElementID = ""
                    End Try
                    Try
                        Mes.SoftwareElementState = obj2("SoftwareElementState")
                    Catch
                        Mes.SoftwareElementState = ""
                    End Try
                    Try
                        Mes.TargetOperatingSystem = obj2("TargetOperatingSystem")
                    Catch
                        Mes.TargetOperatingSystem = ""
                    End Try
                    Try
                        Mes.ListOfLanguages = ""
                        For k = 0 To obj2("ListOfLanguages").length - 1
                            Mes.ListOfLanguages += obj2("ListOfLanguages")(k)
                        Next
                    Catch
                        Mes.ListOfLanguages = ""
                    End Try
                    Try
                        Mes.BIOSVersion = ""
                        For k = 0 To obj2("BIOSVersion").length - 1
                            Mes.BIOSVersion += obj2("BIOSVersion")(k)
                        Next
                    Catch
                        Mes.BIOSVersion = ""
                    End Try
                    Try
                        Mes.BiosCharacteristics = ""
                        For k = 0 To obj2("BiosCharacteristics").length - 1
                            Mes.BiosCharacteristics += obj2("BiosCharacteristics")(k)
                        Next
                    Catch ex As Exception
                        Mes.BiosCharacteristics = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As BIOS_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Structure Base_info
            Public Name As String
            Public Caption As String
            Public SerialNumber As String
            Public Manufacturer As String
            Public Description As String
            Public Model As String
            Public HostingBoard As String
            Public HotSwappable As String
            Public InstallDate As String
            Public Version As String
            Public Weight As String
            Public Width As String
            Public Depth As String
            Public Height As String
            Public Tag As String
            Public Status As String
            Public SKU As String
            Public SlotLayout As String
            Public SpecialRequirements As String
            Public RequiresDaughterBoard As String
            Public RequirementsDescription As String
            Public Replaceable As String
            Public Removable As String
            Public Product As String
            Public PoweredOn As String
            Public PartNumber As String
            Public OtherIdentifyingInfo As String
        End Structure
        Public Shared Function Get_Base_info() As Base_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_BaseBoard")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As Base_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.SerialNumber = obj2("SerialNumber")
                    Catch
                        Mes.SerialNumber = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.Model = obj2("Model")
                    Catch
                        Mes.Model = ""
                    End Try
                    Try
                        Mes.HostingBoard = obj2("HostingBoard")
                    Catch
                        Mes.HostingBoard = ""
                    End Try
                    Try
                        Mes.HotSwappable = obj2("HotSwappable")
                    Catch
                        Mes.HotSwappable = ""
                    End Try
                    Try
                        Mes.InstallDate = obj2("InstallDate")
                    Catch
                        Mes.InstallDate = ""
                    End Try
                    Try
                        Mes.Version = obj2("Version")
                    Catch
                        Mes.Version = ""
                    End Try
                    Try
                        Mes.Weight = obj2("Weight")
                    Catch
                        Mes.Weight = ""
                    End Try
                    Try
                        Mes.Width = obj2("Width")
                    Catch
                        Mes.Width = ""
                    End Try
                    Try
                        Mes.Depth = obj2("Depth")
                    Catch
                        Mes.Depth = ""
                    End Try
                    Try
                        Mes.Height = obj2("Height")
                    Catch
                        Mes.Height = ""
                    End Try
                    Try
                        Mes.Tag = obj2("Tag")
                    Catch
                        Mes.Tag = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.SKU = obj2("SKU")
                    Catch
                        Mes.SKU = ""
                    End Try
                    Try
                        Mes.SlotLayout = obj2("SlotLayout")
                    Catch
                        Mes.SlotLayout = ""
                    End Try
                    Try
                        Mes.SpecialRequirements = obj2("SpecialRequirements")
                    Catch
                        Mes.SpecialRequirements = ""
                    End Try
                    Try
                        Mes.RequiresDaughterBoard = obj2("RequiresDaughterBoard")
                    Catch
                        Mes.RequiresDaughterBoard = ""
                    End Try
                    Try
                        Mes.RequirementsDescription = obj2("RequirementsDescription")
                    Catch
                        Mes.RequirementsDescription = ""
                    End Try
                    Try
                        Mes.Replaceable = obj2("Replaceable")
                    Catch
                        Mes.Replaceable = ""
                    End Try
                    Try
                        Mes.Removable = obj2("Removable")
                    Catch
                        Mes.Removable = ""
                    End Try
                    Try
                        Mes.Product = obj2("Product")
                    Catch
                        Mes.Product = ""
                    End Try
                    Try
                        Mes.PoweredOn = obj2("PoweredOn")
                    Catch
                        Mes.PoweredOn = ""
                    End Try
                    Try
                        Mes.PartNumber = obj2("PartNumber")
                    Catch
                        Mes.PartNumber = ""
                    End Try
                    Try
                        Mes.OtherIdentifyingInfo = obj2("OtherIdentifyingInfo")
                    Catch
                        Mes.OtherIdentifyingInfo = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As Base_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Structure Motherbord_info
            Public Name As String
            Public Caption As String
            Public PNPDeviceID As String
            Public DeviceID As String
            Public Description As String
            Public Availability As String
            Public ConfigManagerErrorCode As String
            Public ConfigManagerUserConfig As String
            Public CreationClassName As String
            Public ErrorDescription As String
            Public InstallDate As String
            Public LastErrorCode As String
            Public PowerManagementSupported As String
            Public PrimaryBusType As String
            Public RevisionNumber As String
            Public SecondaryBusType As String
            Public Status As String
            Public StatusInfo As String
            Public SystemCreationClassName As String
            Public SystemName As String
        End Structure
        Public Shared Function Get_Motherbord_info() As Motherbord_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_MotherboardDevice")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As Motherbord_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.PNPDeviceID = obj2("PNPDeviceID")
                    Catch
                        Mes.PNPDeviceID = ""
                    End Try
                    Try
                        Mes.DeviceID = obj2("DeviceID")
                    Catch
                        Mes.DeviceID = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.Availability = obj2("Availability")
                    Catch
                        Mes.Availability = ""
                    End Try
                    Try
                        Mes.ConfigManagerErrorCode = obj2("ConfigManagerErrorCode")
                    Catch
                        Mes.ConfigManagerErrorCode = ""
                    End Try
                    Try
                        Mes.ConfigManagerUserConfig = obj2("ConfigManagerUserConfig")
                    Catch
                        Mes.ConfigManagerUserConfig = ""
                    End Try
                    Try
                        Mes.CreationClassName = obj2("CreationClassName")
                    Catch
                        Mes.CreationClassName = ""
                    End Try
                    Try
                        Mes.ErrorDescription = obj2("ErrorDescription")
                    Catch
                        Mes.ErrorDescription = ""
                    End Try
                    Try
                        Mes.InstallDate = obj2("InstallDate")
                    Catch
                        Mes.InstallDate = ""
                    End Try
                    Try
                        Mes.LastErrorCode = obj2("LastErrorCode")
                    Catch
                        Mes.LastErrorCode = ""
                    End Try
                    Try
                        Mes.PowerManagementSupported = obj2("PowerManagementSupported")
                    Catch
                        Mes.PowerManagementSupported = ""
                    End Try
                    Try
                        Mes.PrimaryBusType = obj2("PrimaryBusType")
                    Catch
                        Mes.PrimaryBusType = ""
                    End Try
                    Try
                        Mes.RevisionNumber = obj2("RevisionNumber")
                    Catch
                        Mes.RevisionNumber = ""
                    End Try
                    Try
                        Mes.SecondaryBusType = obj2("SecondaryBusType")
                    Catch
                        Mes.SecondaryBusType = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.StatusInfo = obj2("StatusInfo")
                    Catch
                        Mes.StatusInfo = ""
                    End Try
                    Try
                        Mes.SystemCreationClassName = obj2("SystemCreationClassName")
                    Catch
                        Mes.SystemCreationClassName = ""
                    End Try
                    Try
                        Mes.SystemName = obj2("SystemName")
                    Catch
                        Mes.SystemName = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As Motherbord_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Structure OnBordDevice_info
            Public Name As String
            Public Caption As String
            Public PNPDeviceID As String
            Public Description As String
            Public Manufacturer As String
            Public Model As String
            Public SerialNumber As String
            Public SKU As String
            Public Status As String
            Public Version As String
            Public Tag As String
            Public PartNumber As String
            Public Replaceable As String
            Public PoweredOn As String
            Public DeviceType As String
            Public CreationClassName As String
            Public HotSwappable As String
            Public Removable As String
            Public InstallDate As String
            Public OtherIdentifyingInfo As String
            Public Enabled As String
        End Structure
        Public Shared Function Get_OnBordDevice_info() As OnBordDevice_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_OnBoardDevice")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As OnBordDevice_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.PNPDeviceID = obj2("PNPDeviceID")
                    Catch
                        Mes.PNPDeviceID = ""
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.Model = obj2("Model")
                    Catch
                        Mes.Model = ""
                    End Try
                    Try
                        Mes.SerialNumber = obj2("SerialNumber")
                    Catch
                        Mes.SerialNumber = ""
                    End Try
                    Try
                        Mes.SKU = obj2("SKU")
                    Catch
                        Mes.SKU = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.Version = obj2("Version")
                    Catch
                        Mes.Version = ""
                    End Try
                    Try
                        Mes.Tag = obj2("Tag")
                    Catch
                        Mes.Tag = ""
                    End Try
                    Try
                        Mes.PartNumber = obj2("PartNumber")
                    Catch
                        Mes.PartNumber = ""
                    End Try
                    Try
                        Mes.Replaceable = obj2("Replaceable")
                    Catch
                        Mes.Replaceable = ""
                    End Try
                    Try
                        Mes.PoweredOn = obj2("PoweredOn")
                    Catch
                        Mes.PoweredOn = ""
                    End Try
                    Try
                        Mes.DeviceType = obj2("DeviceType")
                    Catch
                        Mes.DeviceType = ""
                    End Try
                    Try
                        Mes.CreationClassName = obj2("CreationClassName")
                    Catch
                        Mes.CreationClassName = ""
                    End Try
                    Try
                        Mes.HotSwappable = obj2("HotSwappable")
                    Catch
                        Mes.HotSwappable = ""
                    End Try
                    Try
                        Mes.Removable = obj2("Removable")
                    Catch
                        Mes.Removable = ""
                    End Try
                    Try
                        Mes.InstallDate = obj2("InstallDate")
                    Catch
                        Mes.InstallDate = ""
                    End Try
                    Try
                        Mes.OtherIdentifyingInfo = obj2("OtherIdentifyingInfo")
                    Catch
                        Mes.OtherIdentifyingInfo = ""
                    End Try
                    Try
                        Mes.Enabled = obj2("Enabled")
                    Catch
                        Mes.Enabled = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As OnBordDevice_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
         Public Structure PhysicalMemory_info
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public Tag As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public Name As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public Caption As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Description As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public MemoryType As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Manufacturer As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
            Public Model As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public DeviceLocator As String
            Public Capacity As Long
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public SerialNumber As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Version As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public PartNumber As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public DataWidth As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Speed As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Status As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public TotalWidth As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public TypeDetail As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public SKU As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public Replaceable As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public Removable As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public BankLabel As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public CreationClassName As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public FormFactor As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public HotSwappable As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public InstallDate As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public InterleaveDataDepth As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public InterleavePosition As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public OtherIdentifyingInfo As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public PositionInRow As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public PoweredOn As String
        End Structure
        Public Shared Function Get_PhysicalMemory_info() As PhysicalMemory_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_PhysicalMemory")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As PhysicalMemory_info
                    Try
                        Mes.Tag = obj2("Tag")
                    Catch
                        Mes.Tag = ""
                    End Try
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = 0
                    End Try
                    Try
                        Mes.Description = obj2("Description")
                    Catch
                        Mes.Description = ""
                    End Try
                    Try
                        Mes.MemoryType = obj2("MemoryType")
                    Catch
                        Mes.MemoryType = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.Model = obj2("Model")
                    Catch
                        Mes.Model = ""
                    End Try
                    Try
                        Mes.DeviceLocator = obj2("DeviceLocator")
                    Catch
                        Mes.DeviceLocator = ""
                    End Try
                    Try
                        Mes.Capacity = obj2("Capacity") / 1048576
                    Catch
                        Mes.Capacity = 0
                    End Try
                    Try
                        Mes.SerialNumber = obj2("SerialNumber")
                    Catch
                        Mes.SerialNumber = ""
                    End Try
                    Try
                        Mes.Version = obj2("Version")
                    Catch
                        Mes.Version = ""
                    End Try
                    Try
                        Mes.PartNumber = obj2("PartNumber")
                    Catch
                        Mes.PartNumber = ""
                    End Try
                    Try
                        Mes.DataWidth = obj2("DataWidth")
                    Catch
                        Mes.DataWidth = ""
                    End Try
                    Try
                        Mes.Speed = obj2("Speed")
                    Catch
                        Mes.Speed = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.TotalWidth = obj2("TotalWidth")
                    Catch
                        Mes.TotalWidth = ""
                    End Try
                    Try
                        Mes.TypeDetail = obj2("TypeDetail")
                    Catch
                        Mes.TypeDetail = ""
                    End Try
                    Try
                        Mes.SKU = obj2("SKU")
                    Catch
                        Mes.SKU = ""
                    End Try
                    Try
                        Mes.Replaceable = obj2("Replaceable")
                    Catch
                        Mes.Replaceable = ""
                    End Try
                    Try
                        Mes.Removable = obj2("Removable")
                    Catch
                        Mes.Removable = ""
                    End Try
                    Try
                        Mes.BankLabel = obj2("BankLabel")
                    Catch
                        Mes.BankLabel = ""
                    End Try
                    Try
                        Mes.CreationClassName = obj2("CreationClassName")
                    Catch
                        Mes.CreationClassName = ""
                    End Try
                    Try
                        Mes.FormFactor = obj2("FormFactor")
                    Catch
                        Mes.FormFactor = ""
                    End Try
                    Try
                        Mes.HotSwappable = obj2("HotSwappable")
                    Catch
                        Mes.HotSwappable = ""
                    End Try
                    Try
                        Mes.InstallDate = obj2("InstallDate")
                    Catch
                        Mes.InstallDate = ""
                    End Try
                    Try
                        Mes.InterleaveDataDepth = obj2("InterleaveDataDepth")
                    Catch
                        Mes.InterleaveDataDepth = ""
                    End Try
                    Try
                        Mes.InterleavePosition = obj2("InterleavePosition")
                    Catch
                        Mes.InterleavePosition = ""
                    End Try
                    Try
                        Mes.OtherIdentifyingInfo = obj2("OtherIdentifyingInfo")
                    Catch
                        Mes.OtherIdentifyingInfo = ""
                    End Try
                    Try
                        Mes.PositionInRow = obj2("PositionInRow")
                    Catch
                        Mes.PositionInRow = ""
                    End Try
                    Try
                        Mes.PoweredOn = obj2("PoweredOn")
                    Catch
                        Mes.PoweredOn = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As PhysicalMemory_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Structure Audio_info
            Public Name As String
            Public Caption As String
            Public DeviceID As String
            Public Manufacturer As String
            Public PNPDeviceID As String
            Public PowerManagementSupported As String
            Public Status As String
        End Structure
        Public Shared Function Get_Audio_info() As Audio_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As Audio_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.DeviceID = obj2("DeviceID")
                    Catch
                        Mes.DeviceID = ""
                    End Try
                    Try
                        Mes.Manufacturer = obj2("Manufacturer")
                    Catch
                        Mes.Manufacturer = ""
                    End Try
                    Try
                        Mes.PNPDeviceID = obj2("PNPDeviceID")
                    Catch
                        Mes.PNPDeviceID = ""
                    End Try
                    Try
                        Mes.PowerManagementSupported = obj2("PowerManagementSupported")
                    Catch
                        Mes.PowerManagementSupported = ""
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As Audio_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
         Public Structure Video_info
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public Name As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public Caption As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public VideoModeDescription As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public VideoProcessor As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public AdapterCompatibility As String
            Public AdapterRAM As Long
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public Status As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public DriverVersion As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public InstalledDisplayDrivers As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)>
            Public InfFilename As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public PNPDeviceID As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)>
            Public DeviceID As String
        End Structure
        Public Shared Function Get_Video_info() As Video_info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_VideoController")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim Mes As Video_info
                    Try
                        Mes.Name = obj2("Name")
                    Catch
                        Mes.Name = ""
                    End Try
                    Try
                        Mes.Caption = obj2("Caption")
                    Catch
                        Mes.Caption = ""
                    End Try
                    Try
                        Mes.VideoModeDescription = obj2("VideoModeDescription")
                    Catch
                        Mes.VideoModeDescription = ""
                    End Try
                    Try
                        Mes.VideoProcessor = obj2("VideoProcessor")
                    Catch
                        Mes.VideoProcessor = ""
                    End Try
                    Try
                        Mes.AdapterCompatibility = obj2("AdapterCompatibility")
                    Catch
                        Mes.AdapterCompatibility = ""
                    End Try
                    Try
                        Mes.AdapterRAM = obj2("AdapterRAM") / 1048576
                    Catch
                        Mes.AdapterRAM = 0
                    End Try
                    Try
                        Mes.Status = obj2("Status")
                    Catch
                        Mes.Status = ""
                    End Try
                    Try
                        Mes.DriverVersion = obj2("DriverVersion")
                    Catch
                        Mes.DriverVersion = ""
                    End Try
                    Try
                        Mes.InstalledDisplayDrivers = obj2("InstalledDisplayDrivers")
                    Catch
                        Mes.InstalledDisplayDrivers = ""
                    End Try
                    Try
                        Mes.InfFilename = obj2("InfFilename")
                    Catch
                        Mes.InfFilename = ""
                    End Try
                    Try
                        Mes.PNPDeviceID = obj2("PNPDeviceID")
                    Catch
                        Mes.PNPDeviceID = ""
                    End Try
                    Try
                        Mes.DeviceID = obj2("DeviceID")
                    Catch
                        Mes.DeviceID = ""
                    End Try
                    Packect.Add(Mes)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As Video_info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function

        Public Structure USB_Info
            Public Name As String
            Public Caption As String
            Public Description As String
            Public DeviceID As String
            Public VID As String
            Public PID As String
            Public Availability As UShort
            Public PNPDeviceID As String
        End Structure
        Public Shared Function Get_USB_info() As USB_Info()
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim opt As New System.Management.ManagementObjectSearcher("Select * from Win32_USBHub")
                For Each obj2 As System.Management.ManagementObject In opt.Get
                    Dim usb As New USB_Info
                    usb.Name = obj2("Name")
                    usb.Caption = obj2("Caption")
                    usb.Description = obj2("Description")
                    usb.DeviceID = obj2("DeviceID")
                    usb.PNPDeviceID = obj2("PNPDeviceID")
                    usb.Availability = CUShort(obj2("Availability"))
                    Try
                        If obj2("DeviceID").ToString.Contains("\") Then
                            Dim b() As String = Split(obj2("DeviceID").ToString, "\")
                            If b(1).Contains("&") Then
                                Dim c() As String = Split(b(1), "&")
                                usb.VID = c(0).ToUpper.Replace("VID_", "")
                                usb.PID = c(1).ToUpper.Replace("PID_", "")
                            End If
                        End If
                    Catch
                        usb.VID = ""
                        usb.PID = ""
                    End Try
                    Packect.Add(usb)
                Next
                If Packect.Count - 1 >= 0 Then
                    Dim Res(Packect.Count - 1) As USB_Info
                    Packect.CopyTo(Res)
                    Return Res
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
                Return Nothing
            End Try
        End Function

        Public Shared ReadOnly Property CPUID As String()
            Get
            Try
                Dim Packect As System.Collections.ArrayList = New System.Collections.ArrayList
                Dim cpu As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor")
                For Each obj1 As System.Management.ManagementObject In cpu.Get
                        Dim Mes As String
                        Try
                            Mes = obj1("ProcessorID").ToString.Trim
                        Catch
                            Mes = ""
                        End Try
                        Packect.Add(Mes)
                Next
                    If Packect.Count > 0 Then
                        Dim Res(Packect.Count - 1) As String
                        Packect.CopyTo(Res)
                        Return Res
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

