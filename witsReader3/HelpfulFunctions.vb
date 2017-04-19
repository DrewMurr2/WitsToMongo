Module HelpfulFunctions
    Function jsNow() As Long 'I dont think I need this after all because Mongo automatically converts
        Return jsDate(Date.UtcNow())
    End Function

    Function jsDate(d As Date) As Long
        Return d.Subtract(New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds
    End Function

End Module
