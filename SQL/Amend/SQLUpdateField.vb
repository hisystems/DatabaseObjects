
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLUpdateField
        Inherits SQLFieldValue

        Private pobjSourceValue As SQLExpression

        Public Sub New(ByVal strDestinationFieldName As String, ByVal strSourceFieldName As String)

            Me.New(strDestinationFieldName, New SQLFieldExpression(strSourceFieldName))

        End Sub

        Public Sub New(ByVal strDestinationFieldName As String, ByVal objSourceExpression As SQLExpression)

            If strDestinationFieldName = String.Empty Then
                Throw New ArgumentNullException("DestinationFieldName")
            End If

            If objSourceExpression Is Nothing Then
                Throw New ArgumentNullException("SourceExpression")
            End If

            MyBase.Name = strDestinationFieldName
            pobjSourceValue = objSourceExpression

        End Sub

        Public Shadows ReadOnly Property Value() As SQLExpression
            Get

                Return pobjSourceValue

            End Get
        End Property

    End Class

End Namespace
