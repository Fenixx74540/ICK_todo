using System;
using System.Data;
using ICK_todo.DataAccess.Model;
using ServiceStack.OrmLite;

namespace ICK_todo.DataAccess
{

    //plik odpowiedizlny za tworzenie bazy danych i ustawienie m.in. kategorii
    internal class DbContext
    {
        private static IDbConnection _db;
        public static Exception Exception;

        public static IDbConnection GetInstance()
        {
            var dbFactory = new OrmLiteConnectionFactory(
                "Data Source=Database/todo.db;Version=3;",
                SqliteDialect.Provider);

            try
            {
                if (_db == null)
                {
                    _db = dbFactory.Open();
                    Migrate();
                }

                if (_db.State == ConnectionState.Broken || _db.State == ConnectionState.Closed)
                    _db = dbFactory.Open();

                return _db;
            }
            catch (Exception err)
            {
                Exception = err;
                return null;
            }
        }


        private static void Migrate()
        {
            var db = GetInstance();

            if(db.CreateTableIfNotExists<Category>())
            {
                db.Save(new Category() { CategoryName = "Prywatne" });
                db.Save(new Category() { CategoryName = "Zakupy" });
                db.Save(new Category() { CategoryName = "Praca" });
                db.Save(new Category() { CategoryName = "Studia" });
                db.Save(new Category() { CategoryName = "Nawyki" });
            }

            if (db.CreateTableIfNotExists<TodoItem>())
            {
                db.Save(new TodoItem()
                {
                    CategoryId = 1,
                    Description = "Testowe zadanie"
                });

                db.Save(new TodoItem()
                {
                    CategoryId = 2,
                    Description = "Testowe użycie kalendarza"
                });

                db.Save(new TodoItem()
                {
                    CategoryId = 1,
                    Description = "Grupowanie zadań przy pomocy kategorii"
                });
            }
        }
    }
}
