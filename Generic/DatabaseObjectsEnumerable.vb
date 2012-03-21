
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'


Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This extends Generic.DatabaseObjects and implements the IEnumerable interface, 
    ''' therefore providing support for the "For Each" construct. 
    ''' If IEnumerable is not required then inherit from Generic.DatabaseObjects. Generally, 
    ''' inheriting from Generic.DatabaseObjectsEnumerable is preferable to 
    ''' Generic.DatabaseObjects as it generates simpler and more readable code.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public MustInherit Class DatabaseObjectsEnumerable(Of T As IDatabaseObject)
        Inherits Generic.DatabaseObjects(Of T)
        Implements Collections.Generic.IEnumerable(Of T)

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

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return MyBase.ObjectsList.GetEnumerator

        End Function

        Private Function GetEnumerator_() As System.Collections.Generic.IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator

            Return MyBase.ObjectsList.GetEnumerator

        End Function

    End Class

End Namespace
