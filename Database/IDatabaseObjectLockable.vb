
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

''' --------------------------------------------------------------------------------
''' <summary>
''' Specifies the interface of all classes that are considered lockable by the library.
''' See DatabaseObjectLockable, DatabaseObjectUsingAttributesLockable for further
''' details.
''' </summary>
''' --------------------------------------------------------------------------------
Public Interface IDatabaseObjectLockable

    Sub Lock()
    Sub UnLock()
    ReadOnly Property LockedByUserID() As String
    ReadOnly Property IsLocked() As Boolean
    ReadOnly Property IsLockedByCurrentUser() As Boolean

End Interface
