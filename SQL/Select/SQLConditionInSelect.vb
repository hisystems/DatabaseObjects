
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Explicit On 
Option Strict On

Namespace SQL

    Public Class SQLConditionInSelect

        'This class represents an IN operation together with a SELECT statement
        'i.e.  ... ProductID IN (SELECT ProductID FROM Product WHERE ...)

        Private pobjTable As SQLSelectTable
        Private pstrFieldName As String
        Private pobjSelect As SQLSelect
        Private pbNotInSelect As Boolean

        Public Property Table() As SQLSelectTable
            Get

                Return pobjTable

            End Get

            Set(ByVal Value As SQLSelectTable)

                pobjTable = Value

            End Set
        End Property

        Public Property FieldName() As String
            Get

                Return pstrFieldName

            End Get

            Set(ByVal Value As String)

                pstrFieldName = Value

            End Set
        End Property

        Public Property [Select]() As SQLSelect
            Get

                Return pobjSelect

            End Get

            Set(ByVal Value As SQLSelect)

                pobjSelect = Value

            End Set
        End Property

        Public Property NotInSelect() As Boolean
            Get

                Return pbNotInSelect

            End Get

            Set(ByVal value As Boolean)

                pbNotInSelect = value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                If FieldName = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("FieldName not set.")
                End If

                If [Select] Is Nothing Then
                    Throw New Exceptions.DatabaseObjectsException("SelectSet not set.")
                End If

                [Select].ConnectionType = eConnectionType

                Dim strIn As String

                If pbNotInSelect Then
                    strIN = "NOT IN"
                Else
                    strIN = "IN"
                End If

                Return _
                    SQLFieldNameAndTablePrefix(Me.Table, Me.FieldName, eConnectionType) & _
                    " " & strIn & " (" & [Select].SQL & ")"

            End Get
        End Property

    End Class

End Namespace
