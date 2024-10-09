namespace Frends.Tableau.Publish.Tests;

using System;
using System.IO;
using Frends.Tableau.Publish.Definitions;
using NUnit.Framework;

[TestFixture]
internal class BasicTest : TestBase
{
    [Test]
    public void Test_Publish_BasicAuthentication()
    {
        var result = Tableau.Publish(basicConnection, input, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].File.Contains("twbx"));
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_MissingUsername()
    {
        var connection = basicConnection;
        connection.UserName = null;
        Assert.Throws<ArgumentException>(() => Tableau.Publish(connection, input, options, default));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_MissingPassword()
    {
        var connection = basicConnection;
        connection.UserName = null;
        Assert.Throws<ArgumentException>(() => Tableau.Publish(connection, input, options, default));
    }

    [Test]
    public void Test_Publish_SiteMissing()
    {
        var connection = basicConnection;
        connection.SiteName = null;
        var result = Tableau.Publish(connection, input, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].File.Contains("twbx"));
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_Project_MissingName()
    {
        var newInput = input;
        newInput.ProjectName = null;
        Assert.Throws<ArgumentNullException>(() => Tableau.Publish(basicConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_Project_MissingDescription()
    {
        var newInput = input;
        newInput.ProjectDescription = null;
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_FilesMissing_IsNull()
    {
        var newInput = input;
        newInput.Files = null;
        Assert.Throws<NullReferenceException>(() => Tableau.Publish(basicConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_FilesMissing()
    {
        var newInput = input;
        newInput.Files = new string[] { @"c:\\temp\notexist" };
        Assert.Throws<FileNotFoundException>(() => Tableau.Publish(basicConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_MissingXml()
    {
        var newInput = input;
        newInput.CreateXml = false;
        Assert.Throws<Exception>(() => Tableau.Publish(basicConnection, newInput, options, default));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_AppendFalse()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.DataSources;
        newInput.Files = new string[] { DatasourceFile };
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_AsJobFalse()
    {
        var newInput = input;
        newInput.AsJob = false;
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_FileTypeMissing()
    {
        var newInput = input;
        newInput.FileType = null;
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_OverwriteFalse()
    {
        var newInput = input;
        newInput.Overwrite = false;
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_ResourceTypeFlow()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.Flows;
        newInput.Files = new string[] { FlowFile };
        newInput.FileType = "tflx";
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_ResourceTypeFlow_SingleRequestFalse()
    {
        var newInput = input;
        newInput.SingleRequest = false;
        newInput.ResourceType = ResourceTypes.Flows;
        newInput.Files = new string[] { FlowFile };
        newInput.FileType = "tflx";
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("fileSize=\"1\""));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_ResourceTypeDataSource()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.DataSources;
        newInput.Files = new string[] { DatasourceFile };
        newInput.FileType = "tdsx";
        newInput.Overwrite = false; // Both append and Overwrite cannot be true

        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_ResourceTypeDataSource_SingleRequestFalse()
    {
        var newInput = input;
        newInput.ResourceType = ResourceTypes.DataSources;
        newInput.SingleRequest = false;
        newInput.Files = new string[] { DatasourceFile };
        newInput.FileType = "tdsx";
        newInput.Append = false; // Both append and Overwrite cannot be true

        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_SingleRequestFalse()
    {
        var newInput = input;
        newInput.SingleRequest = false;
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }

    [Test]
    public void Test_Publish_BasicAuthentication_WorkbookNameMissing()
    {
        var newInput = input;
        newInput.SingleRequest = false;
        newInput.Files = new string[] { WorkbookFile, WorkbookFile };
        var result = Tableau.Publish(basicConnection, newInput, options, default);
        Assert.IsTrue(result.Data[0].Success);
        Assert.IsTrue(result.Data[0].StatusCode < 400);
        Assert.IsTrue(result.Data[0].Message.Contains("createdAt"));
    }
}