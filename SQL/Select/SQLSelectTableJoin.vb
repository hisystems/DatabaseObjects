
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTableJoin

        Public Enum Type
            Inner
            FullOuter
            LeftOuter
            RightOuter
        End Enum

        Private pobjParent As SQLSelectTableJoins
        Private pobjLeftTable As SQLSelectTableBase
        Private pobjRightTable As SQLSelectTableBase
        Private pobjConditions As SQLSelectTableJoinConditions
        Private peType As SQLSelectTableJoin.Type

        Friend Sub New(ByVal objParent As SQLSelectTableJoins)

            MyBase.New()
            pobjParent = objParent
            pobjConditions = New SQLSelectTableJoinConditions(Me)

        End Sub

        Property TheType() As SQLSelectTableJoin.Type
            Get

                Return peType

            End Get

            Set(ByVal Value As SQLSelectTableJoin.Type)

                peType = Value

            End Set
        End Property

        Public Property LeftTable() As SQLSelectTableBase
            Get
                LeftTable = pobjLeftTable
            End Get

            Set(ByVal Value As SQLSelectTableBase)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjLeftTable = Value

            End Set
        End Property

        Public Property RightTable() As SQLSelectTableBase
            Get
                Return pobjRightTable
            End Get

            Set(ByVal Value As SQLSelectTableBase)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjRightTable = Value

            End Set
        End Property

        Public Property Where() As SQLSelectTableJoinConditions
            Get

                Return pobjConditions

            End Get

            Set(ByVal Value As SQLSelectTableJoinConditions)

                pobjConditions = Value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty
                Dim strJoin As String = String.Empty

                Select Case Me.TheType
                    Case SQLSelectTableJoin.Type.Inner
                        strJoin = "INNER JOIN"
                    Case SQLSelectTableJoin.Type.FullOuter
                        strJoin = "FULL OUTER JOIN"
                    Case SQLSelectTableJoin.Type.LeftOuter
                        strJoin = "LEFT OUTER JOIN"
                    Case SQLSelectTableJoin.Type.RightOuter
                        strJoin = "RIGHT OUTER JOIN"
                End Select

                If Index() = 0 Then
                    strSQL = pobjLeftTable.SQL(eConnectionType)
                Else
                    strSQL = pobjParent(Index() - 1).SQL(eConnectionType)
                End If

                strSQL &= " " & strJoin & " " & pobjRightTable.SQL(eConnectionType)

                If Not pobjConditions Is Nothing AndAlso Not pobjConditions.IsEmpty Then
                    strSQL &= " ON " & pobjConditions.SQL(eConnectionType)
                End If

                'Surround the join with parentheses - MS Access won't accept it otherwise
                Return "(" & strSQL & ")"

            End Get
        End Property

        Private Function Index() As Integer

            For intIndex As Integer = 0 To pobjParent.Count - 1
                If pobjParent(intIndex) Is Me Then
                    Return intIndex
                End If
            Next

        End Function

    End Class

End Namespace
