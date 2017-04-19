Public Class RawInstant
    Public Property CapturedTime As Date
    Public Property data As New List(Of dataPair)
    Public Sub New()
        CapturedTime = Date.UtcNow()
    End Sub
    Public Sub add(i, v)
        data.Add(New dataPair() With {.id = i, .val = v})
    End Sub
    Public Class dataPair
        Public Property id As Integer
        Public Property val As String
    End Class
End Class
