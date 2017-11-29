// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Mvc.TagHelpers
{
    /// <summary>
    /// Renders a partial view.
    /// </summary>
    [HtmlTargetElement("partial", Attributes = "name", TagStructure = TagStructure.WithoutEndTag)]
    public class PartialTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly IViewBufferScope _viewBufferScope;

        public PartialTagHelper(
            IHtmlGenerator htmlGenerator,
            IViewBufferScope viewBufferScope)
        {
            _htmlGenerator = htmlGenerator ?? throw new ArgumentNullException(nameof(htmlGenerator));
            _viewBufferScope = viewBufferScope ?? throw new ArgumentNullException(nameof(viewBufferScope));
        }

        /// <summary>
        /// The name of the partial view used to create the HTML markup. Must not be <c>null</c>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A model to pass into the partial view.
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// A <see cref="ViewDataDictionary"/> to pass into the partial view.
        /// </summary>
        public ViewDataDictionary ViewData { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var viewBuffer = new ViewBuffer(_viewBufferScope, Name, ViewBuffer.PartialViewPageSize);
            using (var writer = new ViewBufferTextWriter(viewBuffer, Encoding.UTF8))
            {
                await _htmlGenerator.RenderPartialViewAsync(
                    ViewContext,
                    Name,
                    Model,
                    ViewData,
                    writer);

                output.SuppressOutput();
                output.Content.SetHtmlContent(viewBuffer);
            }
        }
    }
}
