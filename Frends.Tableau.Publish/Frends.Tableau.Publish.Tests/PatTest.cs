namespace Frends.Tableau.Publish.Tests;

using Frends.Tableau.Publish.Definitions;
using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
internal class PatTest : TestBase
{
    [Test]
    public void Test_Publish_PatAuthentication()
    {
        var result = Tableau.Publish(patConnection, input, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_SiteMissing()
    {
        var con = patConnection;
        con.SiteName = null;
        var result = Tableau.Publish(con, input, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_Project_MissingName()
    {
        var newInput = input;
        newInput.ProjectName = null;
        Assert.Throws<ArgumentNullException>(() => Tableau.Publish(patConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_PatAuthentication_Project_MissingDescription()
    {
        var newInput = input;
        newInput.ProjectDescription = null;
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_FilesMissing_IsNull()
    {
        var newInput = input;
        newInput.Files = null;
        Assert.Throws<NullReferenceException>(() => Tableau.Publish(patConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_PatAuthentication_FilesMissing()
    {
        var newInput = input;
        newInput.Files = new string[] { @"c:\\temp\notexist" };
        Assert.Throws<FileNotFoundException>(() => Tableau.Publish(patConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_PatAuthentication_MissingXml()
    {
        var newInput = input;
        newInput.CreateXml = false;
        Assert.Throws<Exception>(() => Tableau.Publish(patConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_PatAuthentication_AppendFalse()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.DataSources;
        newInput.Files = new string[] { DatasourceFile };
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_AsJobFalse()
    {
        var newInput = input;
        newInput.AsJob = false;
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_FileTypeMissing()
    {
        var newInput = input;
        newInput.FileType = null;
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_OverwriteFalse()
    {
        var newInput = input;
        newInput.Overwrite = false;
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_ResourceTypeFlow()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.Flows;
        newInput.Files = new string[] { FlowFile };
        newInput.FileType = "tflx";
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_ResourceTypeFlow_SingleRequestFalse()
    {
        var newInput = input;
        newInput.SingleRequest = false;
        newInput.ResourceType = ResourceTypes.Flows;
        newInput.Files = new string[] { FlowFile };
        newInput.FileType = "tflx";
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("fileSize=\"1\""));
    }

    [Test]
    public void Test_Publish_PatAuthentication_ResourceTypeDataSource()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.DataSources;
        newInput.Files = new string[] { DatasourceFile };
        newInput.FileType = "tdsx";
        newInput.Overwrite = false; // Both append and Overwrite cannot be true

        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_ResourceTypeDataSource_SingleRequestFalse()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.DataSources;
        newInput.SingleRequest = false;
        newInput.Files = new string[] { DatasourceFile };
        newInput.FileType = "tdsx";
        newInput.Append = false; // Both append and Overwrite cannot be true

        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_SingleRequestFalse()
    {
        var newInput = input;
        newInput.SingleRequest = false;
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_WorkbookNameMissing()
    {
        var newInput = input;
        newInput.SingleRequest = false;
        newInput.Files = new string[] { WorkbookFile, WorkbookFile };
        var result = Tableau.Publish(patConnection, newInput, options, default);
        Assert.IsTrue(result.FileUploadResults[0].Success);
        Assert.IsTrue(result.FileUploadResults[0].StatusCode < 400);
        Assert.IsTrue(result.FileUploadResults[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_PatAuthentication_MissingPATName()
    {
        var con = patConnection;
        con.PatName = null;
        Assert.Throws<ArgumentException>(() => Tableau.Publish(con, input, options, default));
    }

    [Test]
    public void Test_Publish_PatAuthentication_MissingPatSecret()
    {
        var con = patConnection;
        con.PatSecret = null;
        Assert.Throws<ArgumentException>(() => Tableau.Publish(con, input, options, default));
    }
}