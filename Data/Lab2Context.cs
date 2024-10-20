using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lab2.Models;

namespace Lab2.Data
{
    public class Lab2Context : DbContext
    {
        public Lab2Context (DbContextOptions<Lab2Context> options)
            : base(options)
        {
        }

        public DbSet<Lab2.Models.Book> Book { get; set; } = default!;
        public DbSet<Lab2.Models.Publisher> Publisher { get; set; } = default!;
        public DbSet<Lab2.Models.Author> Author { get; set; } = default!;
    }
}
