
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

''' <summary>
''' This class is deprecated. Use DatabaseObjects.DatabaseObject instead, it provides the same functionality.
''' </summary>
<Obsolete("Use DatabaseObjects.DatabaseObject instead, it provides the same functionality.")> _
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

End Class
