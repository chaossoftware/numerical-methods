using ChaosSoft.Core.Data;
using ChaosSoft.NumericalMethods.Equations;
using ChaosSoft.NumericalMethods.Orthogonalization;
using Unicorn.Taf.Core.Testing;
using Unicorn.Taf.Core.Testing.Attributes;
using Unicorn.Taf.Core.Verification;
using UnitTests.DataGenerators.OdeSys;

namespace UnitTests.Suites.Ode
{
    [Suite("ODE solvers tests")]
    public class OdeSolversTests : TestSuite
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

        public List<DataSet> SolutionConfigurations() =>
            new List<DataSet>
            {
                new DataSet("Discrete + CGS", SolverType.Discrete, QrType.CGS),
                new DataSet("RK4 + MGS", SolverType.RK4, QrType.MGS),
                new DataSet("RK5 + CGS", SolverType.RK5, QrType.CGS),
            };

        [Test("Discrete solver test")]
        [TestData(nameof(SolutionConfigurations))]
        public void QrAndSolverTest(SolverType solverType, QrType qrType)
        {
            SystemBase odeSys = solverType == SolverType.Discrete ? 
                new HenonMapLinearized() : 
                new LorenzAttractorLinearized();

            string systemName = solverType == SolverType.Discrete ? "henon" : "lorenz";

            string expectedDataFile = 
                $"{systemName}-{solverType.ToString().ToLowerInvariant()}-{qrType.ToString().ToLowerInvariant()}.dat";

            OrthogonalizationBase ort = qrType == QrType.CGS ? 
                new ClassicGrammSchmidt(odeSys.Count) :
                new ModifiedGrammSchmidt(odeSys.Count);

            SolverBase solver = solverType switch
            {
                SolverType.Discrete => new DiscreteSolver(odeSys),
                SolverType.RK4 => new RK4(odeSys, 0.01),
                SolverType.RK5 => new RK5(odeSys, 0.01),
                _ => throw new NotImplementedException(),
            };

            int totalColumns = odeSys.Rows * odeSys.Count;
            double[][] output = new double[totalColumns][];

            for (int i = 0; i < totalColumns; i++)
            {
                output[i] = new double[Steps];
            }

            //StringBuilder sb = new StringBuilder();
            double[] rMatrix = new double[odeSys.Count];

            for (int k = 0; k < Steps; k++)
            {
                solver.NexStep();
                ort.Perform(solver.Solution, rMatrix);

                int col = 0;

                for (int i = 0; i < odeSys.Rows; i++)
                {
                    for (int j = 0; j < odeSys.Count; j++)
                    {
                        output[col++][k] = solver.Solution[i, j];

                        //sb.AppendFormat("{0:0.##########}\t", solver.Solution[i, j]);
                    }
                }

                //sb.AppendLine();
            }

            //string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string currentDir = "D:\\dev\\sources\\chaossoft\\numerical-methods\\src\\UnitTests";
            string path = Path.Combine(currentDir, "TestData", expectedDataFile);

            //File.WriteAllText(path, expectedDataFile), sb.ToString());
            SourceData sd = new SourceData(path);

            for (int i = 0; i < totalColumns; i++)
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
    }
}
