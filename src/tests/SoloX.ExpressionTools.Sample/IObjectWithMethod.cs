// ----------------------------------------------------------------------
// <copyright file="IObjectWithMethod.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.ExpressionTools.Sample
{
    public interface IObjectWithMethod
    {
        IObjectWithMethod Self { get; }

        int BasicMethod(int input);

        IObjectWithMethod TestMethod1(int input);

        IObjectWithMethod TestMethod2(double input1, double input2);
    }
}
