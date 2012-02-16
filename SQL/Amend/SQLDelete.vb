
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLDelete
        Inherits SQLStatement

        Private pobjConditions As SQLConditions = New SQLConditions
        Private pstrTableName As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal strTableName As String)

            Me.TableName = strTableName

        End Sub

        Public Sub New(ByVal strTableName As String, ByVal objWhereCondition As SQLCondition)

            Me.TableName = strTableName
            Me.Where.Add(objWhereCondition)

        End Sub

        Public Property TableName() As String
            Get

                Return pstrTableName

            End Get

            Set(ByVal Value As String)

                pstrTableName = Value

            End Set
        End Property

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

                TableName = TableName.Trim()
                If TableName = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName property has not been set.")
                End If

                strSQL = "DELETE FROM " & SQLConvertIdentifierName(Me.TableName, Me.ConnectionType)

                If Not pobjConditions Is Nothing AndAlso Not pobjConditions.IsEmpty Then
                    strSQL &= " WHERE " & pobjConditions.SQL(Me.ConnectionType)
                End If

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
