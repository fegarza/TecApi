#define PROD
//#undef PROD
//#define LOCAL
//#undef LOCAL

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;




namespace TecAPI.Models.Escolares
{
    public partial class EscolaresContext : DbContext
    {
        public EscolaresContext()
        {
        }

        public EscolaresContext(DbContextOptions<EscolaresContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actividades> Actividades { get; set; }
        public virtual DbSet<Creditos> Creditos { get; set; }
        public virtual DbSet<Grupos> Grupos { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Reportes> Reportes { get; set; }
        public virtual DbSet<Secciones> Secciones { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }

        // Unable to generate entity type for table 'dbo.AlumnoReporte'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
#if PROD
                 optionsBuilder.UseSqlServer("Server= 10.10.10.51; Database = Escolares; User ID=Complementarias;Password=Proyecto.2kl8;Trusted_Connection=False");
#else

#if LOCAL
                optionsBuilder.UseSqlServer("Server= localhost; Database = ESCOLARES; User ID=pipe;Password=7271;Trusted_Connection=True");
#else
                optionsBuilder.UseSqlServer("Server= 192.168.1.75; Database = ESCOLARES; User ID=pipe;Password=7271;Trusted_Connection=False");
#endif
#endif
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Actividades>(entity =>
            {
                entity.HasKey(e => e.ActividadId)
                    .HasName("PK__Activida__981483F0A317680F");

                entity.Property(e => e.ActividadId).HasColumnName("ActividadID");

                entity.Property(e => e.SeccionId).HasColumnName("SeccionID");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Seccion)
                    .WithMany(p => p.Actividades)
                    .HasForeignKey(d => d.SeccionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Actividad__Secci__41EDCAC5");
            });

            modelBuilder.Entity<Creditos>(entity =>
            {
                entity.HasKey(e => e.CreditoId)
                    .HasName("PK__Creditos__4FE406FDFB2E30A5");

                entity.Property(e => e.CreditoId).HasColumnName("CreditoID");

                entity.Property(e => e.ActividadId).HasColumnName("ActividadID");

                entity.Property(e => e.CarreraId).HasColumnName("CarreraID");

                entity.Property(e => e.EstadoDeLaActividad)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoDeLaFirma)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.FechaDeOficio).HasColumnType("date");

                entity.Property(e => e.GrupoId).HasColumnName("GrupoID");

                entity.Property(e => e.JefeId).HasColumnName("JefeID");

                entity.Property(e => e.NumeroDeControl)
                    .IsRequired()
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ResponsableId).HasColumnName("ResponsableID");

                entity.HasOne(d => d.Actividad)
                    .WithMany(p => p.Creditos)
                    .HasForeignKey(d => d.ActividadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Creditos__Activi__40F9A68C");
            });

            modelBuilder.Entity<Grupos>(entity =>
            {
                entity.HasKey(e => e.GrupoId)
                    .HasName("PK__Grupos__556BF060A78F7BFF");

                entity.Property(e => e.GrupoId).HasColumnName("GrupoID");

                entity.Property(e => e.ActividadId).HasColumnName("ActividadID");

                entity.Property(e => e.ResponsableId).HasColumnName("ResponsableID");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Actividad)
                    .WithMany(p => p.Grupos)
                    .HasForeignKey(d => d.ActividadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Grupos__Activida__42E1EEFE");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK__Logs__5E5499A8F567E6D7");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            });

            modelBuilder.Entity<Reportes>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CarreraId).HasColumnName("CarreraID");

                entity.Property(e => e.FechaDeOficio).HasColumnType("date");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Secciones>(entity =>
            {
                entity.HasKey(e => e.SeccionId)
                    .HasName("PK__Seccione__18B61621FF27B41F");

                entity.Property(e => e.SeccionId).HasColumnName("SeccionID");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(e => e.UsuarioId)
                    .HasName("PK__Usuarios__2B3DE7981BE48B82");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.Property(e => e.Apellidos)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.DepartamentoId).HasColumnName("DepartamentoID");

                entity.Property(e => e.Genero)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.PersonalId).HasColumnName("PersonalID");

                entity.Property(e => e.Rol)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });
        }
    }
}
