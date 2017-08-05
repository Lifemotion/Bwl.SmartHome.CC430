Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class CcLeakSensorDevice
    Inherits CcBaseDevice
    Private _leakDetected As Boolean = False
    Private _leakAction As New SmartStateScheme
    Private _normalLevel As UInt16 = 0
    Public Sub New(logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(logger, guid, shc)
        _objectScheme.ClassID = "CcLeakSensor"
        _objectScheme.DefaultCaption = "Датчик протечки " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Датчики"}
        _objectScheme.DefaultShortName = ""
        _leakAction.ID = "leakState"
        _leakAction.Type = SmartStateType.stateYesNo
        _leakAction.DefaultCaption = "Состояние"
        _objectScheme.States.Add(_leakAction)
        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then

        End If
    End Sub

    Public Overrides Sub MessageProcessor(msg As Message)
        _lastSuccessRequest = Now
        Dim adc = msg.Data(0) * 256 + msg.Data(1)
        _logger.AddMessage(adc.ToString)
        If _normalLevel = 0 Then _normalLevel = adc
        Dim state = _normalLevel - adc > 1000
        If state Then
            Try
                _shc.SmartHome.Objects.SetValue(_guid, _leakAction.ID, If(state, "Dry", "Leak"), ChangedBy.device)
                _leakDetected = state
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class

