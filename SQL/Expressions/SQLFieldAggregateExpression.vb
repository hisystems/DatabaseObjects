
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLFieldAggregateExpression
        Inherits SQLFieldExpression

        Private peAggregateFunction As AggregateFunction = AggregateFunction.None

        Public Sub New()


        End Sub

        Public Sub New(ByVal strFieldName As String)

            MyBase.New(strFieldName)

        End Sub

        Public Sub New(ByVal objTable As SQLSelectTable, ByVal strFieldName As String)

            MyBase.New(objTable, strFieldName)

        End Sub

        Public Sub New(ByVal objTable As SQLSelectTable, ByVal strFieldName As String, ByVal eAggregate As AggregateFunction)

            MyBase.New(objTable, strFieldName)
            Me.AggregateFunction = eAggregate

        End Sub

        Public Property AggregateFunction() As SQL.AggregateFunction
            Get

                Return peAggregateFunction

            End Get

            Set(ByVal Value As SQL.AggregateFunction)

                peAggregateFunction = Value

            End Set
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            If Me.AggregateFunction = AggregateFunction.None Then
                Throw New InvalidOperationException("AggregateFunction unspecified for " & Me.GetType.Name)
            End If

            Dim strFieldName As String = String.Empty

            If Me.AggregateFunction = AggregateFunction.Count Then
                strFieldName = "*"
            Else
                strFieldName = MyBase.SQL(eConnectionType)
            End If

            Return SQLConvertAggregate(Me.AggregateFunction) & "(" & strFieldName & ")"

        End Function

    End Class

End Namespace
