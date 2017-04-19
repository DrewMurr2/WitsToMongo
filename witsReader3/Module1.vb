Imports System.IO
Imports Newtonsoft.Json
Imports MongoDB.Driver
Imports MongoDB.Bson

Module Module1
    Dim request As String = "&&" & vbCr & vbLf & "0001-9999" & vbCr & vbLf & "!!" & vbCr & vbLf
    Dim WitsSettings As WitsConnectionSettings
    Dim AlternateRequest As Boolean = False
    Dim MongoC As MongoClient
    Dim MongoRawDB As IMongoDatabase
    Dim MongoSettingsDB As IMongoDatabase
    Dim RawInstantsCollection As IMongoCollection(Of RawInstant)
    Dim WitsConnectionSettingsCollection As IMongoCollection(Of WitsConnectionSettings)
    Sub Main()
        '****Main should only be run on first program launch
        StartOver()
        Console.ReadLine()
    End Sub

    Sub StartOver()
        Console.WriteLine("StartOver " & Date.Now())
        '*****StartOver should be called any time an error is thrown or the system has not recorded data in a set time interval
        MongoC = New MongoClient() 'Establish Mongo Client
        MongoRawDB = MongoC.GetDatabase("Raw") ' Establish Mongo Raw Database
        MongoSettingsDB = MongoC.GetDatabase("Settings") ' Establish Mongo Settings Database

        Dim thisInstant = Date.UtcNow()
        RawInstantsCollection = MongoRawDB.GetCollection(Of RawInstant)("RawInstants_" & thisInstant.Month() & "_" & thisInstant.Year()) ' Establish Mongo Collection this naming convention is RawInstants_Mo_Year
        WitsConnectionSettingsCollection = MongoSettingsDB.GetCollection(Of WitsConnectionSettings
            )("WitsConnection") ' Establish Mongo Wits Connection Collection 

        SerialPort1 = New Ports.SerialPort 'Create Serial Port Adapter
        requestTimer = New Timers.Timer 'Create request timer - used if WitsSettings in Request mode

        RetrieveSettings() 'RetrieveSettings gets the JSON encoded Wits Settings and applies the properties to the SerialPort1 object

        Connect() 'Connect connects to the serial port specified in the settings

        'If Requestmode is true then Request mode sends a Wits Request according to the specified timer
        If WitsSettings.Request_Mode Then
            requestTimer.Interval = WitsSettings.Request_Interval_Seconds * 1000
            requestTimer.Start()
        End If

        '****receivedTimer tries again if nothing is received
        receivedTimer = New Timers.Timer
        receivedTimer.Interval = 30000 'If no data is received within 30 seconds it tries again.
        receivedTimer.Start()

    End Sub
    Sub DestroyAndRestart()
        DestroyOldConnections()
        StartOver()
    End Sub
    Sub DestroyOldConnections()
        If SerialPort1.IsOpen Then SerialPort1.Close()
        SerialPort1.Dispose()
        SerialPort1 = Nothing
        receivedTimer.Stop()
        receivedTimer.Dispose()
        receivedTimer = Nothing
        requestTimer.Stop()
        requestTimer.Dispose()
        requestTimer = Nothing
    End Sub
    Sub RetrieveSettings() 'Retrieves the JSON settings file in C:\WitsReader\WitsConfig.json
        Dim stringFromWitsFile = File.ReadAllText("C:\WitsReader\WitsConfig.json")
        WitsSettings = JsonConvert.DeserializeObject(Of WitsConnectionSettings)(stringFromWitsFile)
        For i = 1 To 5
            WitsConnectionSettingsCollection.InsertOne(WitsSettings)
        Next
        Dim f As FilterDefinition(Of WitsConnectionSettings)

        Dim hh = WitsConnectionSettingsCollection.Find(f.Equals(New WitsConnectionSettings() With {.Baudrate = 9600}))

    End Sub


    Sub Connect() 'Connect to the serial port with the established Wits Settings
        If SerialPort1.IsOpen Then SerialPort1.Close()
        SerialPort1.PortName = WitsSettings.Comport
        SerialPort1.BaudRate = WitsSettings.Baudrate
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
    End Sub

    Sub requestTimerTick() Handles requestTimer.Elapsed 'Sends requests to the EDR to update data
        requestTimer.Stop()
        Console.WriteLine("RequestTimer: " & Date.Now())
        AlternateRequest = Not AlternateRequest  'Alternating the request string is a best practice established through experience
        If AlternateRequest Then '-It hasn't been established if this code effectively alternates the request
            SerialPort1.WriteLine(request)
        Else
            SerialPort1.WriteLine("&&" & vbCr & vbLf & WitsSettings.Request_String & vbCr & vbLf & "!!")
        End If
        If Not receivedTimer.Enabled Then receivedTimer.Start() 'This line of code should not exist - I can not figure out why receivedTimer is closing and not opening back up
        requestTimer.Start()
    End Sub
    Sub receivedTimerTick() Handles receivedTimer.Elapsed
        receivedTimer.Stop()
        DestroyAndRestart()
    End Sub
    Private Sub SerialPort1_DataReceived(sender As Object, e As Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Try
            receivedTimer.Stop()
            Dim dataString As String = SerialPort1.ReadTo("!!")
            Dim StrAr As String() = dataString.Split(Environment.NewLine)
            For Each str As String In StrAr
                str = str.Replace(vbLf, "")
            Next
            Dim WitsID As Double
            Dim newRawInstant As New RawInstant
            For Each Str As String In StrAr
                Dim WitsString = Left(Str, 5)
                WitsString = WitsString.Replace(vbLf, "")
                Dim l As Integer = Str.Length - 5
                If l > 0 Then
                    Dim WitsValue = Right(Str, l)
                    If Double.TryParse(WitsString, WitsID) Then
                        newRawInstant.add(WitsID, WitsValue)
                    End If
                End If
            Next
            If newRawInstant.data.Count > 0 Then RawInstantsCollection.InsertOne(newRawInstant)
            receivedTimer.Start()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            DestroyAndRestart()
        End Try
    End Sub

    Friend WithEvents SerialPort1 As Ports.SerialPort
    Friend WithEvents requestTimer As Timers.Timer
    Friend WithEvents receivedTimer As Timers.Timer


    '******************BELOW IS CODE I USED WHILE BUILDING AND TESTING*****************************
    '  Dim w As New WitsConfig
    'Dim s As String = JsonConvert.SerializeObject(w)
    'FileOpen(37, "C:\WitsReader\WitsConfig.json", OpenMode.Output)
    'PrintLine(37, s)
    'FileClose(37)
    'For i = 0 To 1000
    '        RawInstants.InsertOne(RandomRawInstant())
    '    Next
    'Public Function RandomRawInstant()

    '    Dim nri As New RawInstant
    '    For i = 1000 To 1015
    '        StrCnt = (StrCnt + 1) Mod strArr.Length
    '        nri.add(i, strArr(StrCnt))
    '    Next
    '    Return nri

    'End Function
    'Dim strArr As String() = {"one", "two", "three", "four", "five"}
    'Dim StrCnt = 0

End Module
