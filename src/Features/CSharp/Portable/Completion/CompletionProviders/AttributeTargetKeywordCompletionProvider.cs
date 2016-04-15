// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Completion.Providers;
using Microsoft.CodeAnalysis.CSharp.Completion.KeywordRecommenders;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CSharp.Completion.Providers
{
    internal class AttributeTargetKeywordCompletionProvider : KeywordCompletionProvider
    {
        public AttributeTargetKeywordCompletionProvider()
            : base(KeywordRecommenders)
        {
        }

        static ImmutableArray<IKeywordRecommender<CSharpSyntaxContext>> KeywordRecommenders
            => new IKeywordRecommender<CSharpSyntaxContext>[]
            {
                new AssemblyKeywordRecommender(),
                new ModuleKeywordRecommender(),
                new TypeKeywordRecommender(),
                new TypeVarKeywordRecommender(),
                new FieldKeywordRecommender(),
                new PropertyKeywordRecommender(),
                new EventKeywordRecommender(),
                new MethodKeywordRecommender(),
                new ParamKeywordRecommender(),
                new ReturnKeywordRecommender(),
            }
            .ToImmutableArray();

        public override bool IsTriggerCharacter(SourceText text, int characterPosition, OptionSet options)
            => text[characterPosition] == '[' || CompletionUtilities.IsTriggerAfterSpaceOrStartOfWordCharacter(text, characterPosition, options);
    }
}
