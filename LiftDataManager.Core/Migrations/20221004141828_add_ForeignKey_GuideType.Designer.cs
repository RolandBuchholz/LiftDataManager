﻿// <auto-generated />
using LiftDataManager.Core.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    [DbContext(typeof(ParameterContext))]
    [Migration("20221004141828_add_ForeignKey_GuideType")]
    partial class add_ForeignKey_GuideType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.BuildingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("BuildingTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.CENumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CENumbers", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.DeliveryType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DeliveryTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.ElevatorStandard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ElevatorStandards", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.FireClosure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FireClosures", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.FireClosureBy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FireClosureBys", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.GoodsLiftStandard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GoodsLiftStandards", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.InstallationInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("InstallationInfos", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.LiftType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CargoType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("DriveType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LiftTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.LoadingDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LoadingDevices", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.MachineRoomPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MachineRoomPositions", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.RailBracketFixing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RailBracketFixings", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.ShaftFrameFieldFilling", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ShaftFrameFieldFillings", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.ShaftType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ShaftTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.SmokeExtractionShaft", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SmokeExtractionShafts", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.VandalResistant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("VandalResistants", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.CarFrameType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CarFrameBaseType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<int>("CarFrameWeight")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DriveType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<bool>("HasMachineRoom")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCFPControlled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CarFrameTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.Coating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Coatings", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideModelType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuideTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuideTypeId");

                    b.ToTable("GuideModelTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideRails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("UsageAsCarRail")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UsageAsCwtRail")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("GuideRailss", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideRailsStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GuideRailsStatuss", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GuideTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.LiftPositionSystem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LiftPositionSystems", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.LoadWeighingDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LoadWeighingDevices", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.OverspeedGovernor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OverspeedGovernors", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.ReducedProtectionSpace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ReducedProtectionSpaces", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.SafetyGearModelType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SafetyGearModelTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.SafetyGearType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SafetyGearTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ParameterCategorys", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("DefaultUserEditable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("ParameterCategory")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ParameterTyp")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ParameterDtos", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterTyp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ParameterTyps", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterTypeCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ParameterTypeCodes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Schacht.WallMaterial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WallMaterials", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.TypeExaminationCertificate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CertificateNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("ManufacturerName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProductName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TypeExaminationCertificates", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideModelType", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideType", "GuideType")
                        .WithMany("GuideModelType")
                        .HasForeignKey("GuideTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuideType");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideType", b =>
                {
                    b.Navigation("GuideModelType");
                });
#pragma warning restore 612, 618
        }
    }
}
