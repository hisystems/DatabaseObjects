
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLValueExpression
        Inherits SQLExpression

        Private pobjValue As Object

        ''' <summary>
        ''' Specifies NULL.
        ''' </summary>
        Public Sub New()

        End Sub

        Public Sub New(ByVal objValue As Object)

            pobjValue = objValue

        End Sub

        Public ReadOnly Property Value() As Object
            Get

                Return pobjValue

            End Get
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Return Misc.SQLConvertValue(pobjValue, eConnectionType)

        End Function

    End Class

End Namespace
