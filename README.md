trendingBot 2.0
===============


VB.NET code -> https://github.com/varocarbas/trendingBot_2_VB<br>
Ready to use executable -> http://www.customsolvers.com/downloads/ccount/click.php?id=9

trendingBot
-----------

Trend-finding tool based on the two following ideas:
* Taking as much advantage as possible from the computational power. Consequently, its calculations are based on: detailed combinatorics and minimum number of simplifications.
* Reducing the intervention of the user to its minimum expression. On the other hand, note that it is expected to be used by experienced analysts, who will apply their knowledge to maximise the output results.

The mathematical implementation of the aforementioned ideas is given by second degree polynomial fits, whose dependent variable is the result of a detailed combinatorics process. That is: 

`prediction = A + B*VAR + C*VAR^2`

Where:<br>
`VAR` -> fictitious variable created by a combination of the input variables raised to different exponents.<br>
`A`, `B` and `C` -> second degree polynomial coefficients calculated via regression on the training set.


After the fits have been created, a "decision algorithm" comes into picture to determine which ones deliver the best performance. 

You can find more information about trendingBot 1.0 in: http://www.customsolvers.com/trendingbot.html


Improvements in version 2.0
---------------------------

* It is much more user-oriented. Version 1.0 was a mere calculation engine with no GUI. 
* It can also locate the best trends without specifying the variable to be predicted (`Variable to predict` -> `Find the best options`).
* Much less computationally expensive: both CPU and memory usage are notably lower.
* The sub-algorithm in charge of determining the most suitable trends is now more complex, extensible and reliable.
* Even though version 1.0 accounted for more combinations (i.e., higher number of exponential variations), version 2.0 considers a wider spectrum of situations (i.e., additions and logarithms). In any case, the new algorithm can easily be extended to account for as many combinations as required.


Program
-------

The GUI is intuitive and the code commented in detail. In any case, some clarifications might be required:

* It relies on CSV files for I/O (i.e., "inputs.csv" and "outputs.csv"). Regarding "inputs.csv": it can have as many columns (i.e., variables) and rows (i.e., cases) as required; the first row is for column names; all the rows have to have the same number of columns (i.e., commas); commas are escaped when included between quotes (e.g., `"col1,col2", col3` represents 2 columns).
* It accepts non-numerical inputs (i.e., categorical and date/time). The user will be prompted what to do for each non-numerical column.
* The user can input the expected accuracy level, that is: thresholds to be considered while analysing potential trends (e.g., high expected accuracy means that only fits delivering a very low error are considered).
* Additionally to the output file (i.e., "outputs.csv"), all the results are displayed in the GUI. It also includes a calculation functionality for the user to test the output solutions.


Recommendations of use
----------------------

Since the first moment, trendingBot was created as a helping tool for numerical modellers. Although it has become quite powerful (and its potential growth is virtually unlimited), this issue hasn't changed: it should only be used by experienced analysts.

Some ideas to bear in mind:
* "No trend was found" has to be seen as a perfectly valid output. Test this program only with datasets including variables (i.e., columns) expected to have some kind of relationship.
* Even under the most permissive conditions (i.e., `Expected accuracy` -> `Low`), trendingBot looks only for reasonably solid trends (>= 70% accuracy).
* The outliers-detection and out-of-sample understanding capabilities of the current version are still too limited (see "Further work" section). In any case, no extrapolation should be performed under any circumstance; what means that: no output solution should be tested with values outside the minimum/maximum for each variable given by the training set.
* The weakest point of this approach will always be the computational expense/time requirements. This problem should be minimised by accounting for a number of variables (i.e., columns) and cases (i.e., rows) as low as possible. Estimation of time requirements:

TODO


Further work
------------

TODO
