namespace VisualMutator.Infrastructure
{
    using System;
    using UsefulTools.ExtensionMethods;

    public static class UserMessages
    {
       

public static string ErrorPretest_TestsFailed(string firstTestThatFailedFullName, string message)
{
    return @"One or more tests failed or were inconclusive on unmodified source assemblies. 
If you continue with session, its results will be inacurate.

First test that failed: {0}
Message: {1}

Do you want to continue mutation session?
"


        .Formatted(firstTestThatFailedFullName, message);


}
public static string ErrorPretest_UnknownError(string exceptionMessage)
{
    return 
@"Error occurred while pre-testing source assemblies. Mutation session cannot continue.

Exception:
{0}"
        .Formatted(exceptionMessage);


}

public static string ErrorPretest_VerificationFailure(string exceptionMessage)
{
    return
@"Some of source project assemblies failed verification.
Mutant verification will be disabled, also errors may occur during testing and while decompiling code for preview.

Details:

{0}"
        .Formatted(exceptionMessage);


}

public static string WarningAssemblyNotLoaded()
{
    return
        @"One or more assemblies could not be loaded. Ensure that projects are built and their assemblies exist on disk.";
}
public static string ErrorPretest_Cancelled()
{
    return
@"Testing was cancelled due to timeout.
Do you want to continue mutation session?";
}
public static string ErrorBadMethodSelected()
{
    return
        @"Method you selected to mutate is a test. You must select a non-test method.";
}

        public static string ErrorStrongNameSignedAssembly()
        {
            return
                    @"One of the assemblies is strong name signed. This will not allow to run any code in mutated assemblies. Please disable the assembly signing in project properties.";
        }

        public static string ErrorTestsLoading()
        {
            return
                    @"An error occurred while loading tests.";
        }
    }
}