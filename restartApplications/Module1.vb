Module Module1

    Sub Main()
        OpenProcess()
        System.Threading.Thread.Sleep(1000)
        ReadAndReturnProcesses()
    End Sub
    Sub ReadAndReturnProcesses()
        ' Get processes.
        Dim processes() As Process = Process.GetProcesses()
        Console.WriteLine("Count: {0}", processes.Length)

        ' Loop over processes.
        For Each p As Process In processes
            ' Display process properties.
            Console.WriteLine(p.ProcessName + "/" + p.Id.ToString())
        Next
        Console.ReadLine()
    End Sub
    Sub OpenProcess()
        Dim readWitsData As New Process()
        readWitsData.StartInfo.FileName = "C:\\HelloWorld.exe"

        '' readWitsData.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        readWitsData.Start()
    End Sub



End Module
