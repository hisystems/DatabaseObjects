' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Namespace Constraints

    ''' <summary>
    ''' Ensures that the email address is valid.
    ''' Credit to: http://regexlib.com/REDetails.aspx?regexp_id=711
    ''' </summary>
    Public Class EmailAddressConstraint
        Inherits RegExConstraint

        Public Sub New()

            MyBase.New("^((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)$")

        End Sub

        Public Overrides Function ToString() As String

            Return "email address is valid"

        End Function

    End Class

End Namespace
