using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace trendingBot2
{
    /// <summary>
    /// Class storing all the method to trigger the main calculations (i.e., regression and subsequent analysis).
    /// As a consequence of the huge amount of information which has to be managed, even under the current simplistic
    /// conditions (the number of combinations might be relevantly extended), this new algorithm (unlikely the old one)
    /// has been built such that the intermediate storage of information is as low as possible. Thus, the whole calculation
    /// and subsequent analysis process is performed every time for each combination (in the Combinatorics class)
    /// </summary>
    public class MainCalcs
    {
        public static BackgroundWorker bgwMain; //Static instance of the BackgroundWorker performing all the calculations, which is called from different parts of the code
        public MainCalcs(BackgroundWorker temp)
        {
            bgwMain = temp;
        }

        //Method starting all the calculations which, as explained above, consists basically in a call to the corresponding method in the Combinatorics class
        public Results startCalcs(List<Input> inputCols, int indepIndex, FitConfig curFitConfig)
        {
            Combinatorics curCombinatorics = new Combinatorics();
            Results curResults = new Results();
            curResults.allInputs = inputCols;
            curResults.config = new Config();
            curResults.config.operations.Add(Operation.Addition);
            curResults.config.exponents = createListExponents();
            curResults.config.maxNoCombs = 20;
            curResults.config.fitConfig = curFitConfig;

            //Due to the huge amount of potential cases, the a-apriori better structured combinations -> calculations (regressions) -> analysis is not present.
            //Both calculations and analysis are perform for each single combination, such that only the "good-enough" ones are stored
           
            curResults = curCombinatorics.startCombinatorics(curResults, indepIndex);

            curResults.combinations = curResults.combinations.OrderByDescending(x => x.assessment.globalRating).ThenBy(x => x.averError).ThenBy(x => x.independentVar.input.displayedName).ThenBy(x => x.dependentVars.items.Count).ToList();

            return curResults;
        }

        //Method creating the list of exponents which will be considered during all the calculations.
        //Although this should never be a user input (against trendingBot ideas: taking care of everything internally),
        //the exact definition of this list (and, in any case, the number of its elements) should be one of the first things
        //to be extended while trying to improve this code
        private List<double> createListExponents()
        {
            List<double> outList = new List<double>();

            outList.Add(-2.0);
            outList.Add(-1.0);
            outList.Add(-0.5);
            outList.Add(0.5);
            outList.Add(1.0);
            outList.Add(2.0);
            outList.Add(-9999.10); //Log10 -> a mere example to show how easily a so different operation might be brought into the current framework

            return outList;
        }
    }

    /// <summary>
    /// Class storing all the information about the results of the calculations (e.g., list of fits and their corresponding assessment)
    /// </summary>
    public class Results
    {
        public List<Input> allInputs; //List of columns as read from the input file (+ some eventual modifications like, for example, conversion of categorical ones)
        public List<ValidCombination> combinations; //List of all the valid combinations found under the given input conditions
        public Config config; //Basic conditions under which the calculations are performed
        public Stopwatch sw;
        public string totTime;

        public Results()
        {
            config = new Config();
            allInputs = new List<Input>();
            combinations = new List<ValidCombination>();
        }
    }

    /// <summary>
    /// Class storing all the variables defining the basic conditions under which the calculations will be performed
    /// </summary>
    public class Config
    {
        public List<double> exponents; //List of exponents to be considered in the combinatorics part
        public List<Operation> operations;  //List of operations to be considered in the combinatorics part
        public int maxNoCombs; //Maximum number of valid combinations to be stored (by default: 20)
        public FitConfig fitConfig; //Basic conditions to be applied while performing the regression calculations
        public LimitValues limitVals; //List of threshold values

        public Config()
        {
            exponents = new List<double>();
            operations = new List<Operation>();
            operations.Add(Operation.Multiplication); //Multiplication has always to be present
            fitConfig = new FitConfig();
            limitVals = new LimitValues();
        }
    }

    /// <summary>
    /// Class storing a list of threshold values, which are considered while performing different comparisons
    /// </summary>
    public class LimitValues
    {
        public double valueIsZero;
        public double tooRelevantWeight, tooRelevantWeight2; //Variables setting the threshold from which the given weight (of the variable(s) out of the other ones in the combination) can be considered too important
        public double maxErrorConstant; //One of the weaknesses of the current version is not detecting small variations (and thus tending towards outputing constant fits); this limit tries to correct this problem.
        public double maxErrorToIgnoreVar; //While ignoring redundant variables, this is the error level below which both situations (before and after the removal) can be considered identical
        public SimilarityRatios similarity; //Ratios determining the similarity between two values
        public LimitValues()
        {
            valueIsZero = 0.00000001;
            tooRelevantWeight = 0.99;
            tooRelevantWeight2 = 0.90;
            maxErrorConstant = 0.000001;
            maxErrorToIgnoreVar = 0.001;
            similarity = new SimilarityRatios();
        }
    }

    /// <summary>
    /// Class storing different ratios to perform comparisons between values. Example: high similarity would imply a close-to-1 ratio.
    /// It has been included mainly to create a comprehensive code; but it is not used too much in the current version
    /// </summary>
    public class SimilarityRatios
    {
        public double high;
        public double medium;
        public double low;

        public SimilarityRatios()
        {
           high = 0.9999;
           medium = 0.999;
           low = 0.95;
        }
    }

    /// <summary>
    /// Class including all the variables required to define (suitable) fits within the input conditions (i.e., expected accuracy)
    /// </summary>
    public class FitConfig
    {
        public double averLimit; //Maximum error per case/row (when comparing the prediction against the real value). This variable is considered together with the next one
        public double minPercBelowLimit; //Mininum percentage of cases with an error <= averLimit. Both variables represent the most important error-related assessment of the fits
        public double globalAver; //Maximum average error for all the cases/rows
        public int minNoCases; //Minimum number of cases. Unlikely the variables above, this one is not considered while determining whether a valid trend exists or not; but while assessing its reliability
        public Accuracy expectedAccuracy; //As defined by the user
    }

    public enum Accuracy { High, Medium, Low }
}
