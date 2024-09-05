using ChaosSoft.Core.Data;
using ChaosSoft.NumericalMethods.Ode;
using ChaosSoft.NumericalMethods.Ode.Linearized;
using ChaosSoft.NumericalMethods.QrDecomposition;
using Unicorn.Taf.Core.Testing;
using Unicorn.Taf.Core.Testing.Attributes;
using Unicorn.Taf.Core.Verification;
using UnitTests.DataGenerators.OdeSys;

namespace UnitTests.Suites.Ode
{
    [Suite("ODE solvers tests")]
    public class OdeTests : TestSuite
    {
        public enum QrType
        {
            CGS,
            MGS
        }

        public enum SolverType
        {
            Discrete,
            RK4,
            RK5
        }

        private const int Steps = 500;

        public List<DataSet> ConfigurationsData() =>
            new List<DataSet>
            {
                new DataSet("Discrete + CGS", SolverType.Discrete, QrType.CGS),
                new DataSet("RK4 + MGS", SolverType.RK4, QrType.MGS),
                new DataSet("RK5 + CGS", SolverType.RK5, QrType.CGS),
            };

        [Test("ODE solution test")]
        [TestData(nameof(ConfigurationsData))]
        public void OdeSolutionTest(SolverType solverType, QrType qrType)
        {
            string expectedDataFile = GetDataFileName(solverType, qrType);

            IOdeSys odeSys = solverType == SolverType.Discrete ? 
                new HenonMap() : 
                new LorenzAttractor();

            OdeSolverBase solver = solverType switch
            {
                SolverType.Discrete => new ChaosSoft.NumericalMethods.Ode.DiscreteSolver(odeSys),
                SolverType.RK4 => new ChaosSoft.NumericalMethods.Ode.RK4(odeSys, 0.01),
                SolverType.RK5 => new ChaosSoft.NumericalMethods.Ode.RK5(odeSys, 0.01),
                _ => throw new NotImplementedException(),
            };

            if (solverType != SolverType.Discrete)
            {
                solver.SetInitialConditions(0, new double[] { 1, 1, 1 });
            }

            double[][] output = new double[odeSys.N][];

            for (int i = 0; i < odeSys.N; i++)
            {
                output[i] = new double[Steps];
            }

            for (int k = 0; k < Steps; k++)
            {
                solver.NextStep();

                for (int j = 0; j < odeSys.N; j++)
                {
                    output[j][k] = solver.Solution[j];
                }
            }

            PerformCheck(expectedDataFile, odeSys.N, output);
        }

        [Test("Linearized ODE solution test")]
        [TestData(nameof(ConfigurationsData))]
        public void LinearizedOdeSolutionTest(SolverType solverType, QrType qrType)
        {
            string expectedDataFile = GetDataFileName(solverType, qrType);

            ILinearizedOdeSys odeSys = solverType == SolverType.Discrete ?
                new HenonMapLinearized() :
                new LorenzAttractorLinearized();

            IQrDecomposition ort = qrType == QrType.CGS ?
                new ClassicGrammSchmidt(odeSys.N) :
                new ModifiedGrammSchmidt(odeSys.N);

            LinearizedOdeSolverBase solver = solverType switch
            {
                SolverType.Discrete => new ChaosSoft.NumericalMethods.Ode.Linearized.DiscreteSolver(odeSys),
                SolverType.RK4 => new ChaosSoft.NumericalMethods.Ode.Linearized.RK4(odeSys, 0.01),
                SolverType.RK5 => new ChaosSoft.NumericalMethods.Ode.Linearized.RK5(odeSys, 0.01),
                _ => throw new NotImplementedException(),
            };

            if (solverType != SolverType.Discrete)
            {
                solver.SetInitialConditions(0, new double[] { 1, 1, 1 });
            }

            double[,] initialConditions = new double[odeSys.N, odeSys.N];

            for (int i = 0; i < odeSys.N; i++)
            {
                initialConditions[i, i] = 1;
            }

            solver.SetLinearInitialConditions(initialConditions);

            int totalColumns = (odeSys.N + 1) * odeSys.N;

            double[][] output = new double[totalColumns][];

            for (int i = 0; i < totalColumns; i++)
            {
                output[i] = new double[Steps];
            }

            double[] rMatrix = new double[odeSys.N];

            for (int k = 0; k < Steps; k++)
            {
                solver.NextStep();
                ort.Perform(solver.Linearization, rMatrix);

                int col = 0;

                for (int j = 0; j < odeSys.N; j++)
                {
                    output[col++][k] = solver.Solution[j];
                }

                for (int i = 0; i < odeSys.N; i++)
                {
                    for (int j = 0; j < odeSys.N; j++)
                    {
                        output[col++][k] = solver.Linearization[i, j];
                    }
                }
            }

            PerformCheck(expectedDataFile, totalColumns, output);
        }

        private void PerformCheck(string expectedDataFile, int columnsToCheck, double[][] output)
        {
            //string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string currentDir = "D:\\dev\\sources\\chaossoft\\numerical-methods\\src\\UnitTests";
            string path = Path.Combine(currentDir, "TestData", expectedDataFile);

            //File.WriteAllText(path, expectedDataFile), sb.ToString());
            SourceData sd = new SourceData(path);

            for (int i = 0; i < columnsToCheck; i++)
            {
                double[] expectedColumn = sd.GetColumn(i);
                double[] actualColumn = output[i];

                for (int k = 0; k < Steps; k++)
                {
                    if (Math.Abs(actualColumn[k] - expectedColumn[k]) > 1e-8)
                    {
                        Assert.Fail($"Column {i}, Row {k}: expected {expectedColumn[k]} but was {actualColumn[k]}");
                    }
                }
            }
        }

        private static string GetDataFileName(SolverType solver, QrType qr)
        {
            string systemName = solver == SolverType.Discrete ? "henon" : "lorenz";
            return $"{systemName}-{solver.ToString().ToLowerInvariant()}-{qr.ToString().ToLowerInvariant()}.dat";
        }
    }
}
