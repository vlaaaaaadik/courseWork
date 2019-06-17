using SQLite;

namespace Xplore
{
    class Database
    {
        public string sqliteFilename;
        public string libraryPath;
        public string path;
        public SQLiteConnection database;

        public Database()
        {
            sqliteFilename = "newdb.sqlite";
            libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            path = System.IO.Path.Combine(libraryPath, sqliteFilename);
            database = new SQLiteConnection(path);
        }
        public Database(string name)
        {
            sqliteFilename = name + ".sqlite";
            libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            path = System.IO.Path.Combine(libraryPath, sqliteFilename);
            database = new SQLiteConnection(path);
        }
    }

}