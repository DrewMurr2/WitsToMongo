Imports MongoDB.Driver
Imports MongoDB.Bson

Public Class RawInstant
    Public Property Client As New MongoClient
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

    Public Function getCollection() As IMongoCollection(Of RawInstant)
        Return Client.GetDatabase("Raw").GetCollection(Of RawInstant)("RawInstants_" & CapturedTime.Month() & "_" & CapturedTime.Year()) ' Establish Mongo Collection this naming convention is RawInstants_Mo_Year
    End Function

    Public Sub Save()
        getCollection.InsertOne(Me)
    End Sub
End Class
