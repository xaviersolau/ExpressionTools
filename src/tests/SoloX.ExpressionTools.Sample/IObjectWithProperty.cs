// ----------------------------------------------------------------------
// <copyright file="IObjectWithProperty.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.ExpressionTools.Sample
{
    public interface IObjectWithProperty
    {
        int MyInt { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
        int[] MyArray { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
