using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Treinamento.Procorretor.Models;

namespace Treinamento.Procorretor.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
  public DbSet<VideoModel> Videos { get; set; }
}
