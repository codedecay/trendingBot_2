trendingBot 2.0
===============


VB.NET code -> https://github.com/varocarbas/trendingBot_2_VB

Ready to use executable -> http://www.customsolvers.com/downloads/ccount/click.php?id=9

trendingBot
-----------

Trend-finding tool based on the two following ideas:
* Taking as much advantage as possible from the computational power. Consequently, its calculations are based on: detailed combinatorics and minimum number of simplifications.
* Reducing the intervention of the user to its minimum expression. On the other hand, it should be noted that it is expected to be used by experienced analysts, who will apply their knowledge to maximse the output results.

Since the first version, the mathematical implementation has consisted in second degree polynomial fits, whose dependent variable is the result of a detailed combinatorics process. That is: 

`prediction = A + B*VAR + C*VAR^2`

Where:

`VAR` -> fictitious variable created by a combination of the input variables raised to different exponents.

`A`, `B` and `C` -> second degree polynomial coefficients calculated via regression on the training set.


After the fits have been created, a "decision algorithm" comes into picture to determine which ones deliver the most adequate solution. 

You can find more information about trendingBot 1.0 in: http://www.customsolvers.com/trendingbot.html


Improvements in version 2.0
---------------------------

* Version 1.0 was a mere calculation engine with no GUI. Version 2.0 is much more user-oriented.
* Much less computationally expensive: both CPU and memory usage are notably lower.
* The suitable-trend determination is more reliable now.
* Even though version 1.0 accounted for more combinations (i.e., higher number of exponential variations), version 2.0 considers a wider spectrum of situations (i.e., additions or logarithms); in any case, its algorithm can easily be extended to account for as many combinations as required.


Program
-------

The GUI is intuitive and the code commented in detail. In any case, there are some basic ideas which should be beard in mind:

* It relies on CSV for I/O (i.e., "inputs.csv" and "outputs.csv"). Regarding "inputs.csv": it can have as many columns and rows as required; the first row is for column names; all the rows have to have the same number of columns (i.e., commas); commas are escaped when included between quotes (e.g., `"col1,cold2", col3` represents 2 columns).
* It accepts non-numerical inputs (i.e., categorical and date/time). The user will be prompted about what to do for each non-numerical column.
* The user can input the expected accuracy level, that is: what thresholds should be considered while analysing potential trends (e.g., high expected accuracy means that only fits delivering a very low error would be considered).
* Additionally to the output file (i.e., "outputs.csv"), all the results are displayed in the GUI. There is also a calculation functionality allowing the user to test all the output solutions.


Recommendations of use
----------------------

TODO


Further work
------------

TODO
