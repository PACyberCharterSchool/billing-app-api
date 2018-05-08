﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Converters;
using Microsoft.EntityFrameworkCore.Storage.Internal;
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
					.HasAnnotation("ProductVersion", "2.1.0-preview1-28290")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

			modelBuilder.Entity("models.AuditRecord", b =>
					{
						b.Property<int>("Id")
											.ValueGeneratedOnAdd();

						b.Property<string>("Activity")
											.IsRequired();

						b.Property<DateTime>("Timestamp");

						b.Property<string>("Username")
											.IsRequired();

						b.HasKey("Id");

						b.ToTable("AuditRecords");
					});

			modelBuilder.Entity("models.CommittedStudentStatusRecord", b =>
					{
						b.Property<int>("Id")
											.ValueGeneratedOnAdd();

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

						b.Property<ulong?>("StudentPaSecuredId");

						b.Property<string>("StudentState");

						b.Property<string>("StudentStreet1");

						b.Property<string>("StudentStreet2");

						b.Property<DateTime?>("StudentWithdrawalDate");

						b.Property<string>("StudentZipCode");

						b.HasKey("Id");

						b.ToTable("CommittedStudentStatusRecords");
					});

			modelBuilder.Entity("models.PendingStudentStatusRecord", b =>
					{
						b.Property<int>("Id")
											.ValueGeneratedOnAdd();

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

						b.Property<ulong?>("StudentPaSecuredId");

						b.Property<string>("StudentState");

						b.Property<string>("StudentStreet1");

						b.Property<string>("StudentStreet2");

						b.Property<DateTime?>("StudentWithdrawalDate");

						b.Property<string>("StudentZipCode");

						b.HasKey("Id");

						b.ToTable("PendingStudentStatusRecords");
					});

			modelBuilder.Entity("models.SchoolDistrict", b =>
					{
						b.Property<int>("Id")
											.ValueGeneratedOnAdd();

						b.Property<decimal?>("AlternateRate");

						b.Property<int>("Aun");

						b.Property<DateTime>("Created");

						b.Property<DateTime>("LastUpdated");

						b.Property<string>("Name");

						b.Property<SchoolDistrictPaymentType>("PaymentType")
											.ValueGeneratedOnAdd()
											.HasDefaultValue("ACH")
											.HasConversion(new ValueConverter<SchoolDistrictPaymentType, string>(v => default(string), v => default(SchoolDistrictPaymentType)));

						b.Property<decimal>("Rate")
											.ValueGeneratedOnAdd()
											.HasDefaultValue(0m);

						b.HasKey("Id");

						b.HasIndex("Aun")
											.IsUnique();

						b.ToTable("SchoolDistricts");
					});

			modelBuilder.Entity("models.Student", b =>
					{
						b.Property<int>("Id")
											.ValueGeneratedOnAdd();

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

						b.Property<ulong?>("PASecuredId");

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
											.ValueGeneratedOnAdd();

						b.Property<StudentActivity>("Activity")
											.IsRequired()
											.HasConversion(new ValueConverter<StudentActivity, string>(v => default(string), v => default(StudentActivity)));

						b.Property<string>("BatchHash");

						b.Property<string>("NextData");

						b.Property<string>("PACyberId");

						b.Property<string>("PreviousData");

						b.Property<int>("Sequence");

						b.Property<DateTime>("Timestamp");

						b.HasKey("Id");

						b.ToTable("StudentActivityRecords");
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
