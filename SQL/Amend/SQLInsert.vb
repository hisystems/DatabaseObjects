
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLInsert
        Inherits SQLStatement

        Private pobjFields As SQLFieldValues = New SQLFieldValues
        Private pstrTableName As String = String.Empty

        Public Sub New()

        End Sub

        Public Property TableName() As String
            Get

                Return pstrTableName

            End Get

            Set(ByVal Value As String)

                pstrTableName = Value

            End Set
        End Property

        Public Property Fields() As SQLFieldValues
            Get

                Return pobjFields

            End Get

            Set(ByVal Value As SQLFieldValues)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjFields = Value

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strFields As String = String.Empty
                Dim strFieldValues As String = String.Empty

                TableName = TableName.Trim()
                If TableName = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName property has not been set.")
                End If

                If pobjFields.Count = 0 Then
                    Throw New Exceptions.DatabaseObjectsException("Field values have not been set.")
                End If

                For intIndex As Integer = 0 To pobjFields.Count - 1
                    strFields &= SQLConvertIdentifierName(pobjFields(intIndex).Name, Me.ConnectionType)
                    If intIndex <> pobjFields.Count - 1 Then
                        strFields &= ","
                    End If
                Next

                'For intIndex As Integer = 0 To pobjFields.Count - 1
                '    strFieldValues &= SQLConvertValue(pobjFields(intIndex).Value, Me.ConnectionType)
                '    If intIndex <> pobjFields.Count - 1 Then
                '        strFieldValues &= ","
                '    End If
                'Next

                Dim objField As SQLFieldValue

                For intIndex As Integer = 0 To pobjFields.Count - 1
                    objField = pobjFields(intIndex)

                    If TypeOf objField.Value Is SQLExpression Then
                        strFieldValues &= DirectCast(objField.Value, SQLExpression).SQL(Me.ConnectionType)
                    Else
                        strFieldValues &= SQLConvertValue(pobjFields(intIndex).Value, Me.ConnectionType)
                    End If

                    If intIndex <> pobjFields.Count - 1 Then
                        strFieldValues &= ","
                    End If
                Next

                Return _
                    "INSERT INTO " & SQLConvertIdentifierName(Me.TableName, Me.ConnectionType) & " " & _
                    "(" & strFields & ") VALUES (" & strFieldValues & ")"

            End Get
        End Property

    End Class

End Namespace
