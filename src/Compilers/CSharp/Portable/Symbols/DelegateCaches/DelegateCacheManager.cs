// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    using ModuleScopedContainerCollection = ConcurrentDictionary<TypeSymbol, ModuleScopedDelegateCacheContainer>;

    internal sealed class DelegateCacheManager : CommonDelegateCacheManager, IComparer<ModuleScopedDelegateCacheContainer>
    {
        private ModuleScopedContainerCollection _lazyModuleScopedContainers;

        private ModuleScopedContainerCollection ModuleScopedContainers
        {
            get
            {
                if (_lazyModuleScopedContainers == null)
                {
                    Interlocked.CompareExchange(ref _lazyModuleScopedContainers, new ModuleScopedContainerCollection(), null);
                }

                return _lazyModuleScopedContainers;
            }
        }

        internal DelegateCacheContainer ObtainModuleScopedContainer(CSharpCompilation compilation, TypeSymbol delegateType)
        {
            return ModuleScopedContainers.GetOrAdd(delegateType, t => new ModuleScopedDelegateCacheContainer(compilation, t));
        }

        internal void AssignNames(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            if (_lazyModuleScopedContainers == null)
            {
                return;
            }

            var moduleId = moduleBuilder.GetModuleIdForSynthesizedTopLevelTypes();
            var generation = moduleBuilder.CurrentGenerationOrdinal;

            var containerIndex = 0;
            foreach (var container in GetModuleScopedContainers())
            {
                container.AssignName(GeneratedNames.MakeDelegateCacheContainerName(containerIndex, generation, moduleId));
                containerIndex++;

                var fieldIndex = 0;
                foreach (var field in container.GetAllCreatedFields())
                {
                    field.AssignName(GeneratedNames.MakeDelegateCacheContainerFieldName(field.TargetMethodName, fieldIndex));
                    fieldIndex++;
                }
            }
        }

        /// <remarks>The order should be fixed.</remarks>
        internal ImmutableArray<ModuleScopedDelegateCacheContainer> GetModuleScopedContainers()
        {
            var containers = _lazyModuleScopedContainers;
            if (containers != null)
            {
                var builder = ArrayBuilder<ModuleScopedDelegateCacheContainer>.GetInstance();

                foreach (var container in containers.Values)
                {
                    builder.Add(container);
                }

                builder.Sort(this);

                return builder.ToImmutableAndFree();
            }

            return ImmutableArray<ModuleScopedDelegateCacheContainer>.Empty;
        }

        public int Compare(ModuleScopedDelegateCacheContainer x, ModuleScopedDelegateCacheContainer y) => String.CompareOrdinal(x.SortKey, y.SortKey);
    }
}
