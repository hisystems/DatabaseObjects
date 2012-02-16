
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLCondition

        Private pstrFieldName As String = String.Empty
        Private pobjValue As Object
        Private peCompare As ComparisonOperator
        Private pobjTable As SQLSelectTable

        Friend Sub New()

        End Sub

        Public Sub New( _
            ByVal strFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object)

            Me.FieldName = strFieldName
            Me.Compare = eCompare
            pobjValue = objValue

        End Sub

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

                CompareValuePairAssertValid(Me.Compare, pobjValue)

                Return _
                    SQLFieldNameAndTablePrefix(Me.Table, Me.FieldName, eConnectionType) & " " & _
                    SQLConvertCondition(Me.Compare, Me.Value, eConnectionType)

            End Get
        End Property

    End Class

End Namespace
