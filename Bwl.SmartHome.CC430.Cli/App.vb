Imports Bwl.Framework
Imports Bwl.SmartHome.SimplSerial.AvrControl

Module App
    Private _app As New AppBase
    Private _storage As SettingsStorage = _app.RootStorage
    Private _logger As Logger = _app.RootLogger
    Private _client As New SmartHomeClient(_storage, _logger)

    Private _bus As New Hardware.SimplSerial.SimplSerialBus
    Private _portSetting As New StringSetting(_storage, "BusPort", "")
    Private _deviceManager As New DeviceManager(_bus, _logger, _client)

    Sub Main()
        _logger.ConnectWriter(New ConsoleLogWriter)
        Dim cmd = Command().Split(" ")
        For Each cmdItem In cmd
            Console.WriteLine(cmdItem)
            Dim parts = cmdItem.Split("*")
            If parts.Length = 2 Then
                Select Case parts(0)
                    Case "server"
                        _client.Transport.AddressSetting.Value = parts(1)
                    Case "busport"
                        _portSetting.Value = parts(1)
                End Select
            End If
        Next

        _deviceManager.Drivers.Add(New SsSwitchOneDriver(_bus, _logger, _client))
        _deviceManager.Drivers.Add(New SsMultiSwitchDriver(_bus, _logger, _client))
        Console.WriteLine("Bwl.SmartHome.SimplSerial.AvrControl.Cli")
        Console.WriteLine()
        For Each df In _deviceManager.Drivers
            Console.WriteLine("Driver: " + df.GetType.Name)
        Next

        If _portSetting.Value = "" Then
            Dim devices = System.IO.Ports.SerialPort.GetPortNames
            For Each port In devices
                If port.ToLower.Contains("com") Or port.ToLower.Contains("ttyusb") Then
                    _portSetting.Value = port
                    Exit For
                End If
            Next
        End If

        Dim pollThread As New Threading.Thread(AddressOf Poll)
        pollThread.Start()

        Dim updateObjectsThread As New Threading.Thread(AddressOf UpdateObjects)
        updateObjectsThread.Start()

        Dim searchDevicesThread As New Threading.Thread(AddressOf SearchDevices)
        searchDevicesThread.Start()
        Do
            Threading.Thread.Sleep(1)
        Loop
    End Sub

    Private Sub Poll()
        Do
            If _bus.IsConnected = False And _portSetting.Value > "" Then
                Try
                    _bus.SerialDevice.DeviceAddress = _portSetting.Value
                    _bus.SerialDevice.DeviceSpeed = 9600
                    _bus.Connect()
                    Dim ret = _bus.RequestDeviceInfo(0)
                    Console.WriteLine(ret.DeviceName)
                    _logger.AddMessage("Bus Connected on port " + _portSetting.Value)
                    For i = 1 To 5
                        _deviceManager.SearchDevices()
                    Next
                    _logger.AddMessage("Search finished")
                Catch ex As Exception
                    _logger.AddWarning(ex.Message)
                End Try
            End If

            If _bus.IsConnected Then
                _deviceManager.PollDevices()
            End If
            Threading.Thread.Sleep(500)
        Loop
    End Sub

    Private Sub SearchDevices()
        Do
            Try
                _deviceManager.SearchDevices()
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(60000)
        Loop
    End Sub

    Private Sub UpdateObjects()
        Do
            _deviceManager.UpdateObjects()
            Threading.Thread.Sleep(1000)
        Loop
    End Sub
End Module
