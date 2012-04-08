' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On
Option Infer On

Imports System.Reflection
Imports System.Linq
Imports System.Collections.Generic

Friend Module ObjectReferenceEarlyBinding

    ''' <summary>
    ''' Used to maintain a cache of collection assemblies that can be traversed
    ''' </summary>
    Private Class CollectionTypesInAssemblies

        Private items As New List(Of CollectionTypesInAssembly)

        Default Public ReadOnly Property Item(assembly As Assembly) As CollectionTypesInAssembly
            Get

                Dim existingItem = items.SingleOrDefault(Function(workingItem) workingItem.Assembly.Equals(assembly))

                If existingItem Is Nothing Then
                    Dim newItem = New CollectionTypesInAssembly(assembly)
                    items.Add(newItem)
                    Return newItem
                Else
                    Return existingItem
                End If

            End Get
        End Property

    End Class

    Private Class CollectionTypesInAssembly

        Public ReadOnly Assembly As Assembly
        Private ReadOnly items As CollectionType()

        Public Sub New(assembly As Assembly)

            Me.Assembly = assembly
            Me.Items = GetCollectionTypesUsingTableAndDistinctFieldAttributes(assembly)

        End Sub

        Default Public ReadOnly Property Item(itemInstanceType As Type) As CollectionType
            Get

                Return Me.items.Single(Function(type) type.ItemInstanceType.Equals(itemInstanceType))

            End Get
        End Property

        ''' <summary>
        ''' Indicates whether there is associated collection type information for the specified item instance type.
        ''' </summary>
        Public Function Exists(itemInstanceType As Type) As Boolean

            Return Me.items.FirstOrDefault(Function(type) type.ItemInstanceType.Equals(itemInstanceType)) IsNot Nothing

        End Function

        Private Function GetCollectionTypesUsingTableAndDistinctFieldAttributes(assemblyToSearch As Assembly) As CollectionType()

            Dim collectionTypes As New List(Of CollectionType)

            For Each collectionType In GetRealCollectionTypes(assemblyToSearch)
                Dim customAttributes = collectionType.GetCustomAttributes(inherit:=True)
                If customAttributes IsNot Nothing Then
                    Dim tableAttribute = DirectCast(customAttributes.SingleOrDefault(Function(attribute) TypeOf attribute Is TableAttribute), TableAttribute)
                    If tableAttribute IsNot Nothing Then
                        Dim distinctFieldAttribute = DirectCast(customAttributes.SingleOrDefault(Function(attribute) TypeOf attribute Is DistinctFieldAttribute), DistinctFieldAttribute)
                        If distinctFieldAttribute IsNot Nothing Then        ' Technically, a DistinctFieldAttribute is always defined with a TableAttribute so this check is somewhat unnecessary
                            Dim itemInstanceAttribute = DirectCast(customAttributes.SingleOrDefault(Function(attribute) TypeOf attribute Is ItemInstanceAttribute), ItemInstanceAttribute)
                            Dim itemInstanceType As Type = Nothing
                            If itemInstanceAttribute Is Nothing Then
                                itemInstanceType = DatabaseObjectsItemInstance.GetGenericCollectionTArgument(collectionType)
                            Else
                                itemInstanceType = itemInstanceAttribute.Type
                            End If
                            collectionTypes.Add(New CollectionType With {.TableName = tableAttribute.Name, .DistinctFieldName = distinctFieldAttribute.Name, .ItemInstanceType = itemInstanceType})
                        End If
                    End If
                End If
            Next

            Return collectionTypes.ToArray

        End Function

        Private Function GetRealCollectionTypes(assemblyToSearch As Assembly) As IEnumerable(Of Type)

            Dim excludeCollectionTypeNames As New List(Of String)

#If DEBUG Then
            excludeCollectionTypeNames.Add("DatabaseObjects.UnitTests.ItemInstanceTests+TableWithNoItemInstanceAttribute")
