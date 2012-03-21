
' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports System.Data

Public MustInherit Class RootContainer

    Private _database As Database

    Protected Sub New(ByVal database As Database)

        If database Is Nothing Then
            Throw New ArgumentNullException
        End If

        Me._database = database

    End Sub

    Protected Friend ReadOnly Property Database As Database
        Get

            Return Me._database

        End Get
    End Property

End Class

