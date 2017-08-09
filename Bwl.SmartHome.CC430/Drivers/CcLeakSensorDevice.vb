Public Class CcLeakSensorDevice
    Inherits CcBaseDevice
    Private _leakDetected As Boolean = False
    Private _leakAction As New SmartStateScheme
    Private _normalLevel As UInt16 = 0
    Private _workState = "online"
    Private ReadOnly _messagePeriod As TimeSpan = TimeSpan.FromSeconds(60)
    Public Sub New(logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(logger, guid, shc)
        _leakAction.ID = "leak"
        _leakAction.Type = SmartStateType.stateYesNo
        _leakAction.DefaultCaption = "Протечка"
        _objectScheme.ClassID = "SsValveDriver"
        _objectScheme.DefaultCaption = "Датчик утечки " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Датчики"}
        _objectScheme.DefaultShortName = ""
        _objectScheme.States.Add(_leakAction)
        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
        AddHandler _shc.SendObjectsSchemesTimer, AddressOf SendObjectsTimerHandler
    End Sub

    Private Sub SendObjectsTimerHandler()
        Try
            If TimeSpan.Compare(Now - _lastSuccessRequest, _messagePeriod) > 0 Then
                _workState = "offline"
            Else
                _workState = "online"
            End If
            _objectScheme.DefaultCaption = "Датчик утечки [" + _workState + "] " + Guid
            _shc.SmartHome.Objects.SetScheme(_guid, _objectScheme)
        Catch ex As Exception
            _logger.AddWarning(ex.Message)
        End Try
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then

        End If
    End Sub

    Public Overrides Sub MessageProcessor(msg As Message)
        _lastSuccessRequest = Now
        Dim adc = msg.Data(0) * 256 + msg.Data(1)
        _logger.AddMessage("MSG: " + msg.RSSI.ToString + "dB")
        If _normalLevel = 0 Then _normalLevel = adc
        Dim state = _normalLevel - adc > 1000
        Try
            _shc.SmartHome.Objects.SetValue(_guid, _leakAction.ID, If(state, "yes", "no"), ChangedBy.device)
            If (_leakDetected <> state) Then
                _shc.SmartHome.Objects.SetScheme(_guid, _objectScheme)
                SendObjectsTimerHandler()
            End If
            _leakDetected = state
        Catch ex As Exception

        End Try
    End Sub
End Class

