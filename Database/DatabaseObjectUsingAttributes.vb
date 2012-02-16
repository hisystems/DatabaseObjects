
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
''' This class is identical to DatabaseObject except that rather than using the 
''' LoadFields and SaveFields the properties are automatically set and retrieved
''' using the FieldMappingAttribute attribute. If necessary, the LoadFields and 
''' SaveFields functions can still be overridden and the MyBase.LoadFields and 
''' MyBase.SaveFields functions explicity called to load the database fields
''' that have been marked with the FieldMappingAttribute attribute.
''' </summary>
''' <remarks>
''' When loading or saving the object, all properties and fields of any base classes 
''' are traversed and their values respectively set or read, up through the 
''' inheritance chain until the top level class has been loaded. 
''' </remarks>
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
Public MustInherit Class DatabaseObjectUsingAttributes
    Inherits DatabaseObject

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new DatabaseObject with the parent collection that this object is 
    ''' associated with.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Sub New(ByVal objParentCollection As DatabaseObjects)

        MyBase.New(objParentCollection)

    End Sub

    '''' --------------------------------------------------------------------------------
    '''' <summary>
    '''' Initializes a new DatabaseObject with the parent collection that this object is 
    '''' associated with. Also initializes the object from a set of database field
    '''' values.
    '''' </summary>
    '''' --------------------------------------------------------------------------------
    'Protected Sub New(ByVal objParentCollection As DatabaseObjects, ByVal objFieldValues As SQL.SQLFieldValues)

    '    MyBase.New(objParentCollection, objFieldValues)

    'End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets the properties and fields marked with the FieldMappingAttribute with the 
    ''' values from the database record. Properties or fields that are an enum data 
    ''' type are automatically converted from the database integer value to the equivalent 
    ''' enum. For properties and fields marked with the FieldMappingObjectHookAttribute 
    ''' the property's or field's object is also traversed for properties or fields marked
    ''' with the FieldMappingAttribute. 
    ''' Loads the lowest order base class that does not exist in the 
    ''' DatabaseObjects assembly first up through to the highest order class.
    ''' This function should generally not be called from an inheritor.
    ''' Use LoadFieldValues() to correctly load this object from a set of field values.
    ''' </summary>
    ''' <remarks>
    ''' Generally, this function should only be overriden in order to perform any custom
    ''' loading of data from the database. To completely initialize and load the object 
    ''' from an SQL.SQLFieldValues object call the protected LoadFieldValues(SQL.SQLFieldValues)
    ''' sub.
    ''' </remarks>
    ''' <example> 
    ''' <code>
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("Name")&gt; _
    ''' Private pstrName As String
    ''' 
    ''' OR
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("Name")&gt; _
    ''' Public Property Name() As String
    '''     Get
    ''' 
    '''         Return pstrName
    ''' 
    '''     End Get
    ''' 
    '''     Set(ByVal Value As String)
    ''' 
    '''         pstrName = Value
    ''' 
    '''     End Set
    ''' 
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    Protected Overrides Sub LoadFields(ByVal objFields As SQL.SQLFieldValues)

        DatabaseObjectUsingAttributesHelper.LoadFieldsForBaseTypes(Me, Me.GetType, objFields)

    End Sub


    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the values from the properties and fields marked with the FieldMappingAttribute 
    ''' to be saved to the database. Properties or fields that return an enum data type are 
    ''' automatically converted from the enum to the equivalent integer value for database
    ''' storage. For properties and fields marked with the FieldMappingObjectHookAttribute 
    ''' the property's or field's object is also traversed for properties or fields marked
    ''' with the FieldMappingAttribute. 
    ''' </summary>
    ''' <example> 
    ''' <code>
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("Name")&gt; _
    ''' Private pstrName As String
    ''' 
    ''' OR
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("Name")&gt; _
    ''' Public Property Name() As String
    '''     Get
    ''' 
    '''         Return pstrName
    ''' 
    '''     End Get
    ''' 
    '''     Set(ByVal Value As String)
    ''' 
    '''         pstrName = Value
    ''' 
    '''     End Set
    ''' 
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    Protected Overrides Function SaveFields() As SQL.SQLFieldValues

        Return DatabaseObjectUsingAttributesHelper.SaveFieldsForBaseTypes(Me, Me.GetType)

    End Function


End Class
