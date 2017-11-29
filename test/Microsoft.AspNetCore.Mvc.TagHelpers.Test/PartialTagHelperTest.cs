// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TestCommon;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.WebEncoders.Testing;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.TagHelpers
{
    public class PartialTagHelperTest
    {
        [Fact]
        public async Task ProcessAsync_RendersPartialView()
        {
            // Arrange
            var expected = "Hello world!";
            var bufferScope = new TestViewBufferScope();
            var generator = new Mock<IHtmlGenerator>();
            var partialName = "_Partial";
            var model = new object();
            var viewData = new ViewDataDictionary(new TestModelMetadataProvider(), new ModelStateDictionary());
            var viewContext = new ViewContext
            {
                ViewData = new ViewDataDictionary(new TestModelMetadataProvider(), new ModelStateDictionary()),
            };

            generator.Setup(m => m.RenderPartialViewAsync(viewContext, partialName, model, viewData, It.IsAny<TextWriter>()))
                .Callback((ViewContext _, string __, object ___, ViewDataDictionary ____, TextWriter writer) =>
                {
                    writer.Write(expected);
                })
                .Returns(Task.CompletedTask)
                .Verifiable();

            var tagHelper = new PartialTagHelper(generator.Object, bufferScope)
            {
                Name = partialName,
                Model = model,
                ViewContext = viewContext,
                ViewData = viewData,
            };
            var tagHelperContext = GetTagHelperContext();
            var output = GetTagHelperOutput();

            // Act
            await tagHelper.ProcessAsync(tagHelperContext, output);

            // Assert
            generator.Verify();
            var content = HtmlContentUtilities.HtmlContentToString(output.Content, new HtmlTestEncoder());
            Assert.Equal(expected, content);
        }

        private static TagHelperContext GetTagHelperContext()
        {
            return new TagHelperContext(
                "partial",
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
        }

        private static TagHelperOutput GetTagHelperOutput()
        {
            return new TagHelperOutput(
                "partial",
                new TagHelperAttributeList(),
                (_, __) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        }
    }
}
