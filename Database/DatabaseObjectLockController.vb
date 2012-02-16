
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
''' This is the controller class that initializes the lock table and the user
''' ID that is to be associated with all locking operations. In most situations,  
''' only one instance of this class is ever created and this instance is passed into
''' the constructor for all DatabaseObjectLockable and DatabaseObjectUsingAttributesLockable 
''' instances.
''' </summary>
''' --------------------------------------------------------------------------------
Public Class DatabaseObjectLockController

    Private pstrCurrentUserID As String
    Private pstrLockTableName As String
    Private pobjConnection As Database.ConnectionController

    Public Sub New(ByVal objDatabase As Database, ByVal strLockTableName As String, ByVal strCurrentUserID As String)

        If objDatabase Is Nothing Then
            Throw New ArgumentNullException("Database")
        ElseIf strCurrentUserID = String.Empty Then
            Throw New ArgumentNullException("User")
        ElseIf strLockTableName = String.Empty Then
            Throw New ArgumentNullException("Lock table name")
        End If

        pobjConnection = objDatabase.Connection
        pstrCurrentUserID = strCurrentUserID
        pstrLockTableName = strLockTableName

        EnsureTableExists()

    End Sub

    Private Function EnsureTableExists() As Boolean

        Dim bTableExists As Boolean
        Dim objTableExists As New SQL.SQLTableExists(pstrLockTableName)

        pobjConnection.Start()

        With pobjConnection.Execute(objTableExists)
            bTableExists = .Read
            .Close()
        End With

        If Not bTableExists Then
            pobjConnection.ExecuteNonQuery(CreateTable)
            pobjConnection.ExecuteNonQuery(CreateTableIndex)
        End If

        pobjConnection.Finished()

    End Function

    ''' <summary>
    ''' Returns whether the object is locked.
    ''' </summary>
    Public ReadOnly Property IsLocked(ByVal objCollection As IDatabaseObjects, ByVal objObject As IDatabaseObject) As Boolean
        Get

            Return Me.LockRecordExists(objCollection.TableName, objObject)

        End Get
    End Property

    ''' <summary>
    ''' Returns whether the object is locked by the current user. Specifically, the user that was specified
    ''' in the constructor.
    ''' </summary>
    Public ReadOnly Property IsLockedByCurrentUser(ByVal objCollection As IDatabaseObjects, ByVal objObject As IDatabaseObject) As Boolean
        Get

            Return Me.LockRecordExists(objCollection.TableName, objObject, _
                New SQL.SQLCondition("UserID", SQL.ComparisonOperator.EqualTo, pstrCurrentUserID))

        End Get
    End Property

    ''' <summary>
    ''' Returns the user ID that has the object locked.
    ''' Throws an exception if the object is not locked.
    ''' </summary>
    Public ReadOnly Property LockedByUserID(ByVal objCollection As IDatabaseObjects, ByVal objObject As IDatabaseObject) As String
        Get

            'If Not Me.IsLocked(objCollection, objObject) Then
            '    Throw New MethodAccessException("Object is not locked")
            'End If

            Dim objSelect As New SQL.SQLSelect

            objSelect.Fields.Add("UserID")
            objSelect.Tables.Add(pstrLockTableName)
            objSelect.Where.Add("TableName", SQL.ComparisonOperator.EqualTo, objCollection.TableName)
            objSelect.Where.Add("RecordID", SQL.ComparisonOperator.EqualTo, CStr(objObject.DistinctValue))

            pobjConnection.Start()

            Dim objReader As IDataReader = pobjConnection.Execute(objSelect)

            If objReader.Read() Then
                LockedByUserID = CStr(objReader(0))
            Else
                Throw New Exceptions.DatabaseObjectsException("Object is not locked")
            End If

            objReader.Close()
            pobjConnection.Finished()

        End Get
    End Property

    Private Function LockRecordExists( _
        ByVal strTableName As String, _
        ByVal objObject As IDatabaseObject, _
        Optional ByVal objAdditionalCondition As SQL.SQLCondition = Nothing) As Boolean

        Dim objSelect As New SQL.SQLSelect

        objSelect.Fields.Add(String.Empty, SQL.AggregateFunction.Count)
        objSelect.Tables.Add(pstrLockTableName)
        objSelect.Where.Add("TableName", SQL.ComparisonOperator.EqualTo, strTableName)
        objSelect.Where.Add("RecordID", SQL.ComparisonOperator.EqualTo, CStr(objObject.DistinctValue))
        If Not objAdditionalCondition Is Nothing Then
            objSelect.Where.Add(objAdditionalCondition)
        End If

        pobjConnection.Start()
        Dim objReader As IDataReader = pobjConnection.Execute(objSelect)
        objReader.Read()

        LockRecordExists = CInt(objReader(0)) <> 0

        objReader.Close()
        pobjConnection.Finished()

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Locks an object. 
    ''' Throws an exception if the object is already locked.
    ''' Throws an exception if the object is not been saved.
    ''' Because it is possible that between calling IsLocked and calling Lock another
    ''' user may have locked the object. Therefore, it is recommended calling Lock and then 
    ''' trapping the Exceptions.ObjectAlreadyExistsException to determine whether the object is already locked.
    ''' </summary>
    ''' <exception cref="Exceptions.DatabaseObjectsException">Thrown if the object has not been saved.</exception>
    ''' <exception cref="Exceptions.ObjectAlreadyExistsException">Thrown if the object has already been locked.</exception>
    ''' --------------------------------------------------------------------------------
    Public Sub Lock(ByVal objCollection As IDatabaseObjects, ByVal objObject As IDatabaseObject)

        If Not objObject.IsSaved Then
            Throw New Exceptions.DatabaseObjectsException("Object is not saved and cannot be locked")
        End If

        Dim objInsert As New SQL.SQLInsert
        objInsert.TableName = pstrLockTableName
        objInsert.Fields.Add("TableName", objCollection.TableName)
        objInsert.Fields.Add("RecordID", CStr(objObject.DistinctValue))
        objInsert.Fields.Add("UserID", pstrCurrentUserID)

        'If another user/connection has managed to add a record to the database just before 
        'this connection has a DatabaseObjectsException will be thrown because duplicate keys will 
        'be added to the table.

        pobjConnection.Start()

        Try
            pobjConnection.ExecuteNonQuery(objInsert)
        Catch ex As Exceptions.DatabaseObjectsException
            pobjConnection.Finished()
            Throw New Exceptions.ObjectAlreadyLockedException(objCollection, objObject)
        End Try

        pobjConnection.Finished()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' UnLocks this object. Throws an exception if the object is not locked by the current 
    ''' user or the object has not been saved.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Sub UnLock(ByVal objCollection As IDatabaseObjects, ByVal objObject As IDatabaseObject)

        'If the table is locked by someone else
        If Not Me.IsLockedByCurrentUser(objCollection, objObject) Then
            Throw New MethodAccessException("Object already locked")
        ElseIf Not objObject.IsSaved Then
            Throw New MethodAccessException("Object is not saved and cannot be unlocked")
        End If

        Dim objDelete As New SQL.SQLDelete
        objDelete.TableName = pstrLockTableName
        objDelete.Where.Add("TableName", SQL.ComparisonOperator.EqualTo, objCollection.TableName)
        objDelete.Where.Add("RecordID", SQL.ComparisonOperator.EqualTo, CStr(objObject.DistinctValue))
        objDelete.Where.Add("UserID", SQL.ComparisonOperator.EqualTo, pstrCurrentUserID)

        pobjConnection.Start()
        pobjConnection.ExecuteNonQuery(objDelete)
        pobjConnection.Finished()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Provides a means by which to ensure all locks have been removed for this user
    ''' in situations where an unexpected exception occurs and/or the user logs out of 
    ''' system.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Sub UnlockAll()

        Dim objDelete As New SQL.SQLDelete
        objDelete.TableName = pstrLockTableName
        objDelete.Where.Add("UserID", SQL.ComparisonOperator.EqualTo, pstrCurrentUserID)

        pobjConnection.Start()
        pobjConnection.ExecuteNonQuery(objDelete)
        pobjConnection.Finished()

    End Sub

    Private Function CreateTable() As SQL.SQLStatement

        Dim objTable As New SQL.SQLCreateTable

        objTable.Name = pstrLockTableName
        objTable.Fields.Add("TableName", SQL.DataType.VariableCharacter, 50)
        objTable.Fields.Add("RecordID", SQL.DataType.VariableCharacter, 20)
        objTable.Fields.Add("UserID", SQL.DataType.VariableCharacter, 255)  'Accounts for windows user names 

        Return objTable

    End Function

    Private Function CreateTableIndex() As SQL.SQLStatement

        Dim objIndex As New SQL.SQLCreateIndex

        objIndex.Name = "Primary"
        objIndex.IsUnique = True
        objIndex.TableName = pstrLockTableName
        objIndex.Fields.Add("TableName")
        objIndex.Fields.Add("RecordID")
        objIndex.Fields.Add("UserID")

        Return objIndex

    End Function

End Class
