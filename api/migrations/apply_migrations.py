import psycopg2
from pathlib import Path

connection = psycopg2.connect(
    database="astep",
    user="postgres",
    password="postgres",
    host="localhost",
    port="5432",
)

cursor = connection.cursor()

migrations_dir = Path(__file__).resolve().parent / "files"

for file in migrations_dir.iterdir():
    print(f"Applying: {file.name}...")
    cursor.execute(file.read_text())

connection.commit()
print("All migrations successfully applied!")
connection.close()
