
' ___________________________________________________
'
'  � Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

''' --------------------------------------------------------------------------------
''' <summary>
''' This class is identical to DatabaseObjects except that it implements the 
''' IEnumerable interface, therefore providing support for the "For Each" construct. 
''' If IEnumerable is not required then inherit from DatabaseObjects. Generally, 
''' inheriting from DatabaseObjectsEnumerable is preferable to DatabaseObjects as it 
''' generates simpler and more readable code.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public MustInherit Class DatabaseObjectsEnumerable
    Inherits DatabaseObjects
    Implements IEnumerable

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

End Class
