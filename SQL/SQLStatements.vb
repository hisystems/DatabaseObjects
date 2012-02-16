

' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace SQL

    Public Class SQLStatements
        Inherits SQL.SQLStatement
        Implements Collections.Generic.IEnumerable(Of ISQLStatement)

        Private pobjStatements As New Collections.Generic.List(Of SQL.ISQLStatement)

        Public Sub New()

        End Sub

        Public Sub New(ByVal objStatements As SQL.ISQLStatement())

            If objStatements Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjStatements.AddRange(objStatements)

        End Sub

        Public Sub Add(ByVal objStatement As ISQLStatement)

            pobjStatements.Add(objStatement)

        End Sub

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String = String.Empty

                For Each objStatement As SQL.ISQLStatement In pobjStatements
                    strSQL &= objStatement.SQL & "; "
                Next

                Return strSQL

            End Get
        End Property

        Private Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ISQLStatement) Implements System.Collections.Generic.IEnumerable(Of ISQLStatement).GetEnumerator

            Return pobjStatements.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjStatements.GetEnumerator

        End Function

    End Class

End Namespace