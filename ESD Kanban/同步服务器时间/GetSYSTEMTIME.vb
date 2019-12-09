Imports System.Runtime.InteropServices

''' <summary>
''' 设置系统时间
''' </summary>
Public Class GetSYSTEMTIME
    ''' <summary>
    ''' 同步时间结构
    ''' </summary>
    Public Structure SYSTEMTIME
        Public wYear As UShort
        Public wMonth As UShort
        Public wDayOfWeek As UShort
        Public wDay As UShort
        Public wHour As UShort
        Public wMinute As UShort
        Public wSecond As UShort
        Public wMilliseconds As UShort

        Public Sub FromDateTime(ByVal time As DateTime)
            wYear = CUShort(time.Year)
            wMonth = CUShort(time.Month)
            wDayOfWeek = CUShort(time.DayOfWeek)
            wDay = CUShort(time.Day)
            wHour = CUShort(time.Hour)
            wMinute = CUShort(time.Minute)
            wSecond = CUShort(time.Second)
            wMilliseconds = CUShort(time.Millisecond)
        End Sub

        Public Function ToDateTime() As DateTime
            Return New DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds)
        End Function

        Public Shared Function ToDateTime(ByVal time As SYSTEMTIME) As DateTime
            Return time.ToDateTime()
        End Function



    End Structure

    <DllImport("Kernel32.dll")>
    Public Shared Function SetLocalTime(ByRef Time As SYSTEMTIME) As Boolean
    End Function
    <DllImport("Kernel32.dll")>
    Public Shared Sub GetLocalTime(ByRef Time As SYSTEMTIME)
    End Sub

    ''' <summary>
    ''' 同步服务器时间
    ''' </summary>
    Public Shared Sub Synchronization_time()
        Try
            '同步服务器时间
            Dim t As DateTime = DateTime.Now
            t = t.AddDays(7)
            t = DateTime.Parse(Getdate)
            Dim st As SYSTEMTIME = New SYSTEMTIME()
            st.FromDateTime(t)
            If SetLocalTime(st) = False Then
                log("时间同步失败!")
            End If
        Catch ex As Exception
            log("同步时间错误-" & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub


End Class


