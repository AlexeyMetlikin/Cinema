using System;
using System.Data.Entity;

namespace CinemaServer.Models.Context
{
    public class CinemaDbInit : DropCreateDatabaseIfModelChanges<EFDbContext>
    {
        protected override void Seed(EFDbContext context)
        {
            context.Movies.Add(new Entities.Movie { Name = "Пираты Карибского моря 5", ShowTime = new TimeSpan(14, 0, 0) });
            context.Movies.Add(new Entities.Movie { Name = "Форсаж 8", ShowTime = new TimeSpan(18, 50, 0) });
            context.Movies.Add(new Entities.Movie { Name = "Список Шиндлера", ShowTime = new TimeSpan(19, 30, 0) });
            context.Movies.Add(new Entities.Movie { Name = "Зеленая миля", ShowTime = new TimeSpan(12, 10, 0) });
            context.Movies.Add(new Entities.Movie { Name = "Бойцовский клуб", ShowTime = new TimeSpan(13, 15, 0) });
            context.Movies.Add(new Entities.Movie { Name = "Форест Гамп", ShowTime = new TimeSpan(11, 0, 0) });
            base.Seed(context);
        }
    }
}