Public Class Message

    Public Property Guid As String = ""
    Public Property RSSI As Int16
    Public Property Data As Byte()
    Public Property Time As DateTime = Now
    Sub New(_data As Byte())
        Dim list As List(Of Byte) = New List(Of Byte)
        For i = 0 To 3
            Guid += (Conversion.Hex(_data(i)) + "-")
        Next
        Guid = Guid.Substring(0, Guid.Length - 1)
        RSSI = -(_data(4) And &H7F)
        For i = 5 To _data.Length - 1
            list.Add(_data(i))
        Next
        Me.Data = list.ToArray()
    End Sub
End Class
