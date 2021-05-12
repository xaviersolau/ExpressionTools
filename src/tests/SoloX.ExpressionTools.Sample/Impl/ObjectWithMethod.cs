// ----------------------------------------------------------------------
// <copyright file="ObjectWithMethod.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.ExpressionTools.Sample.Impl
{
    public class ObjectWithMethod : IObjectWithMethod
    {
        public IObjectWithMethod Self
        {
            get { return this; }
        }

        public int BasicMethod(int input)
        {
            return input;
        }

        public IObjectWithMethod TestMethod1(int input)
        {
            return input > 0 ? this : null;
        }

        public IObjectWithMethod TestMethod2(double input1, double input2)
        {
            return input1 + input2 > 0 ? this : null;
        }
    }
}
