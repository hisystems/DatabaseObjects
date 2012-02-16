
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
''' The KeyFieldAttribute should specify the field name that uniquely identifies each object 
''' within the collection. As opposed to the ordinal/index position or distinct field, the key field 
''' provides another method of accessing a particular object within the collection. 
''' The key field must be unique within the collection. If the SubsetAttribute
''' has been specified then the key field only needs to be unique within 
''' the subset, not the entire table. Specifying this attribute is optional.
''' </summary>
''' <example>
''' <code>
'''    &lt;KeyField("CustomerName")&gt;
'''    Public Class Customers
'''        ...
''' </code>
''' </example>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)> _
Public Class KeyFieldAttribute
    Inherits Attribute

    Private pstrKeyFieldName As String

    ''' <summary>
    ''' Specifies the field name that uniquely identifies each object 
    ''' within the collection. As opposed to the ordinal/index position or distinct field, 
    ''' the key field provides another method of accessing a particular object within the collection. 
    ''' The key field must be unique within the collection. If the SubsetAttribute
    ''' has been specified then the key field only needs to be unique within 
    ''' the subset, not the entire table. Specifying this attribute is optional.
    ''' </summary>
    Public Sub New(ByVal strKeyFieldName As String)

        If strKeyFieldName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrKeyFieldName = strKeyFieldName

    End Sub

    Public ReadOnly Property Name() As String
        Get

            Return pstrKeyFieldName

        End Get
    End Property

End Class
