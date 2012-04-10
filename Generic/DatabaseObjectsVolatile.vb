
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class is a generic version of DatabaseObjectsVolatile. 
    ''' This class stores all objects associated with this DatabaseObjects collection in 
    ''' memory. Any objects added via VolatileObjectAdd or VolatileObjectDelete only 
    ''' affect the memory list until VolatileObjectsSave() is called. 
    ''' VolatileObjectsSave() will delete any objects flagged for deletion via VolatileObjectDelete()
    ''' and then save any pre-loaded or newly added objects via VolatileObjectsAdd() 
    ''' to the database.
    ''' Item objects can implement IDatabaseObjectVolatile to override the default saving
    ''' behaviour of VolatileObjectsSave().
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public MustInherit Class DatabaseObjectsVolatile(Of T As IDatabaseObject)
        Inherits DatabaseObjectsVolatile
        Implements Collections.Generic.IEnumerable(Of T)

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
        ''' 
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objParent As DatabaseObject)

            MyBase.New(objParent)

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

            MyBase.New(objDatabase, objData)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a new object (created via ItemInstance) to the in-memory list, and flags the 
        ''' item to be saved to the database when VolatileObjectsSave() is called. 
        ''' Returns the new object that has been added to the in-memory list.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Protected Shadows Function VolatileObjectAdd() As T

            Dim objNewItem As T = Me.ItemInstance_

            Me.VolatileObjectAdd(objNewItem)

            Return objNewItem

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Adds an item to the in-memory list, and flags the item to be saved to the database 
        ''' when VolatileObjectsSave() is called. 
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Protected Shadows Sub VolatileObjectAdd(ByVal objItem As T)

            MyBase.VolatileObjectAdd(objItem)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns an item at the specific index in the in-memory list.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Protected Shadows ReadOnly Property VolatileObjectByOrdinal(ByVal intIndex As Integer) As T
            Get

                Return DirectCast(MyBase.VolatileObjectByOrdinal(intIndex), T)

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the index at which the object exists in the in-memory list.
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">ArgumentOutOfRangeException</exception>
        ''' --------------------------------------------------------------------------------
        Protected Shadows Function VolatileObjectIndexOf(ByVal objObject As T) As Integer

            Return MyBase.VolatileObjectIndexOf(DirectCast(objObject, T))

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Removes the item from the in-memory list, and flags the item to be deleted when
        ''' VolatileObjectsSave() is called.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Protected Shadows Sub VolatileObjectDelete(ByVal objItem As T)

            MyBase.VolatileObjectDelete(objItem)

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
        Protected Shadows ReadOnly Property VolatileObjects() As T()
            Get

                Dim objSourceObjects As IDatabaseObject() = MyBase.VolatileObjects
                Dim objDestinationObjects(objSourceObjects.Length - 1) As T

                Array.Copy(objSourceObjects, objDestinationObjects, objSourceObjects.Length)

                Return objDestinationObjects

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns all objects that have been flagged for deletion via VolatileObjectDelete().
        ''' After VolatileObjectsSave() has been called this list is cleared and a zero length array
        ''' would be returned.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Protected Shadows ReadOnly Property VolatileObjectsToDelete() As T()
            Get

                Dim objSourceObjects As IDatabaseObject() = MyBase.VolatileObjectsToDelete
                Dim objDestinationObjects(objSourceObjects.Length - 1) As T

                Array.Copy(objSourceObjects, objDestinationObjects, objSourceObjects.Length)

                Return objDestinationObjects

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Should return an instance of the class that is associated with this 
        ''' collection of objects. This is the generic version of the ItemInstance function.
        ''' It is suffixed with an underscore so that it does not conflict with the underlying
        ''' non-generic equivalent ItemInstance function. It's purpose is indentical to the
        ''' non-generic version.
        ''' </summary>
        ''' 
        ''' <example> 
        ''' <code>
        ''' Protected Overrides Function ItemInstance_() As Product
        ''' 
        '''     Return New Product
        ''' 
        ''' End Function
        ''' </code>
        ''' </example>    
        ''' --------------------------------------------------------------------------------
        Protected Overridable Function ItemInstance_() As T

            Return DirectCast(MyBase.ItemInstance, T)

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the enumerator for all objects currently in the in-memory list.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Protected Shadows Function VolatileObjectsEnumerator() As System.Collections.Generic.IEnumerator(Of T) 

            Return New Collections.Generic.List(Of T)(Me.VolatileObjects).GetEnumerator

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' For some reason - I believe it may be a bug/omission in the compiler, the above VolatileObjectsEnumerator
        ''' cannot be used to also implement System.Collections.Generic.IEnumerable(Of T).GetEnumerator.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Private Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator

            Return Me.VolatileObjectsEnumerator

        End Function

    End Class

End Namespace
