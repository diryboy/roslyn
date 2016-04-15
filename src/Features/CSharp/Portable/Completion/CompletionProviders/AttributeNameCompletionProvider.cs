// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Completion.Providers;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Completion.Providers
{
    internal class AttributeNameCompletionProvider : SymbolCompletionProvider
    {
        public override bool IsTriggerCharacter(SourceText text, int characterPosition, OptionSet options) => text[characterPosition] == '[';

        protected override Task<IEnumerable<ISymbol>> GetSymbolsWorker(AbstractSyntaxContext context, int position, OptionSet options, CancellationToken cancellationToken)
        {
            if (context.IsAttributeNameContext)
            {
                return base.GetSymbolsWorker(context, position, options, cancellationToken);
            }
            else
            {
                return SpecializedTasks.EmptyEnumerable<ISymbol>();
            }
        }
    }
}
