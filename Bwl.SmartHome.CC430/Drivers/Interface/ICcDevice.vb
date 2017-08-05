Public Interface ICcDevice
    ReadOnly Property Guid As String
    ReadOnly Property Rssi As String
    ReadOnly Property LastActiveTime As Date
    Sub MessageProcessor(msg As Message)
    Sub UpdateServerObjects()
End Interface
