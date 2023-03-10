// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MyWebApi.Migrations
{
    [DbContext(typeof(AppContext))]
    [Migration("20221221145713_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("Author", b =>
                {
                    b.Property<string>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RepoId")
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorId");

                    b.HasIndex("RepoId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Commit", b =>
                {
                    b.Property<string>("CommitId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("CommitId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Commits");
                });

            modelBuilder.Entity("Repo", b =>
                {
                    b.Property<string>("RepoId")
                        .HasColumnType("TEXT");

                    b.HasKey("RepoId");

                    b.ToTable("Repos");
                });

            modelBuilder.Entity("Author", b =>
                {
                    b.HasOne("Repo", null)
                        .WithMany("Authors")
                        .HasForeignKey("RepoId");
                });

            modelBuilder.Entity("Commit", b =>
                {
                    b.HasOne("Author", null)
                        .WithMany("Commits")
                        .HasForeignKey("AuthorId");
                });

            modelBuilder.Entity("Author", b =>
                {
                    b.Navigation("Commits");
                });

            modelBuilder.Entity("Repo", b =>
                {
                    b.Navigation("Authors");
                });
#pragma warning restore 612, 618
        }
    }
}
