
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLFieldValue

        Private pstrName As String
        Private pobjValue As Object

        Public Sub New()


        End Sub

        Public Sub New(ByVal strFieldName As String, ByVal objNewValue As Object)

            Me.Name = strFieldName
            Me.Value = objNewValue

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public Property Value() As Object
            Get

                Return pobjValue

            End Get

            Set(ByVal Value As Object)

                pobjValue = Value

            End Set
        End Property

        Public ReadOnly Property ValueIsDBNull() As Boolean
            Get

                Return DBNull.Value.Equals(Me.Value)

            End Get
        End Property

    End Class

End Namespace
