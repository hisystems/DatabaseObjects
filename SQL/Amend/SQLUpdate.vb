
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLUpdate
        Inherits SQLStatement

        Private pobjFields As New SQLUpdateFields
        Private pobjConditions As New SQLConditions
        Private pstrTableName As String = String.Empty

        Public Sub New()

        End Sub

        Public Sub New(ByVal strTableName As String)

            Me.TableName = strTableName

        End Sub

        Public Sub New(ByVal strTableName As String, ByVal objValue As SQLFieldValue, ByVal objWhere As SQLCondition)

            Me.TableName = strTableName
            Me.Fields.Add(objValue)
            Me.Where.Add(objWhere)

        End Sub

        Public Property TableName() As String
            Get

                Return pstrTableName

            End Get

            Set(ByVal Value As String)

                pstrTableName = Value

            End Set
        End Property

        Public Property Fields() As SQLUpdateFields
            Get

                Return pobjFields

            End Get

            Set(ByVal Value As SQLUpdateFields)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjFields = Value

            End Set
        End Property

        '''' <summary>
        '''' Allows fields to be set to arithmetic operations or other fields.
        '''' </summary>
        'Public Property UpdateFields() As SQLUpdateFields
        '    Get

        '        Return pobjUpdateFields

        '    End Get

        '    Set(ByVal Value As SQLUpdateFields)

        '        If Value Is Nothing Then
        '            Throw New ArgumentNullException
        '        End If

        '        pobjUpdateFields = Value

        '    End Set
        'End Property

        Public Property Where() As SQLConditions
            Get

                Return pobjConditions

            End Get

            Set(ByVal Value As SQLConditions)

                pobjConditions = Value

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String
                Dim strFieldValues As String = String.Empty

                If TableName.Trim = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName property has not been set.")
                ElseIf pobjFields.Count = 0 Then
                    Throw New Exceptions.DatabaseObjectsException("Field values have not been set.")
                End If

                Dim objField As SQLFieldValue

                For intIndex As Integer = 0 To pobjFields.Count - 1
                    objField = pobjFields(intIndex)
                    'Check the field name has been set. Can't really check whether the value has been set or not.
                    If objField.Name = String.Empty Then
                        Throw New Exceptions.DatabaseObjectsException("Field Name has not been set.")
                    End If

                    If TypeOf objField Is SQLUpdateField Then
                        strFieldValues &= SQLConvertIdentifierName(objField.Name, Me.ConnectionType) & " = " & DirectCast(objField, SQLUpdateField).Value.SQL(Me.ConnectionType)
                    ElseIf TypeOf objField Is SQLFieldValue Then
                        strFieldValues &= SQLConvertIdentifierName(objField.Name, Me.ConnectionType) & " = " & SQLConvertValue(objField.Value, Me.ConnectionType)
                    Else
                        Throw New NotSupportedException(objField.GetType.Name)
                    End If

                    If intIndex <> pobjFields.Count - 1 Then
                        strFieldValues &= ", "
                    End If
                Next

                strSQL = _
                    "UPDATE " & SQLConvertIdentifierName(Me.TableName.Trim, Me.ConnectionType) & " " & _
                    "SET " & strFieldValues

                If Not pobjConditions Is Nothing AndAlso Not pobjConditions.IsEmpty Then
                    strSQL &= " WHERE " & pobjConditions.SQL(Me.ConnectionType)
                End If

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
