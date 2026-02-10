import pyodbc
import sys

conn_str = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=156.67.104.130,1433;"
    "DATABASE=NtsDb;"
    "UID=goc_user;"
    "PWD=Z7!qM9#L@R2s$Wk9;"
    "TrustServerCertificate=yes;"
)

conn = pyodbc.connect(conn_str)
cursor = conn.cursor()

# Get query from command line argument, or use default
if len(sys.argv) > 1:
    query = sys.argv[1]
else:
    query = "SELECT * FROM Products"

# Execute the query
cursor.execute(query)

print("QUERY RESULTS:\n")
columns = [column[0] for column in cursor.description]
print(" | ".join(f"{col:<15}" for col in columns))
print("-" * (len(columns) * 18))

for row in cursor.fetchall():
    print(" | ".join(f"{str(val) if val else 'NULL':<15}" for val in row))

cursor.close()
conn.close()
