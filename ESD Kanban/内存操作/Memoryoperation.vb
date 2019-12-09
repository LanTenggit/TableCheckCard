Imports System.Runtime.InteropServices

Public Class Memoryoperation
    <DllImport("kernel32.dll", ExactSpelling:=True)>
    Private Shared Function GetCurrentProcess() As IntPtr
    End Function
    <DllImport("advapi32.dll", ExactSpelling:=True, SetLastError:=True)>
    Private Shared Function OpenProcessToken(ByVal h As IntPtr, ByVal acc As Integer, ByRef phtok As IntPtr) As Boolean
    End Function
    Private Const TOKEN_QUERY As Integer = &H8
    Private Const TOKEN_ADJUST_PRIVILEGES As Integer = &H20
    <DllImportAttribute("kernel32.dll", EntryPoint:="ReadProcessMemory")>
    Public Shared Function ReadProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As IntPtr, ByVal nSize As Integer, ByVal lpNumberOfBytesRead As IntPtr) As Boolean
    End Function
    <DllImportAttribute("kernel32.dll", EntryPoint:="OpenProcess")>
    Public Shared Function OpenProcess(ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr
    End Function
    <DllImport("kernel32.dll")>
    Private Shared Sub CloseHandle(ByVal hObject As IntPtr)
    End Sub
    ''' <summary>
    ''' 写内存
    ''' </summary>
    ''' <param name="hProcess"></param>
    ''' <param name="lpBaseAddress"></param>
    ''' <param name="lpBuffer"></param>
    ''' <param name="nSize"></param>
    ''' <param name="lpNumberOfBytesWritten"></param>
    ''' <returns></returns>
    <DllImportAttribute("kernel32.dll", EntryPoint:="WriteProcessMemory")>
    Public Shared Function WriteProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Integer(), ByVal nSize As Integer, ByVal lpNumberOfBytesWritten As IntPtr) As Boolean
    End Function

    ''' <summary>
    ''' 获取窗体的进程标识ID
    ''' </summary>
    ''' <param name="windowTitle"></param>
    ''' <returns></returns>
    Public Function GetPid(ByVal windowTitle As String) As Integer
        Dim rs As Integer = 0
        Dim arrayProcess As Process() = Process.GetProcesses()

        For Each p As Process In arrayProcess

            If p.MainWindowTitle.IndexOf(windowTitle) <> -1 Then
                rs = p.Id
                Exit For
            End If
        Next
        Return rs
    End Function

    ''' <summary>
    ''' 根据进程名获取PID
    ''' </summary>
    ''' <param name="processName"></param>
    ''' <returns></returns>
    Public Function GetPidByProcessName(ByVal processName As String) As Integer
        Dim arrayProcess As Process() = Process.GetProcessesByName(processName)

        For Each p As Process In arrayProcess
            Return p.Id
        Next
        Return 0
    End Function

    ''' <summary>
    ''' 根据窗体标题查找窗口句柄（支持模糊匹配）
    ''' </summary>
    ''' <param name="title"></param>
    ''' <returns></returns>
    Public Function FindWindow(ByVal title As String) As IntPtr
        Dim ps As Process() = Process.GetProcesses()

        For Each p As Process In ps

            If p.MainWindowTitle.IndexOf(title) <> -1 Then
                Return p.MainWindowHandle
            End If
        Next
        Return IntPtr.Zero
    End Function

    ''' <summary>
    ''' 读取内存中的值
    ''' </summary>
    ''' <param name="baseAddress">需要读取的地址</param>
    ''' <param name="processName">进程名</param>
    ''' <returns>返回 值</returns>
    Public Function ReadMemoryValue(ByVal baseAddress As Int64, ByVal processName As String) As Int64
        Try
            Dim buffer As Byte() = New Byte(3) {}
            Dim byteAddress As IntPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0)
            Dim hProcess As IntPtr = OpenProcess(&H1F0FFF, False, GetPidByProcessName(processName))
            ReadProcessMemory(hProcess, CType(baseAddress, IntPtr), byteAddress, 4, IntPtr.Zero)
            CloseHandle(hProcess)
            Return Marshal.ReadInt64(byteAddress)
        Catch
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' 读取内存中的值
    ''' </summary>
    ''' <param name="baseAddress">需要读取的地址</param>
    ''' <param name="Pid">进程PID</param>
    ''' <returns>返回 值</returns>
    Public Function ReadMemoryValue(ByVal baseAddress As Int64, ByVal Pid As Integer) As Int64
        Try
            Dim buffer As Byte() = New Byte(3) {}
            Dim byteAddress As IntPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0)
            Dim hProcess As IntPtr = OpenProcess(&H1F0FFF, False, Pid)
            ReadProcessMemory(hProcess, CType(baseAddress, IntPtr), byteAddress, 4, IntPtr.Zero)
            CloseHandle(hProcess)
            Return Marshal.ReadInt64(byteAddress)
        Catch ex As System.Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' 将值写入指定内存地址中
    ''' </summary>
    ''' <param name="baseAddress">需要写入的地址</param>
    ''' <param name="processName">进程名</param>
    ''' <param name="value">写入值</param>
    Public Sub WriteMemoryValue(ByVal baseAddress As Int64, ByVal processName As String, ByVal value As Integer)
        Try
            Dim hProcess As IntPtr = OpenProcess(&H1F0FFF, False, GetPidByProcessName(processName))
            WriteProcessMemory(hProcess, CType(baseAddress, IntPtr), New Integer() {value}, 4, IntPtr.Zero)
            CloseHandle(hProcess)
        Catch ex As System.Exception
        End Try
    End Sub

    ''' <summary>
    ''' 将值写入指定内存地址中
    ''' </summary>
    ''' <param name="baseAddress">需要写入的地址</param>
    ''' <param name="Pid">进程PID</param>
    ''' <param name="value">写入值</param>
    Public Sub WriteMemoryValue(ByVal baseAddress As Int64, ByVal Pid As Integer, ByVal value As Integer)
        Try
            Dim hProcess As IntPtr = OpenProcess(&H1F0FFF, False, Pid)
            WriteProcessMemory(hProcess, CType(baseAddress, IntPtr), New Integer() {value}, 4, IntPtr.Zero)
            CloseHandle(hProcess)
        Catch ex As System.Exception
        End Try
    End Sub
End Class

'//调用就不做演示了，应该看的很明白，再写几个读写内存常用的方法

'//如果是读写64位的程序，记得自身程序必须是64位，或者是AnyCpu(自动适应位数)，然后加入以下代码，一般调用一次即可
'IntPtr hproc = GetCurrentProcess();
'IntPtr htok = IntPtr.Zero;
'If (!OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok))
'{
'    Throw New Exception("Open Process Token fail");
'}

'//将字符串转成16进制整形 - 此字符串必须是表示十六进制的值
'Int64 realstr = Convert.ToInt64(“123DFC”, 16);

'//十进制转16进制字符串 - 把150转成16进制
'String Address = Convert.ToString(150, 16)