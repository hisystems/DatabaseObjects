
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class extends DatabaseObjects.Generic.DatabaseObjectsEnumerable by providing public
    ''' Add, Item and Count properties. 
    ''' For more information please see DatabaseObjects.DatabaseObjects.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    '''
    Public MustInherit Class DatabaseObjectsList(Of T As IDatabaseObject)
        Inherits Generic.DatabaseObjectsEnumerable(Of T)

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

        ''' <summary>
        ''' Creates and returns a new object associated with this collection.
        ''' </summary>
        Public Overridable Function Add() As T

            Return Me.ItemInstance_

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of items in this collection.
        ''' </summary>
        ''' <remarks>
        ''' This property onforwards a call to DatabaseObjects.ObjectsCount.
        ''' </remarks>
        ''' --------------------------------------------------------------------------------
        '''
        Public ReadOnly Property Count() As Integer
            Get

                Return MyBase.ObjectsCount

            End Get
        End Property

        '''' --------------------------------------------------------------------------------
        '''' <summary>
        '''' Deletes the object's from the database.
        '''' </summary>
        '''' <remarks>
        '''' This function onforwards a call to DatabaseObjects.ObjectDelete.
        '''' </remarks>
        '''' --------------------------------------------------------------------------------
        '''' 
        'Public Overridable Sub Delete(ByRef objItem As T)

        '    MyBase.ObjectDelete(objItem)

        'End Sub

    End Class

End Namespace
