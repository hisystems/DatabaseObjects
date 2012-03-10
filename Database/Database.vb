
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports DatabaseObjects
Imports System.Data
Imports System.Diagnostics
Imports System.Transactions


''' --------------------------------------------------------------------------------
''' <summary>
''' Represents a database connection and provides a set of functions that work
''' in conjunction with classes implementing IDatabaseObjects and IDatabaseObject. 
''' The Database class automatically generates and executes the required SQL 
''' statements to perform common database operations such as saving, deleting
''' searching etc. based on the values returned via the IDatabaseObjects and 
''' IDatabaseObject interfaces.
''' Typically, this class is only used when explicitly implementing the IDatabaseObjects 
''' and IDatabaseObject interfaces rather than inheriting from DatabaseObjects (or 
''' DatabaseObjectsEnumerable) and DatabaseObject. 
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public Class Database

    Public Enum ConnectionType
        SQLServer
        MicrosoftAccess
        MySQL
        Pervasive
        SQLServerCompactEdition
        HyperSQL
    End Enum

    Private pobjConnection As ConnectionController
    Private pobjTransactions As TransactionsClass

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new Database instance specifying the database to connect to and SQL 
    ''' syntax to use with the database. Each function call of the Database class opens 
    ''' and closes a connection. Therefore, connection pooling should be enabled 
    ''' for optimal performance.
    ''' </summary>
    ''' 
    ''' <param name="strConnectionString">
    ''' A database connection string to either a Microsoft Access, SQLServer, Pervasive or MySQL 
    ''' database. For example, 'Provider=Microsoft.Jet.OLEDB.4.0;Data 
    ''' Source=northwind.mdb;Jet OLEDB:Database Password=;'.
    ''' </param>
    ''' 
    ''' <param name="eConnectionType">
    ''' Indicates the SQL syntax to generate for the database specified in strConnectionString.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Sub New( _
        ByVal strConnectionString As String, _
        ByVal eConnectionType As ConnectionType)

        pobjConnection = New ConnectionController(strConnectionString, eConnectionType)
        pobjTransactions = New TransactionsClass(pobjConnection)

    End Sub

    ''' <summary>
    ''' Initializes the Database instance with the database connection to utilise.
    ''' The connection is not opened until it is required.
    ''' The supplied connection should not be opened.
    ''' </summary>
    ''' <param name="objDatabaseConnection">An unopened connection to the database.</param>
    ''' <remarks></remarks>
    Public Sub New( _
        ByVal objDatabaseConnection As IDbConnection, _
        ByVal eConnectionType As ConnectionType)

        pobjConnection = New ConnectionController(objDatabaseConnection, eConnectionType)
        pobjTransactions = New TransactionsClass(pobjConnection)

    End Sub

    ' ''' --------------------------------------------------------------------------------
    ' ''' <summary>
    ' ''' Allows the DatabaseUsingSchemaTranslations to override the connection controller
    ' ''' and override the field and table name translations.
    ' ''' </summary>
    ' ''' --------------------------------------------------------------------------------
    'Protected Sub New(ByVal objConnection As ConnectionController)

    '    If objConnection Is Nothing Then
    '        Throw New ArgumentNullException
    '    End If

    '    pobjConnection = objConnection
    '    pobjTransactions = New TransactionsClass(pobjConnection)

    'End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an instance of an object from the collection using a distinct value (see
    ''' IDatabaseObjects.DistinctFieldName). If the collection has implemented the 
    ''' IDatabaseObjects.Subset function then the objDistinctValue need only be unique 
    ''' within the collection's subset, not the entire database table.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection that contains the object.
    ''' </param>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value that uniquely identifies the object within the collection. This is the value
    ''' of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example> Loads a product using a product ID of 123
    ''' <code>
    ''' objProduct = objDatabase.Object(NorthwindDB.Products, 123)
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function [Object]( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objDistinctValue As Object) As IDatabaseObject

        Return ObjectFromFieldValues(objCollection, Me.ObjectFieldValues(objCollection, objDistinctValue))

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an instance of an object from the collection using a distinct value (see
    ''' IDatabaseObjects.DistinctFieldName). If the collection has implemented the 
    ''' IDatabaseObjects.Subset function then the objDistinctValue need only be unique 
    ''' within the collection's subset, not the entire database table.
    ''' Returns Nothing/null if the distinct value does not exist in the database.
    ''' This feature is what differentiates Database.Object() from Database.ObjectIfExists().
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection that contains the object.
    ''' </param>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value that uniquely identifies the object within the collection. This is the value
    ''' of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example> Loads a product using a product ID of 123
    ''' <code>
    ''' objProduct = objDatabase.Object(NorthwindDB.Products, 123)
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectIfExists( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objDistinctValue As Object) As IDatabaseObject

        Dim objFieldValues As SQL.SQLFieldValues = Me.ObjectFieldValuesIfExists(objCollection, objDistinctValue)

        If objFieldValues Is Nothing Then
            Return Nothing
        Else
            Return ObjectFromFieldValues(objCollection, objFieldValues)
        End If

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the database fields for an object from the collection using a distinct value 
    ''' (see IDatabaseObjects.DistinctFieldName). If the collection has implemented the 
    ''' IDatabaseObjects.Subset function then the objDistinctValue need only be unique 
    ''' within the collection's subset, not the entire database table.
    ''' This is typically used to interogate the database fields before loading the 
    ''' object with a call to ObjectFromFieldValues.
    ''' This function is rarely used and generally the Object function suffices.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection that contains the object.
    ''' </param>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value that uniquely identifies the object within the collection. This is the value
    ''' of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectFieldValues( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objDistinctValue As Object) As SQL.SQLFieldValues

        Dim objFieldValues As SQL.SQLFieldValues = ObjectFieldValuesIfExists(objCollection, objDistinctValue)

        If objFieldValues Is Nothing Then
            Throw New Exceptions.ObjectDoesNotExistException(objCollection, objDistinctValue)
        End If

        Return objFieldValues

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the database fields for an object from the collection using a distinct value 
    ''' (see IDatabaseObjects.DistinctFieldName). 
    ''' Returns Nothing/null if the distinct value does not exist.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Private Function ObjectFieldValuesIfExists( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objDistinctValue As Object) As SQL.SQLFieldValues

        Dim objFieldValues As SQL.SQLFieldValues
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objReader As IDataReader
        Dim objSubset As SQL.SQLConditions

        With objSelect
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objDistinctValue)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                .Where.Add(objSubset)
            End If
        End With

        pobjConnection.Start()
        objReader = pobjConnection.Execute(objSelect)

        If objReader.Read() Then
            objFieldValues = FieldValuesFromDataReader(objCollection, objReader)
        Else
            objFieldValues = Nothing
        End If

        objReader.Close()
        pobjConnection.Finished()

        Return objFieldValues

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an object exists for the specified distinct value in the collection.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection that is searched for the distinct value.
    ''' </param>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value to search for in the collection. This is the value of the field defined 
    ''' by the collection's IDatabaseObjects.DistinctFieldName function.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectExistsByDistinctValue( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objDistinctValue As Object) As Boolean

        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objReader As IDataReader
        Dim objSubset As SQL.SQLConditions

        With objSelect
            .Tables.Add(objCollection.TableName)
            .Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objDistinctValue)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                .Where.Add(objSubset)
            End If
        End With

        pobjConnection.Start()

        objReader = pobjConnection.Execute(objSelect)
        ObjectExistsByDistinctValue = objReader.Read

        objReader.Close()
        pobjConnection.Finished()

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Extracts the fields to save to the database from the objItem.SaveFields function.
    ''' The fields are then written to the database using either an SQL INSERT or UPDATE 
    ''' depending on whether the object has already been saved. If the collection has 
    ''' implemented IDatabaseObjects.KeyFieldName then objItem's key is also validated to 
    ''' ensure it is not null and unique within the collection. If objCollection has 
    ''' implemented IDatabaseObjects.Subset then objItem should exist within objCollection. 
    ''' If not, a duplicate key error may occur if the obItem's key is being used in 
    ''' another subset in the same table. If a record is being amended 
    ''' (IDatabaseObject.IsSaved returns true) then the function will "AND" the collection's 
    ''' IDatabaseObjects.Subset conditions and the objItem's IDatabaseObject.DistinctValue 
    ''' value to create the WHERE clause in the UPDATE statement. Therefore, the 
    ''' combination of the IDatabaseObjects.Subset and IDatabaseObject.DistinctValue 
    ''' conditions MUST identify only one record in the table. Otherwise multiple records 
    ''' will be updated with the same data. If data is only inserted and not amended 
    ''' (usually a rare occurance) then this requirement is unnecessary.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which contains or will contain the object to save.
    ''' </param>
    ''' 
    ''' <param name="objItem">
    ''' The object to save to the database. The values saved to the database are extracted from the 
    ''' SQLFieldValues object returned from IDatabaseObject.SaveFields.
    ''' </param>
    ''' 
    ''' <example> Saves a product object (Me) to the database.
    ''' <code>
    ''' Public Sub Save()
    ''' 
    '''     objDatabase.ObjectSave(NorthwindDB.Products, Me)
    ''' 
    ''' End Sub
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Sub ObjectSave( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject)

        Dim objFieldValues As SQL.SQLFieldValues
        Dim objUpdate As SQL.SQLUpdate
        Dim objInsert As SQL.SQLInsert
        Dim objSubset As SQL.SQLConditions

