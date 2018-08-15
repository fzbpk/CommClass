Imports System.Management
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Net.Sockets

Public Class NetWork

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

    Public Enum WIFIauthentication As Byte
        None = 0
        WEP_Open = 1
        WEP_Share = 2
        WPA_TKIP = 3
        WPA_AES = 4
        WPS2_TKIP = 5
        WPS2_AES = 6
    End Enum


    Public Structure NetWorkInf
        Public Index As Integer '网卡号
        Public InterfaceIndex As Integer '接口号
        Public Description As String '这个是惟一的网卡标识
        Public Caption As String '网卡信息
        Public DatabasePath As String '网关驱动
        Public SettingID As String '注册表位置
        Public DHCPEnabled As Boolean '是否为DHCP
        Public DNSEnabledForWINSResolution As Boolean '是否为WINS
        Public DNSHostName As String '计算机网络名
        Public ServiceName As String
        Public MACAddress As String 'MAC地址
        Public IPAddress As String() 'ipv4地址
        Public IPSubnet As String() '子网掩码
        Public DefaultIPGateway As String() '网关
        Public DNSServerSearchOrder As String() 'DNS
        Public DomainDNSRegistrationEnabled As Boolean
        Public FullDNSRegistrationEnabled As Boolean
        Public GatewayCostMetric As UInt16()
        Public IPConnectionMetric As Integer
        Public IPEnabled As Boolean '网卡是否活动
        Public DNSDomain As String
    End Structure

    Public Structure NetNameInf
        Public Name As String
        Public NetworkInterfaceType As NetworkInterfaceType
        Public Status As OperationalStatus
        Public Description As String
        Public index As String
        Public Speed As Long
        Public SupportsMulticast As Boolean
        Public MAC As String
        Public DHCPEnable As Boolean
        Public Address As String()
        Public SubMask As String()
        Public GateWay As String()
        Public DNS As String()
    End Structure

    ''' <summary>
    ''' 获取所有网络适配器名称
    ''' </summary>
    ''' <returns>网络适配器信息</returns>
    ''' <remarks></remarks>
    Public Shared Function GetALLNetWorkAP() As NetWorkInf()
        Try
            Dim Inf As NetWorkInf = Nothing
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim Wmi As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration")
            For Each WmiObj As ManagementObject In Wmi.Get
                Inf.Caption = WmiObj("Caption")
                Inf.DatabasePath = WmiObj("DatabasePath")
                Inf.DefaultIPGateway = WmiObj("DefaultIPGateway")
                Inf.Description = WmiObj("Description")
                Inf.DHCPEnabled = WmiObj("DHCPEnabled")
                Inf.DNSEnabledForWINSResolution = WmiObj("DNSEnabledForWINSResolution")
                Inf.DNSHostName = WmiObj("DNSHostName")
                Inf.DNSServerSearchOrder = WmiObj("DNSServerSearchOrder")
                Inf.DomainDNSRegistrationEnabled = WmiObj("DomainDNSRegistrationEnabled")
                Inf.FullDNSRegistrationEnabled = WmiObj("FullDNSRegistrationEnabled")
                Inf.GatewayCostMetric = WmiObj("GatewayCostMetric")
                Inf.Index = WmiObj("Index")
                Inf.InterfaceIndex = WmiObj("InterfaceIndex")
                Inf.IPAddress = WmiObj("IPAddress")
                Inf.IPConnectionMetric = WmiObj("IPConnectionMetric")
                Inf.IPEnabled = WmiObj("IPEnabled")
                Inf.IPSubnet = WmiObj("IPSubnet")
                Try
                    Inf.MACAddress = WmiObj("MACAddress").ToString
                Catch
                    Inf.MACAddress = ""
                End Try
                Inf.SettingID = WmiObj("SettingID")
                Inf.ServiceName = WmiObj("ServiceName")
                Inf.DNSDomain = WmiObj("DNSDomain")
                List.Add(Inf)
            Next
            Dim Filelist As NetWorkInf() = Nothing
            If List.Count > 0 Then
                ReDim Filelist(List.Count - 1)
                List.CopyTo(Filelist)
            End If
            Return Filelist
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取可用网络适配器名称
    ''' </summary>
    ''' <returns>网络适配器信息</returns>
    ''' <remarks></remarks>
    Public Shared Function GetEnableNetWorkAP() As NetWorkInf()
        Try
            Dim Inf As NetWorkInf = Nothing
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim Wmi As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration")
            For Each WmiObj As ManagementObject In Wmi.Get
                Inf.Caption = WmiObj("Caption")
                Inf.DatabasePath = WmiObj("DatabasePath")
                Inf.DefaultIPGateway = WmiObj("DefaultIPGateway")
                Inf.Description = WmiObj("Description")
                Inf.DHCPEnabled = WmiObj("DHCPEnabled")
                Inf.DNSEnabledForWINSResolution = WmiObj("DNSEnabledForWINSResolution")
                Inf.DNSHostName = WmiObj("DNSHostName")
                Inf.DNSServerSearchOrder = WmiObj("DNSServerSearchOrder")
                Inf.DomainDNSRegistrationEnabled = WmiObj("DomainDNSRegistrationEnabled")
                Inf.FullDNSRegistrationEnabled = WmiObj("FullDNSRegistrationEnabled")
                Inf.GatewayCostMetric = WmiObj("GatewayCostMetric")
                Inf.Index = WmiObj("Index")
                Inf.InterfaceIndex = WmiObj("InterfaceIndex")
                Inf.IPAddress = WmiObj("IPAddress")
                Inf.IPConnectionMetric = WmiObj("IPConnectionMetric")
                Inf.IPEnabled = WmiObj("IPEnabled")
                Inf.IPSubnet = WmiObj("IPSubnet")
                Try
                    Inf.MACAddress = WmiObj("MACAddress").ToString
                Catch
                    Inf.MACAddress = ""
                End Try
                Inf.SettingID = WmiObj("SettingID")
                Inf.ServiceName = WmiObj("ServiceName")
                Inf.DNSDomain = WmiObj("DNSDomain")
                If CBool(WmiObj("IPEnabled")) Then
                    List.Add(Inf)
                End If
            Next
            Dim Filelist As NetWorkInf() = Nothing
            If List.Count > 0 Then
                ReDim Filelist(List.Count - 1)
                List.CopyTo(Filelist)
            End If
            Return Filelist
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取网络适配器信息
    ''' </summary>
    ''' <param name="Description">网络适配器名称</param>
    ''' <returns>网络适配器信息</returns>
    ''' <remarks></remarks>
    Public Shared Function GetNetWorkInf(ByVal Description As String) As NetWorkInf
        Try
            Dim Inf As NetWorkInf = Nothing
            Dim Wmi As New ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE Description=""" & Description & """")
            For Each WmiObj As ManagementObject In Wmi.Get
                Inf.Caption = WmiObj("Caption")
                Inf.DatabasePath = WmiObj("DatabasePath")
                Inf.DefaultIPGateway = WmiObj("DefaultIPGateway")
                Inf.Description = WmiObj("Description")
                Inf.DHCPEnabled = WmiObj("DHCPEnabled")
                Inf.DNSEnabledForWINSResolution = WmiObj("DNSEnabledForWINSResolution")
                Inf.DNSHostName = WmiObj("DNSHostName")
                Inf.DNSServerSearchOrder = WmiObj("DNSServerSearchOrder")
                Inf.DomainDNSRegistrationEnabled = WmiObj("DomainDNSRegistrationEnabled")
                Inf.FullDNSRegistrationEnabled = WmiObj("FullDNSRegistrationEnabled")
                Inf.GatewayCostMetric = WmiObj("GatewayCostMetric")
                Inf.Index = WmiObj("Index")
                Inf.InterfaceIndex = WmiObj("InterfaceIndex")
                Inf.IPAddress = WmiObj("IPAddress")
                Inf.IPConnectionMetric = WmiObj("IPConnectionMetric")
                Inf.IPEnabled = WmiObj("IPEnabled")
                Inf.IPSubnet = WmiObj("IPSubnet")
                Try
                    Inf.MACAddress = WmiObj("MACAddress").ToString
                Catch
                    Inf.MACAddress = ""
                End Try
                Inf.SettingID = WmiObj("SettingID")
                Inf.ServiceName = WmiObj("ServiceName")
                Inf.DNSDomain = WmiObj("DNSDomain")
                Exit For
            Next
            Return Inf
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取所有网络连接名称
    ''' </summary>
    ''' <returns>网络连接名称</returns>
    ''' <remarks></remarks>
    Public Shared Function GetALLNetWorkName() As String()
        Try
            Dim Name As String = ""
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
            Dim adapter As NetworkInterface
            For Each adapter In adapters
                If adapter.NetworkInterfaceType = NetworkInterfaceType.Ethernet Or adapter.NetworkInterfaceType = NetworkInterfaceType.FastEthernetFx Or adapter.NetworkInterfaceType = NetworkInterfaceType.FastEthernetT Or adapter.NetworkInterfaceType = NetworkInterfaceType.GigabitEthernet Or adapter.NetworkInterfaceType = NetworkInterfaceType.Wireless80211 Then
                    Name = adapter.Name
                    List.Add(Name)
                End If
            Next adapter
            Dim Filelist As String() = Nothing
            If List.Count > 0 Then
                ReDim Filelist(List.Count - 1)
                List.CopyTo(Filelist)
            End If
            Return Filelist
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取可用网络连接名称
    ''' </summary>
    ''' <returns>网络连接名称</returns>
    ''' <remarks></remarks>
    Public Shared Function GetEnableNetWorkName() As String()
        Try
            Dim Name As String = ""
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
            Dim adapter As NetworkInterface
            For Each adapter In adapters
                If adapter.NetworkInterfaceType = NetworkInterfaceType.Ethernet Or adapter.NetworkInterfaceType = NetworkInterfaceType.FastEthernetFx Or adapter.NetworkInterfaceType = NetworkInterfaceType.FastEthernetT Or adapter.NetworkInterfaceType = NetworkInterfaceType.GigabitEthernet Or adapter.NetworkInterfaceType = NetworkInterfaceType.Wireless80211 Then
                    Name = adapter.Name
                    If adapter.OperationalStatus = OperationalStatus.Up Then
                        List.Add(Name)
                    End If
                End If
            Next adapter
            Dim Filelist As String() = Nothing
            If List.Count > 0 Then
                ReDim Filelist(List.Count - 1)
                List.CopyTo(Filelist)
            End If
            Return Filelist
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 网络适配器名转网络连接名
    ''' </summary>
    ''' <param name="NetName">网络适配器名</param>
    ''' <returns>网络连接名</returns>
    ''' <remarks></remarks>
    Public Shared Function NetNameToNetLink(ByVal NetName As String) As String
        Try
            If NetName Is Nothing Then
                Return ""
            ElseIf NetName.Trim = "" Then
                Return ""
            End If
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
            Dim adapter As NetworkInterface
            For Each adapter In adapters
                If adapter.Description.ToLower.Contains(NetName.ToLower.Trim) Then
                    Return adapter.Name
                End If
            Next adapter
            Return ""
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 网络连接名转网络适配器名
    ''' </summary>
    ''' <param name="NetLink">网络连接名</param>
    ''' <returns>网络适配器名</returns>
    ''' <remarks></remarks>
    Public Shared Function NetLinkToNetName(ByVal NetLink As String) As String
        Try
            If NetLink Is Nothing Then
                Return ""
            ElseIf NetLink.Trim = "" Then
                Return ""
            End If
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
            Dim adapter As NetworkInterface
            For Each adapter In adapters
                If adapter.Name.ToLower.Contains(NetLink.ToLower.Trim) Then
                    Return adapter.Description
                End If
            Next adapter
            Return ""
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 获取网络连接信息
    ''' </summary>
    ''' <param name="Name">网络连接名</param>
    ''' <returns>网络连接信息</returns>
    ''' <remarks></remarks>
    Public Shared Function GetNetWorkNameInfBYName(ByVal Name As String) As NetNameInf
        Try
            Dim inf As NetNameInf = Nothing
            Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
            Dim adapter As NetworkInterface
            For Each adapter In adapters
                If Name = adapter.Name Then
                    Dim IPProperties As IPInterfaceProperties = adapter.GetIPProperties
                    inf.DHCPEnable = IPProperties.GetIPv4Properties.IsDhcpEnabled
                    Dim IPV4 As UnicastIPAddressInformationCollection = IPProperties.UnicastAddresses
                    If IPV4.Count > 0 Then
                        Dim IPS(IPV4.Count - 1) As String
                        Dim MaskS(IPV4.Count - 1) As String
                        Dim IPV4IP As UnicastIPAddressInformation
                        Dim i As Integer = 0
                        For Each IPV4IP In IPV4
                            IPS(i) = IPV4IP.Address.ToString
                            MaskS(i) = IPV4IP.IPv4Mask.ToString
                        Next
                        inf.Address = IPS
                        inf.SubMask = MaskS
                        IPS = Nothing
                        IPV4IP = Nothing
                        i = Nothing
                    Else
                        inf.Address = Nothing
                    End If
                    IPV4 = Nothing
                    Dim GWv4 As GatewayIPAddressInformationCollection = IPProperties.GatewayAddresses
                    If GWv4.Count > 0 Then
                        Dim GPS(GWv4.Count - 1) As String
                        Dim GWV4IP As GatewayIPAddressInformation
                        Dim i As Integer = 0
                        For Each GWV4IP In GWv4
                            GPS(i) = GWV4IP.Address.ToString
                        Next
                        inf.GateWay = GPS
                        GPS = Nothing
                        GWV4IP = Nothing
                        i = Nothing
                    Else
                        inf.GateWay = Nothing
                    End If
                    GWv4 = Nothing
                    Dim DNSv4 As IPAddressCollection = IPProperties.DnsAddresses
                    If DNSv4.Count > 0 Then
                        Dim NPS(DNSv4.Count - 1) As String
                        Dim DNSV4IP As Net.IPAddress
                        Dim i As Integer = 0
                        For Each DNSV4IP In DNSv4
                            NPS(i) = DNSV4IP.ToString
                        Next
                        inf.DNS = NPS
                        NPS = Nothing
                        DNSV4IP = Nothing
                        i = Nothing
                    Else
                        inf.DNS = Nothing
                    End If
                    DNSv4 = Nothing
                    inf.Description = adapter.Description
                    inf.index = adapter.Id
                    inf.MAC = adapter.GetPhysicalAddress.ToString
                    inf.Name = adapter.Name
                    inf.NetworkInterfaceType = adapter.NetworkInterfaceType
                    inf.Speed = adapter.Speed
                    inf.Status = adapter.OperationalStatus
                    inf.SupportsMulticast = adapter.SupportsMulticast
                    Exit For
                End If
            Next adapter
            Return inf
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取网络连接信息
    ''' </summary>
    ''' <param name="Description">网络连接名</param>
    ''' <returns>网络连接信息</returns>
    ''' <remarks></remarks>
    Public Shared Function GetNetWorkNameInfBYDescription(ByVal Description As String) As NetNameInf
        Try
            Dim inf As NetNameInf = Nothing
            Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
            Dim adapter As NetworkInterface
            For Each adapter In adapters
                If Description = adapter.Description Then
                    Dim IPV4 As UnicastIPAddressInformationCollection = adapter.GetIPProperties.UnicastAddresses
                    If IPV4.Count > 0 Then
                        Dim IPS(IPV4.Count - 1) As String
                        Dim IPV4IP As UnicastIPAddressInformation
                        Dim i As Integer = 0
                        For Each IPV4IP In IPV4
                            IPS(i) = IPV4IP.Address.ToString
                        Next
                        inf.Address = IPS
                        IPS = Nothing
                        IPV4IP = Nothing
                        i = Nothing
                    Else
                        inf.Address = Nothing
                    End If
                    IPV4 = Nothing
                    inf.Description = adapter.Description
                    inf.index = adapter.Id
                    inf.MAC = adapter.GetPhysicalAddress.ToString
                    inf.Name = adapter.Name
                    inf.NetworkInterfaceType = adapter.NetworkInterfaceType
                    inf.Speed = adapter.Speed
                    inf.Status = adapter.OperationalStatus
                    inf.SupportsMulticast = adapter.SupportsMulticast
                    Exit For
                End If
            Next adapter
            Return inf
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 设置网络适配器IP
    ''' </summary>
    ''' <param name="NetLink">网络适配器</param>
    ''' <param name="DHCP">DHCP开启</param>
    ''' <param name="IP">IP</param>
    ''' <param name="Mask">掩码</param>
    ''' <param name="GateWay">网关</param>
    ''' <param name="DNS">DNS地址</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetIPAP(ByVal NetLink As String, ByVal DHCP As Boolean, ByVal IP As String, ByVal Mask As String, ByVal GateWay As String, ByVal DNS As String) As Boolean
        Try
            Dim Name As String = ""
            Dim List As System.Collections.ArrayList = New System.Collections.ArrayList
            Dim Wmi As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration")
            For Each WmiObj As ManagementObject In Wmi.Get
                Name = WmiObj("Description")
                If Name.ToLower.Trim = NetLink.ToLower.Trim Then
                    If DHCP Then
                        Dim newDNS As ManagementBaseObject = WmiObj.GetMethodParameters("SetDNSServerSearchOrder")
                        newDNS("DNSServerSearchOrder") = Nothing
                        Dim enableDHCP As ManagementBaseObject = WmiObj.InvokeMethod("EnableDHCP", Nothing, Nothing)
                        Dim setDNS As ManagementBaseObject = WmiObj.InvokeMethod("SetDNSServerSearchOrder", newDNS, Nothing)
                    Else
                        Dim ipaddr As Net.IPAddress = Nothing
                        If Net.IPAddress.TryParse(IP.Trim, ipaddr) Then
                            If Mask.Trim = "" Then
                                Mask = "255.255.255.0"
                            End If
                            Dim newIP As ManagementBaseObject = WmiObj.GetMethodParameters("EnableStatic")
                            newIP("IPAddress") = IP.Split(",")
                            newIP("SubnetMask") = New String() {Mask}
                            Dim SetIPAddr As ManagementBaseObject = WmiObj.InvokeMethod("EnableStatic", newIP, Nothing)
                            If Net.IPAddress.TryParse(GateWay.Trim, ipaddr) Then
                                Dim newGate As ManagementBaseObject = WmiObj.GetMethodParameters("SetGateways")
                                newGate("DefaultIPGateway") = New String() {GateWay}
                                newGate("GatewayCostMetric") = New Integer() {1}
                                Dim setGateways As ManagementBaseObject = WmiObj.InvokeMethod("SetGateways", newGate, Nothing)
                            End If
                        End If
                        If Net.IPAddress.TryParse(DNS.Trim, ipaddr) Then
                            Dim newDNS As ManagementBaseObject = WmiObj.GetMethodParameters("SetDNSServerSearchOrder")
                            newDNS("DNSServerSearchOrder") = DNS.Split(",")
                            Dim setDNS As ManagementBaseObject = WmiObj.InvokeMethod("SetDNSServerSearchOrder", newDNS, Nothing)
                        End If
                    End If
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 设置网络连接IP
    ''' </summary>
    ''' <param name="NetName">网络适配器</param>
    ''' <param name="DHCP">DHCP开启</param>
    ''' <param name="IP">IP</param>
    ''' <param name="Mask">掩码</param>
    ''' <param name="GateWay">网关</param>
    ''' <param name="DNS">DNS地址</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetIP(ByVal NetName As String, ByVal DHCP As Boolean, ByVal IP As String, ByVal Mask As String, ByVal GateWay As String, ByVal DNS As String)
        Try
            If DHCP Then
                Shell("netsh interface ip set address name=""" + NetName + """ source=dhcp ")
                Shell("netsh interface ip set dns name=""" + NetName + """ source=dhcp ")
            Else
                If IP.Trim <> "" Then
                    If Mask.Trim = "" Then
                        Mask = "255.255.255.0"
                    End If
                    If GateWay.Trim <> "" Then
                        Shell("netsh interface ip set address name=""" + NetName + """  static " + IP + " " + Mask + " " + GateWay + " 1")
                    Else
                        Shell("netsh interface ip set address name=""" + NetName + """  static " + IP + " " + Mask + " none 0")
                    End If
                End If
                If DNS.Trim <> "" Then
                    Shell("netsh interface ip set dns name=""" + NetName + """ static " + DNS + " primary")
                End If
            End If
            Return True
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function


    ''----------------------------------------------------------------------------------
    ''--------------------------------Native WiFi Functions----------------------------
    ''----------------------------------------------------------------------------------
    Private Const WLAN_API_VERSION_2_0 As Integer = 1 'Windows Vista WiFi API Version
    Private Const ERROR_SUCCESS As Integer = 0

    Private Enum WlanIntfOpcode
        AutoconfEnabled = 1
        BackgroundScanEnabled
        MediaStreamingMode
        RadioState
        BssType
        InterfaceState
        CurrentConnection
        ChannelNumber
        SupportedInfrastructureAuthCipherPairs
        SupportedAdhocAuthCipherPairs
        SupportedCountryOrRegionStringList
        CurrentOperationMode
        Statistics = &H10000101
        RSSI
        SecurityStart = &H20010000
        SecurityEnd = &H2FFFFFFF
        IhvStart = &H30000000
        IhvEnd = &H3FFFFFFF
    End Enum

    Private Enum WLAN_INTERFACE_STATE As Integer
        wlan_interface_state_not_ready = 0
        wlan_interface_state_connected
        wlan_interface_state_ad_hoc_network_formed
        wlan_interface_state_disconnecting
        wlan_interface_state_disconnected
        wlan_interface_state_associating
        wlan_interface_state_discovering
        wlan_interface_state_authenticating
    End Enum

    Private Enum WlanOpcodeValueType
        QueryOnly = 0
        SetByGroupPolicy = 1
        SetByUser = 2
        Invalid = 3
    End Enum

    Private Enum WlanGetAvailableNetworkFlags
        IncludeAllAdhocProfiles = &H1
        IncludeAllManualHiddenProfiles = &H2
    End Enum

    Private Enum Dot11CipherAlgorithm
        None = &H0
        WEP40 = &H1
        TKIP = &H2
        CCMP = &H4
        WEP104 = &H5
        WPA_UseGroup = &H100
        RSN_UseGroup = &H100
        WEP = &H101
        IHV_Start = &H80000000
        IHV_End = &HFFFFFFFF
    End Enum

    Private Enum Dot11BssType
        Infrastructure = 1
        Independent = 2
        Any = 3
    End Enum

    Private Enum Dot11PhyType
        Unknown = 0
        Any = Unknown
        FHSS = 1
        DSSS = 2
        IrBaseband = 3
        OFDM = 4
        HRDSSS = 5
        ERP = 6
        IHV_Start = &H80000000
        IHV_End = &HFFFFFFFF
    End Enum

    Private Enum WlanAvailableNetworkFlags
        Connected = &H1
        HasProfile = &H2
    End Enum

    Private Enum Dot11AuthAlgorithm
        IEEE80211_Open = 1
        IEEE80211_SharedKey = 2
        WPA = 3
        WPA_PSK = 4
        WPA_None = 5
        RSNA = 6
        RSNA_PSK = 7
        IHV_Start = &H80000000
        IHV_End = &HFFFFFFFF
    End Enum

    Private Enum WlanProfileFlags
        AllUser = 0
        GroupPolicy = 1
        User = 2
    End Enum

    Private Enum WlanAccess
        ReadAccess = &H20000 Or &H1
        ExecuteAccess = ReadAccess Or &H20
        WriteAccess = ReadAccess Or ExecuteAccess Or &H2 Or &H10000 Or &H40000
    End Enum

    Public Enum WlanNotificationSource
        None = 0
        All = &HFFFF
        ACM = &H8
        MSM = &H10
        Security = &H20
        IHV = &H40
    End Enum

    Private Enum WlanNotificationCodeAcm
        AutoconfEnabled = 1
        AutoconfDisabled
        BackgroundScanEnabled
        BackgroundScanDisabled
        BssTypeChange
        PowerSettingChange
        ScanComplete
        ScanFail
        ConnectionStart
        ConnectionComplete
        ConnectionAttemptFail
        FilterListChange
        InterfaceArrival
        InterfaceRemoval
        ProfileChange
        ProfileNameChange
        ProfilesExhausted
        NetworkNotAvailable
        NetworkAvailable
        Disconnecting
        Disconnected
        AdhocNetworkStateChange
    End Enum

    Private Enum WlanNotificationCodeMsm
        Associating = 1
        Associated
        Authenticating
        Connected
        RoamingStart
        RoamingEnd
        RadioStateChange
        SignalQualityChange
        Disassociating
        Disconnected
        PeerJoin
        PeerLeave
        AdapterRemoval
        AdapterOperationModeChange
    End Enum

    Private Enum WlanConnectionFlags
        HiddenNetwork = &H1
        AdhocJoinOnly = &H2
        IgnorePrivacyBit = &H4
        EapolPassthrough = &H8
    End Enum

    Private Enum WlanConnectionMode
        Profile = 0
        TemporaryProfile
        DiscoverySecure
        DiscoveryUnsecure
        Auto
        Invalid
    End Enum

    Private Enum WlanAdhocNetworkState
        Formed = 0
        Connected = 1
    End Enum

    Private Enum WlanInterfaceState
        NotReady = 0
        Connected = 1
        AdHocNetworkFormed = 2
        Disconnecting = 3
        Disconnected = 4
        Associating = 5
        Discovering = 6
        Authenticating = 7
    End Enum

    Private Enum Dot11OperationMode
        Unknown = &H0
        Station = &H1
        AP = &H2
        ExtensibleStation = &H4
        NetworkMonitor = &H80000000
    End Enum

    Private Enum WlanReasonCode
        Success = 0
        UNKNOWN = &H10000 + 1
        RANGE_SIZE = &H10000
        BASE = &H10000 + RANGE_SIZE
        AC_BASE = &H10000 + RANGE_SIZE
        AC_CONNECT_BASE = (AC_BASE + RANGE_SIZE / 2)
        AC_END = (AC_BASE + RANGE_SIZE - 1)
        PROFILE_BASE = &H10000 + (7 * RANGE_SIZE)
        PROFILE_CONNECT_BASE = (PROFILE_BASE + RANGE_SIZE / 2)
        PROFILE_END = (PROFILE_BASE + RANGE_SIZE - 1)
        MSM_BASE = &H10000 + (2 * RANGE_SIZE)
        MSM_CONNECT_BASE = (MSM_BASE + RANGE_SIZE / 2)
        MSM_END = (MSM_BASE + RANGE_SIZE - 1)
        MSMSEC_BASE = &H10000 + (3 * RANGE_SIZE)
        MSMSEC_CONNECT_BASE = (MSMSEC_BASE + RANGE_SIZE / 2)
        MSMSEC_END = (MSMSEC_BASE + RANGE_SIZE - 1)
        NETWORK_NOT_COMPATIBLE = (AC_BASE + 1)
        PROFILE_NOT_COMPATIBLE = (AC_BASE + 2)
        NO_AUTO_CONNECTION = (AC_CONNECT_BASE + 1)
        NOT_VISIBLE = (AC_CONNECT_BASE + 2)
        GP_DENIED = (AC_CONNECT_BASE + 3)
        USER_DENIED = (AC_CONNECT_BASE + 4)
        BSS_TYPE_NOT_ALLOWED = (AC_CONNECT_BASE + 5)
        IN_FAILED_LIST = (AC_CONNECT_BASE + 6)
        IN_BLOCKED_LIST = (AC_CONNECT_BASE + 7)
        SSID_LIST_TOO_LONG = (AC_CONNECT_BASE + 8)
        CONNECT_CALL_FAIL = (AC_CONNECT_BASE + 9)
        SCAN_CALL_FAIL = (AC_CONNECT_BASE + 10)
        NETWORK_NOT_AVAILABLE = (AC_CONNECT_BASE + 11)
        PROFILE_CHANGED_OR_DELETED = (AC_CONNECT_BASE + 12)
        KEY_MISMATCH = (AC_CONNECT_BASE + 13)
        USER_NOT_RESPOND = (AC_CONNECT_BASE + 14)
        INVALID_PROFILE_SCHEMA = (PROFILE_BASE + 1)
        PROFILE_MISSING = (PROFILE_BASE + 2)
        INVALID_PROFILE_NAME = (PROFILE_BASE + 3)
        INVALID_PROFILE_TYPE = (PROFILE_BASE + 4)
        INVALID_PHY_TYPE = (PROFILE_BASE + 5)
        MSM_SECURITY_MISSING = (PROFILE_BASE + 6)
        IHV_SECURITY_NOT_SUPPORTED = (PROFILE_BASE + 7)
        IHV_OUI_MISMATCH = (PROFILE_BASE + 8)
        IHV_OUI_MISSING = (PROFILE_BASE + 9)
        IHV_SETTINGS_MISSING = (PROFILE_BASE + 10)
        CONFLICT_SECURITY = (PROFILE_BASE + 11)
        SECURITY_MISSING = (PROFILE_BASE + 12)
        INVALID_BSS_TYPE = (PROFILE_BASE + 13)
        INVALID_ADHOC_CONNECTION_MODE = (PROFILE_BASE + 14)
        NON_BROADCAST_SET_FOR_ADHOC = (PROFILE_BASE + 15)
        AUTO_SWITCH_SET_FOR_ADHOC = (PROFILE_BASE + 16)
        AUTO_SWITCH_SET_FOR_MANUAL_CONNECTION = (PROFILE_BASE + 17)
        IHV_SECURITY_ONEX_MISSING = (PROFILE_BASE + 18)
        PROFILE_SSID_INVALID = (PROFILE_BASE + 19)
        TOO_MANY_SSID = (PROFILE_BASE + 20)
        UNSUPPORTED_SECURITY_SET_BY_OS = (MSM_BASE + 1)
        UNSUPPORTED_SECURITY_SET = (MSM_BASE + 2)
        BSS_TYPE_UNMATCH = (MSM_BASE + 3)
        PHY_TYPE_UNMATCH = (MSM_BASE + 4)
        DATARATE_UNMATCH = (MSM_BASE + 5)
        USER_CANCELLED = (MSM_CONNECT_BASE + 1)
        ASSOCIATION_FAILURE = (MSM_CONNECT_BASE + 2)
        ASSOCIATION_TIMEOUT = (MSM_CONNECT_BASE + 3)
        PRE_SECURITY_FAILURE = (MSM_CONNECT_BASE + 4)
        START_SECURITY_FAILURE = (MSM_CONNECT_BASE + 5)
        SECURITY_FAILURE = (MSM_CONNECT_BASE + 6)
        SECURITY_TIMEOUT = (MSM_CONNECT_BASE + 7)
        ROAMING_FAILURE = (MSM_CONNECT_BASE + 8)
        ROAMING_SECURITY_FAILURE = (MSM_CONNECT_BASE + 9)
        ADHOC_SECURITY_FAILURE = (MSM_CONNECT_BASE + 10)
        DRIVER_DISCONNECTED = (MSM_CONNECT_BASE + 11)
        DRIVER_OPERATION_FAILURE = (MSM_CONNECT_BASE + 12)
        IHV_NOT_AVAILABLE = (MSM_CONNECT_BASE + 13)
        IHV_NOT_RESPONDING = (MSM_CONNECT_BASE + 14)
        DISCONNECT_TIMEOUT = (MSM_CONNECT_BASE + 15)
        INTERNAL_FAILURE = (MSM_CONNECT_BASE + 16)
        UI_REQUEST_TIMEOUT = (MSM_CONNECT_BASE + 17)
        TOO_MANY_SECURITY_ATTEMPTS = (MSM_CONNECT_BASE + 18)
        MSMSEC_MIN = MSMSEC_BASE
        MSMSEC_PROFILE_INVALID_KEY_INDEX = (MSMSEC_BASE + 1)
        MSMSEC_PROFILE_PSK_PRESENT = (MSMSEC_BASE + 2)
        MSMSEC_PROFILE_KEY_LENGTH = (MSMSEC_BASE + 3)
        MSMSEC_PROFILE_PSK_LENGTH = (MSMSEC_BASE + 4)
        MSMSEC_PROFILE_NO_AUTH_CIPHER_SPECIFIED = (MSMSEC_BASE + 5)
        MSMSEC_PROFILE_TOO_MANY_AUTH_CIPHER_SPECIFIED = (MSMSEC_BASE + 6)
        MSMSEC_PROFILE_DUPLICATE_AUTH_CIPHER = (MSMSEC_BASE + 7)
        MSMSEC_PROFILE_RAWDATA_INVALID = (MSMSEC_BASE + 8)
        MSMSEC_PROFILE_INVALID_AUTH_CIPHER = (MSMSEC_BASE + 9)
        MSMSEC_PROFILE_ONEX_DISABLED = (MSMSEC_BASE + 10)
        MSMSEC_PROFILE_ONEX_ENABLED = (MSMSEC_BASE + 11)
        MSMSEC_PROFILE_INVALID_PMKCACHE_MODE = (MSMSEC_BASE + 12)
        MSMSEC_PROFILE_INVALID_PMKCACHE_SIZE = (MSMSEC_BASE + 13)
        MSMSEC_PROFILE_INVALID_PMKCACHE_TTL = (MSMSEC_BASE + 14)
        MSMSEC_PROFILE_INVALID_PREAUTH_MODE = (MSMSEC_BASE + 15)
        MSMSEC_PROFILE_INVALID_PREAUTH_THROTTLE = (MSMSEC_BASE + 16)
        MSMSEC_PROFILE_PREAUTH_ONLY_ENABLED = (MSMSEC_BASE + 17)
        MSMSEC_CAPABILITY_NETWORK = (MSMSEC_BASE + 18)
        MSMSEC_CAPABILITY_NIC = (MSMSEC_BASE + 19)
        MSMSEC_CAPABILITY_PROFILE = (MSMSEC_BASE + 20)
        MSMSEC_CAPABILITY_DISCOVERY = (MSMSEC_BASE + 21)
        MSMSEC_PROFILE_PASSPHRASE_CHAR = (MSMSEC_BASE + 22)
        MSMSEC_PROFILE_KEYMATERIAL_CHAR = (MSMSEC_BASE + 23)
        MSMSEC_PROFILE_WRONG_KEYTYPE = (MSMSEC_BASE + 24)
        MSMSEC_MIXED_CELL = (MSMSEC_BASE + 25)
        MSMSEC_PROFILE_AUTH_TIMERS_INVALID = (MSMSEC_BASE + 26)
        MSMSEC_PROFILE_INVALID_GKEY_INTV = (MSMSEC_BASE + 27)
        MSMSEC_TRANSITION_NETWORK = (MSMSEC_BASE + 28)
        MSMSEC_PROFILE_KEY_UNMAPPED_CHAR = (MSMSEC_BASE + 29)
        MSMSEC_CAPABILITY_PROFILE_AUTH = (MSMSEC_BASE + 30)
        MSMSEC_CAPABILITY_PROFILE_CIPHER = (MSMSEC_BASE + 31)
        MSMSEC_UI_REQUEST_FAILURE = (MSMSEC_CONNECT_BASE + 1)
        MSMSEC_AUTH_START_TIMEOUT = (MSMSEC_CONNECT_BASE + 2)
        MSMSEC_AUTH_SUCCESS_TIMEOUT = (MSMSEC_CONNECT_BASE + 3)
        MSMSEC_KEY_START_TIMEOUT = (MSMSEC_CONNECT_BASE + 4)
        MSMSEC_KEY_SUCCESS_TIMEOUT = (MSMSEC_CONNECT_BASE + 5)
        MSMSEC_M3_MISSING_KEY_DATA = (MSMSEC_CONNECT_BASE + 6)
        MSMSEC_M3_MISSING_IE = (MSMSEC_CONNECT_BASE + 7)
        MSMSEC_M3_MISSING_GRP_KEY = (MSMSEC_CONNECT_BASE + 8)
        MSMSEC_PR_IE_MATCHING = (MSMSEC_CONNECT_BASE + 9)
        MSMSEC_SEC_IE_MATCHING = (MSMSEC_CONNECT_BASE + 10)
        MSMSEC_NO_PAIRWISE_KEY = (MSMSEC_CONNECT_BASE + 11)
        MSMSEC_G1_MISSING_KEY_DATA = (MSMSEC_CONNECT_BASE + 12)
        MSMSEC_G1_MISSING_GRP_KEY = (MSMSEC_CONNECT_BASE + 13)
        MSMSEC_PEER_INDICATED_INSECURE = (MSMSEC_CONNECT_BASE + 14)
        MSMSEC_NO_AUTHENTICATOR = (MSMSEC_CONNECT_BASE + 15)
        MSMSEC_NIC_FAILURE = (MSMSEC_CONNECT_BASE + 16)
        MSMSEC_CANCELLED = (MSMSEC_CONNECT_BASE + 17)
        MSMSEC_KEY_FORMAT = (MSMSEC_CONNECT_BASE + 18)
        MSMSEC_DOWNGRADE_DETECTED = (MSMSEC_CONNECT_BASE + 19)
        MSMSEC_PSK_MISMATCH_SUSPECTED = (MSMSEC_CONNECT_BASE + 20)
        MSMSEC_FORCED_FAILURE = (MSMSEC_CONNECT_BASE + 21)
        MSMSEC_SECURITY_UI_FAILURE = (MSMSEC_CONNECT_BASE + 22)
        MSMSEC_MAX = MSMSEC_END
    End Enum

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure WlanConnectionAttributes
        Public isState As WlanInterfaceState
        Public wlanConnectionMode As WlanConnectionMode
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
        Public profileName As String
        Public wlanAssociationAttributes As WlanAssociationAttributes
        Public wlanSecurityAttributes As WlanSecurityAttributes
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure WlanInterfaceInfo
        Public interfaceGuid As Guid
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=512)>
        Public interfaceDescription As String
        Public isState As WlanInterfaceState
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanInterfaceInfoListHeader
        Public numberOfItems As UInteger
        Public index As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanProfileInfoListHeader
        Public numberOfItems As UInteger
        Public index As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure WlanProfileInfo
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
        Public profileName As String
        Public profileFlags As WlanProfileFlags
    End Structure


    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanSecurityAttributes
        <MarshalAs(UnmanagedType.Bool)>
        Public securityEnabled As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public oneXEnabled As Boolean
        Public dot11AuthAlgorithm As Dot11AuthAlgorithm
        Public dot11CipherAlgorithm As Dot11CipherAlgorithm
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanAssociationAttributes
        Public dot11Ssid As Dot11Ssid
        Public dot11BssType As Dot11BssType
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)>
        Public dot11Bssid As Byte()
        Public dot11PhyType As Dot11PhyType
        Public dot11PhyIndex As UInteger
        Public wlanSignalQuality As UInteger
        Public rxRate As UInteger
        Public txRate As UInteger
        Public ReadOnly Property ssid As PhysicalAddress
            Get
                Return New PhysicalAddress(dot11Bssid)
            End Get
        End Property
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanBssListHeader
        Public totalSize As UInteger
        Public numberOfItems As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanConnectionParameters
        Public wlanConnectionMode As WlanConnectionMode
        <MarshalAs(UnmanagedType.LPWStr)>
        Public profile As String
        Public dot11SsidPtr As IntPtr
        Public desiredBssidListPtr As IntPtr
        Public dot11BssType As Dot11BssType
        Public flags As WlanConnectionFlags
    End Structure


    <StructLayout(LayoutKind.Sequential)>
    Public Structure WlanNotificationData
        Public notificationSource As WlanNotificationSource
        Public notificationCode As Integer
        Public interfaceGuid As Guid
        Public dataSize As Integer
        Public dataPtr As IntPtr
        Public ReadOnly Property NotificationObj As Object
            Get
                If (notificationSource = WlanNotificationSource.MSM) Then
                    Return CType(notificationCode, WlanNotificationCodeMsm)
                ElseIf (notificationSource = WlanNotificationSource.ACM) Then
                    Return CType(notificationCode, WlanNotificationCodeAcm)
                Else
                    Return notificationCode
                End If
            End Get
        End Property
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanAvailableNetworkListHeader
        Public numberOfItems As UInteger
        Public index As UInteger
    End Structure

    Private Structure Dot11Ssid
        Public SSIDLength As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)>
        Public SSID As Byte()
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanRateSet
        Private rateSetLength As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=126)>
        Private rateSet As UShort()
        Public ReadOnly Property Rates As UShort()
            Get
                Dim rate(rateSetLength / Marshal.SizeOf(GetType(UShort))) As UShort
                Array.Copy(rateSet, rate, rate.Length)
                Return rate
            End Get
        End Property
        Public Function GetRateInMbps(ByVal rate As Integer) As Double
            Return (rateSet(rate) And &H7FFF) * 0.5
        End Function
    End Structure

    Public Structure SSID_Info
        Public SSID As String
        Public MAC As String
        Public linkQuality As UInteger
        Public Auth As WIFIauthentication
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WlanBssEntry
        Public dot11Ssid As Dot11Ssid
        Public phyId As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)>
        Public dot11Bssid As Byte()
        Public dot11BssType As Dot11BssType
        Public dot11BssPhyType As Dot11PhyType
        Public rssi As Integer
        Public linkQuality As UInteger
        Public inRegDomain As Boolean
        Public beaconPeriod As UShort
        Public timestamp As ULong
        Public hostTimestamp As ULong
        Public capabilityInformation As UShort
        Public chCenterFrequency As UInteger
        Public wlanRateSet As WlanRateSet
        Public ieOffset As UInteger
        Public ieSize As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure WLAN_INTERFACE_INFO
        Public InterfaceGuid As Guid
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
        Public strInterfaceDescription As String
        Public isState As WLAN_INTERFACE_STATE
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WLAN_INTERFACE_INFO_LIST
        Public dwNumberofItems As Integer
        Public dwIndex As Integer
        Public InterfaceInfo() As WLAN_INTERFACE_INFO
        Private _ppInterfaceList As IntPtr
        Sub New(ByVal pList As IntPtr)
            dwNumberofItems = Marshal.ReadInt32(pList, 0)
            dwIndex = Marshal.ReadInt32(pList, 4)
            ReDim InterfaceInfo(dwNumberofItems)
            For i As Integer = 0 To dwNumberofItems
                Dim pItemList As IntPtr = New IntPtr(pList.ToInt32() + (i * 532) + 8)
                Dim wii As WLAN_INTERFACE_INFO = New WLAN_INTERFACE_INFO()
                wii = CType(Marshal.PtrToStructure(pItemList, GetType(WLAN_INTERFACE_INFO)), WLAN_INTERFACE_INFO)
                InterfaceInfo(i) = wii
            Next
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure WlanAvailableNetwork
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
        Public profileName As String
        Public dot11Ssid As Dot11Ssid
        Public dot11BssType As Dot11BssType
        Public numberOfBssids As UInteger
        Public networkConnectablea As Boolean
        Public wlanNotConnectableReason As WlanReasonCode
        Private numberOfPhyTypes As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
        Private dot11PhyType As Dot11PhyType()
        Public ReadOnly Property Dot11PhyTypes As Dot11PhyType()
            Get
                Dim ret(numberOfPhyTypes) As Dot11PhyType
                Array.Copy(dot11PhyType, ret, numberOfPhyTypes)
                Return ret
            End Get
        End Property
        Public morePhyTypes As Boolean
        Public wlanSignalQuality As UInteger
        Public securityEnabled As Boolean
        Public dot11DefaultAuthAlgorithm As Dot11AuthAlgorithm
        Public dot11DefaultCipherAlgorithm As Dot11CipherAlgorithm
        Public flags As WlanAvailableNetworkFlags
        Public reserved As UInteger
    End Structure


    Private Declare Function WlanOpenHandle Lib "wlanapi" (ByVal dwClientVersion As UInteger, ByVal pReserved As IntPtr, ByRef pdwNegotiatedVersion As UInteger, ByRef phClientHandle As IntPtr) As UInteger
    Private Declare Function WlanCloseHandle Lib "wlanapi" (ByVal hClientHandle As IntPtr, ByVal pReserved As IntPtr) As UInteger
    Private Declare Function WlanEnumInterfaces Lib "wlanapi" (ByVal hClientHandle As IntPtr, ByVal pReserved As IntPtr, ByRef ppInterfaceList As IntPtr) As UInteger
    Private Declare Function WlanQueryInterface Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal opCode As WlanIntfOpcode, ByRef pReserved As IntPtr, ByRef dataSize As Integer, ByRef ppData As IntPtr, ByRef wlanOpcodeValueType As WlanOpcodeValueType) As Integer
    Private Declare Function WlanSetInterface Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal opCode As WlanIntfOpcode, ByVal dataSize As UInteger, ByVal pData As IntPtr, ByVal pReserved As IntPtr) As Integer
    Private Declare Function WlanScan Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal pDot11Ssid As IntPtr, ByVal pIeData As IntPtr, ByRef pReserved As IntPtr) As Integer
    Private Declare Function WlanGetAvailableNetworkList Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByRef interfaceGuid As Guid, ByVal flags As WlanAvailableNetworkFlags, ByVal reservedPtr As IntPtr, ByRef availableNetworkListPtr As IntPtr)
    Private Declare Function WlanGetProfile Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, <MarshalAs(UnmanagedType.LPWStr)> ByVal profileName As String, ByVal pReserved As IntPtr, ByRef profileXml As IntPtr, ByRef flags As WlanProfileFlags, Optional ByRef grantedAccess As WlanAccess = WlanAccess.ExecuteAccess) As Integer
    Private Declare Function WlanGetProfileList Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal pReserved As IntPtr, ByRef profileList As IntPtr) As Integer
    Private Declare Function WlanReasonCodeToString Lib "wlanapi" (ByVal reasonCode As WlanReasonCode, ByVal bufferSize As Integer, ByRef stringBuffer As Text.StringBuilder, ByVal pReserved As IntPtr) As Integer
    Private Declare Function WlanRegisterNotification Lib "wlanapi" (ByVal clientHandle As IntPtr, ByVal notifSource As WlanNotificationSource, ByVal ignoreDuplicate As Boolean, ByVal funcCallback As WlanNotificationCallbackDelegate, ByVal callbackContext As IntPtr, ByVal reserved As IntPtr, ByRef prevNotifSource As WlanNotificationSource) As Integer
    Private Declare Function WlanGetNetworkBssList Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal dot11SsidInt As IntPtr, ByVal dot11BssType As Dot11BssType, ByVal securityEnabled As Boolean, ByVal reservedPtr As IntPtr, ByRef wlanBssList As IntPtr) As Integer
    Private Declare Function WlanConnect Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByRef connectionParameters As WlanConnectionParameters, ByVal pReserved As IntPtr) As Integer
    Private Declare Function WlanDisconnect Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal pReserved As IntPtr) As Integer

    Private Declare Function WlanDeleteProfile Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, <MarshalAs(UnmanagedType.LPWStr)> ByVal profileName As String, ByVal pReserved As IntPtr) As Integer
    Private Declare Function WlanSetProfile Lib "wlanapi" (ByVal clientHandle As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceGuid As Guid, ByVal flags As WlanProfileFlags, <MarshalAs(UnmanagedType.LPStr)> ByVal profileXml As String, <MarshalAs(UnmanagedType.LPWStr)> ByVal allUserProfileSecurity As String, ByVal overwrite As Boolean, ByVal pReserved As IntPtr, ByRef reasonCode As WlanReasonCode) As Integer
    Private Declare Sub WlanFreeMemory Lib "wlanapi" (ByVal pmemory As IntPtr)


    Public Delegate Sub WlanNotificationCallbackDelegate(ByRef notificationData As WlanNotificationData, ByVal context As IntPtr)

    Private Shared Function GetWlanInterfaceInfo(ByVal name As String, ByRef Handle As IntPtr, ByRef wlaninfo As WlanInterfaceInfo) As Boolean
        Try
            Dim negotiatedVersion As UInteger = 0
            Dim ifaceList As IntPtr = IntPtr.Zero
            Dim res As Integer = WlanOpenHandle(WLAN_API_VERSION_2_0, IntPtr.Zero, negotiatedVersion, Handle)
            If res = 0 Then
                WlanEnumInterfaces(Handle, IntPtr.Zero, ifaceList)
                If res = 0 Then
                    Dim header As WlanInterfaceInfoListHeader = CType(Marshal.PtrToStructure(ifaceList, GetType(WlanInterfaceInfoListHeader)), WlanInterfaceInfoListHeader)
                    Dim listIterator As Int64 = ifaceList.ToInt64() + Marshal.SizeOf(header)
                    For i As Integer = 0 To header.numberOfItems - 1
                        Dim info As WlanInterfaceInfo = CType(Marshal.PtrToStructure(New IntPtr(listIterator), GetType(WlanInterfaceInfo)), WlanInterfaceInfo)
                        listIterator += Marshal.SizeOf(info)
                        If info.interfaceDescription.Trim <> "" And name.Trim <> "" And info.interfaceDescription.ToLower.Contains(name.ToLower) Then
                            wlaninfo = info
                            Return True
                        End If
                    Next
                    Return False
                Else
                    ErrMsg = res.ToString
                    Handle = IntPtr.Zero
                    Return False
                End If
            Else
                ErrMsg = res.ToString
                Handle = IntPtr.Zero
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Handle = IntPtr.Zero
            Return False
        End Try
    End Function

    Private Shared Sub CloseWLAN(ByVal hwd As IntPtr)
        Try
            WlanCloseHandle(hwd, IntPtr.Zero)
        Catch ex As Exception
            ErrMsg = ex.Message
        End Try
    End Sub

    Private Shared Function ProfilesIsExist(ByVal hwd As IntPtr, ByVal info As WlanInterfaceInfo, ByVal NetName As String, ByRef SSID As String) As Boolean
        Try
            Dim profileListPtr As IntPtr = IntPtr.Zero
            If WlanGetProfileList(hwd, info.interfaceGuid, IntPtr.Zero, profileListPtr) = 0 Then
                Dim header As WlanProfileInfoListHeader = CType(Marshal.PtrToStructure(profileListPtr, GetType(WlanProfileInfoListHeader)), WlanProfileInfoListHeader)
                Dim profileListIterator As Long = profileListPtr.ToInt64() + Marshal.SizeOf(GetType(WlanProfileInfoListHeader))
                Dim res As Boolean = False
                For i As Integer = 0 To header.numberOfItems - 1
                    Dim profileInfo As WlanProfileInfo = CType(Marshal.PtrToStructure(New IntPtr(profileListIterator), GetType(WlanProfileInfo)), WlanProfileInfo)
                    profileListIterator += Marshal.SizeOf(profileInfo)
                    Dim ssidstr As String = profileInfo.profileName
                    If ssidstr.ToUpper.Trim.Contains(SSID.ToUpper.Trim) Then
                        SSID = ssidstr
                        res = True
                        Exit For
                    End If
                Next
                WlanFreeMemory(profileListPtr)
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
    ''' 获取所有WIFI适配器名称
    ''' </summary>
    ''' <returns>WIFI适配器</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property WIFIAdapter As String()
        Get
            Try
                Dim pack As List(Of String) = New List(Of String)
                Dim Handle As IntPtr
                Dim negotiatedVersion As UInteger = 0
                Dim ifaceList As IntPtr = IntPtr.Zero
                Dim res As Integer = WlanOpenHandle(WLAN_API_VERSION_2_0, IntPtr.Zero, negotiatedVersion, Handle)
                If res = 0 Then
                    WlanEnumInterfaces(Handle, IntPtr.Zero, ifaceList)
                    If res = 0 Then
                        Dim header As WlanInterfaceInfoListHeader = CType(Marshal.PtrToStructure(ifaceList, GetType(WlanInterfaceInfoListHeader)), WlanInterfaceInfoListHeader)
                        Dim listIterator As Int64 = ifaceList.ToInt64() + Marshal.SizeOf(header)
                        For i As Integer = 0 To header.numberOfItems - 1
                            Dim info As WlanInterfaceInfo = CType(Marshal.PtrToStructure(New IntPtr(listIterator), GetType(WlanInterfaceInfo)), WlanInterfaceInfo)
                            listIterator += Marshal.SizeOf(info)
                            If info.interfaceDescription.Trim <> "" Then
                                pack.Add(info.interfaceDescription.Trim)
                            End If
                        Next
                        If pack.Count > 0 Then
                            Dim ress(pack.Count - 1) As String
                            pack.CopyTo(ress)
                            CloseWLAN(Handle)
                            Return ress
                        Else
                            CloseWLAN(Handle)
                            Return Nothing
                        End If
                    Else
                        CloseWLAN(Handle)
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

    ''' <summary>
    ''' 配置是否自动设置
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <param name="SW">是否</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function SetAutoCfg(ByVal NetName As String, ByVal SW As Boolean) As Boolean
        Try
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                Dim opCode As WlanIntfOpcode = WlanIntfOpcode.AutoconfEnabled
                Dim valuePtr As IntPtr = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(GetType(Integer)))
                Dim valueSize As Integer = 0
                If SW Then
                    Marshal.WriteInt32(valuePtr, 1)
                Else
                    Marshal.WriteInt32(valuePtr, 0)
                End If
                If WlanSetInterface(hwd, info.interfaceGuid, opCode, Marshal.SizeOf(GetType(Integer)), valuePtr, IntPtr.Zero) = 0 Then
                    WlanFreeMemory(valuePtr)
                    Return True
                Else
                    WlanFreeMemory(valuePtr)
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
    ''' 获取是否自动配置
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <returns>是否</returns>
    ''' <remarks></remarks>
    Public Shared Function GetAutoCfg(ByVal NetName As String) As Boolean
        Try
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                Dim opCode As WlanIntfOpcode = WlanIntfOpcode.AutoconfEnabled
                Dim valuePtr As IntPtr = IntPtr.Zero
                Dim valueSize As Integer = 0
                Dim opcodeValueType As WlanOpcodeValueType
                If WlanQueryInterface(hwd, info.interfaceGuid, opCode, IntPtr.Zero, valueSize, valuePtr, opcodeValueType) = 0 Then
                    Dim res As Integer = Marshal.ReadInt32(valuePtr)
                    WlanFreeMemory(valuePtr)
                    Return res
                Else
                    WlanFreeMemory(valuePtr)
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

    Private Shared Function AvailableSSID(ByVal NetName As String) As SSID_Info()
        Try
            Dim flags As WlanGetAvailableNetworkFlags = WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles
            Dim pack As List(Of SSID_Info) = New List(Of SSID_Info)
            Dim chk As List(Of String) = New List(Of String)
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                WlanScan(hwd, info.interfaceGuid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
                Dim availNetListPtr As IntPtr = IntPtr.Zero
                If WlanGetAvailableNetworkList(hwd, info.interfaceGuid, flags, Nothing, availNetListPtr) = 0 Then
                    Dim availNetListHeader As WlanAvailableNetworkListHeader = CType(Marshal.PtrToStructure(availNetListPtr, GetType(WlanAvailableNetworkListHeader)), WlanAvailableNetworkListHeader)
                    Dim availNetListIt As Long = availNetListPtr.ToInt64() + Marshal.SizeOf(GetType(WlanAvailableNetworkListHeader))
                    For i As Integer = 0 To availNetListHeader.numberOfItems - 1
                        Dim ssid As WlanAvailableNetwork = CType(Marshal.PtrToStructure(New IntPtr(availNetListIt), GetType(WlanAvailableNetwork)), WlanAvailableNetwork)
                        availNetListIt += Marshal.SizeOf(GetType(WlanAvailableNetwork))
                        If ssid.dot11Ssid.SSIDLength > 0 Then
                            Dim ssidstr As String = Encoding.ASCII.GetString(ssid.dot11Ssid.SSID, 0, ssid.dot11Ssid.SSIDLength)
                            If Not chk.Contains(ssidstr) Then
                                chk.Add(ssidstr)
                                Dim ssidis As SSID_Info = New SSID_Info
                                ssidis.SSID = ssidstr
                                ssidis.linkQuality = ssid.wlanSignalQuality
                            End If
                        End If
                    Next
                    WlanFreeMemory(availNetListPtr)
                    If pack.Count > 0 Then
                        Dim res(pack.Count - 1) As SSID_Info
                        pack.CopyTo(res)
                        Return res
                    Else
                        Return Nothing
                    End If
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取搜索的SSID
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <returns>SSID信息</returns>
    ''' <remarks></remarks>
    Public Shared Function SSID(ByVal NetName As String) As SSID_Info()
        Try
            Dim pack As List(Of SSID_Info) = New List(Of SSID_Info)
            Dim chk As List(Of String) = New List(Of String)
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                WlanScan(hwd, info.interfaceGuid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
                Dim bssListPtr As IntPtr = IntPtr.Zero
                If WlanGetNetworkBssList(hwd, info.interfaceGuid, IntPtr.Zero, Dot11BssType.Any, False, IntPtr.Zero, bssListPtr) = 0 Then
                    Dim bssListHeader As WlanBssListHeader = CType(Marshal.PtrToStructure(bssListPtr, GetType(WlanBssListHeader)), WlanBssListHeader)
                    Dim bssListIt As Long = bssListPtr.ToInt64() + Marshal.SizeOf(GetType(WlanBssListHeader))
                    For i As Integer = 0 To bssListHeader.numberOfItems - 1
                        Dim bssEntries As WlanBssEntry = CType(Marshal.PtrToStructure(New IntPtr(bssListIt), GetType(WlanBssEntry)), WlanBssEntry)
                        bssListIt += Marshal.SizeOf(GetType(WlanBssEntry))
                        Try
                            If bssEntries.dot11Ssid.SSIDLength > 0 Then
                                Dim ssidstr As String = Encoding.ASCII.GetString(bssEntries.dot11Ssid.SSID, 0, bssEntries.dot11Ssid.SSIDLength)
                                If Not chk.Contains(ssidstr) Then
                                    chk.Add(ssidstr)
                                    Dim ssidis As SSID_Info = New SSID_Info
                                    ssidis.SSID = ssidstr
                                    ssidis.linkQuality = bssEntries.linkQuality
                                    Dim temp As String = ""
                                    For j As Integer = 0 To bssEntries.dot11Bssid.Length - 1
                                        If j = 0 Then
                                            temp += "" + bssEntries.dot11Bssid(j).ToString("X2")
                                        Else
                                            temp += ":" + bssEntries.dot11Bssid(j).ToString("X2")
                                        End If
                                    Next
                                    ssidis.MAC = temp
                                    pack.Add(ssidis)
                                End If
                            End If
                        Catch

                        End Try
                    Next
                End If
                WlanFreeMemory(bssListPtr)
                If pack.Count > 0 Then
                    Dim res(pack.Count - 1) As SSID_Info
                    pack.CopyTo(res)
                    CloseWLAN(hwd)
                    Return res
                Else
                    CloseWLAN(hwd)
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 已配置SSID连接配置
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <returns>SSID连接配置</returns>
    ''' <remarks></remarks>
    Public Shared Function SSIDProfiles(ByVal NetName As String) As String()
        Try
            Dim pack As List(Of String) = New List(Of String)
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                Dim profileListPtr As IntPtr = IntPtr.Zero
                If WlanGetProfileList(hwd, info.interfaceGuid, IntPtr.Zero, profileListPtr) = 0 Then
                    Dim header As WlanProfileInfoListHeader = CType(Marshal.PtrToStructure(profileListPtr, GetType(WlanProfileInfoListHeader)), WlanProfileInfoListHeader)
                    Dim profileListIterator As Long = profileListPtr.ToInt64() + Marshal.SizeOf(GetType(WlanProfileInfoListHeader))
                    For i As Integer = 0 To header.numberOfItems - 1
                        Dim profileInfo As WlanProfileInfo = CType(Marshal.PtrToStructure(New IntPtr(profileListIterator), GetType(WlanProfileInfo)), WlanProfileInfo)
                        profileListIterator += Marshal.SizeOf(profileInfo)
                        Dim ssidstr As String = profileInfo.profileName
                        If Not pack.Contains(ssidstr) Then
                            pack.Add(ssidstr)
                        End If
                    Next
                    WlanFreeMemory(profileListPtr)
                    If pack.Count > 0 Then
                        Dim res(pack.Count - 1) As String
                        pack.CopyTo(res)
                        CloseWLAN(hwd)
                        Return res
                    Else
                        CloseWLAN(hwd)
                        Return Nothing
                    End If
                Else
                    CloseWLAN(hwd)
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 连接SSID
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <param name="SSID">要连接的SSID</param>
    ''' <param name="Author">认证模式</param>
    ''' <param name="PassWord">密码</param>
    ''' <param name="Path">临时保存位置</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function ConnectWIFI(ByVal NetName As String, ByVal SSID As String, ByVal Author As WIFIauthentication, ByVal PassWord As String, Optional ByVal Path As String = "") As Boolean
        Try
            Dim mode As WlanConnectionMode = WlanConnectionMode.Profile
            Dim BssType As Dot11BssType = Dot11BssType.Any
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            Dim res As Integer = 0
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                    Dim ssidhex As String = ""
                    Dim buf() As Byte = System.Text.Encoding.ASCII.GetBytes(SSID.ToUpper)
                    For i As Integer = 0 To buf.Length - 1
                        ssidhex += buf(i).ToString("X2")
                    Next
                    Dim authentication As String = ""
                    Dim encryption As String = ""
                    Dim keytype As String = ""
                    Select Case Author
                        Case WIFIauthentication.WEP_Open
                            authentication = "open"
                            encryption = "WEP"
                            keytype = "networkKey"
                        Case WIFIauthentication.WEP_Share
                            authentication = "shared"
                            encryption = "WEP"
                            keytype = "networkKey"
                        Case WIFIauthentication.WPA_TKIP
                            authentication = "WPAPSK"
                            encryption = "TKIP"
                            keytype = "passPhrase"
                        Case WIFIauthentication.WPA_AES
                            authentication = "WPAPSK"
                            encryption = "AES"
                            keytype = "passPhrase"
                        Case WIFIauthentication.WPS2_TKIP
                            authentication = "WPA2PSK"
                            encryption = "TKIP"
                            keytype = "passPhrase"
                        Case WIFIauthentication.WPS2_AES
                            authentication = "WPA2PSK"
                            encryption = "AES"
                            keytype = "passPhrase"
                        Case Else
                            authentication = "open"
                            encryption = "none"
                            keytype = "networkKey"
                    End Select
                    Dim reasonCode As WlanReasonCode
                    Dim flags As WlanProfileFlags = WlanProfileFlags.AllUser
                    Dim profilexml As String = ""
                    profilexml = "<?xml version=""1.0"" ?>" & vbCrLf & _
                                 "<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">" & vbCrLf & _
                                 "   <name>" & SSID.ToUpper & "</name>" & vbCrLf & _
                                 "   <SSIDConfig>" & vbCrLf & _
                                 "       <SSID> " & vbCrLf & _
                                 "          <hex>" & ssidhex & "</hex> " & vbCrLf & _
                                 "          <name>" & SSID.ToUpper & "</name> " & vbCrLf & _
                                 "       </SSID> " & vbCrLf & _
                                 "       <nonBroadcast>false</nonBroadcast>" & vbCrLf & _
                                 "   </SSIDConfig>" & vbCrLf & _
                                 "   <connectionType>ESS</connectionType>" & vbCrLf & _
                                 "   <connectionMode>auto</connectionMode>" & vbCrLf & _
                                 "   <autoSwitch>false</autoSwitch>" & vbCrLf & _
                                 "   <MSM>" & vbCrLf & _
                                 "     <security>" & vbCrLf & _
                                 "       <authEncryption>" & vbCrLf & _
                                 "          <authentication>" & authentication & "</authentication>" & vbCrLf & _
                                 "          <encryption>" & encryption & "</encryption>" & vbCrLf & _
                                 "          <useOneX>false</useOneX>" & vbCrLf & _
                                 "       </authEncryption>" & vbCrLf & _
                                 "       <sharedKey>" & vbCrLf & _
                                 "          <keyType>" & keytype & "</keyType>" & vbCrLf & _
                                 "          <protected>false</protected>" & vbCrLf & _
                                 "          <keyMaterial>" & PassWord & "</keyMaterial>" & vbCrLf & _
                                 "       </sharedKey>" & vbCrLf & _
                                 "     </security>" & vbCrLf & _
                                 "    </MSM>" & vbCrLf & _
                                 "</WLANProfile>"
                    res = WlanSetProfile(hwd, info.interfaceGuid, flags, profilexml, Nothing, True, IntPtr.Zero, reasonCode)
                    If res = 0 Then
                        If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                            ErrMsg = "创建配置文件失败"
                            CloseWLAN(hwd)
                            Return False
                        End If
                    ElseIf res = 1206 Then
                        Dim x86 As System.OperatingSystem = System.Environment.OSVersion
                        If x86.Version.Major < 6 Then
                            ErrMsg = "系统版本不支持创建网络连接"
                            CloseWLAN(hwd)
                            Return False
                        End If
                        If Path Is Nothing Then
                            Path = System.Environment.CurrentDirectory
                        ElseIf Path.Trim = "" Then
                            Path = System.Environment.CurrentDirectory
                        Else
                            If Path.EndsWith("\") Then
                                Path = Left(Path, Path.Length - 1)
                            End If
                            Path = Path + "\" + SSID.Trim + ".xml"
                        End If
                        Try
                            Dim tt As IO.StreamWriter = New IO.StreamWriter(Path, False, System.Text.Encoding.ASCII)
                            tt.Write(profilexml)
                            tt.Close()
                        Catch

                        End Try
                        Dim NetLink As String = ""
                        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
                        Dim adapter As NetworkInterface
                        For Each adapter In adapters
                            If adapter.Description.ToLower.Contains(NetName.ToLower.Trim) Then
                                NetLink = adapter.Name
                                Exit For
                            End If
                        Next adapter
                        If NetLink.Trim = "" Or Not IO.File.Exists(Path) Then
                            CloseWLAN(hwd)
                            ErrMsg = "创建配置文件失败:找不到配置文件或网络设备"
                            Return False
                        End If
                        Try
                            Dim pro As System.Diagnostics.Process = New System.Diagnostics.Process()
                            pro.StartInfo.UseShellExecute = False
                            pro.StartInfo.FileName = "netsh.exe"
                            pro.StartInfo.Arguments = "wlan add profile filename=""" + Path + """ interface=""" + NetLink.Trim + """ "
                            pro.StartInfo.RedirectStandardOutput = True
                            pro.StartInfo.RedirectStandardError = True
                            pro.StartInfo.CreateNoWindow = True
                            pro.Start()
                            pro.WaitForExit()
                            NetLink = pro.StandardOutput.ReadToEnd
                        Catch ex As Exception
                            NetLink = ex.Message
                        End Try
                        If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                            ErrMsg = "创建配置文件失败：" + NetLink
                            CloseWLAN(hwd)
                            Return False
                        Else
                            Try
                                IO.File.Delete(Path)
                            Catch

                            End Try
                        End If
                    Else
                        ErrMsg = "创建配置文件失败" + res.ToString
                        CloseWLAN(hwd)
                        Return False
                    End If
                End If
                Dim connectionParams As WlanConnectionParameters = New WlanConnectionParameters()
                connectionParams.wlanConnectionMode = mode
                connectionParams.profile = SSID
                connectionParams.dot11BssType = BssType
                connectionParams.flags = 0
                res = WlanConnect(hwd, info.interfaceGuid, connectionParams, IntPtr.Zero)
                If res = 0 Then
                    CloseWLAN(hwd)
                    Return True
                Else
                    ErrMsg = "WLANAPI错误代码:" + res.ToString
                    CloseWLAN(hwd)
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
    ''' 添加网络连接
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <param name="SSID">要连接的SSID</param>
    ''' <param name="Author">认证模式</param>
    ''' <param name="PassWord">密码</param>
    ''' <param name="Path">临时保存位置</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function AddProfile(ByVal NetName As String, ByVal SSID As String, ByVal Author As WIFIauthentication, ByVal PassWord As String, Optional ByVal Path As String = "") As String
        Try
            Dim mode As WlanConnectionMode = WlanConnectionMode.Profile
            Dim BssType As Dot11BssType = Dot11BssType.Any
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            Dim res As Integer = 0
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                    Dim ssidhex As String = ""
                    Dim buf() As Byte = System.Text.Encoding.ASCII.GetBytes(SSID.ToUpper)
                    For i As Integer = 0 To buf.Length - 1
                        ssidhex += buf(i).ToString("X2")
                    Next
                    Dim authentication As String = ""
                    Dim encryption As String = ""
                    Dim keytype As String = ""
                    Select Case Author
                        Case WIFIauthentication.WEP_Open
                            authentication = "open"
                            encryption = "WEP"
                            keytype = "networkKey"
                        Case WIFIauthentication.WEP_Share
                            authentication = "shared"
                            encryption = "WEP"
                            keytype = "networkKey"
                        Case WIFIauthentication.WPA_TKIP
                            authentication = "WPAPSK"
                            encryption = "TKIP"
                            keytype = "passPhrase"
                        Case WIFIauthentication.WPA_AES
                            authentication = "WPAPSK"
                            encryption = "AES"
                            keytype = "passPhrase"
                        Case WIFIauthentication.WPS2_TKIP
                            authentication = "WPA2PSK"
                            encryption = "TKIP"
                            keytype = "passPhrase"
                        Case WIFIauthentication.WPS2_AES
                            authentication = "WPA2PSK"
                            encryption = "AES"
                            keytype = "passPhrase"
                        Case Else
                            authentication = "open"
                            encryption = "none"
                            keytype = "networkKey"
                    End Select
                    Dim reasonCode As WlanReasonCode
                    Dim flags As WlanProfileFlags = WlanProfileFlags.AllUser
                    Dim profilexml As String = ""
                    profilexml = "<?xml version=""1.0"" ?>" & vbCrLf & _
                                 "<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">" & vbCrLf & _
                                 "   <name>" & SSID.ToUpper & "</name>" & vbCrLf & _
                                 "   <SSIDConfig>" & vbCrLf & _
                                 "       <SSID> " & vbCrLf & _
                                 "          <hex>" & ssidhex & "</hex> " & vbCrLf & _
                                 "          <name>" & SSID.ToUpper & "</name> " & vbCrLf & _
                                 "       </SSID> " & vbCrLf & _
                                 "       <nonBroadcast>false</nonBroadcast>" & vbCrLf & _
                                 "   </SSIDConfig>" & vbCrLf & _
                                 "   <connectionType>ESS</connectionType>" & vbCrLf & _
                                 "   <connectionMode>auto</connectionMode>" & vbCrLf & _
                                 "   <autoSwitch>false</autoSwitch>" & vbCrLf & _
                                 "   <MSM>" & vbCrLf & _
                                 "     <security>" & vbCrLf & _
                                 "       <authEncryption>" & vbCrLf & _
                                 "          <authentication>" & authentication & "</authentication>" & vbCrLf & _
                                 "          <encryption>" & encryption & "</encryption>" & vbCrLf & _
                                 "          <useOneX>false</useOneX>" & vbCrLf & _
                                 "       </authEncryption>" & vbCrLf & _
                                 "       <sharedKey>" & vbCrLf & _
                                 "          <keyType>" & keytype & "</keyType>" & vbCrLf & _
                                 "          <protected>false</protected>" & vbCrLf & _
                                 "          <keyMaterial>" & PassWord & "</keyMaterial>" & vbCrLf & _
                                 "       </sharedKey>" & vbCrLf & _
                                 "     </security>" & vbCrLf & _
                                 "    </MSM>" & vbCrLf & _
                                 "</WLANProfile>"
                    res = WlanSetProfile(hwd, info.interfaceGuid, flags, profilexml, Nothing, True, IntPtr.Zero, reasonCode)
                    If res = 0 Then
                        If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                            CloseWLAN(hwd)
                            ErrMsg = "创建配置文件失败"
                            Return False
                        Else
                            Return True
                        End If
                    ElseIf res = 1206 Then
                        Dim x86 As System.OperatingSystem = System.Environment.OSVersion
                        If x86.Version.Major < 6 Then
                            CloseWLAN(hwd)
                            ErrMsg = "系统版本不支持创建网络连接"
                            Return False
                        End If
                        If Path Is Nothing Then
                            Path = System.Environment.CurrentDirectory
                        ElseIf Path.Trim = "" Then
                            Path = System.Environment.CurrentDirectory
                        Else
                            If Path.EndsWith("\") Then
                                Path = Left(Path, Path.Length - 1)
                            End If
                            Path = Path + "\" + SSID.Trim + ".xml"
                        End If
                        Try
                            Dim tt As IO.StreamWriter = New IO.StreamWriter(Path, False, System.Text.Encoding.ASCII)
                            tt.Write(profilexml)
                            tt.Close()
                        Catch

                        End Try
                        Dim NetLink As String = ""
                        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
                        Dim adapter As NetworkInterface
                        For Each adapter In adapters
                            If adapter.Description.ToLower.Contains(NetName.ToLower.Trim) Then
                                NetLink = adapter.Name
                                Exit For
                            End If
                        Next adapter
                        If NetLink.Trim = "" Or Not IO.File.Exists(Path) Then
                            CloseWLAN(hwd)
                            ErrMsg = "创建配置文件失败:找不到配置文件或网络设备"
                            Return False
                        End If
                        Try
                            Dim pro As System.Diagnostics.Process = New System.Diagnostics.Process()
                            pro.StartInfo.UseShellExecute = False
                            pro.StartInfo.FileName = "netsh.exe"
                            pro.StartInfo.Arguments = "wlan add profile filename=""" + Path + """ interface=""" + NetLink.Trim + """ "
                            pro.StartInfo.RedirectStandardOutput = True
                            pro.StartInfo.RedirectStandardError = True
                            pro.StartInfo.CreateNoWindow = True
                            pro.Start()
                            pro.WaitForExit()
                            NetLink = pro.StandardOutput.ReadToEnd
                        Catch ex As Exception
                            NetLink = ex.Message
                        End Try
                        If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                            CloseWLAN(hwd)
                            ErrMsg = "创建配置文件失败：" + NetLink
                            Return False
                        Else
                            Try
                                IO.File.Delete(Path)
                            Catch

                            End Try
                            CloseWLAN(hwd)
                            Return True
                        End If
                    Else
                        CloseWLAN(hwd)
                        ErrMsg = "创建配置文件失败" + res.ToString
                        Return False
                    End If
                Else
                    CloseWLAN(hwd)
                    Return True
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
    ''' 编辑网络连接
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <param name="SSID">要连接的SSID</param>
    ''' <param name="Author">认证模式</param>
    ''' <param name="PassWord">密码</param>
    ''' <param name="Path">临时保存位置</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function editProfile(ByVal NetName As String, ByVal SSID As String, ByVal Author As WIFIauthentication, ByVal PassWord As String, Optional ByVal Path As String = "") As String
        Try
            Dim mode As WlanConnectionMode = WlanConnectionMode.Profile
            Dim BssType As Dot11BssType = Dot11BssType.Any
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            Dim res As Integer = 0
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                    WlanDeleteProfile(hwd, info.interfaceGuid, SSID, IntPtr.Zero)
                End If
                Dim ssidhex As String = ""
                Dim buf() As Byte = System.Text.Encoding.ASCII.GetBytes(SSID.ToUpper)
                For i As Integer = 0 To buf.Length - 1
                    ssidhex += buf(i).ToString("X2")
                Next
                Dim authentication As String = ""
                Dim encryption As String = ""
                Dim keytype As String = ""
                Select Case Author
                    Case WIFIauthentication.WEP_Open
                        authentication = "open"
                        encryption = "WEP"
                        keytype = "networkKey"
                    Case WIFIauthentication.WEP_Share
                        authentication = "shared"
                        encryption = "WEP"
                        keytype = "networkKey"
                    Case WIFIauthentication.WPA_TKIP
                        authentication = "WPAPSK"
                        encryption = "TKIP"
                        keytype = "passPhrase"
                    Case WIFIauthentication.WPA_AES
                        authentication = "WPAPSK"
                        encryption = "AES"
                        keytype = "passPhrase"
                    Case WIFIauthentication.WPS2_TKIP
                        authentication = "WPA2PSK"
                        encryption = "TKIP"
                        keytype = "passPhrase"
                    Case WIFIauthentication.WPS2_AES
                        authentication = "WPA2PSK"
                        encryption = "AES"
                        keytype = "passPhrase"
                    Case Else
                        authentication = "open"
                        encryption = "none"
                        keytype = "networkKey"
                End Select
                Dim reasonCode As WlanReasonCode
                Dim flags As WlanProfileFlags = WlanProfileFlags.AllUser
                Dim profilexml As String = ""
                profilexml = "<?xml version=""1.0"" ?>" & vbCrLf & _
                             "<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">" & vbCrLf & _
                             "   <name>" & SSID.ToUpper & "</name>" & vbCrLf & _
                             "   <SSIDConfig>" & vbCrLf & _
                             "       <SSID> " & vbCrLf & _
                             "          <hex>" & ssidhex & "</hex> " & vbCrLf & _
                             "          <name>" & SSID.ToUpper & "</name> " & vbCrLf & _
                             "       </SSID> " & vbCrLf & _
                             "       <nonBroadcast>false</nonBroadcast>" & vbCrLf & _
                             "   </SSIDConfig>" & vbCrLf & _
                             "   <connectionType>ESS</connectionType>" & vbCrLf & _
                             "   <connectionMode>auto</connectionMode>" & vbCrLf & _
                             "   <autoSwitch>false</autoSwitch>" & vbCrLf & _
                             "   <MSM>" & vbCrLf & _
                             "     <security>" & vbCrLf & _
                             "       <authEncryption>" & vbCrLf & _
                             "          <authentication>" & authentication & "</authentication>" & vbCrLf & _
                             "          <encryption>" & encryption & "</encryption>" & vbCrLf & _
                             "          <useOneX>false</useOneX>" & vbCrLf & _
                             "       </authEncryption>" & vbCrLf & _
                             "       <sharedKey>" & vbCrLf & _
                             "          <keyType>" & keytype & "</keyType>" & vbCrLf & _
                             "          <protected>false</protected>" & vbCrLf & _
                             "          <keyMaterial>" & PassWord & "</keyMaterial>" & vbCrLf & _
                             "       </sharedKey>" & vbCrLf & _
                             "     </security>" & vbCrLf & _
                             "    </MSM>" & vbCrLf & _
                             "</WLANProfile>"
                res = WlanSetProfile(hwd, info.interfaceGuid, flags, profilexml, Nothing, True, IntPtr.Zero, reasonCode)
                If res = 0 Then
                    If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                        CloseWLAN(hwd)
                        ErrMsg = "创建配置文件失败"
                        Return False
                    Else
                        Return True
                    End If
                ElseIf res = 1206 Then
                    Dim x86 As System.OperatingSystem = System.Environment.OSVersion
                    If x86.Version.Major < 6 Then
                        CloseWLAN(hwd)
                        ErrMsg = "系统版本不支持创建网络连接"
                        Return False
                    End If
                    If Path Is Nothing Then
                        Path = System.Environment.CurrentDirectory
                    ElseIf Path.Trim = "" Then
                        Path = System.Environment.CurrentDirectory
                    Else
                        If Path.EndsWith("\") Then
                            Path = Left(Path, Path.Length - 1)
                        End If
                        Path = Path + "\" + SSID.Trim + ".xml"
                    End If
                    Try
                        Dim tt As IO.StreamWriter = New IO.StreamWriter(Path, False, System.Text.Encoding.ASCII)
                        tt.Write(profilexml)
                        tt.Close()
                    Catch

                    End Try
                    Dim NetLink As String = ""
                    Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
                    Dim adapter As NetworkInterface
                    For Each adapter In adapters
                        If adapter.Description.ToLower.Contains(NetName.ToLower.Trim) Then
                            NetLink = adapter.Name
                            Exit For
                        End If
                    Next adapter
                    If NetLink.Trim = "" Or Not IO.File.Exists(Path) Then
                        CloseWLAN(hwd)
                        ErrMsg = "创建配置文件失败:找不到配置文件或网络设备"
                        Return False
                    End If
                    Try
                        Dim pro As System.Diagnostics.Process = New System.Diagnostics.Process()
                        pro.StartInfo.UseShellExecute = False
                        pro.StartInfo.FileName = "netsh.exe"
                        pro.StartInfo.Arguments = "wlan add profile filename=""" + Path + """ interface=""" + NetLink.Trim + """ "
                        pro.StartInfo.RedirectStandardOutput = True
                        pro.StartInfo.RedirectStandardError = True
                        pro.StartInfo.CreateNoWindow = True
                        pro.Start()
                        pro.WaitForExit()
                        NetLink = pro.StandardOutput.ReadToEnd
                    Catch ex As Exception
                        NetLink = ex.Message
                    End Try
                    If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                        CloseWLAN(hwd)
                        ErrMsg = "创建配置文件失败：" + NetLink
                        Return False
                    Else
                        Try
                            IO.File.Delete(Path)
                        Catch

                        End Try
                        CloseWLAN(hwd)
                        Return True
                    End If
                Else
                    ErrMsg = "创建配置文件失败" + res.ToString
                    CloseWLAN(hwd)
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
    ''' 删除网络连接
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <param name="SSID">SSID配置</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function DeleteProfile(ByVal NetName As String, ByVal SSID As String) As String
        Try
            Dim mode As WlanConnectionMode = WlanConnectionMode.Profile
            Dim BssType As Dot11BssType = Dot11BssType.Any
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            Dim res As Integer = 0
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                If Not ProfilesIsExist(hwd, info, NetName, SSID) Then
                    WlanDeleteProfile(hwd, info.interfaceGuid, SSID, IntPtr.Zero)
                End If
                CloseWLAN(hwd)
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 当前连接的SSID
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <returns>SSID</returns>
    ''' <remarks></remarks>
    Public Shared Function CurrentSSID(ByVal NetName As String) As String
        Try
            Dim mode As WlanConnectionMode = WlanConnectionMode.Profile
            Dim BssType As Dot11BssType = Dot11BssType.Any
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            Dim res As Integer = 0
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                Dim valueSize As Integer = 0
                Dim valuePtr As IntPtr = IntPtr.Zero
                Dim opcodeValueType As WlanOpcodeValueType
                res = WlanQueryInterface(hwd, info.interfaceGuid, WlanIntfOpcode.CurrentConnection, IntPtr.Zero, valueSize, valuePtr, opcodeValueType)
                If res = 0 Then
                    Dim ssid As WlanConnectionAttributes = CType(Marshal.PtrToStructure(valuePtr, GetType(WlanConnectionAttributes)), WlanConnectionAttributes)
                    If ssid.profileName Is Nothing Then
                        CloseWLAN(hwd)
                        Return ""
                    Else
                        CloseWLAN(hwd)
                        Return ssid.profileName.Trim()
                    End If
                Else
                    ErrMsg = res.ToString
                    CloseWLAN(hwd)
                    Return ""
                End If
            Else
                Return ""
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 断开WIFI连接
    ''' </summary>
    ''' <param name="NetName">WIFI适配器</param>
    ''' <returns>执行结果</returns>
    ''' <remarks></remarks>
    Public Shared Function DisconnectWIFI(ByVal NetName As String) As Boolean
        Try
            Dim mode As WlanConnectionMode = WlanConnectionMode.Profile
            Dim BssType As Dot11BssType = Dot11BssType.Any
            Dim hwd As IntPtr = IntPtr.Zero
            Dim info As New WlanInterfaceInfo
            Dim res As Integer = 0
            If GetWlanInterfaceInfo(NetName, hwd, info) Then
                WlanDisconnect(hwd, info.interfaceGuid, IntPtr.Zero)
                CloseWLAN(hwd)
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' PING函数
    ''' </summary>
    ''' <param name="DestAddr">目前地址</param>
    ''' <param name="PackSize">包大小</param>
    ''' <param name="ConnectTime">超时</param>
    ''' <returns>延时</returns>
    ''' <remarks></remarks>
    Public Shared Function Ping(ByVal DestAddr As String, Optional ByVal PackSize As UShort = 0, Optional ByVal ConnectTime As Integer = 2000) As Integer
        Try
            If DestAddr Is Nothing Then
                Return -1
            ElseIf DestAddr.Trim = "" Then
                Return -1
            Else
                Dim pings As System.Net.NetworkInformation.Ping = New System.Net.NetworkInformation.Ping()
                Dim Res As System.Net.NetworkInformation.PingReply
                If PackSize = 0 Then
                    Res = pings.Send(DestAddr, ConnectTime)
                Else
                    Dim buf(PackSize - 1) As Byte
                    For i As Integer = LBound(buf) To UBound(buf)
                        buf(i) = 0
                    Next
                    Res = pings.Send(DestAddr, ConnectTime, buf)
                End If
                If Res.Status = IPStatus.Success Then
                    Return Res.RoundtripTime
                Else
                    Return -3
                End If
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return -2
        End Try
    End Function

    Public Shared Function TelNet(ByVal DestAddr As String, DestPort As Integer) As Boolean
        Try
            Dim ipv4 As IPAddress = IPAddress.Broadcast
            If DestAddr Is Nothing Then
                Return False
            ElseIf DestAddr.Trim = "" Then
                Return False
            ElseIf Not Net.IPAddress.TryParse(DestAddr.Trim, ipv4) Then
                Return False
            Else
                Dim listener As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                Dim RemoteEndPoint As New IPEndPoint(Net.IPAddress.Parse(DestAddr), DestPort)
                listener.Connect(RemoteEndPoint)
                System.Threading.Thread.Sleep(1000)
                listener.Close()
                Return True
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取本地IP组
    ''' </summary>
    ''' <returns>本地IP组</returns>
    ''' <remarks></remarks>
    Public Shared Function LocalIPV4() As String()
        Try
            Dim Address() As System.Net.IPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList
            If Address.Length > 0 Then
                Dim iplist As List(Of String) = New List(Of String)
                For i As Integer = LBound(Address) To UBound(Address)
                    If Address(i).AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                        iplist.Add(Address(i).ToString)
                    End If
                Next
                If iplist.Count > 0 Then
                    Dim ss(iplist.Count - 1) As String
                    iplist.CopyTo(ss)
                    Return ss
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 获取本地IP组
    ''' </summary>
    ''' <returns>本地IP组</returns>
    ''' <remarks></remarks>
    Public Shared Function LocalIPV6() As String()
        Try
            Dim Address() As System.Net.IPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList
            If Address.Length > 0 Then
                Dim iplist As List(Of String) = New List(Of String)
                For i As Integer = LBound(Address) To UBound(Address)
                    If Address(i).AddressFamily = Net.Sockets.AddressFamily.InterNetworkV6 Then
                        iplist.Add(Address(i).ToString)
                    End If
                Next
                If iplist.Count > 0 Then
                    Dim ss(iplist.Count - 1) As String
                    iplist.CopyTo(ss)
                    Return ss
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            ErrMsg = ex.Message
            Return Nothing
        End Try
    End Function

End Class
