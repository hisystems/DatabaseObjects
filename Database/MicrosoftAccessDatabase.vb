
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Public Class MicrosoftAccessDatabase
    Inherits Database

    ''' <summary>
    ''' Connects to a Microsoft Access mdb database file.
    ''' Connects to a database with a blank password.
    ''' </summary>
    ''' <param name="strDatabaseFilePath">
    ''' The full or relative path to the mdb file - including the mdb file name and extension.
    ''' i.e. Data\database.mdb or C:\Data\database.mdb
    ''' </param>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDatabaseFilePath As String)

        Me.New(strDatabaseFilePath, String.Empty)

    End Sub

    ''' <summary>
    ''' Connects to a Microsoft Access mdb database file.
    ''' </summary>
    ''' <param name="strDatabaseFilePath">
    ''' The full or relative path to the mdb file - including the mdb file name and extension.
    ''' i.e. Data\database.mdb or C:\Data\database.mdb
    ''' </param>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDatabaseFilePath As String, ByVal strDatabasePassword As String)

        MyBase.New("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strDatabaseFilePath & ";Jet OLEDB:Database Password=" & strDatabasePassword & ";OLE DB Services=-3;", ConnectionType.MicrosoftAccess)

        If String.IsNullOrEmpty(strDatabaseFilePath) Then
            Throw New ArgumentNullException("Database File Path")
        End If

    End Sub

End Class
