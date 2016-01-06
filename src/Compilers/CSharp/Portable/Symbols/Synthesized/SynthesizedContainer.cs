// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// A container synthesized for a lambda, iterator method, async method, or dynamic-sites.
    /// </summary>
    internal abstract class SynthesizedContainer : SynthesizedContainerBase
    {
        private readonly string _name;
        private readonly TypeMap _typeMap;
        private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

        protected SynthesizedContainer(string name, int parameterCount, bool returnsVoid)
        {
            Debug.Assert(name != null);
            _name = name;
            _typeMap = TypeMap.Empty;
            _typeParameters = CreateTypeParameters(parameterCount, returnsVoid);
        }

        protected SynthesizedContainer(string name, MethodSymbol topLevelMethod)
        {
            Debug.Assert(name != null);
            Debug.Assert(topLevelMethod != null);

            _name = name;
            _typeMap = TypeMap.Empty.WithAlphaRename(topLevelMethod, this, out _typeParameters);
        }

        protected SynthesizedContainer(string name, ImmutableArray<TypeParameterSymbol> typeParameters, TypeMap typeMap)
        {
            Debug.Assert(name != null);
            Debug.Assert(!typeParameters.IsDefault);
            Debug.Assert(typeMap != null);

            _name = name;
            _typeParameters = typeParameters;
            _typeMap = typeMap;
        }

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override bool IsStatic => false;

        public sealed override string Name => _name;

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

        internal TypeMap TypeMap => _typeMap;

        public override ImmutableArray<Symbol> GetMembers()
        {
            Symbol constructor = this.Constructor;
            return (object)constructor == null ? ImmutableArray<Symbol>.Empty : ImmutableArray.Create(constructor);
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            var ctor = Constructor;
            return ((object)ctor != null && name == ctor.Name) ? ImmutableArray.Create<Symbol>(ctor) : ImmutableArray<Symbol>.Empty;
        }

        private ImmutableArray<TypeParameterSymbol> CreateTypeParameters(int parameterCount, bool returnsVoid)
        {
            var typeParameters = ArrayBuilder<TypeParameterSymbol>.GetInstance(parameterCount + (returnsVoid ? 0 : 1));
            for (int i = 0; i < parameterCount; i++)
            {
                typeParameters.Add(new AnonymousTypeManager.AnonymousTypeParameterSymbol(this, i, "T" + (i + 1)));
            }

            if (!returnsVoid)
            {
                typeParameters.Add(new AnonymousTypeManager.AnonymousTypeParameterSymbol(this, parameterCount, "TResult"));
            }

            return typeParameters.ToImmutableAndFree();
        }
    }
}
