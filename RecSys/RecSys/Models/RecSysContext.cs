using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RecSys.Models;

public partial class RecSysContext : DbContext
{
    public RecSysContext(DbContextOptions<RecSysContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseLecturer> CourseLecturers { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<PastStudentsChoice> PastStudentsChoices { get; set; }

    public virtual DbSet<RecHistory> RecHistories { get; set; }

    public virtual DbSet<RecRating> RecRatings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__2AA84FD12B199746");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId)
                  .ValueGeneratedOnAdd();
            entity.Property(e => e.Abbreviations)
                .HasMaxLength(255)
                .HasColumnName("abbreviations");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(50)
                .HasColumnName("courseCode");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("courseName");
        });

        modelBuilder.Entity<CourseLecturer>(entity =>
        {
            entity.HasKey(e => new { e.SvId, e.CourseId, e.Year, e.Sem }).HasName("PK__CourseLe__733F8E5E9FB58C9E");

            entity.ToTable("CourseLecturer");

            entity.Property(e => e.SvId).HasColumnName("svId");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.Sem).HasColumnName("sem");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseLecturers)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseLec__cours__70DDC3D8");

            entity.HasOne(d => d.Sv).WithMany(p => p.CourseLecturers)
                .HasForeignKey(d => d.SvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseLect__svId__6FE99F9F");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.SvId).HasName("PK__Lecturer__31A84D7B6411AC8F");

            entity.Property(e => e.SvId)
                  .ValueGeneratedOnAdd();
            entity.Property(e => e.Eligibility).HasColumnName("eligibility");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.SvExpertise)
                .HasMaxLength(255)
                .HasColumnName("svExpertise");
            entity.Property(e => e.SvName)
                .HasMaxLength(255)
                .HasColumnName("svName");
        });

        modelBuilder.Entity<PastStudentsChoice>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__past_stu__4D11D63C99A35C74");

            entity.ToTable("past_students_choices");

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("studentId");
            entity.Property(e => e.ProjectFocus)
                .HasMaxLength(255)
                .HasColumnName("project_focus");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.SvId).HasColumnName("svId");
        });

        modelBuilder.Entity<RecHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__RecHisto__4D7B4ABDCC5A894D");

            entity.ToTable("RecHistory");

            entity.Property(e => e.ProjectFocus).HasMaxLength(255);
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<RecRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__RecRatin__FCCDF87CBED54A06");

            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.History)
                .WithMany(p => p.RecRatings)
                .HasForeignKey(d => d.HistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecRating__Histo__05D8E0BE");

            entity.HasOne(d => d.RecommendedLecturer)
                .WithMany()
                .HasForeignKey(d => d.RecommendedLecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecRating_Lecturer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}