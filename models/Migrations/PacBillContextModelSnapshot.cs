﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using models;

namespace models.Migrations
{
    [DbContext(typeof(PacBillContext))]
    partial class PacBillContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rc1-32029")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("models.AuditRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Activity")
                        .IsRequired();

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("models.Calendar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("SchoolYear");

                    b.HasKey("Id");

                    b.HasIndex("SchoolYear")
                        .IsUnique()
                        .HasFilter("[SchoolYear] IS NOT NULL");

                    b.ToTable("Calendars");
                });

            modelBuilder.Entity("models.CalendarDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CalendarId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("DayOfWeek");

                    b.Property<byte>("Membership");

                    b.Property<byte>("SchoolDay");

                    b.HasKey("Id");

                    b.HasIndex("CalendarId");

                    b.ToTable("CalendarDays");
                });

            modelBuilder.Entity("models.CommittedStudentStatusRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActivitySchoolYear");

                    b.Property<string>("BatchFilename");

                    b.Property<string>("BatchHash");

                    b.Property<int>("BatchRow");

                    b.Property<DateTime>("BatchTime");

                    b.Property<DateTime>("CommitTime");

                    b.Property<int>("SchoolDistrictId");

                    b.Property<string>("SchoolDistrictName");

                    b.Property<string>("StudentCity");

                    b.Property<DateTime?>("StudentCurrentIep");

                    b.Property<DateTime>("StudentDateOfBirth");

                    b.Property<DateTime>("StudentEnrollmentDate");

                    b.Property<string>("StudentFirstName");

                    b.Property<DateTime?>("StudentFormerIep");

                    b.Property<string>("StudentGradeLevel");

                    b.Property<string>("StudentId");

                    b.Property<bool>("StudentIsSpecialEducation");

                    b.Property<string>("StudentLastName");

                    b.Property<string>("StudentMiddleInitial");

                    b.Property<DateTime?>("StudentNorep");

                    b.Property<decimal?>("StudentPaSecuredId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("StudentState");

                    b.Property<string>("StudentStreet1");

                    b.Property<string>("StudentStreet2");

                    b.Property<DateTime?>("StudentWithdrawalDate");

                    b.Property<string>("StudentZipCode");

                    b.HasKey("Id");

                    b.ToTable("CommittedStudentStatusRecords");
                });

            modelBuilder.Entity("models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Date");

                    b.Property<string>("ExternalId");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("PaymentId");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<string>("SchoolYear");

                    b.Property<int>("Split");

                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("Check");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.HasIndex("PaymentId", "Split")
                        .IsUnique()
                        .HasFilter("[PaymentId] IS NOT NULL");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("models.PendingStudentStatusRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActivitySchoolYear");

                    b.Property<string>("BatchFilename");

                    b.Property<string>("BatchHash");

                    b.Property<DateTime>("BatchTime");

                    b.Property<int>("SchoolDistrictId");

                    b.Property<string>("SchoolDistrictName");

                    b.Property<string>("StudentCity");

                    b.Property<DateTime?>("StudentCurrentIep");

                    b.Property<DateTime>("StudentDateOfBirth");

                    b.Property<DateTime>("StudentEnrollmentDate");

                    b.Property<string>("StudentFirstName");

                    b.Property<DateTime?>("StudentFormerIep");

                    b.Property<string>("StudentGradeLevel");

                    b.Property<string>("StudentId");

                    b.Property<bool>("StudentIsSpecialEducation");

                    b.Property<string>("StudentLastName");

                    b.Property<string>("StudentMiddleInitial");

                    b.Property<DateTime?>("StudentNorep");

                    b.Property<decimal?>("StudentPaSecuredId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("StudentState");

                    b.Property<string>("StudentStreet1");

                    b.Property<string>("StudentStreet2");

                    b.Property<DateTime?>("StudentWithdrawalDate");

                    b.Property<string>("StudentZipCode");

                    b.HasKey("Id");

                    b.ToTable("PendingStudentStatusRecords");
                });

            modelBuilder.Entity("models.Refund", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<string>("CheckNumber");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Date");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<string>("SchoolYear");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.ToTable("Refunds");
                });

            modelBuilder.Entity("models.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Data");

                    b.Property<string>("Name");

                    b.Property<string>("SchoolYear");

                    b.Property<int?>("TemplateId");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.HasIndex("TemplateId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("models.SchoolDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("AlternateRate");

                    b.Property<decimal?>("AlternateSpecialEducationRate");

                    b.Property<int>("Aun");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("PaymentType")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("ACH");

                    b.Property<decimal>("Rate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0m);

                    b.Property<decimal>("SpecialEducationRate");

                    b.HasKey("Id");

                    b.HasIndex("Aun")
                        .IsUnique();

                    b.ToTable("SchoolDistricts");
                });

            modelBuilder.Entity("models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime?>("CurrentIep");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("FirstName");

                    b.Property<DateTime?>("FormerIep");

                    b.Property<string>("Grade");

                    b.Property<bool>("IsSpecialEducation");

                    b.Property<string>("LastName");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("MiddleInitial");

                    b.Property<DateTime?>("NorepDate");

                    b.Property<string>("PACyberId");

                    b.Property<decimal?>("PASecuredId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("State");

                    b.Property<string>("Street1");

                    b.Property<string>("Street2");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("PACyberId")
                        .IsUnique()
                        .HasFilter("[PACyberId] IS NOT NULL");

                    b.HasIndex("SchoolDistrictId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("models.StudentActivityRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Activity")
                        .IsRequired();

                    b.Property<string>("BatchHash");

                    b.Property<string>("NextData");

                    b.Property<string>("PACyberId");

                    b.Property<string>("PreviousData");

                    b.Property<int>("Sequence");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");

                    b.ToTable("StudentActivityRecords");
                });

            modelBuilder.Entity("models.Template", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Content");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("ReportType");

                    b.Property<string>("SchoolYear");

                    b.HasKey("Id");

                    b.HasIndex("ReportType", "SchoolYear")
                        .IsUnique()
                        .HasFilter("[ReportType] IS NOT NULL AND [SchoolYear] IS NOT NULL");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("models.CalendarDay", b =>
                {
                    b.HasOne("models.Calendar", "Calendar")
                        .WithMany("Days")
                        .HasForeignKey("CalendarId");
                });

            modelBuilder.Entity("models.Payment", b =>
                {
                    b.HasOne("models.SchoolDistrict", "SchoolDistrict")
                        .WithMany()
                        .HasForeignKey("SchoolDistrictId");
                });

            modelBuilder.Entity("models.Refund", b =>
                {
                    b.HasOne("models.SchoolDistrict", "SchoolDistrict")
                        .WithMany()
                        .HasForeignKey("SchoolDistrictId");
                });

            modelBuilder.Entity("models.Report", b =>
                {
                    b.HasOne("models.Template", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId");
                });

            modelBuilder.Entity("models.Student", b =>
                {
                    b.HasOne("models.SchoolDistrict", "SchoolDistrict")
                        .WithMany("Students")
                        .HasForeignKey("SchoolDistrictId");
                });
#pragma warning restore 612, 618
        }
    }
}
