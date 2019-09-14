using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TecAPI.Models.Tutorias
{
    public partial class TUTORIASContext : DbContext
    {
        public TUTORIASContext()
        {
        }

        public TUTORIASContext(DbContextOptions<TUTORIASContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccionesTutoriales> AccionesTutoriales { get; set; }
        public virtual DbSet<Areas> Areas { get; set; }
        public virtual DbSet<Atenciones> Atenciones { get; set; }
        public virtual DbSet<Canalizaciones> Canalizaciones { get; set; }
        public virtual DbSet<Carreras> Carreras { get; set; }
        public virtual DbSet<Departamentos> Departamentos { get; set; }
        public virtual DbSet<Estudiantes> Estudiantes { get; set; }
        public virtual DbSet<EstudiantesDatos> EstudiantesDatos { get; set; }
        public virtual DbSet<EstudiantesSesiones> EstudiantesSesiones { get; set; }
        public virtual DbSet<Grupos> Grupos { get; set; }
        public virtual DbSet<Personales> Personales { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Sesiones> Sesiones { get; set; }
        public virtual DbSet<SesionesIndividuales> SesionesIndividuales { get; set; }
        public virtual DbSet<Titulos> Titulos { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server= 192.168.1.78,1433; Database = TUTORIAS; User ID=pipe;Password=72719902;Trusted_Connection=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AccionesTutoriales>(entity =>
            {
                entity.HasIndex(e => e.Fecha)
                    .HasName("UQ__Acciones__B30C8A5E32B4A91C")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.PersonalId).HasColumnName("PersonalID");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.HasOne(d => d.Personal)
                    .WithMany(p => p.AccionesTutoriales)
                    .HasForeignKey(d => d.PersonalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccionTutorial_Personal");
            });

            modelBuilder.Entity<Areas>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Correo)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Atenciones>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AreaId).HasColumnName("AreaID");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Atenciones)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Atencion_Area");
            });

            modelBuilder.Entity<Canalizaciones>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AtencionId).HasColumnName("AtencionID");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.PersonalId).HasColumnName("PersonalID");

                entity.HasOne(d => d.Atencion)
                    .WithMany(p => p.Canalizaciones)
                    .HasForeignKey(d => d.AtencionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Canalizacion_Atencion");

                entity.HasOne(d => d.Estudiante)
                    .WithMany(p => p.Canalizaciones)
                    .HasForeignKey(d => d.EstudianteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Canalizacion_Estudiante");

                entity.HasOne(d => d.Personal)
                    .WithMany(p => p.Canalizaciones)
                    .HasForeignKey(d => d.PersonalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Canalizacion_Personal");
            });

            modelBuilder.Entity<Carreras>(entity =>
            {
                entity.HasIndex(e => e.Carcve)
                    .HasName("UQ__Carreras__5501AA81808942ED")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(90)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Departamentos>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Estudiantes>(entity =>
            {
                entity.HasIndex(e => e.NumeroDeControl)
                    .HasName("UQ__Estudian__85382F59D8EF7B73")
                    .IsUnique();

                entity.HasIndex(e => e.UsuarioId)
                    .HasName("UQ__Estudian__2B3DE799449B119E")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CarreraId).HasColumnName("CarreraID");

                entity.Property(e => e.GrupoId).HasColumnName("GrupoID");

                entity.Property(e => e.NumeroDeControl)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Semestre).HasComputedColumnSql("(((1)+((CONVERT([int],datepart(year,getdate()))-CONVERT([int],concat('20',substring([NumeroDeControl],(1),(2)))))-(1))*(2))+case when CONVERT([int],datepart(month,getdate()))<(6) then (1) else (2) end)");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.HasOne(d => d.Carrera)
                    .WithMany(p => p.Estudiantes)
                    .HasForeignKey(d => d.CarreraId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Estudiante_Carrera");

                entity.HasOne(d => d.Grupo)
                    .WithMany(p => p.Estudiantes)
                    .HasForeignKey(d => d.GrupoId)
                    .HasConstraintName("FK_Estudiante_Grupo");

                entity.HasOne(d => d.Usuario)
                    .WithOne(p => p.Estudiantes)
                    .HasForeignKey<Estudiantes>(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Estudiante_Usuario");
            });

            modelBuilder.Entity<EstudiantesDatos>(entity =>
            {
                entity.HasIndex(e => e.EstudianteId)
                    .HasName("UQ__Estudian__6F768339276150DA")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BecadoPor)
                    .HasMaxLength(18)
                    .IsUnicode(false);

                entity.Property(e => e.CalleDomicilio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CependenciaEconomica)
                    .HasMaxLength(13)
                    .IsUnicode(false);

                entity.Property(e => e.CiudadNacimiento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoPostalDomicilio)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ColoniaDomicilio)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Empresa)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoCivil)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoNacimiento)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");

                entity.Property(e => e.EstudiosMadre)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.EstudiosPadre)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");

                entity.Property(e => e.Horario)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LugarTrabajoFamiliar)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LugarTrabajoMadre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LugarTrabajoPadre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nss)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.NumDomicilio)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ObservacionesHigiene)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PesadillasTerroresNocturnosAque)
                    .HasColumnName("PesadillasTerroresNocturnosAQue")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoDomicilio)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoMovil)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoTrabajoFamiliar)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoTrabajoMadre)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoTrabajoPadre)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Estudiante)
                    .WithOne(p => p.EstudiantesDatos)
                    .HasForeignKey<EstudiantesDatos>(d => d.EstudianteId)
                    .HasConstraintName("FK_EstudiantesDatos_Estudiante");
            });

            modelBuilder.Entity<EstudiantesSesiones>(entity =>
            {
                entity.HasKey(e => new { e.SesionId, e.EstudianteId, e.GrupoId })
                    .HasName("PK_Estudiantes_Sesiones");

                entity.Property(e => e.SesionId).HasColumnName("SesionID");

                entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");

                entity.Property(e => e.GrupoId).HasColumnName("GrupoID");

                entity.HasOne(d => d.Estudiante)
                    .WithMany(p => p.EstudiantesSesiones)
                    .HasForeignKey(d => d.EstudianteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EstudianteSesion_Estudiante");

                entity.HasOne(d => d.Grupo)
                    .WithMany(p => p.EstudiantesSesiones)
                    .HasForeignKey(d => d.GrupoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EstudianteSesion_Grupo");

                entity.HasOne(d => d.Sesion)
                    .WithMany(p => p.EstudiantesSesiones)
                    .HasForeignKey(d => d.SesionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EstudianteSesion_Sesion");
            });

            modelBuilder.Entity<Grupos>(entity =>
            {
                entity.HasIndex(e => e.PersonalId)
                    .HasName("UQ__Grupos__283437127E2CE395")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PersonalId).HasColumnName("PersonalID");

                entity.Property(e => e.Salon)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.HasOne(d => d.Personal)
                    .WithOne(p => p.Grupos)
                    .HasForeignKey<Grupos>(d => d.PersonalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_Grupo_Personal");
            });

            modelBuilder.Entity<Personales>(entity =>
            {
                entity.HasIndex(e => e.Cve)
                    .HasName("UQ__Personal__C1FE2DBEF2963B2D")
                    .IsUnique();

                entity.HasIndex(e => e.UsuarioId)
                    .HasName("UQ__Personal__2B3DE799FCBE890C")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cargo)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Cve)
                    .IsRequired()
                    .HasColumnName("CVE")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.DepartamentoId).HasColumnName("DepartamentoID");

                entity.Property(e => e.TituloId).HasColumnName("TituloID");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.HasOne(d => d.Departamento)
                    .WithMany(p => p.Personales)
                    .HasForeignKey(d => d.DepartamentoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Personal_Departamento");

                entity.HasOne(d => d.Titulo)
                    .WithMany(p => p.Personales)
                    .HasForeignKey(d => d.TituloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Personal_Titulo");

                entity.HasOne(d => d.Usuario)
                    .WithOne(p => p.Personales)
                    .HasForeignKey<Personales>(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Personal_Usuario");
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cargo)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Contenido).HasColumnType("text");

                entity.Property(e => e.PersonalId).HasColumnName("PersonalID");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.HasOne(d => d.Personal)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PersonalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_Personal");
            });

            modelBuilder.Entity<Sesiones>(entity =>
            {
                entity.HasIndex(e => new { e.AccionTutorialId, e.DepartamentoId })
                    .HasName("UQ_Sesiones_Departamento_AccionTutorial")
                    .IsUnique();

                entity.HasIndex(e => new { e.DepartamentoId, e.Fecha })
                    .HasName("UQ_Sesiones_Departamento_Fecha")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccionTutorialId).HasColumnName("AccionTutorialID");

                entity.Property(e => e.DepartamentoId).HasColumnName("DepartamentoID");

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.HasOne(d => d.AccionTutorial)
                    .WithMany(p => p.Sesiones)
                    .HasForeignKey(d => d.AccionTutorialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sesion_AccionTutorial");

                entity.HasOne(d => d.Departamento)
                    .WithMany(p => p.Sesiones)
                    .HasForeignKey(d => d.DepartamentoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sesion_Departamento");
            });

            modelBuilder.Entity<SesionesIndividuales>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");

                entity.Property(e => e.PersonalId).HasColumnName("PersonalID");

                entity.HasOne(d => d.Estudiante)
                    .WithMany(p => p.SesionesIndividuales)
                    .HasForeignKey(d => d.EstudianteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SesionIndividual_Estudiante");

                entity.HasOne(d => d.Personal)
                    .WithMany(p => p.SesionesIndividuales)
                    .HasForeignKey(d => d.PersonalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SesionIndividual_Personal");
            });

            modelBuilder.Entity<Titulos>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Usuarios__A9D105347AD0042C")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ApellidoMaterno)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ApellidoPaterno)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Genero)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(182)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(((([Nombre]+' ')+[ApellidoPaterno])+' ')+[ApellidoMaterno])");

                entity.Property(e => e.Publica)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });
        }
    }
}
