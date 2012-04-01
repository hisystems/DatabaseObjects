
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
''' The TableJoinAttribute indicates that this collection's table should be joined
''' with another.
''' This function is useful in optimising database loading speeds by allowing multiple 
''' tables to be joined into one data set. The resultant data set can then be used to load 
''' objects from the associated tables avoiding subsequent SQL calls. For a complete 
''' example, see the demonstration program. 
''' To further customer the subset (i.e. make it conditional)
''' simply override the TableJoins function and do not specify a TableJoinAttribute.
''' </summary>
''' <example>
''' <code>
'''    &lt;TableJoin("MainProductID", "Products", "ProductID")&gt;
'''    Public Class Customers
'''        ...
''' </code>
''' </example>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=True, Inherited:=True)> _
Public Class TableJoinAttribute
    Inherits Attribute

    Private pstrJoinFieldName As String
    Private pstrJoinToTableName As String
    Private pstrJoinToFieldName As String

    ''' <summary>
    ''' Specifies the field to join and the additional table and field name to which it is joined.
    ''' </summary>
    ''' <param name="strJoinFieldName">The name of the field in the collection's primary table that is to be joined to another table.</param>
    ''' <param name="strJoinToTableName">The name of the table to join with the collection's primary table.</param>
    ''' <param name="strJoinToFieldName">The name of the field in the table which is to be joined with the collection's primary table.</param>
    Public Sub New(ByVal strJoinFieldName As String, ByVal strJoinToTableName As String, ByVal strJoinToFieldName As String)

        If strJoinFieldName = String.Empty Then
            Throw New ArgumentNullException("Join Field Name")
        ElseIf strJoinToTableName = String.Empty Then
            Throw New ArgumentNullException("Join To Table Name")
        ElseIf strJoinToFieldName = String.Empty Then
            Throw New ArgumentNullException("Join To Field Name")
        End If

        pstrJoinFieldName = strJoinFieldName
        pstrJoinToTableName = strJoinToTableName
        pstrJoinToFieldName = strJoinToFieldName

    End Sub

    Public ReadOnly Property FieldName() As String
        Get

            Return pstrJoinFieldName

        End Get
    End Property

    Public ReadOnly Property ToTableName() As String
        Get

            Return pstrJoinToTableName

        End Get
    End Property

    Public ReadOnly Property ToFieldName() As String
        Get

            Return pstrJoinToFieldName

        End Get
    End Property

End Class
