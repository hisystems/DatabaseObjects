
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
''' Specifies the field name that uniquely identifies each object 
''' within the collection. Typically, this is the field name of an identity or auto 
''' increment field. If the SubSetAttribute has been specified
''' then the strDistinctFieldName need only be unique within the subset not the 
''' entire table. The strDistinctFieldName and can be identical to the field name
''' specified with a KeyField attribute.
''' This attribute must be specified on a DatabaseObjects*UsingAttributes class.
''' This attribute is used to implement the IDatabaseObjects.DistinctFieldName
''' and IDatabaseObjects.DistinctFieldAutoIncrements functions.
''' </summary>
''' <example>
''' <code>
'''    &lt;DistinctField("CustomerID", bAutoIncrements:=True)&gt;
'''    Public Class Customers
'''        ...
''' </code>
''' </example>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)> _
Public Class DistinctFieldAttribute
    Inherits Attribute

    Private pstrDistinctFieldName As String
    Private peFieldValueAutomaticAssignment As SQL.FieldValueAutoAssignmentType = SQL.FieldValueAutoAssignmentType.None

    ''' <summary>
    ''' Specifies the field name that uniquely identifies each object 
    ''' within the collection.
    ''' </summary>
    Public Sub New(ByVal strDistinctFieldName As String)

        Me.New(strDistinctFieldName, False)

    End Sub

    ''' <summary>
    ''' Specifies the field name that uniquely identifies each object 
    ''' within the collection. Typically, this is the field name of an identity or auto 
    ''' increment field in which case the bAutoIncrements value should be set to true.
    ''' </summary>
    Public Sub New(ByVal strDistinctFieldName As String, ByVal bAutoIncrements As Boolean)

        If strDistinctFieldName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrDistinctFieldName = strDistinctFieldName
        If bAutoIncrements Then
            peFieldValueAutomaticAssignment = SQL.FieldValueAutoAssignmentType.AutoIncrement
        End If

    End Sub

    ''' <summary>
    ''' Specifies the field name that uniquely identifies each object 
    ''' within the collection. Typically, this is the field name of an identity or auto 
    ''' increment field in which case the bAutoIncrements value should be set to true.
    ''' </summary>
    Public Sub New(ByVal strDistinctFieldName As String, ByVal eAutomaticAssignment As SQL.FieldValueAutoAssignmentType)

        If strDistinctFieldName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrDistinctFieldName = strDistinctFieldName
        peFieldValueAutomaticAssignment = eAutomaticAssignment

    End Sub

    Public ReadOnly Property Name() As String
        Get

            Return pstrDistinctFieldName

        End Get
    End Property

    Public ReadOnly Property AutomaticAssignment() As SQL.FieldValueAutoAssignmentType
        Get

            Return peFieldValueAutomaticAssignment

        End Get
    End Property

End Class
