namespace ChaosSoft.Core.NumericalMethods
{
    /// <summary>
    /// Interface for numerical methods.
    /// </summary>
    public interface IDescribable
    {
        /// <summary>
        /// Gets string with help information on the numerical method.
        /// </summary>
        /// <returns>string with help</returns>
        string GetHelp();

        /// <summary>
        /// Gets results in string representation.
        /// </summary>
        /// <returns>string with help</returns>
        string GetResultAsString();
    }
}
