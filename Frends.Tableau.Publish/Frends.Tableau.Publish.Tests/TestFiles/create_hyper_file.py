from tableauhyperapi import HyperProcess, Connection, Telemetry, TableDefinition, SqlType, Inserter, TableName, CreateMode
from datetime import datetime
import os

# Define the table schema
table_definition = TableDefinition(
    table_name=TableName("Extract", "Extract"),
    columns=[
        TableDefinition.Column("Order ID", SqlType.text()),
        TableDefinition.Column("Order Date", SqlType.date()),
        TableDefinition.Column("Sales", SqlType.double())
    ]
)

# Start the Hyper process
with HyperProcess(telemetry=Telemetry.SEND_USAGE_DATA_TO_TABLEAU) as hyper:
    # Create a new Hyper file
    with Connection(endpoint=hyper.endpoint, database="sample.hyper", create_mode=CreateMode.CREATE_AND_REPLACE) as connection:
        # Create the schema
        connection.catalog.create_schema("Extract")
        
        # Create the table
        connection.catalog.create_table(table_definition)
        
        # Insert data into the table
        with Inserter(connection, table_definition) as inserter:
            inserter.add_row(["CA-2016-152156", datetime.strptime("2016-11-08", "%Y-%m-%d").date(), 261.96])
            inserter.add_row(["CA-2016-152157", datetime.strptime("2016-11-08", "%Y-%m-%d").date(), 731.94])
            inserter.execute()

# Check if the file was created
if os.path.exists("sample.hyper"):
    print("Hyper file created successfully.")
else:
    print("Failed to create Hyper file.")