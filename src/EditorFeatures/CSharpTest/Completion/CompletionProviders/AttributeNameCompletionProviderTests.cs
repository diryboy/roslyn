// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Completion.Providers;
using Microsoft.CodeAnalysis.Editor.UnitTests.Workspaces;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Completion.CompletionProviders
{
    public class AttributeNameCompletionProviderTests : AbstractCSharpCompletionProviderTests
    {
        public AttributeNameCompletionProviderTests(CSharpTestWorkspaceFixture workspaceFixture)
            : base(workspaceFixture)
        {
        }

        internal override CompletionListProvider CreateCompletionProvider() => new AttributeNameCompletionProvider();
    }
}
