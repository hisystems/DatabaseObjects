
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

        Private pstrLeftTableFieldName As String
        Private peCompare As ComparisonOperator
        Private pstrRightTableFieldName As String
        Private pobjParent As SQLSelectTableJoinConditions

        Friend Sub New(ByVal objParent As SQLSelectTableJoinConditions)

            pobjParent = objParent

        End Sub

        Public Property RightTableFieldName() As String
            Get

                Return pstrRightTableFieldName

            End Get

            Set(ByVal Value As String)

                pstrRightTableFieldName = Value

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

        Public Property LeftTableFieldName() As String
            Get

                Return pstrLeftTableFieldName

            End Get

            Set(ByVal Value As String)

                pstrLeftTableFieldName = Value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String

                If LeftTableFieldName.Trim = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("LeftTableFieldName has not been specified.")
                End If

                If RightTableFieldName.Trim = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("RightTableFieldName has not been specified.")
                End If

                strSQL = _
                    SQLFieldNameAndTablePrefix(pobjParent.Parent.LeftTable, LeftTableFieldName, eConnectionType) & " " & _
                    SQLConvertCompare(Compare) & " " & SQLFieldNameAndTablePrefix(pobjParent.Parent.RightTable, RightTableFieldName, eConnectionType)

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
