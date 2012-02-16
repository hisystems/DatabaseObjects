
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Interface ISQLStatement

        Property ConnectionType() As Database.ConnectionType
        ReadOnly Property SQL() As String

    End Interface

End Namespace
