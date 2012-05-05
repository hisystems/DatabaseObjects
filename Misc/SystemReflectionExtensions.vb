Imports System.Runtime.CompilerServices
Imports System.IO

Friend Module SystemReflectionExtensions

    ''' <summary>
    ''' Returns the full class name and field name.
    ''' </summary>
    <Extension()>
    Public Function FullName(ByVal field As System.Reflection.FieldInfo) As String

        Return field.DeclaringType.FullName & System.Type.Delimiter & field.Name

    End Function

    ''' <summary>
    ''' Returns the full class name and property name.
    ''' </summary>
    <Extension()>
    Public Function FullName(ByVal [property] As System.Reflection.PropertyInfo) As String

        Return [property].DeclaringType.FullName & System.Type.Delimiter & [property].Name

    End Function

End Module

