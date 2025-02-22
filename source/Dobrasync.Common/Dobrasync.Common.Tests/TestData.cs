namespace Dobrasync.Common.Tests;

public static class TestData
{
    public static readonly string TestDataDirectoryPath = "Data";
    public static readonly string TestDataLargeTestfilePath = Path.Join(TestDataDirectoryPath, "LargeTestfile.txt");
    public static readonly string TestDataMediumTestfilePath = Path.Join(TestDataDirectoryPath, "MediumTestfile.txt");
    public static readonly string TestDataTestfilePath = Path.Join(TestDataDirectoryPath, "Testfile.txt");
}