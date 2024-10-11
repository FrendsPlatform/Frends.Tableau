namespace Frends.Tableau.CreateHyperFile;

using Frends.Tableau.CreateHyperFile.Definitions;
using global::Tableau.HyperAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

/// <summary>
/// Tableau Task.
/// </summary>
public static class Tableau
{
    /// <summary>
    /// Frends Task for Tableau create hyper file operation.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Tableau.CreateHyperFile).
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Options parameters.</param>
    /// <param name="cancellationToken">Cancellation token given by Frends.</param>
    /// <returns>Object { bool Success, int RecordsAffected, string Message }</returns>
    public static Definitions.Result CreateHyperFile([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        int recordsAffected = 0;
        try
        {
            var jarray = JArray.Parse(input.Json);
            var dictionary = new Dictionary<string, string>()
            {
              { "log_file_max_count", options.LogFileMaxCount },
              { "log_file_size_limit", options.LogFileSizeLimit },
            };

            if (!Directory.Exists(Path.GetDirectoryName(input.Path)) && options.CreateDirectory)
                Directory.CreateDirectory(Path.GetDirectoryName(input.Path));

            using var hyperProcess = new HyperProcess(Telemetry.DoNotSendUsageDataToTableau, null, new Dictionary<string, string>());

            // Connection 1
            using var connection = new Connection(hyperProcess.Endpoint, input.Path, (CreateMode)3, (Dictionary<string, string>)null);
            var tableName = new TableName(new SchemaName("Extract"), new Name("Extract"));
            var columnList = new List<TableDefinition.Column>();

            foreach (JProperty jproperty in jarray[0].Cast<JProperty>())
            {
                SqlType sqlType = SqlType.Int();
                if (jproperty.Value.Type == JTokenType.Integer)
                    sqlType = SqlType.Int();
                if (jproperty.Value.Type == JTokenType.Float)
                    sqlType = SqlType.Double();
                if (jproperty.Value.Type == JTokenType.String)
                    sqlType = SqlType.Text();
                if (jproperty.Value.Type == JTokenType.Boolean)
                    sqlType = SqlType.Bool();
                if (jproperty.Value.Type == JTokenType.Date)
                    sqlType = SqlType.Text();
                if (jproperty.Value.Type == JTokenType.Date)
                    sqlType = SqlType.Date();
                if (jproperty.Value.Type == JTokenType.Bytes)
                    sqlType = SqlType.Bytes();
                columnList.Add(new TableDefinition.Column(jproperty.Name, sqlType, Nullability.Nullable));
            }

            var tableDefinition = new TableDefinition(tableName, columnList, Persistence.Permanent);
            connection.Catalog.CreateSchema(new SchemaName("Extract"));

            // Connection 2
            using var connection2 = new Connection(hyperProcess.Endpoint, input.Path, CreateMode.CreateIfNotExists);
            connection2.Catalog.CreateTable(tableDefinition);
            var objArrayList = new List<object[]>();

            foreach (JObject jobject in jarray.Cast<JObject>())
            {
                var objectList = new List<object>();
                foreach (JProperty jproperty in ((IEnumerable<JToken>)jobject).Cast<JProperty>())
                    objectList.Add(jproperty.Value.ToObject<object>());

                objArrayList.Add(objectList.ToArray());
            }

            IEnumerable<object[]> source = objArrayList;
            using var inserter = new Inserter(connection2, tableDefinition, (IEnumerable<string>)null);
            recordsAffected = source.Count<object[]>();

            foreach (object[] objArray in source)
                inserter.AddRow(objArray);

            inserter.Execute();
        }
        catch (HyperException ex)
        {
            if (options.ThrowException)
                throw;

            return new Definitions.Result(false, recordsAffected, ((Exception)ex).Message);
        }
        catch (Exception ex)
        {
            if (options.ThrowException)
                throw;

            return new Definitions.Result(false, recordsAffected, ex.Message);
        }

        return new Definitions.Result(true, recordsAffected, "Success");
    }
}