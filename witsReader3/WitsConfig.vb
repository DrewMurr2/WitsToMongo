Imports MongoDB.Bson.Serialization.Attributes
Imports MongoDB.Driver
Imports MongoDB.Bson
<BsonIgnoreExtraElements>
Public Class Settings
    Public Shared Property db As IMongoDatabase = New MongoClient().GetDatabase("Settings")
    <BsonIgnoreExtraElements>
    Public Class WitsConnectionSettings
        Public Property sett As Settings
        Public Shared Property collection As IMongoCollection(Of WitsConnectionSettings) = db.GetCollection(Of WitsConnectionSettings)("WitsConnection")
        Public Property UpdateTime As Date
        Public Property Baudrate As Integer = 9600
        Public Property Comport As String = "COM1"
        Public Property Request_Mode As Boolean = True
        Public Property Request_String As String = "1996-9999"
        Public Property Request_Interval_Seconds As Integer = 5

        Public Sub New()
            UpdateTime = Date.UtcNow()
        End Sub

        Public Shared Function Latest() As WitsConnectionSettings
            Return collection.Find(Function(s) s.Baudrate <> -1).SortByDescending(Function(s) s.UpdateTime).Limit(1).ToListAsync().Result(0)
        End Function

    End Class
End Class