#If UseAutoAssignment Then
        Dim objNewGUID As System.Guid
#End If

        objFieldValues = objItem.SaveFields

        If objFieldValues Is Nothing Then
            Throw New Exceptions.DatabaseObjectsException(Misc.TypeName(objItem) & " IDatabaseObject.SaveFields not implemented")
        End If

        'Add the distinct field value if it hasn't been added via the SaveFields sub
        If Not objFieldValues.Exists(objCollection.DistinctFieldName) Then
#If UseAutoAssignment Then
            Select objCollection.DistinctFieldAutoAssignment
                Case SQL.FieldValueAutoAssignmentType.None
                    objFieldValues.Add(objCollection.DistinctFieldName, objItem.DistinctValue)
                Case SQL.FieldValueAutoAssignmentType.NewUniqueIdentifier
                    'For a new object, with a GUID that should be automatically assigned 
                    'Create a new GUID for the distinct field so that it saved for the INSERT
                    If Not objItem.IsSaved Then
                        objNewGUID = System.Guid.NewGuid
                        objFieldValues.Add(objCollection.DistinctFieldName, objNewGUID)
                    End If
            End Select
#Else
            If Not objCollection.DistinctFieldAutoIncrements Then
                objFieldValues.Add(objCollection.DistinctFieldName, objItem.DistinctValue)
            End If
#End If
        End If

#If Not Debug Then
        ItemKeyEnsureValid(objCollection, objItem, objFieldValues)
#End If

        pobjConnection.Start()

        If objItem.IsSaved Then
            objUpdate = New SQL.SQLUpdate
            objUpdate.TableName = objCollection.TableName
            objUpdate.Fields.Add(objFieldValues)
            objUpdate.Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objItem.DistinctValue)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                objUpdate.Where.Add(objSubset)
            End If

            pobjConnection.ExecuteNonQuery(objUpdate)
        Else
            objInsert = New SQL.SQLInsert
            objInsert.TableName = objCollection.TableName
            objInsert.Fields = objFieldValues
            pobjConnection.ExecuteNonQuery(objInsert)

            Dim objRollbackDistinctValue As Object = objItem.DistinctValue

#If UseAutoAssignment Then
            If objCollection.DistinctFieldAutoAssignment = SQL.FieldValueAutoAssignmentType.NewUniqueIdentifier Then
                objItem.DistinctValue = objNewGUID
            ElseIf objCollection.DistinctFieldAutoAssignment = SQL.FieldValueAutoAssignmentType.AutoIncrement Then
#Else
            If objCollection.DistinctFieldAutoIncrements Then
