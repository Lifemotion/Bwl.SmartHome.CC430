Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public MustInherit Class CcBaseDevice
    Implements ICcDevice
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient
    Protected _guid As String
    Protected _rssi
    Protected _objectScheme As New SmartObjectScheme
    Protected _rnd As New Random
    Protected _lastSuccessRequest As DateTime


    Public ReadOnly Property Rssi As String Implements ICcDevice.Rssi
        Get
            Return _rssi
        End Get
    End Property

    Public ReadOnly Property Guid As String Implements ICcDevice.Guid
        Get
            Return _guid
        End Get
    End Property

    Public ReadOnly Property LastActiveTime As Date Implements ICcDevice.LastActiveTime
        Get
            Return _lastSuccessRequest
        End Get
    End Property

    Public Sub New(logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        _logger = logger
        _shc = shc
        _guid = guid
    End Sub

    Public Sub UpdateServerObjects() Implements ICcDevice.UpdateServerObjects
        _shc.SmartHome.Objects.SetScheme(_guid, _objectScheme)
    End Sub

    Public MustOverride Sub MessageProcessor(msg As Message) Implements ICcDevice.MessageProcessor

End Class
