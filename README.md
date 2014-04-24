trendingBot 2.0
===============


VB.NET code -> https://github.com/varocarbas/trendingBot_2_VB<br>
Ready to use executable -> http://www.customsolvers.com/downloads/ccount/click.php?id=9

trendingBot
-----------

Trend-finding tool based on the two following ideas:
* Taking as much advantage as possible from the computational power. Consequently, its calculations are based on: detailed combinatorics and minimum number of simplifications.
* Reducing the intervention of the user to its minimum expression. In any case, note that trendingBot is target to experienced analysts; that is: users are not expected to participate in the calculations but in the proper interpretation of the output results.

The mathematical implementation of the aforementioned ideas is given by second degree polynomial fits, whose dependent variables are the result of a detailed combinatorics process.

`prediction = A + B*VAR + C*VAR^2`

Where:<br>
`VAR` -> fictitious variable created by a combination of the input variables raised to different exponents.<br>
`A`, `B` and `C` -> second degree polynomial coefficients calculated via regression on the training set.


After the fits have been created, a "decision algorithm" comes into picture to determine which ones deliver the best performance. 

For more information about trendingBot 1.0 visit: http://www.customsolvers.com/trendingbot.html


Improvements in version 2.0
---------------------------

* Much more user-oriented. Version 1.0 was a mere calculation engine with no GUI. 
* Can also find the best trends without specifying the variable to be predicted (`Variable to predict` -> `Find the best options`).
* Much less computationally expensive: both CPU and memory usage are notably lower.
* The sub-algorithm in charge of determining the most suitable trends is now more complex, extensible and reliable.
* The combinations are formed on account of a wider spectrum of operations (i.e., additions and logarithms). 


Program
-------

The GUI is intuitive and the code commented in detail. In any case, some clarifications might be required:

* It relies on CSV files for I/O (i.e., "inputs.csv" and "outputs.csv"). Regarding "inputs.csv": it can have as many columns (i.e., variables) and rows (i.e., cases) as required; the first row is for column names; all the rows have to have the same number of columns (i.e., commas); commas are escaped when included between quotes (e.g., `"col1,col2", col3` represents 2 columns).
* It accepts non-numerical inputs (i.e., categorical and date/time). The user will be prompted what to do for each non-numerical column.
* The user can input the expected accuracy level, that is: thresholds to be considered while analysing potential trends (e.g., high expected accuracy means that only fits delivering a low error are considered).
* The results are shown in both the GUI and the output file (i.e., "outputs.csv"). The GUI includes calculating functionalities for the user to test the output solutions.


Recommendations of use
----------------------

Some ideas to bear in mind:
* "No trend was found" has to be seen as a perfectly valid output. Test this program only with datasets including variables (i.e., columns) expected to have some kind of relationship.
* Even under the most permissive conditions (i.e., `Expected accuracy` -> `Low`), trendingBot looks only for reasonably solid trends (>= 65% accuracy).
* The outliers-detection and out-of-sample understanding capabilities of the current version are still not well developed (see "Further work" section). In any case, extrapolations should be avoided as much as possible.
* The weakest point of this approach is its computational expense/time requirements. This problem should be minimised with a proper definition of the training dataset, that is: as less variables/columns and cases/rows as possible. The table below shows how the size of the input dataset affects the time requirements.


** | No. of variables (columns) | 4 | 5 | 6 
----|---|:---:|:---:|:---:
No. of cases (rows) |  |  |  | 
30 | One variable to predict | < 1 minute | < 1 minute |  00:01:11
 | Find the best options | < 1 minute | < 1 minute | 00:09:31 
100 | One variable to predict | < 1 minute | < 1 minute | 00:03:34  
 | Find the best options | < 1 minute | < 1 minute  |  00:29:41
1000 | One variable to predict | < 1 minute | 00:02:29  | 00:34:57  
 | Find the best options | < 1 minute | 00:17:34 |  

NOTE: tested on a 7-core Intel Pentium @ 3.40GHz and 12GB RAM under normal load conditions.<br>
NOTE 2: an in-the-worst-scenario dataset was tested (i.e., many valid trends present).


Further work
------------

The current code can only be seen as a detailed descriptive of a different way to face multivariate problems. The main goal of this repository (the structure of the code and even the user-friendly GUI) is helping anyone interested to get a solid grasp about the potentiality of this approach. That is: this program represent a basic framework over which a definitive program might be built.

Expected improvements:
* In the current code, the main focus has been put on clarity (i.e., code structure and GUI); although this is acceptable under the current conditions, it does not address the main problem of this approach (i.e., being too computationally expensive). Thus the first issue to take care of is maximising the efficiency of the algorithm: firstly by making the most time-consuming part (i.e., combinatorics loops) as efficient as possible; and secondly by including some hardware-optimisation approaches (e.g., parallelisation).
* Even after performing the improvements in the previous point, this approach might not be fully maximised without the most adequate hardware. Since the start, it was assumed that trendingBot was meant to be run on clusters.
* Regarding the algorithm, the most important issues to be improved are:
   * Combinations to create the fictitious dependent variables: a much higher range of variations for exponents and logarithms; it might also be worthy to include further scenarios (e.g., trigonometric functions). Further operations between variables should also be included (e.g., substraction or division).
   * Type of fit to relate the dependent variables and the ones to be predicted: at least, 3rd degree polynomial and logarithmic fits should be added.
   * Definition of valid trends: in the current version, this analysis is mainly focused on errors (i.e., current prediction with respect to real value). A solid outliers-detection algorithm should be included; additionally, the analysis of the corresponding data (valid predictions vs. real values) should consider much more than loosely-grouped errors (e.g., detailed sampling sub-algorithm).
   * Trend rating: although the current structure (i.e., list of weighted assessment factors) does not need to be changed, the number and quality of the factors being considered should be improved notably. For example: there is no factor taking care of the outside-of-sample expected behaviour, what might be very helpful to track over-fitting (e.g., training set is 1,2,3 -> 4,5,6 and the given fit delivers a good performance; but when input 4 the result is -99; this issue is not being taken care of by the current version).