#End If

            Return _
                assemblyToSearch.GetTypes() _
                .Where(Function(type) Not type.IsAbstract) _
                .Where(Function(type) type.IsSubclassOf(GetType(DatabaseObjects))) _
                .Where(Function(type) Not excludeCollectionTypeNames.Contains(type.FullName))

        End Function

    End Class

    Private Class CollectionType

        Public TableName As String
        Public DistinctFieldName As String
        Public ItemInstanceType As Type

    End Class

    ''' <summary>
    ''' </summary>
    ''' <returns>Returns an empty list if there are no early binding required for the item instance type, otherwise the table joins required.</returns>
    Friend Function GetTableJoins(collection As IDatabaseObjects, collectionTable As SQL.SQLSelectTable, itemInstanceType As Type) As SQL.SQLSelectTableJoins

        Static collectionTypesInAssemblies As New CollectionTypesInAssemblies

        Dim tableJoins As New SQL.SQLSelectTableJoins
        Dim leftTable As SQL.SQLSelectTableBase = collectionTable

        For Each fieldInfo In GetObjectReferenceEarlyBindingFieldsAndForBaseTypes(itemInstanceType)
            Dim fieldTypeAssembly = fieldInfo.ObjectReferenceFieldType.Assembly    ' assume that the collection type is defined in the same assembly as the item instance type

            If Not collectionTypesInAssemblies(fieldTypeAssembly).Exists(fieldInfo.ObjectReferenceFieldType) Then
                Throw New InvalidOperationException("The associated collection type for " & fieldInfo.ObjectReferenceFieldType.Name & " could not be found. The collection type must; inherit from DatabaseObjects.Generic.DatabaseObjects or have an ItemInstanceAttribute that specifies the type, have a TableAttribute and DistinctFieldAttributes and be defined in assembly " & fieldTypeAssembly.FullName)
            End If

            Dim fieldAssociatedCollectionInfo = collectionTypesInAssemblies(fieldTypeAssembly)(fieldInfo.ObjectReferenceFieldType)

            Dim rightTable As New SQL.SQLSelectTable(fieldAssociatedCollectionInfo.TableName)
            Dim tableJoin = tableJoins.Add(leftTable, SQL.SQLSelectTableJoin.Type.Inner, rightTable)
            tableJoin.Where.Add(New SQL.SQLFieldExpression(collectionTable, fieldInfo.FieldMappingName), SQL.ComparisonOperator.EqualTo, New SQL.SQLFieldExpression(New SQL.SQLSelectTable(fieldAssociatedCollectionInfo.TableName), fieldAssociatedCollectionInfo.DistinctFieldName))
            leftTable = tableJoin
        Next

        Return tableJoins

    End Function

    ''' <summary>
    ''' Traverses the item instance type and any base classes for reference to fields marked with the
    ''' ObjectReferenceEarlyBindingAttribute.
    ''' </summary>
    Private Function GetObjectReferenceEarlyBindingFieldsAndForBaseTypes(itemInstanceType As Type) As ObjectReferenceEarlyBindingField()

        Dim fields As New List(Of ObjectReferenceEarlyBindingField)

        'Loop through all of the base classes and stop when the base class is in the DatabaseObjects assembly or the root Object type is reached. 
        'The root Object type could be reached if the IDatabaseObject is implemented directly and no classes are inherited from DatabaseObjects
        While Not itemInstanceType.Assembly.Equals(Reflection.Assembly.GetExecutingAssembly) AndAlso Not itemInstanceType.Equals(GetType(Object))
            'Find ObjectReference fields with a FieldMappingAttribute - if no FieldMappingAttribute then the source field id cannot be found
            For Each fieldInfo In GetObjectReferenceEarlyBindingFields(itemInstanceType)
                fields.Add(fieldInfo)
            Next
            itemInstanceType = itemInstanceType.BaseType
        End While

        Return fields.ToArray

    End Function

    Friend Class ObjectReferenceEarlyBindingField

        ''' <summary>
        ''' The type T that is defined on the DatabaseObjects.Generic.ObjectReference class.
        ''' </summary>
        ''' <remarks></remarks>
        Public ObjectReferenceFieldType As Type
        Public Field As FieldInfo
        Public FieldMappingName As String

    End Class

    ''' <summary>
    ''' Returns all of the fields on an item instance type that have:
    ''' 1. ObjectReferenceEarlyBindingAttribute specified.
    ''' 2. FieldMappingAttribute specified
    ''' 3. Attached to a type of DatabaseObjects.Generic.ObjectReference class
    ''' </summary>
    ''' <exception cref="InvalidOperationException">
    ''' When ObjectReferenceEarlyBindingAttribute is associated incorrectly.
    ''' </exception>
    Friend Function GetObjectReferenceEarlyBindingFields(itemInstanceType As Type) As ObjectReferenceEarlyBindingField()

        Dim fieldTypes As New List(Of ObjectReferenceEarlyBindingField)

        For Each field In itemInstanceType.GetFields(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.DeclaredOnly)
            Dim customAttributes = field.GetCustomAttributes(inherit:=True).Cast(Of Attribute)()
            If customAttributes IsNot Nothing AndAlso customAttributes.SingleOrDefault(Function(attribute) TypeOf attribute Is ObjectReferenceEarlyBindingAttribute) IsNot Nothing Then
                Dim fieldMappingAttribute = DirectCast(customAttributes.SingleOrDefault(Function(attribute) TypeOf attribute Is FieldMappingAttribute), FieldMappingAttribute)
                If fieldMappingAttribute IsNot Nothing Then
                    If GenericTypesAreEqual(field.FieldType, GetType(Generic.ObjectReference(Of IDatabaseObject))) Then
                        fieldTypes.Add(New ObjectReferenceEarlyBindingField With {.Field = field, .ObjectReferenceFieldType = field.FieldType.GetGenericArguments(0), .FieldMappingName = fieldMappingAttribute.FieldName})
                    Else
                        Throw New InvalidOperationException("ObjectReferenceEarlyBindingAttribute specified on " & field.DeclaringType.Name & "." & field.Name & " is invalid because it must be specified on a field of type " & GetType(Generic.ObjectReference(Of IDatabaseObject)).FullName)
                    End If
                Else
                    Throw New InvalidOperationException("ObjectReferenceEarlyBindingAttribute specified on " & field.DeclaringType.Name & "." & field.Name & " is invalid because it must also be specified with a " & GetType(FieldMappingAttribute).FullName)
                End If
            End If
        Next

        Return fieldTypes.ToArray

    End Function

    ''' <summary>
    ''' Determines whether the base classes are equal, ignoring any type arguments specified on the types.
    ''' </summary>
    Friend Function GenericTypesAreEqual(type1 As Type, type2 As Type) As Boolean

        Return _
            type1.FullName.Split("`"c)(0).Equals(type2.FullName.Split("`"c)(0)) AndAlso
            type1.Module.Equals(type2.Module)

    End Function

End Module
