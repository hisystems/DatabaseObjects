
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLFieldExpression
        Inherits SQLExpression

        Private pstrFieldName As String
        Private pobjTable As SQLSelectTable

        Public Sub New()


        End Sub

        Public Sub New(ByVal strFieldName As String)

            Me.Name = strFieldName

        End Sub

        Public Sub New(ByVal objTable As SQLSelectTable, ByVal strFieldName As String)

            Me.Name = strFieldName
            Me.Table = objTable

        End Sub

        Public Property Name() As String
            Get

                Return pstrFieldName

            End Get

            Set(ByVal value As String)

                pstrFieldName = value

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

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Dim strSQL As String = String.Empty

            If Me.Name = String.Empty Then
                Throw New Exceptions.DatabaseObjectsException("Field Name has not been set.")
            End If

            strSQL &= Misc.SQLFieldNameAndTablePrefix(pobjTable, pstrFieldName, eConnectionType)

            Return strSQL

        End Function

    End Class

End Namespace
