﻿// <auto-generated />
using LiftDataManager.Core.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    [DbContext(typeof(ParameterContext))]
    partial class ParameterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.CargoType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CargoTypes", (string)null);
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

                    b.Property<int>("CargoTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DriveTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CargoTypeId");

                    b.HasIndex("DriveTypeId");

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

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.DriveType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DriveTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.CarFrameBaseType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CarFrameBaseTypes", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.CarFrameType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CarFrameBaseTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CarFrameWeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DriveTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasMachineRoom")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCFPControlled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CarFrameBaseTypeId");

                    b.HasIndex("DriveTypeId");

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

                    b.Property<int>("TypeExaminationCertificateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TypeExaminationCertificateId");

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

                    b.Property<int>("TypeExaminationCertificateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TypeExaminationCertificateId");

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

                    b.Property<int>("SafetyGearTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TypeExaminationCertificateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SafetyGearTypeId");

                    b.HasIndex("TypeExaminationCertificateId");

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
                        .HasMaxLength(20)
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

                    b.Property<int>("ParameterCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParameterTypId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParameterTypeCodeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParameterCategoryId");

                    b.HasIndex("ParameterTypId");

                    b.HasIndex("ParameterTypeCodeId");

                    b.ToTable("ParameterDtos", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterTyp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
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
                        .HasMaxLength(20)
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

                    b.HasKey("Id");

                    b.ToTable("TypeExaminationCertificates", (string)null);
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.LiftType", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.CargoType", "CargoType")
                        .WithMany("LiftTypes")
                        .HasForeignKey("CargoTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.DriveType", "DriveType")
                        .WithMany("LiftTypes")
                        .HasForeignKey("DriveTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CargoType");

                    b.Navigation("DriveType");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.CarFrameType", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.CarFrameBaseType", "CarFrameBaseType")
                        .WithMany("CarFrameTypes")
                        .HasForeignKey("CarFrameBaseTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.DriveType", "DriveType")
                        .WithMany("CarFrameTypes")
                        .HasForeignKey("DriveTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CarFrameBaseType");

                    b.Navigation("DriveType");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideModelType", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideType", "GuideType")
                        .WithMany("GuideModelTypes")
                        .HasForeignKey("GuideTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuideType");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.LiftPositionSystem", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.TypeExaminationCertificate", "TypeExaminationCertificate")
                        .WithMany("LiftPositionSystems")
                        .HasForeignKey("TypeExaminationCertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TypeExaminationCertificate");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.OverspeedGovernor", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.TypeExaminationCertificate", "TypeExaminationCertificate")
                        .WithMany("OverspeedGovernors")
                        .HasForeignKey("TypeExaminationCertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TypeExaminationCertificate");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.SafetyGearModelType", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.SafetyGearType", "SafetyGearType")
                        .WithMany("SafetyGearModelTypes")
                        .HasForeignKey("SafetyGearTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.TypeExaminationCertificate", "TypeExaminationCertificate")
                        .WithMany("SafetyGearModelTypes")
                        .HasForeignKey("TypeExaminationCertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SafetyGearType");

                    b.Navigation("TypeExaminationCertificate");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterDto", b =>
                {
                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.ParameterCategory", "ParameterCategory")
                        .WithMany("ParameterDtos")
                        .HasForeignKey("ParameterCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.ParameterTyp", "ParameterTyp")
                        .WithMany("ParameterDtos")
                        .HasForeignKey("ParameterTypId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LiftDataManager.Core.DataAccessLayer.Models.ParameterTypeCode", "ParameterTypeCode")
                        .WithMany("ParameterDtos")
                        .HasForeignKey("ParameterTypeCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParameterCategory");

                    b.Navigation("ParameterTyp");

                    b.Navigation("ParameterTypeCode");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten.CargoType", b =>
                {
                    b.Navigation("LiftTypes");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.DriveType", b =>
                {
                    b.Navigation("CarFrameTypes");

                    b.Navigation("LiftTypes");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.CarFrameBaseType", b =>
                {
                    b.Navigation("CarFrameTypes");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.GuideType", b =>
                {
                    b.Navigation("GuideModelTypes");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb.SafetyGearType", b =>
                {
                    b.Navigation("SafetyGearModelTypes");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterCategory", b =>
                {
                    b.Navigation("ParameterDtos");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterTyp", b =>
                {
                    b.Navigation("ParameterDtos");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.ParameterTypeCode", b =>
                {
                    b.Navigation("ParameterDtos");
                });

            modelBuilder.Entity("LiftDataManager.Core.DataAccessLayer.Models.TypeExaminationCertificate", b =>
                {
                    b.Navigation("LiftPositionSystems");

                    b.Navigation("OverspeedGovernors");

                    b.Navigation("SafetyGearModelTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
