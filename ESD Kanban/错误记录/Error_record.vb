Public Class Error_record
    Public Sub New()
        '空构造
    End Sub

    ''' <summary>
    ''' 记录错误信息
    ''' </summary>
    ''' <param name="ErrString">错误信息</param>
    ''' <param name="StackTrace">详细描述</param>
    Public Sub New(ByVal ErrString As String, ByVal StackTrace As String)
        GetErrString = ErrString
        GetStackTrace = StackTrace
    End Sub


    ''' <summary>
    ''' 错误信息
    ''' </summary>
    ''' <returns></returns>
    Public Shared Property GetErrString As String

    ''' <summary>
    ''' 详细的错误信息
    ''' </summary>
    ''' <returns></returns>
    Public Shared Property GetStackTrace As String


    ''' <summary>
    ''' 记录错误信息
    ''' </summary>
    ''' <param name="ErrString">错误信息</param>
    ''' <param name="StackTrace">详细描述</param>
    Public Shared Sub Geterr(ByVal ErrString As String, ByVal StackTrace As String)
        GetErrString = ErrString
        GetStackTrace = StackTrace
    End Sub

End Class
