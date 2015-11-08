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
    using ConcurrentFramesDictionary = ConcurrentDictionary<MethodSymbol, MethodGroupConversionCacheFrame>;

    internal sealed class MethodGroupConversionCacheFrameManager : CommonMethodGroupConversionCacheFrameManager, IComparer<MethodGroupConversionCacheFrame>
    {
        internal CSharpCompilation Compilation { get; }

        private ConcurrentFramesDictionary _lazyFrames;

        private ConcurrentFramesDictionary Frames
        {
            get
            {
                if (_lazyFrames == null)
                {
                    for (var previousSubmission = Compilation.PreviousSubmission; previousSubmission != null; previousSubmission = previousSubmission.PreviousSubmission)
                    {
                        var previousFrames = previousSubmission.MethodGroupConversionCacheFrameManager._lazyFrames;

                        if (previousFrames != null)
                        {
                            Interlocked.CompareExchange(ref _lazyFrames, new ConcurrentFramesDictionary(previousFrames), null);
                            return _lazyFrames;
                        }
                    }

                    Interlocked.CompareExchange(ref _lazyFrames, new ConcurrentFramesDictionary(), null);
                }

                return _lazyFrames;
            }
        }

        internal MethodGroupConversionCacheFrameManager(CSharpCompilation compilation)
        {
            Compilation = compilation;
        }

        public MethodGroupConversionCacheFrame ObtainCacheFrame(MethodSymbol targetMethod)
        {
            return Frames.GetOrAdd(targetMethod, method => MethodGroupConversionCacheFrame.Create(this, method));
        }

        /// <remarks>The order should be fixed.</remarks>
        internal ImmutableArray<MethodGroupConversionCacheFrame> GetAllCreatedFrames()
        {
            var builder = ArrayBuilder<MethodGroupConversionCacheFrame>.GetInstance();

            foreach (var frame in Frames.Values)
            {
                if (ReferenceEquals(frame.Manager, this))
                {
                    builder.Add(frame);
                }
            }

            builder.Sort(this);

            return builder.ToImmutableAndFree();
        }

        public int Compare(MethodGroupConversionCacheFrame x, MethodGroupConversionCacheFrame y)
        {
            return String.CompareOrdinal(x.SortKey, y.SortKey);
        }

        /// <summary>
        /// Resets numbering in frame names.
        /// </summary>
        internal void AssignFrameNames(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            foreach (var previousTarget in moduleBuilder.GetPreviousMethodGroupConversionTargets())
            {
                Frames.GetOrAdd(previousTarget, method => MethodGroupConversionCacheFrame.Create(this, method));
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

            var nextIndex = moduleBuilder.GetNextMethodGroupConversionCacheFrameIndex();
            foreach (var frame in GetAllCreatedFrames())
            {
                string name;
                int index;
                if (!moduleBuilder.TryGetMethodGroupConversionCacheFrameName(frame, out name, out index))
                {
                    index = nextIndex++;
                    name = GeneratedNames.MakeMethodGroupConversionCacheFrameName(index, Compilation.GetSubmissionSlotIndex(), moduleId);
                }

                frame.AssignNameAndIndex(name, index);
            }
        }

        internal NamedTypeSymbol System_Object => Compilation.GetSpecialType(SpecialType.System_Object);
    }
}
