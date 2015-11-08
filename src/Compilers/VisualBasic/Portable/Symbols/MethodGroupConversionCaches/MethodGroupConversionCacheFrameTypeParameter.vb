' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Threading
Imports Microsoft.CodeAnalysis

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Friend Class MethodGroupConversionCacheFrameTypeParameter
        Inherits TypeParameterSymbol

        Public Overrides ReadOnly Property ContainingSymbol As Symbol
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property Ordinal As Integer
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property TypeParameterKind As TypeParameterKind
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property Variance As VarianceKind
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides Sub EnsureAllConstraintsAreResolved()
            Throw New NotImplementedException()
        End Sub
    End Class

End Namespace
