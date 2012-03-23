
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
        Inherits SQLSelectTableBase

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

        Protected Overrides ReadOnly Property Source(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = _
                    pobjLeftTable.SQL(eConnectionType) & " " & GetJoinString(Me.TheType) & " " & pobjRightTable.SQL(eConnectionType)

                If Not pobjConditions Is Nothing AndAlso Not pobjConditions.IsEmpty Then
                    strSQL &= " ON " & pobjConditions.SQL(eConnectionType)
                End If

                'Surround the join with parentheses - MS Access won't accept it otherwise
                Return "(" & strSQL & ")"

            End Get
        End Property

        Private Shared Function GetJoinString(eJoinType As Type) As String

            Select Case eJoinType
                Case SQLSelectTableJoin.Type.Inner
                    Return "INNER JOIN"
                Case SQLSelectTableJoin.Type.FullOuter
                    Return "FULL OUTER JOIN"
                Case SQLSelectTableJoin.Type.LeftOuter
                    Return "LEFT OUTER JOIN"
                Case SQLSelectTableJoin.Type.RightOuter
                    Return "RIGHT OUTER JOIN"
                Case Else
                    Throw New NotImplementedException(eJoinType.ToString)
            End Select

        End Function

    End Class

End Namespace
