
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTable
        Inherits SQLSelectTableBase

        Private pstrSchemaName As String
        Private pstrName As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal strName As String)

            Me.Name = strName

        End Sub

        Public Sub New(ByVal strName As String, ByVal strAlias As String)

            Me.New(strName)
            MyBase.Alias = strAlias

        End Sub

        Public Property SchemaName As String
            Get

                Return pstrSchemaName

            End Get

            Set(ByVal value As String)

                pstrSchemaName = value

            End Set
        End Property

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                If Value.Trim = String.Empty Then
                    Throw New ArgumentNullException
                Else
                    pstrName = Value
                End If

            End Set
        End Property

        Friend Overrides Function GetPrefix() As String

            If Me.Alias <> String.Empty Then
                Return MyBase.Alias
            Else
                Return pstrName
            End If

        End Function

        Protected Overrides ReadOnly Property Source(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                If Me.Name.Trim = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("Table has not been set.")
                End If

                Dim strSQL As String = String.Empty

                If pstrSchemaName <> String.Empty Then
                    strSQL &= SQLConvertIdentifierName(pstrSchemaName, eConnectionType) & "."
                End If

                Return strSQL & SQLConvertIdentifierName(Me.Name, eConnectionType)

            End Get
        End Property

    End Class

End Namespace
