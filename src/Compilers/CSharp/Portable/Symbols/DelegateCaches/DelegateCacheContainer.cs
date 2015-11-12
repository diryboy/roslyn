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
    /// <summary>
    /// This class holds ALL types of delegates that are converted from ONE target method
    /// Type parameters are generated for: <see cref="GetTypeArgumentsFromConversion(TypeSymbol, MethodSymbol)"/>
    ///     1. All type parameters from the containing types of the target method
    ///     2. All type parameters from the target method
    ///     3. The delegate type
    /// </summary>
    /// <remarks>
    /// This symbol is created while lowering, and is collected and assigned name and index before emit.
    /// </remarks>
    internal sealed class DelegateCacheContainer : NamedTypeSymbol
    {
        internal readonly DelegateCacheManager Manager;

        public override Symbol ContainingSymbol => Manager.Compilation.SourceModule.GlobalNamespace;

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override bool IsStatic => true;

        public override TypeKind TypeKind => TypeKind.Class;

        private readonly int _typeParametersCount;
        public override int Arity => _typeParametersCount;

        public readonly SynthesizedFieldSymbol DelegateField;

        private string _name;
        public override string Name => _name;

        private int _index = -1;
        public int Index => _index;

        /// <summary>
        /// This is used to deterministically sort the collected containers before emit, as they are at top level.
        /// </summary>
        public string SortKey { get; }

        private readonly ImmutableArray<TypeParameterSymbol> _builtTypeParameters;
        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _builtTypeParameters;

        private DelegateCacheContainer(
                DelegateCacheManager manager,
                MethodSymbol targetMethod,
                int arity)
        {
            Manager = manager;
            SortKey = SymbolDisplay.ToDisplayString(targetMethod, SymbolDisplayFormat.FullyQualifiedFormat);

            _typeParametersCount = arity;

            var typeParamsBuilder = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            for ( int i = 0; i < arity; i++ )
            {
                typeParamsBuilder.Add(new DelegateCacheContainerTypeParameter(this, i));
            }
            _builtTypeParameters = typeParamsBuilder.ToImmutableAndFree();

            var fieldType = TypeParameters[0];
            var fieldName = GeneratedNames.MakeDelegateCacheContainerFieldName(targetMethod.Name);
            DelegateField = new SynthesizedFieldSymbol(this, fieldType, fieldName, isPublic: true, isStatic: true);
        }

        /// <summary>
        /// Assign the name and index to be serialized in the module.
        /// </summary>
        /// <remarks>This method is only intended to be called from <see cref="DelegateCacheManager"/></remarks>
        internal void AssignNameAndIndex(string name, int index)
        {
            Debug.Assert(_name == null && name != null, "AssignNameAndIndex should only be done once.");

            _name = name;
            _index = index;
        }

        /// <remarks>This method is only intended to be called from <see cref="DelegateCacheManager"/></remarks>
        internal static DelegateCacheContainer Create(DelegateCacheManager manager, MethodSymbol targetMethod)
        {
            Debug.Assert(manager != null);
            Debug.Assert(targetMethod.IsDefinition);

            return new DelegateCacheContainer(manager, targetMethod, CalculateNeededArity(targetMethod));
        }

        private static int CalculateNeededArity(MethodSymbol targetMethod)
        {
            var arity = 1; // the type of the delegate

            arity += targetMethod.Arity;

            for (var containingType = targetMethod.ContainingType; (object)containingType != null; containingType = containingType.ContainingType)
            {
                arity += containingType.Arity;
            }

            return arity;
        }

        public static ImmutableArray<TypeSymbol> GetTypeArgumentsFromConversion(TypeSymbol conversionType, MethodSymbol targetMethod)
        {
            var typeArgumentsBuilder = ArrayBuilder<TypeSymbol>.GetInstance();

            typeArgumentsBuilder.Add(conversionType);

            if (targetMethod.Arity > 0)
            {
                var args = targetMethod.TypeArguments;
                typeArgumentsBuilder.AddRange(args);
            }

            for (var containingType = targetMethod.ContainingType; containingType != null; containingType = containingType.ContainingType)
            {
                if (containingType.Arity > 0)
                {
                    var args = containingType.TypeArguments;
                    typeArgumentsBuilder.AddRange(args);
                }
            }

            return typeArgumentsBuilder.ToImmutableAndFree();
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            Debug.Assert((object)DelegateField != null);
            return ImmutableArray.Create<Symbol>(DelegateField);
        }

        internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(compilationState, ref attributes);

            var compilation = ContainingSymbol.DeclaringCompilation;
            Debug.Assert(compilation != null, "MethodGroupConversionCacheFrame is not contained in a source module?");

            AddSynthesizedAttribute(ref attributes, compilation.TrySynthesizeAttribute(
                WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
        }

        internal override ImmutableArray<ImmutableArray<CustomModifier>> TypeArgumentsCustomModifiers => CreateEmptyTypeArgumentsCustomModifiers();

        internal override bool HasTypeArgumentsCustomModifiers => false;

        internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => StaticCast<TypeSymbol>.From(TypeParameters);

        public override NamedTypeSymbol ConstructedFrom => this;

        public override bool MightContainExtensionMethods => false;

        internal override bool MangleName => true;

        public override IEnumerable<string> MemberNames { get { yield return DelegateField.Name; } }

        internal override bool HasSpecialName => false;

        internal override bool IsComImport => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override bool ShouldAddWinRTMembers => false;

        internal override bool IsSerializable => false;

        internal override TypeLayout Layout => default(TypeLayout);

        internal override CharSet MarshallingCharSet => CharSet.Ansi;

        internal override bool HasDeclarativeSecurity => false;

        internal override bool IsInterface => false;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => Manager.System_Object;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool IsAbstract => true;

        public override bool IsSealed => true;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override AttributeUsageInfo GetAttributeUsageInfo() => default(AttributeUsageInfo);

        public override ImmutableArray<Symbol> GetMembers(string name)
            => String.Equals(name, DelegateField.Name, StringComparison.Ordinal)
                ? StaticCast<Symbol>.From(ImmutableArray.Create(DelegateField))
                : ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity) => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers() => ImmutableArray<Symbol>.Empty;

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name) => ImmutableArray<Symbol>.Empty;

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<Symbol> basesBeingResolved) => Manager.System_Object;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
            => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation() => SpecializedCollections.EmptyEnumerable<SecurityAttribute>();

        internal override ImmutableArray<string> GetAppliedConditionalSymbols() => ImmutableArray<string>.Empty;

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit() => SpecializedCollections.SingletonCollection(DelegateField);

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<Symbol> basesBeingResolved = null)
            => ImmutableArray<NamedTypeSymbol>.Empty;
    }
}
