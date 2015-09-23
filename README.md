VisualMutator
-----------------------------
A mutation testing tool for Visual Studio IDE

LICENCE
-------

GNU GENERAL PUBLIC LICENSE

Institute of Computer Science, Warsaw University of Technology

Why Mutation testing?
-------------------
Tests will test the code, but what tests the test?
Many people uses code coverage to check that their test
covers any possible scenario and its quality. But it's not enough,
just because a test get to a line of code doesn't mean it is fully tested.
Mutation testing is for testing the tests, 
it will ensure that you tested any scenario in your code.

How mutation testing works & example
--------------------------

It will change a line of your tested code(not the tests themselves but what they test),
and if the test still passes it means you haven't tested something.
for example take a look at this function:

    int Multipy(int a, int b) 
    {
    	return a*b;
    }

said you wrote this test:

    Multipy(3, 1) == 3;

the tests will still pass and using only code coverage you might think that it covers all the tests,
but nope the tests aren't really covered.

mutation tests will create mutations of the code:

    int Multipy(int a, int b) 
    {
    	return a+b;
    }
    
and

    int Multipy(int a, int b) 
    {
    	return a-b;
    }
(and some more)
and since the test still passes although you changed the line of code,
it might mean that you haven't really tested anything.
So now you will fix your test to this:

    Assert.That(Multipy(3, 2) == 6);
   
   and all your mutations will be killed :D

Tutorial
--------
http://visualmutator.github.io/web/documents/VisualMutator%20-%20User%20Manual%20-%202.0.pdf
check the tutorial of how to use visualmutator.

Troubleshooting
----------------
You might get errors if you're using a different unit framework then NUnit,
so make sure you don't have any other frameworks in your code(even using).