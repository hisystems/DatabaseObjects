
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Explicit On 
Option Strict On

Namespace SQL

    Public Class SQLConditionFieldCompare

        Private pobjTable1 As SQLSelectTable
        Private pstrFieldName1 As String
        Private peCompare As ComparisonOperator
        Private pobjTable2 As SQLSelectTable
        Private pstrFieldName2 As String

        Public Property Table1() As SQLSelectTable
            Get

                Return pobjTable1

            End Get

            Set(ByVal Value As SQLSelectTable)

                pobjTable1 = Value

            End Set
        End Property

        Public Property FieldName1() As String
            Get

                Return pstrFieldName1

            End Get

            Set(ByVal Value As String)

                pstrFieldName1 = Value

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

        Public Property Table2() As SQLSelectTable
            Get

                Return pobjTable2

            End Get

            Set(ByVal Value As SQLSelectTable)

                pobjTable2 = Value

            End Set
        End Property

        Public Property FieldName2() As String
            Get

                Return pstrFieldName2

            End Get

            Set(ByVal Value As String)

                pstrFieldName2 = Value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                If FieldName1 = String.Empty Then
                    Throw New ArgumentException("FieldName1 not set.")
                End If

                If FieldName2 = String.Empty Then
                    Throw New ArgumentException("FieldName2 not set.")
                End If

                Return _
                    SQLFieldNameAndTablePrefix(Me.Table1, Me.FieldName1, eConnectionType) & " " & _
                    SQLConvertCompare(Me.Compare) & " " & _
                    SQLFieldNameAndTablePrefix(Me.Table2, Me.FieldName2, eConnectionType)

            End Get
        End Property

    End Class

End Namespace
