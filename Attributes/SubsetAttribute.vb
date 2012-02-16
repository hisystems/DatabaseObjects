
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
''' The SubsetAttribute class is used to indicate that the collection is a subset
''' of the parent collection. The library uses the DatabaseObjects.Parent.DistinctValue
''' property to determine the value to filter this table by using the field name specified
''' in the attribute. For example if the collection was a set of InvoiceLines then
''' the table would filter on the InvoiceID in the InvoicesLines table using the InvoiceID
''' from the parent Invoice object.
''' To further customer the subset (i.e. make it conditional)
''' simply override the Subset function and do not specify a SubsetAttribute.
''' </summary>
''' <example>
''' <code>
'''    &lt;Subset("InvoiceID")&gt;
'''    Public Class InvoiceLines
'''        ...
''' </code>
''' </example>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)> _
Public Class SubsetAttribute
    Inherits Attribute

    Private pstrFieldName As String

    ''' <summary>
    ''' Specifies the field name used to subset/filter the table so that the collection
    ''' represents the appropriate records from the database table.
    ''' For example, "InvoiceID" for an InvoiceLines collection.
    ''' </summary>
    Public Sub New(ByVal strUsingFieldName As String)

        If strUsingFieldName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrFieldName = strUsingFieldName

    End Sub

    Public ReadOnly Property FieldName() As String
        Get

            Return pstrFieldName

        End Get
    End Property

End Class
