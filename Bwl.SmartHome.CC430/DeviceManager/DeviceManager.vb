Imports System.IO.Ports
Imports Bwl.Hardware.SimplSerial

Public Class DeviceManager
    Public ReadOnly Property Drivers As New List(Of ICcDriver)
    Public ReadOnly Property Devices As New Dictionary(Of String, ICcDevice)
    Private Property _adapterName As String = "CC430 UsbAdapter"
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient
    Protected _rnd As New Random
    Protected _port As SerialPort = New SerialPort()

    Public Sub New(logger As Framework.Logger, shc As SmartHomeClient)
        _logger = logger
        _shc = shc
    End Sub

    Public Sub Start()
        Dim th = New Threading.Thread(AddressOf DataProcessor)
        th.Start()
    End Sub

    Public Sub DataProcessor()
        Dim _lastByte As Byte = 0
        Dim bytes As List(Of Byte) = New List(Of Byte)
        While True
            If (_port.IsOpen) Then
                Try
                    While _port.BytesToRead = 0
                        Threading.Thread.Sleep(10)
                    End While
                    Dim b = CByte(_port.ReadByte)
                    If _lastByte = &H55 And b = &HAA Then bytes.Clear()
                    If _lastByte = &HAA And b = &H55 Then
                        bytes.RemoveAt(bytes.Count - 1)
                        bytes.RemoveAt(0)
                        bytes.RemoveAt(0)
                        Dim crc = bytes(bytes.Count - 1) + bytes(bytes.Count - 2) * 256
                        bytes.RemoveAt(bytes.Count - 1)
                        bytes.RemoveAt(bytes.Count - 1)
                        Dim data = bytes.ToArray
                        Dim _crcCalc = CRC16(data)
                        If _crcCalc = crc Then
                            Dim msg = New Message(data)
                            UpdateObjects(msg)
                            _port.Write(&H1)
                        End If
                    End If
                    bytes.Add(b)
                    _lastByte = b
                Catch ex As Exception
                    _lastByte = 0
                End Try
            Else
                Threading.Thread.Sleep(500)
                Dim ports = FTDIFunctions.GetFtdiPorts()
                For Each port In ports
                    If port.Description.ToLower.Contains(_adapterName.ToLower) Then
                        _port.PortName = (FTDIFunctions.DetectFtdiSystemPortName(port))
                        _port.BaudRate = 19200
                        _port.Open()
                    End If
                Next
            End If
        End While
    End Sub

    Public Sub UpdateObjects(msg As Message)
        If Not Devices.ContainsKey(msg.Guid) Then
            Dim marker = msg.Guid.Split("-")(0)
            If (marker.Equals("79")) Then
                _logger.AddMessage("New device: leak sensor " + msg.Guid)
                Devices.Add(msg.Guid, New CcLeakSensorDevice(_logger, msg.Guid, _shc))
            ElseIf marker.Equals("78") Then
                _logger.AddMessage("New device: smoke sensor " + msg.Guid)

            End If
        End If
        If Devices.ContainsKey(msg.Guid) Then
            Devices(msg.Guid).MessageProcessor(msg)
        Else
            _logger.AddMessage("Unknow devices " + msg.Guid)
        End If
    End Sub

    Public Shared Function CRC16(msg As Byte()) As UShort
        Dim crc As UShort = &HFFFF
        Dim i As Integer = 0, size As Integer = msg.Length
        While i < size
            crc = CUShort((crc >> 8) Or (crc << 8))
            crc = crc Xor msg(i)
            crc = crc Xor ((crc And &HFF) >> 4)
            crc = crc Xor ((crc << 8) << 4)
            crc = crc Xor (((crc And &HFF) << 4) << 1)
            i += 1
        End While
        Return crc
    End Function
End Class
