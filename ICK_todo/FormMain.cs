using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ICK_todo.DataAccess;
using ICK_todo.DataAccess.Model;
using ServiceStack.OrmLite;
using Kimtoo.BindingProvider;
using Bunifu.Utils;

namespace ICK_todo
{
    public partial class FormMain : Form
    {
        private DateTime _date = DateTime.Today;
        private string _category = "All";
        private string _status = "All";

        public FormMain()
        {
            InitializeComponent();

            calendar1.Value = DateTime.Today;
            ReloadCategories();

            var db = DbContext.GetInstance();
            grid.OnEdit<TodoItem>((r, c) => db.Save(r) || true);
            grid.OnDelete<TodoItem>((r, c) => db.Delete(r) >= 0);
            grid.OnError<TodoItem>((r, c) => bunifuSnackbar1.Show(this, DbContext.Exception.Message, Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error));
        }

        //funkcja do odświeżania danych zaciąganych z bazy danych
        private void ReloadCategories()
        {
            var db = DbContext.GetInstance();
            List<string> catItems = new List<string>();
            catItems.Add("All");

            foreach (var category in db.Select<Category>())
                catItems.Add(category.CategoryName);

            optCat.Items = catItems.ToArray();
        }

        //funkcja do odświeżania danych po wprowadzeniu zmian
        private void ReloadData()
        {
            var db = DbContext.GetInstance();

            var data = db.Select<TodoItem>();
            data = data.Where(r => this._date >= r.StartDate.Date && this._date <= r.EndDate.Date).ToList();

            if (this._category != "All")
                data = data.Where(r => r.CategoryId == Category.GetCategoryByName(this._category).id).ToList();

            if (this._status != "All")
                data = data.Where(r => r.Done == (this._status == "Complete")).ToList();

            grid.Bind(data);
        }

        //ustawienie stałej szerokości dla typu zadania, aby wygląd się nie zlewał ani nie nachodził na inne elementy
        private void FormMain_Shown(object sender, EventArgs e)
        {
            menu.Width = 308;
            ReloadData();
        }

        //ustawienie dzisiejszej daty w kalendarzu
        private void calendar1_ValueChanged(object sender, EventArgs e)
        {
            this._date = calendar1.Value;
            ReloadData();
        }

        //ustawienie wyboru kategorii zadania i ustawienie widoczności przycisku dodania zadania NIE widoczny w kategroii "All"
        private void optCat_OnSelectionChange(object sender, EventArgs e)
        {
            this._category = sender.ToString();
            btnAdd.Visible = this._category != "All";
            ReloadData();
        }


        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            ReloadData();
            grid.SearchRows(textSearch.Text.Trim());
        }

        //obsługa przycisku dodania zadania
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var db = DbContext.GetInstance();
            var newTask = new TodoItem()
            {
                CategoryId = Category.GetCategoryByName(this._category).id,
            };
            db.Save(newTask);
            grid.Bind(newTask, 1);
        }

        //obsługa przycisku usunięcia zadania
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow.Tag == null) return;
            grid.DeleteRow<TodoItem>(grid.CurrentRow);
        }

        private void menu_OnItemSelected(object sender, string path, EventArgs e)
        {
            this._status = path.ToString();
            ReloadData();
        }
    }
}
