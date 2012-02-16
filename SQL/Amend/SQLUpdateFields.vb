
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLUpdateFields
        Inherits SQLFieldValues

        Public Overloads Function AddCopy(ByVal strDestinationFieldName As String, ByVal strSourceFieldName As String) As SQLUpdateField

            Dim objField As New SQLUpdateField(strDestinationFieldName, strSourceFieldName)

            Me.Add(objField)

            Return objField

        End Function

        Public Overloads Function Add(ByVal strDestinationFieldName As String, ByVal objExpression As SQLExpression) As SQLUpdateField

            Dim objField As New SQLUpdateField(strDestinationFieldName, objExpression)

            pobjFields.Add(objField)

            Return objField

        End Function

        Public Overloads Sub Add(ByVal objField As SQLUpdateField)

            pobjFields.Add(objField)

        End Sub

        Public Overloads Sub Add(ByVal objFieldValues As SQLFieldValues)

            For Each objFieldValue As SQLFieldValue In objFieldValues
                MyBase.Add(objFieldValue)
            Next

        End Sub

    End Class

End Namespace

