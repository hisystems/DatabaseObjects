' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    ''' <summary>
    ''' Represents '*' or 'T.*' when used to select all fields from a table or join.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SQLAllFieldsExpression
        Inherits SQLExpression

        Private pobjTable As SQLSelectTable

        Public Sub New()

        End Sub

        Public Sub New(ByVal objTable As SQLSelectTable)

            Me.Table = objTable

        End Sub

        Public Property Table() As SQLSelectTable
            Get

                Return pobjTable

            End Get

            Set(ByVal Value As SQLSelectTable)

                pobjTable = Value

            End Set
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Dim strSQL As String = String.Empty

            If pobjTable IsNot Nothing Then
                strSQL &= SQLTablePrefix(pobjTable, eConnectionType) & "."
            End If

            Return strSQL & "*"

        End Function

    End Class

End Namespace
