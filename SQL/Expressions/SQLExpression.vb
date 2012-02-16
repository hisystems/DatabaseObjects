
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public MustInherit Class SQLExpression

        Friend MustOverride Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

    End Class

End Namespace
