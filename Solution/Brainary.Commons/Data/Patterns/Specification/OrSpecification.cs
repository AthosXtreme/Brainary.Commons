// ===================================================================================
// Microsoft Developer & Platform Evangelism
// =================================================================================== 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license, 
// http://microsoftnlayerapp.codeplex.com/license
// ===================================================================================

namespace Brainary.Commons.Data.Patterns.Specification
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A Logic OR Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class OrSpecification<T>
         : CompositeSpecification<T>
         where T : class
    {
        #region Members

        private readonly ISpecification<T> rightSideSpecification;
        private readonly ISpecification<T> leftSideSpecification;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Default constructor for AndSpecification
        /// </summary>
        /// <param name="leftSide">Left side specification</param>
        /// <param name="rightSide">Right side specification</param>
        public OrSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
        {
            if (leftSide == null)
                throw new ArgumentNullException("leftSide");

            if (rightSide == null)
                throw new ArgumentNullException("rightSide");

            leftSideSpecification = leftSide;
            rightSideSpecification = rightSide;
        }

        #endregion

        #region Composite Specification overrides

        /// <summary>
        /// Left side specification
        /// </summary>
        public override ISpecification<T> LeftSideSpecification
        {
            get { return leftSideSpecification; }
        }

        /// <summary>
        /// Righ side specification
        /// </summary>
        public override ISpecification<T> RightSideSpecification
        {
            get { return rightSideSpecification; }
        }
        
        /// <summary>
        /// <see cref="ISpecification{TEntity}"/>
        /// </summary>
        /// <returns><see cref="ISpecification{TEntity}"/></returns>
        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            Expression<Func<T, bool>> left = leftSideSpecification.SatisfiedBy();
            Expression<Func<T, bool>> right = rightSideSpecification.SatisfiedBy();

            return left.Or(right);
        }

        #endregion
    }
}
