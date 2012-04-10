
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

''' <summary>
''' This class is deprecated. Use DatabaseObjects.DatabaseObjectLockable instead, it provides the same functionality.
''' </summary>
<Obsolete("Use DatabaseObjects.DatabaseObjectLockable instead, it provides the same functionality.")> _
Public MustInherit Class DatabaseObjectUsingAttributesLockable
    Inherits DatabaseObjectLockable

    ''' <summary>
    ''' Initializes a new DatabaseObject with the parent collection that this object is 
    ''' associated with and the lock controller to be used with this object.
    ''' </summary>
    Protected Sub New(ByVal objParent As DatabaseObjects, ByVal objLockController As DatabaseObjectLockController)

        MyBase.New(objParent, objLockController)

    End Sub

End Class
