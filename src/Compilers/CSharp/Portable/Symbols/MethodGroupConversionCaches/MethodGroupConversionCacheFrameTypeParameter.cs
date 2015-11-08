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
    internal sealed class MethodGroupConversionCacheFrameTypeParameter : TypeParameterSymbol
    {
        private readonly string _name;
        private readonly int _index;
        private readonly Symbol Frame;

        public MethodGroupConversionCacheFrameTypeParameter(MethodGroupConversionCacheFrame frame, int index)
        {
            Frame = frame;
            _name = "T" + StringExtensions.GetNumeral(index);
            _index = index;
        }

        public override string Name => _name;

        public override Symbol ContainingSymbol => Frame;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool HasConstructorConstraint => false;

        public override bool HasReferenceTypeConstraint => false;

        public override bool HasValueTypeConstraint => false;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override int Ordinal => _index;

        public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

        public override VarianceKind Variance => VarianceKind.None;

        internal override void EnsureAllConstraintsAreResolved() { }

        internal override ImmutableArray<TypeSymbol> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress) => ImmutableArray<TypeSymbol>.Empty;

        internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress) => null;

        internal override NamedTypeSymbol GetEffectiveBaseClass( ConsList<TypeParameterSymbol> inProgress ) => null;

        internal override ImmutableArray<NamedTypeSymbol> GetInterfaces( ConsList<TypeParameterSymbol> inProgress ) => ImmutableArray<NamedTypeSymbol>.Empty;
    }
}
