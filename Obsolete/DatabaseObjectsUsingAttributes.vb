' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

''' <summary>
''' This class is deprecated. Use DatabaseObjects.DatabaseObjects instead, it provides the same functionality.
''' </summary>
<Obsolete("Use DatabaseObjects.DatabaseObjects instead, it provides the same functionality.")> _
Public MustInherit Class DatabaseObjectsUsingAttributes
    Inherits DatabaseObjects

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

End Class
