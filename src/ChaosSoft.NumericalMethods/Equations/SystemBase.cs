namespace ChaosSoft.NumericalMethods.Equations
{
    /// <summary>
    /// Provides with abstraction for ode systems implementations.
    /// </summary>
    public abstract class SystemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBase"/> class with equations count.
        /// </summary>
        /// <param name="count"></param>
        protected SystemBase(int count)
        {
            Count = count;
            Rows = 1;
        }

        /// <summary>
        /// Gets equations system name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets count of original system equations.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets count of linear + linearized (if any) equations rows.
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// Sets system parameters from array.
        /// </summary>
        /// <param name="parameters">array of parameters</param>
        public abstract void SetParameters(params double[] parameters);

        /// <summary>
        /// Gets default initial conditions for the system.
        /// </summary>
        public abstract void SetInitialConditions(double[,] current);

        /// <summary>
        /// Gets derivatives from current solution based on defined equations.
        /// </summary>
        /// <param name="current">current solution</param>
        /// <param name="derivs">derivatives</param>
        /// <returns></returns>
        public abstract void GetDerivatives(double[,] current, double[,] derivs);

        /// <summary>
        /// Gets file name based on system and it's current configuration.
        /// </summary>
        /// <returns></returns>
        public abstract string ToFileName();

        /// <summary>
        /// Gets template string for system info using system parameters names.
        /// </summary>
        /// <param name="paramNames">system parameters names</param>
        /// <returns></returns>
        protected string GetInfoTemplate(params string[] paramNames)
        {
            string template = $"{Name} (";

            for (int i = 0; i < paramNames.Length; i++)
            {
                template += $" {paramNames[i]}={{{i}:0.###}}";
            }

            template += " )";

            return template;
        }

        /// <summary>
        /// Gets template string for file name using system parameters names.
        /// </summary>
        /// <param name="paramNames">system parameters names</param>
        /// <returns></returns>
        protected string GetFileNameTemplate(params string[] paramNames)
        {
            string template = $"{Name}";

            for (int i = 0; i < paramNames.Length; i++)
            {
                template += $"_{paramNames[i]}={{{i}:0.###}}";
            }

            return template;
        }
    }
}
