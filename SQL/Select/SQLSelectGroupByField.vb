' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au<http://www.hisystems.com.au> - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectGroupByField

        Private pobjExpression As SQLExpression

        Friend Sub New(ByVal objExpression As SQLExpression)

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

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Return pobjExpression.SQL(eConnectionType)

            End Get
        End Property

    End Class

End Namespace

