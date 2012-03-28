
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTableJoinCondition

        Private pobjLeftExpression As SQLExpression
        Private peCompare As ComparisonOperator
        Private pobjRightExpression As SQLExpression
        Private pobjParent As SQLSelectTableJoinConditions

        Friend Sub New(ByVal objParent As SQLSelectTableJoinConditions)

            pobjParent = objParent

        End Sub

        Public Property RightExpression() As SQLExpression
            Get

                Return pobjRightExpression

            End Get

            Set(ByVal Value As SQLExpression)

                pobjRightExpression = Value

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

        Public Property LeftExpression() As SQLExpression
            Get

                Return pobjLeftExpression

            End Get

            Set(ByVal Value As SQLExpression)

                pobjLeftExpression = Value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                'Account for the situation where EqualTo to NULL is appropriately translated to 'IS NULL'
                If TypeOf pobjRightExpression Is SQLValueExpression Then
                    Return _
                        pobjLeftExpression.SQL(eConnectionType) & " " & _
                        SQLConvertCondition(Compare, DirectCast(pobjRightExpression, SQLValueExpression).Value, eConnectionType)
                Else
                    Return _
                        pobjLeftExpression.SQL(eConnectionType) & " " & _
                        SQLConvertCompare(Compare) & " " & _
                        pobjRightExpression.SQL(eConnectionType)
                End If

            End Get
        End Property

    End Class

End Namespace
