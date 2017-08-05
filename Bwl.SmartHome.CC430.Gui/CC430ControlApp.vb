Imports Bwl.Framework

Public Class CC430ControlApp
    Inherits SmartHomeClientBase
    Private _dm As DeviceManager
    Private Sub TestApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _dm = New DeviceManager(_logger, _client)
        _dm.Start()
    End Sub

    Private Sub tPoll_Tick(sender As Object, e As EventArgs) Handles tPoll.Tick

    End Sub

    Private Sub tStates_Tick(sender As Object, e As EventArgs) Handles tStates.Tick
        lbDevices.Items.Clear()
        For Each device In _dm.Devices.Values
            lbDevices.Items.Add(device.Guid + "        " + device.LastActiveTime)
        Next
    End Sub
End Class
