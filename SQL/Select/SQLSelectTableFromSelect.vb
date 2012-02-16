
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTableFromSelect
        Inherits SQLSelectTableBase

        Private pobjSelect As SQLSelect

        Public Sub New(ByVal objSelect As SQLSelect, ByVal strAlias As String)

            If strAlias = String.Empty Then
                Throw New ArgumentNullException
            ElseIf objSelect Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjSelect = objSelect
            MyBase.Alias = strAlias

        End Sub

        Friend Overrides Function GetPrefix() As String

            Return MyBase.Alias

        End Function
         
        Protected Overrides ReadOnly Property Source(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                pobjSelect.ConnectionType = eConnectionType
                Return "(" & pobjSelect.SQL & ")"

            End Get
        End Property

    End Class

End Namespace
