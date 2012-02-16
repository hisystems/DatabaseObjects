
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

''' --------------------------------------------------------------------------------
''' <summary>
''' The FieldMappingAttribute class is used to define a mapping between a database
''' field and a class property. This attribute is used with the 
''' DatabaseObjectUsingAttributes class.
''' This field can also be used for loading fields that are of type 
''' DatabaseObjects.ObjectReference or DatabaseObjects.Generic.ObjectReference.
''' </summary>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field, AllowMultiple:=False, Inherited:=True)> _
Public Class FieldMappingAttribute
    Inherits Attribute

    Private pstrFieldName As String

    ''' --------------------------------------------------------------------------------
    ''' <param name="strFieldName">
    ''' The name of the database field associated with this property or field.
    ''' </param>
    ''' <example>
    ''' Loads a field:
    ''' <code>
    '''  
    ''' &lt;DatabaseObjects.FieldMapping("Name")&gt; _
    ''' Private pstrName As String
    ''' 
    ''' </code>
    ''' Loads an object:
    ''' <code>
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("ProductGroupID")&gt; _
    ''' Private pobjGroup As New Generic.ObjectReference(Of ProductGroup, Integer)(Database.ProductGroups)
    ''' 
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    Public Sub New(ByVal strFieldName As String)

        If strFieldName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrFieldName = strFieldName

    End Sub

    Public ReadOnly Property FieldName() As String
        Get

            Return pstrFieldName

        End Get
    End Property

End Class