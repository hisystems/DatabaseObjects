
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' <summary>
    ''' This class is deprecated. Use DatabaseObjects.Generic.DatabaseObjectsVolatile instead, it provides the same functionality.
    ''' </summary>
    <Obsolete("Use DatabaseObjects.Generic.DatabaseObjectsVolatile instead, it provides the same functionality.")> _
    Public MustInherit Class DatabaseObjectsVolatileUsingAttributes(Of T As IDatabaseObject)
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

    End Class

End Namespace
