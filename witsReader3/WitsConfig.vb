Public Class WitsConnectionSettings

    Public Property UpdateTime As Date
    Public Property Baudrate As Integer = 9600
    Public Property Comport As String = "COM1"
    Public Property Request_Mode As Boolean = True
    Public Property Request_String As String = "1996-9999"
    Public Property Request_Interval_Seconds As Integer = 5

    Public Sub New()
        UpdateTime = Date.UtcNow()
    End Sub

End Class
