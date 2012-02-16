
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLConditionSelect

        'This class allows a conditional value generated from an SELECT statement to be added as an SQL condition.
        'i.e.  ... WHERE (SELECT MAX(StockOnHand) FROM Product WHERE Supplier.ProductID = Product.ProductID) > 1000

        Private pobjValue As Object
        Private pobjSelect As SQLSelect
        Private peCompare As ComparisonOperator

        Public Sub New()

        End Sub

        Public Property [Select]() As SQLSelect
            Get

                Return pobjSelect

            End Get

            Set(ByVal Value As SQLSelect)

                pobjSelect = Value

            End Set
        End Property

        Public Property Compare() As ComparisonOperator
            Get

                Return peCompare

            End Get

            Set(ByVal Value As ComparisonOperator)

                peCompare = Value

            End Set
        End Property

        Public Property Value() As Object
            Get

                Value = pobjValue

            End Get

            Set(ByVal Value As Object)

                pobjValue = SQLConditionValue(Value)

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                If Me.Select Is Nothing Then
                    Throw New Exceptions.DatabaseObjectsException("Select is not set.")
                End If

                CompareValuePairAssertValid(Me.Compare, pobjValue)

                Me.Select.ConnectionType = eConnectionType
                Return Condition(Me.Select, Me.Compare, pobjValue, eConnectionType)

            End Get
        End Property

        Private Function Condition( _
            ByVal objSelect As SQLSelect, _
            ByVal eCompare As ComparisonOperator, _
            ByVal vValue As Object, _
            ByVal eConnectionType As Database.ConnectionType) As String

            SQLConvertBooleanValue(vValue, eCompare)

            Return _
                "(" & objSelect.SQL & ") " & _
                SQLConvertCompare(eCompare) & " " & _
                SQLConvertValue(vValue, eConnectionType)

        End Function

    End Class

End Namespace
