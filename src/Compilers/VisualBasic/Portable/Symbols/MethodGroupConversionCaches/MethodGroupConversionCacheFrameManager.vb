' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.Symbols

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Friend Class MethodGroupConversionCacheFrameManager
        Inherits CommonMethodGroupConversionCacheFrameManager

        Public Sub New(compilation As VisualBasicCompilation)
            MyBase.New()
        End Sub

        Friend Function GetAllCreatedFrames() As ImmutableArray(Of MethodGroupConversionCacheFrame)
            Return ImmutableArray(Of MethodGroupConversionCacheFrame).Empty
        End Function

        Friend Sub AssignFrameNames(moduleBuilder As PEModuleBuilder, diagnostics As DiagnosticBag)

        End Sub

    End Class

End Namespace
