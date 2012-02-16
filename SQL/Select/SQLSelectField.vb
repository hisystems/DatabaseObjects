
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectField

        Private pstrAlias As String
        Private pobjExpression As SQLExpression

        Public Sub New(ByVal objExpression As SQLExpression)

            If objExpression Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjExpression = objExpression

        End Sub

        Friend ReadOnly Property Expression As SQLExpression
            Get

                Return pobjExpression

            End Get
        End Property

        Public Property [Alias]() As String
            Get

                [Alias] = pstrAlias

            End Get

            Set(ByVal Value As String)

                pstrAlias = Value.Trim

            End Set
        End Property

        Friend Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Dim strSQL As String = pobjExpression.SQL(eConnectionType)

            If Me.Alias <> String.Empty Then
                strSQL &= " AS " & SQLConvertIdentifierName(Me.Alias, eConnectionType)
            End If

            Return strSQL

        End Function

    End Class

End Namespace