#End If
                objItem.DistinctValue = pobjConnection.ExecuteScalar(New SQL.SQLAutoIncrementValue)
            End If

            objItem.IsSaved = True

            If Transaction.Current IsNot Nothing Then
                Transaction.Current.EnlistVolatile(New TransactionExecuteActionOnRollback(Sub() objItem.IsSaved = False), EnlistmentOptions.None)
                Transaction.Current.EnlistVolatile(New TransactionExecuteActionOnRollback(Sub() objItem.DistinctValue = objRollbackDistinctValue), EnlistmentOptions.None)
            End If
        End If

        pobjConnection.Finished()

    End Sub

    Private Sub ItemKeyEnsureValid( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject, _
        ByVal objFieldValues As SQL.SQLFieldValues)

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect
        Dim objKeyFieldValue As Object
        Dim objSubset As SQL.SQLConditions

        'If the key field is set and the key field is specified in the object
        If objCollection.KeyFieldName <> String.Empty AndAlso objFieldValues.Exists(objCollection.KeyFieldName) Then
            objKeyFieldValue = ItemKeyFieldValue(objCollection, objItem, objFieldValues)
            EnsureKeyDataTypeValid(objKeyFieldValue)

            If TypeOf objKeyFieldValue Is String Then
                If DirectCast(objKeyFieldValue, String).Trim = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException(Misc.TypeName(objItem) & " " & objCollection.KeyFieldName & " field is Null")
                End If
            End If

            objSelect = New SQL.SQLSelect

            With objSelect
                .Tables.Add(objCollection.TableName)
                .Fields.Add(objCollection.KeyFieldName)
                .Where.Add(objCollection.KeyFieldName, SQL.ComparisonOperator.EqualTo, objKeyFieldValue)
                objSubset = objCollection.Subset
                If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                    .Where.Add(objSubset)
                End If

                If objItem.IsSaved Then
                    .Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.NotEqualTo, objItem.DistinctValue)
                End If
            End With

            pobjConnection.Start()

            objReader = pobjConnection.Execute(objSelect)

            If objReader.Read Then
                Throw New Exceptions.ObjectAlreadyExistsException(objItem, objKeyFieldValue)
            End If

            objReader.Close()
            pobjConnection.Finished()
        End If

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an object from the collection using a unique key value. 
    ''' The key must be unique within the collection. If the collection's 
    ''' IDatabaseObjects.Subset has been implemented then the key need only be unique 
    ''' within the subset specified, not the entire database table. 
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which contains the object.
    ''' </param>
    ''' 
    ''' <param name="objKey">
    ''' The key that identifies the object with the collection. The key is the value of 
    ''' the field defined by the collection's IDatabaseObjects.KeyFieldName.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example>
    ''' <code>
    ''' Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
    '''     Get
    ''' 
    '''         Return objDatabase.ObjectByKey(Me, strProductCode)
    ''' 
    '''     End Get
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectByKey( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objKey As Object) As IDatabaseObject

        Dim objObject As IDatabaseObject = ObjectByKeyIfExists(objCollection, objKey)

        If objObject Is Nothing Then
            Throw New Exceptions.ObjectDoesNotExistException(objCollection, objKey)
        End If

        Return objObject

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an object from the collection using a unique key value. 
    ''' The key must be unique within the collection. If the collection's 
    ''' IDatabaseObjects.Subset has been implemented then the key need only be unique 
    ''' within the subset specified, not the entire database table. 
    ''' Returns Nothing/null if the object does exist with the specified key.
    ''' This feature is what differentiates Database.ObjectByKey() from Database.ObjectByKeyExists().
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which contains the object.
    ''' </param>
    ''' 
    ''' <param name="objKey">
    ''' The key that identifies the object with the collection. The key is the value of 
    ''' the field defined by the collection's IDatabaseObjects.KeyFieldName.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example>
    ''' <code>
    ''' Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
    '''     Get
    ''' 
    '''         Return objDatabase.ObjectByKey(Me, strProductCode)
    ''' 
    '''     End Get
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectByKeyIfExists( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objKey As Object) As IDatabaseObject

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objSubset As SQL.SQLConditions
        Dim objReturnObject As IDatabaseObject

        EnsureKeyDataTypeValid(objKey)

        With objSelect
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .Where.Add(objCollection.KeyFieldName, SQL.ComparisonOperator.EqualTo, objKey)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                .Where.Add(objSubset)
            End If
        End With

        pobjConnection.Start()

        objReader = pobjConnection.Execute(objSelect)

        If objReader.Read Then
            objReturnObject = ObjectFromDataReader(objCollection, objReader)
        Else
            objReturnObject = Nothing
        End If

        objReader.Close()
        pobjConnection.Finished()

        Return objReturnObject

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' ObjectByOrdinalFirst returns the first object in the collection respectively 
    ''' filtered and sorted by the collection's IDatabaseObjects.Subset and 
    ''' IDatabaseObjects.OrderBy values. It differs from ObjectByOrdinal in that it only 
    ''' loads the first record from the database table not the entire table.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which contains the object.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example>
    ''' <code>
    ''' 'Ideal for loading default objects
    ''' Dim objDefaultSupplier As Supplier = objDatabase.ObjectByOrdinalFirst(objGlobalSuppliersInstance)
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectByOrdinalFirst( _
        ByVal objCollection As IDatabaseObjects) As IDatabaseObject

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect

        With objSelect
            'only select the first row of the recordset
            .Top = 1
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .Where = objCollection.Subset
            .OrderBy = objCollection.OrderBy
        End With

        pobjConnection.Start()

        objReader = pobjConnection.Execute(objSelect)

        If objReader.Read() Then
            ObjectByOrdinalFirst = ObjectFromDataReader(objCollection, objReader)
        Else
            Throw New Exceptions.ObjectDoesNotExistException(objCollection, "TOP 1")
        End If

        objReader.Close()
        pobjConnection.Finished()

    End Function


    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the last object in the collection respectively
    ''' filtered and sorted by the collection's IDatabaseObjects.Subset and
    ''' IDatabaseObjects.OrderBy values. It differs from ObjectByOrdinal in that it only
    ''' loads the first record from the database table not the entire table.
    ''' </summary>
    '''
    ''' <param name="objCollection">
    ''' The collection which contains the object.
    ''' </param>
    '''
    ''' <returns><see cref="IDatabaseObject" /> (DatabaseObjects.IDatabaseObject)</returns>
    '''
    ''' <example>
    ''' <code>
    ''' 'Ideal for loading default objects
    ''' Dim objDefaultSupplier As Supplier = objDatabase.ObjectByOrdinalFirst(objGlobalSuppliersInstance)
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    '''
    Public Function ObjectByOrdinalLast( _
        ByVal objCollection As IDatabaseObjects) As IDatabaseObject

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect

        With objSelect
            'only select the first row of the recordset
            .Top = 1
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .Where = objCollection.Subset

            Dim objOrderBy As SQL.SQLSelectOrderByFields = objCollection.OrderBy
            If objOrderBy IsNot Nothing Then
                objOrderBy.OrderingReverseAll()
                .OrderBy = objOrderBy
            End If
        End With

        pobjConnection.Start()

        objReader = pobjConnection.Execute(objSelect)

        If objReader.Read() Then
            ObjectByOrdinalLast = ObjectFromDataReader(objCollection, objReader)
        Else
            Throw New Exceptions.ObjectDoesNotExistException(objCollection, "TOP 1 with reversed ordering")
        End If

        objReader.Close()
        pobjConnection.Finished()

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of items in the collection. If the collection's 
    ''' IDatabaseObjects.Subset has been implemented then this function returns the 
    ''' number of records within the subset, not the entire table.
    ''' Also utilises the table joins so that any filters specified on the subset
    ''' can be used.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The object for which the number of records are returned.
    ''' </param>
    ''' 
    ''' <returns><see cref="Int32" />	(System.Int32)</returns>
    ''' 
    ''' <example> 
    ''' <code>
    ''' 'Return the number of items in this collection.
    ''' Public ReadOnly Property Count() As Integer
    '''     Get
    ''' 
    '''         Return objDatabase.ObjectsCount(Me)
    ''' 
    '''     End Get
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectsCount( _
        ByVal objCollection As IDatabaseObjects) As Integer

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect

        With objSelect
            .Where = objCollection.Subset
            .Fields.Add(String.Empty, SQL.AggregateFunction.Count)
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
        End With

        pobjConnection.Start()

        objReader = pobjConnection.Execute(objSelect)
        objReader.Read()

        ObjectsCount = CType(objReader(0), Integer)

        objReader.Close()
        pobjConnection.Finished()

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the key exists within the collection. If the collection's 
    ''' IDatabaseObjects.Subset has been set then only the subset is searched not the 
    ''' entire table.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection to search within. 
    ''' </param>
    ''' 
    ''' <param name="objKey">
    ''' The key value to search by.
    ''' </param>
    ''' 
    ''' <returns><see cref="Boolean" />	(System.Boolean)</returns>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Public Function Exists(ByVal strProductCode As String) As Boolean
    ''' 
    '''     Return objDatabase.ObjectExists(Me, strProductCode)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectExists( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objKey As Object) As Boolean

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objSubset As SQL.SQLConditions

        EnsureKeyDataTypeValid(objKey)

        With objSelect
            .Tables.Add(objCollection.TableName)
            '.Fields.Add objCollection.DistinctFieldName
            .Where.Add(objCollection.KeyFieldName, SQL.ComparisonOperator.EqualTo, objKey)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                .Where.Add(objSubset)
            End If
        End With

        pobjConnection.Start()

        objReader = pobjConnection.Execute(objSelect)
        ObjectExists = objReader.Read

        objReader.Close()
        pobjConnection.Finished()

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes an object's database record. If the collection's IDatabaseObjects.Subset 
    ''' has been implemented then the object must exist within the subset, otherwise the 
    ''' object will not be deleted. If the object has not been saved to the database the 
    ''' function will exit without executing an SQL DELETE command. After deleting the 
    ''' database record the object is set to Nothing. The calling function should receive 
    ''' the object ByRef for this to have any affect. Setting the object to Nothing  
    ''' minimises the possibility of the deleted object being used in code after 
    ''' ObjectDelete has been called.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection that contains the object to delete. If the item does not exist 
    ''' within the collection then the object will not be deleted.
    ''' </param>
    ''' 
    ''' <param name="objItem">
    ''' The object to delete. The calling function should receive this object ByRef 
    ''' as the object is set to Nothing after deletion. 
    ''' Reference Type: <see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)
    ''' </param>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Public Sub Delete(ByRef objProduct As Product)
    ''' 
    '''     objDatabase.ObjectDelete(Me, objProduct)
    '''     'objProduct will now be Nothing
    ''' 
    ''' End Sub 
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Sub ObjectDelete( _
        ByVal objCollection As IDatabaseObjects, _
        ByRef objItem As IDatabaseObject)

        If objItem.IsSaved Then
            Dim objDelete As SQL.SQLDelete = New SQL.SQLDelete
            Dim objSubset As SQL.SQLConditions

            With objDelete
                .TableName = objCollection.TableName
                .Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objItem.DistinctValue)
                objSubset = objCollection.Subset
                If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                    .Where.Add(objSubset)
                End If
            End With

            pobjConnection.Start()
            pobjConnection.ExecuteNonQuery(objDelete)
            pobjConnection.Finished()

            objItem.IsSaved = False

            If Transaction.Current IsNot Nothing Then
                Dim objItemCopy As IDatabaseObject = objItem
                Transaction.Current.EnlistVolatile(New TransactionExecuteActionOnRollback(Sub() objItemCopy.IsSaved = True), EnlistmentOptions.None)
            End If
        End If

        'The function that calls ObjectDelete objItem MUST be ByRef for this to have any effect
        objItem = Nothing

    End Sub

    Private Class TransactionExecuteActionOnRollback
        Implements IEnlistmentNotification

        Private pobjAction As Action

        Public Sub New(ByVal objAction As Action)

            If objAction Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjAction = objAction

        End Sub

        Private Sub Commit(ByVal enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.Commit

        End Sub

        Private Sub InDoubt(ByVal enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.InDoubt

        End Sub

        Private Sub Prepare(ByVal preparingEnlistment As System.Transactions.PreparingEnlistment) Implements System.Transactions.IEnlistmentNotification.Prepare

            preparingEnlistment.Prepared()

        End Sub

        Private Sub Rollback(ByVal enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.Rollback

            pobjAction.Invoke()

        End Sub

    End Class

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes all of the objects in the collection. If IDatabaseObjects.Subset 
    ''' has been implemented then only the objects within the subset are deleted, not 
    ''' the table's entire contents.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection from which all objects are to be deleted.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Sub ObjectsDeleteAll( _
        ByVal objCollection As IDatabaseObjects)

        Dim objDelete As SQL.SQLDelete = New SQL.SQLDelete

        With objDelete
            .TableName = objCollection.TableName
            .Where = objCollection.Subset
        End With

        pobjConnection.Start()
        pobjConnection.ExecuteNonQuery(objDelete)
        pobjConnection.Finished()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an IList object containing all of the collection's associated child 
    ''' objects. This function is useful when loading a set of objects for a subset or 
    ''' for use with the IEnumerable interface. 
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which contains the objects to load.
    ''' </param>
    ''' 
    ''' <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
    ''' 
    ''' <example> 
    ''' <code>
    ''' 'Can be used to provide an enumerator for use with the "For Each" clause
    ''' Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
    ''' 
    '''     Return objDatabase.ObjectsList(objGlobalProductsInstance).GetEnumerator
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectsList( _
        ByVal objCollection As IDatabaseObjects) As IList

        Dim objArrayList As IList = New ArrayList
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objReader As IDataReader

        With objSelect
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .Where = objCollection.Subset
            .OrderBy = objCollection.OrderBy
        End With

        pobjConnection.Start()
        objReader = pobjConnection.Execute(objSelect)

        While objReader.Read
            objArrayList.Add(ObjectFromDataReader(objCollection, objReader))
        End While

        objReader.Close()
        pobjConnection.Finished()

        Return objArrayList

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of IDatabaseObject objects contained within this collection.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ObjectsArray( _
        ByVal objCollection As IDatabaseObjects) As IDatabaseObject()

        Dim objList As IList = Me.ObjectsList(objCollection)
        Dim objArray(objList.Count - 1) As IDatabaseObject

        objList.CopyTo(objArray, 0)

        Return objArray

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an IDictionary object. Each key/value pair contains a key and
    ''' the object associated with the key.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which specifies the objects to load.
    ''' </param>
    ''' 
    ''' <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectsDictionary( _
        ByVal objCollection As IDatabaseObjects) As IDictionary

        Return ObjectsDictionaryBase(objCollection)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an IDictionary object. Each key/value pair contains a distinct 
    ''' value and the object associated with the distinct value.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection which specifies the objects to load.
    ''' </param>
    ''' 
    ''' <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectsDictionaryByDistinctValue( _
        ByVal objCollection As IDatabaseObjects) As IDictionary

        Return ObjectsDictionaryBase(objCollection, bKeyIsDistinctField:=True)

    End Function

    Private Function ObjectsDictionaryBase( _
        ByVal objCollection As IDatabaseObjects, _
        Optional ByVal bKeyIsDistinctField As Boolean = False) As IDictionary

        'Returns an IDictionary with the key being either the DistinctField or KeyField

        Dim objDictionary As IDictionary = New Hashtable
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objReader As IDataReader
        Dim strKeyField As String

        With objSelect
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .Where = objCollection.Subset
            .OrderBy = objCollection.OrderBy
        End With

        pobjConnection.Start()
        objReader = pobjConnection.Execute(objSelect)

        If bKeyIsDistinctField Then
            strKeyField = objCollection.DistinctFieldName
        Else
            strKeyField = objCollection.KeyFieldName
        End If

        While objReader.Read
            objDictionary.Add(objReader(strKeyField), ObjectFromDataReader(objCollection, objReader))
        End While

        objReader.Close()
        pobjConnection.Finished()

        Return objDictionary

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns a collection of objects that match the specified search criteria. 
    ''' This function utilises any subsets, ordering or table joins specified in the 
    ''' collection. To add a set of conditions to the objSearchCriteria object with 
    ''' higher precendance use the "Add(SQLConditions)" overloaded function as this will 
    ''' wrap the conditions within parentheses.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection to search within.
    ''' </param>
    ''' 
    ''' <param name="objSearchCriteria">
    ''' The criteria to search for within the collection. To add a set of conditions with 
    ''' with higher precendance use the "Add(SQLConditions)" overloaded function as this 
    ''' will wrap the conditions within parentheses.
    ''' </param>
    ''' 
    ''' <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
    ''' 
    ''' <remarks>
    ''' The following wildcard characters are used when using the LIKE operator (extract
    ''' from Microsoft Transact-SQL Reference)
    ''' 
    ''' 
    ''' <font size="1">
    ''' <table width="659" border="1" cellspacing="1" cellpadding="4">
    '''   <tr>
    '''     <th width="16%" height="20">Wildcard character</th>
    '''     <th width="22%">Description</th>
    '''     <th width="62%">Example</th>
    '''   </tr>
    '''   <tr>
    '''     <td>%</td>
    '''     <td>Any string of zero or more characters.</td>
    '''     <td>WHERE title LIKE '%computer%' finds all book titles with the word 
    '''         'computer' anywhere in the book title.</td>
    '''   </tr>
    '''   <tr>
    '''     <td>_ (underscore)</td>
    '''     <td>Any single character.</td>
    '''     <td>WHERE au_fname LIKE '_ean' finds all four-letter first names that end
    '''       with ean (Dean, Sean, and so on).</td>
    '''   </tr>
    ''' </table>
    ''' </font>
    ''' </remarks>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Public Function Search(ByVal objSearchCriteria As Object, ByVal eType As SearchType) As IList
    ''' 
    '''     Dim objConditions As SQL.SQLConditions = New SQL.SQLConditions
    ''' 
    '''     Select Case eType
    '''         Case SearchType.DescriptionPrefix
    '''             objConditions.Add("ProductName", SQL.ComparisonOperator.Like, objSearchCriteria &amp; "%")
    '''         Case SearchType.Description
    '''             objConditions.Add("ProductName", SQL.ComparisonOperator.Like, "%" &amp; objSearchCriteria &amp; "%")
    '''     End Select
    '''
    '''     Return objDatabase.ObjectsSearch(objGlobalProductsInstance, objConditions)
    '''
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Function ObjectsSearch( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objSearchCriteria As SQL.SQLConditions) As IList

        Dim objReader As IDataReader
        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objResults As ArrayList = New ArrayList

        If objSearchCriteria.IsEmpty Then
            Throw New ArgumentException("Search criteria is empty")
        End If

        With objSelect
            Dim objPrimaryTable As SQL.SQLSelectTable = .Tables.Add(objCollection.TableName)
            .Tables.Joins = objCollection.TableJoins(objPrimaryTable, .Tables)
            .OrderBy = objCollection.OrderBy
            .Where = objCollection.Subset

            If Not objSearchCriteria Is Nothing Then
                If .Where Is Nothing Then .Where = New SQL.SQLConditions
                .Where.Add(objSearchCriteria)
            End If
        End With

        pobjConnection.Start()
        objReader = pobjConnection.Execute(objSelect)

        While objReader.Read
            objResults.Add(ObjectFromDataReader(objCollection, objReader))
        End While

        objReader.Close()
        pobjConnection.Finished()

        Return objResults

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Locks the database record associated with this object by selecting and locking
    ''' the row in the database. Supported in Microsoft SQLServer, Pervasive and MySQL.
    ''' The record lock is released when the transaction is committed or rolled back.
    ''' Throws an exception if not in transaction mode.
    ''' Returns the field values from the record that has been locked.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ObjectLockRecord( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject) As SQL.SQLFieldValues

        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objSubset As SQL.SQLConditions

        With objSelect
            .PerformLocking = True
            .Tables.Add(objCollection.TableName)
            .Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objItem.DistinctValue)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                .Where.Add(objSubset)
            End If
        End With

        Dim objReader As IDataReader = Me.Transactions.Execute(objSelect)
        Dim objFieldValues As SQL.SQLFieldValues

        If objReader.Read() Then
            objFieldValues = FieldValuesFromDataReader(objCollection, objReader)
        Else
            Throw New Exceptions.ObjectDoesNotExistException(objCollection, objItem.DistinctValue)
        End If

        objReader.Close()

        Return objFieldValues

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets and returns the field value from the database record associated with the 
    ''' object and collection.
    ''' </summary>
    ''' <param name="objItem">
    ''' The object which represents the database record to be read. Specifically,
    ''' the object's distinct field name is used to determine which record to read.
    ''' </param>
    ''' <param name="strFieldName">
    ''' The name of the database field that is to be read.
    ''' </param>
    ''' <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
    ''' --------------------------------------------------------------------------------
    Public Function ObjectGetFieldValue( _
        ByVal objItem As DatabaseObject, _
        ByVal strFieldName As String) As Object

        Return Me.ObjectGetFieldValue(objItem.ParentCollection, objItem, strFieldName)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets and returns the field value from the database record associated with the 
    ''' object and collection.
    ''' </summary>
    ''' <param name="objCollection">
    ''' The collection that the object exists within. 
    ''' The function utilises the collection's subset and tablename to determine which
    ''' table and record to read.
    ''' Returns DBNull.Value if the field is NULL.
    ''' </param>
    ''' <param name="objItem">
    ''' The object which represents the database record to be read. Specifically,
    ''' the object's distinct field name is used to determine which record to read.
    ''' </param>
    ''' <param name="strFieldName">
    ''' The name of the database field that is to be read.
    ''' </param>
    ''' <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
    ''' --------------------------------------------------------------------------------
    Public Function ObjectGetFieldValue( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject, _
        ByVal strFieldName As String) As Object

        If Not objItem.IsSaved Then
            Throw New Exceptions.ObjectDoesNotExistException(objItem)
        End If

        Dim objSelect As SQL.SQLSelect = New SQL.SQLSelect
        Dim objSubset As SQL.SQLConditions

        With objSelect
            .Fields.Add(strFieldName)
            .Tables.Add(objCollection.TableName)
            .Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objItem.DistinctValue)
            objSubset = objCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                .Where.Add(objSubset)
            End If
        End With

        Me.Connection.Start()

        Dim objReader As IDataReader = Me.Connection.Execute(objSelect)
        Dim objReadValue As Object

        If objReader.Read() Then
            objReadValue = objReader(0)
        Else
            Throw New Exceptions.ObjectDoesNotExistException(objCollection, objItem.DistinctValue)
        End If

        objReader.Close()
        Me.Connection.Finished()

        Return objReadValue

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets the field value for the database record associated with the object and
    ''' collection.
    ''' </summary>
    ''' <param name="objItem">
    ''' The object which represents the database record to be set. Specifically,
    ''' the object's distinct field name is used to determine which record to modify.
    ''' </param>
    ''' <param name="strFieldName">
    ''' The name of the database field that is to be set.
    ''' </param>
    ''' <param name="objNewValue">
    ''' The new value that the database field it to be set to.
    ''' If Nothing/null then the field is set to NULL.
    ''' </param>
    ''' <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved</exception>
    ''' --------------------------------------------------------------------------------
    Public Sub ObjectSetFieldValue( _
        ByVal objItem As DatabaseObject, _
        ByVal strFieldName As String, _
        ByVal objNewValue As Object)

        Me.ObjectSetFieldValue(objItem.ParentCollection, objItem, strFieldName, objNewValue)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets the field value for the database record associated with the object and
    ''' collection.
    ''' </summary>
    ''' <param name="objCollection">
    ''' The collection that the object exists within. 
    ''' The function utilises the collection's subset and tablename to determine which
    ''' table and record to modify.
    ''' </param>
    ''' <param name="objItem">
    ''' The object which represents the database record to be set. Specifically,
    ''' the object's distinct field name is used to determine which record to modify.
    ''' </param>
    ''' <param name="strFieldName">
    ''' The name of the database field that is to be set.
    ''' </param>
    ''' <param name="objNewValue">
    ''' The new value that the database field it to be set to.
    ''' If Nothing/null then the field is set to NULL.
    ''' </param>
    ''' <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved</exception>
    ''' --------------------------------------------------------------------------------
    Public Sub ObjectSetFieldValue( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject, _
        ByVal strFieldName As String, _
        ByVal objNewValue As Object)

        If Not objItem.IsSaved Then
            Throw New Exceptions.ObjectDoesNotExistException(objItem)
        End If

        Dim objSubset As SQL.SQLConditions
        Dim objUpdate As New SQL.SQLUpdate
        objUpdate.TableName = objCollection.TableName
        objUpdate.Fields.Add(strFieldName, objNewValue)
        objUpdate.Where.Add(objCollection.DistinctFieldName, SQL.ComparisonOperator.EqualTo, objItem.DistinctValue)
        objSubset = objCollection.Subset
        If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
            objUpdate.Where.Add(objSubset)
        End If

        Me.Connection.ExecuteNonQueryWithConnect(objUpdate)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Creates and initializes an object from the current record of a DataRow object.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection associated with the IDataReader object. 
    ''' </param>
    ''' 
    ''' <param name="objRow">
    ''' The data to be copied into a new IDatabaseObject object.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Shared Function ObjectFromDataRow( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objRow As System.Data.DataRow) As IDatabaseObject

        Dim objTable As Data.DataTable = objRow.Table.Clone
        objTable.Rows.Add(objRow)
        Return ObjectFromFieldValues(objCollection, FieldValuesFromDataReader(objCollection, objTable.CreateDataReader))

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Creates and initializes an object from the current record of an IDataReader object.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection associated with the IDataReader object. 
    ''' </param>
    ''' 
    ''' <param name="objReader">
    ''' The data to be copied into a new IDatabaseObject object.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public Shared Function ObjectFromDataReader( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objReader As IDataReader) As IDatabaseObject

        Return ObjectFromFieldValues(objCollection, FieldValuesFromDataReader(objCollection, objReader))

    End Function

    Private Shared Function FieldValuesFromDataReader( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objReader As IDataReader) As SQL.SQLFieldValues

        Dim strFieldName As String
        Dim strTablePrefix As String
        Dim objFieldValues As SQL.SQLFieldValues

        'check that the distinct field name exists
        If Not FieldExists(objReader, objCollection.DistinctFieldName) Then
            Throw New Exceptions.DatabaseObjectsException(DirectCast(objCollection, Object).GetType.Name & ".DistinctFieldName '" & objCollection.DistinctFieldName & "' is invalid")
        End If

        objFieldValues = New SQL.SQLFieldValues
        strTablePrefix = objCollection.TableName & "."

        'Copy the recordset values into the SQL.SQLFieldValues object
        For intIndex As Integer = 0 To objReader.FieldCount - 1
            'If the recordset has been loaded with a join then it may be prefixed with
            'the table name - this is the case with Microsoft Access
            'If so remove the table name if the table prefix is the same as objCollection.TableName
            'All of the other joined fields with tablename prefixes on the fields will remain. This is ok considering
            'most of the time an inner join has been performed where the field names are equal in the 2 joined tables
            strFieldName = objReader.GetName(intIndex)
            If strFieldName.IndexOf(strTablePrefix) = 0 Then
                objFieldValues.Add(strFieldName.Substring(strTablePrefix.Length), objReader(intIndex))
            Else
                objFieldValues.Add(strFieldName, objReader(intIndex))
            End If
        Next

        Return objFieldValues

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Creates and initializes an object from the values contained in an SQLFieldValues object. 
    ''' This function is generally used from within an IDatabaseObject.Load function when 
    ''' the IDatabaseObjects.TableJoins function has been implemented.
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection associated with the field values.
    ''' </param>
    ''' 
    ''' <param name="objFieldValues">
    ''' The data container from which to load a new object.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    Public Shared Function ObjectFromFieldValues( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objFieldValues As SQL.SQLFieldValues) As IDatabaseObject

        Dim objItem As IDatabaseObject

        If TypeOf objCollection Is IDatabaseObjectsMultipleSubclass Then
            objItem = DirectCast(objCollection, IDatabaseObjectsMultipleSubclass).ItemInstanceForSubclass(objFieldValues)
        Else
            objItem = objCollection.ItemInstance
        End If

        ObjectLoad(objCollection, objItem, objFieldValues)

        Return objItem

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes an existing object with values from a set of database fields.
    ''' Specifically, sets the IDatbaseObject.IsSaved property to true, 
    ''' sets the IDatbaseObject.DistinctValue using the provided data and 
    ''' calls IDatbaseObject.LoadFields().
    ''' </summary>
    ''' 
    ''' <param name="objItem">
    ''' The object into which the data should be copied into.
    ''' </param>
    ''' 
    ''' <param name="objFieldValues">
    ''' The data container that contains the data to be copied into the object.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Public Shared Sub ObjectLoad( _
        ByVal objItem As DatabaseObject, _
        ByVal objFieldValues As SQL.SQLFieldValues)

        ObjectLoad(objItem.ParentCollection, objItem, objFieldValues)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes an existing object with values from a set of database fields.
    ''' Specifically, sets the IDatbaseObject.IsSaved property to true, 
    ''' sets the IDatbaseObject.DistinctValue using the provided data and 
    ''' calls IDatbaseObject.LoadFields().
    ''' </summary>
    ''' 
    ''' <param name="objItem">
    ''' The object into which the data should be copied into.
    ''' </param>
    ''' 
    ''' <param name="objData">
    ''' The data container that contains the data to be copied into the object.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Public Shared Sub ObjectLoad( _
        ByVal objItem As DatabaseObject, _
        ByVal objData As IDataReader)

        ObjectLoad(objItem, FieldValuesFromDataReader(objItem.ParentCollection, objData))

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes an existing object with values from a set of database fields.
    ''' Specifically, sets the IDatbaseObject.IsSaved property to true, 
    ''' sets the IDatbaseObject.DistinctValue using the provided data and 
    ''' calls IDatbaseObject.LoadFields().
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection associated with the field values. This is not used 
    ''' to create an object - but to set the distinct field for the object using the
    ''' IDatabaseObjects.DistinctFieldName property.
    ''' </param>
    ''' 
    ''' <param name="objFieldValues">
    ''' The data container that contains the data to be copied into the object.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Public Shared Sub ObjectLoad( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject, _
        ByVal objFieldValues As SQL.SQLFieldValues)

        If objFieldValues Is Nothing Then
            Throw New ArgumentNullException
        End If

        objItem.IsSaved = True
        objItem.DistinctValue = objFieldValues(objCollection.DistinctFieldName).Value
        objItem.LoadFields(objFieldValues)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes an existing object with values from a set of database fields.
    ''' Specifically, sets the IDatbaseObject.IsSaved property to true, 
    ''' sets the IDatbaseObject.DistinctValue using the provided data and 
    ''' calls IDatbaseObject.LoadFields().
    ''' </summary>
    ''' 
    ''' <param name="objCollection">
    ''' The collection associated with the field values. This is not used 
    ''' to create an object - but to set the distinct field for the object using the
    ''' IDatabaseObjects.DistinctFieldName property.
    ''' </param>
    ''' 
    ''' <param name="objData">
    ''' The data container that contains the data to be copied into the object.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Public Shared Sub ObjectLoad( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject, _
        ByVal objData As IDataReader)

        ObjectLoad(objCollection, objItem, FieldValuesFromDataReader(objCollection, objData))

    End Sub

    Private Shared Function FieldExists( _
        ByVal objReader As IDataReader, _
        ByVal strFieldName As String) As Boolean

        Dim bExists As Boolean
        Dim strReaderFieldName As String

        For intIndex As Integer = 0 To objReader.FieldCount - 1
            strReaderFieldName = objReader.GetName(intIndex)
            If strReaderFieldName.IndexOf("."c) >= 0 Then
                strReaderFieldName = strReaderFieldName.Split("."c)(1)
            End If
            If String.Compare(strReaderFieldName, strFieldName, ignoreCase:=True) = 0 Then
                bExists = True
                Exit For
            End If
        Next

        Return bExists

    End Function

    Private Sub EnsureKeyDataTypeValid(ByVal objKey As Object)

        'If TypeOf objKey Is Object Then
        '    Throw New ArgumentInvalidDataTypeException(objKey)
        'End If

    End Sub

    Private Function ItemKeyFieldValue( _
        ByVal objCollection As IDatabaseObjects, _
        ByVal objItem As IDatabaseObject, _
        ByVal objFieldValues As SQL.SQLFieldValues) As Object

        'On the rare occurance that the KeyField is the same as the DistinctField
        'then the key value may not have been set in the Save and therefore be
        'available in the objFieldValues collection. In which case the
        'key has to be extracted from the objItem.DistinctField.
        Dim objKeyFieldValue As Object

        If String.Compare(objCollection.DistinctFieldName, objCollection.KeyFieldName, ignoreCase:=True) = 0 Then
            objKeyFieldValue = objItem.DistinctValue
        Else
            objKeyFieldValue = objFieldValues(objCollection.KeyFieldName).Value
        End If

        Return objKeyFieldValue

    End Function

    Public ReadOnly Property Transactions() As TransactionsClass
        Get

            Return pobjTransactions

        End Get
    End Property

    Public ReadOnly Property Connection() As ConnectionController
        Get

            Return pobjConnection

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Provides a mechanism for starting beginning, commiting and rolling back transactions.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Class TransactionsClass

        Private pobjConnectionController As ConnectionController

        Friend Sub New(ByVal objConnection As ConnectionController)

            pobjConnectionController = objConnection

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Notifies that a transaction has begun and that all modifications to the database
        ''' are only committed after a call to Commit. Alternatively, if 
        ''' Rollback is called then all changes are aborted. To execute other 
        ''' statements for the transaction call the Execute and ExecuteNonQuery functions. 
        ''' Because all changes to the database must be executed on the same connection 
        ''' DatabaseObjects maintains an open connection until the Commit or Rollback functions 
        ''' are called. When transactions are not being used connections are opened and closed 
        ''' for each SQL statement executed i.e. (INSERT/UPDATE/SELECT...).
        ''' </summary>
        '''
        ''' <example> 
        ''' <code>
        ''' Public Sub Shadows Save()
        ''' 
        '''     Mybase.ParentDatabase.Transactions.Begin()
        ''' 
        '''     MyBase.Save
        '''     Me.Details.Save
        '''
        '''     'Execute any other statements here via
        '''     'MyBase.ParentDatabase.Transactions.Execute()...
        '''
        '''     Mybase.ParentDatabase.Transactions.Commit()
        '''
        ''' End sub
        ''' </code>
        ''' </example>
        ''' --------------------------------------------------------------------------------
        Public Sub Begin()

            pobjConnectionController.BeginTransaction(Data.IsolationLevel.Unspecified)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Notifies that a transaction has begun and that all modifications to the database
        ''' are only committed after a call to Commit. Alternatively, if 
        ''' Rollback is called then all changes are aborted. To execute other 
        ''' statements for the transaction call the Execute and ExecuteNonQuery functions. 
        ''' Because all changes to the database must be executed on the same connection 
        ''' DatabaseObjects maintains an open connection until the Commit or Rollback functions 
        ''' are called. When transactions are not being used connections are opened and closed 
        ''' for each SQL statement executed i.e. (INSERT/UPDATE/SELECT...).
        ''' </summary>
        '''
        ''' <example> 
        ''' <code>
        ''' Public Sub Shadows Save()
        ''' 
        '''     Mybase.ParentDatabase.Transactions.Begin()
        ''' 
        '''     MyBase.Save
        '''     Me.Details.Save
        '''
        '''     'Execute any other statements here via
        '''     'MyBase.ParentDatabase.Transactions.Execute()...
        '''
        '''     Mybase.ParentDatabase.Transactions.Commit()
        '''
        ''' End sub
        ''' </code>
        ''' </example>
        ''' --------------------------------------------------------------------------------
        Public Sub Begin(ByVal eIsolationLevel As Data.IsolationLevel)

            pobjConnectionController.BeginTransaction(eIsolationLevel)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Indicates whether a transaction is currently in progress, caused by a call to Begin.
        ''' Once the final, outer Commit or Rollback has been called false is returned.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public ReadOnly Property InProgress() As Boolean
            Get

                Return pobjConnectionController.InTransactionMode

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Commits all statements that have been executed after the Begin() call.
        ''' The database connection is closed after the transaction has been committed.
        ''' </summary>
        '''
        ''' <example> 
        ''' <code>
        ''' Public Sub Shadows Save()
        ''' 
        '''     Mybase.ParentDatabase.Transactions.Begin()
        ''' 
        '''     MyBase.Save
        '''     Me.Details.Save
        '''
        '''     'Execute any other statements here via
        '''     'MyBase.ParentDatabase.Transactions.Execute()...
        '''
        '''     Mybase.ParentDatabase.Transactions.Commit()
        '''
        ''' End sub
        ''' </code>
        ''' </example>
        ''' --------------------------------------------------------------------------------
        Public Sub Commit()

            pobjConnectionController.CommitTransaction()

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Rollsback all statements that have been executed after the Begin() call.
        ''' The database connection is closed after the transaction has been rolled back.
        ''' Subsequent calls are ignored.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Sub Rollback()

            pobjConnectionController.RollbackTransaction()

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Allows an SQL statement to be executed on the current transaction connection.
        ''' If a transaction is not in progress an exception will occur.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Sub ExecuteNonQuery(ByVal objStatement As SQL.ISQLStatement)

            If Not pobjConnectionController.InTransactionMode Then
                Throw New Exceptions.DatabaseObjectsException("Not in transaction mode")
            End If

            pobjConnectionController.ExecuteNonQuery(objStatement)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Allows the SQL statements to be executed on the current transaction connection.
        ''' If a transaction is not in progress an exception will occur.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Sub ExecuteNonQuery(ByVal objStatements As SQL.ISQLStatement())

            Me.ExecuteNonQuery(New SQL.SQLStatements(objStatements))

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Allows an SQL statement to be executed on the current transaction connection.
        ''' If a transaction is not in progress an exception will occur.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function Execute(ByVal objStatement As SQL.ISQLStatement) As IDataReader

            If Not pobjConnectionController.InTransactionMode Then
                Throw New Exceptions.DatabaseObjectsException("Not in transaction mode")
            End If

            Return pobjConnectionController.Execute(objStatement)

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Allows the SQL statements to be executed on the current transaction connection.
        ''' If a transaction is not in progress an exception will occur.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function Execute(ByVal objStatements As SQL.ISQLStatement()) As IDataReader

            Return Me.Execute(New SQL.SQLStatements(objStatements))

        End Function

    End Class

    ''' <summary>
    ''' Manages transactions when used with the System.Transactions.TransactionScope class.
    ''' </summary>
    Private Class TransactionsManager
        Implements System.Transactions.IEnlistmentNotification

        Private pobjConnectionController As ConnectionController

        Public Sub New(ByVal objConnectionController As ConnectionController)

            If objConnectionController Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjConnectionController = objConnectionController

        End Sub

        Friend Sub BeginTransaction(ByVal eIsolationLevel As System.Transactions.IsolationLevel)

            pobjConnectionController.BeginTransaction(TranslateIsolationLevel(eIsolationLevel))

        End Sub

        ''' <summary>
        ''' Translate the enum equivalents from the System.Transactions.IsolationLevel enum 
        ''' to the System.Data.IsolationLevel enum.
        ''' </summary>
        Private Function TranslateIsolationLevel(ByVal eIsolationLevel As System.Transactions.IsolationLevel) As System.Data.IsolationLevel

            Select Case eIsolationLevel
                Case System.Transactions.IsolationLevel.ReadCommitted
                    Return System.Data.IsolationLevel.ReadCommitted

                Case System.Transactions.IsolationLevel.ReadUncommitted
                    Return System.Data.IsolationLevel.ReadUncommitted

                Case System.Transactions.IsolationLevel.RepeatableRead
                    Return System.Data.IsolationLevel.RepeatableRead

                Case System.Transactions.IsolationLevel.Serializable
                    Return System.Data.IsolationLevel.Serializable

                Case System.Transactions.IsolationLevel.Snapshot
                    Return System.Data.IsolationLevel.Snapshot

                Case System.Transactions.IsolationLevel.Unspecified
                    Return Data.IsolationLevel.Unspecified

                Case Else
                    Throw New NotImplementedException(eIsolationLevel.ToString)
            End Select

        End Function

        Private Sub Prepare(ByVal preparingEnlistment As System.Transactions.PreparingEnlistment) Implements System.Transactions.IEnlistmentNotification.Prepare

            preparingEnlistment.Prepared()

        End Sub

        Private Sub Commit(ByVal enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.Commit

            pobjConnectionController.CommitTransaction()

        End Sub

        Private Sub InDoubt(ByVal enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.InDoubt

        End Sub

        Private Sub Rollback(ByVal enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.Rollback

            pobjConnectionController.RollbackTransaction()

        End Sub

    End Class

    Public Class ConnectionController

        ''' <summary>
        ''' Fired after an SQL statement has been executed.
        ''' Useful for trace logging of SQL statements executed.
        ''' </summary>
        Public Event StatementExecuted(ByVal objStatement As SQL.ISQLStatement)

        Private pobjConnection As IDbConnection
        'This is only used for SQLServerCompact databases
        Private pobjTransactions As New System.Collections.Generic.Stack(Of IDbTransaction)

        Private peConnectionType As ConnectionType
        Private pintConnectionCount As Integer = 0
        Private pintTransactionLevel As Integer = 0
        Private pobjLastTransaction As Transaction = Nothing
        Private pbInTransactionMode As Boolean = False

        Friend Sub New( _
            ByVal strConnectionString As String, _
            ByVal eConnectionType As ConnectionType)

            Me.New(CreateConnection(strConnectionString), eConnectionType)

        End Sub

        Friend Sub New( _
            ByVal objConnection As IDbConnection, _
            ByVal eConnectionType As ConnectionType)

            If objConnection Is Nothing Then
                Throw New ArgumentNullException("Connection")
            End If

            pobjConnection = objConnection
            peConnectionType = eConnectionType
            SQL.SQLStatement.DefaultConnectionType = eConnectionType

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Indicates that either Execute or ExecuteNonQuery is going to be used
        ''' and that a connection needs to be opened if one is not already.
        ''' If in transaction mode (Transactions.Begin has been called) then the 
        ''' current connection is left opened.
        ''' If not in transaction mode then a new connection is opened.
        ''' Always call Start before using Execute or ExecuteNonQuery whether in
        ''' transaction mode or not as the library will open the connection if necessary.
        ''' Once the connection is no longer required, call Database.Connection.Finished() 
        ''' or Database.Transactions.Commit().
        ''' If called within a TransactionScope() then a database transaction is automatically started.
        ''' </summary>
        ''' <remarks>
        ''' This feature is particularly relevant when database records are locked 
        ''' during transactions. If a second connection outside of the DatabaseObjects
        ''' library is used then a possible deadlock could occur. Using the Execute
        ''' and ExecuteNonQuery functions means that a new connection is opened if not
        ''' in transaction mode or the current transaction connection is used - thereby
        ''' avoiding potential deadlocks.
        ''' </remarks>
        ''' --------------------------------------------------------------------------------
        Public Sub Start()

            Dim objCurrentTransaction As Transaction = Transaction.Current

            If objCurrentTransaction Is Nothing Then
                ' When not called from with a TransactionScope ensure a connection is open.
                ConnectionStart()
            Else
                'Only create a new transaction when in a different TransactionScope
                If pobjLastTransaction <> objCurrentTransaction Then
                    ' When called from within a TransactionScope ensure a connection is open and a transaction started.
                    StartLocalTransaction(objCurrentTransaction)
                    pobjLastTransaction = objCurrentTransaction
                End If
            End If

        End Sub

        Private Sub StartLocalTransaction(ByVal objCurrentTransaction As System.Transactions.Transaction)

            ' When called from within a TransactionScope enlist a resource manager and begin the transaction
            Dim objTransactionManager As TransactionsManager = New TransactionsManager(Me)

            If peConnectionType = ConnectionType.MicrosoftAccess Then
                'If this is the default isolation level
                If objCurrentTransaction.IsolationLevel = System.Transactions.IsolationLevel.Serializable Then
                    'Isolation levels are not supported in Microsoft Access therefore ignore the isolation level specified
                    objTransactionManager.BeginTransaction(System.Transactions.IsolationLevel.Unspecified)
                Else
                    Throw New Exceptions.DatabaseObjectsException("Isolation Level " & objCurrentTransaction.IsolationLevel.ToString & " is not supported for Microsoft Access")
                End If
            Else
                objTransactionManager.BeginTransaction(objCurrentTransaction.IsolationLevel)
            End If

            objCurrentTransaction.EnlistVolatile(objTransactionManager, EnlistmentOptions.None)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Indicates that either Execute or ExecuteNonQuery have been called and are not 
        ''' going to be called again.
        ''' If in transaction mode (Transactions.Begin has been called) then the 
        ''' connection is left open until Transactions.Commit or Rollback is called.
        ''' If not in transaction mode then the connection is closed.
        ''' Always call Finished when finished using the connection whether in
        ''' transaction mode or not as the library will close the connection if necessary.
        ''' If called within a TransactionScope() then the database transaction will be committed
        ''' at the end of the TransactionScope().
        ''' </summary>
        ''' <remarks>
        ''' This feature is particularly relevant when database records are locked 
        ''' during transactions. If a second connection outside of the DatabaseObjects
        ''' library is used then a possible deadlock could occur. Using the Execute
        ''' and ExecuteNonQuery functions means that a new connection is opened if not
        ''' in transaction mode or the current transaction connection is used - thereby
        ''' avoiding potential deadlocks.
        ''' </remarks>
        ''' --------------------------------------------------------------------------------
        Public Sub Finished()

            Dim objCurrentTransaction As Transaction = Transaction.Current

            If objCurrentTransaction Is Nothing Then
                ' When not called from with a TransactionScope the connection as finished.
                ConnectionFinished()
            Else
                ' When within a TransactionScope then the TransactionManager will commit and 
                ' close the connection as necessary at the end of the TransactionScope's using statement.
            End If

        End Sub

        ''' <summary>
        ''' Allows the use of TransactionScope around a set of Execute, ExecuteNonQuery or ExecuteScalar
        ''' with out requiring an explicit call to .Start() as it should be implied from the fact that
        ''' the statements are wrapped in a TransactionScope() using statement.
        ''' </summary>
        Private Sub StartIfInTransactionScope()

            If pobjConnection.State = ConnectionState.Closed AndAlso Transaction.Current IsNot Nothing Then
                ' This will cause the transaction to start
                Me.Start()
            End If

        End Sub

        Private Sub ConnectionStart()

            If pintConnectionCount = 0 Then
                pobjConnection.Open()
            End If

            pintConnectionCount += 1

        End Sub

        Private Sub ConnectionFinished()

            pintConnectionCount -= 1

            If pintConnectionCount <= 0 Then
                pintConnectionCount = 0
                If pobjConnection.State = ConnectionState.Closed Then
                    Throw New Exceptions.DatabaseObjectsException("Connection has already been closed.")
                End If
                pobjConnection.Close()
            End If

        End Sub

        Friend Sub BeginTransaction(ByVal eIsolationLevel As Data.IsolationLevel)

            ConnectionStart()

            Select Case peConnectionType
                Case ConnectionType.SQLServerCompactEdition
                    Dim objTransaction As IDbTransaction = pobjConnection.BeginTransaction(eIsolationLevel)
                    'Simulate that SET TRANSACTION ISOLATION LEVEL has been called
                    pobjTransactions.Push(objTransaction)

                    If eIsolationLevel <> Data.IsolationLevel.Unspecified Then
                        RaiseEvent StatementExecuted(New SQL.SQLSetTransactionIsolationLevel(eIsolationLevel))
                    End If
                    'Simulate that BEGIN TRANSACTION has been called
                    RaiseEvent StatementExecuted(New SQL.SQLBeginTransaction())
                Case Else
                    If eIsolationLevel <> Data.IsolationLevel.Unspecified Then
                        ExecuteNonQuery(New SQL.SQLSetTransactionIsolationLevel(eIsolationLevel))
                    End If
                    ExecuteNonQuery(New SQL.SQLBeginTransaction)
            End Select

            pintTransactionLevel += 1

        End Sub

        Friend Sub CommitTransaction()

            Select Case peConnectionType
                Case ConnectionType.SQLServerCompactEdition
                    ' Compact edition does not directly support use of COMMIT TRANSACTION
                    Dim objTransaction As IDbTransaction = pobjTransactions.Pop
                    objTransaction.Commit()
                    'Simulate that COMMIT TRANSACTION has been called
                    RaiseEvent StatementExecuted(New SQL.SQLCommitTransaction())
                Case Else
                    ExecuteNonQuery(New SQL.SQLCommitTransaction)
            End Select

            ConnectionFinished()
            pintTransactionLevel -= 1

        End Sub

        Friend Sub RollbackTransaction()

            If pintTransactionLevel <= 0 Then
                Throw New MethodAccessException("A transaction has not been started")
            End If

            Select Case peConnectionType
                Case ConnectionType.SQLServerCompactEdition
                    ' Compact edition does not directly support use of ROLLBACK TRANSACTION
                    Dim objTransaction As IDbTransaction = pobjTransactions.Pop
                    objTransaction.Rollback()
                    'Simulate that COMMIT TRANSACTION has been called
                    RaiseEvent StatementExecuted(New SQL.SQLRollbackTransaction())
                Case Else
                    ExecuteNonQuery(New SQL.SQLRollbackTransaction)
            End Select

            ConnectionFinished()
            pintTransactionLevel -= 1

        End Sub

        Friend ReadOnly Property InTransactionMode() As Boolean
            Get

                Return pintTransactionLevel > 0

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statement. 
        ''' Returns Nothing/null if no record was selected, otherwise the first field from the
        ''' returned result.
        ''' ConnectionController.Start must be called prior to and 
        ''' ConnectionController.Finished afterwards or the statement be wrapped in a TransactionScope().
        ''' Otherwise the connection will not be correctly closed.
        ''' If wrapped in a TransactionScope call then Start is implicitly called if it has
        ''' not been called previously with the transaction scope.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function ExecuteScalar(ByVal objSQLStatement As SQL.ISQLStatement) As Object

            StartIfInTransactionScope()

            Dim objDataReader As IDataReader = ExecuteInternal(pobjConnection, objSQLStatement)

            If objDataReader.Read() Then
                Return objDataReader(0)
            Else
                Return Nothing
            End If

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statement. 
        ''' Returns Nothing/null if no record was selected, otherwise the first field from the
        ''' returned result.
        ''' ConnectionController.Start and 
        ''' ConnectionController.Finished are automatically called.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function ExecuteScalarWithConnect(ByVal objSQLStatement As SQL.ISQLStatement) As Object

            Me.Start()

            Dim objDataReader As IDataReader = ExecuteInternal(pobjConnection, objSQLStatement)
            If objDataReader.Read() Then
                ExecuteScalarWithConnect = objDataReader(0)
            Else
                ExecuteScalarWithConnect = Nothing
            End If

            Me.Finished()

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statement. 
        ''' ConnectionController.Start must be called prior to and 
        ''' ConnectionController.Finished afterwards or the statement be wrapped in a TransactionScope().
        ''' Otherwise the connection will not be correctly closed.
        ''' If wrapped in a TransactionScope call then Start is implicitly called if it has
        ''' not been called previously with the transaction scope.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function Execute(ByVal objSQLStatement As SQL.ISQLStatement) As IDataReader

            StartIfInTransactionScope()

            Return ExecuteInternal(pobjConnection, objSQLStatement)

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statements. 
        ''' ConnectionController.Start must be called prior to and 
        ''' ConnectionController.Finished afterwards or the statement be wrapped in a TransactionScope().
        ''' Otherwise the connection will not be correctly closed.
        ''' If wrapped in a TransactionScope call then Start is implicitly called if it has
        ''' not been called previously with the transaction scope.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function Execute(ByVal objSQLStatements As SQL.ISQLStatement()) As IDataReader

            StartIfInTransactionScope()

            Return ExecuteInternal(pobjConnection, New SQL.SQLStatements(objSQLStatements))

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statement. 
        ''' ConnectionController.Start must be called prior to and 
        ''' ConnectionController.Finished afterwards or the statement be wrapped in a TransactionScope().
        ''' Otherwise the connection will not be correctly closed.
        ''' If wrapped in a TransactionScope call then Start is implicitly called if it has
        ''' not been called previously with the transaction scope.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function ExecuteNonQuery(ByVal objSQLStatement As SQL.ISQLStatement) As Integer

            StartIfInTransactionScope()

            Return ExecuteNonQueryInternal(pobjConnection, objSQLStatement)

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statements. 
        ''' ConnectionController.Start must be called prior to and 
        ''' ConnectionController.Finished afterwards or the statement be wrapped in a TransactionScope().
        ''' Otherwise the connection will not be correctly closed.
        ''' If wrapped in a TransactionScope call then Start is implicitly called if it has
        ''' not been called previously with the transaction scope.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function ExecuteNonQuery(ByVal objSQLStatements As SQL.ISQLStatement()) As Integer

            StartIfInTransactionScope()

            Return ExecuteNonQueryInternal(pobjConnection, New SQL.SQLStatements(objSQLStatements))

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes an SQL statement on a new connection from the connection pool or 
        ''' if in transaction mode the current connection.
        ''' ConnectionController.Start or ConnectionController.Finished do not have to be called.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function ExecuteNonQueryWithConnect(ByVal objSQLStatement As SQL.ISQLStatement) As Integer

            Me.Start()
            ExecuteNonQueryWithConnect = Me.ExecuteNonQuery(objSQLStatement)
            Me.Finished()

        End Function

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Executes the SQL statements on a new connection from the connection pool or 
        ''' if in transaction mode the current connection.
        ''' ConnectionController.Start or ConnectionController.Finished do not have to be called.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Function ExecuteNonQueryWithConnect(ByVal objSQLStatements As SQL.ISQLStatement()) As Integer

            Me.Start()
            ExecuteNonQueryWithConnect = Me.ExecuteNonQuery(New SQL.SQLStatements(objSQLStatements))
            Me.Finished()

        End Function

        Protected Overridable Function ExecuteInternal( _
            ByVal objConnection As IDbConnection, _
            ByVal objSQLStatement As SQL.ISQLStatement) As IDataReader

            If objConnection Is Nothing Then
                Throw New Exceptions.DatabaseObjectsException("Connection is not open, call Database.Connection.Start() or Database.Transactions.Begin()")
            End If

            objSQLStatement.ConnectionType = peConnectionType
            Dim strSQL As String = objSQLStatement.SQL

            With objConnection.CreateCommand
                .CommandText = strSQL
                If pobjTransactions.Count > 0 Then
                    .Transaction = pobjTransactions.Peek  'Only used for SQLServerCompactEdition
                End If
                Try
                    ExecuteInternal = .ExecuteReader()
                Catch ex As Exception
                    Throw New Exceptions.DatabaseObjectsException("Execute failed: " & strSQL, ex)
                End Try
                RaiseEvent StatementExecuted(objSQLStatement)
            End With

        End Function

        Protected Overridable Function ExecuteNonQueryInternal( _
            ByVal objConnection As IDbConnection, _
            ByVal objSQLStatement As SQL.ISQLStatement) As Integer

            If objConnection Is Nothing Then
                Throw New Exceptions.DatabaseObjectsException("Connection is not open, call Database.Connection.Start() or Database.Transactions.Begin()")
            End If

            objSQLStatement.ConnectionType = peConnectionType
            Dim strSQL As String = objSQLStatement.SQL

            With objConnection.CreateCommand
                .CommandText = strSQL
                If pobjTransactions.Count > 0 Then
                    .Transaction = pobjTransactions.Peek  'Only used for SQLServerCompactEdition
                End If
                Try
                    ExecuteNonQueryInternal = .ExecuteNonQuery()
                Catch ex As Exception
                    Throw New Exceptions.DatabaseObjectsException("ExecuteNonQuery failed: " & strSQL, ex)
                End Try
                RaiseEvent StatementExecuted(objSQLStatement)
            End With

        End Function

        Private Shared Function CreateConnection(ByVal strConnectionString As String) As IDbConnection

            'Searches for an occurance of 'Provider='
            'If found then it is assumed to be an OLEDB connection otherwise an ODBC connection
            If SQL.Misc.GetDictionaryFromConnectionString(strConnectionString).ContainsKey("provider") Then
                Return New OleDb.OleDbConnection(strConnectionString)
            Else
                Return New Odbc.OdbcConnection(strConnectionString)
            End If

        End Function

    End Class

End Class

