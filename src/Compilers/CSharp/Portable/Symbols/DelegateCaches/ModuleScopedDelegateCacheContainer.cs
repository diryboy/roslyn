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

        private static readonly SymbolDisplayFormat MethodSortKeyFormat = new SymbolDisplayFormat
        (
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
        );

        private readonly TypeSymbol _delegateType;
        private readonly FieldSymbolsCollection _delegateFields = new FieldSymbolsCollection();

        private readonly Symbol _containingSymbol;
        public override Symbol ContainingSymbol => _containingSymbol;

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        /// <remarks>This is only intended to be used from <see cref="DelegateCacheManager"/></remarks>
        internal ModuleScopedDelegateCacheContainer(CSharpCompilation compilation, TypeSymbol delegateType)
            : base(DelegateCacheContainerKind.ModuleScopedConcrete)
        {
            _containingSymbol = compilation.SourceModule.GlobalNamespace;
            _delegateType = delegateType;
            SortKey = delegateType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        internal override FieldSymbol ObtainCacheField(SyntheticBoundNodeFactory factory, TypeSymbol delegateType, MethodSymbol targetMethod)
        {
            Debug.Assert(_delegateType == delegateType);

            return _delegateFields.GetOrAdd(targetMethod, m => new ModuleScopedDelegateCacheContainerField(this, m.Name, _delegateType, m.ToDisplayString(MethodSortKeyFormat)));
        }

        /// <remarks>The order should be fixed.</remarks>
        internal ImmutableArray<ModuleScopedDelegateCacheContainerField> GetAllCreatedFields()
        {
            Debug.Assert(_delegateFields != null && _delegateFields.Count != 0);

            var builder = ArrayBuilder<ModuleScopedDelegateCacheContainerField>.GetInstance();

            foreach (var field in _delegateFields.Values)
            {
                builder.Add(field);
            }
            builder.Sort(this);

            return builder.ToImmutableAndFree();
        }

        public int Compare(ModuleScopedDelegateCacheContainerField x, ModuleScopedDelegateCacheContainerField y) => String.CompareOrdinal(x.SortKey, y.SortKey);

        public override ImmutableArray<Symbol> GetMembers() => StaticCast<Symbol>.From(GetAllCreatedFields());

        /// <summary>
        /// Assign the name to be serialized in the module.
        /// </summary>
        /// <remarks>This method is only intended to be called from <see cref="DelegateCacheManager"/></remarks>
        internal void AssignName(string name)
        {
            Debug.Assert(_name == null && name != null, "Name should only be assigned once.");

            _name = name;
        }
    }
}
