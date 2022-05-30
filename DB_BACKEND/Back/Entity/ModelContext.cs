using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Back.Entity
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Instruct> Instructs { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Take> Takes { get; set; }
        public virtual DbSet<TrainingPlan> TrainingPlans { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("Data Source=47.103.94.33:1521/xe;User ID=cherry;Password=123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CHERRY");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.AdminId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("AdminID");

                entity.Property(e => e.Department).HasMaxLength(15);

                entity.Property(e => e.Name).HasMaxLength(10);

                entity.HasOne(d => d.AdminNavigation)
                    .WithOne(p => p.Admin)
                    .HasForeignKey<Admin>(d => d.AdminId)
                    .HasConstraintName("Admins_FK_User");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.Property(e => e.ApplicationId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("ApplicationID");

                entity.Property(e => e.AdminId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("AdminID");

                entity.Property(e => e.Reason)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.State).HasPrecision(1);

                entity.Property(e => e.Time).HasColumnType("DATE");

                entity.Property(e => e.Type).HasPrecision(1);

                entity.Property(e => e.UserId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("Applications_FK_Admins");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Applications_FK_Users");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("Attendance");

                entity.Property(e => e.AttendanceId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("AttendanceID");

                entity.Property(e => e.CourseId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("CourseID");

                entity.Property(e => e.EndTime).HasColumnType("DATE");

                entity.Property(e => e.IsEffective).HasPrecision(1);

                entity.Property(e => e.StartTime).HasColumnType("DATE");

                entity.Property(e => e.StudentId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("StudentID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Attendances)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("Attendance_FK_Courses");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Attendances)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("Attendance_FK_Students");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.CourseId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("CourseID");

                entity.Property(e => e.CourseName).HasMaxLength(20);

                entity.Property(e => e.Credit).HasPrecision(2);

                entity.Property(e => e.IsRequired).HasPrecision(1);

                entity.Property(e => e.MeetingId)
                    .HasPrecision(9)
                    .HasColumnName("MeetingID");

                entity.Property(e => e.Semester).HasMaxLength(10);

                entity.Property(e => e.TimeSlot).HasMaxLength(30);

                entity.Property(e => e.Year).HasMaxLength(4);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.Property(e => e.ExamId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("ExamID");

                entity.Property(e => e.CourseId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("CourseID");

                entity.Property(e => e.EndTime).HasColumnType("DATE");

                entity.Property(e => e.MeetingId)
                    .HasPrecision(9)
                    .HasColumnName("MeetingID");

                entity.Property(e => e.StartTime).HasColumnType("DATE");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("Exams_FK_Course");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasKey(e => new { e.ExamId, e.StudentId })
                    .HasName("Grade_PK");

                entity.ToTable("Grade");

                entity.Property(e => e.ExamId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("ExamID");

                entity.Property(e => e.StudentId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("StudentID");

                entity.Property(e => e.Grade_)
                    .HasPrecision(3)
                    .HasColumnName("Grade");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.Grades)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Grade_FK_Exam");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Grades)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Grade_FK_Student");
            });

            modelBuilder.Entity<Instruct>(entity =>
            {
                entity.HasKey(e => new { e.CourseId, e.InstructorId })
                    .HasName("Instructs_PK");

                entity.Property(e => e.CourseId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("CourseID");

                entity.Property(e => e.InstructorId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("InstructorID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Instructs)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Instructs_FK_Course");

                entity.HasOne(d => d.Instructor)
                    .WithMany(p => p.Instructs)
                    .HasForeignKey(d => d.InstructorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Instructs_FK_Instructor");
            });

            modelBuilder.Entity<Instructor>(entity =>
            {
                entity.Property(e => e.InstructorId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("InstructorID");

                entity.Property(e => e.Department).HasMaxLength(15);

                entity.Property(e => e.Name).HasMaxLength(10);

                entity.HasOne(d => d.InstructorNavigation)
                    .WithOne(p => p.Instructor)
                    .HasForeignKey<Instructor>(d => d.InstructorId)
                    .HasConstraintName("Instructors_FK_User");
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.Property(e => e.RecordId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("RecordID");

                entity.Property(e => e.CourseId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("CourseID");

                entity.Property(e => e.Time).HasColumnType("DATE");

                entity.Property(e => e.Url)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("Records_FK_Courses");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.StudentId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("StudentID");

                entity.Property(e => e.Credit).HasPrecision(3);

                entity.Property(e => e.Grade).HasPrecision(4);

                entity.Property(e => e.Major).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(10);

                entity.HasOne(d => d.StudentNavigation)
                    .WithOne(p => p.Student)
                    .HasForeignKey<Student>(d => d.StudentId)
                    .HasConstraintName("Students_FK_User");

                entity.HasOne(d => d.TrainingPlan)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => new { d.Major, d.Grade })
                    .HasConstraintName("Students_FK_Plan");
            });

            modelBuilder.Entity<Take>(entity =>
            {
                entity.HasKey(e => new { e.CourseId, e.StudentId })
                    .HasName("Takes_PK");

                entity.Property(e => e.CourseId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("CourseID");

                entity.Property(e => e.StudentId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("StudentID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Takes)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Takes_FK_Course");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Takes)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Takes_FK_Student");
            });

            modelBuilder.Entity<TrainingPlan>(entity =>
            {
                entity.HasKey(e => new { e.Major, e.Grade })
                    .HasName("TrainingPlan_PK");

                entity.ToTable("TrainingPlan");

                entity.Property(e => e.Major).HasMaxLength(20);

                entity.Property(e => e.Grade).HasPrecision(4);

                entity.Property(e => e.AdminId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("AdminID");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.TrainingPlans)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("TrainingPlan_FK_Admin");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnType("NUMBER(20)")
                    .HasColumnName("UserID");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.UserName).HasMaxLength(20);

                entity.Property(e => e.UserType).HasPrecision(1);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
