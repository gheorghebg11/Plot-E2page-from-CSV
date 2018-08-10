Rule in the .csv:
	- General 
		- the first line contains the header, see the specific columns for what is allowed in the header.
		- 
		
		
	- Header
		- it doesn't matter if there are capitals or spaces, as they are erased during parsing. Also, columns can be in any order.
		
		
	- Specific columns
		- Name
			- it contains the mathematical name of the element.
			- the header for the column for the name of an element is "name".
			- if it's a product of elements, there should be a space between each monomial.
			- the symbol "t" stands for the specific element \tau.
			- the name of each monomial should be of the form "symbol", "symbol index", "symbol ^ power" or "symbol index ^ power" where symbol represents one or more characters (which have to be letters, capital or not), index represents the subscript and should be a digit or a number, and power represents the power and should be a digit or a number. Recall that spaces do not matter. 
			- For example, all the following are acceptable "k", "h0", "h0^2", "P^2 h_1", "t k" which stands for \tau * k, wherease "tk" would stand for an element named tk that is not \tau divisible.
			- For example, the following is not a correct format "h0h1" since there should be a space separating them.
		- Label column
			- it contains the latex code for the label if you want it to be drawn.
			- the header for the column for the label of an element is "label".
		- Angle
			- it contains the angle to which the label will be drawn. It has to be a (signed) integer in degrees.
			- the header for the column is "angle".
			- the angle 0 corresponds to the point (1,0).
			- the angles 135 and - 225 are the same.
		- Topological stem
			- it contains the topological stem of the element which has to be a number.
			- the header for the column for the stem is either "stem" or "s".
		- Adams filtration
			- it contains the Adams filtration of the element which has to be a number.
			- the header for the column for the filtration is either "filtration", "filt" or "f".
		- Motivic weight
			- it contains the motivic weight of the element which has to be a number.
			- the header for the column for the weight is either "weight" or "w".
		- Tau Torsion
			- it contains the smallest power of tau, which annihilates it after multiplication.
			- it has to be a number, or can be empty. The number 0 or an empty cell means that it is not tau torsion TODO in code.			
			- the header for the column for the tau torsion is either "ttorsion", "tautorsion" or "taut".
		- Hidden Tau
			- it contains either the word "hid" or a positive integer 1,2,...
			- 
		- Extensions Target
			- it contains the target element after an extension by an element, i.e., a multiplication by an element.
			- it can either contain a name which has to follow the rules for a name (see the column Name for more details), or the word "loc". See Special rules below for the meaning of the word "loc".
			- the header for the columns for extensions have to either contain the word "target", "ext", or "extension". The leftover after erasing one of these three words (and extra spaces) should be the name of the element by which we multiply to create the extension.
			- no other column should contain any of these 3 words, in particular careful with the differential column which should not be called "drtarget" to avoid a clash.
		- Extensions Info
			- it contains information about the extension, which can be of two different nature. It can contain the letter "h" which means hidden extension, resulting in a dotted line. It can also contain an expression of the form "tx" where x is a number and has to come immediately after the letter t, meaning that the extension is tau^x torsion, resulting in changing the color of the line. It can also contain a mix of those, such as "h t3" or "t2h", where spaces are ignored.
			- the header has to contain the word "info". The leftover after erasing this word (and extra spaces) should be the name of the element by which we multiply to create the extension, and has to match a column for an extension target. It doesn't matter if the column for the extension info is before or after the column for the target, or even if they are next to each other.
			- if a cell extension info contains information but the associated cell extension target is empty, the information will be ignored.
		- DIFF COLUMN
		- SHIFT COLUMN
	- 

Special Rules:
	- if there is a column extension by h1 and the word "loc" appears, it is assumed that from there on the tower is all tau^1 torsion. 
	- no commas are allowed in any column, as that would mess with the format of the .csv and would add an extra cell. If it happens, the line is ignored.
	- by default, the label of an element is displayed if it is not the extension of an element, with the exception of 1 and the h_i's.
	
Drawing The Graph:
	- Parameters to set:
		-
	
	- Colors:
		-
	
	- Others:
		- The label is shown by default if the element is not an extension.
		- 