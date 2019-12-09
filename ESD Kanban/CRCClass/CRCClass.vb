Imports System.Text

Public Class CRCClass

    '* 
    '        * 16进制char转换成整型 
    '        *  
    '        * @param c 
    '        * @return 
    '        

    Public Shared Function parse(c As Char) As Integer
        If c >= "a" Then
            Return (Convert.ToInt32(c) - Convert.ToInt32("a") + 10) And &HF

        End If
        If c >= "A" Then
            Return (Convert.ToInt32(c) - Convert.ToInt32(65) + 10) And &HF
        End If
        Return (Convert.ToInt32(c) - Convert.ToInt32("0")) And &HF
    End Function



    ''' <summary>
    ''' 十六进制字符串转换成字节数组
    ''' </summary>
    ''' <param name="hexstr"></param>
    ''' <returns></returns>
    Public Shared Function HexString2Bytes(hexstr As String) As Byte()
        'Dim bytSj() As Byte
        'Dim i As Integer
        'bytSj = StrConv(hexstr, vbFromUnicode)
        'For i = 0 To UBound(bytSj)
        '    strHexSj = strHexSj & Right("0" & Hex(bytSj(i)), 2)
        'Next
        Dim str() As String = hexstr.Split(" ")
        Dim strhex As String = ""
        For index = 0 To str.Length - 1

            strhex += str(index)
        Next



        Dim b As Byte() = New Byte(strhex.Length / 2 - 1) {}
        For index = 0 To b.Length - 1
            If str(index) = " "c Then
                b(index) = 0
            Else
                b(index) = CByte("&H" & str(index))

                'b(index) = CLng("&H" & str(index))
            End If

        Next








        'Dim b As Byte() = New Byte(hexstr.Length / 2) {}
        'Dim j As Integer = 0
        'Dim i As Integer = 0
        'While i < b.Length
        '    Dim c0 As Char = hexstr(j + 1)
        '    Dim c1 As Char = hexstr(j + 1)
        '    b(i) = CType((parse(c0) << 4) Or parse(c1), Byte)
        '    i += 1
        'End While
        Return b
    End Function




    ''' <summary>
    '''字节数组转为十六进制字符串
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    Public Shared Function ByteArrayToHexString(data As Byte()) As String
        '字节数组转为十六进制字符串  
        Dim sb As New StringBuilder(data.Length * 3)
        For Each b As Byte In data
            sb.Append(Convert.ToString(b, 16).PadLeft(2, "0"c).PadRight(3, " "c))
        Next
        Return sb.ToString().ToUpper()
    End Function


    ''' <summary>
    ''' CRC计算
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    Public Shared Function CRCCalc(data As String) As String
        Dim datas As String() = data.Split(" ")
        Dim bytedata As New List(Of Byte)()

        For Each str As String In datas
            bytedata.Add(Byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier))
        Next
        Dim crcbuf As Byte() = bytedata.ToArray()
        '计算并填写CRC校验码
        Dim crc As Integer = &HFFFF
        Dim len As Integer = crcbuf.Length
        Dim n As Integer = 0
        While n < len
            Dim i As Byte
            crc = crc Xor crcbuf(n)
            i = 0
            While i < 8
                Dim TT As Integer
                TT = crc And 1
                crc = crc >> 1
                crc = crc And &H7FFF
                If TT = 1 Then
                    crc = crc Xor &HA001
                End If
                crc = crc And &HFFFF
                i += 1

            End While
            n += 1
        End While
        Dim redata As String() = New String(1) {}
        redata(1) = Convert.ToString(CType((crc >> 8) And &HFF, Byte), 16)
        redata(0) = Convert.ToString(CType((crc And &HFF), Byte), 16)
        redata(0).PadLeft(2, "0")
        Return redata(0).PadLeft(2, "0").ToUpper() + " " + redata(1).PadLeft(2, "0").ToUpper()
    End Function


End Class



