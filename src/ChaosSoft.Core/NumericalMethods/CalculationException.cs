using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ChaosSoft.Core.NumericalMethods
{
    /// <summary>
    /// Thrown in case when error in calculations occurs.
    /// </summary>
    [Serializable]
    public class CalculationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculationException"/> class.
        /// </summary>
        public CalculationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculationException"/> class with specified message.
        /// </summary>
        /// <param name="exception">exception message</param>
        public CalculationException(string exception)
            : base(exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculationException"/> class with specified serialization info and context.
        /// </summary>
        /// <param name="info">serialization info</param>
        /// <param name="context">streaming context</param>
        protected CalculationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Set serialization info
        /// </summary>
        /// <param name="info">serialization info</param>
        /// <param name="context">streaming context</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) =>
            base.GetObjectData(info, context);
    }
}
