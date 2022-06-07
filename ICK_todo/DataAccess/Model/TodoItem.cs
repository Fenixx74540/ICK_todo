using System;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace ICK_todo.DataAccess.Model
{
    [Alias("todo_list")]
    internal class TodoItem
    {
        [PrimaryKey]
        [AutoIncrement]
        public int id { get; set; }

        public string Description { get; set; } = string.Empty;

        [References(typeof(Category))]
        public int CategoryId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);
        
        public bool Done { get; set; } = false;

        public Category GetCategory()
            => DbContext.GetInstance().SingleById<Category>(this.CategoryId);

        //funkcja potrzeba do ustalenie jaki status ma zadanie
        public string GetStatus()
        {
            if(this.Done) return "Complete";
            if(this.EndDate >= DateTime.Now)
            {
                return "Pending";
            }
            else
            {
                return "Past due";
            }
        }
    }
}
