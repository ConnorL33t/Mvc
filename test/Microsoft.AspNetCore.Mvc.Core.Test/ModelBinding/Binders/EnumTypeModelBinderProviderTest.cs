// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class EnumTypeModelBinderProviderTest
    {
        [Theory]
        [InlineData(typeof(CarType))]
        [InlineData(typeof(CarType?))]
        public void ReturnsBinder_ForEnumType(Type modelType)
        {
            // Arrange
            var provider = new EnumTypeModelBinderProvider(new TestMvcOptions(allowBindingUndefinedValueToEnumType: true).Value);
            var context = new TestModelBinderProviderContext(modelType);

            // Act
            var result = provider.GetBinder(context);

            // Assert
            Assert.IsType<EnumTypeModelBinder>(result);
        }

        [Theory]
        [InlineData(typeof(CarOptions))]
        [InlineData(typeof(CarOptions?))]
        public void ReturnsBinder_ForFlagsEnumType(Type modelType)
        {
            // Arrange
            var provider = new EnumTypeModelBinderProvider(new TestMvcOptions(allowBindingUndefinedValueToEnumType: true).Value);
            var context = new TestModelBinderProviderContext(modelType);

            // Act
            var result = provider.GetBinder(context);

            // Assert
            Assert.IsType<EnumTypeModelBinder>(result);
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(int?))]
        public void DoesNotReturnBinder_ForNonEnumTypes(Type modelType)
        {
            // Arrange
            var provider = new EnumTypeModelBinderProvider(new TestMvcOptions(allowBindingUndefinedValueToEnumType: false).Value);
            var context = new TestModelBinderProviderContext(modelType);

            // Act
            var result = provider.GetBinder(context);

            // Assert
            Assert.Null(result);
        }

        enum CarType
        {
            Sedan,
            Coupe
        }

        [Flags]
        public enum CarOptions
        {
            SunRoof = 0x01,
            Spoiler = 0x02,
            FogLights = 0x04,
            TintedWindows = 0x08,
        }

        private class TestMvcOptions : IOptions<MvcOptions>
        {
            private readonly MvcOptions _options;

            public TestMvcOptions(bool allowBindingUndefinedValueToEnumType)
            {
                _options = new MvcOptions();
                _options.AllowBindingUndefinedValueToEnumType = allowBindingUndefinedValueToEnumType;
            }

            public MvcOptions Value
            {
                get
                {
                    return _options;
                }
            }
        }
    }
}
