
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On
Option Infer On

''' --------------------------------------------------------------------------------
''' <summary>
''' Populates all fields and properties marked with the 
''' FieldMapping and FieldMappingObjectHook attributes with a set of 
''' </summary>
''' --------------------------------------------------------------------------------
Public Module DatabaseObjectUsingAttributesHelper

    Private Const pcePropertyFieldScope As Reflection.BindingFlags = _
        Reflection.BindingFlags.Instance Or _
        Reflection.BindingFlags.Public Or _
        Reflection.BindingFlags.NonPublic Or _
        Reflection.BindingFlags.DeclaredOnly

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Saves an object with fields or properties marked with DatabaseObjects.FieldMapping attributes 
    ''' to the SQL.SQLFieldValues object for saving to the database.
    ''' </summary>
    ''' <param name="objObject">
    ''' The generic object that does not implement IDatabaseObject
    ''' but contains FieldMapping attributes.
    ''' </param>
    ''' <remarks>
    ''' Typically used from within the overridden SaveFields function when loading a generic object normally
    ''' marked with the FieldMappingObjectHook attribute.
    ''' </remarks>
    ''' --------------------------------------------------------------------------------
    Public Function SaveFieldsForObject(ByVal objObject As Object) As SQL.SQLFieldValues

        If objObject Is Nothing Then
            Throw New ArgumentNullException
        End If

        Return SaveFieldsForObject(objObject, objObject.GetType)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Recurses through the lowest order base classes up to the highest order classes loading fields 
    ''' and hooked objects on each object.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function SaveFieldsForBaseTypes(ByVal objObject As Object, ByVal objType As System.Type) As SQL.SQLFieldValues

        Dim objFieldValues As New SQL.SQLFieldValues

        'Need to check that the type is not at the object level which can occur when traversing through hooked objects
        If Not objType.Assembly.Equals(Reflection.Assembly.GetExecutingAssembly) AndAlso Not objType.Equals(GetType(Object)) Then
            objFieldValues.Add(SaveFieldsForBaseTypes(objObject, objType.BaseType))
            objFieldValues.Add(SaveFieldsForObject(objObject, objType))
            objFieldValues.Add(SaveFieldsForHookedObjects(objObject, objType))
        End If

        Return objFieldValues

    End Function

    Private Function SaveFieldsForHookedObjects(ByVal objObject As Object, ByVal objType As System.Type) As SQL.SQLFieldValues

        Dim objAttributes As Object()
        Dim objFieldObject As Object
        Dim objFieldValues As New SQL.SQLFieldValues

        'Search for fields that have the FieldMappingObjectHookAttribute
        For Each objField As Reflection.FieldInfo In objType.GetFields(pcePropertyFieldScope)
            objAttributes = objField.GetCustomAttributes(GetType(FieldMappingObjectHookAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMappingObjectHook As FieldMappingObjectHookAttribute In objAttributes
                    If objField.FieldType.IsValueType Then
                        Throw New Exceptions.DatabaseObjectsException("Field " & objField.Name & " marked with FieldMappingObjectHook attribute on value type - must be a class type")
                    Else
                        objFieldObject = objField.GetValue(objObject)
                        If objFieldObject Is Nothing Then Throw New Exceptions.DatabaseObjectsException("Field " & objField.Name & " marked with " & GetType(FieldMappingObjectHookAttribute).Name & " is Nothing")
                        objFieldValues.Add(SaveFieldsForBaseTypes(objFieldObject, objFieldObject.GetType))
                    End If
                Next
            End If
        Next

        'Search for properties that have the FieldMappingObjectHookAttribute
        For Each objProperty As Reflection.PropertyInfo In objType.GetProperties(pcePropertyFieldScope)
            objAttributes = objProperty.GetCustomAttributes(GetType(FieldMappingObjectHookAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMappingObjectHook As FieldMappingObjectHookAttribute In objAttributes
                    If objProperty.CanRead Then
                        If objProperty.PropertyType.IsValueType Then
                            Throw New Exceptions.DatabaseObjectsException("Property " & objProperty.Name & " marked with FieldMappingObjectHook attribute on value type - must be a class type")
                        Else
                            objFieldObject = objProperty.GetValue(objObject, Nothing)
                            If objFieldObject Is Nothing Then Throw New Exceptions.DatabaseObjectsException("Property " & objProperty.Name & " marked with " & GetType(FieldMappingObjectHookAttribute).Name & " is Nothing")
                            objFieldValues.Add(SaveFieldsForBaseTypes(objFieldObject, objFieldObject.GetType))
                        End If
                    End If
                Next
            End If
        Next

        Return objFieldValues

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Saves an object with fields or properties marked with DatabaseObjects.FieldMapping attributes 
    ''' to the fields values, which can be used to save to the database.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Private Function SaveFieldsForObject(ByVal objObject As Object, ByVal objType As System.Type) As SQL.SQLFieldValues

        Dim objAttributes As Object()
        Dim objFieldValues As New SQL.SQLFieldValues

        'Search for fields that have the FieldMappingAttribute
        For Each objField As Reflection.FieldInfo In objType.GetFields(pcePropertyFieldScope)
            objAttributes = objField.GetCustomAttributes(GetType(FieldMappingAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMapping As FieldMappingAttribute In objAttributes
                    Try
                        'If an enum field then convert the enum to an integer 
                        If objField.FieldType.IsEnum Then
                            objFieldValues.Add(objFieldMapping.FieldName, CInt(objField.GetValue(objObject)))
                            'Get the distinct value if this is an ObjectReference field
                        ElseIf TypeOf objField.GetValue(objObject) Is ObjectReference Then
                            objFieldValues.Add(objFieldMapping.FieldName, DirectCast(objField.GetValue(objObject), ObjectReference).DistinctValue)
                        Else
                            objFieldValues.Add(objFieldMapping.FieldName, objField.GetValue(objObject))
                        End If
                    Catch ex As Exception
                        Throw New Exceptions.DatabaseObjectsException("Field '" & objType.Name & "." & objField.Name & "' could not be read.", ex)
                    End Try
                Next
            End If
        Next

        'Search for properties that have the FieldMappingAttribute
        For Each objProperty As Reflection.PropertyInfo In objType.GetProperties(pcePropertyFieldScope)
            objAttributes = objProperty.GetCustomAttributes(GetType(FieldMappingAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMapping As FieldMappingAttribute In objAttributes
                    If objProperty.CanRead Then
                        Try
                            'If an enum field then convert the enum to an integer 
                            If objProperty.PropertyType.IsEnum Then
                                objFieldValues.Add(objFieldMapping.FieldName, CInt(objProperty.GetValue(objObject, Nothing)))
                            Else
                                objFieldValues.Add(objFieldMapping.FieldName, objProperty.GetValue(objObject, Nothing))
                            End If
                        Catch ex As Exception
                            Throw New Exceptions.DatabaseObjectsException("Property '" & objType.Name & "." & objProperty.Name & "' could not be read.", ex)
                        End Try
                    End If
                Next
            End If
        Next

        Return objFieldValues

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Performs loading an object marked with the FieldMappingObjectHook attribute.
    ''' Loads an object with fields or properties marked with DatabaseObjects.FieldMapping attributes 
    ''' with the fields from the database.
    ''' </summary>
    ''' <param name="objObject">
    ''' The generic object that does not implement IDatabaseObject
    ''' but contains FieldMapping attributes.
    ''' </param>
    ''' <remarks>
    ''' Typically used from within the overridden LoadFields function when loading a generic object normally
    ''' marked with the FieldMappingObjectHook attribute.
    ''' </remarks>
    ''' --------------------------------------------------------------------------------
    Public Sub LoadFieldsForObject(ByVal objObject As Object, ByVal objFields As SQL.SQLFieldValues)

        If objObject Is Nothing Then
            Throw New ArgumentNullException
        End If

        LoadFieldsForObject(objObject, objObject.GetType, objFields)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Recurses through the lowest order base classes up to the highest order classes loading fields 
    ''' and hooked objects on each object.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Sub LoadFieldsForBaseTypes(ByVal objObject As Object, ByVal objType As System.Type, ByVal objFields As SQL.SQLFieldValues)

        'Skip classes in the DatabaseObjects assembly or class instances of type Object
        'Need to check that the type is not at the object level which can occur when traversing through hooked objects
        If Not objType.Assembly.Equals(Reflection.Assembly.GetExecutingAssembly) AndAlso Not objType.Equals(GetType(Object)) Then
            LoadFieldsForBaseTypes(objObject, objType.BaseType, objFields)
            LoadFieldsForObject(objObject, objType, objFields)
            LoadFieldsForHookedObjects(objObject, objType, objFields)
            LoadFieldsForObjectReferenceEarlyBinding(objObject, objType, objFields)
        End If

    End Sub

    Private Sub LoadFieldsForObjectReferenceEarlyBinding(ByVal objectToLoad As Object, ByVal type As System.Type, ByVal fieldValues As SQL.SQLFieldValues)

        For Each earlyBindingField In ObjectReferenceEarlyBinding.GetObjectReferenceEarlyBindingFields(type)
            Dim objectReference = DirectCast(earlyBindingField.Field.GetValue(objectToLoad), ObjectReference)
            objectReference.Object = Database.ObjectFromFieldValues(objectReference.ParentCollection, fieldValues)
        Next

    End Sub

    Private Sub LoadFieldsForHookedObjects(ByVal objObject As Object, ByVal objType As System.Type, ByVal objFields As SQL.SQLFieldValues)

        Dim objAttributes As Object()
        Dim objFieldObject As Object

        'Search for fields that have the FieldMappingObjectHookAttribute
        For Each objField As Reflection.FieldInfo In objType.GetFields(pcePropertyFieldScope)
            objAttributes = objField.GetCustomAttributes(GetType(FieldMappingObjectHookAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMappingObjectHook As FieldMappingObjectHookAttribute In objAttributes
                    If objField.FieldType.IsValueType Then
                        Throw New Exceptions.DatabaseObjectsException("Field " & objField.Name & " marked with FieldMappingObjectHook attribute on value type - must be a class type")
                    Else
                        objFieldObject = objField.GetValue(objObject)
                        If objFieldObject Is Nothing Then Throw New Exceptions.DatabaseObjectsException("Field " & objField.Name & " marked with " & GetType(FieldMappingObjectHookAttribute).Name & " is Nothing")
                        LoadFieldsForBaseTypes(objFieldObject, objFieldObject.GetType, objFields)
                    End If
                Next
            End If
        Next

        'Search for properties that have the FieldMappingObjectHookAttribute
        For Each objProperty As Reflection.PropertyInfo In objType.GetProperties(pcePropertyFieldScope)
            objAttributes = objProperty.GetCustomAttributes(GetType(FieldMappingObjectHookAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMappingObjectHook As FieldMappingObjectHookAttribute In objAttributes
                    If objProperty.CanRead Then
                        If objProperty.PropertyType.IsValueType Then
                            Throw New Exceptions.DatabaseObjectsException("Property " & objProperty.Name & " marked with FieldMappingObjectHook attribute on value type - must be a class type")
                        Else
                            objFieldObject = objProperty.GetValue(objObject, Nothing)
                            If objFieldObject Is Nothing Then Throw New Exceptions.DatabaseObjectsException("Property " & objProperty.Name & " marked with " & GetType(FieldMappingObjectHookAttribute).Name & " is Nothing")
                            LoadFieldsForBaseTypes(objFieldObject, objFieldObject.GetType, objFields)
                        End If
                    End If
                Next
            End If
        Next

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads an object with fields or properties marked with DatabaseObjects.FieldMapping attributes 
    ''' with the fields from the database.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Private Sub LoadFieldsForObject(ByVal objObject As Object, ByVal objType As System.Type, ByVal objFields As SQL.SQLFieldValues)

        Dim objAttributes As Object()

        'Search for fields that have the FieldMappingAttribute
        For Each objField As Reflection.FieldInfo In objType.GetFields(pcePropertyFieldScope)
            objAttributes = objField.GetCustomAttributes(GetType(FieldMappingAttribute), True)
            If Not objAttributes Is Nothing Then
                For Each objFieldMapping As FieldMappingAttribute In objAttributes
                    If Not objFields(objFieldMapping.FieldName).Value Is DBNull.Value Then
                        Try
                            'If an enum field then convert the integer to the enum equivalent
                            If objField.FieldType.IsEnum Then
                                objField.SetValue(objObject, _
                                    System.Enum.ToObject(objField.FieldType, _
                                    objFields(objFieldMapping.FieldName).Value))
                                'Set the distinct value if this is an ObjectReference field
                            ElseIf TypeOf objField.GetValue(objObject) Is ObjectReference Then
                                DirectCast(objField.GetValue(objObject), ObjectReference).DistinctValue = objFields(objFieldMapping.FieldName).Value
                            Else
                                objField.SetValue(objObject, objFields(objFieldMapping.FieldName).Value)
                            End If
                        Catch ex As Exception
                            Throw New Exceptions.DatabaseObjectsException("Field '" & objField.Name & "' could not be set.", ex)
                        End Try
                    End If
                Next
            End If
        Next

        'Search for properties that have the FieldMappingAttribute
        For Each objProperty As Reflection.PropertyInfo In objType.GetProperties(pcePropertyFieldScope)
            objAttributes = objProperty.GetCustomAttributes(GetType(FieldMappingAttribute), True)
            'Diagnostics.Debug.Print("Traversing : " & objType.Name & "." & objProperty.Name)
            If Not objAttributes Is Nothing Then
                For Each objFieldMapping As FieldMappingAttribute In objAttributes
                    If objProperty.CanWrite Then
                        If Not objFields(objFieldMapping.FieldName).Value Is DBNull.Value Then
                            Try
                                'If an enum field then convert the integer to the enum equivalent
                                If objProperty.PropertyType.IsEnum Then
                                    objProperty.SetValue(objObject, _
                                        System.Enum.ToObject(objProperty.PropertyType, _
                                        objFields(objFieldMapping.FieldName).Value), _
                                        Nothing)
                                Else
                                    objProperty.SetValue(objObject, objFields(objFieldMapping.FieldName).Value, Nothing)
                                End If
                            Catch ex As Exception
                                Throw New Exceptions.DatabaseObjectsException("Property '" & objProperty.Name & "' could not be set.", ex)
                            End Try
                        End If
                    End If
                Next
            End If
        Next

    End Sub

End Module
