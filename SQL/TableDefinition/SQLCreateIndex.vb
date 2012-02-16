
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLCreateIndex
        Inherits SQLStatement

        Private pstrName As String
        Private pstrTableName As String
        Private pbIsUnique As Boolean
        Private pobjFields As SQLIndexFields = New SQLIndexFields

        Public Sub New()

        End Sub

        Public Sub New(ByVal strIndexName As String, ByVal strTableName As String, ByVal strFieldNames() As String)

            Me.Name = strIndexName
            Me.TableName = strTableName

            For Each strFieldName As String In strFieldNames
                Me.Fields.Add(strFieldName)
            Next

        End Sub

        Public Sub New(ByVal strIndexName As String, ByVal strTableName As String, ByVal strFieldNames() As String, ByVal bIsUnique As Boolean)

            Me.New(strIndexName, strTableName, strFieldNames)
            pbIsUnique = bIsUnique

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public Property TableName() As String
            Get

                Return pstrTableName

            End Get

            Set(ByVal Value As String)

                pstrTableName = Value.Trim

            End Set
        End Property

        Public Property IsUnique() As Boolean
            Get

                Return pbIsUnique

            End Get

            Set(ByVal Value As Boolean)

                pbIsUnique = Value

            End Set
        End Property

        Public ReadOnly Property Fields() As SQLIndexFields
            Get

                Return pobjFields

            End Get
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String = String.Empty

                'Although the index name is optional with SQL Server it is not optional with MySQL or Pervasive
                If Me.Name = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("IndexName has not been set.")
                End If

                If Me.TableName = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName has not been set.")
                End If

                strSQL = _
                    "CREATE " & UniqueString() & "INDEX " & _
                    SQLConvertIdentifierName(Me.Name, Me.ConnectionType) & " ON " & _
                    SQLConvertIdentifierName(Me.TableName, Me.ConnectionType) & _
                    " (" & pobjFields.SQL(Me.ConnectionType) & ")"

                Return strSQL

            End Get
        End Property

        Private Function UniqueString() As String

            If pbIsUnique Then
                Return "UNIQUE "
            Else
                Return ""
            End If

        End Function

    End Class


    Public Class SQLIndexFields
        Implements System.Collections.Generic.IEnumerable(Of SQLIndexField)

        Private pobjFields As New Collections.Generic.List(Of SQLIndexField)

        Friend Sub New()

        End Sub

        Public Function Add() As SQLIndexField

            Return Add("", OrderBy.Ascending)

        End Function

        Public Function Add( _
            ByVal strFieldName As String) As SQLIndexField

            Return Add(strFieldName, OrderBy.Ascending)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eOrder As OrderBy) As SQLIndexField

            Dim objField As SQLIndexField = New SQLIndexField

            With objField
                .Name = strFieldName
                .Order = eOrder
            End With

            pobjFields.Add(objField)
            Add = objField

        End Function

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Const cstrSeperator As String = ", "

                Dim strSQL As String = String.Empty

                For Each objField As SQLIndexField In pobjFields
                    strSQL &= objField.SQL(eConnectionType) & cstrSeperator
                Next

                Return strSQL.Substring(0, strSQL.Length - cstrSeperator.Length)      'remove the last comma and space

            End Get
        End Property

        Private Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of SQLIndexField) Implements System.Collections.Generic.IEnumerable(Of SQLIndexField).GetEnumerator

            Return pobjFields.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjFields.GetEnumerator

        End Function

    End Class



    Public Class SQLIndexField

        Private pstrName As String
        Private peOrder As OrderBy

        Friend Sub New()

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public Property Order() As OrderBy
            Get

                Return peOrder

            End Get

            Set(ByVal Value As OrderBy)

                peOrder = Value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Return SQLConvertIdentifierName(Me.Name, eConnectionType) & OrderString()

            End Get
        End Property

        Private Function OrderString() As String

            If peOrder = OrderBy.Descending Then
                Return " DESC"
            Else
                Return ""
            End If

        End Function

    End Class

End Namespace
