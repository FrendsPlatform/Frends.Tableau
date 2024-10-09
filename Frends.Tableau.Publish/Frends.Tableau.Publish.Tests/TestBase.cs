namespace Frends.Tableau.Publish.Tests;

using Frends.Tableau.Publish.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

[TestFixture]
internal class TestBase
{
    protected static readonly string BaseUrl = "https://prod-uk-a.online.tableau.com/api/3.4/";
    protected static readonly string SiteName = Environment.GetEnvironmentVariable("TableauSiteName");
    protected static readonly string PatName = Environment.GetEnvironmentVariable("TableauPatName");
    protected static readonly string PatSecret = Environment.GetEnvironmentVariable("TableauPatSecret");
    protected static readonly string Username = Environment.GetEnvironmentVariable("TableauUsername");
    protected static readonly string Password = Environment.GetEnvironmentVariable("TableauPassword");
    protected static readonly string WorkbookFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestFiles\Superstore.twbx"));
    protected static readonly string FlowFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestFiles\Superstore Flow.tflx"));
    protected static readonly string DatasourceFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestFiles\\sample.hyper"));
    protected static readonly string XmlFileName = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"..\..\..\TestFiles\Publish-Testing_{Guid.NewGuid()}.xml"));

    protected static Connection basicConnection;
    protected static Connection patConnection;
    protected static Input input;
    protected static Options options;

    /*
    public static Input Input => new()
    {
        ProjectId = null,
        ProjectName = $"TaskProject-{Guid.NewGuid()}",
        ProjectDescription = $"This is description for TaskProject-{Guid.NewGuid()}",
        Files = new string[] { WorkbookFile },
        Xml = XmlFileName,
        CreateXml = true,
        Append = true,
        AsJob = true,
        FileType = "twbx",
        Overwrite = true,
        ResourceType = ResourceTypes.Workbooks,
        SingleRequest = true,
        ResourceName = $@"Publish-Testing_{Guid.NewGuid()}",
    };

    public static Connection PatConnection => new()
    {
        AuthenticationType = AuthenticationTypes.PAT,
        BaseUrl = BaseUrl,
        PatName = PatName,
        PatSecret = PatSecret,
        SiteName = SiteName,
    };

    public static Connection BasicConnection => new()
    {
        AuthenticationType = AuthenticationTypes.Basic,
        BaseUrl = BaseUrl,
        SiteName = SiteName,
        UserName = Username,
        Password = Password,
    };

    public static Options Options => new()
    {
        CreateProject = true,
    };
    */

    [SetUp]
    public void SetUp()
    {
        input = new Input
        {
            ProjectId = null,
            ProjectName = $"TaskProject-{Guid.NewGuid()}",
            ProjectDescription = $"This is description for TaskProject-{Guid.NewGuid()}",
            Files = new string[] { WorkbookFile },
            Xml = XmlFileName,
            CreateXml = true,
            Append = true,
            AsJob = true,
            FileType = "twbx",
            Overwrite = true,
            ResourceType = ResourceTypes.Workbooks,
            SingleRequest = true,
            ResourceName = $@"Publish-Testing_{Guid.NewGuid()}",
        };

        basicConnection = new Connection
        {
            AuthenticationType = AuthenticationTypes.Basic,
            BaseUrl = BaseUrl,
            SiteName = SiteName,
            UserName = Username,
            Password = Password,
        };

        patConnection = new Connection
        {
            AuthenticationType = AuthenticationTypes.PAT,
            BaseUrl = BaseUrl,
            PatName = PatName,
            PatSecret = PatSecret,
            SiteName = SiteName,
        };

        options = new Options
        {
            CreateProject = true,
        };
    }

    [OneTimeTearDown]
    public void CleanUp()
    {
        string[] files = Directory.GetFiles(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestFiles")), "Publish-Testing*");

        foreach (string file in files)
            File.Delete(file);
    }

    [TearDown]
    public void Waiting()
    {
        Thread.Sleep(1000);
    }
}