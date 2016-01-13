// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    using FieldSymbolsCollection = System.Collections.Concurrent.ConcurrentDictionary<MethodSymbol, ModuleScopedDelegateCacheContainerField>;

    /// <summary>
    /// This symbol is created while lowering, and is collected and have name assigned before emit.
    /// </summary>
    internal class ModuleScopedDelegateCacheContainer : DelegateCacheContainer, IComparer<ModuleScopedDelegateCacheContainerField>
    {
        // The container is a top level type, and the compilation can be parallel.
        // To ensure simple & deterministic output, we need a unique and deterministic sort key when it's created.
        // When all methods are compiled thus all containers are created, and when things are happening serially before emit, 
        // we sort the containers by their sort key, then use the indices to name them.
        private string _name;
        public override string Name => _name;
        public string SortKey { get; }

        private bool _frozen;

        private static readonly SymbolDisplayFormat MethodSortKeyFormat = new SymbolDisplayFormat
        (
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
        );

        private readonly NamedTypeSymbol _delegateType;
        private readonly FieldSymbolsCollection _delegateFields = new FieldSymbolsCollection();

        private readonly Symbol _containingSymbol;
        public override Symbol ContainingSymbol => _containingSymbol;

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        /// <remarks>This is only intended to be used from <see cref="ModuleScopedDelegateCacheManager"/></remarks>
        internal ModuleScopedDelegateCacheContainer(NamespaceSymbol globalNamespace, NamedTypeSymbol delegateType)
            : base(DelegateCacheContainerKind.ModuleScopedConcrete)
        {
            _containingSymbol = globalNamespace;
            _delegateType = delegateType;
            SortKey = delegateType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        internal override FieldSymbol ObtainCacheField(SyntheticBoundNodeFactory factory, NamedTypeSymbol delegateType, MethodSymbol targetMethod)
        {
            Debug.Assert(!_frozen);
            Debug.Assert(_delegateType == delegateType);

            return _delegateFields.GetOrAdd(targetMethod, m => new ModuleScopedDelegateCacheContainerField(this, m.Name, _delegateType, m.ToDisplayString(MethodSortKeyFormat)));
        }

        internal void AssignNamesAndFreeze(string moduleId, int index, int generation)
        {
            Debug.Assert(!_frozen);

            _name = GeneratedNames.MakeDelegateCacheContainerName(index, generation, moduleId);

            var fs = CollectAllCreatedFields();
            for (int i = 0; i < fs.Length; i++)
            {
                var f = fs[i];
                f.AssignName(GeneratedNames.MakeDelegateCacheContainerFieldName(f.TargetMethodName, i));
            }

            _frozen = true;
        }

        /// <remarks>The order should be fixed.</remarks>
        private ImmutableArray<ModuleScopedDelegateCacheContainerField> CollectAllCreatedFields()
        {
            Debug.Assert(_delegateFields != null && _delegateFields.Count != 0);

            var builder = ArrayBuilder<ModuleScopedDelegateCacheContainerField>.GetInstance();

            builder.AddRange(_delegateFields.Values);
            builder.Sort(this);

            return builder.ToImmutableAndFree();
        }

        private ImmutableArray<ModuleScopedDelegateCacheContainerField> GetAllCreatedFields()
        {
            Debug.Assert(_frozen);

            return CollectAllCreatedFields();
        }

        public int Compare(ModuleScopedDelegateCacheContainerField x, ModuleScopedDelegateCacheContainerField y) => String.CompareOrdinal(x.SortKey, y.SortKey);

        public override ImmutableArray<Symbol> GetMembers() => StaticCast<Symbol>.From(GetAllCreatedFields());
    }
}
