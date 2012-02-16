
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

''' --------------------------------------------------------------------------------
''' <summary>
''' The TableAttribute class specifies the name of the database table that this collection
''' is associated with.
''' This attribute must be specified for all DatabaseObjects*UsingAttributes classes.
''' </summary>
''' <example>
''' <code>
'''    &lt;Table("Customers")&gt;
'''    Public Class Customers
'''        ...
''' </code>
''' </example>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)> _
Public Class TableAttribute
    Inherits Attribute

    Private pstrTableName As String


    ''' <summary>
    ''' Specifies the name of the database table that is the source of this collection.
    ''' </summary>
    Public Sub New(ByVal strTableName As String)

        If strTableName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrTableName = strTableName

    End Sub

    Public ReadOnly Property Name() As String
        Get

            Return pstrTableName

        End Get
    End Property

End Class
