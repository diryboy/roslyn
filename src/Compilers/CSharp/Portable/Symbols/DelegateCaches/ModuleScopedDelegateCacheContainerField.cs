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
    internal class ModuleScopedDelegateCacheContainerField : SynthesizedFieldSymbol
    {
        /// Why the _name is not readonly? Why the SortKey?
        /// See comments above <see cref="ModuleScopedDelegateCacheContainer._name"/>
        private string _name;

        public ModuleScopedDelegateCacheContainerField(DelegateCacheContainer container, string targetMethodName, TypeSymbol type, string sortkey)
            : base(container, type, sortkey, isPublic: true, isReadOnly: false, isStatic: true)
        {
            TargetMethodName = targetMethodName;
            SortKey = sortkey;
        }

        public override string Name => _name;

        public string SortKey { get; }

        // Save this value for a debug friendly name.
        public string TargetMethodName { get; }

        /// <remarks>This method is only intended to be called from <see cref="ModuleScopedDelegateCacheContainer"/></remarks>.
        internal void AssignName(string name)
        {
            Debug.Assert(name != null && _name == null, "Name should only be assigned once.");

            _name = name;
        }
    }
}
