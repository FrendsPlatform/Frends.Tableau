namespace Frends.Tableau.CreateHyperFile.Tests;
using Frends.Tableau.CreateHyperFile.Definitions;
using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
internal class CreateHyperFileTests
{
    static Input input;
    static Options options;

    private readonly string Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles/Superstore.hyper"));

    [SetUp]
    public void SetUp()
    {
        input = new Input()
        {
            Path = Path,
            Json = @"[
                            {
                                ""Name"": ""John Doe"",
                                ""Age"": 30,
                                ""Salary"": 50000.50,
                                ""IsActive"": true,
                                ""JoiningDate"": ""2023-10-11"",
                                ""BinaryData"": ""VGhpcyBpcyBzb21lIGJpbmFyeSBkYXRh""
                            },
                            {
                                ""Name"": ""Jane Smith"",
                                ""Age"": 25,
                                ""Salary"": 45000.75,
                                ""IsActive"": false,
                                ""JoiningDate"": ""2024-01-01"",
                                ""BinaryData"": ""U29tZSBvdGhlciBiaW5hcnkgZGF0YQ==""
                            }
                         ]",
        };

        options = new Options()
        {
            ThrowException = true,
            CreateDirectory = true,
            LogFileMaxCount = "2",
            LogFileSizeLimit = "100M",
        };
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
            Directory.Delete(System.IO.Path.GetDirectoryName(Path), true);
    }

    [Test]
    public void CreateHyperFile_ValidInput_ShouldReturnSuccess()
    {
        var result = Tableau.CreateHyperFile(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.RecordsAffected);
        Assert.AreEqual("Success", result.Message);
    }

    [Test]
    public void CreateHyperFile_CreateDirectory()
    {
        var i = input;
        i.Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles/New/Superstore.hyper"));
        var result = Tableau.CreateHyperFile(i, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.RecordsAffected);
        Assert.AreEqual("Success", result.Message);
    }

    [Test]
    public void CreateHyperFile_LogFileSizeLimit()
    {
        var o = options;
        o.LogFileSizeLimit = "10M";
        var result = Tableau.CreateHyperFile(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.RecordsAffected);
        Assert.AreEqual("Success", result.Message);
    }

    [Test]
    public void CreateHyperFile_LogFileMaxCount()
    {
        var o = options;
        o.LogFileMaxCount = "1";
        var result = Tableau.CreateHyperFile(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.RecordsAffected);
        Assert.AreEqual("Success", result.Message);
    }
}
