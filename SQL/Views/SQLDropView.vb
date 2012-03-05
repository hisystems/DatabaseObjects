' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLDropView
        Inherits SQLStatement

        Private pstrName As String

        Public Sub New(ByVal strViewName As String)

            If String.IsNullOrEmpty(strViewName) Then
                Throw New ArgumentNullException()
            End If

            pstrName = strViewName

        End Sub

        Public Overrides ReadOnly Property SQL() As String
            Get

                Return _
                    "DROP VIEW " & _
                    SQLConvertIdentifierName(pstrName, Me.ConnectionType) 

            End Get
        End Property

    End Class

End Namespace
