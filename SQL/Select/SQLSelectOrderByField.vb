
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectOrderByField

        Private pstrName As String
        Private pobjTable As SQLSelectTable
        Private peOrder As OrderBy = OrderBy.Ascending
        Private peAggregateFunction As SQL.AggregateFunction = Global.DatabaseObjects.SQL.AggregateFunction.None

        Friend Sub New()

            MyBase.New()

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

        Public Sub OrderingReverse()

            If peOrder <> OrderBy.None Then
                peOrder = Not peOrder
            End If

        End Sub

        Public Property AggregateFunction() As SQL.AggregateFunction
            Get

                Return peAggregateFunction

            End Get

            Set(ByVal Value As SQL.AggregateFunction)

                peAggregateFunction = Value

            End Set
        End Property

        Public Property Table() As SQLSelectTable
            Get

                Return pobjTable

            End Get

            Set(ByVal Value As SQLSelectTable)

                pobjTable = Value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty

                If Me.Name = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("Order By field has not been set.")
                End If

                If Me.AggregateFunction > 0 Then
                    strSQL = SQLConvertAggregate(Me.AggregateFunction) & "("
                End If

                strSQL &= SQLFieldNameAndTablePrefix(Me.Table, Me.Name, eConnectionType)

                If Me.AggregateFunction > 0 Then
                    strSQL &= ")"
                End If

                Select Case Me.Order
                    Case OrderBy.Ascending
                    Case OrderBy.Descending
                        strSQL &= " DESC"
                End Select

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
