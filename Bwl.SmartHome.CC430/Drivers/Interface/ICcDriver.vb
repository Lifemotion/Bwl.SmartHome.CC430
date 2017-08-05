Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Interface ICcDriver
    Function CreateDevice(guid As String) As ICcDevice
    Function IsDeviceSupported(devicename As String) As Boolean
End Interface
