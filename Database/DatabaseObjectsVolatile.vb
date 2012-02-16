
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

''' --------------------------------------------------------------------------------
''' <summary>
''' This class extends DatabaseObjects by storing all objects associated with this
''' DatabaseObjects collection in memory. Any objects added via VolatileObjectAdd() 
''' or VolatileObjectDelete() only affect the memory list until VolatileObjectsSave() is called.
''' VolatileObjectsSave() will delete any objects flagged for deletion via VolatileObjectDelete()
''' and then save any pre-loaded or newly added objects via VolatileObjectsAdd() 
''' to the database.
''' Item objects can implement IDatabaseObjectVolatile to override the default saving
''' behaviour of VolatileObjectsSave().
''' </summary>
''' --------------------------------------------------------------------------------
Public MustInherit Class DatabaseObjectsVolatile
    Inherits DatabaseObjects
    Implements IEnumerable

    Private pobjItems As IList
    Private pobjItemsToDelete As IList = New ArrayList
    Private pobjData As Object

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance with the parent that it is associated with.
    ''' </summary>
    ''' 
    ''' <param name="objParent">
    ''' The parent collection that this collection is associated with. This is often
    ''' useful so that the SubSet property can use the Parent to filter 
    ''' by a particular value pertinent to the parent object. 
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Protected Sub New(ByVal objParent As DatabaseObject)

        MyBase.New(objParent)
        VolatileItemsLoad()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance with it's associated database.
    ''' </summary>
    ''' 
    ''' <param name="objDatabase">
    ''' The database that this collection is associated with.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Protected Sub New(ByVal objDatabase As Database)

        MyBase.New(objDatabase)
        VolatileItemsLoad()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance with it's associated database and data that 
    ''' can be used for specifying a subset.
    ''' </summary>
    ''' <remarks>
    ''' The Data propety is set before the items list is loaded.
    ''' </remarks>
    ''' 
    ''' <param name="objDatabase">
    ''' The database that this collection is associated with.
    ''' </param>
    ''' 
    ''' <param name="objData">
    ''' An additional object that is usually required so that it can be used 
    ''' as a filter in the SubSet function.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Protected Sub New(ByVal objDatabase As Database, ByVal objData As Object)

        MyBase.New(objDatabase)

        pobjData = objData
        VolatileItemsLoad()

    End Sub

    ''' <summary>
    ''' Loads the items for this collection.
    ''' This is usually overridden in base class so that code can be executed before the objects list is loaded.
    ''' Because otherwise code cannot be executed before the MyBase.New or :base() call to this base class.
    ''' as the items are loaded as part of the constructor.
    ''' </summary>
    Protected Overridable Sub VolatileItemsLoad()

        pobjItems = MyBase.ObjectsList

    End Sub

    ''' <summary>
    ''' Returns the argument passed into the constructor New(Database, Object).
    ''' </summary>
    Protected ReadOnly Property Data() As Object
        Get

            Return pobjData

        End Get
    End Property

    '''' --------------------------------------------------------------------------------
    '''' <summary>
    '''' Initializes a new instance with it's associated database.
    '''' </summary>
    '''' 
    '''' <param name="objDatabase">
    '''' The database that this collection is associated with.
    '''' </param>
    ' '''
    '''' <param name="objParent">
    '''' The parent collection that this collection is associated with. This is often
    '''' useful so that the SubSet property can use the Parent to filter 
    '''' by a particular value pertinent to the parent object. Using the object
    '''' is optional. 
    '''' </param>
    '''' --------------------------------------------------------------------------------
    'Protected Sub New(ByVal objDatabase As Database, ByVal objParent As Object)

    '    MyBase.New(objDatabase)

    '    pobjParent = objParent
    '    pobjItems = MyBase.ObjectsList

    'End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Adds a new object (created via ItemInstance) to the in-memory list, and flags the 
    ''' item to be saved to the database when VolatileObjectsSave() is called. 
    ''' Returns the new object that has been added to the in-memory list.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Function VolatileObjectAdd() As IDatabaseObject

        Dim objNewItem As IDatabaseObject = Me.ItemInstance

        Me.VolatileObjectAdd(objNewItem)

        Return objNewItem

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Adds an item to the in-memory list, and flags the item to be saved to the database 
    ''' when VolatileObjectsSave() is called. 
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub VolatileObjectAdd(ByVal objItem As IDatabaseObject)

        pobjItems.Add(objItem)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an item at the specific index in the in-memory list.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected ReadOnly Property VolatileObjectByOrdinal(ByVal intIndex As Integer) As IDatabaseObject
        Get

            Return DirectCast(pobjItems(intIndex), IDatabaseObject)

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the index at which the object exists in the in-memory list.
    ''' </summary>
    ''' <exception cref="ArgumentOutOfRangeException">ArgumentOutOfRangeException</exception>
    ''' --------------------------------------------------------------------------------
    Protected Function VolatileObjectIndexOf(ByVal objObject As IDatabaseObject) As Integer

        Dim intIndex As Integer = 0

        For Each objItem As IDatabaseObject In pobjItems
            If objItem Is objObject Then
                Return intIndex
            End If
            intIndex += 1
        Next

        Throw New ArgumentOutOfRangeException

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Removes the item from the in-memory list, and flags the item to be deleted when
    ''' VolatileObjectsSave() is called.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub VolatileObjectDelete(ByVal objItem As IDatabaseObject)

        pobjItemsToDelete.Add(objItem)
        pobjItems.Remove(objItem)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns all items that have been added or were part of the initial list of objects. If
    ''' there are no items in the list then a zero length array is returned.
    ''' This list does not include any objects that have been deleted via VolatileObjectDelete().
    ''' After VolatileObjectsSave() has been called this list remains the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected ReadOnly Property VolatileObjects() As IDatabaseObject()
        Get

            Dim objItems(pobjItems.Count - 1) As IDatabaseObject
            pobjItems.CopyTo(objItems, 0)

            Return objItems

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns all objects that have been flagged for deletion via VolatileObjectDelete().
    ''' After VolatileObjectsSave() has been called this list is cleared and a zero length array
    ''' would be returned.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected ReadOnly Property VolatileObjectsToDelete() As IDatabaseObject()
        Get

            Dim objItems(pobjItemsToDelete.Count - 1) As IDatabaseObject
            pobjItemsToDelete.CopyTo(objItems, 0)

            Return objItems

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Clears all items in the in-memory list. Items are NOT flagged for deletion and 
    ''' subsequently deleted in VolatileObjectsSave().
    ''' Use VolatileObjectsDeleteAll to for this purpose.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub VolatileObjectsClear()

        pobjItems.Clear()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Removes all items from the in-memory list, and flags them to be deleted on the next
    ''' VolatileObjectsSave(). To clear the list without deleting the objects use
    ''' VolatileObjectsClear().
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub VolatileObjectsDeleteAll()

        For Each objItem As IDatabaseObject In pobjItems
            pobjItemsToDelete.Add(objItem)
        Next

        pobjItems.Clear()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' All items flagged for deletion (via VolatileObjectDelete()) are deleted in the database. 
    ''' Then all items that have been added to the in-memory list (via VolatileObjectAdd())
    ''' are added/saved to the database. If any of the objects implement IDatabaseObjectVolatile
    ''' then the IDatabaseObjectVolatile.Save function is called instead of the default
    ''' Database.ObjectSave() call. This allows the object to perform any additional 
    ''' checking and validation.
    ''' IDatabaseObjectVolatile.Save is assumed to save itself to the database. 
    ''' i.e. via a call to MyBase.Save().
    ''' Objects that do not implement IDatabaseObjectVolatile are still saved via the
    ''' Database.ObjectSave() function.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Friend Sub SaveAll()
    ''' 
    '''     Mybase.VolatileObjectsSave()
    ''' 
    ''' End Sub
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Overridable Sub VolatileObjectsSave()

        For Each objItem As IDatabaseObject In pobjItemsToDelete
            Me.ParentDatabase.ObjectDelete(Me, objItem)
        Next

        pobjItemsToDelete.Clear()

        For Each objItem As IDatabaseObject In pobjItems
            If TypeOf objItem Is IDatabaseObjectVolatile Then
                DirectCast(objItem, IDatabaseObjectVolatile).Save()
            Else
                Me.ParentDatabase.ObjectSave(Me, objItem)
            End If
        Next

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of items in the in-memory list.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected ReadOnly Property VolatileObjectsCount() As Integer
        Get

            Return pobjItems.Count

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the enumerator for all objects currently in the in-memory list.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Function VolatileObjectsEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

        Return pobjItems.GetEnumerator

    End Function

End Class
