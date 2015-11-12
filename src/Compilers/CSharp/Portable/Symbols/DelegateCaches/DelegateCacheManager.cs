// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class DelegateCacheManager : CommonDelegateCacheManager, IComparer<DelegateCacheContainer>
    {
        internal CSharpCompilation Compilation { get; }

        private ConcurrentDictionary<MethodSymbol, DelegateCacheContainer> _lazyContainers;

        private ConcurrentDictionary<MethodSymbol, DelegateCacheContainer> Containers
        {
            get
            {
                if (_lazyContainers == null)
                {
                    for (var previousSubmission = Compilation.PreviousSubmission; previousSubmission != null; previousSubmission = previousSubmission.PreviousSubmission)
                    {
                        var previousFrames = previousSubmission.DelegateCacheManager._lazyContainers;

                        if (previousFrames != null)
                        {
                            Interlocked.CompareExchange(ref _lazyContainers, new ConcurrentDictionary<MethodSymbol, DelegateCacheContainer>(previousFrames), null);
                            return _lazyContainers;
                        }
                    }

                    Interlocked.CompareExchange(ref _lazyContainers, new ConcurrentDictionary<MethodSymbol, DelegateCacheContainer>(), null);
                }

                return _lazyContainers;
            }
        }

        internal DelegateCacheManager(CSharpCompilation compilation)
        {
            Compilation = compilation;
        }

        public DelegateCacheContainer ObtainCacheContainer(MethodSymbol targetMethod)
        {
            return Containers.GetOrAdd(targetMethod, method => DelegateCacheContainer.Create(this, method));
        }

        /// <remarks>The order should be fixed.</remarks>
        internal ImmutableArray<DelegateCacheContainer> GetAllCreatedContainers()
        {
            var builder = ArrayBuilder<DelegateCacheContainer>.GetInstance();

            foreach (var frame in Containers.Values)
            {
                if (ReferenceEquals(frame.Manager, this))
                {
                    builder.Add(frame);
                }
            }

            builder.Sort(this);

            return builder.ToImmutableAndFree();
        }

        public int Compare(DelegateCacheContainer x, DelegateCacheContainer y)
        {
            return String.CompareOrdinal(x.SortKey, y.SortKey);
        }

        /// <summary>
        /// Resets numbering in container names?
        /// </summary>
        internal void AssignContainerNames(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            foreach (var previousTarget in moduleBuilder.GetPreviousMethodGroupConversionTargets())
            {
                Containers.GetOrAdd(previousTarget, method => DelegateCacheContainer.Create(this, method));
            }

            string moduleId;

            if (moduleBuilder.OutputKind == OutputKind.NetModule)
            {
                moduleId = moduleBuilder.Name;

                string extension = OutputKind.NetModule.GetDefaultExtension();

                if (moduleId.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    moduleId = moduleId.Substring(0, moduleId.Length - extension.Length);
                }

                moduleId = MetadataHelpers.MangleForTypeNameIfNeeded(moduleId);
            }
            else
            {
                moduleId = string.Empty;
            }

            var nextIndex = moduleBuilder.GetNextDelegateCacheContainerIndex();
            foreach (var frame in GetAllCreatedContainers())
            {
                string name;
                int index;
                if (!moduleBuilder.TryGetDelegateCacheContainerName(frame, out name, out index))
                {
                    index = nextIndex++;
                    name = GeneratedNames.MakeDelegateCacheContainerName(index, Compilation.GetSubmissionSlotIndex(), moduleId);
                }

                frame.AssignNameAndIndex(name, index);
            }
        }

        internal NamedTypeSymbol System_Object => Compilation.GetSpecialType(SpecialType.System_Object);
    }
}
