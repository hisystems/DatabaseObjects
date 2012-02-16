
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
''' Extends the capabilities of the DatabaseObjectUsingAttributes by providing a set of routines 
''' that lock and unlock this object for exclusive access for a particular user.
''' When Lock() is called a record is written to the lock table which includes the
''' object's associated table name, the object's distinct value and the user ID
''' specified in the DatabaseObjectLockController. When Unlock() is called this 
''' record is deleted. If another or the current user has locked the object then the 
''' IsLocked property will return true.
''' The DatabseObjects library does not inhibit the loading and/or saving of any 
''' locked objects.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public MustInherit Class DatabaseObjectUsingAttributesLockable
    Inherits DatabaseObjectUsingAttributes
    Implements IDatabaseObjectLockable

    Private pobjLockController As DatabaseObjectLockController

    Protected Sub New(ByVal objParent As DatabaseObjects, ByVal objLockController As DatabaseObjectLockController)

        MyBase.New(objParent)

        If objLockController Is Nothing Then
            Throw New ArgumentNullException
        End If

        pobjLockController = objLockController

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Indicates whether the current object is Locked either by the current user or 
    ''' another user.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Overridable ReadOnly Property IsLocked() As Boolean Implements IDatabaseObjectLockable.IsLocked
        Get

            Return pobjLockController.IsLocked(MyBase.ParentCollection, Me)

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the user identifier that currently has the object locked. Throws an 
    ''' exception if the object is not locked by a user.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected ReadOnly Property LockedByUserID() As String Implements IDatabaseObjectLockable.LockedByUserID
        Get

            Return pobjLockController.LockedByUserID(MyBase.ParentCollection, Me)

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Locks this object. 
    ''' Throws an exception if this object is already locked.
    ''' Throws an exception if this object has not been saved.
    ''' It is possible that between calling IsLocked and Lock another
    ''' user may have locked the object. Therefore, it is recommended calling Lock and then 
    ''' trapping the MethodAccessException to determine whether the lock actually succeeded.
    ''' </summary>
    ''' <exception cref="Exceptions.DatabaseObjectsException">Thrown if the object has not been saved or the object is already locked.</exception>
    ''' --------------------------------------------------------------------------------
    Public Sub Lock() Implements IDatabaseObjectLockable.Lock

        pobjLockController.Lock(MyBase.ParentCollection, Me)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' UnLocks this object. Throws an exception if the object is not locked or the
    ''' object has not been saved.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Sub UnLock() Implements IDatabaseObjectLockable.UnLock

        pobjLockController.UnLock(MyBase.ParentCollection, Me)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the current user has the object locked.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public ReadOnly Property IsLockedByCurrentUser() As Boolean Implements IDatabaseObjectLockable.IsLockedByCurrentUser
        Get

            Return pobjLockController.IsLockedByCurrentUser(MyBase.ParentCollection, Me)

        End Get
    End Property

End Class
