' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLCreateView
        Inherits SQLStatement

        Private pstrName As String
        Private pobjSelect As SQLSelect

        Public Sub New()

        End Sub

        Public Sub New(ByVal strViewName As String, ByVal objSelectStatement As SQLSelect)

            Me.Name = strViewName
            Me.Select = objSelectStatement

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                If String.IsNullOrEmpty(Value) Then
                    Throw New ArgumentNullException()
                End If

                pstrName = Value

            End Set
        End Property

        Public Property [Select] As SQLSelect
            Get

                Return pobjSelect

            End Get

            Set(value As SQLSelect)

                If value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjSelect = value

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                If String.IsNullOrEmpty(Me.Name) Then
                    Throw New Exceptions.DatabaseObjectsException("View name has not been set")
                ElseIf pobjSelect Is Nothing Then
                    Throw New Exceptions.DatabaseObjectsException("Select statement has not been set")
                End If

                Return _
                    "CREATE VIEW " & _
                    SQLConvertIdentifierName(Me.Name, Me.ConnectionType) & " AS " & _
                    pobjSelect.SQL

            End Get
        End Property

    End Class

End Namespace
