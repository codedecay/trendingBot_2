using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace trendingBot2
{
    /// <summary>
    /// Class containing all the I/O-related code, that is: reading from inputs.csv and writing to outputs.csv
    /// </summary>
    public class IO
    {
        //Method in charge or reading the CSV file with the input values
        public AllInputs readInputs()
        {
            AllInputs allInputs = new AllInputs();
            bool errorPresent = false;
            try
            {
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\inputs.csv"))
                {
                    string curLine = null;
                    int totCols = 0;
                    while ((curLine = sr.ReadLine()) != null)
                    {
                        curLine = curLine.Trim();

                        if (curLine.Length == 0 && totCols > 0) break;

                        string[] temp = colsInRow(curLine);
                        if ((totCols == 0 && temp.Length >= 2) || (totCols > 0 && temp.Length == totCols))
                        {
                            if (totCols == 0)
                            {
                                //Heading
                                allInputs = readHeading(temp, allInputs);
                                totCols = temp.Length;
                            }
                            else
                            {
                                //Input values line
                                allInputs = readLine(temp, allInputs);

                                if (allInputs.inputs.Count >= 0.75 * Int32.MaxValue)
                                {
                                    allInputs.maxRowConsidered = allInputs.inputs.Count; //Too many rows. Some of them will be ignored
                                    break;
                                }
                            }
                        }
                        else
                        {
                            errorPresent = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
                errorPresent = true;
            }

            if (errorPresent)
            {
                //Just one unaccounted error is enough to not perform the calculations at all
                allInputs = new AllInputs();
                MessageBox.Show("There was an error while reading \"inputs.csv\".");
            }

            return allInputs;
        }

        //Method extracting the columns in each row (each of the elements between commas). It accounts for CSV peculiarities (e.g., ignoring commas inside quotes)
        private string[] colsInRow(string fullRow)
        {
            string[] cols = null;

            if (fullRow.Contains("\"") || fullRow.Contains("'"))
            {
                cols = colsInRowWithQuotes(fullRow);
            }
            else
            {
                cols = fullRow.Split(',');
            }

            return cols;
        }

        //Method extracting the columns in the given row when the row contains quotes (CSV format ignores commas inside quotes; that is: no new column should be considered)
        //Example: in cases like col1, col2, "col31, col32",... "col31, col32" has to be considered part of the same column
        private string[] colsInRowWithQuotes(string fullRow)
        {
            bool openingQuote = false;
            bool openingQuote2 = false;
            bool commaInBetween = false;
            string textSoFar = "";

            string tempString = fullRow;
            
            //Loop iterating through all the elements in the row and replacing the commas to be ignored (i.e., ones in quotes) with a different string.
            //In this way, the Split function below will ignore them; the proper division in columns will be performed; and the substitution strings will be converted back to commas
            foreach (char c in tempString)
            {
                if (openingQuote || openingQuote2) textSoFar = textSoFar + c.ToString();

                if (c == '"')
                {
                    if (!openingQuote)
                    {
                        textSoFar = "\"";
                        openingQuote = true;
                    }
                    else
                    {
                        if (commaInBetween)
                        {
                            fullRow = fullRow.Replace(textSoFar, textSoFar.Replace(",", "commaCommacomma22"));
                        }
                        openingQuote = false;
                    }
                }
                else if (c == '\'')
                {
                    textSoFar = "'"; ;
                    openingQuote2 = true;
                }
                else if (c == ',')
                {
                    if (openingQuote || openingQuote2) commaInBetween = true;
                }
            }

            string[] cols = fullRow.Split(',');
            if (fullRow.Contains("commaCommacomma22"))
            {
                //Commas to be skipped were found
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].Contains("commaCommacomma22")) cols[i] = cols[i].Replace("commaCommacomma22", ",");
                }
            }

            return cols;
        }

        //Method extracting the values for all the columns in the first row (heading). The resulting number of columns will define all the subsequent rows. In case of finding a single row not having
        //this number of columns, the input file would be considered faulty and no calculations would be performed
        private AllInputs readHeading(string[] line, AllInputs colsSoFar)
        {
            for (int i = 0; i < line.Length; i++)
            {
                colsSoFar.inputs.Add(new Input { displayedIndex = i + 1, name = line[i].Trim(), displayedName = Common.getDisplayedName(i + 1, line[i].Trim(), false, false), displayedShortName = Common.getDisplayedName(i + 1, line[i].Trim(), true, false), vals = new List<double>(), type = new InputType() { mainType = MainTypes.Numerical} });
            }

            return colsSoFar;
        }

        //Method extracting the values for all the columns in any row after the first one (i.e., rows including input values)
        private AllInputs readLine(string[] line, AllInputs colsSoFar)
        {
            try
            {
                for (int i = 0; i < line.Length; i++)
                {

                    if (colsSoFar.inputs[i].type.mainType == MainTypes.Numerical)
                    {
                        double curVal;
                        if (double.TryParse(line[i], out curVal))
                        {
                            colsSoFar.inputs[i].vals.Add(curVal);
                        }
                        else
                        {
                            if (line[i] == null || line[i].Trim().Length == 0)
                            {
                                colsSoFar.inputs[i].type.mainType = MainTypes.Blank;
                            }
                            else
                            {
                                colsSoFar.inputs[i].type.mainType = MainTypes.NonNumerical;
                            }
                        }
                    }

                    colsSoFar.inputs[i].vals2.Add(line[i]);
                }
            }
            catch
            {
                colsSoFar = null;
            }

            return colsSoFar;
        }

        //Method called every time the calculations are completed (in any case: after finding valid trends or not) to write the results to the output file
        public void writeOutputs(Results curResults)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\outputs.csv"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("No.,");
                    sb.Append("Independent variable,");
                    sb.Append("Dependent variable,");
                    sb.Append("Polynomial coefficients (A|B|C),");
                    sb.Append("Reliability for the expected accuracy,");
                    sb.Append("In-sample average error (%),");
                    sb.Append("Total time,");
                    sb.Append("Expected accuracy,");
                    sb.Append("Output equation");

                    sw.WriteLine(sb.ToString());

                    //Loop iterating through all the valid solutions and printing out the required information for each of them
                    int count = 0;
                    foreach (ValidCombination comb in curResults.combinations)
                    {
                        count = count + 1;
                        sb = new StringBuilder();
                        sb.Append(count.ToString() + ",");
                        sb.Append("[" + comb.independentVar.input.name + "],");
                        sb.Append(createCombinationString(comb) + ",");
                        sb.Append(comb.coeffs.A.ToString(Common.curCulture) + "|" + comb.coeffs.B.ToString(Common.curCulture) + "|" + comb.coeffs.C.ToString(Common.curCulture) + ",");
                        sb.Append(comb.assessment.globalRating.ToString("N2", Common.curCulture) + ",");
                        sb.Append((100 * comb.averError).ToString("N2", Common.curCulture));

                        if (count == 1)
                        {
                            sb.Append("," + curResults.totTime + ",");
                            sb.Append(curResults.config.fitConfig.expectedAccuracy.ToString().ToUpper() + ",");
                            sb.Append("[independent]=A+B*[dependent]+C*[dependent]^2");
                        }

                        sw.WriteLine(sb.ToString());
                    }
                }
            }
            catch
            {
                MessageBox.Show("There was an error while writing the results to \"outputs.csv\"");
            }
        }

        //Function called by the one above to create the string including all the information from the given combination, for example: [var1] + [var2]^2
        private string createCombinationString(ValidCombination curCombination)
        {
            string outString = "";

            for (int i = 0; i < curCombination.dependentVars.items.Count; i++)
            {
                Variable curVar = curCombination.dependentVars.items[i].variable;
                double curExp = curCombination.dependentVars.items[i].exponent;
                Operation curOperation = curCombination.dependentVars.items[i].operation;
                string curString = "[" + curVar.input.name + "]";

                if (curExp != 1)
                {
                    double logVal = Common.isLogarithm(curExp);
                    bool isLog = false;
                    if (logVal != 0.0)
                    {
                        if (logVal == 10)
                        {
                            isLog = true;
                            curString = "Log10" + curString;
                        }
                    }

                    if (!isLog) curString = curString + "^" + curExp.ToString("N2", Common.curCulture);
                }

                if (i < curCombination.dependentVars.items.Count - 1) curString = curString + Common.getSignFromoperation(curOperation, true);

                outString = outString + curString;

                outString.Replace("+-", "-");
                outString.Replace("-+", "-");
                outString.Replace("--", "+");
                outString.Replace("++", "+");
            }

            return outString;
        }
    }

    /// <summary>
    /// Class storing all the input information to be considered during the calculations: the original inputs read from the file and also the required modifications (e.g., scale for categorical inputs)
    /// </summary>
    public class AllInputs
    {
        public List<Input> inputs; //All the inputs values separated by columns
        public bool updateCompleted; //Boolean flag helping to synchronise the BackgroundWorker used during the calculations with the user interventions when performing certain modifications (e.g., non-numerical columns)
        public int maxRowConsidered; //If this value is greater than zero, this would mean that not all the input rows are being considered (because of being too many) -> limitation quite unlikely to be reached

        public AllInputs()
        {
            inputs = new List<Input>();
        }
    }

    /// <summary>
    /// Each column in the input file (in its original version or after some relevant modifications have been applied to it; for example: scale for categorical inputs) is associated with
    /// one "Input" class. During the calculations, instances of the "Variable" class are being considered (they include the original "Input" information)
    /// </summary>
    public class Input
    {
        public int displayedIndex;
        public string name;
        public string displayedName;
        public string displayedShortName;
        public List<double> vals; //All the final values displayed to the user via DataGridView. They include modifications instructed by the user (e.g., categorical/time conversion)
        public List<string> vals2; //Temporary storage for non-numerical values, before they are converted into the corresponding type (e.g., categorical/time)
        public InputType type;
        public InputPreAnalysis preAnalysis; //Storing all the results from the pre-analysis performed for each column
        public Dictionary<string, double> conversionDict; //Required by non-numerical columns (i.e., categorical or date-time) to store the original-converted information
        public int wrongRowsCount;
        public double min;
        public double max;
        public int totDiffVals; //Variable storing the number of different values in the given column. This information is used while assessing its suitability

        public Input()
        {
            vals = new List<double>();
            vals2 = new List<string>();
            preAnalysis = new InputPreAnalysis();
            conversionDict = new Dictionary<string, double>();
            type = new InputType();
        }
    }

    /// <summary>
    /// Class storing the pre-analysis performed for each column right after creating it. This information is used in different stages while assessing the suitability of the given variable
    /// </summary>
    public class InputPreAnalysis
    {
        public double averValue;
        public double averVariabilityValue;
        public int commonValsCount;
        public int rating;
    }

    /// <summary>
    /// Class storing all the information regarding the type of the given column: numerical, non-numerical, etc.
    /// </summary>
    public class InputType
    {
        public MainTypes mainType;
        public DateTimeTypes secType;
    }

    public enum MainTypes { Numerical, Constant, NonNumerical, DateTime, Categorical, Blank }
    public enum DateTimeTypes { Time, Year, Month, Weekday, Day }
}
