' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Threading
Imports Microsoft.CodeAnalysis

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Friend Class MethodGroupConversionCacheFrame
        Inherits NamedTypeSymbol

        Public Overrides ReadOnly Property Arity As Integer
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property ContainingSymbol As Symbol
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property IsMustInherit As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property IsNotInheritable As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property TypeKind As TypeKind
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property CanConstruct As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property CoClassType As TypeSymbol
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property DefaultPropertyName As String
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property HasEmbeddedAttribute As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property HasSpecialName As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property IsComImport As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property IsInterface As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property IsSerializable As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property Layout As TypeLayout
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property MangleName As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property ObsoleteAttributeData As ObsoleteAttributeData
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property TypeArgumentsCustomModifiers As ImmutableArray(Of ImmutableArray(Of CustomModifier))
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides ReadOnly Property TypeSubstitution As TypeSubstitution
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Friend Overrides Sub GenerateDeclarationErrors(cancellationToken As CancellationToken)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function Construct(typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetMembers(name As String) As ImmutableArray(Of Symbol)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetTypeMembers(name As String) As ImmutableArray(Of NamedTypeSymbol)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetTypeMembers(name As String, arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function GetSecurityInformation() As IEnumerable(Of Cci.SecurityAttribute)
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function GetUnificationUseSiteDiagnosticRecursive(owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function InternalSubstituteTypeParameters(substitution As TypeSubstitution) As TypeWithModifiers
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function MakeAcyclicBaseType(diagnostics As DiagnosticBag) As NamedTypeSymbol
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function MakeAcyclicInterfaces(diagnostics As DiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function MakeDeclaredBase(basesBeingResolved As ConsList(Of Symbol), diagnostics As DiagnosticBag) As NamedTypeSymbol
            Throw New NotImplementedException()
        End Function

        Friend Overrides Function MakeDeclaredInterfaces(basesBeingResolved As ConsList(Of Symbol), diagnostics As DiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace
