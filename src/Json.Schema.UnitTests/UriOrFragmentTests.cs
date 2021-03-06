﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.Json.Schema.UnitTests
{
    public class UriOrFragmentTests
    {
        [Fact(DisplayName = "UriOrFragment accepts fragment")]
        public void AcceptsFragment()
        {
            var target = new UriOrFragment("#fragment");

            target.IsFragment.Should().BeTrue();
            target.IsUri.Should().BeFalse();
            target.Fragment.Should().Be("#fragment");
        }

        [Fact(DisplayName = "UriOrFragment accepts empty fragment")]
        public void AcceptsEmptyFragment()
        {
            var target = new UriOrFragment("#");

            target.IsFragment.Should().BeTrue();
            target.IsUri.Should().BeFalse();
            target.Fragment.Should().Be("#");
        }

        [Fact(DisplayName = "UriOrFragment accepts absolute URI")]
        public void AcceptsAbsoluteUri()
        {
            var target = new UriOrFragment("http://www.example.com/products.html");

            target.IsUri.Should().BeTrue();
            target.IsFragment.Should().BeFalse();
            target.Uri.Should().Be(new Uri("http://www.example.com/products.html"));
        }

        [Fact(DisplayName = "UriOrFragment accepts relative URI")]
        public void AcceptsRelativeUri()
        {
            var target = new UriOrFragment("products.html");

            target.IsUri.Should().BeTrue();
            target.IsFragment.Should().BeFalse();
            target.Uri.Should().Be(new Uri("products.html", UriKind.Relative));
        }

        [Fact(DisplayName = "UriOrFragment accepts relative URI with fragment")]
        public void AcceptsRelativeUriWithFragment()
        {
            var target = new UriOrFragment("products.html#fragment");

            target.IsUri.Should().BeTrue();
            target.IsFragment.Should().BeFalse();
            
            // Uri.Equals compare fragments on relative URIs.
            target.Uri.Should().Be(new Uri("products.html#fragment", UriKind.Relative));
        }

        [Fact(DisplayName = "UriOrFragment accepts absolute URI with fragment")]
        public void AcceptsAbsoluteUriWithFragment()
        {
            var target = new UriOrFragment("http://host/products.html#fragment");

            target.IsUri.Should().BeTrue();
            target.IsFragment.Should().BeFalse();

            // Uri.Equals does not compare fragments on absolute URIs...
            target.Uri.Should().Be(new Uri("http://host/products.html", UriKind.Absolute));

            // ... so we'll do it by hand.
            target.Uri.Fragment.Should().Be("#fragment");
        }

        [Fact(DisplayName = "UriOrFragment throws on invalid URI")]
        public void ThrowsOnInvalidUri()
        {
            UriOrFragment target;
            Action action = () =>
            {
                // Bad character in port number. It's hard to find a good example of an invalid
                // URI because System.Uri implicitly URL-escapes invalid characters in most
                // locations. But it can't rescue a bad port number.
                target = new UriOrFragment(@"http://www.example.com:80y/products.html");
            };

            action.Should().Throw<UriFormatException>();
        }

        public static readonly TheoryData<EqualityTestCase> EqualityTestCases = new TheoryData<EqualityTestCase>
        {
            new EqualityTestCase(
                "Compare fragment to same fragment",
                "#f",
                "#f",
                true
            ),

            new EqualityTestCase(
                "Compare fragment to different fragment",
                "#f",
                "#g",
                false
            ),

            new EqualityTestCase(
                "Compare fragment to null",
                "#fragment",
                null,
                false
            ),

            new EqualityTestCase(
                "Compare fragment to URI",
                "#f",
                "f",
                false
            ),

            new EqualityTestCase(
                "Compare relative URI to same relative URI",
                "u",
                "u",
                true
            ),

            new EqualityTestCase(
                "Compare relative URI to different relative URI",
                "u",
                "v",
                false
            ),

            new EqualityTestCase(
                "Compare relative URIs with same fragments",
                "u#f",
                "u#f",
                true
            ),

            new EqualityTestCase(
                "Compare relative URIs with different fragments",
                "u#f",
                "u#g",
                false
            ),

            new EqualityTestCase(
                "Compare absolute URI to same absolute URI",
                "http://u",
                "http://u",
                true
            ),

            new EqualityTestCase(
                "Compare absolute URI to different absolute URI",
                "http://u",
                "http://v",
                false
            ),

            new EqualityTestCase(
                "Compare absolute URIs with same fragments",
                "http://u#f",
                "http://u#f",
                true
            ),

            new EqualityTestCase(
                "Compare absolute URIs with different fragments",
                "http://u#f",
                "http://u#g",
                false
            ),

            new EqualityTestCase(
                "Compare URI to null",
                "uri",
                null,
                false
            ),
        };

        [Theory(DisplayName = "UriOrFragment equality")]
        [MemberData(nameof(EqualityTestCases))]
        public void EqualityTests(EqualityTestCase test)
        {
            UriOrFragment left = test.Left == null
                ? null
                : new UriOrFragment(test.Left);

            UriOrFragment right = test.Right == null
                ? null
                : new UriOrFragment(test.Right);

            left.Equals(right).Should().Be(test.ShouldBeEqual);
            (left == right).Should().Be(test.ShouldBeEqual);
            (left != right).Should().Be(!test.ShouldBeEqual);
        }
    }
}
