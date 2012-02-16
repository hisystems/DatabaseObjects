
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class extends Generic.DatabaseObjectsVolatile by providing by adding
    ''' a public Add, Item, Count and Delete functions.
    ''' For more information please see 
    ''' <see cref="Generic.DatabaseObjectsVolatile(Of T)">Generic.DatabaseObjectsVolatile</see>.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public MustInherit Class DatabaseObjectsVolatileList(Of T As IDatabaseObject)
        Inherits Generic.DatabaseObjectsVolatile(Of T)

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

        '    MyBase.New(objDatabase, objParent)

        'End Sub

        ''' <summary>
        ''' Creates and returns a new object which has been added to the in-memory list.
        ''' </summary>
        Public Overridable Function Add() As T

            Return MyBase.VolatileObjectAdd

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns an item from the in-memory list at a specific ordinal index.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Default Public Overridable ReadOnly Property Item(ByVal intIndex As Integer) As T
            Get

                Return MyBase.VolatileObjectByOrdinal(intIndex)

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Removes the item from the in-memory list, and flags the item to be deleted.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Public Overridable Sub Delete(ByRef objItem As T)

            MyBase.VolatileObjectDelete(objItem)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of items in the in-memory list.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public ReadOnly Property Count() As Integer
            Get

                Return MyBase.VolatileObjectsCount

            End Get
        End Property

    End Class

End Namespace
