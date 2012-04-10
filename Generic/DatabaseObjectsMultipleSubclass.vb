
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Generic

    ''' <summary>
    ''' Extends DatabaseObjects.Generic.DatabaseObjects but provides a additional override ItemInstanceForSubclass_
    ''' which allows different subclasses to be created (that inherit from T) based on the contents of a database record
    ''' as specified by the SQLFieldValues argument.
    ''' </summary>
    Public MustInherit Class DatabaseObjectsMultipleSubclass(Of T As IDatabaseObject)
        Inherits DatabaseObjects(Of T)
        Implements IDatabaseObjectsMultipleSubclass

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Should return an instance of the class that is associated with this collection of objects. 
        ''' The associated class must implement the IDatabaseObjectMultipleSubclass interface.
        ''' </summary>
        ''' <param name="objFieldValues">
        ''' The database record field values that can be used to determine the type of subclass to be loaded.
        ''' Do NOT call ObjectFromFieldValues() or ObjectLoad(). These functions are called after
        ''' ItemInstanceForSubclass returns.
        ''' from this function.
        ''' </param>
        ''' 
        ''' <example> 
        ''' <code>
        ''' Protected Overrides Function ItemInstanceForSubclass_(ByVal objFieldValues As SQL.SQLFieldValues) 
        ''' 
        '''     If objSubclassRecord("Type") = "Special" Then
        '''         Return New SpecialisedProduct
        '''     Else
        '''         Return New Product
        '''     End If
        ''' 
        ''' End Function
        ''' </code>
        ''' </example>    
        ''' --------------------------------------------------------------------------------
        Protected MustOverride Function ItemInstanceForSubclass_(ByVal objFieldValues As SQL.SQLFieldValues) As T

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new DatabaseObjects with it's associated database.
        ''' </summary>
        ''' 
        ''' <param name="objDatabase">
        ''' The database that this collection is associated with.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objDatabase As Database)

            MyBase.New(objDatabase)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes with it the associated root container and database.
        ''' </summary>
        ''' 
        ''' <param name="rootContainer">
        ''' The root object that this collection is associated with.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal rootContainer As RootContainer)

            MyBase.New(rootContainer)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new DatabaseObjects with it's associated parent object.
        ''' The Parent property can be used to access the parent variable passed into this constructor.
        ''' </summary>
        ''' 
        ''' <param name="objParent">
        ''' The parent object that this collection is associated with.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objParent As DatabaseObject)

            MyBase.New(objParent)

        End Sub

        Private Function ItemInstanceForSubclass(ByVal objFieldValues As SQL.SQLFieldValues) As IDatabaseObject Implements IDatabaseObjectsMultipleSubclass.ItemInstanceForSubclass

            Return Me.ItemInstanceForSubclass_(objFieldValues)

        End Function

        Protected Overrides Function ItemInstance_() As T

            Throw New NotSupportedException("ItemInstance_ is not supported for IDatabaseObjectsMultipleSubclass objects")

        End Function

        Protected Overrides Function ItemInstance() As IDatabaseObject

            Throw New NotSupportedException("ItemInstance is not supported for IDatabaseObjectsMultipleSubclass objects")

        End Function

    End Class

End Namespace